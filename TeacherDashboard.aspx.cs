using System;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace BrainBlitz
{
    public partial class TeacherDashboard : System.Web.UI.Page
    {
        // Add this class-level variable
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
                // Store the ID for the rest of the page to use
                CurrentTeacherId = (int)Session["UserID"];
            }
            // --- End Session Check ---

            if (!IsPostBack && Session["FullName"] != null)
            {
                lblTeacherName.Text = "Welcome, " + Session["FullName"].ToString() + "."; 
            }

            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(LoadDashboardStats));
            }
        }

        private async Task LoadDashboardStats()
        {
            // Use the class-level variable
            int teacherId = CurrentTeacherId;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var studentsTask = GetTotalStudentsAsync(connection, teacherId);
                var quizzesTask = GetActiveQuizzesAsync(connection, teacherId);
                var resourcesTask = GetTotalResourcesAsync(connection, teacherId);
                var avgScoreTask = GetAverageScoreAsync(connection, teacherId); // <-- This is now fixed
                var subjectsTask = LoadSubjectsAsync(connection, teacherId);
                var activityTask = LoadRecentActivityAsync(connection, teacherId); // <-- This is now fixed

                await Task.WhenAll(studentsTask, quizzesTask, resourcesTask, avgScoreTask, subjectsTask, activityTask);

                lblTotalStudents.Text = studentsTask.Result.ToString();
                lblActiveQuizzes.Text = quizzesTask.Result.ToString();
                lblResources.Text = resourcesTask.Result.ToString();
                lblAverageScore.Text = avgScoreTask.Result + "%"; // Will now be a percentage

                rptSubjects.DataSource = subjectsTask.Result;
                rptSubjects.DataBind();

                rptRecentActivity.DataSource = activityTask.Result;
                rptRecentActivity.DataBind();
            }
        }

        // Card 1: Total Students (No change, this is correct)
        private async Task<int> GetTotalStudentsAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                SELECT COUNT(DISTINCT se.userID)
                FROM SubjectEnrollments se
                JOIN Subjects s ON se.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : (int)result;
            }
        }

        // Card 2: Active Quizzes (No change, this is correct)
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

        // Card 3: Resources (Still a placeholder)
        private async Task<int> GetTotalResourcesAsync(SqlConnection connection, int teacherId)
        {
            // TODO: Replace with real query when Resources table is ready
            string query = "SELECT COUNT(resourceID) FROM Resources WHERE userID = @TeacherId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeacherId", teacherId);
                object result = await command.ExecuteScalarAsync();
                return (result == DBNull.Value) ? 0 : (int)result;
            }
            // await Task.Delay(1);
            // return 0;
        }

        // Card 4: Average Score (FIXED)
        private async Task<string> GetAverageScoreAsync(SqlConnection connection, int teacherId)
        {
            // This query calculates the percentage for each attempt *first*, then averages them.
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
                    AND qt.TotalPoints > 0"; // Avoid division by zero

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
                    // Format the final average percentage to one decimal place
                    return ((double)result).ToString("N1");
                }
            }
        }

        // "My Subjects" List (FIXED)
        private async Task<DataTable> LoadSubjectsAsync(SqlConnection connection, int teacherId)
        {
            // This query now fetches MaxPoints and AverageScorePoints
            string query = @"
                SELECT 
                    s.name AS SubjectName,
                    (SELECT COUNT(q.quizID) 
                     FROM Quizzes q 
                     WHERE q.subjectID = s.subjectID AND q.userID = @TeacherId) AS QuizCount,
                     
                    (SELECT COUNT(DISTINCT se.userID) 
                     FROM SubjectEnrollments se 
                     WHERE se.subjectID = s.subjectID) AS StudentCount,
                     
                    -- TODO: Update with real query when Resources is ready
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

        // "Recent Activity" List (FIXED)
        private async Task<DataTable> LoadRecentActivityAsync(SqlConnection connection, int teacherId)
        {
            // 1. Update the Query to fetch MaxPoints
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
                    qa.score AS PointsEarned, -- Renamed for clarity
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

                // 2. Add new columns for calculated values
                dataTable.Columns.Add("TimeAgo", typeof(string));
                dataTable.Columns.Add("ScoreDisplay", typeof(string));
                dataTable.Columns.Add("ScoreClass", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime finishedAt = (DateTime)row["finishedAt"];

                    // 3. Use the helper method for calculation
                    // Call the static helper class
                    string percentageStr = CalculatePercentage(row["PointsEarned"], row["MaxPoints"]);
                    int percentageInt = 0;
                    int.TryParse(percentageStr.Replace("%", ""), out percentageInt); // Get the int value for color logic

                    row["TimeAgo"] = FormatTimeAgo(DateTime.Now.Subtract(finishedAt));
                    row["ScoreDisplay"] = percentageStr; // Use the formatted string (e.g., "75%")
                    row["ScoreClass"] = percentageInt >= 80 ? "activity-score activity-score-good" : "activity-score activity-score-bad";
                }

                return dataTable;
            }
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