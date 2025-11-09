using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class StudentProgress : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Student")
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindQuizzes();
            }
        }

        private int GetCurrentStudentId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void BindQuizzes()
        {
            int userId = GetCurrentStudentId();
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    q.quizID,
                    q.title,
                    ISNULL(s.name, 'General') AS SubjectName,
                    COUNT(*) AS AttemptCount,
                    MAX(qa.finishedAt) AS LastFinished
                FROM QuizAttempts qa
                INNER JOIN Quizzes q ON qa.quizID = q.quizID
                LEFT JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE qa.userID = @UserId AND qa.finishedAt IS NOT NULL
                GROUP BY q.quizID, q.title, s.name
                ORDER BY LastFinished DESC;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0)
            {
                pnlNoQuizzes.Visible = true;
            }
            else
            {
                pnlNoQuizzes.Visible = false;
            }

            // add css column and remember selected quiz
            dt.Columns.Add("ItemCss", typeof(string));
            int selectedQuizId = ViewState["SelectedQuizId"] == null ? -1 : (int)ViewState["SelectedQuizId"];

            foreach (DataRow row in dt.Rows)
            {
                int quizId = Convert.ToInt32(row["quizID"]);
                string css = "quiz-list-item";
                if (quizId == selectedQuizId)
                {
                    css += " selected";
                }
                row["ItemCss"] = css;
            }

            rptQuizzes.DataSource = dt;
            rptQuizzes.DataBind();

            // if we already had a selected quiz (postback from attempts), rebind attempts
            if (selectedQuizId > 0)
            {
                BindAttempts(selectedQuizId);
            }
        }

        private int GetQuizMaxScore(int quizId)
        {
            int maxScore = 0;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(points), 0)
                FROM Questions
                WHERE quizID = @QuizId;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    maxScore = Convert.ToInt32(result);
                }
            }

            if (maxScore == 0) maxScore = 1; // avoid divide by zero
            return maxScore;
        }

        private void BindAttempts(int quizId)
        {
            int userId = GetCurrentStudentId();
            int maxScore = GetQuizMaxScore(quizId);

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    qa.attemptID,
                    qa.score,
                    qa.finishedAt
                FROM QuizAttempts qa
                WHERE qa.userID = @UserId
                  AND qa.quizID = @QuizId
                  AND qa.finishedAt IS NOT NULL
                ORDER BY qa.finishedAt DESC;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            pnlNoQuizSelected.Visible = false;
            pnlAttempts.Visible = dt.Rows.Count > 0;

            dt.Columns.Add("ScorePercent", typeof(string));
            dt.Columns.Add("ScoreDisplay", typeof(string));
            dt.Columns.Add("ScoreClass", typeof(string));
            dt.Columns.Add("AttemptDate", typeof(string));
            dt.Columns.Add("AttemptTime", typeof(string));
            dt.Columns.Add("ItemCss", typeof(string));

            int selectedAttemptId = ViewState["SelectedAttemptId"] == null ? -1 : (int)ViewState["SelectedAttemptId"];

            foreach (DataRow row in dt.Rows)
            {
                int score = row["score"] == DBNull.Value ? 0 : Convert.ToInt32(row["score"]);
                double percent = (double)score / maxScore * 100.0;
                row["ScorePercent"] = Math.Round(percent, 0) + "%";
                row["ScoreDisplay"] = score + "/" + maxScore;

                row["ScoreClass"] = percent >= 70 ? "attempt-score good" : "attempt-score bad";

                if (row["finishedAt"] != DBNull.Value)
                {
                    DateTime finished = Convert.ToDateTime(row["finishedAt"]);
                    row["AttemptDate"] = finished.ToString("MMM dd, yyyy");
                    row["AttemptTime"] = finished.ToString("h:mm tt");
                }
                else
                {
                    row["AttemptDate"] = "";
                    row["AttemptTime"] = "";
                }

                int attemptId = Convert.ToInt32(row["attemptID"]);
                string css = "attempt-item";
                if (attemptId == selectedAttemptId)
                {
                    css += " selected";
                }
                row["ItemCss"] = css;
            }

            rptAttempts.DataSource = dt;
            rptAttempts.DataBind();

            // if a specific attempt is already selected, rebind its details
            if (selectedAttemptId > 0)
            {
                BindAttemptDetails(selectedAttemptId, maxScore);
            }
            else
            {
                pnlNoAttemptSelected.Visible = true;
                pnlAttemptDetails.Visible = false;
            }
        }

        private void BindAttemptDetails(int attemptId, int maxScore)
        {
            int userId = GetCurrentStudentId();

            int quizId = 0;
            int score = 0;

            // get quizId and score for this attempt
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT quizID, score
                FROM QuizAttempts
                WHERE attemptID = @AttemptId AND userID = @UserId;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        pnlNoAttemptSelected.Visible = true;
                        pnlAttemptDetails.Visible = false;
                        return;
                    }
                    quizId = Convert.ToInt32(reader["quizID"]);
                    score = reader["score"] == DBNull.Value ? 0 : Convert.ToInt32(reader["score"]);
                }
            }

            if (maxScore <= 0)
            {
                maxScore = GetQuizMaxScore(quizId);
            }

            double percent = (double)score / maxScore * 100.0;
            lblDetailScore.Text = score + "/" + maxScore;
            lblDetailAccuracy.Text = Math.Round(percent, 0) + "%";

            // now get question + options + student answers
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    q.questionID,
                    q.text AS QuestionText,
                    q.points,
                    o.optionID,
                    o.optionText,
                    o.isCorrect AS OptionIsCorrect,
                    aa.selectedOptionID,
                    aa.isCorrect AS AnswerIsCorrect
                FROM Questions q
                LEFT JOIN Options o ON o.questionID = q.questionID
                LEFT JOIN AttemptAnswers aa 
                    ON aa.questionID = q.questionID AND aa.attemptID = @AttemptId
                WHERE q.quizID = @QuizId
                ORDER BY q.questionID, o.optionID;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@AttemptId", attemptId);
                cmd.Parameters.AddWithValue("@QuizId", quizId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            StringBuilder sb = new StringBuilder();
            int currentQuestionId = -1;
            int questionIndex = 0;
            bool currentQuestionCorrect = false;
            int optionIndex = 0;

            foreach (DataRow row in dt.Rows)
            {
                int questionId = Convert.ToInt32(row["questionID"]);

                if (questionId != currentQuestionId)
                {
                    // close previous
                    if (currentQuestionId != -1)
                    {
                        sb.Append("</div>"); // close options container
                        sb.Append("</div>"); // close question block
                    }

                    currentQuestionId = questionId;
                    questionIndex++;
                    optionIndex = 0;

                    bool ansCorrect = row["AnswerIsCorrect"] != DBNull.Value && Convert.ToBoolean(row["AnswerIsCorrect"]);
                    currentQuestionCorrect = ansCorrect;

                    string blockClass = ansCorrect ? "question-block correct" : "question-block wrong";

                    sb.AppendFormat("<div class='{0}'>", blockClass);
                    sb.AppendFormat("<div class='question-text'>{0}. {1}</div>", questionIndex,
                        Server.HtmlEncode(row["QuestionText"].ToString()));
                    sb.Append("<div>"); // options container
                }

                if (row["optionID"] == DBNull.Value) continue;

                optionIndex++;
                int optionId = Convert.ToInt32(row["optionID"]);
                string optionText = row["optionText"] == DBNull.Value ? "" : row["optionText"].ToString();
                bool isCorrectOption = row["OptionIsCorrect"] != DBNull.Value && Convert.ToBoolean(row["OptionIsCorrect"]);

                bool hasSelected = row["selectedOptionID"] != DBNull.Value;
                bool isStudentChoice = hasSelected && optionId == Convert.ToInt32(row["selectedOptionID"]);

                char labelChar = (char)('A' + (optionIndex - 1));
                string optionClass = "option-row";
                if (isCorrectOption) optionClass += " correct";
                else if (isStudentChoice && !isCorrectOption) optionClass += " student";

                sb.AppendFormat("<div class='{0}'>", optionClass);
                sb.AppendFormat("<span class='option-label'>{0}.</span> {1}",
                    labelChar, Server.HtmlEncode(optionText));

                if (isCorrectOption)
                {
                    sb.Append("<span class='option-tag correct'>Correct</span>");
                }
                if (isStudentChoice && !isCorrectOption)
                {
                    sb.Append("<span class='option-tag student'>Your answer</span>");
                }
                sb.Append("</div>");
            }

            // close last question
            if (currentQuestionId != -1)
            {
                sb.Append("</div></div>");
            }

            litQuestionDetails.Text = sb.ToString();

            pnlNoAttemptSelected.Visible = false;
            pnlAttemptDetails.Visible = true;
        }

        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectQuiz")
            {
                int quizId = Convert.ToInt32(e.CommandArgument);
                ViewState["SelectedQuizId"] = quizId;
                ViewState["SelectedAttemptId"] = null;
                BindQuizzes();          // refresh left column highlighting
                BindAttempts(quizId);   // load attempts
            }
        }

        protected void rptAttempts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectAttempt")
            {
                int attemptId = Convert.ToInt32(e.CommandArgument);
                ViewState["SelectedAttemptId"] = attemptId;

                int quizId = ViewState["SelectedQuizId"] == null ? -1 : (int)ViewState["SelectedQuizId"];
                if (quizId > 0)
                {
                    // rebind attempts to update selected styling, then details
                    BindAttempts(quizId);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }
    }
}