using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization; // Added for TryParse

namespace BrainBlitz
{
    public partial class TeacherQuiz : System.Web.UI.Page
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
                LoadQuizzes();
            }
        }

        // --- Data Loading (FIXED) ---
        private void LoadQuizzes(string searchTerm = "")
        {
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
                // --- THIS QUERY IS NOW CORRECTED ---
                string query = @"
                    SELECT
                        q.quizID AS QuizID,
                        q.title AS QuizTitle,
                        ISNULL(s.name, 'N/A') AS SubjectName,
                        
                        (SELECT COUNT(questionID) FROM Questions qu WHERE qu.quizID = q.quizID) AS QuestionCount,
                        
                        (SELECT COUNT(attemptID) FROM QuizAttempts qa WHERE qa.quizID = q.quizID) AS AttemptCount,
                        
                        -- Get total possible points for the quiz
                        ISNULL((SELECT SUM(points) FROM Questions qu_pts WHERE qu_pts.quizID = q.quizID), 0) AS MaxPoints, 
                        
                        -- Get average *raw score* (points earned), not percentage
                        ISNULL((SELECT AVG(CAST(score AS DECIMAL(5,2)))
                                FROM QuizAttempts qa_avg
                                WHERE qa_avg.quizID = q.quizID AND qa_avg.finishedAt IS NOT NULL), 0) AS AverageScorePoints,
                        
                        -- Determine status
                        CASE WHEN q.isPublished = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                    FROM
                        Quizzes q
                    LEFT JOIN
                        Subjects s ON q.subjectID = s.subjectID
                    WHERE
                        q.userID = @TeacherId"; // Filter quizzes CREATED BY this teacher

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND (q.title LIKE @SearchTerm OR s.name LIKE @SearchTerm)";
                }

                query += " ORDER BY q.quizID DESC";

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
                        System.Diagnostics.Debug.WriteLine("SQL Error loading quizzes: " + ex.Message);
                        lblErrorMessage.Text = "Error loading quiz data. Please check database connection and table names.";
                        lblErrorMessage.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("General Error loading quizzes: " + ex.Message);
                        lblErrorMessage.Text = "An unexpected error occurred while loading quizzes.";
                        lblErrorMessage.Visible = true;
                    }
                }
            }

            rptQuizzes.DataSource = dtQuizzes;
            rptQuizzes.DataBind();

            // Find the panel within the Repeater's Controls collection AFTER DataBind()
            Panel pnlNoQuizzes = (Panel)rptQuizzes.Controls[rptQuizzes.Controls.Count - 1].FindControl("pnlNoQuizzes");
            if (pnlNoQuizzes != null)
            {
                pnlNoQuizzes.Visible = (dtQuizzes.Rows.Count == 0);
            }
        }

        // --- Event Handlers (No Change) ---
        protected void txtSearchQuizzes_TextChanged(object sender, EventArgs e)
        {
            LoadQuizzes(txtSearchQuizzes.Text.Trim());
        }

        protected void btnCreateQuiz_Click(object sender, EventArgs e)
        {
            Response.Redirect("CreateQuiz.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Auth.aspx");
        }

        protected void rptQuizzes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            string commandArgument = e.CommandArgument.ToString();

            if (e.CommandName == "ToggleStatus")
            {
                string[] args = commandArgument.Split(',');
                if (args.Length == 2)
                {
                    string quizIdStr = args[0];
                    string currentStatus = args[1];
                    bool newIsPublished = (currentStatus.ToLower() == "inactive");
                    ToggleQuizStatus(quizIdStr, newIsPublished);
                    LoadQuizzes(txtSearchQuizzes.Text.Trim());
                }
            }
            else if (e.CommandName == "View")
            {
                Response.Redirect($"ViewQuiz.aspx?quizID={commandArgument}");
            }
            else if (e.CommandName == "Edit")
            {
                Response.Redirect($"EditQuiz.aspx?quizID={commandArgument}");
            }
            else if (e.CommandName == "Delete")
            {
                DeleteQuiz(commandArgument);
                LoadQuizzes(txtSearchQuizzes.Text.Trim());
            }
        }

        private void ToggleQuizStatus(string quizId, bool newIsPublished)
        {
            // ... (your existing ToggleQuizStatus logic is correct) ...
            int teacherId = CurrentTeacherId;
            if (teacherId <= 0)
            {
                lblErrorMessage.Text = "Session error. Cannot update status.";
                lblErrorMessage.Visible = true;
                return;
            }
            lblErrorMessage.Text = "";
            lblErrorMessage.Visible = false;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Quizzes
                                 SET isPublished = @NewStatus
                                 WHERE quizID = @QuizId AND userID = @TeacherId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NewStatus", newIsPublished);
                    cmd.Parameters.AddWithValue("@QuizId", quizId);
                    cmd.Parameters.AddWithValue("@TeacherId", teacherId);
                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            lblErrorMessage.Text = "Could not update status. Quiz not found or permission denied.";
                            lblErrorMessage.Visible = true;
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

        private void DeleteQuiz(string quizId)
        {
            // ... (your existing DeleteQuiz logic is correct) ...
            System.Diagnostics.Debug.WriteLine($"Attempting to delete Quiz ID: {quizId}");
            lblErrorMessage.Text = "";
            lblErrorMessage.Visible = false;
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    string checkQuery = "SELECT COUNT(*) FROM Quizzes WHERE quizID = @QuizId AND userID = @TeacherId";
                    int quizExists = 0;
                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, con, transaction))
                    {
                        cmdCheck.Parameters.AddWithValue("@QuizId", quizId);
                        cmdCheck.Parameters.AddWithValue("@TeacherId", CurrentTeacherId);
                        quizExists = (int)cmdCheck.ExecuteScalar();
                    }
                    if (quizExists == 0)
                    {
                        transaction.Rollback();
                        lblErrorMessage.Text = "Quiz not found or you do not have permission to delete it.";
                        lblErrorMessage.Visible = true;
                        return;
                    }
                    string deleteAnswersQuery = "DELETE FROM AttemptAnswers WHERE attemptID IN (SELECT attemptID FROM QuizAttempts WHERE quizID = @QuizId)";
                    using (SqlCommand cmd = new SqlCommand(deleteAnswersQuery, con, transaction)) { cmd.Parameters.AddWithValue("@QuizId", quizId); cmd.ExecuteNonQuery(); }
                    string deleteAttemptsQuery = "DELETE FROM QuizAttempts WHERE quizID = @QuizId";
                    using (SqlCommand cmd = new SqlCommand(deleteAttemptsQuery, con, transaction)) { cmd.Parameters.AddWithValue("@QuizId", quizId); cmd.ExecuteNonQuery(); }
                    string deleteOptionsQuery = "DELETE FROM Options WHERE questionID IN (SELECT questionID FROM Questions WHERE quizID = @QuizId)";
                    using (SqlCommand cmd = new SqlCommand(deleteOptionsQuery, con, transaction)) { cmd.Parameters.AddWithValue("@QuizId", quizId); cmd.ExecuteNonQuery(); }
                    string deleteQuestionsQuery = "DELETE FROM Questions WHERE quizID = @QuizId";
                    using (SqlCommand cmd = new SqlCommand(deleteQuestionsQuery, con, transaction)) { cmd.Parameters.AddWithValue("@QuizId", quizId); cmd.ExecuteNonQuery(); }
                    string deleteQuizQuery = "DELETE FROM Quizzes WHERE quizID = @QuizId AND userID = @TeacherId";
                    using (SqlCommand cmd = new SqlCommand(deleteQuizQuery, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("@QuizId", quizId);
                        cmd.Parameters.AddWithValue("@TeacherId", CurrentTeacherId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { if (transaction.Connection != null) transaction.Rollback(); }
                    catch (Exception rbEx) { System.Diagnostics.Debug.WriteLine("Rollback Error during delete: " + rbEx.Message); }
                    System.Diagnostics.Debug.WriteLine("Error deleting quiz: " + ex.Message);
                    lblErrorMessage.Text = "Error deleting quiz: " + ex.Message;
                    lblErrorMessage.Visible = true;
                }
            }
        }

        // --- Helper methods for CSS classes ---
        protected string GetSubjectTagClass(string subjectName)
        {
            // ... (this method is correct) ...
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
            // ... (this method is correct) ...
            switch (status?.ToLower())
            {
                case "active": return "tag tag-status-active";
                case "inactive": return "tag tag-status-inactive";
                default: return "tag";
            }
        }

        // --- GetScoreClass (FIXED) ---
        // This method now takes the raw data and calculates the percentage
        protected string GetScoreClass(object pointsEarned, object maxPoints)
        {
            if (pointsEarned == DBNull.Value || maxPoints == DBNull.Value || pointsEarned == null || maxPoints == null)
            {
                return "score-bad"; // Default
            }

            try
            {
                decimal earned = Convert.ToDecimal(pointsEarned);
                decimal max = Convert.ToDecimal(maxPoints);

                if (max == 0) return "score-bad"; // Or a neutral class

                decimal percentage = (earned / max) * 100;
                int score = (int)Math.Round(percentage);

                if (score >= 80) return "score-good";
                if (score >= 60) return "score-medium";
                return "score-bad";
            }
            catch
            {
                return "score-bad"; // Default on error
            }
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