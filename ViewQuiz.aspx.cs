using BrainBlitz.Student;
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
    // Make sure your ASPX file Inherits="BrainBlitz.ViewQuiz"
    public partial class ViewQuiz : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;
        private int CurrentQuizId = 0;

        // We only need AllQuestions.
        private List<QuizQuestion> AllQuestions
        {
            // Use a unique session key for ViewQuiz to avoid conflicts
            get { return (List<QuizQuestion>)Session["ViewQuiz_Questions_" + CurrentQuizId]; }
            set { Session["ViewQuiz_Questions_" + CurrentQuizId] = value; }
        }

        // --- UserAnswers Dictionary REMOVED ---

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check for TEACHER role
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            CurrentTeacherId = (int)Session["UserID"];

            // Get QuizID
            if (!int.TryParse(Request.QueryString["quizID"], out CurrentQuizId))
            {
                Response.Redirect("~/TeacherQuiz.aspx"); // Redirect to TEACHER quiz list
                return;
            }

            if (!IsPostBack)
            {
                StartQuiz();
            }
        }

        private void StartQuiz()
        {
            AllQuestions = null; // Clear any previous quiz preview
            hfCurrentQuestionIndex.Value = "0";

            LoadQuestionsFromDB();

            if (AllQuestions == null || AllQuestions.Count == 0)
            {
                Response.Redirect("~/TeacherQuiz.aspx"); // Redirect to TEACHER quiz list
                return;
            }

            DisplayQuestion(0);
        }

        private void LoadQuestionsFromDB()
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // This query also checks if the quiz belongs to the teacher for security
                string query = @"
                    SELECT 
                        q.title AS QuizTitle,
                        qn.questionID, 
                        qn.text AS QuestionText, 
                        qn.points,
                        o.optionID, 
                        o.optionText, 
                        o.isCorrect
                    FROM Quizzes q
                    JOIN Questions qn ON q.quizID = qn.quizID
                    LEFT JOIN Options o ON qn.questionID = o.questionID
                    WHERE q.quizID = @QuizID AND q.userID = @TeacherID
                    ORDER BY qn.questionID, o.optionID;
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                    cmd.Parameters.AddWithValue("@TeacherID", CurrentTeacherId); // Security check
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        QuizQuestion currentQuestion = null;
                        int currentQuestionId = 0;
                        string quizTitle = "Quiz Preview";

                        while (reader.Read())
                        {
                            // Set title from the first row
                            if (string.Equals(quizTitle, "Quiz Preview", StringComparison.OrdinalIgnoreCase) && reader["QuizTitle"] != DBNull.Value)
                            {
                                quizTitle = reader["QuizTitle"].ToString();
                            }

                            int qId = (int)reader["questionID"];
                            if (qId != currentQuestionId)
                            {
                                currentQuestionId = qId;
                                currentQuestion = new QuizQuestion
                                {
                                    QuestionID = qId,
                                    QuestionText = reader["QuestionText"].ToString(),
                                    Points = (int)reader["points"],
                                    Options = new List<QuizOption>()
                                };
                                questions.Add(currentQuestion);
                            }

                            // Check if options exist before adding
                            if (reader["optionID"] != DBNull.Value && currentQuestion != null)
                            {
                                currentQuestion.Options.Add(new QuizOption
                                {
                                    OptionID = (int)reader["optionID"],
                                    OptionText = reader["optionText"].ToString(),
                                    IsCorrect = (bool)reader["isCorrect"]
                                });
                            }
                        }

                        if (questions.Count == 0)
                        {
                            // Quiz not found or doesn't belong to this teacher
                            Response.Redirect("~/TeacherQuiz.aspx");
                            return;
                        }

                        lblQuizTitle.Text = quizTitle;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error loading quiz: " + ex.Message);
                        // TODO: Show error
                    }
                }
            }
            AllQuestions = questions;
        }

        // Inside ViewQuiz.aspx.cs
        private void DisplayQuestion(int questionIndex)
        {
            if (AllQuestions == null || questionIndex < 0 || questionIndex >= AllQuestions.Count)
            {
                return; // Index out of bounds
            }

            QuizQuestion q = AllQuestions[questionIndex];
            lblQuestionTag.Text = $"Question {questionIndex + 1}";
            lblQuestionProgress.Text = $"Question {questionIndex + 1} of {AllQuestions.Count}";
            lblQuestionText.Text = q.QuestionText;
            lblQuestionPoints.Text = $"({q.Points} Points)";

            decimal progressPercent = ((decimal)(questionIndex + 1) / AllQuestions.Count) * 100;
            pnlProgressBarFill.Style["width"] = $"{progressPercent:0.#}%";

            rblOptions.Items.Clear();
            char optionLetter = 'A';

            QuizOption correctOption = q.Options.FirstOrDefault(opt => opt.IsCorrect);
            int correctOptionId = (correctOption != null) ? correctOption.OptionID : -1;

            foreach (var opt in q.Options)
            {
                ListItem li = new ListItem();
                // Your li.Text is fine, but let's simplify for clarity
                li.Text = $"<span class=\"option-letter\">{optionLetter++}</span> {HttpUtility.HtmlEncode(opt.OptionText)}";
                li.Value = opt.OptionID.ToString();

                // --- Modifications for Preview ---
                if (opt.OptionID == correctOptionId)
                {
                    li.Selected = true; // This makes the radio button "checked"
                    li.Attributes["class"] = "correct-answer-highlight"; // This adds the CSS class
                }
                // --- End Modifications ---

                rblOptions.Items.Add(li);
            }

            // --- ADD THIS LINE ---
            // Disables the *entire* list at once, preserving the "checked" state
            rblOptions.Enabled = false;

            btnPrevious.Enabled = (questionIndex > 0);
            btnNext.Visible = (questionIndex < AllQuestions.Count - 1);

            // Hide the "Submit" button if it exists
            Button btnSubmit = (Button)FindControl("btnSubmit");
            if (btnSubmit != null)
            {
                btnSubmit.Visible = false;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int currentIndex = int.Parse(hfCurrentQuestionIndex.Value);
            currentIndex++;
            hfCurrentQuestionIndex.Value = currentIndex.ToString();
            DisplayQuestion(currentIndex);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            int currentIndex = int.Parse(hfCurrentQuestionIndex.Value);
            currentIndex--;
            hfCurrentQuestionIndex.Value = currentIndex.ToString();
            DisplayQuestion(currentIndex);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            AllQuestions = null; // Clear session
            Response.Redirect("TeacherQuiz.aspx"); // Use your teacher quiz list page
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Auth.aspx");
        }
    }
}