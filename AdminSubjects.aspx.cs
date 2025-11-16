using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class AdminSubjects : System.Web.UI.Page
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
                BindSubjectsGrid();
            }
        }

        private void BindSubjectsGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT subjectID, name, description 
                                   FROM Subjects 
                                   ORDER BY name ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSubjects.DataSource = dt;
                    gvSubjects.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading subjects: " + ex.Message, false);
            }
        }

        protected void btnCreateSubject_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string subjectName = txtSubjectName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(subjectName))
            {
                ShowMessage("Please enter a subject name.", false);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Check if subject already exists
                    string checkQuery = "SELECT COUNT(*) FROM Subjects WHERE name = @Name";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Name", subjectName);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ShowMessage("A subject with this name already exists.", false);
                        return;
                    }

                    // Insert new subject
                    string insertQuery = @"INSERT INTO Subjects (name, description) 
                                         VALUES (@Name, @Description)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Name", subjectName);
                    insertCmd.Parameters.AddWithValue("@Description",
                        string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Subject created successfully!", true);

                        // Clear form
                        txtSubjectName.Text = string.Empty;
                        txtDescription.Text = string.Empty;

                        // Refresh grid
                        BindSubjectsGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to create subject.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error creating subject: " + ex.Message, false);
            }
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSubject")
            {
                int subjectId = Convert.ToInt32(e.CommandArgument);
                DeleteSubject(subjectId);
            }
        }

        private void DeleteSubject(int subjectId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Subjects WHERE subjectID = @SubjectID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Subject deleted successfully.", true);
                        BindSubjectsGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to delete subject.", false);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Check if it's a foreign key constraint error
                if (ex.Number == 547)
                {
                    ShowMessage("Cannot delete this subject. It is currently assigned to teachers or students.", false);
                }
                else
                {
                    ShowMessage("Error deleting subject: " + ex.Message, false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting subject: " + ex.Message, false);
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
            Response.Redirect("Landing.aspx");
        }
    }
}