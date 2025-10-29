using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace BrainBlitz
{
    public partial class Auth : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any previous messages
                ClearMessages();
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ClearMessages();

            string email = txtSignInEmail.Text.Trim();
            string password = txtSignInPassword.Text;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT userID, fullName, passwordHash, role FROM Users WHERE email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["passwordHash"].ToString();

                                // Verify password
                                if (VerifyPassword(password, storedHash))
                                {
                                    // Store user information in session
                                    Session["UserId"] = reader["userID"];
                                    Session["FullName"] = reader["fullName"];
                                    Session["Email"] = email;
                                    Session["Role"] = reader["role"];

                                    // Redirect to dashboard or home page
                                    Response.Redirect("Dashboard.aspx");
                                }
                                else
                                {
                                    lblSignInPasswordError.Text = "Invalid password";
                                }
                            }
                            else
                            {
                                lblSignInEmailError.Text = "Email not found";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblSignInPasswordError.Text = "An error occurred. Please try again.";
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Sign In Error: {ex.Message}");
            }
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ClearMessages();

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
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            lblSignUpEmailError.Text = "Email already exists";
                            return;
                        }
                    }

                    // Hash the password
                    string passwordHash = HashPassword(password);

                    // Insert new user
                    string insertQuery = @"INSERT INTO Users (fullName, email, passwordHash, role, createdAt) 
                                          VALUES (@FullName, @Email, @PasswordHash, @Role, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblSignUpSuccess.Text = "Account created successfully! Please sign in.";
                            lblSignUpSuccess.Style["display"] = "block";

                            // Clear form fields
                            txtFullName.Text = "";
                            txtSignUpEmail.Text = "";
                            txtSignUpPassword.Text = "";

                            // Optional: Auto switch to sign in form after a delay
                            ClientScript.RegisterStartupScript(this.GetType(), "SwitchToSignIn",
                                "setTimeout(function(){ toggleForm('signin'); }, 2000);", true);
                        }
                        else
                        {
                            lblSignUpPasswordError.Text = "Failed to create account. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblSignUpPasswordError.Text = "An error occurred. Please try again.";
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Sign Up Error: {ex.Message}");
            }
        }

        private void ClearMessages()
        {
            lblSignInSuccess.Text = "";
            lblSignInEmailError.Text = "";
            lblSignInPasswordError.Text = "";
            lblSignUpSuccess.Text = "";
            lblSignUpNameError.Text = "";
            lblSignUpEmailError.Text = "";
            lblSignUpPasswordError.Text = "";

            lblSignInSuccess.Style["display"] = "none";
            lblSignUpSuccess.Style["display"] = "none";
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string inputHash = HashPassword(password);
            return inputHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}