using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class Auth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is already logged in and redirect if necessary
                if (Session["userID"] != null && Session["role"] != null)
                {
                    // Call the new redirection helper method
                    RedirectUserBasedOnRole(Session["Role"].ToString());
                }
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string email = txtSignInEmail.Text.Trim();
                string password = txtSignInPassword.Text;

                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"SELECT userID, passwordHash, fullName, role, isActive
                                         FROM [Users]
                                         WHERE email = @Email";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool isActive = reader.GetBoolean(reader.GetOrdinal("isActive"));
                                    if (!isActive)
                                    {
                                        ShowErrorMessage(lblSignInMessage, "Your account has been deactivated. Please contact support.");
                                        return;
                                    }

                                    string storedHash = reader.GetString(reader.GetOrdinal("passwordHash"));
                                    string role = reader.GetString(reader.GetOrdinal("role")); // Get the role

                                    if (VerifyPassword(password, storedHash))
                                    {
                                        // Login successful
                                        Session["UserID"] = reader.GetInt32(reader.GetOrdinal("userID"));
                                        Session["FullName"] = reader.GetString(reader.GetOrdinal("fullName"));
                                        Session["Email"] = email;
                                        Session["Role"] = role; // Store the role

                                        // --- REDIRECT BASED ON ROLE ---
                                        RedirectUserBasedOnRole(role);
                                        // --- END REDIRECT ---
                                    }
                                    else
                                    {
                                        ShowErrorMessage(lblSignInMessage, "Invalid email or password.");
                                    }
                                }
                                else
                                {
                                    ShowErrorMessage(lblSignInMessage, "Invalid email or password.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(lblSignInMessage, "An error occurred. Please try again later.");
                    System.Diagnostics.Debug.WriteLine($"Sign In Error: {ex.Message}");
                }
            }
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string fullName = txtFullName.Text.Trim();
                string email = txtSignUpEmail.Text.Trim();
                string password = txtSignUpPassword.Text;
                string role = ddlRole.SelectedValue;

                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Check if email already exists
                        string checkQuery = "SELECT COUNT(*) FROM [Users] WHERE email = @Email";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            int count = (int)checkCmd.ExecuteScalar();
                            if (count > 0)
                            {
                                ShowErrorMessage(lblSignUpMessage, "An account with this email already exists.");
                                return;
                            }
                        }

                        // Hash the password
                        string passwordHash = HashPassword(password);

                        // Insert new user
                        string insertQuery = @"INSERT INTO [Users] (email, passwordHash, fullName, role, isActive, createdAt)
                                               VALUES (@Email, @PasswordHash, @FullName, @Role, 1, GETDATE());
                                               SELECT CAST(SCOPE_IDENTITY() as int)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Email", email);
                            insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                            insertCmd.Parameters.AddWithValue("@FullName", fullName);
                            insertCmd.Parameters.AddWithValue("@Role", role);

                            int newUserID = (int)insertCmd.ExecuteScalar();

                            // Auto-login after successful registration
                            Session["UserID"] = newUserID;
                            Session["FullName"] = fullName;
                            Session["Email"] = email;
                            Session["Role"] = role;

                            // --- REDIRECT BASED ON ROLE ---
                            RedirectUserBasedOnRole(role);
                            // --- END REDIRECT ---
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(lblSignUpMessage, "An error occurred during registration.");
                    System.Diagnostics.Debug.WriteLine($"Sign Up Error: {ex.Message}");
                }
            }
        }

        // --- Helper Methods ---
        private void RedirectUserBasedOnRole(string role)
        {
            switch (role)
            {
                case "Teacher":
                    Response.Redirect("~/TeacherDashboard.aspx");
                    break;
                case "Student":
                    Response.Redirect("~/StudentDashboard.aspx");
                    break;
                case "Admin":
                    Response.Redirect("~/AdminDashboard.aspx");
                    break;
                default:
                    // Fallback for unknown roles or if role is null/empty
                    ShowErrorMessage(lblSignInMessage, "Invalid user role detected. Please contact support.");
                    Response.Redirect("~/Auth.aspx");
                    break;
            }
        }

        // Helper to show messages consistently
        private void ShowErrorMessage(Label labelControl, string message)
        {
            labelControl.Text = message;
            labelControl.CssClass = "error-message";
            labelControl.Visible = true;
        }


        // Hash password using SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Verify password against stored hash
        private bool VerifyPassword(string password, string storedHash)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
        }
    }
}