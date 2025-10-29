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
        private int NumberOfQuestions
        {
            get { return (int)(ViewState["QuestionCount"] ?? 1); }
            set { ViewState["QuestionCount"] = value; }
        }

        // Store question data to persist across postbacks
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

        // Track which questions are deleted
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSubjects();
            }
            else
            {
                // Save current form data before recreating controls
                SaveCurrentFormData();
            }

            CreateQuestionControls(); // Recreate controls on every load
        }

        private void LoadSubjects()
        {
            int teacherId = 1; // --- TODO: Get actual teacher ID ---
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

        // Save form data before controls are recreated
        private void SaveCurrentFormData()
        {
            var saved = SavedQuestions;
            var deleted = DeletedQuestions;

            foreach (Control control in phQuestions.Controls)
            {
                if (control is Panel questionCard && control.ID.StartsWith("QuestionCard_"))
                {
                    int qNum = GetQuestionNumberFromID(questionCard.ID);

                    // Skip if already marked as deleted
                    if (deleted.Contains(qNum))
                        continue;

                    TextBox txtQuestion = (TextBox)questionCard.FindControl("txtQuestionText_" + qNum);
                    if (txtQuestion == null)
                        continue;

                    QuestionData qData = new QuestionData
                    {
                        QuestionText = txtQuestion.Text,
                        Options = new Dictionary<string, string>()
                    };

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

            for (int i = 1; i <= NumberOfQuestions; i++)
            {
                // Skip deleted questions
                if (!deleted.Contains(i))
                {
                    AddSingleQuestionControlSet(i);
                }
            }
        }

        private void AddSingleQuestionControlSet(int questionNumber)
        {
            Panel questionCard = new Panel { CssClass = "form-card question-card" };
            questionCard.ID = "QuestionCard_" + questionNumber;

            // --- Question Title Row ---
            Panel titleRow = new Panel { CssClass = "question-title-row" };
            titleRow.Controls.Add(new Literal { Text = $"<h2 class=\"card-title\">Question {questionNumber}</h2>" });

            // --- Delete Button (Only if more than one active question) ---
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

            // --- Question Text Group ---
            Panel textGroup = new Panel { CssClass = "form-group" };
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
            questionCard.Controls.Add(textGroup);

            // --- Answer Options Group ---
            Panel optionsGroup = new Panel { CssClass = "form-group" };
            optionsGroup.Controls.Add(new Literal { Text = "<span class=\"form-label\">Answer Options</span>" });

            Panel optionsListDiv = new Panel { CssClass = "options-list" };
            string radioGroupName = "CorrectAnswerGroup_Q" + questionNumber;

            string[] optionLetters = { "A", "B", "C", "D" };
            for (int j = 0; j < optionLetters.Length; j++)
            {
                Panel optionRow = new Panel { CssClass = "option-row" };
                RadioButton rb = new RadioButton
                {
                    ID = $"rbOption_{questionNumber}_{optionLetters[j]}",
                    GroupName = radioGroupName
                };
                rb.Attributes["value"] = optionLetters[j];

                TextBox txtOpt = new TextBox
                {
                    ID = $"txtOption_{questionNumber}_{optionLetters[j]}",
                    CssClass = "input-field"
                };
                txtOpt.Attributes.Add("placeholder", $"Option {optionLetters[j]}");

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

            // --- RESTORE saved data ---
            var saved = SavedQuestions;
            if (saved.ContainsKey(questionNumber))
            {
                QuestionData qData = saved[questionNumber];
                txtText.Text = qData.QuestionText;

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
                // Default first option as checked for new questions
                RadioButton firstRb = (RadioButton)questionCard.FindControl($"rbOption_{questionNumber}_A");
                if (firstRb != null)
                    firstRb.Checked = true;
            }

            phQuestions.Controls.Add(questionCard);
        }

        protected void DeleteQuestion_Click(object sender, EventArgs e)
        {
            LinkButton btnDelete = (LinkButton)sender;
            int questionNumberToDelete = int.Parse(btnDelete.CommandArgument);

            var deleted = DeletedQuestions;
            deleted.Add(questionNumberToDelete);
            DeletedQuestions = deleted;

            // Also remove from saved data
            var saved = SavedQuestions;
            if (saved.ContainsKey(questionNumberToDelete))
            {
                saved.Remove(questionNumberToDelete);
                SavedQuestions = saved;
            }

            System.Diagnostics.Debug.WriteLine($"Deleted Question {questionNumberToDelete}");
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            NumberOfQuestions++;
            System.Diagnostics.Debug.WriteLine($"Add Question clicked. Question count is now: {NumberOfQuestions}");
        }

        protected void btnCreateQuizSubmit_Click(object sender, EventArgs e)
        {
            // Trigger validation
            Page.Validate();
            if (!Page.IsValid)
            {
                lblErrorMessage.Text = "Please fix the validation errors.";
                lblErrorMessage.Visible = true;
                return;
            }

            // Save current form data before validation
            SaveCurrentFormData();

            // Validate at least one complete question exists
            var saved = SavedQuestions;
            var deleted = DeletedQuestions;

            int validQuestionCount = 0;
            foreach (var kvp in saved)
            {
                if (!deleted.Contains(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value.QuestionText))
                {
                    // Check if at least one option has text
                    bool hasOptions = kvp.Value.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt));
                    if (hasOptions)
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

            string quizTitle = txtQuizTitle.Text;
            int subjectId = int.Parse(ddlSubject.SelectedValue);
            string difficulty = ddlDifficulty.SelectedValue;
            int teacherId = 1; // --- TODO: Get actual teacher ID ---

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            int newQuizId = -1;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. Save Quiz Header
                    string insertQuizQuery = @"INSERT INTO Quizzes (userID, subjectID, title, isPublished, difficulty, createdAt) 
                                              OUTPUT INSERTED.quizID 
                                              VALUES (@UserID, @SubjectID, @Title, @IsPublished, @Difficulty, GETDATE())";

                    using (SqlCommand cmdQuiz = new SqlCommand(insertQuizQuery, con, transaction))
                    {
                        cmdQuiz.Parameters.AddWithValue("@UserID", teacherId);
                        cmdQuiz.Parameters.AddWithValue("@SubjectID", subjectId);
                        cmdQuiz.Parameters.AddWithValue("@Title", quizTitle);
                        cmdQuiz.Parameters.AddWithValue("@IsPublished", 0);
                        cmdQuiz.Parameters.AddWithValue("@Difficulty", difficulty);
                        newQuizId = (int)cmdQuiz.ExecuteScalar();
                    }

                    if (newQuizId <= 0)
                        throw new Exception("Failed to create quiz header.");

                    // 2. Save Questions from ViewState
                    foreach (var kvp in saved)
                    {
                        int questionNumber = kvp.Key;
                        QuestionData qData = kvp.Value;

                        // Skip deleted or empty questions
                        if (deleted.Contains(questionNumber) || string.IsNullOrWhiteSpace(qData.QuestionText))
                            continue;

                        // Check if has at least one option with text
                        bool hasValidOptions = qData.Options.Values.Any(opt => !string.IsNullOrWhiteSpace(opt));
                        if (!hasValidOptions)
                            continue;

                        // Save Question
                        string insertQuestionQuery = @"INSERT INTO Questions (quizID, text, points) 
                                                      OUTPUT INSERTED.questionID 
                                                      VALUES (@QuizID, @Text, @Points)";

                        int newQuestionId;
                        using (SqlCommand cmdQuestion = new SqlCommand(insertQuestionQuery, con, transaction))
                        {
                            cmdQuestion.Parameters.AddWithValue("@QuizID", newQuizId);
                            cmdQuestion.Parameters.AddWithValue("@Text", qData.QuestionText);
                            cmdQuestion.Parameters.AddWithValue("@Points", 10); // Default 10 points
                            newQuestionId = (int)cmdQuestion.ExecuteScalar();
                        }

                        if (newQuestionId <= 0)
                            throw new Exception($"Failed to create question {questionNumber}.");

                        // Save Options
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
                    Response.Redirect("Quizzes.aspx");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("Error saving quiz: " + ex.Message);
                    lblErrorMessage.Text = "Error saving quiz: " + ex.Message;
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
            Response.Redirect("Login.aspx");
        }
    }

    // Helper class to store question data (must be Serializable for ViewState)
    [Serializable]
    public class QuestionData
    {
        public string QuestionText { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public string CorrectAnswer { get; set; }

        public QuestionData()
        {
            Options = new Dictionary<string, string>();
            CorrectAnswer = "A"; // Default
        }
    }
}