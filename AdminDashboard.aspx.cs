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
    public partial class AdminDashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                 if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                 {
                     Response.Redirect("Auth.aspx");
                     return;
                 }

                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            LoadSummaryCards();
            LoadUserOverview();
            LoadRecentUsers();
        }

        private void LoadSummaryCards()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Total Users
                    string queryUsers = "SELECT COUNT(*) FROM Users";
                    using (SqlCommand cmd = new SqlCommand(queryUsers, conn))
                    {
                        int totalUsers = (int)cmd.ExecuteScalar();
                        lblTotalStudents.Text = totalUsers.ToString();
                    }

                    // Total Subjects
                    string querySubjects = "SELECT COUNT(*) FROM Subjects";
                    using (SqlCommand cmd = new SqlCommand(querySubjects, conn))
                    {
                        int totalSubjects = (int)cmd.ExecuteScalar();
                        lblTotalSubjects.Text = totalSubjects.ToString();
                    }

                    // Total Resources
                    string queryResources = "SELECT COUNT(*) FROM Resources";
                    using (SqlCommand cmd = new SqlCommand(queryResources, conn))
                    {
                        int totalResources = (int)cmd.ExecuteScalar();
                        lblTotalResources.Text = totalResources.ToString();
                    }

                    // Total Quizzes
                    string queryQuizzes = "SELECT COUNT(*) FROM Quizzes";
                    using (SqlCommand cmd = new SqlCommand(queryQuizzes, conn))
                    {
                        int totalQuizzes = (int)cmd.ExecuteScalar();
                        lblTotalQuizzes.Text = totalQuizzes.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Log error or display message
                    System.Diagnostics.Debug.WriteLine("Error loading summary: " + ex.Message);
                    lblTotalStudents.Text = "0";
                    lblTotalSubjects.Text = "0";
                    lblTotalResources.Text = "0";
                    lblTotalQuizzes.Text = "0";
                }
            }
        }

        private void LoadUserOverview()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Students Count
                    string queryStudents = "SELECT COUNT(*) FROM Users WHERE role = 'Student'";
                    using (SqlCommand cmd = new SqlCommand(queryStudents, conn))
                    {
                        lblStudentsCount.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Active Students
                    string queryActiveStudents = "SELECT COUNT(*) FROM Users WHERE role = 'Student' AND isActive = 1";
                    using (SqlCommand cmd = new SqlCommand(queryActiveStudents, conn))
                    {
                        lblActiveStudents.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Teachers Count
                    string queryTeachers = "SELECT COUNT(*) FROM Users WHERE role = 'Teacher'";
                    using (SqlCommand cmd = new SqlCommand(queryTeachers, conn))
                    {
                        lblTeachersCount.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Active Teachers
                    string queryActiveTeachers = "SELECT COUNT(*) FROM Users WHERE role = 'Teacher' AND isActive = 1";
                    using (SqlCommand cmd = new SqlCommand(queryActiveTeachers, conn))
                    {
                        lblActiveTeachers.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Admins Count
                    string queryAdmins = "SELECT COUNT(*) FROM Users WHERE role = 'Admin'";
                    using (SqlCommand cmd = new SqlCommand(queryAdmins, conn))
                    {
                        lblAdminsCount.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Active Admins
                    string queryActiveAdmins = "SELECT COUNT(*) FROM Users WHERE role = 'Admin' AND isActive = 1";
                    using (SqlCommand cmd = new SqlCommand(queryActiveAdmins, conn))
                    {
                        lblActiveAdmins.Text = cmd.ExecuteScalar().ToString();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading user overview: " + ex.Message);
                    lblStudentsCount.Text = "0";
                    lblActiveStudents.Text = "0";
                    lblTeachersCount.Text = "0";
                    lblActiveTeachers.Text = "0";
                    lblAdminsCount.Text = "0";
                    lblActiveAdmins.Text = "0";
                }
            }
        }

        private void LoadRecentUsers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT TOP 5 
                                        fullName AS UserName, 
                                        email AS Email, 
                                        role AS RoleDisplay,
                                        createdAt,
                                        CASE 
                                            WHEN DATEDIFF(MINUTE, createdAt, GETDATE()) < 60 THEN 
                                                CAST(DATEDIFF(MINUTE, createdAt, GETDATE()) AS VARCHAR) + ' minutes ago'
                                            WHEN DATEDIFF(HOUR, createdAt, GETDATE()) < 24 THEN 
                                                CAST(DATEDIFF(HOUR, createdAt, GETDATE()) AS VARCHAR) + ' hours ago'
                                            WHEN DATEDIFF(DAY, createdAt, GETDATE()) < 30 THEN 
                                                CAST(DATEDIFF(DAY, createdAt, GETDATE()) AS VARCHAR) + ' days ago'
                                            ELSE 
                                                CAST(DATEDIFF(MONTH, createdAt, GETDATE()) AS VARCHAR) + ' months ago'
                                        END AS TimeAgo
                                     FROM Users 
                                     ORDER BY createdAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        rptRecentUsers.DataSource = dt;
                        rptRecentUsers.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading recent users: " + ex.Message);
                    // Bind empty data to show no records
                    rptRecentUsers.DataSource = new DataTable();
                    rptRecentUsers.DataBind();
                }
            }
        }
    }
}