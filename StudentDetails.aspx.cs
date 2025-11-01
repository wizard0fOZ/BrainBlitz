using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz.Teacher
{
    public partial class StudentDetails : System.Web.UI.Page
    {
        private int CurrentTeacherId = -1;
        private int CurrentStudentId = 0;

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

            // --- Get Student ID from URL ---
            if (!int.TryParse(Request.QueryString["studentID"], out CurrentStudentId) || CurrentStudentId <= 0)
            {
                Response.Redirect("~/Performance.aspx");
                return;
            }

            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(LoadStudentDetails));
            }
        }

        private async Task LoadStudentDetails()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Load student's name
                await LoadStudentName(connection);

                // Load the detailed attempts list
                await LoadStudentAttempts(connection);
            }
        }

        private async Task LoadStudentName(SqlConnection con)
        {
            // Query to get student's name (verify they are a student)
            string query = "SELECT fullName FROM [Users] WHERE userID = @StudentID AND role = 'Student'";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@StudentID", CurrentStudentId);
                object result = await cmd.ExecuteScalarAsync();
                if (result != null)
                {
                    lblStudentName.Text = result.ToString() + "'s Performance";
                }
                else
                {
                    lblStudentName.Text = "Student Not Found";
                    lblErrorMessage.Text = "Student not found or invalid permissions.";
                    lblErrorMessage.Visible = true;
                }
            }
        }

        private async Task LoadStudentAttempts(SqlConnection con)
        {
            DataTable dt = new DataTable();

            // Get all attempts for this student on quizzes taught by this teacher
            string query = @"
                WITH QuizTotalPoints AS (
                    SELECT 
                        quizID, 
                        SUM(points) AS TotalPoints
                    FROM Questions
                    GROUP BY quizID
                )
                SELECT 
                    q.quizID,
                    q.title AS QuizTitle,
                    s.name AS SubjectName,
                    qa.finishedAt AS FinishedAt,
                    qa.score AS PointsEarned,
                    ISNULL(qt.TotalPoints, 0) AS MaxPoints,
                    ROUND(CASE 
                        WHEN qt.TotalPoints > 0 THEN (CAST(qa.score AS float) / qt.TotalPoints) * 100
                        ELSE 0 
                    END, 2) AS Percentage
                FROM 
                    QuizAttempts qa
                JOIN 
                    Quizzes q ON qa.quizID = q.quizID
                JOIN 
                    Subjects s ON q.subjectID = s.subjectID
                LEFT JOIN 
                    QuizTotalPoints qt ON q.quizID = qt.quizID
                WHERE 
                    s.assignedTo = @TeacherID 
                    AND qa.userID = @StudentID
                    AND qa.finishedAt IS NOT NULL
                ORDER BY 
                    qa.finishedAt DESC;
            ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@TeacherID", CurrentTeacherId);
                cmd.Parameters.AddWithValue("@StudentID", CurrentStudentId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }

            // Bind data to repeater
            rptStudentAttempts.DataSource = dt;
            rptStudentAttempts.DataBind();

            // Find and set visibility of "No Attempts" panel in the footer
            if (rptStudentAttempts.Controls.Count > 0)
            {
                RepeaterItem footer = rptStudentAttempts.Controls[rptStudentAttempts.Controls.Count - 1] as RepeaterItem;
                if (footer != null && footer.ItemType == ListItemType.Footer)
                {
                    Panel pnlNoAttempts = footer.FindControl("pnlNoAttempts") as Panel;
                    if (pnlNoAttempts != null)
                    {
                        pnlNoAttempts.Visible = (dt.Rows.Count == 0);
                    }
                }
            }

            // Calculate and display summary stats
            if (dt.Rows.Count > 0)
            {
                lblTotalAttempts.Text = dt.Rows.Count.ToString();

                // Get unique quiz IDs
                var uniqueQuizzes = dt.AsEnumerable()
                    .Select(row => row.Field<int>("quizID"))
                    .Distinct()
                    .Count();
                lblQuizzesTaken.Text = uniqueQuizzes.ToString();

                // Calculate average percentage
                double avgPercentage = dt.AsEnumerable()
                    .Average(row => Convert.ToDouble(row["Percentage"]));
                lblOverallAverage.Text = $"{avgPercentage:F2}%";
            }
            else
            {
                lblTotalAttempts.Text = "0";
                lblQuizzesTaken.Text = "0";
                lblOverallAverage.Text = "N/A";
            }
        }

        #region Event Handlers

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Performance.aspx");
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

        protected string GetScoreClass(object percentage)
        {
            if (percentage == DBNull.Value || percentage == null)
                return "score-bad";

            double score = Convert.ToDouble(percentage);
            if (score >= 80) return "score-good";
            if (score >= 60) return "score-medium";
            return "score-bad";
        }

        #endregion
    }
}