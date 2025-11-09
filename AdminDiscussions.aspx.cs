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
    public partial class AdminDiscussions : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Only admin
            if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindDiscussions();
            }
        }

        private int GetCurrentAdminId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void BindDiscussions()
        {
            lblMessage.Text = "";
            pnlEmpty.Visible = false;

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                string sql = @"
                    SELECT 
                        d.discussionID AS DiscussionId,
                        d.resourceID   AS ResourceId,
                        d.message      AS Message,
                        d.createdAt,
                        d.isFlagged,
                        d.flaggedAt,
                        d.flagReason,
                        u.fullName     AS AuthorName,
                        fu.fullName    AS FlaggedByName,
                        r.title        AS ResourceTitle,
                        r.type         AS Type,
                        s.name         AS SubjectName
                    FROM ResourceDiscussions d
                    INNER JOIN Resources r ON d.resourceID = r.resourceID
                    INNER JOIN Subjects s  ON r.subjectID = s.subjectID
                    INNER JOIN Users u     ON d.userID = u.userID
                    LEFT JOIN Users fu     ON d.flaggedByUserID = fu.userID
                    WHERE d.isDeleted = 0
                ";

                // Status filter
                bool flaggedOnly = (ddlStatus.SelectedValue == "flagged");
                if (flaggedOnly)
                {
                    sql += " AND d.isFlagged = 1";
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    sql += @"
                        AND (
                            r.title LIKE @Search
                            OR d.message LIKE @Search
                            OR u.fullName LIKE @Search
                        )";
                    cmd.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");
                }

                sql += " ORDER BY d.isFlagged DESC, d.flaggedAt DESC, d.createdAt DESC;";

                cmd.CommandText = sql;

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            // Add formatted columns
            dt.Columns.Add("CreatedAtDisplay", typeof(string));
            dt.Columns.Add("FlaggedAtDisplay", typeof(string));
            dt.Columns.Add("MessageShort", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                if (row["createdAt"] != DBNull.Value)
                {
                    DateTime created = Convert.ToDateTime(row["createdAt"]);
                    row["CreatedAtDisplay"] = created.ToString("dd MMM yyyy, HH:mm");
                }
                else
                {
                    row["CreatedAtDisplay"] = "";
                }

                if (row["flaggedAt"] != DBNull.Value)
                {
                    DateTime flaggedAt = Convert.ToDateTime(row["flaggedAt"]);
                    row["FlaggedAtDisplay"] = flaggedAt.ToString("dd MMM yyyy, HH:mm");
                }
                else
                {
                    row["FlaggedAtDisplay"] = "";
                }

                string msg = row["Message"].ToString();
                if (msg.Length > 160)
                    row["MessageShort"] = msg.Substring(0, 157) + "...";
                else
                    row["MessageShort"] = msg;
            }

            rptDiscussions.DataSource = dt;
            rptDiscussions.DataBind();

            pnlEmpty.Visible = dt.Rows.Count == 0;
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            BindDiscussions();
        }

        protected void rptDiscussions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int discussionId;
            if (!int.TryParse(e.CommandArgument.ToString(), out discussionId))
                return;

            if (e.CommandName == "ClearFlag")
            {
                ClearFlag(discussionId);
                BindDiscussions();
            }
            else if (e.CommandName == "DeleteComment")
            {
                DeleteComment(discussionId);
                BindDiscussions();
            }
        }

        private void ClearFlag(int discussionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE ResourceDiscussions
                    SET isFlagged = 0,
                        flaggedByUserID = NULL,
                        flaggedAt = NULL,
                        flagReason = NULL
                    WHERE discussionID = @Id;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", discussionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.CssClass = "message success";
                lblMessage.Text = "Flag cleared successfully.";
            }
            catch
            {
                lblMessage.CssClass = "message error";
                lblMessage.Text = "Failed to clear flag.";
            }
        }

        private void DeleteComment(int discussionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE ResourceDiscussions
                    SET isDeleted = 1
                    WHERE discussionID = @Id
                       OR parentDiscussionID = @Id;  -- delete replies too
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", discussionId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.CssClass = "message success";
                lblMessage.Text = "Comment and its replies deleted.";
            }
            catch
            {
                lblMessage.CssClass = "message error";
                lblMessage.Text = "Failed to delete comment.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }
    }
}