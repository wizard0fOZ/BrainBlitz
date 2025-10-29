using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
// Removed unused: System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BrainBlitz
{
    // Make sure this class name 'TeacherQuiz' matches the 'Inherits' attribute in your Quizzes.aspx file
    public partial class TeacherQuiz : System.Web.UI.Page
    {
        // Class-level variable to store the validated teacher ID
        private int CurrentTeacherId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Session Check ---
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Auth.aspx"); // Redirect if not a logged-in Teacher
                return; // Stop processing if redirecting
            }
            else
            {
                CurrentTeacherId = (int)Session["UserID"];
            }
            // --- End Session Check ---

            if (!IsPostBack)
            {
                LoadQuizzes(); // Load quizzes on initial page load
            }
        }

        // --- Data Loading ---
        private void LoadQuizzes(string searchTerm = "")
        {
            // Use the validated ID from the class variable
            int teacherId = CurrentTeacherId;
            if (teacherId <= 0)
            {
                lblErrorMessage.Text = "Could not verify teacher identity. Please log in again.";
                lblErrorMessage.Visible = true;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            DataTable dtQuizzes = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL Query joining Quizzes and Subjects, using subqueries for counts/averages
                // Removed QuizDuration
                string query = @"
                    SELECT
                        q.quizID AS QuizID,
                        q.title AS QuizTitle,
                        ISNULL(s.name, 'N/A') AS SubjectName,
                        -- Count questions efficiently
                        (SELECT COUNT(questionID) FROM Questions qu WHERE qu.quizID = q.quizID) AS QuestionCount,
                        -- Count attempts efficiently
                        (SELECT COUNT(attemptID) FROM QuizAttempts qa WHERE qa.quizID = q.quizID) AS AttemptCount,
                        -- Calculate average score efficiently (handle division by zero if no attempts)
                        ISNULL((SELECT AVG(CAST(score AS DECIMAL(5,2)))
                                FROM QuizAttempts qa_avg
                                WHERE qa_avg.quizID = q.quizID AND qa_avg.finishedAt IS NOT NULL), 0) AS AverageScore,
                        -- Determine status
                        CASE WHEN q.isPublished = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                    FROM
                        Quizzes q
                    LEFT JOIN
                        Subjects s ON q.subjectID = s.subjectID
                    WHERE
                        q.userID = @TeacherId"; // Filter quizzes CREATED BY this teacher

                // Add search filter if searchTerm is provided
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (q.title LIKE @SearchTerm OR s.name LIKE @SearchTerm)";
                }

                query += " ORDER BY q.quizID DESC"; // Show newest quizzes first


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
                        adapter.Fill(dtQuizzes); // Fill the DataTable with results
                    }
                    catch (SqlException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("SQL Error loading quizzes: " + ex.Message);
                        lblErrorMessage.Text = "Error loading quiz data. Please check database connection and table names.";
                        lblErrorMessage.Visible = true;
                    }
                    catch (Exception ex) // Catch other potential errors
                    {
                        System.Diagnostics.Debug.WriteLine("General Error loading quizzes: " + ex.Message);
                        lblErrorMessage.Text = "An unexpected error occurred while loading quizzes.";
                        lblErrorMessage.Visible = true;
                    }
                }
            } // using SqlConnection ensures the connection is closed

            // Bind the data to the Repeater
            rptQuizzes.DataSource = dtQuizzes;
            rptQuizzes.DataBind();

            // Handle visibility of "No quizzes found" message in the footer template
            // Find the panel within the Repeater's Controls collection AFTER DataBind()
            Panel pnlNoQuizzes = (Panel)rptQuizzes.Controls[rptQuizzes.Controls.Count - 1].FindControl("pnlNoQuizzes");
            if (pnlNoQuizzes != null)
            {
                pnlNoQuizzes.Visible = (dtQuizzes.Rows.Count == 0);
            }
        }

        // --- Event Handlers ---

        protected void txtSearchQuizzes_TextChanged(object sender, EventArgs e)
        {
            // Reload quizzes applying the search filter
            LoadQuizzes(txtSearchQuizzes.Text.Trim());
        }

        protected void btnCreateQuiz_Click(object sender, EventArgs e)
        {
            // Redirect to the Create Quiz page
            Response.Redirect("CreateQuiz.aspx");
        }

        // --- Header Button Handlers ---
        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx"); // Redirect to dashboard
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Implement full logout logic
            Session.Clear();
            Session.Abandon();
            // Consider FormsAuthentication.SignOut(); if using Forms Authentication
            Response.Redirect("Auth.aspx"); // Redirect to login page
        }

        // --- Repeater Command Handler (Placeholder Actions) ---
        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            string commandArgument = e.CommandArgument.ToString();

            if (e.CommandName == "ToggleStatus")
            {
                // CommandArgument is "QuizID,Status" (e.g., "10,Active")
                string[] args = commandArgument.Split(',');
                if (args.Length == 2)
                {
                    string quizIdStr = args[0];
                    string currentStatus = args[1];

                    // Determine the NEW status (toggle it)
                    bool newIsPublished = (currentStatus.ToLower() == "inactive");

                    ToggleQuizStatus(quizIdStr, newIsPublished); // Call the update method
                    LoadQuizzes(txtSearchQuizzes.Text.Trim()); // Refresh the list
                }
            }
            else if (e.CommandName == "View")
            {
                string quizId = commandArgument; // View/Edit/Delete only pass QuizID
                Response.Redirect($"ViewQuiz.aspx?quizID={quizId}");
            }
            else if (e.CommandName == "Edit")
            {
                string quizId = commandArgument;
                Response.Redirect($"EditQuiz.aspx?quizID={quizId}");
            }
            else if (e.CommandName == "Delete")
            {
                string quizId = commandArgument;
                DeleteQuiz(quizId);
                LoadQuizzes(txtSearchQuizzes.Text.Trim());
            }
        }

        // --- NEW METHOD to update the status in the DB ---
        private void ToggleQuizStatus(string quizId, bool newIsPublished)
        {
            // Ensure teacherId is valid (should be set in Page_Load)
            int teacherId = CurrentTeacherId;
            if (teacherId <= 0)
            {
                lblErrorMessage.Text = "Session error. Cannot update status.";
                lblErrorMessage.Visible = true;
                return;
            }

            lblErrorMessage.Text = ""; // Clear previous errors
            lblErrorMessage.Visible = false;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Update the isPublished status for the specific quiz owned by this teacher
                string query = @"UPDATE Quizzes
                             SET isPublished = @NewStatus
                             WHERE quizID = @QuizId AND userID = @TeacherId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NewStatus", newIsPublished); // bool is automatically converted (1 or 0)
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId); // Security check: Ensure teacher owns this quiz
                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // Quiz not found or doesn't belong to this teacher
                            lblErrorMessage.Text = "Could not update status. Quiz not found or permission denied.";
                            lblErrorMessage.Visible = true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Toggled status for Quiz ID: {quizId} to {newIsPublished}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error toggling quiz status: {ex.Message}");
                        lblErrorMessage.Text = "An error occurred while updating the quiz status.";
                        lblErrorMessage.Visible = true;
                    }
                }
            }
        }

        // --- Delete Logic (Requires careful implementation) ---
        private void DeleteQuiz(string quizId)
        {
            System.Diagnostics.Debug.WriteLine($"Attempting to delete Quiz ID: {quizId}. Requires DB implementation.");
            lblErrorMessage.Text = ""; // Clear previous errors
            lblErrorMessage.Visible = false;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    // Important: Delete child records first! Order matters due to Foreign Keys.
                    string deleteQuery = @"
                        -- Delete answers for attempts related to this quiz
                        DELETE FROM AttemptAnswers WHERE attemptID IN (SELECT attemptID FROM QuizAttempts WHERE quizID = @QuizId);
                        -- Delete attempts for this quiz
                        DELETE FROM QuizAttempts WHERE quizID = @QuizId;
                        -- Delete options for questions related to this quiz
                        DELETE FROM Options WHERE questionID IN (SELECT questionID FROM Questions WHERE quizID = @QuizId);
                        -- Delete questions for this quiz
                        DELETE FROM Questions WHERE quizID = @QuizId;
                        -- Finally delete the quiz itself (only if owned by current teacher for security)
                        DELETE FROM Quizzes WHERE quizID = @QuizId AND userID = @TeacherId;
                   ";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizId", quizId);
                        cmd.Parameters.AddWithValue("@TeacherId", CurrentTeacherId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        // Check rowsAffected if needed, especially the last delete to confirm ownership.
                        if (rowsAffected < 1) // Check if the final delete affected rows (meaning the quiz existed and belonged to the teacher)
                        {
                            // This might happen if the quiz didn't belong to the teacher or was already deleted
                            // Throw an exception to trigger rollback or handle appropriately
                            // throw new Exception("Quiz not found or you do not have permission to delete it.");
                            System.Diagnostics.Debug.WriteLine($"Delete affected {rowsAffected} rows for Quiz ID {quizId}. May indicate quiz not found or permission issue.");
                        }
                    }
                    transaction.Commit(); // Commit only if all deletes succeed
                    System.Diagnostics.Debug.WriteLine($"Successfully deleted Quiz ID: {quizId} and related data.");
                }
                catch (Exception ex)
                {
                    try { if (transaction.Connection != null) transaction.Rollback(); }
                    catch (Exception rbEx) { System.Diagnostics.Debug.WriteLine("Rollback Error during delete: " + rbEx.Message); }

                    System.Diagnostics.Debug.WriteLine("Error deleting quiz: " + ex.Message);
                    lblErrorMessage.Text = "Error deleting quiz. It might be in use or no longer exist.";
                    lblErrorMessage.Visible = true;
                }
            } // using SqlConnection
        }


        // --- Helper methods for CSS classes (Keep as they are) ---
        protected string GetSubjectTagClass(string subjectName)
        {
            switch (subjectName?.ToLower())
            {
                case "mathematics": return "tag tag-subject-math";
                case "science": return "tag tag-subject-science";
                case "english": return "tag tag-subject-english";
                default: return "tag tag-subject-default";
            }
        }

        protected string GetStatusTagClass(string status)
        {
            switch (status?.ToLower())
            {
                case "active": return "tag tag-status-active";
                case "inactive": return "tag tag-status-inactive";
                default: return "tag";
            }
        }

        protected string GetScoreClass(object averageScoreObj)
        {
            if (averageScoreObj == DBNull.Value || averageScoreObj == null) return "";

            decimal averageScoreDec;
            // Use TryParse with CultureInfo.InvariantCulture for reliable decimal parsing
            if (!decimal.TryParse(averageScoreObj.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out averageScoreDec)) return "";

            int score = (int)Math.Round(averageScoreDec);

            if (score >= 80) return "score-good";
            if (score >= 60) return "score-medium";
            return "score-bad";
        }

    }
}