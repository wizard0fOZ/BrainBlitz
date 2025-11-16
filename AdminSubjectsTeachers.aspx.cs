using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class AdminSubjectsTeachers : System.Web.UI.Page
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
                    string query = @"SELECT 
                                        s.subjectID,
                                        s.name,
                                        s.description,
                                        s.assignedTo,
                                        u.fullName AS teacherName,
                                        u.email AS teacherEmail
                                     FROM Subjects s
                                     LEFT JOIN Users u ON s.assignedTo = u.userID
                                     ORDER BY s.name ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSubjects.DataSource = dt;
                    gvSubjects.DataBind();

                    // Populate teacher dropdowns
                    foreach (GridViewRow row in gvSubjects.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            DropDownList ddlTeachers = (DropDownList)row.FindControl("ddlTeachers");
                            if (ddlTeachers != null)
                            {
                                PopulateTeachersDropDown(ddlTeachers);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading subjects: " + ex.Message, false);
            }
        }

        private void PopulateTeachersDropDown(DropDownList ddl)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                        userID, 
                                        fullName + ' (' + email + ')' AS TeacherDisplay
                                     FROM Users 
                                     WHERE role = 'Teacher' AND isActive = 1
                                     ORDER BY fullName ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddl.DataSource = dt;
                    ddl.DataTextField = "TeacherDisplay";
                    ddl.DataValueField = "userID";
                    ddl.DataBind();

                    ddl.Items.Insert(0, new ListItem("-- Select Teacher --", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading teachers: " + ex.Message, false);
            }
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int subjectId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "AssignTeacher")
            {
                // Find the row
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                DropDownList ddlTeachers = (DropDownList)row.FindControl("ddlTeachers");

                if (ddlTeachers != null && ddlTeachers.SelectedValue != "0")
                {
                    int teacherId = Convert.ToInt32(ddlTeachers.SelectedValue);
                    AssignTeacher(subjectId, teacherId);
                }
                else
                {
                    ShowMessage("Please select a teacher to assign.", false);
                }
            }
            else if (e.CommandName == "RemoveTeacher")
            {
                RemoveTeacher(subjectId);
            }
        }

        private void AssignTeacher(int subjectId, int teacherId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if teacher is already assigned to this subject
                    string checkQuery = "SELECT assignedTo FROM Subjects WHERE subjectID = @SubjectID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    object currentTeacher = checkCmd.ExecuteScalar();

                    if (currentTeacher != null && currentTeacher != DBNull.Value)
                    {
                        int currentTeacherId = Convert.ToInt32(currentTeacher);
                        if (currentTeacherId == teacherId)
                        {
                            ShowMessage("This teacher is already assigned to this subject.", false);
                            return;
                        }
                    }

                    // Assign teacher to subject
                    string updateQuery = @"UPDATE Subjects 
                                         SET assignedTo = @TeacherID 
                                         WHERE subjectID = @SubjectID";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    updateCmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Teacher assigned successfully!", true);
                        BindSubjectsGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to assign teacher.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error assigning teacher: " + ex.Message, false);
            }
        }

        private void RemoveTeacher(int subjectId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Subjects 
                                   SET assignedTo = NULL 
                                   WHERE subjectID = @SubjectID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Teacher removed successfully.", true);
                        BindSubjectsGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to remove teacher.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error removing teacher: " + ex.Message, false);
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