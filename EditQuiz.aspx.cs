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
    public partial class EditQuiz : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;
        private int CurrentQuizId = 0;

        // Use ViewState instead of Session for better isolation
        private int NumberOfQuestions
        {
            get { return (int)(ViewState["QuestionCount"] ?? 0); }
            set { ViewState["QuestionCount"] = value; }
        }

        private Dictionary<int, QuestionData> SavedQuestions
        {
            get
            {
                if (ViewState["SavedQuestions"] == null)
                    ViewState["SavedQuestions"] = new Dictionary<int, QuestionData>();
                return (Dictionary<int, QuestionData>)ViewState["SavedQuestions"];
            }
            set { ViewState["SavedQuestions"] = value; }
        }

        private HashSet<int> DeletedQuestions
        {
            get
            {
                if (ViewState["DeletedQuestions"] == null)
                    ViewState["DeletedQuestions"] = new HashSet<int>();
                return (HashSet<int>)ViewState["DeletedQuestions"];
            }
            set { ViewState["DeletedQuestions"] = value; }
        }

        private HashSet<int> DbQuestionsToDelete
        {
            get
            {
                if (ViewState["DbQuestionsToDelete"] == null)
                    ViewState["DbQuestionsToDelete"] = new HashSet<int>();
                return (HashSet<int>)ViewState["DbQuestionsToDelete"];
            }
            set { ViewState["DbQuestionsToDelete"] = value; }
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

            // --- Get Quiz ID from URL ---
            if (!int.TryParse(Request.QueryString["quizID"], out CurrentQuizId) || CurrentQuizId <= 0)
            {
                Response.Redirect("~/TeacherQuiz.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSubjects();
                LoadQuizData(); // Load data first - sets NumberOfQuestions
                CreateQuestionControls(); // Create controls after loading data
            }
            else
            {
                // On postback, recreate controls BEFORE events fire
                CreateQuestionControls();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Save form data after all events have fired
            if (IsPostBack)
            {
                SaveCurrentFormData();
            }

            // Recreate controls with updated state
            CreateQuestionControls();
        }

        private void LoadQuizData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            var savedQuestions = new Dictionary<int, QuestionData>();
            int questionCounter = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // Load Quiz Header
                    string quizQuery = "SELECT title, subjectID, difficulty FROM Quizzes WHERE quizID = @QuizID AND userID = @TeacherID";
                    using (SqlCommand cmdQuiz = new SqlCommand(quizQuery, con))
                    {
                        cmdQuiz.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                        cmdQuiz.Parameters.AddWithValue("@TeacherID", CurrentTeacherId);
                        using (SqlDataReader reader = cmdQuiz.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtQuizTitle.Text = reader["title"].ToString();
                                ddlSubject.SelectedValue = reader["subjectID"].ToString();
                                ddlDifficulty.SelectedValue = reader["difficulty"].ToString().Trim();
                            }
                            else
                            {
                                lblErrorMessage.Text = "Quiz not found or you do not have permission to edit it.";
                                lblErrorMessage.Visible = true;
                                btnUpdateQuiz.Enabled = false;
                                return;
                            }
                        }
                    }

                    // Load Questions and Options
                    string questionsQuery = @"SELECT q.questionID, q.text, q.points, o.optionID, o.optionText, o.isCorrect
                                              FROM Questions q
                                              LEFT JOIN Options o ON q.questionID = o.questionID
                                              WHERE q.quizID = @QuizID
                                              ORDER BY q.questionID, o.optionID";

                    using (SqlCommand cmdQuestions = new SqlCommand(questionsQuery, con))
                    {
                        cmdQuestions.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                        using (SqlDataReader reader = cmdQuestions.ExecuteReader())
                        {
                            QuestionData currentQuestion = null;
                            int currentQuestionId = -1;
                            string[] optionLetters = { "A", "B", "C", "D" };
                            int optionIndex = 0;

                            while (reader.Read())
                            {
                                int questionId = (int)reader["questionID"];

                                // New question encountered
                                if (questionId != currentQuestionId)
                                {
                                    questionCounter++;
                                    currentQuestionId = questionId;
                                    optionIndex = 0;

                                    currentQuestion = new QuestionData();
                                    currentQuestion.QuestionText = reader["text"].ToString();
                                    currentQuestion.Points = (int)reader["points"];
                                    currentQuestion.DatabaseQuestionID = questionId;

                                    savedQuestions[questionCounter] = currentQuestion;

                                    System.Diagnostics.Debug.WriteLine($"Loading Question {questionCounter}: {currentQuestion.QuestionText}");
                                }

                                // Add options
                                if (reader["optionID"] != DBNull.Value && currentQuestion != null && optionIndex < 4)
                                {
                                    string optionLetter = optionLetters[optionIndex];
                                    string optionText = reader["optionText"].ToString();
                                    currentQuestion.Options[optionLetter] = optionText;

                                    System.Diagnostics.Debug.WriteLine($"  Option {optionLetter}: {optionText}");

                                    if ((bool)reader["isCorrect"])
                                    {
                                        currentQuestion.CorrectAnswer = optionLetter;
                                        System.Diagnostics.Debug.WriteLine($"  Correct Answer: {optionLetter}");
                                    }
                                    optionIndex++;
                                }
                            }
                        }
                    }

                    NumberOfQuestions = questionCounter;
                    SavedQuestions = savedQuestions;

                    System.Diagnostics.Debug.WriteLine($"Total questions loaded: {questionCounter}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading quiz data: " + ex.Message);
                    lblErrorMessage.Text = "Error loading quiz data: " + ex.Message;
                    lblErrorMessage.Visible = true;
                }
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
                        System.Diagnostics.Debug.WriteLine("Error loading subjects: " + ex.Message);
                        lblErrorMessage.Text = "Error loading subjects. Please try again later.";
                        lblErrorMessage.Visible = true;
                    }
                }
            }
            ddlSubject.Items.Insert(0, new ListItem("-- Select Subject --", ""));
        }

        private void SaveCurrentFormData()
        {
            var saved = SavedQuestions;
            var deleted = DeletedQuestions;

            foreach (Control control in phQuestions.Controls)
            {
                if (control is Panel questionCard && control.ID != null && control.ID.StartsWith("QuestionCard_"))
                {
                    int qNum = GetQuestionNumberFromID(questionCard.ID);

                    if (deleted.Contains(qNum))
                        continue;

                    TextBox txtQuestion = (TextBox)questionCard.FindControl("txtQuestionText_" + qNum);
                    TextBox txtPoints = (TextBox)questionCard.FindControl("txtPoints_" + qNum);

                    if (txtQuestion == null)
                        continue;

                    // Retrieve existing data to preserve DatabaseQuestionID
                    QuestionData qData = saved.ContainsKey(qNum) ? saved[qNum] : new QuestionData();

                    qData.QuestionText = txtQuestion.Text;
                    qData.Options = new Dictionary<string, string>();

                    // Parse points
                    if (txtPoints != null && int.TryParse(txtPoints.Text, out int pointsValue) && pointsValue > 0)
                    {
                        qData.Points = pointsValue;
                    }
                    else
                    {
                        qData.Points = 10;
                    }

                    // Get selected radio button
                    string radioGroupName = "CorrectAnswerGroup_Q" + qNum;
                    qData.CorrectAnswer = Request.Form[radioGroupName] ?? "A";

                    // Get option values
                    string[] optionLetters = { "A", "B", "C", "D" };
                    foreach (string letter in optionLetters)
                    {
                        TextBox txtOpt = (TextBox)questionCard.FindControl($"txtOption_{qNum}_{letter}");
                        if (txtOpt != null)
                            qData.Options[letter] = txtOpt.Text;
                    }

                    saved[qNum] = qData;
                }
            }

            SavedQuestions = saved;
        }

        private void CreateQuestionControls()
        {
            phQuestions.Controls.Clear();
            var deleted = DeletedQuestions;
            int displayIndex = 1;

            for (int i = 1; i <= NumberOfQuestions; i++)
            {
                if (!deleted.Contains(i))
                {
                    AddSingleQuestionControlSet(i, displayIndex);
                    displayIndex++;
                }
            }
        }

        private void AddSingleQuestionControlSet(int questionNumber, int displayIndex)
        {
            Panel questionCard = new Panel { CssClass = "form-card question-card" };
            questionCard.ID = "QuestionCard_" + questionNumber;

            // --- Question Title Row ---
            Panel titleRow = new Panel { CssClass = "question-title-row" };
            titleRow.Controls.Add(new Literal { Text = $"<h2 class=\"card-title\">Question {displayIndex}</h2>" });

            // --- Delete Button ---
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

            // --- Question Row ---
            Panel questionRow = new Panel { CssClass = "form-row" };

            // Question Text Group
            Panel textGroup = new Panel { CssClass = "form-group" };
            textGroup.Style.Add("flex", "5");
            Label lblText = new Label
            {
                AssociatedControlID = "txtQuestionText_" + questionNumber,
                CssClass = "form-label",
                Text = "Question Text"
            };
            TextBox txtText = new TextBox
            {
                ID = "txtQuestionText_" + questionNumber,
                CssClass = "textarea-field",
                TextMode = TextBoxMode.MultiLine
            };
            txtText.Attributes.Add("placeholder", "Enter your question here...");
            textGroup.Controls.Add(lblText);
            textGroup.Controls.Add(txtText);
            questionRow.Controls.Add(textGroup);

            // Points Group
            Panel pointsGroup = new Panel { CssClass = "form-group" };
            pointsGroup.Style.Add("flex", "1");
            Label lblPoints = new Label
            {
                AssociatedControlID = "txtPoints_" + questionNumber,
                CssClass = "form-label",
                Text = "Points"
            };
            TextBox txtPoints = new TextBox
            {
                ID = "txtPoints_" + questionNumber,
                CssClass = "input-field",
                TextMode = TextBoxMode.Number,
                Text = "10"
            };
            txtPoints.Attributes.Add("min", "1");
            txtPoints.Attributes.Add("max", "100");

            RangeValidator rvPoints = new RangeValidator
            {
                ID = "rvPoints_" + questionNumber,
                ControlToValidate = txtPoints.ID,
                MinimumValue = "1",
                MaximumValue = "100",
                Type = ValidationDataType.Integer,
                ErrorMessage = "Points must be 1-100",
                ForeColor = System.Drawing.Color.Red,
                Display = ValidatorDisplay.Dynamic
            };
            pointsGroup.Controls.Add(lblPoints);
            pointsGroup.Controls.Add(txtPoints);
            pointsGroup.Controls.Add(rvPoints);
            questionRow.Controls.Add(pointsGroup);

            questionCard.Controls.Add(questionRow);

            // --- Answer Options Group ---
            Panel optionsGroup = new Panel { CssClass = "form-group" };
            optionsGroup.Controls.Add(new Literal { Text = "<span class=\"form-label\">Answer Options</span>" });

            Panel optionsListDiv = new Panel { CssClass = "options-list" };
            string radioGroupName = "CorrectAnswerGroup_Q" + questionNumber;
            string[] optionLetters = { "A", "B", "C", "D" };

            for (int j = 0; j < optionLetters.Length; j++)
            {
                string letter = optionLetters[j];
                Panel optionRow = new Panel { CssClass = "option-row" };
                RadioButton rb = new RadioButton
                {
                    ID = $"rbOption_{questionNumber}_{letter}",
                    GroupName = radioGroupName
                };
                rb.Attributes["value"] = letter;

                TextBox txtOpt = new TextBox
                {
                    ID = $"txtOption_{questionNumber}_{letter}",
                    CssClass = "input-field"
                };
                txtOpt.Attributes.Add("placeholder", $"Option {letter}");

                optionRow.Controls.Add(rb);
                optionRow.Controls.Add(txtOpt);
                optionsListDiv.Controls.Add(optionRow);
            }

            optionsGroup.Controls.Add(optionsListDiv);
            optionsGroup.Controls.Add(new Literal
            {
                Text = "<span class=\"helper-text\">Select the radio button for the correct answer</span>"
            });
            questionCard.Controls.Add(optionsGroup);

            // Add to placeholder FIRST before restoring data
            phQuestions.Controls.Add(questionCard);

            // --- RESTORE saved data AFTER adding to placeholder ---
            var saved = SavedQuestions;
            System.Diagnostics.Debug.WriteLine($"Restoring data for question {questionNumber}, Saved questions count: {saved.Count}");

            if (saved.ContainsKey(questionNumber))
            {
                QuestionData qData = saved[questionNumber];
                System.Diagnostics.Debug.WriteLine($"Found data for question {questionNumber}: {qData.QuestionText}");
                System.Diagnostics.Debug.WriteLine($"Options count: {qData.Options.Count}");

                txtText.Text = qData.QuestionText;
                txtPoints.Text = qData.Points.ToString();

                foreach (string letter in optionLetters)
                {
                    TextBox txtOpt = (TextBox)questionCard.FindControl($"txtOption_{questionNumber}_{letter}");
                    RadioButton rb = (RadioButton)questionCard.FindControl($"rbOption_{questionNumber}_{letter}");

                    if (txtOpt != null && qData.Options.ContainsKey(letter))
                    {
                        txtOpt.Text = qData.Options[letter];
                        System.Diagnostics.Debug.WriteLine($"Set option {letter} to: {qData.Options[letter]}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not set option {letter} - txtOpt null: {txtOpt == null}, has key: {qData.Options.ContainsKey(letter)}");
                    }

                    if (rb != null)
                        rb.Checked = (letter == qData.CorrectAnswer);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No saved data found for question {questionNumber}");
                // Default values for new questions
                txtPoints.Text = "10";
                RadioButton firstRb = (RadioButton)questionCard.FindControl($"rbOption_{questionNumber}_A");
                if (firstRb != null)
                    firstRb.Checked = true;
            }
        }

        protected void DeleteQuestion_Click(object sender, EventArgs e)
        {
            LinkButton btnDelete = (LinkButton)sender;
            if (!int.TryParse(btnDelete.CommandArgument, out int questionNumberToDelete))
                return;

            var deleted = DeletedQuestions;
            deleted.Add(questionNumberToDelete);
            DeletedQuestions = deleted;

            var saved = SavedQuestions;
            // Mark for DB deletion if it's an existing question
            if (saved.ContainsKey(questionNumberToDelete) && saved[questionNumberToDelete].DatabaseQuestionID > 0)
            {
                var dbDeleted = DbQuestionsToDelete;
                dbDeleted.Add(saved[questionNumberToDelete].DatabaseQuestionID);
                DbQuestionsToDelete = dbDeleted;
            }
            saved.Remove(questionNumberToDelete);
            SavedQuestions = saved;

            System.Diagnostics.Debug.WriteLine($"Marked Question {questionNumberToDelete} for deletion.");
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            NumberOfQuestions++;
            System.Diagnostics.Debug.WriteLine($"Add Question clicked. Question count is now: {NumberOfQuestions}");
        }

        protected void btnUpdateQuiz_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                lblErrorMessage.Text = "Please fix the validation errors.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Save current form data
            SaveCurrentFormData();

            var saved = SavedQuestions;
            var deleted = DeletedQuestions;
            var dbDeleted = DbQuestionsToDelete;

            // Validate at least one complete question exists
            int validQuestionCount = 0;
            foreach (var kvp in saved)
            {
                if (!deleted.Contains(kvp.Key) &&
                    !string.IsNullOrWhiteSpace(kvp.Value.QuestionText) &&
                    kvp.Value.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt)))
                {
                    validQuestionCount++;
                }
            }

            if (validQuestionCount == 0)
            {
                lblErrorMessage.Text = "Please add at least one complete question with answer options.";
                lblErrorMessage.Visible = true;
                return;
            }

            lblErrorMessage.Visible = false;

            string quizTitle = txtQuizTitle.Text.Trim();
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

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. UPDATE Quiz Header
                    string updateQuizQuery = @"UPDATE Quizzes 
                                               SET title = @Title, subjectID = @SubjectID, difficulty = @Difficulty
                                               WHERE quizID = @QuizID AND userID = @UserID";
                    using (SqlCommand cmdQuiz = new SqlCommand(updateQuizQuery, con, transaction))
                    {
                        cmdQuiz.Parameters.AddWithValue("@Title", quizTitle);
                        cmdQuiz.Parameters.AddWithValue("@SubjectID", subjectId);
                        cmdQuiz.Parameters.AddWithValue("@Difficulty", difficulty);
                        cmdQuiz.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                        cmdQuiz.Parameters.AddWithValue("@UserID", teacherId);
                        cmdQuiz.ExecuteNonQuery();
                    }

                    // 2. Delete Questions marked for deletion
                    if (dbDeleted.Count > 0)
                    {
                        string deleteOptionsQuery = $"DELETE FROM Options WHERE questionID IN ({string.Join(",", dbDeleted)})";
                        using (SqlCommand cmdDelOpt = new SqlCommand(deleteOptionsQuery, con, transaction))
                        {
                            cmdDelOpt.ExecuteNonQuery();
                        }

                        string deleteQuestionsQuery = $"DELETE FROM Questions WHERE questionID IN ({string.Join(",", dbDeleted)}) AND quizID = @QuizID";
                        using (SqlCommand cmdDelQ = new SqlCommand(deleteQuestionsQuery, con, transaction))
                        {
                            cmdDelQ.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                            cmdDelQ.ExecuteNonQuery();
                        }
                    }

                    // 3. Update existing questions and insert new ones
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

                        if (qData.DatabaseQuestionID > 0) // Existing Question -> UPDATE
                        {
                            int existingQuestionId = qData.DatabaseQuestionID;

                            // Update Question
                            string updateQuestionQuery = @"UPDATE Questions SET text = @Text, points = @Points 
                                                           WHERE questionID = @QuestionID AND quizID = @QuizID";
                            using (SqlCommand cmdUpdateQ = new SqlCommand(updateQuestionQuery, con, transaction))
                            {
                                cmdUpdateQ.Parameters.AddWithValue("@Text", qData.QuestionText.Trim());
                                cmdUpdateQ.Parameters.AddWithValue("@Points", qData.Points);
                                cmdUpdateQ.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                cmdUpdateQ.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                                cmdUpdateQ.ExecuteNonQuery();
                            }

                            // Delete existing options
                            string deleteOptionsQuery = "DELETE FROM Options WHERE questionID = @QuestionID";
                            using (SqlCommand cmdDelOpt = new SqlCommand(deleteOptionsQuery, con, transaction))
                            {
                                cmdDelOpt.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                cmdDelOpt.ExecuteNonQuery();
                            }

                            // Insert new options
                            string insertOptionQuery = @"INSERT INTO Options (questionID, optionText, isCorrect)
                                                         VALUES (@QuestionID, @OptionText, @IsCorrect)";
                            foreach (var option in qData.Options)
                            {
                                if (!string.IsNullOrWhiteSpace(option.Value))
                                {
                                    using (SqlCommand cmdOption = new SqlCommand(insertOptionQuery, con, transaction))
                                    {
                                        cmdOption.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                        cmdOption.Parameters.AddWithValue("@OptionText", option.Value.Trim());
                                        cmdOption.Parameters.AddWithValue("@IsCorrect", option.Key == qData.CorrectAnswer);
                                        cmdOption.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        else // New Question -> INSERT
                        {
                            int newQuestionId;
                            string insertQuestionQuery = @"INSERT INTO Questions (quizID, text, points)
                                                           OUTPUT INSERTED.questionID
                                                           VALUES (@QuizID, @Text, @Points)";
                            using (SqlCommand cmdQuestion = new SqlCommand(insertQuestionQuery, con, transaction))
                            {
                                cmdQuestion.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                                cmdQuestion.Parameters.AddWithValue("@Text", qData.QuestionText.Trim());
                                cmdQuestion.Parameters.AddWithValue("@Points", qData.Points);
                                newQuestionId = (int)cmdQuestion.ExecuteScalar();
                            }

                            if (newQuestionId <= 0)
                                throw new Exception($"Failed to create new question {questionNumber}.");

                            // Insert options
                            string insertOptionQuery = @"INSERT INTO Options (questionID, optionText, isCorrect)
                                                         VALUES (@QuestionID, @OptionText, @IsCorrect)";
                            foreach (var option in qData.Options)
                            {
                                if (!string.IsNullOrWhiteSpace(option.Value))
                                {
                                    using (SqlCommand cmdOption = new SqlCommand(insertOptionQuery, con, transaction))
                                    {
                                        cmdOption.Parameters.AddWithValue("@QuestionID", newQuestionId);
                                        cmdOption.Parameters.AddWithValue("@OptionText", option.Value.Trim());
                                        cmdOption.Parameters.AddWithValue("@IsCorrect", option.Key == qData.CorrectAnswer);
                                        cmdOption.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }

                    transaction.Commit();

                    // Clear ViewState after successful save
                    ViewState.Clear();

                    Response.Redirect("TeacherQuiz.aspx");
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (transaction != null && transaction.Connection != null)
                            transaction.Rollback();
                    }
                    catch (Exception rbEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Rollback Error: " + rbEx.Message);
                    }

                    System.Diagnostics.Debug.WriteLine("Error updating quiz: " + ex.Message);
                    lblErrorMessage.Text = "Error updating quiz: " + ex.Message;
                    lblErrorMessage.Visible = true;
                }
            }
        }

        private int GetQuestionNumberFromID(string controlId)
        {
            try
            {
                return int.Parse(controlId.Split('_').Last());
            }
            catch
            {
                return 0;
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Auth.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ViewState.Clear();
            Response.Redirect("TeacherQuiz.aspx");
        }
    }
}