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
                // Check if user is already logged in
                if (Session["UserID"] != null)
                {
                    Response.Redirect("~/Dashboard.aspx");
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
                                       FROM Users 
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
                                        lblSignInMessage.Text = "Your account has been deactivated. Please contact support.";
                                        lblSignInMessage.CssClass = "error-message";
                                        lblSignInMessage.Visible = true;
                                        return;
                                    }

                                    string storedHash = reader.GetString(reader.GetOrdinal("passwordHash"));

                                    if (VerifyPassword(password, storedHash))
                                    {
                                        // Login successful
                                        Session["UserID"] = reader.GetInt32(reader.GetOrdinal("userID"));
                                        Session["FullName"] = reader.GetString(reader.GetOrdinal("fullName"));
                                        Session["Email"] = email;
                                        Session["Role"] = reader.GetString(reader.GetOrdinal("role"));

                                        Response.Redirect("~/Dashboard.aspx");
                                    }
                                    else
                                    {
                                        lblSignInMessage.Text = "Invalid email or password.";
                                        lblSignInMessage.CssClass = "error-message";
                                        lblSignInMessage.Visible = true;
                                    }
                                }
                                else
                                {
                                    lblSignInMessage.Text = "Invalid email or password.";
                                    lblSignInMessage.CssClass = "error-message";
                                    lblSignInMessage.Visible = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblSignInMessage.Text = "An error occurred. Please try again later.";
                    lblSignInMessage.CssClass = "error-message";
                    lblSignInMessage.Visible = true;

                    // Log the error (implement your logging mechanism)
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
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE email = @Email";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", email);
                            int count = (int)checkCmd.ExecuteScalar();

                            if (count > 0)
                            {
                                lblSignUpMessage.Text = "An account with this email already exists.";
                                lblSignUpMessage.CssClass = "error-message";
                                lblSignUpMessage.Visible = true;
                                return;
                            }
                        }

                        // Hash the password
                        string passwordHash = HashPassword(password);

                        // Insert new user
                        string insertQuery = @"INSERT INTO Users (email, passwordHash, fullName, role, isActive, createdAt) 
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

                            Response.Redirect("~/Dashboard.aspx");
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblSignUpMessage.Text = "An error occurred during registration. Please try again later.";
                    lblSignUpMessage.CssClass = "error-message";
                    lblSignUpMessage.Visible = true;

                    // Log the error (implement your logging mechanism)
                    System.Diagnostics.Debug.WriteLine($"Sign Up Error: {ex.Message}");
                }
            }
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