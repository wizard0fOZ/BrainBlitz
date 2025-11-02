using System;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
// Removed unused: using System.Web.Security;
// Removed unused: using System.Globalization;

namespace BrainBlitz
{
    public partial class TeacherDashboard : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;

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

            if (!IsPostBack && Session["FullName"] != null)
            {
                lblTeacherName.Text = "Hello, " + Session["FullName"].ToString() + "!";
            }

            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(async () => await LoadDashboardStats()));
            }
        }

        private async Task LoadDashboardStats()
        {
            int teacherId = CurrentTeacherId;
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var studentsTask = GetTotalStudentsAsync(connection, teacherId);
                var quizzesTask = GetActiveQuizzesAsync(connection, teacherId);
                var resourcesTask = GetTotalResourcesAsync(connection, teacherId);
                var avgScoreTask = GetAverageScoreAsync(connection, teacherId);
                var subjectsTask = LoadSubjectsAsync(connection, teacherId);
                var activityTask = LoadRecentActivityAsync(connection, teacherId);

                await Task.WhenAll(studentsTask, quizzesTask, resourcesTask, avgScoreTask, subjectsTask, activityTask);

                lblTotalStudents.Text = studentsTask.Result.ToString();
                lblActiveQuizzes.Text = quizzesTask.Result.ToString();
                lblResources.Text = resourcesTask.Result.ToString();
                lblAverageScore.Text = avgScoreTask.Result; // This result is already formatted (e.g., "80.0" or "N/A")

                rptSubjects.DataSource = subjectsTask.Result;
                rptSubjects.DataBind();

                rptRecentActivity.DataSource = activityTask.Result;
                rptRecentActivity.DataBind();
            }
        }

        // Card 1: Total Students
        private async Task<int> GetTotalStudentsAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                SELECT COUNT(DISTINCT se.userID)
                FROM SubjectEnrollments se
                JOIN Subjects s ON se.subjectID = s.subjectID
                JOIN [Users] u ON se.userID = u.userID   -- Join User table
                WHERE s.assignedTo = @TeacherId
                  AND u.role = 'Student'"; // Filter for role

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : (int)result;
            }
        }

        // Card 2: Active Quizzes (This query is correct)
        private async Task<int> GetActiveQuizzesAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                SELECT COUNT(q.quizID) 
                FROM Quizzes q
                JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId AND q.isPublished = 1";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : (int)result;
            }
        }

        // Card 3: Resources (Updated to use correct table name)
        private async Task<int> GetTotalResourcesAsync(SqlConnection connection, int teacherId)
        {
            // Querying the Resources table
            string query = "SELECT COUNT(resourceID) FROM Resources WHERE userID = @TeacherId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : (int)result;
            }
        }

        // Card 4: Average Score (This query is correct for calculating avg percentage)
        private async Task<string> GetAverageScoreAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                WITH QuizTotalPoints AS (
                    SELECT 
                        quizID, 
                        SUM(points) AS TotalPoints
                    FROM Questions
                    GROUP BY quizID
                )
                SELECT 
                    AVG( 
                        CASE 
                            WHEN qt.TotalPoints > 0 THEN (CAST(qa.score AS float) / qt.TotalPoints) * 100
                            ELSE 0 
                        END
                    ) 
                FROM 
                    QuizAttempts qa
                JOIN 
                    Quizzes q ON qa.quizID = q.quizID
                JOIN 
                    Subjects s ON q.subjectID = s.subjectID
                JOIN 
                    QuizTotalPoints qt ON q.quizID = qt.quizID
                WHERE 
                    s.assignedTo = @TeacherId 
                    AND qa.finishedAt IS NOT NULL 
                    AND qt.TotalPoints > 0";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();

                if (result == DBNull.Value)
                {
                    return "N/A";
                }
                else
                {
                    return ((double)result).ToString("N2") + "%";
                }
            }
        }

        // "My Subjects" List (FIXED: StudentCount query now filters for 'Student' role)
        private async Task<DataTable> LoadSubjectsAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                SELECT 
                    s.name AS SubjectName,
                    (SELECT COUNT(q.quizID) 
                     FROM Quizzes q 
                     WHERE q.subjectID = s.subjectID AND q.userID = @TeacherId) AS QuizCount,
                    
                    (SELECT COUNT(DISTINCT se.userID) 
                     FROM SubjectEnrollments se
                     JOIN [Users] u ON se.userID = u.userID -- Add join
                     WHERE se.subjectID = s.subjectID
                       AND u.role = 'Student') AS StudentCount, -- Add filter
                     
                    (SELECT COUNT(r.resourceID) FROM Resources r WHERE r.subjectID = s.subjectID AND r.userID = @TeacherId) AS ResourceCount
                FROM 
                    Subjects s
                WHERE
                    s.assignedTo = @TeacherId
            ";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        // "Recent Activity" List (FIXED: Uses FormattingHelpers.CalculatePercentage)
        private async Task<DataTable> LoadRecentActivityAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                WITH QuizTotalPoints AS (
                    SELECT 
                        quizID, 
                        SUM(points) AS TotalPoints
                    FROM Questions
                    GROUP BY quizID
                )
                SELECT TOP 3
                    u.fullName AS StudentName,
                    q.title AS QuizTitle,
                    qa.finishedAt,
                    qa.score AS PointsEarned,
                    ISNULL(qt.TotalPoints, 0) AS MaxPoints
                FROM 
                    QuizAttempts qa
                JOIN 
                    [Users] u ON qa.userID = u.userID
                JOIN
                    Quizzes q ON qa.quizID = q.quizID
                JOIN
                    Subjects s ON q.subjectID = s.subjectID
                LEFT JOIN 
                    QuizTotalPoints qt ON q.quizID = qt.quizID
                WHERE 
                    s.assignedTo = @TeacherId AND qa.finishedAt IS NOT NULL
                ORDER BY 
                    qa.finishedAt DESC
            ";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataTable.Columns.Add("TimeAgo", typeof(string));
                dataTable.Columns.Add("ScoreDisplay", typeof(string));
                dataTable.Columns.Add("ScoreClass", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime finishedAt = (DateTime)row["finishedAt"];

                    // Call the static helper class
                    string percentageStr = CalculatePercentage(row["PointsEarned"], row["MaxPoints"]);
                    int.TryParse(percentageStr.Replace("%", ""), out int percentageInt);

                    row["TimeAgo"] = FormatTimeAgo(DateTime.Now.Subtract(finishedAt));
                    row["ScoreDisplay"] = percentageStr; // Use the formatted string (e.g., "75%")
                    row["ScoreClass"] = percentageInt >= 80 ? "activity-score activity-score-good" : "activity-score activity-score-bad";
                }

                return dataTable;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Auth.aspx");
        }
        // Helper function for formatting time
        private string FormatTimeAgo(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays > 1)
            {
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalHours > 1)
            {
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
            }
            if (timeSpan.TotalMinutes > 1)
            {
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
            }
            return "Just now";
        }
        protected string CalculatePercentage(object pointsEarned, object maxPoints)
        {
            // Check for null or DBNull values
            if (pointsEarned == DBNull.Value || maxPoints == DBNull.Value || pointsEarned == null || maxPoints == null)
            {
                return "N/A";
            }

            try
            {
                decimal earned = Convert.ToDecimal(pointsEarned);
                decimal max = Convert.ToDecimal(maxPoints);

                if (max == 0)
                {
                    return "N/A"; // Avoid division by zero
                }

                decimal percentage = (earned / max) * 100;
                // "N0" formats as a whole number (e.g., 75)
                return $"{percentage:N0}%";
            }
            catch (Exception)
            {
                return "N/A"; // In case of any conversion error
            }
        }
    }
}