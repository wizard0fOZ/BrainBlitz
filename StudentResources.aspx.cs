using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BrainBlitz
{
    public partial class StudentResources : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Student")
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindSubjectFilter();
                BindResources();
            }
        }

        private int GetCurrentUserId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void BindSubjectFilter()
        {
            ddlSubjectFilter.Items.Clear();
            ddlSubjectFilter.Items.Add(new ListItem("All Subjects", ""));

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT s.subjectID, s.name
                FROM Subjects s
                INNER JOIN SubjectEnrollments se ON se.subjectID = s.subjectID
                WHERE se.userID = @UserId
                ORDER BY s.name;
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlSubjectFilter.Items.Add(
                            new ListItem(
                                reader["name"].ToString(),
                                reader["subjectID"].ToString()
                            )
                        );
                    }
                }
            }
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
                        r.description,
                        s.name AS SubjectName,
                        CASE WHEN b.userID IS NULL THEN 0 ELSE 1 END AS IsBookmarked
                    FROM Resources r
                    INNER JOIN Subjects s ON r.subjectID = s.subjectID
                    INNER JOIN SubjectEnrollments se 
                        ON se.subjectID = s.subjectID AND se.userID = @UserId
                    LEFT JOIN Bookmarks b 
                        ON b.resourceID = r.resourceID AND b.userID = @UserId
                    WHERE 1 = 1
                ";

                cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());

                // Subject filter
                if (!string.IsNullOrWhiteSpace(ddlSubjectFilter.SelectedValue))
                {
                    sql += " AND r.subjectID = @SubjectId";
                    cmd.Parameters.AddWithValue("@SubjectId", ddlSubjectFilter.SelectedValue);
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    sql += " AND (r.title LIKE @Search OR r.description LIKE @Search OR s.name LIKE @Search)";
                    cmd.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");
                }

                sql += " ORDER BY r.createdAt DESC;";

                cmd.CommandText = sql;

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                // add ShortDescription column
                dt.Columns.Add("ShortDescription", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string full = row["description"] == DBNull.Value ? "" : row["description"].ToString();
                    if (full.Length > 120)
                    {
                        row["ShortDescription"] = full.Substring(0, 117) + "...";
                    }
                    else
                    {
                        row["ShortDescription"] = full;
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

        protected void rptResources_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ToggleBookmark")
            {
                int resourceId = Convert.ToInt32(e.CommandArgument);
                ToggleBookmark(resourceId);
                BindResources();
            }
        }

        private void ToggleBookmark(int resourceId)
        {
            int userId = GetCurrentUserId();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                // check if exists
                using (SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Bookmarks
                    WHERE userID = @UserId AND resourceID = @ResourceId;
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    checkCmd.Parameters.AddWithValue("@ResourceId", resourceId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // remove bookmark
                        using (SqlCommand deleteCmd = new SqlCommand(@"
                            DELETE FROM Bookmarks
                            WHERE userID = @UserId AND resourceID = @ResourceId;
                        ", conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@UserId", userId);
                            deleteCmd.Parameters.AddWithValue("@ResourceId", resourceId);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // add bookmark
                        using (SqlCommand insertCmd = new SqlCommand(@"
                            INSERT INTO Bookmarks (userID, resourceID, createdAt)
                            VALUES (@UserId, @ResourceId, GETDATE());
                        ", conn))
                        {
                            insertCmd.Parameters.AddWithValue("@UserId", userId);
                            insertCmd.Parameters.AddWithValue("@ResourceId", resourceId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /* NEW: auto search + clear handlers */

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            BindResources();
        }

        protected void ddlSubjectFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindResources();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;

            if (ddlSubjectFilter.Items.Count > 0)
            {
                ddlSubjectFilter.SelectedIndex = 0; // "All Subjects"
            }

            BindResources();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }
    }
}
