using System;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data; // You need this for DataTable

namespace BrainBlitz
{
    public partial class TeacherDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(LoadDashboardStats));
            }
        }

        private async Task LoadDashboardStats()
        {
            int teacherId = 1; // FOR TESTING: Hardcoding teacher's userID
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Setup all tasks to run in parallel
                var studentsTask = GetTotalStudentsAsync(connection, teacherId);
                var quizzesTask = GetActiveQuizzesAsync(connection, teacherId);
                var resourcesTask = GetTotalResourcesAsync(connection, teacherId);
                var avgScoreTask = GetAverageScoreAsync(connection, teacherId);
                var subjectsTask = LoadSubjectsAsync(connection, teacherId);
                var activityTask = LoadRecentActivityAsync(connection, teacherId);

                // Wait for them all to finish
                await Task.WhenAll(studentsTask, quizzesTask, resourcesTask, avgScoreTask, subjectsTask, activityTask);

                // Assign results for summary cards
                lblTotalStudents.Text = studentsTask.Result.ToString();
                lblActiveQuizzes.Text = quizzesTask.Result.ToString();
                lblResources.Text = resourcesTask.Result.ToString();
                lblAverageScore.Text = avgScoreTask.Result;

                // Bind results for the new lists
                rptSubjects.DataSource = subjectsTask.Result;
                rptSubjects.DataBind();

                rptRecentActivity.DataSource = activityTask.Result;
                rptRecentActivity.DataBind();
            }
        }

        // Card 1: Total Students (UPDATED to new, better logic)
        private async Task<int> GetTotalStudentsAsync(SqlConnection connection, int teacherId)
        {
            // NEW LOGIC: Count students enrolled in subjects this teacher is assigned to.
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

        // Card 2: Active Quizzes (UPDATED to use assignedTo)
        private async Task<int> GetActiveQuizzesAsync(SqlConnection connection, int teacherId)
        {
            // Counts published quizzes in subjects this teacher is assigned to.
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
            // This will be 0 until your teammate builds the Resources table.
            await Task.Delay(1);
            return 0;
        }

        // Card 4: Average Score (UPDATED to use assignedTo)
        private async Task<string> GetAverageScoreAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
                SELECT AVG(CAST(qa.score AS float)) 
                FROM QuizAttempts qa
                JOIN Quizzes q ON qa.quizID = q.quizID
                JOIN Subjects s ON q.subjectID = s.subjectID
                WHERE s.assignedTo = @TeacherId AND qa.finishedAt IS NOT NULL";

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
                    return ((double)result).ToString("N1");
                }
            }
        }

        // NEW FUNCTION for "My Subjects"
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
                     WHERE se.subjectID = s.subjectID) AS StudentCount,
                     
                    0 AS ResourceCount -- Placeholder for Resources
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

                // SqlDataAdapter doesn't have a true async fill, so we run it synchronously 
                // within our async method. This is fine.
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        // NEW FUNCTION for "Recent Activity"
        // NEW FUNCTION for "Recent Activity"
        private async Task<DataTable> LoadRecentActivityAsync(SqlConnection connection, int teacherId)
        {
            string query = @"
        SELECT TOP 3
            u.fullName AS StudentName,
            q.title AS QuizTitle,
            qa.finishedAt,
            qa.score
        FROM 
            QuizAttempts qa
        JOIN 
            [User] u ON qa.userID = u.userID  -- <-- THIS IS THE FIX
        JOIN
            Quizzes q ON qa.quizID = q.quizID
        JOIN
            Subjects s ON q.subjectID = s.subjectID
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
                adapter.Fill(dataTable); // This is Line 190 where the error happened

                // Add new columns to format the data for the web page
                dataTable.Columns.Add("TimeAgo", typeof(string));
                dataTable.Columns.Add("ScoreDisplay", typeof(string));
                dataTable.Columns.Add("ScoreClass", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime finishedAt = (DateTime)row["finishedAt"];
                    int score = (int)row["score"];

                    row["TimeAgo"] = FormatTimeAgo(DateTime.Now.Subtract(finishedAt));
                    row["ScoreDisplay"] = $"{score}%";
                    row["ScoreClass"] = score >= 80 ? "activity-score activity-score-good" : "activity-score activity-score-bad";
                }

                return dataTable;
            }
        }

        // NEW HELPER FUNCTION for formatting time
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
    }
}