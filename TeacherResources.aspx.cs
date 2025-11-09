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
    public partial class TeacherResources : System.Web.UI.Page
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
                BindResources();
            }
        }

        private int GetCurrentTeacherId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void BindResources()
        {
            lblMessage.Text = "";
            pnlEmpty.Visible = false;

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                string sql = @"
                    SELECT 
                        r.resourceID AS ResourceId,
                        r.title AS Title,
                        r.type AS Type,
                        r.createdAt,
                        s.name AS SubjectName
                    FROM Resources r
                    INNER JOIN Subjects s ON r.subjectID = s.subjectID
                    WHERE s.assignedTo = @TeacherId
                ";

                cmd.Parameters.AddWithValue("@TeacherId", GetCurrentTeacherId());

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    sql += " AND (r.title LIKE @Search OR r.description LIKE @Search)";
                    cmd.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");
                }

                sql += " ORDER BY r.createdAt DESC;";

                cmd.CommandText = sql;

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                dt.Columns.Add("CreatedAtFormatted", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["createdAt"] != DBNull.Value)
                    {
                        DateTime dtCreated = Convert.ToDateTime(row["createdAt"]);
                        row["CreatedAtFormatted"] = dtCreated.ToString("dd MMM yyyy");
                    }
                    else
                    {
                        row["CreatedAtFormatted"] = "";
                    }
                }

                rptResources.DataSource = dt;
                rptResources.DataBind();

                if (dt.Rows.Count == 0)
                {
                    pnlEmpty.Visible = true;
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            BindResources();
        }

        protected void rptResources_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteResource")
            {
                int resourceId = Convert.ToInt32(e.CommandArgument);
                DeleteResource(resourceId);
                BindResources();
            }
        }

        private void DeleteResource(int resourceId)
        {
            int teacherId = GetCurrentTeacherId();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                string checkSql = @"
                    SELECT COUNT(*) 
                    FROM Resources r
                    INNER JOIN Subjects s ON r.subjectID = s.subjectID
                    WHERE r.resourceID = @ResourceId AND s.assignedTo = @TeacherId;
                ";

                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    checkCmd.Parameters.AddWithValue("@TeacherId", teacherId);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count == 0)
                    {
                        lblMessage.CssClass = "message error";
                        lblMessage.Text = "You are not allowed to delete this resource.";
                        return;
                    }
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM Bookmarks WHERE resourceID = @ResourceId";
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM Completions WHERE resourceID = @ResourceId";
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM Resources WHERE resourceID = @ResourceId";
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    int affected = cmd.ExecuteNonQuery();

                    if (affected > 0)
                    {
                        lblMessage.CssClass = "message success";
                        lblMessage.Text = "Resource deleted successfully.";
                    }
                    else
                    {
                        lblMessage.CssClass = "message error";
                        lblMessage.Text = "Failed to delete resource.";
                    }
                }
            }
        }

        protected void btnUploadResource_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TeacherResourceEditor.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }
    }
}