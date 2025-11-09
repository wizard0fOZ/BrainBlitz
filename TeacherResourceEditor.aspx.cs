using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class TeacherResourceEditor : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Teacher")
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindSubjectDropdown();

                int resourceId;
                if (int.TryParse(Request.QueryString["id"], out resourceId))
                {
                    // edit mode
                    hfResourceId.Value = resourceId.ToString();
                    lblPageTitle.Text = "Edit Resource";
                    lblPageSubtitle.Text = "Update resource details";
                    btnSave.Text = "Save Changes";
                    LoadResource(resourceId);
                }
            }
        }

        private int GetCurrentTeacherId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void BindSubjectDropdown()
        {
            ddlSubject.Items.Clear();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT subjectID, name
                FROM Subjects
                WHERE assignedTo = @TeacherId
                ORDER BY name;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@TeacherId", GetCurrentTeacherId());
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlSubject.Items.Add(
                            new System.Web.UI.WebControls.ListItem(
                                reader["name"].ToString(),
                                reader["subjectID"].ToString()
                            )
                        );
                    }
                }
            }

            if (ddlSubject.Items.Count == 0)
            {
                ddlSubject.Items.Add(new System.Web.UI.WebControls.ListItem("No subjects assigned", ""));
                ddlSubject.Enabled = false;
                btnSave.Enabled = false;
                lblMessage.CssClass = "message error";
                lblMessage.Text = "You do not have any subjects assigned. Resources cannot be created.";
            }
        }

        private void LoadResource(int resourceId)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT r.resourceID, r.title, r.description, r.type, r.urlOrPath, r.subjectID
                FROM Resources r
                INNER JOIN Subjects s ON r.subjectID = s.subjectID
                WHERE r.resourceID = @ResourceId AND s.assignedTo = @TeacherId;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                cmd.Parameters.AddWithValue("@TeacherId", GetCurrentTeacherId());

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        lblMessage.CssClass = "message error";
                        lblMessage.Text = "Resource not found or you do not have permission to edit it.";
                        btnSave.Enabled = false;
                        return;
                    }

                    txtTitle.Text = reader["title"].ToString();
                    txtDescription.Text = reader["description"] == DBNull.Value ? "" : reader["description"].ToString();
                    ddlType.SelectedValue = reader["type"] == DBNull.Value ? "pdf" : reader["type"].ToString();

                    string subjectId = reader["subjectID"].ToString();
                    if (ddlSubject.Items.FindByValue(subjectId) != null)
                    {
                        ddlSubject.SelectedValue = subjectId;
                    }

                    string path = reader["urlOrPath"] == DBNull.Value ? "" : reader["urlOrPath"].ToString();
                    hfExistingPath.Value = path;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        lblExistingFile.Text = "Current file / URL: " + path;
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "message";

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                ShowError("Please enter a resource title.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlSubject.SelectedValue))
            {
                ShowError("Please select a subject.");
                return;
            }

            string selectedType = ddlType.SelectedValue;
            string urlOrPath = DetermineUrlOrPath();
            if (string.IsNullOrWhiteSpace(urlOrPath))
            {
                ShowError("Please upload a file or provide an external URL.");
                return;
            }

            int subjectId = Convert.ToInt32(ddlSubject.SelectedValue);
            int teacherId = GetCurrentTeacherId();

            int resourceId;
            if (int.TryParse(hfResourceId.Value, out resourceId) && resourceId > 0)
            {
                UpdateResource(resourceId, teacherId, subjectId, selectedType, urlOrPath);
            }
            else
            {
                CreateResource(teacherId, subjectId, selectedType, urlOrPath);
            }
        }

        private string DetermineUrlOrPath()
        {
            string urlOrPath = "";

            // 1. handle file upload
            if (fuResource.HasFile)
            {
                string uploadsFolder = Server.MapPath("~/uploads/resources/");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string extension = Path.GetExtension(fuResource.FileName).ToLower();
                string allowedExtensions = ".pdf,.doc,.docx,.ppt,.pptx,.png,.jpg,.jpeg,.gif,.mp4,.mov,.avi";
                if (!allowedExtensions.Contains(extension))
                {
                    ShowError("File type not allowed.");
                    return "";
                }

                string fileName = Guid.NewGuid().ToString("N") + extension;
                string fullPath = Path.Combine(uploadsFolder, fileName);
                fuResource.SaveAs(fullPath);

                urlOrPath = "~/uploads/resources/" + fileName;
            }
            else if (!string.IsNullOrWhiteSpace(txtExternalUrl.Text))
            {
                urlOrPath = txtExternalUrl.Text.Trim();
            }
            else
            {
                // maybe keep existing path when editing
                if (!string.IsNullOrWhiteSpace(hfExistingPath.Value))
                {
                    urlOrPath = hfExistingPath.Value;
                }
            }

            return urlOrPath;
        }

        private void CreateResource(int teacherId, int subjectId, string type, string urlOrPath)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Resources (userID, subjectID, title, urlOrPath, type, description, createdAt)
                VALUES (@UserId, @SubjectId, @Title, @UrlOrPath, @Type, @Description, GETDATE());
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", teacherId);
                cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@UrlOrPath", urlOrPath);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.CssClass = "message success";
            lblMessage.Text = "Resource created successfully.";
            Response.Redirect("~/TeacherResources.aspx");
        }

        private void UpdateResource(int resourceId, int teacherId, int subjectId, string type, string urlOrPath)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Resources
                SET subjectID = @SubjectId,
                    title = @Title,
                    urlOrPath = @UrlOrPath,
                    type = @Type,
                    description = @Description
                WHERE resourceID = @ResourceId AND userID = @UserId;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@SubjectId", subjectId);
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@UrlOrPath", urlOrPath);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                cmd.Parameters.AddWithValue("@UserId", teacherId);

                conn.Open();
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                {
                    ShowError("Update failed or you do not have permission to edit this resource.");
                    return;
                }
            }

            lblMessage.CssClass = "message success";
            lblMessage.Text = "Resource updated successfully.";
            Response.Redirect("~/TeacherResources.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TeacherResources.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }

        private void ShowError(string message)
        {
            lblMessage.CssClass = "message error";
            lblMessage.Text = message;
        }
    }
}