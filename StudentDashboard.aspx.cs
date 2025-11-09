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
    public partial class WebForm2 : System.Web.UI.Page
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
                LoadStudentName();
                LoadSummaryCards();
                BindRecentQuizzes();
                BindBookmarkedResources(); 
            }
        }

        private int GetCurrentStudentId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void LoadStudentName()
        {
            int userId = GetCurrentStudentId();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT fullName 
                FROM Users
                WHERE userID = @UserId;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                object result = cmd.ExecuteScalar();
                string name = result == null || result == DBNull.Value ? "Student" : result.ToString();
                lblStudentName.Text = "Welcome back, " + name + "!";
            }
        }

        private void LoadSummaryCards()
        {
            int userId = GetCurrentStudentId();

            int completedQuizzes = 0;
            double avgScore = 0;
            int totalAttempts = 0;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    COUNT(DISTINCT CASE WHEN finishedAt IS NOT NULL THEN quizID END) AS CompletedQuizzes,
                    AVG(CAST(score AS FLOAT)) AS AvgScore,
                    COUNT(*) AS TotalAttempts
                FROM QuizAttempts
                WHERE userID = @UserId AND finishedAt IS NOT NULL;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader["CompletedQuizzes"] != DBNull.Value)
                            completedQuizzes = Convert.ToInt32(reader["CompletedQuizzes"]);
                        if (reader["AvgScore"] != DBNull.Value)
                            avgScore = Convert.ToDouble(reader["AvgScore"]);
                        if (reader["TotalAttempts"] != DBNull.Value)
                            totalAttempts = Convert.ToInt32(reader["TotalAttempts"]);
                    }
                }
            }

            lblQuizzesCompleted.Text = completedQuizzes.ToString();
            lblAverageScore.Text = avgScore > 0 ? Math.Round(avgScore, 0).ToString() + "%" : "0%";

            // simple trend placeholders for now
            lblQuizzesTrend.Text = completedQuizzes > 0 ? "+ keep going!" : "No quizzes yet";
            lblAverageTrend.Text = avgScore > 0 ? "Based on completed quizzes" : "";

            // streak calculation based on days with completed attempts
            int streakDays = CalculateStreakDays(userId);
            lblStreak.Text = streakDays + " day" + (streakDays == 1 ? "" : "s");
            lblStreakInfo.Text = streakDays > 0 ? "Consecutive days with activity" : "";

            // hours studied approximation: 0.5h per attempt
            double hours = totalAttempts * 0.5;
            lblHoursStudied.Text = hours.ToString("0.0");
            lblHoursInfo.Text = totalAttempts > 0 ? "Approximate (0.5h per quiz)" : "";
        }

        private int CalculateStreakDays(int userId)
        {
            HashSet<DateTime> dates = new HashSet<DateTime>();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT CONVERT(date, finishedAt) AS d
                FROM QuizAttempts
                WHERE userID = @UserId
                  AND finishedAt IS NOT NULL
                  AND finishedAt >= DATEADD(day, -30, GETDATE());
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["d"] != DBNull.Value)
                        {
                            DateTime day = Convert.ToDateTime(reader["d"]).Date;
                            if (!dates.Contains(day))
                                dates.Add(day);
                        }
                    }
                }
            }

            int streak = 0;
            DateTime current = DateTime.Today;

            while (dates.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }

            return streak;
        }

        private void BindRecentQuizzes()
        {
            int userId = GetCurrentStudentId();

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 3
                    qa.quizID,
                    q.title AS QuizTitle,
                    s.name AS SubjectName,
                    qa.score,
                    qa.finishedAt
                FROM QuizAttempts qa
                INNER JOIN Quizzes q ON qa.quizID = q.quizID
                LEFT JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE qa.userID = @UserId
                  AND qa.finishedAt IS NOT NULL
                ORDER BY qa.finishedAt DESC;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            dt.Columns.Add("ScoreDisplay", typeof(string));
            dt.Columns.Add("ScoreClass", typeof(string));
            dt.Columns.Add("TimeAgo", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                int score = row["score"] == DBNull.Value ? 0 : Convert.ToInt32(row["score"]);
                row["ScoreDisplay"] = score.ToString() + "%";
                row["ScoreClass"] = score >= 80 ? "quiz-score-good" : "quiz-score-bad";

                if (row["finishedAt"] != DBNull.Value)
                {
                    DateTime finished = Convert.ToDateTime(row["finishedAt"]);
                    row["TimeAgo"] = FormatTimeAgo(finished);
                }
                else
                {
                    row["TimeAgo"] = "";
                }
            }

            rptRecentQuizzes.DataSource = dt;
            rptRecentQuizzes.DataBind();
        }

        private string FormatTimeAgo(DateTime dateTime)
        {
            TimeSpan diff = DateTime.Now - dateTime;

            if (diff.TotalMinutes < 1) return "Just now";
            if (diff.TotalHours < 1) return ((int)diff.TotalMinutes) + " minutes ago";
            if (diff.TotalDays < 1) return ((int)diff.TotalHours) + " hours ago";
            if (diff.TotalDays < 7) return ((int)diff.TotalDays) + " days ago";

            int weeks = (int)(diff.TotalDays / 7);
            return weeks + (weeks == 1 ? " week ago" : " weeks ago");
        }

        private void BindBookmarkedResources()
        {
            int userId = GetCurrentStudentId();

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 3
            r.resourceID,
            r.title AS Title,
            r.type AS Type,
            s.name AS SubjectName,
            b.createdAt
        FROM Bookmarks b
        INNER JOIN Resources r ON b.resourceID = r.resourceID
        LEFT JOIN Subjects s ON r.subjectID = s.subjectID
        WHERE b.userID = @UserId
        ORDER BY b.createdAt DESC;
    ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            // add calculated column for "X days ago"
            dt.Columns.Add("BookmarkedAgo", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if (row["createdAt"] != DBNull.Value)
                {
                    DateTime ts = Convert.ToDateTime(row["createdAt"]);
                    row["BookmarkedAgo"] = FormatTimeAgo(ts);
                }
                else
                {
                    row["BookmarkedAgo"] = "";
                }
            }

            rptRecommendedResources.DataSource = dt;
            rptRecommendedResources.DataBind();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }
    }
}