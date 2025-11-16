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
    public partial class ViewQuiz : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;
        private int CurrentQuizId = 0;

        private List<QuizQuestion> AllQuestions
        {
            get { return (List<QuizQuestion>)Session["ViewQuiz_Questions_" + CurrentQuizId]; }
            set { Session["ViewQuiz_Questions_" + CurrentQuizId] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            CurrentTeacherId = (int)Session["UserID"];

            if (!int.TryParse(Request.QueryString["quizID"], out CurrentQuizId))
            {
                Response.Redirect("~/TeacherQuiz.aspx");
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
            hfCurrentQuestionIndex.Value = "0";

            LoadQuestionsFromDB();

            if (AllQuestions == null || AllQuestions.Count == 0)
            {
                Response.Redirect("~/TeacherQuiz.aspx");
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
                    LEFT JOIN Options o ON qn.questionID = o.questionID
                    WHERE q.quizID = @QuizID AND q.userID = @TeacherID
                    ORDER BY qn.questionID, o.optionID;
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@QuizID", CurrentQuizId);
                    cmd.Parameters.AddWithValue("@TeacherID", CurrentTeacherId);
                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        QuizQuestion currentQuestion = null;
                        int currentQuestionId = 0;
                        string quizTitle = "Quiz Preview";

                        while (reader.Read())
                        {
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
                            Response.Redirect("~/TeacherQuiz.aspx");
                            return;
                        }

                        lblQuizTitle.Text = quizTitle;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error loading quiz: " + ex.Message);
                    }
                }
            }
            AllQuestions = questions;
        }

        private void DisplayQuestion(int questionIndex)
        {
            if (AllQuestions == null || questionIndex < 0 || questionIndex >= AllQuestions.Count)
            {
                return;
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
                li.Text = $"<span class=\"option-letter\">{optionLetter++}</span> {HttpUtility.HtmlEncode(opt.OptionText)}";
                li.Value = opt.OptionID.ToString();

                if (opt.OptionID == correctOptionId)
                {
                    li.Selected = true;
                    li.Attributes["class"] = "correct-answer-highlight";
                }

                rblOptions.Items.Add(li);
            }

            rblOptions.Enabled = false;

            btnPrevious.Enabled = (questionIndex > 0);
            btnNext.Visible = (questionIndex < AllQuestions.Count - 1);
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
            AllQuestions = null;
            Response.Redirect("TeacherQuiz.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
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