using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks; // If loading data asynchronously
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BrainBlitz
{
    public partial class TeacherQuiz : System.Web.UI.Page // Ensure this matches your Inherits attribute
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load quizzes when the page first loads
                LoadQuizzes();
                // Or if using async: RegisterAsyncTask(new PageAsyncTask(LoadQuizzesAsync));
            }
        }

        // --- Data Loading ---
        private void LoadQuizzes(string searchTerm = "")
        {
            int teacherId = 1; // --- TODO: Get actual logged-in teacher ID ---
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            DataTable dtQuizzes = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // --- TODO: Write the actual SQL Query ---
                // This query needs to join Quizzes, Subjects, count Questions, count Attempts, calculate Avg Score
                string query = @"
                    SELECT
                        q.quizID AS QuizID,
                        q.title AS QuizTitle,
                        s.name AS SubjectName,
                        -- Placeholder for Duration (Add a column to Quizzes table?)
                        30 AS QuizDuration,
                        -- Count questions for this quiz
                        (SELECT COUNT(*) FROM Questions WHERE quizID = q.quizID) AS QuestionCount,
                        -- Count attempts for this quiz
                        (SELECT COUNT(*) FROM QuizAttempts WHERE quizID = q.quizID) AS AttemptCount,
                        -- Calculate average score for this quiz (handle no attempts)
                        ISNULL((SELECT AVG(CAST(score AS float)) FROM QuizAttempts WHERE quizID = q.quizID AND finishedAt IS NOT NULL), 0) AS AverageScore,
                        -- Determine status based on isPublished
                        CASE WHEN q.isPublished = 1 THEN 'Active' ELSE 'Draft' END AS Status
                    FROM
                        Quizzes q
                    LEFT JOIN
                        Subjects s ON q.subjectID = s.subjectID
                    WHERE
                        q.userID = @TeacherId"; // Filter by the logged-in teacher

                // Add search filter if searchTerm is provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (q.title LIKE @SearchTerm OR s.name LIKE @SearchTerm)";
                }

                query += " ORDER BY q.quizID DESC"; // Or order as needed


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeacherId", teacherId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    try
                    {
                        connection.Open();
                        adapter.Fill(dtQuizzes);
                    }
                    catch (SqlException ex)
                    {
                        // --- TODO: Add error handling (e.g., show message to user) ---
                        Console.WriteLine("SQL Error loading quizzes: " + ex.Message);
                    }
                }
            }

            rptQuizzes.DataSource = dtQuizzes;
            rptQuizzes.DataBind();
        }

        // --- Event Handlers ---

        protected void txtSearchQuizzes_TextChanged(object sender, EventArgs e)
        {
            // Reload quizzes with the search term
            LoadQuizzes(txtSearchQuizzes.Text.Trim());
        }


        protected void btnCreateQuiz_Click(object sender, EventArgs e)
        {
            // --- TODO: Redirect to the Create Quiz page ---
            Response.Redirect("CreateQuiz.aspx");
        }


        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx"); // Or your main dashboard page
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // --- TODO: Implement logout logic (clear session, redirect) ---
            Response.Redirect("Login.aspx"); // Redirect to login page
        }

        // --- Repeater Command Handler (for View, Edit, Delete buttons) ---
        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            string quizId = e.CommandArgument.ToString();

            if (e.CommandName == "View")
            {
                // --- TODO: Redirect to a Quiz Details/View page ---
                Response.Redirect($"ViewQuiz.aspx?quizID={quizId}");
            }
            else if (e.CommandName == "Edit")
            {
                // --- TODO: Redirect to an Edit Quiz page ---
                Response.Redirect($"EditQuiz.aspx?quizID={quizId}");
            }
            else if (e.CommandName == "Delete")
            {
                // --- TODO: Implement Delete logic ---
                DeleteQuiz(quizId);
                LoadQuizzes(txtSearchQuizzes.Text.Trim()); // Refresh list after delete
            }
        }

        private void DeleteQuiz(string quizId)
        {
            // --- TODO: Write SQL DELETE statements (handle related data like questions, attempts first!) ---
            // Example (Needs error handling and FK constraints considered):
            /*
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                 // Delete answers, attempts, options, questions, then quiz
                 string deleteQuery = @"
                      DELETE FROM AttemptAnswers WHERE attemptID IN (SELECT attemptID FROM QuizAttempts WHERE quizID = @QuizId);
                      DELETE FROM QuizAttempts WHERE quizID = @QuizId;
                      DELETE FROM Options WHERE questionID IN (SELECT questionID FROM Questions WHERE quizID = @QuizId);
                      DELETE FROM Questions WHERE quizID = @QuizId;
                      DELETE FROM Quizzes WHERE quizID = @QuizId;
                 ";
                 using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                 {
                      command.Parameters.AddWithValue("@QuizId", quizId);
                      connection.Open();
                      command.ExecuteNonQuery();
                 }
            }
            */
            System.Diagnostics.Debug.WriteLine($"Attempting to delete Quiz ID: {quizId}"); // Placeholder
        }


        // --- Helper methods for CSS classes ---
        protected string GetSubjectTagClass(string subjectName)
        {
            // Basic example - expand with more subjects
            switch (subjectName?.ToLower())
            {
                case "mathematics": return "tag tag-subject-math";
                case "physics": return "tag tag-subject-physics";
                default: return "tag tag-subject-default";
            }
        }

        protected string GetStatusTagClass(string status)
        {
            switch (status?.ToLower())
            {
                case "active": return "tag tag-status-active";
                case "draft": return "tag tag-status-draft";
                default: return "tag";
            }
        }

        protected string GetScoreClass(int score)
        {
            if (score >= 80) return "score-good";
            if (score >= 60) return "score-medium";
            return "score-bad";
        }

    }
}