using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz.Student
{
    public partial class TakeQuiz : System.Web.UI.Page
    {
        private int CurrentStudentId = -1;
        private int CurrentQuizId = 0;

        private List<QuizQuestion> AllQuestions
        {
            get { return (List<QuizQuestion>)Session["CurrentQuizQuestions"]; }
            set { Session["CurrentQuizQuestions"] = value; }
        }

        private Dictionary<int, int> UserAnswers
        {
            get
            {
                if (Session["CurrentQuizAnswers"] == null)
                {
                    Session["CurrentQuizAnswers"] = new Dictionary<int, int>();
                }
                return (Dictionary<int, int>)Session["CurrentQuizAnswers"];
            }
            set { Session["CurrentQuizAnswers"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Student")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            CurrentStudentId = (int)Session["UserID"];

            if (!int.TryParse(Request.QueryString["quizID"], out CurrentQuizId))
            {
                Response.Redirect("~/StudentQuiz.aspx");
                return;
            }

            if (!IsPostBack)
            {
                StartQuiz();
            }
        }

        private void StartQuiz()
        {
            AllQuestions = null;
            UserAnswers = new Dictionary<int, int>();
            hfCurrentQuestionIndex.Value = "0";

            LoadQuestionsFromDB();

            if (AllQuestions == null || AllQuestions.Count == 0)
            {
                // TODO: Add error message
                Response.Redirect("~/StudentQuiz.aspx");
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
                    JOIN Options o ON qn.questionID = o.questionID
                    WHERE q.quizID = @QuizID
                    ORDER BY qn.questionID, o.optionID;
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        QuizQuestion currentQuestion = null;
                        int currentQuestionId = 0;
                        string quizTitle = "Quiz";

                        while (reader.Read())
                        {
                            if (string.IsNullOrEmpty(quizTitle) && reader["QuizTitle"] != DBNull.Value)
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

                            currentQuestion.Options.Add(new QuizOption
                            {
                                OptionID = (int)reader["optionID"],
                                OptionText = reader["optionText"].ToString(),
                                IsCorrect = (bool)reader["isCorrect"]
                            });
                        }

                        lblQuizTitle.Text = quizTitle; // Set quiz title
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error loading quiz: " + ex.Message);
                        // TODO: Show error
                    }
                }
            }
            AllQuestions = questions; // Save all loaded questions to session
        }

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
            foreach (var opt in q.Options)
            {
                ListItem li = new ListItem();
                li.Text = $"<span class=\"option-letter\">{optionLetter++}</span> {HttpUtility.HtmlEncode(opt.OptionText)}";
                li.Value = opt.OptionID.ToString();
                rblOptions.Items.Add(li);
            }

            var answers = UserAnswers;
            if (answers.ContainsKey(q.QuestionID))
            {
                rblOptions.SelectedValue = answers[q.QuestionID].ToString();
            }

            btnPrevious.Enabled = (questionIndex > 0);
            btnNext.Visible = (questionIndex < AllQuestions.Count - 1);
            btnSubmit.Visible = (questionIndex == AllQuestions.Count - 1);
        }

        // --- Button Click Handlers ---

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            int currentIndex = int.Parse(hfCurrentQuestionIndex.Value);
            currentIndex++;
            hfCurrentQuestionIndex.Value = currentIndex.ToString();
            DisplayQuestion(currentIndex);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();
            int currentIndex = int.Parse(hfCurrentQuestionIndex.Value);
            currentIndex--;
            hfCurrentQuestionIndex.Value = currentIndex.ToString();
            DisplayQuestion(currentIndex);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveCurrentAnswer();

            int totalScore = 0;
            int maxScore = 0;
            var answers = UserAnswers;

            foreach (var q in AllQuestions)
            {
                maxScore += q.Points;
                if (answers.ContainsKey(q.QuestionID))
                {
                    int selectedOptionId = answers[q.QuestionID];
                    QuizOption correctOption = q.Options.FirstOrDefault(opt => opt.IsCorrect);
                    if (correctOption != null && correctOption.OptionID == selectedOptionId)
                    {
                        totalScore += q.Points;
                    }
                }
            }

            int newAttemptId = SaveQuizAttempt(CurrentQuizId, CurrentStudentId, totalScore, answers);

            if (newAttemptId > 0)
            {
                AllQuestions = null;
                UserAnswers = null;

                decimal percentage = (maxScore > 0) ? ((decimal)totalScore / maxScore) * 100 : 0;

                // Populate the modal labels
                lblModalQuizTitle.Text = lblQuizTitle.Text;
                lblModalPercentage.Text = $"{percentage:N0}%";
                lblModalScoreDisplay.Text = $"You scored {totalScore} / {maxScore}";

                // Show the modal
                pnlModalBackdrop.Visible = true;
            }
            else
            {
                // Show a simple alert on save failure
                string errorScript = "alert('Error: Your quiz attempt could not be saved. Please try again.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "QuizErrorScript", errorScript, true);
            }
        }

        protected void btnBackToQuizzes_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/StudentQuiz.aspx");
        }

        private void SaveCurrentAnswer()
        {
            if (!string.IsNullOrEmpty(rblOptions.SelectedValue))
            {
                int currentIndex = int.Parse(hfCurrentQuestionIndex.Value);
                if (currentIndex >= 0 && currentIndex < AllQuestions.Count)
                {
                    QuizQuestion q = AllQuestions[currentIndex];
                    int selectedOptionId = int.Parse(rblOptions.SelectedValue);
                    var answers = UserAnswers;
                    answers[q.QuestionID] = selectedOptionId;
                    UserAnswers = answers;
                }
            }
        }

        private int SaveQuizAttempt(int quizId, int studentId, int score, Dictionary<int, int> answers)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            int newAttemptId = -1;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1. Insert into QuizAttempts
                    string attemptQuery = @"INSERT INTO QuizAttempts (userID, quizID, finishedAt, score)
                                            OUTPUT INSERTED.attemptID
                                            VALUES (@UserID, @QuizID, GETDATE(), @Score)";

                    using (SqlCommand cmdAttempt = new SqlCommand(attemptQuery, con, transaction))
                    {
                        cmdAttempt.Parameters.AddWithValue("@UserID", studentId);
                        cmdAttempt.Parameters.AddWithValue("@QuizID", quizId);
                        cmdAttempt.Parameters.AddWithValue("@Score", score);
                        newAttemptId = (int)cmdAttempt.ExecuteScalar();
                    }

                    if (newAttemptId <= 0) throw new Exception("Failed to create QuizAttempt record.");

                    // 2. Insert into AttemptAnswers
                    string answerQuery = @"INSERT INTO AttemptAnswers (attemptID, questionID, selectedOptionID, isCorrect)
                                           VALUES (@AttemptID, @QuestionID, @SelectedOptionID, @IsCorrect)";

                    using (SqlCommand cmdAnswer = new SqlCommand(answerQuery, con, transaction))
                    {
                        foreach (var answer in answers)
                        {
                            int questionId = answer.Key;
                            int selectedOptionId = answer.Value;

                            QuizQuestion q = AllQuestions.FirstOrDefault(ques => ques.QuestionID == questionId);
                            bool isAnswerCorrect = false;
                            if (q != null)
                            {
                                QuizOption correctOption = q.Options.FirstOrDefault(opt => opt.IsCorrect);
                                if (correctOption != null && correctOption.OptionID == selectedOptionId)
                                {
                                    isAnswerCorrect = true;
                                }
                            }

                            cmdAnswer.Parameters.Clear();
                            cmdAnswer.Parameters.AddWithValue("@AttemptID", newAttemptId);
                            cmdAnswer.Parameters.AddWithValue("@QuestionID", questionId);
                            cmdAnswer.Parameters.AddWithValue("@SelectedOptionID", selectedOptionId);
                            cmdAnswer.Parameters.AddWithValue("@IsCorrect", isAnswerCorrect);
                            cmdAnswer.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return newAttemptId;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error saving attempt: " + ex.Message);
                    try { transaction.Rollback(); }
                    catch (Exception rbEx) { System.Diagnostics.Debug.WriteLine("Rollback failed: " + rbEx.Message); }
                    return -1;
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Auth.aspx");
        }
    }

    [Serializable]
    public class QuizQuestion
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public List<QuizOption> Options { get; set; }
    }

    [Serializable]
    public class QuizOption
    {
        public int OptionID { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}