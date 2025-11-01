using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class Performance : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            else
            {
                CurrentTeacherId = (int)Session["UserID"];
            }

            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(async () => await LoadAllPerformanceData()));
            }
        }

        private async Task LoadAllPerformanceData(string searchTerm = "", int subjectFilterId = 0)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Load summary cards
                var totalStudentsTask = GetTotalStudents(connection, CurrentTeacherId, subjectFilterId);
                var classAverageTask = GetClassAverage(connection, CurrentTeacherId, subjectFilterId);
                var activeThisWeekTask = GetActiveThisWeek(connection, CurrentTeacherId, subjectFilterId);
                var totalAttemptsTask = GetTotalAttempts(connection, CurrentTeacherId, subjectFilterId);

                // Load main student list
                var studentListTask = GetStudentPerformanceList(connection, CurrentTeacherId, searchTerm, subjectFilterId);

                await Task.WhenAll(totalStudentsTask, classAverageTask, activeThisWeekTask, totalAttemptsTask, studentListTask);

                // Populate summary cards
                lblTotalStudents.Text = totalStudentsTask.Result.ToString();
                lblClassAverage.Text = classAverageTask.Result;
                lblActiveThisWeek.Text = activeThisWeekTask.Result.ToString();
                lblTotalAttempts.Text = totalAttemptsTask.Result.ToString();

                // Populate main list
                DataTable dt = studentListTask.Result;
                rptPerformance.DataSource = dt;
                rptPerformance.DataBind();

                // Find and set visibility of "No students" panel in the footer
                if (rptPerformance.Controls.Count > 0)
                {
                    RepeaterItem footer = rptPerformance.Controls[rptPerformance.Controls.Count - 1] as RepeaterItem;
                    if (footer != null && footer.ItemType == ListItemType.Footer)
                    {
                        Panel pnlNoStudents = footer.FindControl("pnlNoStudents") as Panel;
                        if (pnlNoStudents != null)
                        {
                            pnlNoStudents.Visible = (dt.Rows.Count == 0);
                        }
                    }
                }
            }
        }

        #region Summary Card Queries

        private async Task<int> GetTotalStudents(SqlConnection con, int teacherId, int subjectId)
        {
            string query = @"
                SELECT COUNT(DISTINCT se.userID)
                FROM SubjectEnrollments se
                JOIN Subjects s ON se.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId";
            if (subjectId > 0) query += " AND s.subjectID = @SubjectID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                if (subjectId > 0) cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                object result = await cmd.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        private async Task<string> GetClassAverage(SqlConnection con, int teacherId, int subjectId)
        {
            string query = @"
                WITH QuizTotalPoints AS (
                    SELECT quizID, SUM(points) AS TotalPoints FROM Questions GROUP BY quizID
                )
                SELECT ROUND(AVG(CASE WHEN qt.TotalPoints > 0 THEN (CAST(qa.score AS float) / qt.TotalPoints) * 100 ELSE 0 END), 2)
                FROM QuizAttempts qa
                JOIN Quizzes q ON qa.quizID = q.quizID
                JOIN Subjects s ON q.subjectID = s.subjectID
                JOIN QuizTotalPoints qt ON q.quizID = qt.quizID
                WHERE s.assignedTo = @TeacherId AND qa.finishedAt IS NOT NULL AND qt.TotalPoints > 0";
            if (subjectId > 0) query += " AND s.subjectID = @SubjectID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                if (subjectId > 0) cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                object result = await cmd.ExecuteScalarAsync();

                if (result == DBNull.Value || result == null)
                    return "N/A";

                double avgValue = Convert.ToDouble(result);
                return $"{avgValue:F2}%";
            }
        }

        private async Task<int> GetActiveThisWeek(SqlConnection con, int teacherId, int subjectId)
        {
            string query = @"
                SELECT COUNT(DISTINCT qa.userID)
                FROM QuizAttempts qa
                JOIN Quizzes q ON qa.quizID = q.quizID
                JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId AND qa.finishedAt >= DATEADD(day, -7, GETDATE())";
            if (subjectId > 0) query += " AND s.subjectID = @SubjectID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                if (subjectId > 0) cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                object result = await cmd.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        private async Task<int> GetTotalAttempts(SqlConnection con, int teacherId, int subjectId)
        {
            string query = @"
                SELECT COUNT(qa.attemptID)
                FROM QuizAttempts qa
                JOIN Quizzes q ON qa.quizID = q.quizID
                JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId";
            if (subjectId > 0) query += " AND s.subjectID = @SubjectID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                if (subjectId > 0) cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                object result = await cmd.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        #endregion

        #region Main List Query

        private async Task<DataTable> GetStudentPerformanceList(SqlConnection con, int teacherId, string searchTerm, int subjectId)
        {
            DataTable dt = new DataTable();
            string query = @"
                WITH StudentAttempts AS (
                    SELECT 
                        qa.userID,
                        qa.quizID,
                        qa.score,
                        qa.finishedAt,
                        s.name AS SubjectName,
                        (SELECT SUM(points) FROM Questions qn WHERE qn.quizID = q.quizID) AS MaxPoints
                    FROM QuizAttempts qa
                    JOIN Quizzes q ON qa.quizID = q.quizID
                    JOIN Subjects s ON q.subjectID = s.subjectID
                    WHERE s.assignedTo = @TeacherId AND qa.finishedAt IS NOT NULL
                        AND (@SubjectID = 0 OR s.subjectID = @SubjectID)
                ),
                StudentAggregates AS (
                    SELECT 
                        userID,
                        COUNT(quizID) AS QuizzesTaken,
                        ROUND(AVG(CASE WHEN MaxPoints > 0 THEN (CAST(score AS float) / MaxPoints) * 100 ELSE 0 END), 2) AS AvgPercentage,
                        MAX(finishedAt) AS LastActivityDate,
                        (SELECT TOP 1 sa.SubjectName FROM StudentAttempts sa WHERE sa.userID = StudentAttempts.userID ORDER BY sa.finishedAt DESC) AS LastSubjectName
                    FROM StudentAttempts
                    GROUP BY userID
                )
                SELECT 
                    u.userID,
                    u.fullName AS StudentName,
                    agg.QuizzesTaken,
                    agg.AvgPercentage,
                    agg.LastActivityDate,
                    agg.LastSubjectName
                FROM StudentAggregates agg
                JOIN [Users] u ON agg.userID = u.userID
                WHERE (@SearchTerm = '' OR u.fullName LIKE @SearchTerm)
                ORDER BY agg.LastActivityDate DESC;
            ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? "" : $"%{searchTerm}%");

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }

            // Add and format the TimeAgo column
            dt.Columns.Add("LastActivityTimeAgo", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if (row["LastActivityDate"] != DBNull.Value)
                {
                    row["LastActivityTimeAgo"] = FormatTimeAgo(DateTime.Now.Subtract((DateTime)row["LastActivityDate"]));
                }
                else
                {
                    row["LastActivityTimeAgo"] = "Never";
                }
            }
            return dt;
        }

        #endregion

        #region Event Handlers

        protected void txtSearchStudents_TextChanged(object sender, EventArgs e)
        {
            int subjectId = 0;
            RegisterAsyncTask(new PageAsyncTask(() => LoadAllPerformanceData(txtSearchStudents.Text.Trim(), subjectId)));
        }


        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            // TODO: Implement report exporting logic
            System.Diagnostics.Debug.WriteLine("Export button clicked.");
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

        #endregion

        #region Helper Methods

        private string FormatTimeAgo(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays > 1) return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
            if (timeSpan.TotalHours > 1) return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
            if (timeSpan.TotalMinutes > 1) return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
            return "Just now";
        }

        protected string GetSubjectTagClass(string subjectName)
        {
            string tag = "tag-subject-default";
            switch (subjectName?.ToLower())
            {
                case "mathematics":
                    tag = "tag-subject-math";
                    break;
                case "english":
                    tag = "tag-subject-english";
                    break;
                case "science":
                    tag = "tag-subject-science";
                    break;
                case "default":
                    tag = "tag-subject-default";
                    break;
            }
            return "tag " + tag;
        }

        protected string GetScoreClass(object avgPercentage)
        {
            if (avgPercentage == DBNull.Value || avgPercentage == null) return "score-bad";
            double score = Convert.ToDouble(avgPercentage);
            if (score >= 80) return "score-good";
            if (score >= 60) return "score-medium";
            return "score-bad";
        }

        #endregion
    }
}