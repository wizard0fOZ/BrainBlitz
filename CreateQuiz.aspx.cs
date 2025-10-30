using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class CreateQuiz : System.Web.UI.Page
    {
        // Class-level variable to store the validated teacher ID
        private int CurrentTeacherId = -1; // Initialize to invalid ID

        // --- State stored in Session to make it available during OnInit ---
        private int NumberOfQuestions
        {
            get
            {
                object o = Session["QuestionCount"];
                return (o == null) ? 1 : (int)o;
            }
            set { Session["QuestionCount"] = value; }
        }

        private Dictionary<int, QuestionData> SavedQuestions
        {
            get
            {
                if (Session["SavedQuestions"] == null)
                    Session["SavedQuestions"] = new Dictionary<int, QuestionData>();
                return (Dictionary<int, QuestionData>)Session["SavedQuestions"];
            }
            set { Session["SavedQuestions"] = value; }
        }

        private HashSet<int> DeletedQuestions
        {
            get
            {
                if (Session["DeletedQuestions"] == null)
                    Session["DeletedQuestions"] = new HashSet<int>();
                return (HashSet<int>)Session["DeletedQuestions"];
            }
            set { Session["DeletedQuestions"] = value; }
        }
        // --- End session-backed state ---

        // Recreate dynamic controls as early as possible so ViewState & post data bind correctly
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Ensure controls exist before LoadViewState/LoadPostData
            CreateQuestionControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Session Check ---
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            else
            {
                CurrentTeacherId = (int)Session["UserID"];
            }
            // --- End Session Check ---

            // Now the dynamic controls exist (recreated in OnInit).
            // Read posted values into our Session-backed SavedQuestions if this is a postback.
            if (IsPostBack)
            {
                SaveCurrentFormData();
            }

            if (!IsPostBack)
            {
                LoadSubjects();
            }
        }

        private void LoadSubjects()
        {
            int teacherId = CurrentTeacherId;
            if (teacherId <= 0) return;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT subjectID, name FROM Subjects WHERE assignedTo = @TeacherId ORDER BY name";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    try
                    {
                        con.Open();
                        sda.Fill(dt);
                        ddlSubject.DataSource = dt;
                        ddlSubject.DataTextField = "name";
                        ddlSubject.DataValueField = "subjectID";
                        ddlSubject.DataBind();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error loading subjects: " + ex.ToString());
                        lblErrorMessage.Text = "Error loading subjects. Please try again later.";
                        lblErrorMessage.Visible = true;
                    }
                }
            }
            ddlSubject.Items.Insert(0, new ListItem("-- Select Subject --", ""));
        }

        // Save posted form data from dynamic controls into Session-backed SavedQuestions
        private void SaveCurrentFormData()
        {
            var saved = SavedQuestions;
            var deleted = DeletedQuestions;

            for (int i = 1; i <= NumberOfQuestions; i++)
            {
                Panel questionCard = (Panel)phQuestions.FindControl("QuestionCard_" + i);
                if (questionCard == null) continue;

                int qNum = i;
                if (deleted.Contains(qNum)) continue;

                TextBox txtQuestion = (TextBox)questionCard.FindControl("txtQuestionText_" + qNum);
                TextBox txtPoints = (TextBox)questionCard.FindControl("txtPoints_" + qNum);

                if (txtQuestion == null || txtPoints == null) continue;

                QuestionData qData = new QuestionData
                {
                    QuestionText = txtQuestion.Text,
                    Options = new Dictionary<string, string>(),
                    Points = 10
                };

                if (int.TryParse(txtPoints.Text, out int pointsValue) && pointsValue > 0)
                {
                    qData.Points = pointsValue;
                }

                // Read radio button checked values (reliable because controls were created in OnInit)
                string[] optionLetters = { "A", "B", "C", "D" };
                string selected = null;
                foreach (var letter in optionLetters)
                {
                    RadioButton rb = (RadioButton)questionCard.FindControl($"rbOption_{qNum}_{letter}");
                    if (rb != null && rb.Checked)
                    {
                        selected = letter;
                        break;
                    }
                }
                qData.CorrectAnswer = selected ?? "A";

                // Read option texts
                foreach (string letter in optionLetters)
                {
                    TextBox txtOpt = (TextBox)questionCard.FindControl($"txtOption_{qNum}_{letter}");
                    if (txtOpt != null)
                        qData.Options[letter] = txtOpt.Text;
                    else
                        qData.Options[letter] = string.Empty;
                }

                saved[qNum] = qData;
            }

            SavedQuestions = saved;
        }

        // Recreate all question controls based on current NumberOfQuestions & DeletedQuestions
        private void CreateQuestionControls()
        {
            phQuestions.Controls.Clear();
            var deleted = DeletedQuestions;

            for (int i = 1; i <= NumberOfQuestions; i++)
            {
                if (!deleted.Contains(i))
                {
                    AddSingleQuestionControlSet(i);
                }
            }
        }

        // Create controls for a single question number
        private void AddSingleQuestionControlSet(int questionNumber)
        {
            Panel questionCard = new Panel { CssClass = "form-card question-card" };
            questionCard.ID = "QuestionCard_" + questionNumber;

            Panel titleRow = new Panel { CssClass = "question-title-row" };

            // compute human-friendly sequential display index (1-based) based on current non-deleted order
            int displayIndex = questionNumber;
            try
            {
                var active = new List<int>();
                for (int i = 1; i <= NumberOfQuestions; i++)
                {
                    if (!DeletedQuestions.Contains(i)) active.Add(i);
                }

                int idx = active.IndexOf(questionNumber);
                displayIndex = (idx >= 0) ? idx + 1 : questionNumber;
                if (displayIndex <= 0) displayIndex = 1;
            }
            catch
            {
                displayIndex = questionNumber;
            }

            titleRow.Controls.Add(new Literal { Text = $"<h2 class=\"card-title\">Question {displayIndex}</h2>" });


            int activeQuestionCount = NumberOfQuestions - DeletedQuestions.Count;
            if (activeQuestionCount > 1)
            {
                LinkButton btnDelete = new LinkButton
                {
                    ID = "btnDeleteQuestion_" + questionNumber,
                    CssClass = "delete-question-button",
                    Text = "<i class=\"fas fa-trash-alt\"></i> Delete",
                    CommandName = "DeleteQuestion",
                    CommandArgument = questionNumber.ToString(),
                    CausesValidation = false
                };
                btnDelete.Click += new EventHandler(DeleteQuestion_Click);
                titleRow.Controls.Add(btnDelete);
            }
            questionCard.Controls.Add(titleRow);

            Panel questionRow = new Panel { CssClass = "form-row" };

            Panel textGroup = new Panel { CssClass = "form-group" };
            textGroup.Style.Add("flex", "5");
            Label lblText = new Label { AssociatedControlID = "txtQuestionText_" + questionNumber, CssClass = "form-label", Text = "Question Text" };
            TextBox txtText = new TextBox { ID = "txtQuestionText_" + questionNumber, CssClass = "textarea-field", TextMode = TextBoxMode.MultiLine };
            txtText.Attributes.Add("placeholder", "Enter your question here...");
            textGroup.Controls.Add(lblText);
            textGroup.Controls.Add(txtText);
            questionRow.Controls.Add(textGroup);

            Panel pointsGroup = new Panel { CssClass = "form-group" };
            pointsGroup.Style.Add("flex", "1");
            Label lblPoints = new Label { AssociatedControlID = "txtPoints_" + questionNumber, CssClass = "form-label", Text = "Points" };
            TextBox txtPoints = new TextBox { ID = "txtPoints_" + questionNumber, CssClass = "input-field", TextMode = TextBoxMode.Number, Text = "10" };
            RangeValidator rvPoints = new RangeValidator
            {
                ControlToValidate = txtPoints.ID,
                MinimumValue = "1",
                MaximumValue = "100",
                Type = ValidationDataType.Integer,
                ErrorMessage = "Points must be between 1-100",
                ForeColor = System.Drawing.Color.Red,
                Display = ValidatorDisplay.Dynamic
            };
            pointsGroup.Controls.Add(lblPoints);
            pointsGroup.Controls.Add(txtPoints);
            pointsGroup.Controls.Add(rvPoints);
            questionRow.Controls.Add(pointsGroup);
            questionCard.Controls.Add(questionRow);

            Panel optionsGroup = new Panel { CssClass = "form-group" };
            optionsGroup.Controls.Add(new Literal { Text = "<span class=\"form-label\">Answer Options</span>" });
            Panel optionsListDiv = new Panel { CssClass = "options-list" };
            string[] optionLetters = { "A", "B", "C", "D" };
            string radioGroupName = "CorrectAnswerGroup_Q" + questionNumber;

            for (int j = 0; j < optionLetters.Length; j++)
            {
                string letter = optionLetters[j];
                Panel optionRow = new Panel { CssClass = "option-row" };
                RadioButton rb = new RadioButton { ID = $"rbOption_{questionNumber}_{letter}", GroupName = radioGroupName };
                rb.Attributes["value"] = letter;
                TextBox txtOpt = new TextBox { ID = $"txtOption_{questionNumber}_{letter}", CssClass = "input-field" };
                txtOpt.Attributes.Add("placeholder", $"Option {letter}");
                optionRow.Controls.Add(rb);
                optionRow.Controls.Add(txtOpt);
                optionsListDiv.Controls.Add(optionRow);
            }
            optionsGroup.Controls.Add(optionsListDiv);
            optionsGroup.Controls.Add(new Literal { Text = "<span class=\"helper-text\">Select the radio button for the correct answer</span>" });
            questionCard.Controls.Add(optionsGroup);

            // Restore saved data (if any)
            var saved = SavedQuestions;
            if (saved.ContainsKey(questionNumber))
            {
                QuestionData qData = saved[questionNumber];
                txtText.Text = qData.QuestionText;
                txtPoints.Text = qData.Points.ToString();

                foreach (string letter in optionLetters)
                {
                    TextBox txtOpt = (TextBox)questionCard.FindControl($"txtOption_{questionNumber}_{letter}");
                    RadioButton rb = (RadioButton)questionCard.FindControl($"rbOption_{questionNumber}_{letter}");

                    if (txtOpt != null && qData.Options.ContainsKey(letter))
                        txtOpt.Text = qData.Options[letter];

                    if (rb != null)
                        rb.Checked = (letter == qData.CorrectAnswer);
                }
            }
            else
            {
                txtPoints.Text = "10";
                RadioButton firstRb = (RadioButton)questionCard.FindControl($"rbOption_{questionNumber}_A");
                if (firstRb != null) firstRb.Checked = true;
            }

            phQuestions.Controls.Add(questionCard);
        }

        protected void DeleteQuestion_Click(object sender, EventArgs e)
        {
            LinkButton btnDelete = (LinkButton)sender;
            if (!int.TryParse(btnDelete.CommandArgument, out int questionNumberToDelete)) return;

            var deleted = DeletedQuestions;
            deleted.Add(questionNumberToDelete);
            DeletedQuestions = deleted;

            var saved = SavedQuestions;
            saved.Remove(questionNumberToDelete);
            SavedQuestions = saved;

            System.Diagnostics.Debug.WriteLine($"Marked Question {questionNumberToDelete} for deletion.");

            // Recreate controls for this same request so UI updates immediately
            CreateQuestionControls();
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            // Increase count and create the new question control immediately
            NumberOfQuestions++;
            AddSingleQuestionControlSet(NumberOfQuestions);

            System.Diagnostics.Debug.WriteLine($"Add Question clicked. Question count is now: {NumberOfQuestions}");
        }

        protected void btnCreateQuizSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                lblErrorMessage.Text = "Please fix the validation errors.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Ensure latest data captured
            SaveCurrentFormData();

            var saved = SavedQuestions;
            var deleted = DeletedQuestions;
            int validQuestionCount = saved.Count(kvp => !deleted.Contains(kvp.Key) &&
                                                        !string.IsNullOrWhiteSpace(kvp.Value.QuestionText) &&
                                                        kvp.Value.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt)));

            if (validQuestionCount == 0)
            {
                lblErrorMessage.Text = "Please add at least one complete question with answer options.";
                lblErrorMessage.Visible = true;
                return;
            }

            lblErrorMessage.Visible = false;

            string quizTitle = txtQuizTitle.Text;
            if (!int.TryParse(ddlSubject.SelectedValue, out int subjectId) || subjectId <= 0)
            {
                lblErrorMessage.Text = "Please select a valid subject.";
                lblErrorMessage.Visible = true;
                return;
            }
            string difficulty = ddlDifficulty.SelectedValue;
            int teacherId = CurrentTeacherId;
            if (teacherId <= 0)
            {
                lblErrorMessage.Text = "Session error. Please log in again.";
                lblErrorMessage.Visible = true;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            bool savedSuccessfully = false;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Save Quiz Header
                        string insertQuizQuery = @"INSERT INTO Quizzes (userID, subjectID, title, isPublished, difficulty)
                                                   OUTPUT INSERTED.quizID
                                                   VALUES (@UserID, @SubjectID, @Title, @IsPublished, @Difficulty)";
                        int newQuizId;
                        using (SqlCommand cmdQuiz = new SqlCommand(insertQuizQuery, con, transaction))
                        {
                            cmdQuiz.Parameters.AddWithValue("@UserID", teacherId);
                            cmdQuiz.Parameters.AddWithValue("@SubjectID", subjectId);
                            cmdQuiz.Parameters.AddWithValue("@Title", quizTitle);
                            cmdQuiz.Parameters.AddWithValue("@IsPublished", 1);
                            cmdQuiz.Parameters.AddWithValue("@Difficulty", difficulty);
                            newQuizId = (int)cmdQuiz.ExecuteScalar();
                        }
                        if (newQuizId <= 0) throw new Exception("Failed to create quiz header.");

                        // 2. Save Questions + Options
                        foreach (var kvp in saved)
                        {
                            int questionNumber = kvp.Key;
                            QuestionData qData = kvp.Value;

                            if (deleted.Contains(questionNumber) ||
                                string.IsNullOrWhiteSpace(qData.QuestionText) ||
                                !qData.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt)))
                            {
                                continue;
                            }

                            int newQuestionId;
                            string insertQuestionQuery = @"INSERT INTO Questions (quizID, text, points)
                                                           OUTPUT INSERTED.questionID
                                                           VALUES (@QuizID, @Text, @Points)";
                            using (SqlCommand cmdQuestion = new SqlCommand(insertQuestionQuery, con, transaction))
                            {
                                cmdQuestion.Parameters.AddWithValue("@QuizID", newQuizId);
                                cmdQuestion.Parameters.AddWithValue("@Text", qData.QuestionText);
                                cmdQuestion.Parameters.AddWithValue("@Points", qData.Points);
                                newQuestionId = (int)cmdQuestion.ExecuteScalar();
                            }
                            if (newQuestionId <= 0) throw new Exception($"Failed to create question number {questionNumber}.");

                            string insertOptionQuery = @"INSERT INTO Options (questionID, optionText, isCorrect)
                                                         VALUES (@QuestionID, @OptionText, @IsCorrect)";
                            foreach (var option in qData.Options)
                            {
                                if (!string.IsNullOrWhiteSpace(option.Value))
                                {
                                    using (SqlCommand cmdOption = new SqlCommand(insertOptionQuery, con, transaction))
                                    {
                                        cmdOption.Parameters.AddWithValue("@QuestionID", newQuestionId);
                                        cmdOption.Parameters.AddWithValue("@OptionText", option.Value);
                                        cmdOption.Parameters.AddWithValue("@IsCorrect", option.Key == qData.CorrectAnswer);
                                        cmdOption.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        savedSuccessfully = true;
                    }
                    catch (Exception ex)
                    {
                        // Try rollback safely
                        try
                        {
                            if (transaction != null && transaction.Connection != null)
                                transaction.Rollback();
                        }
                        catch (Exception rbEx)
                        {
                            System.Diagnostics.Debug.WriteLine("Rollback Error: " + rbEx.ToString());
                        }

                        System.Diagnostics.Debug.WriteLine("Error saving quiz: " + ex.ToString());
                        lblErrorMessage.Text = "Error saving quiz: " + ex.Message;
                        lblErrorMessage.Visible = true;
                        // Return so we do not redirect
                        return;
                    }
                } // transaction disposed
            } // connection disposed

            if (savedSuccessfully)
            {
                // Redirect after successful save. Use false + CompleteRequest to avoid ThreadAbortException while debugging.
                Response.Redirect("TeacherQuiz.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private int GetQuestionNumberFromID(string controlId)
        {
            try { return int.Parse(controlId.Split('_').Last()); }
            catch { return 0; }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // TODO: Add real logout logic (Session.Clear(), etc.)
            Response.Redirect("Login.aspx");
        }
    }
}
