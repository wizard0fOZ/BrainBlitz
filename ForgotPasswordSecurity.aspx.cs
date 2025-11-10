using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace BrainBlitz
{
    public partial class ForgotPasswordSecurity : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // nothing special yet
            }
        }


        protected void btnFindAccount_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            lblLookupMessage.Visible = false;
            lblResetMessage.Visible = false;
            pnlStep2.Visible = false;
            hfUserId.Value = string.Empty;

            string email = txtEmailLookup.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                ShowLookupError("Please enter your email.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT userID, securityQuestionId
                    FROM [Users]
                    WHERE email = @Email AND isActive = 1;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowLookupError("No active account found with that email.");
                            return;
                        }

                        if (reader["securityQuestionId"] == DBNull.Value)
                        {
                            ShowLookupError("This account does not have a security question set. Please contact an administrator.");
                            return;
                        }

                        int userId = Convert.ToInt32(reader["userID"]);
                        int questionId = Convert.ToInt32(reader["securityQuestionId"]);

                        hfUserId.Value = userId.ToString();
                        lblSecurityQuestion.Text = GetSecurityQuestionText(questionId);

                        pnlStep2.Visible = true;

                        txtEmailLookup.Enabled = false;
                        btnFindAccount.Enabled = false;
                    }
                }
            }
            catch
            {
                ShowLookupError("There was a problem looking up your account. Please try again later.");
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            lblResetMessage.Visible = false;

            if (string.IsNullOrEmpty(hfUserId.Value))
            {
                ShowResetError("Session expired. Please start the reset process again.");
                ResetToStep1();
                return;
            }

            int userId;
            if (!int.TryParse(hfUserId.Value, out userId))
            {
                ShowResetError("Invalid session. Please start again.");
                ResetToStep1();
                return;
            }

            string answer = (txtSecurityAnswer.Text ?? "").Trim();
            string newPassword = txtNewPassword.Text;

            if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(newPassword))
            {
                ShowResetError("Please fill in all required fields.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT securityAnswerHash
                    FROM [Users]
                    WHERE userID = @UserId AND isActive = 1;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        ShowResetError("Security information not found for this account.");
                        return;
                    }

                    string storedAnswerHash = result.ToString();
                    string answerHash = HashStringSHA256(answer.ToLowerInvariant());

                    if (!string.Equals(storedAnswerHash, answerHash, StringComparison.OrdinalIgnoreCase))
                    {
                        ShowResetError("Security answer is incorrect.");
                        return;
                    }

                    string newPasswordHash = HashPassword(newPassword);

                    using (SqlCommand updateCmd = new SqlCommand(@"
                        UPDATE [Users]
                        SET passwordHash = @PasswordHash
                        WHERE userID = @UserId;
                    ", conn))
                    {
                        updateCmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                        updateCmd.Parameters.AddWithValue("@UserId", userId);

                        int rows = updateCmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            lblResetMessage.Visible = true;
                            lblResetMessage.CssClass = "success-message";
                            lblResetMessage.Text = "Your password has been reset. You can now sign in with your new password.";

                            // Optionally clear fields
                            txtSecurityAnswer.Text = "";
                            txtNewPassword.Text = "";
                            txtConfirmPassword.Text = "";
                        }
                        else
                        {
                            ShowResetError("Failed to update password. Please try again.");
                        }
                    }
                }
            }
            catch
            {
                ShowResetError("There was a problem resetting your password. Please try again later.");
            }
        }


        private string GetSecurityQuestionText(int questionId)
        {
            switch (questionId)
            {
                case 1:
                    return "What is your favorite color?";
                case 2:
                    return "What city were you born in?";
                case 3:
                    return "What is your mother's maiden name?";
                case 4:
                    return "What was the name of your first pet?";
                default:
                    return "Security question";
            }
        }

        private void ShowLookupError(string message)
        {
            lblLookupMessage.Visible = true;
            lblLookupMessage.CssClass = "error-message";
            lblLookupMessage.Text = message;
        }

        private void ShowResetError(string message)
        {
            lblResetMessage.Visible = true;
            lblResetMessage.CssClass = "error-message";
            lblResetMessage.Text = message;
        }

        private void ResetToStep1()
        {
            pnlStep2.Visible = false;
            hfUserId.Value = "";
            txtEmailLookup.Enabled = true;
            btnFindAccount.Enabled = true;
        }

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

        private string HashStringSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
