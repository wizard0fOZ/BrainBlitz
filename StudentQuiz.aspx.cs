using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz // Assuming this namespace
{
    public partial class StudentQuiz : System.Web.UI.Page // Assuming this class name
    {
        private int CurrentStudentId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- Session Check ---
            if (Session["UserID"] == null || Session["Role"] == null || Session["Role"].ToString() != "Student")
            {
                Response.Redirect("~/Auth.aspx");
                return;
            }
            else
            {
                CurrentStudentId = (int)Session["UserID"];
            }
            // --- End Session Check ---

            if (!IsPostBack)
            {
                LoadQuizzesForStudent(); // Load all quizzes initially
            }
        }

        // Updated method to load data (simplified)
        private void LoadQuizzesForStudent(string searchTerm = "")
        {
            if (CurrentStudentId <= 0) return;

            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            DataTable dtQuizzes = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT DISTINCT
                    q.quizID AS QuizID,
                    q.title AS QuizTitle,
                    q.difficulty AS Difficulty,
                    s.name AS SubjectName,
                    (SELECT COUNT(questionID) FROM Questions qu WHERE qu.quizID = q.quizID) AS QuestionCount,
                    
                    -- ADDED: Get TOTAL possible points for the quiz
                    ISNULL((SELECT SUM(points) FROM Questions qu_pts WHERE qu_pts.quizID = q.quizID), 0) AS MaxPoints,
                    
                    (SELECT COUNT(attemptID) 
                     FROM QuizAttempts qa 
                     WHERE qa.quizID = q.quizID AND qa.userID = @StudentID) AS AttemptCount,
                    
                    -- Renamed to make it clear this is points
                    ISNULL((SELECT MAX(score) 
                            FROM QuizAttempts qa_max 
                            WHERE qa_max.quizID = q.quizID AND qa_max.userID = @StudentID), 0) AS BestScorePoints
                FROM 
                    Quizzes q
                JOIN 
                    Subjects s ON q.subjectID = s.subjectID
                JOIN 
                    SubjectEnrollments se ON s.subjectID = se.subjectID
                WHERE 
                    q.isPublished = 1 
                    AND se.userID = @StudentID
            ";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND q.title LIKE @SearchTerm";
                }
                query += " ORDER BY s.name, q.title";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", CurrentStudentId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    try
                    {
                        con.Open();
                        sda.Fill(dtQuizzes);
                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = "Error loading quizzes. Please try again later.";
                        lblErrorMessage.Visible = true;
                        System.Diagnostics.Debug.WriteLine("Error loading student quizzes: " + ex.Message);
                    }
                }
            }

            rptStudentQuizzes.DataSource = dtQuizzes;
            rptStudentQuizzes.DataBind();
            pnlNoQuizzes.Visible = (dtQuizzes.Rows.Count == 0);
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

        // --- Event Handlers ---
        protected void txtSearchQuizzes_TextChanged(object sender, EventArgs e)
        {
            // Reload quizzes applying the search filter
            LoadQuizzesForStudent(txtSearchQuizzes.Text.Trim());
        }

        // --- REMOVED btnFilter_Click ---

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Auth.aspx");
        }

        // --- Helper Methods ---
        protected string GetSubjectTagClass(string subjectName)
        {
            // You can customize this
            switch (subjectName?.ToLower())
            {
                case "mathematics": return "tag tag-subject-math";
                case "science": return "tag tag-subject-science";
                case "english": return "tag tag-subject-english";
                default: return "tag tag-subject-default";
            }
        }

        protected string GetDifficultyTagClass(string difficulty)
        {
            switch (difficulty?.ToLower().Trim())
            {
                case "easy": return "tag tag-difficulty-easy";
                case "medium": return "tag tag-difficulty-medium";
                case "hard": return "tag tag-difficulty-hard";
                default: return "tag";
            }
        }
    }
}