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

        // --- State stored in Session ---
        private int NumberOfQuestions
        {
            get { return (int)(Session[CurrentQuizId + "_QuestionCount"] ?? 0); } // Use QuizID in key
            set { Session[CurrentQuizId + "_QuestionCount"] = value; }
        }

        private Dictionary<int, QuestionData> SavedQuestions
        {
            get
            {
                if (Session[CurrentQuizId + "_SavedQuestions"] == null)
                    Session[CurrentQuizId + "_SavedQuestions"] = new Dictionary<int, QuestionData>();
                return (Dictionary<int, QuestionData>)Session[CurrentQuizId + "_SavedQuestions"];
            }
            set { Session[CurrentQuizId + "_SavedQuestions"] = value; }
        }

        private HashSet<int> DeletedQuestions
        {
            get
            {
                if (Session[CurrentQuizId + "_DeletedQuestions"] == null)
                    Session[CurrentQuizId + "_DeletedQuestions"] = new HashSet<int>();
                return (HashSet<int>)Session[CurrentQuizId + "_DeletedQuestions"];
            }
            set { Session[CurrentQuizId + "_DeletedQuestions"] = value; }
        }

        // Helper to store Questions to delete from DB (separate from UI-only deletes)
        private HashSet<int> DbQuestionsToDelete
        {
            get
            {
                if (Session[CurrentQuizId + "_DbQuestionsToDelete"] == null)
                    Session[CurrentQuizId + "_DbQuestionsToDelete"] = new HashSet<int>();
                return (HashSet<int>)Session[CurrentQuizId + "_DbQuestionsToDelete"];
            }
            set { Session[CurrentQuizId + "_DbQuestionsToDelete"] = value; }
        }
        // --- End session-backed state ---

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Get QuizID early for session keys
            int.TryParse(Request.QueryString["quizID"], out CurrentQuizId);
            if (CurrentQuizId == 0 && IsPostBack)
            {
                // Try to get it from a hidden field if postback and querystring is lost
                // (Good practice, but not strictly required if querystring is always present)
            }

            // Ensure controls exist before LoadViewState/LoadPostData
            // This rebuilds the structure based on the *previous* request's question count
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

            // --- Get Quiz ID from URL ---
            if (!int.TryParse(Request.QueryString["quizID"], out CurrentQuizId))
            {
                Response.Redirect("~/TeacherQuiz.aspx"); // Use your quiz list page name
                return;
            }


            if (IsPostBack)
            {
                // On postback, controls were built in OnInit,
                // so we save their current values to our Session state
                SaveCurrentFormData();
            }

            CreateQuestionControls();

            if (!IsPostBack)
            {

                ClearSessionState(); // 1. Clear any old data
                LoadSubjects();      // 2. Load dropdowns
                LoadQuizData();      // 3. Load DB data into Session state
                CreateQuestionControls();
            }
        }

        // Use PreRender to build controls based on the *final* state after events
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            // Re-create controls here to reflect changes from Add/Delete clicks
            // But skip if it's the initial load (already built in Page_Load)
            if (IsPostBack)
            {
                CreateQuestionControls();
            }
        }

        private void ClearSessionState()
        {
            Session[CurrentQuizId + "_QuestionCount"] = null;
            Session[CurrentQuizId + "_SavedQuestions"] = null;
            Session[CurrentQuizId + "_DeletedQuestions"] = null;
            Session[CurrentQuizId + "_DbQuestionsToDelete"] = null;
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
                                ddlDifficulty.SelectedValue = reader["difficulty"].ToString();
                            }
                            else
                            {
                                lblErrorMessage.Text = "Quiz not found or you do not have permission to edit it.";
                                lblErrorMessage.Visible = true;
                                btnUpdateQuiz.Enabled = false; // Assuming ASPX button ID is btnUpdateQuiz
                                return;
                            }
                        }
                    }

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
                                if (questionId != currentQuestionId)
                                {
                                    questionCounter++;
                                    currentQuestionId = questionId;
                                    optionIndex = 0;
                                    currentQuestion = new QuestionData
                                    {
                                        QuestionText = reader["text"].ToString(),
                                        Points = (int)reader["points"],
                                        DatabaseQuestionID = questionId // Store DB ID
                                    };
                                    savedQuestions.Add(questionCounter, currentQuestion);
                                }
                                if (reader["optionID"] != DBNull.Value && currentQuestion != null && optionIndex < 4)
                                {
                                    string optionLetter = optionLetters[optionIndex];
                                    currentQuestion.Options[optionLetter] = reader["optionText"].ToString();
                                    if ((bool)reader["isCorrect"])
                                    {
                                        currentQuestion.CorrectAnswer = optionLetter;
                                    }
                                    optionIndex++;
                                }
                            }
                        }
                    }
                    NumberOfQuestions = questionCounter; // Set count based on DB
                    SavedQuestions = savedQuestions;     // Set saved data from DB
                }
                catch (Exception ex)
                {
                    lblErrorMessage.Text = "Error loading quiz data: " + ex.Message;
                    lblErrorMessage.Visible = true;
                }
            }
        }


        private void LoadSubjects()
        {
            // ... (Same as your code) ...
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

        private void SaveCurrentFormData()
        {
            // ... (Same as your code) ...
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

                // Retrieve existing data to preserve DatabaseQuestionID
                QuestionData qData = saved.ContainsKey(qNum) ? saved[qNum] : new QuestionData();

                qData.QuestionText = txtQuestion.Text;
                qData.Options = new Dictionary<string, string>();

                if (int.TryParse(txtPoints.Text, out int pointsValue) && pointsValue > 0)
                {
                    qData.Points = pointsValue;
                }
                else
                {
                    qData.Points = 10;
                }

                string radioGroupName = "CorrectAnswerGroup_Q" + qNum;
                string selectedValue = Request.Form[radioGroupName];
                qData.CorrectAnswer = selectedValue ?? "A";

                string[] optionLetters = { "A", "B", "C", "D" };
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

        private void CreateQuestionControls()
        {
            // ... (Same as your code) ...
            phQuestions.Controls.Clear();
            var deleted = DeletedQuestions;
            int displayIndex = 1; // For sequential numbering (Question 1, Question 2...)

            for (int i = 1; i <= NumberOfQuestions; i++)
            {
                if (!deleted.Contains(i))
                {
                    AddSingleQuestionControlSet(i, displayIndex);
                    displayIndex++; // Increment display index only for visible questions
                }
            }
        }

        private void AddSingleQuestionControlSet(int questionNumber, int displayIndex)
        {
            Panel questionCard = new Panel { CssClass = "form-card question-card" };
            questionCard.ID = "QuestionCard_" + questionNumber; // ID is based on original number

            Panel titleRow = new Panel { CssClass = "question-title-row" };
            titleRow.Controls.Add(new Literal { Text = $"<h2 class=\"card-title\">Question {displayIndex}</h2>" }); // Use display index

            int activeQuestionCount = NumberOfQuestions - DeletedQuestions.Count;
            if (activeQuestionCount > 1)
            {
                LinkButton btnDelete = new LinkButton
                { /* ... delete button setup ... */
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
            { /* ... range validator setup ... */
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

            // Restore saved data
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
                    if (txtOpt != null && qData.Options.ContainsKey(letter)) txtOpt.Text = qData.Options[letter];
                    if (rb != null) rb.Checked = (letter == qData.CorrectAnswer);
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
            // Also mark for DB deletion if it's an existing question
            if (saved.ContainsKey(questionNumberToDelete) && saved[questionNumberToDelete].DatabaseQuestionID > 0)
            {
                var dbDeleted = DbQuestionsToDelete;
                dbDeleted.Add(saved[questionNumberToDelete].DatabaseQuestionID);
                DbQuestionsToDelete = dbDeleted;
            }
            saved.Remove(questionNumberToDelete);
            SavedQuestions = saved;

            System.Diagnostics.Debug.WriteLine($"Marked Question {questionNumberToDelete} for deletion.");
            // Rely on Page_Load cycle (via OnInit/OnPreRender) to rebuild controls
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            NumberOfQuestions++;
            System.Diagnostics.Debug.WriteLine($"Add Question clicked. Question count is now: {NumberOfQuestions}");
            // Rely on Page_Load cycle (via OnInit/OnPreRender) to rebuild controls
        }

        // --- RENAMED & CORRECTED: btnUpdateQuiz_Click ---
        protected void btnUpdateQuiz_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                lblErrorMessage.Text = "Please fix the validation errors.";
                lblErrorMessage.Visible = true;
                return;
            }

            SaveCurrentFormData(); // Capture final data

            var saved = SavedQuestions;
            var deleted = DeletedQuestions;
            var dbDeleted = DbQuestionsToDelete;

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

                        // 2. Process Deleted Questions (from DB)
                        if (dbDeleted.Count > 0)
                        {
                            // Need to delete Options -> Questions
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

                        // 3. Process Saved Questions (Update existing, Insert new)
                        foreach (var kvp in saved)
                        {
                            int questionNumber = kvp.Key;
                            QuestionData qData = kvp.Value;

                            if (deleted.Contains(questionNumber) || string.IsNullOrWhiteSpace(qData.QuestionText) || !qData.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt)))
                            {
                                continue;
                            }

                            if (qData.DatabaseQuestionID > 0) // Existing Question -> UPDATE
                            {
                                int existingQuestionId = qData.DatabaseQuestionID;
                                // Update Question Text/Points
                                string updateQuestionQuery = @"UPDATE Questions SET text = @Text, points = @Points 
                                                               WHERE questionID = @QuestionID AND quizID = @QuizID";
                                using (SqlCommand cmdUpdateQ = new SqlCommand(updateQuestionQuery, con, transaction))
                                {
                                    cmdUpdateQ.Parameters.AddWithValue("@Text", qData.QuestionText);
                                    cmdUpdateQ.Parameters.AddWithValue("@Points", qData.Points);
                                    cmdUpdateQ.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                    cmdUpdateQ.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                                    cmdUpdateQ.ExecuteNonQuery();
                                }
                                // Delete existing options for this question
                                string deleteOptionsQuery = "DELETE FROM Options WHERE questionID = @QuestionID";
                                using (SqlCommand cmdDelOpt = new SqlCommand(deleteOptionsQuery, con, transaction))
                                {
                                    cmdDelOpt.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                    cmdDelOpt.ExecuteNonQuery();
                                }
                                // Insert (new) options
                                string insertOptionQuery = @"INSERT INTO Options (questionID, optionText, isCorrect)
                                                             VALUES (@QuestionID, @OptionText, @IsCorrect)";
                                foreach (var option in qData.Options)
                                {
                                    if (!string.IsNullOrWhiteSpace(option.Value))
                                    {
                                        using (SqlCommand cmdOption = new SqlCommand(insertOptionQuery, con, transaction))
                                        {
                                            cmdOption.Parameters.AddWithValue("@QuestionID", existingQuestionId);
                                            cmdOption.Parameters.AddWithValue("@OptionText", option.Value);
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
                                    cmdQuestion.Parameters.AddWithValue("@QuizID", CurrentQuizId); // Use CurrentQuizId
                                    cmdQuestion.Parameters.AddWithValue("@Text", qData.QuestionText);
                                    cmdQuestion.Parameters.AddWithValue("@Points", qData.Points);
                                    newQuestionId = (int)cmdQuestion.ExecuteScalar();
                                }
                                if (newQuestionId <= 0) throw new Exception($"Failed to create new question {questionNumber}.");

                                // Insert options for new question
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
                        } // End foreach question

                        transaction.Commit();
                        savedSuccessfully = true;
                    }
                    catch (Exception ex)
                    {
                        try { if (transaction.Connection != null) transaction.Rollback(); }
                        catch (Exception rbEx) { System.Diagnostics.Debug.WriteLine("Rollback Error: " + rbEx.ToString()); }

                        System.Diagnostics.Debug.WriteLine("Error saving quiz: " + ex.ToString());
                        lblErrorMessage.Text = "Error saving quiz: " + ex.Message;
                        lblErrorMessage.Visible = true;
                        return; // Stop execution
                    }
                } // transaction disposed
            } // connection disposed

            if (savedSuccessfully)
            {
                ClearSessionState(); // Clear session data for this quiz after successful save
                Response.Redirect("TeacherQuiz.aspx", false); // Use TeacherQuiz.aspx
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
            Session.Clear(); // Add logout logic
            Session.Abandon();
            Response.Redirect("Auth.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearSessionState(); // Clear any changes from session
            Response.Redirect("TeacherQuiz.aspx"); // Redirect back to quiz list
        }

    }

}