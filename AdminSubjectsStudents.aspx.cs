using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class AdminSubjectsStudents : System.Web.UI.Page
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
                                        COUNT(se.enrollmentID) AS enrolledCount
                                     FROM Subjects s
                                     LEFT JOIN SubjectEnrollments se ON s.subjectID = se.subjectID
                                     GROUP BY s.subjectID, s.name, s.description
                                     ORDER BY s.name ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvSubjects.DataSource = dt;
                    gvSubjects.DataBind();

                    // Populate student dropdowns for each row
                    foreach (GridViewRow row in gvSubjects.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            DropDownList ddlStudents = (DropDownList)row.FindControl("ddlStudents");
                            if (ddlStudents != null)
                            {
                                int subjectId = Convert.ToInt32(gvSubjects.DataKeys[row.RowIndex].Value);
                                PopulateStudentsDropDown(ddlStudents, subjectId);
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

        private void PopulateStudentsDropDown(DropDownList ddl, int subjectId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Get students who are NOT already enrolled in this subject
                    string query = @"SELECT 
                                        u.userID, 
                                        u.fullName + ' (' + u.email + ')' AS StudentDisplay
                                     FROM Users u
                                     WHERE u.role = 'Student' 
                                     AND u.isActive = 1
                                     AND u.userID NOT IN (
                                         SELECT userID 
                                         FROM SubjectEnrollments 
                                         WHERE subjectID = @SubjectID
                                     )
                                     ORDER BY u.fullName ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddl.DataSource = dt;
                    ddl.DataTextField = "StudentDisplay";
                    ddl.DataValueField = "userID";
                    ddl.DataBind();

                    // Add default option
                    ddl.Items.Insert(0, new ListItem("-- Select Student --", "0"));
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading students: " + ex.Message, false);
            }
        }

        protected void gvSubjects_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int subjectId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EnrollStudent")
            {
                // Find the row
                GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                DropDownList ddlStudents = (DropDownList)row.FindControl("ddlStudents");

                if (ddlStudents != null && ddlStudents.SelectedValue != "0")
                {
                    int studentId = Convert.ToInt32(ddlStudents.SelectedValue);
                    EnrollStudent(subjectId, studentId);
                }
                else
                {
                    ShowMessage("Please select a student to enroll.", false);
                }
            }
            else if (e.CommandName == "ViewEnrolled")
            {
                hfCurrentSubjectID.Value = subjectId.ToString();
                LoadEnrolledStudents(subjectId);

                // Register script to show modal after page update
                ScriptManager.RegisterStartupScript(upEnrolledList, upEnrolledList.GetType(),
                    "ShowModal", "showModal();", true);
            }
        }

        private void EnrollStudent(int subjectId, int studentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if student is already enrolled
                    string checkQuery = @"SELECT COUNT(*) 
                                        FROM SubjectEnrollments 
                                        WHERE subjectID = @SubjectID AND userID = @UserID";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@SubjectID", subjectId);
                    checkCmd.Parameters.AddWithValue("@UserID", studentId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        ShowMessage("This student is already enrolled in this subject.", false);
                        return;
                    }

                    // Enroll the student
                    string insertQuery = @"INSERT INTO SubjectEnrollments (subjectID, userID, enrolledAt) 
                                         VALUES (@SubjectID, @UserID, GETDATE())";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@SubjectID", subjectId);
                    insertCmd.Parameters.AddWithValue("@UserID", studentId);

                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Student enrolled successfully!", true);
                        BindSubjectsGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to enroll student.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error enrolling student: " + ex.Message, false);
            }
        }

        private void LoadEnrolledStudents(int subjectId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                        se.enrollmentID,
                                        se.userID,
                                        u.fullName,
                                        u.email,
                                        se.enrolledAt
                                     FROM SubjectEnrollments se
                                     INNER JOIN Users u ON se.userID = u.userID
                                     WHERE se.subjectID = @SubjectID
                                     ORDER BY u.fullName ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptEnrolledStudents.DataSource = dt;
                        rptEnrolledStudents.DataBind();
                        rptEnrolledStudents.Visible = true;
                        pnlNoEnrollments.Visible = false;
                    }
                    else
                    {
                        rptEnrolledStudents.Visible = false;
                        pnlNoEnrollments.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading enrolled students: " + ex.Message, false);
            }
        }

        protected void rptEnrolledStudents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveEnrollment")
            {
                int enrollmentId = Convert.ToInt32(e.CommandArgument);
                RemoveEnrollment(enrollmentId);

                // Reload the modal data
                int subjectId = Convert.ToInt32(hfCurrentSubjectID.Value);
                LoadEnrolledStudents(subjectId);

                // Refresh the main grid
                BindSubjectsGrid();

                // Keep modal open
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showModal();", true);
            }
        }

        private void RemoveEnrollment(int enrollmentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"DELETE FROM SubjectEnrollments 
                                   WHERE enrollmentID = @EnrollmentID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Student removed from subject successfully.", true);
                    }
                    else
                    {
                        ShowMessage("Failed to remove student.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error removing student: " + ex.Message, false);
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