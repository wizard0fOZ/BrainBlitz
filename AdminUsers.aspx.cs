using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class AdminUsers : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is admin
            if (Session["UserID"] == null || Session["Role"]?.ToString() != "Admin")
            {
                Response.Redirect("Auth.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindUsersGrid();
            }
        }

        private void BindUsersGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT userID, email, fullName, role, isActive, createdAt 
                                   FROM Users 
                                   ORDER BY createdAt DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading users: " + ex.Message, false);
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            // Prevent admin from modifying their own account
            if (userId == currentUserId)
            {
                ShowMessage("You cannot modify your own account.", false);
                return;
            }

            if (e.CommandName == "ToggleStatus")
            {
                ToggleUserStatus(userId);
            }
            else if (e.CommandName == "DeleteUser")
            {
                DeleteUser(userId);
            }
        }

        private void ToggleUserStatus(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Users 
                                   SET isActive = CASE WHEN isActive = 1 THEN 0 ELSE 1 END 
                                   WHERE userID = @UserID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("User status updated successfully.", true);
                        BindUsersGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to update user status.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error updating user status: " + ex.Message, false);
            }
        }

        private void DeleteUser(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Users WHERE userID = @UserID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("User deleted successfully.", true);
                        BindUsersGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to delete user.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting user: " + ex.Message, false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
            messageBox.Attributes["class"] = isSuccess ? "message-box message-success" : "message-box message-error";
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Auth.aspx");
        }
    }
}