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
    public partial class StudentResourceView : System.Web.UI.Page
    {
        private string cs = ConfigurationManager.ConnectionStrings["BrainBlitzDB"].ConnectionString;

        private DataTable _allReplies;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null)
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            var role = Session["Role"].ToString();
            if (role != "Student" && role != "Teacher" && role != "Admin")
            {
                Response.Redirect("~/Landing.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int resourceId;
                if (!int.TryParse(Request.QueryString["id"], out resourceId))
                {
                    ShowError("Invalid resource.");
                    pnlContent.Visible = false;
                    return;
                }

                hfResourceId.Value = resourceId.ToString();

                ConfigureHeader(role);
                LoadResource(resourceId, role);
            }
        }

        private void ConfigureHeader(string role)
        {
            bool isTeacher = role == "Teacher";
            bool isAdmin = role == "Admin";
            bool isStudent = role == "Student";

            if (isTeacher)
                lnkLogo.HRef = "TeacherDashboard.aspx";
            else if (isAdmin)
                lnkLogo.HRef = "AdminDashboard.aspx";
            else
                lnkLogo.HRef = "StudentDashboard.aspx";

            string from = (Request.QueryString["from"] ?? "").ToLower();

            if (from == "teacher" || isTeacher)
                lnkBack.HRef = "TeacherResources.aspx";
            else if (from == "admin" || isAdmin)
                lnkBack.HRef = "AdminDiscussions.aspx";
            else
                lnkBack.HRef = "StudentResources.aspx";

            // Only students should see bookmark / complete
            btnBookmark.Visible = isStudent;
            btnMarkComplete.Visible = isStudent;
        }

        private int GetCurrentUserId()
        {
            return Convert.ToInt32(Session["UserId"]);
        }

        private void LoadResource(int resourceId, string role)
        {
            bool isTeacher = role == "Teacher";
            bool isAdmin = role == "Admin";
            bool isStudent = role == "Student";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                if (isTeacher || isAdmin)
                {
                    cmd.CommandText = @"
                        SELECT 
                            r.resourceID,
                            r.title,
                            r.description,
                            r.type,
                            r.urlOrPath,
                            s.name AS SubjectName,
                            u.fullName AS TeacherName
                        FROM Resources r
                        INNER JOIN Subjects s ON r.subjectID = s.subjectID
                        LEFT JOIN Users u ON r.userID = u.userID
                        WHERE r.resourceID = @ResourceId;
                    ";
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                }
                else if (isStudent)
                {
                    cmd.CommandText = @"
                        SELECT 
                            r.resourceID,
                            r.title,
                            r.description,
                            r.type,
                            r.urlOrPath,
                            s.name AS SubjectName,
                            u.fullName AS TeacherName,
                            CASE WHEN b.userID IS NULL THEN 0 ELSE 1 END AS IsBookmarked,
                            CASE WHEN c.userID IS NULL THEN 0 ELSE 1 END AS IsCompleted
                        FROM Resources r
                        INNER JOIN Subjects s ON r.subjectID = s.subjectID
                        INNER JOIN SubjectEnrollments se
                            ON se.subjectID = s.subjectID AND se.userID = @UserId
                        LEFT JOIN Users u ON r.userID = u.userID
                        LEFT JOIN Bookmarks b 
                            ON b.resourceID = r.resourceID AND b.userID = @UserId
                        LEFT JOIN Completions c
                            ON c.resourceID = r.resourceID AND c.userID = @UserId
                        WHERE r.resourceID = @ResourceId;
                    ";
                    cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                }
                else
                {
                    ShowError("You do not have access to this resource.");
                    pnlContent.Visible = false;
                    return;
                }

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowError("Resource not found or you do not have access to it.");
                        pnlContent.Visible = false;
                        return;
                    }

                    lblTitle.Text = reader["title"].ToString();
                    lblSubject.Text = reader["SubjectName"].ToString();

                    string type = reader["type"] == DBNull.Value ? "" : reader["type"].ToString();
                    lblType.Text = string.IsNullOrEmpty(type) ? "Resource" : type.ToUpper();

                    string teacherName = reader["TeacherName"] == DBNull.Value
                        ? "Unknown teacher"
                        : reader["TeacherName"].ToString();
                    lblTeacher.Text = teacherName;

                    lblDescription.Text = reader["description"] == DBNull.Value
                        ? ""
                        : reader["description"].ToString();

                    string urlOrPath = reader["urlOrPath"] == DBNull.Value
                        ? ""
                        : reader["urlOrPath"].ToString();

                    if (!string.IsNullOrWhiteSpace(urlOrPath))
                    {
                        hlDownload.NavigateUrl = urlOrPath;
                        hlDownload.Enabled = true;
                    }
                    else
                    {
                        hlDownload.NavigateUrl = "#";
                        hlDownload.Enabled = false;
                    }

                    if (isStudent)
                    {
                        bool isBookmarked = Convert.ToInt32(reader["IsBookmarked"]) == 1;
                        bool isCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1;

                        ConfigureBookmarkButton(isBookmarked);
                        ConfigureCompleteButton(isCompleted);
                    }
                }
            }

            // Preview
            string previewType = null;
            string previewPath = null;

            using (SqlConnection conn2 = new SqlConnection(cs))
            using (SqlCommand cmd2 = new SqlCommand(@"
                SELECT type, urlOrPath 
                FROM Resources 
                WHERE resourceID = @ResourceId;
            ", conn2))
            {
                cmd2.Parameters.AddWithValue("@ResourceId", resourceId);
                conn2.Open();
                using (SqlDataReader r2 = cmd2.ExecuteReader())
                {
                    if (r2.Read())
                    {
                        previewType = r2["type"] == DBNull.Value ? "" : r2["type"].ToString();
                        previewPath = r2["urlOrPath"] == DBNull.Value ? "" : r2["urlOrPath"].ToString();
                    }
                }
            }

            RenderPreview(previewType, previewPath);
            BindDiscussion(resourceId);
        }

        private void ConfigureBookmarkButton(bool isBookmarked)
        {
            if (isBookmarked)
            {
                btnBookmark.CssClass = "secondary-btn active";
                btnBookmark.Text = "<i class='fas fa-bookmark'></i><span>Bookmarked</span>";
            }
            else
            {
                btnBookmark.CssClass = "secondary-btn";
                btnBookmark.Text = "<i class='far fa-bookmark'></i><span>Bookmark</span>";
            }
        }

        private void ConfigureCompleteButton(bool isCompleted)
        {
            if (isCompleted)
            {
                btnMarkComplete.CssClass = "secondary-btn active";
                btnMarkComplete.Text = "<i class='fas fa-check'></i><span>Completed</span>";
                btnMarkComplete.Enabled = false;
            }
            else
            {
                btnMarkComplete.CssClass = "secondary-btn";
                btnMarkComplete.Text = "<i class='fas fa-check'></i><span>Mark Complete</span>";
                btnMarkComplete.Enabled = true;
            }
        }

        // dynamic preview based on type
        private void RenderPreview(string type, string urlOrPath)
        {
            pnlPreview.Controls.Clear();

            if (string.IsNullOrWhiteSpace(urlOrPath))
            {
                pnlPreview.Controls.Add(new Literal
                {
                    Text = "No preview available for this resource."
                });
                return;
            }

            string resolvedUrl = ResolveUrl(urlOrPath);
            string lowerType = (type ?? "").ToLower();

            if (lowerType == "pdf")
            {
                pnlPreview.Controls.Add(new Literal
                {
                    Text = $"<iframe src='{resolvedUrl}' style='width:100%;height:450px;border:none;'></iframe>"
                });
                return;
            }

            if (lowerType == "img" || lowerType == "image")
            {
                pnlPreview.Controls.Add(new Literal
                {
                    Text = $"<img src='{resolvedUrl}' style='max-width:100%;max-height:450px;object-fit:contain;' />"
                });
                return;
            }

            if (lowerType == "video")
            {
                pnlPreview.Controls.Add(new Literal
                {
                    Text = @"
<video controls style='width:100%;max-height:450px;'>
    <source src='" + resolvedUrl + @"' type='video/mp4' />
    Your browser does not support the video tag.
</video>"
                });
                return;
            }

            if (lowerType == "link")
            {
                pnlPreview.Controls.Add(new Literal
                {
                    Text = @"
<div style='margin-bottom:10px;'>This is a web link resource.</div>
<a href='" + resolvedUrl + @"' target='_blank' style='color:#610099;text-decoration:underline;'>
    Open link in new tab
</a>"
                });
                return;
            }

            pnlPreview.Controls.Add(new Literal
            {
                Text = "Preview is not available for this resource type. Use the Download button above."
            });
        }

        protected void btnBookmark_Click(object sender, EventArgs e)
        {
            if ((Session["Role"]?.ToString() ?? "") != "Student") return;

            int resourceId;
            if (!int.TryParse(hfResourceId.Value, out resourceId)) return;

            ToggleBookmark(resourceId);
            LoadResource(resourceId, "Student");
        }

        private void ToggleBookmark(int resourceId)
        {
            int userId = GetCurrentUserId();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Bookmarks
                    WHERE userID = @UserId AND resourceID = @ResourceId;
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    checkCmd.Parameters.AddWithValue("@ResourceId", resourceId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
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

        protected void btnMarkComplete_Click(object sender, EventArgs e)
        {
            if ((Session["Role"]?.ToString() ?? "") != "Student") return;

            int resourceId;
            if (!int.TryParse(hfResourceId.Value, out resourceId)) return;

            int userId = GetCurrentUserId();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Completions
                    WHERE userID = @UserId AND resourceID = @ResourceId;
                ", conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    checkCmd.Parameters.AddWithValue("@ResourceId", resourceId);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count == 0)
                    {
                        using (SqlCommand insertCmd = new SqlCommand(@"
                            INSERT INTO Completions (userID, resourceID, completedAt)
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

            LoadResource(resourceId, "Student");
        }

        private string FormatTimeAgo(DateTime dateTime)
        {
            TimeSpan diff = DateTime.Now - dateTime;

            if (diff.TotalMinutes < 1) return "Just now";
            if (diff.TotalHours < 1) return ((int)diff.TotalMinutes) + " minutes ago";
            if (diff.TotalDays < 1) return ((int)diff.TotalHours) + " hours ago";
            if (diff.TotalDays < 7) return ((int)diff.TotalDays) + " days ago";

            int weeks = (int)(diff.TotalDays / 7);
            return weeks + (weeks == 1 ? " week ago" : " weeks ago");
        }

        private void BindDiscussion(int resourceId)
        {
            DataTable parents = new DataTable();
            _allReplies = new DataTable();

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT d.discussionID AS DiscussionID,
                           d.message      AS Message,
                           d.createdAt,
                           d.IsFlagged,
                           u.fullName     AS AuthorName
                    FROM ResourceDiscussions d
                    INNER JOIN Users u ON d.userID = u.userID
                    WHERE d.resourceID = @ResourceId
                      AND d.isDeleted = 0
                      AND d.parentDiscussionID IS NULL
                    ORDER BY d.createdAt DESC;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(parents);
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT d.discussionID AS DiscussionID,
                           d.parentDiscussionID,
                           d.message      AS Message,
                           d.createdAt,
                           u.fullName     AS AuthorName
                    FROM ResourceDiscussions d
                    INNER JOIN Users u ON d.userID = u.userID
                    WHERE d.resourceID = @ResourceId
                      AND d.isDeleted = 0
                      AND d.parentDiscussionID IS NOT NULL
                    ORDER BY d.createdAt ASC;
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(_allReplies);
                    }
                }
            }

            parents.Columns.Add("TimeAgo", typeof(string));
            foreach (DataRow row in parents.Rows)
            {
                DateTime created = Convert.ToDateTime(row["createdAt"]);
                row["TimeAgo"] = FormatTimeAgo(created);
            }

            _allReplies.Columns.Add("TimeAgo", typeof(string));
            foreach (DataRow row in _allReplies.Rows)
            {
                DateTime created = Convert.ToDateTime(row["createdAt"]);
                row["TimeAgo"] = FormatTimeAgo(created);
            }

            rptThreads.DataSource = parents;
            rptThreads.DataBind();

            int totalComments = parents.Rows.Count + _allReplies.Rows.Count;
            lblCommentCount.Text = totalComments.ToString();
            pnlNoComments.Visible = parents.Rows.Count == 0;
        }

        protected void rptThreads_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            if (_allReplies == null || _allReplies.Rows.Count == 0)
                return;

            DataRowView parentRow = (DataRowView)e.Item.DataItem;
            int discussionId = Convert.ToInt32(parentRow["DiscussionID"]);

            Repeater rptReplies = (Repeater)e.Item.FindControl("rptReplies");
            if (rptReplies != null)
            {
                DataRow[] childRows = _allReplies.Select("parentDiscussionID = " + discussionId);
                if (childRows.Length > 0)
                {
                    DataTable repliesForParent = childRows.CopyToDataTable();
                    rptReplies.DataSource = repliesForParent;
                    rptReplies.DataBind();
                }
            }
        }

        protected void rptThreads_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Reply")
            {
                int parentId;
                if (!int.TryParse(e.CommandArgument.ToString(), out parentId)) return;

                TextBox txtReply = (TextBox)e.Item.FindControl("txtReply");
                if (txtReply == null) return;

                string text = txtReply.Text.Trim();
                if (string.IsNullOrEmpty(text)) return;

                int resourceId;
                if (!int.TryParse(hfResourceId.Value, out resourceId)) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(cs))
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ResourceDiscussions 
                            (resourceID, userID, parentDiscussionID, message, createdAt)
                        VALUES 
                            (@ResourceId, @UserId, @ParentId, @Message, GETDATE());
                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                        cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                        cmd.Parameters.AddWithValue("@ParentId", parentId);
                        cmd.Parameters.AddWithValue("@Message", text);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    txtReply.Text = string.Empty;
                    BindDiscussion(resourceId);
                }
                catch
                {
                    lblMessage.Visible = true;
                    lblMessage.CssClass = "message error";
                    lblMessage.Text = "There was a problem posting your reply. Please try again.";
                }

                return;
            }

            if (e.CommandName == "Flag")
            {
                int discussionId;
                if (!int.TryParse(e.CommandArgument.ToString(), out discussionId)) return;

                int resourceId;
                if (!int.TryParse(hfResourceId.Value, out resourceId)) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(cs))
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE ResourceDiscussions
                        SET IsFlagged = 1,
                            FlaggedByUserID = @UserId,
                            FlaggedAt = GETDATE(),
                            FlagReason = @Reason
                        WHERE discussionID = @DiscussionId;
                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                        cmd.Parameters.AddWithValue("@DiscussionId", discussionId);
                        cmd.Parameters.AddWithValue("@Reason", "Inappropriate or off-topic");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    BindDiscussion(resourceId);

                    lblMessage.Visible = true;
                    lblMessage.CssClass = "message success";
                    lblMessage.Text = "Thanks, this comment has been reported for review.";
                }
                catch
                {
                    lblMessage.Visible = true;
                    lblMessage.CssClass = "message error";
                    lblMessage.Text = "There was a problem reporting this comment. Please try again.";
                }
            }
        }

        protected void btnPostComment_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            int resourceId;
            if (!int.TryParse(hfResourceId.Value, out resourceId)) return;

            string text = txtComment.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                lblMessage.Visible = true;
                lblMessage.CssClass = "message error";
                lblMessage.Text = "Please enter a comment before posting.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO ResourceDiscussions (resourceID, userID, parentDiscussionID, message, createdAt)
                    VALUES (@ResourceId, @UserId, NULL, @Message, GETDATE());
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    cmd.Parameters.AddWithValue("@UserId", GetCurrentUserId());
                    cmd.Parameters.AddWithValue("@Message", text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                txtComment.Text = "";

                lblMessage.Visible = true;
                lblMessage.CssClass = "message success";
                lblMessage.Text = "Your comment has been posted.";

                BindDiscussion(resourceId);
            }
            catch
            {
                lblMessage.Visible = true;
                lblMessage.CssClass = "message error";
                lblMessage.Text = "There was a problem posting your comment. Please try again.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Landing.aspx");
        }

        private void ShowError(string message)
        {
            lblMessage.Visible = true;
            lblMessage.CssClass = "message error";
            lblMessage.Text = message;
        }
    }
}
