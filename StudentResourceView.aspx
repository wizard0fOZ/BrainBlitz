<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentResourceView.aspx.cs" Inherits="BrainBlitz.StudentResourceView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>View Resource - BrainBlitz</title>

    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="Content/Site.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        .header-logout-btn {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 9px 38px;
            position: relative;
            height: 40px;
            top: auto;
            right: auto;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
            color: #fff;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            text-align: center;
            text-decoration: none;
            width: 145px;
        }

        .header-logout-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
        }

        body {
            background-color: #F3F3F3;
            font-family: 'Sansation', sans-serif;
        }

        .resource-page-container {
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
        }

        .back-link {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 14px;
            color: #610099;
            text-decoration: none;
            margin-bottom: 20px;
        }

        .back-link i {
            font-size: 12px;
        }

        .main-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 24px 28px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            margin-bottom: 24px;
        }

        .resource-title {
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 10px;
        }

        .meta-line {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            font-size: 13px;
            color: #757575;
            margin-bottom: 16px;
        }

        .subject-pill {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            background-color: #E6D4FF;
            color: #610099;
        }

        .meta-dot {
            width: 4px;
            height: 4px;
            border-radius: 999px;
            background: #C4C4C4;
            align-self: center;
        }

        .resource-description-full {
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            color: #555555;
            margin-bottom: 20px;
        }

        .button-row {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .primary-btn,
        .secondary-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 16px;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 700;
            text-decoration: none;
            cursor: pointer;
            border: none;
        }

        .primary-btn {
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
        }

        .primary-btn:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        .secondary-btn {
            background: #FFFFFF;
            color: #333333;
            border: 1px solid #E0E0E0;
        }

        .secondary-btn.active {
            background: #E8F5E9;
            color: #2E7D32;
            border-color: #81C784;
        }

        .secondary-btn[disabled] {
            opacity: 0.7;
            cursor: default;
        }

        .section-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 20px 24px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            margin-bottom: 24px;
        }

        .section-title {
            font-family: 'Inter', sans-serif;
            font-weight: 600;
            font-size: 20px;
            margin-bottom: 16px;
        }

        .preview-placeholder {
            background: #FAF5FF;
            border-radius: 10px;
            padding: 24px;
            text-align: center;
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            color: #8D97AA;
        }

        .message {
            display: block;
            margin-bottom: 10px;
        }
        .message.error {
            color: #B00020;
        }
        .message.success {
            color: #008000;
        }

        .discussion-card {
            background: #ffffff;
            border-radius: 10px;
            padding: 20px 22px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.03);
            display: flex;
            flex-direction: column;
            gap: 16px;
        }

        .discussion-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .discussion-title {
            font-weight: 700;
            font-size: 20px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .discussion-title i {
            color: #610099;
        }

        .discussion-empty {
            font-size: 14px;
            color: #8D97AA;
            text-align: center;
            margin: 16px 0;
        }

        .comment-item {
            border-radius: 10px;
            border: 1px solid #E0E0E0;
            padding: 10px 12px;
            margin-bottom: 10px;
            background: #F9F9FF;
        }

        .comment-meta {
            display: flex;
            justify-content: space-between;
            font-size: 12px;
            margin-bottom: 4px;
        }

        .comment-author {
            font-weight: 700;
            color: #333;
        }

        .comment-time {
            color: #8D97AA;
        }

        .comment-body {
            font-size: 14px;
            color: #444;
        }

        .reply-item {
            margin-top: 8px;
            margin-left: 20px;
            border-radius: 8px;
            border: 1px solid #E0E0E0;
            padding: 8px 10px;
            background: #FFFFFF;
        }

        .reply-form {
            margin-top: 8px;
            margin-left: 20px;
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .comment-form {
            margin-top: 10px;
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .comment-input {
            width: 100%;
            border-radius: 10px;
            border: 1px solid #D0D0D0;
            padding: 8px 10px;
            font-family: 'Sansation', sans-serif;
            resize: vertical;
        }

        .comment-button,
        .reply-button {
            align-self: flex-end;
            padding: 6px 14px;
            border-radius: 10px;
            border: none;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #fff;
            font-weight: 700;
            font-size: 12px;
            text-decoration: none;
            cursor: pointer;
            display: inline-block;
        }

        .comment-button:hover,
        .reply-button:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        .comment-flag-link {
            font-size: 12px;
            color: #610099;
            text-decoration: underline;
            margin-left: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="header">
            <div class="header-content">
                <a id="lnkLogo" runat="server" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="resource-page-container">

            <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>

            <asp:Panel ID="pnlContent" runat="server">

                <a id="lnkBack" runat="server" class="back-link">
                    <i class="fas fa-arrow-left"></i>
                    <span>Back to Resources</span>
                </a>

                <asp:HiddenField ID="hfResourceId" runat="server" />

                <div class="main-card">
                    <div class="resource-title">
                        <asp:Label ID="lblTitle" runat="server" />
                    </div>

                    <div class="meta-line">
                        <span class="subject-pill">
                            <asp:Label ID="lblSubject" runat="server" />
                        </span>

                        <div class="meta-dot"></div>
                        <span><asp:Label ID="lblType" runat="server" /></span>

                        <div class="meta-dot"></div>
                        <span>By <asp:Label ID="lblTeacher" runat="server" /></span>
                    </div>

                    <div class="resource-description-full">
                        <asp:Label ID="lblDescription" runat="server" />
                    </div>

                    <div class="button-row">
                        <asp:HyperLink ID="hlDownload" runat="server" CssClass="primary-btn" Target="_blank">
                            <i class="fas fa-download"></i>
                            <span>Download Resource</span>
                        </asp:HyperLink>

                        <asp:LinkButton ID="btnBookmark" runat="server" CssClass="secondary-btn" OnClick="btnBookmark_Click">
                            <i class="far fa-bookmark"></i>
                            <span>Bookmark</span>
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnMarkComplete" runat="server" CssClass="secondary-btn" OnClick="btnMarkComplete_Click">
                            <i class="fas fa-check"></i>
                            <span>Mark Complete</span>
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="section-card">
                    <div class="section-title">Resource Preview</div>
                    <asp:Panel ID="pnlPreview" runat="server" CssClass="preview-placeholder"></asp:Panel>
                </div>

                <div class="discussion-card">
                    <div class="discussion-header">
                        <span class="discussion-title">
                            <i class="fas fa-comment-dots"></i>
                            Discussion (<asp:Label ID="lblCommentCount" runat="server" Text="0" />)
                        </span>
                    </div>

                    <asp:Panel ID="pnlNoComments" runat="server" Visible="false">
                        <div class="discussion-empty">
                            No comments yet. Start the conversation!
                        </div>
                    </asp:Panel>

                    <asp:Repeater ID="rptThreads" runat="server"
                                  OnItemDataBound="rptThreads_ItemDataBound"
                                  OnItemCommand="rptThreads_ItemCommand">
                        <ItemTemplate>
                            <div class="comment-item">
                                <div class="comment-meta">
                                    <span class="comment-author"><%# Eval("AuthorName") %></span>
                                    <span class="comment-time"><%# Eval("TimeAgo") %></span>

                                    <asp:LinkButton ID="btnFlag" runat="server"
                                        CommandName="Flag"
                                        CommandArgument='<%# Eval("DiscussionID") %>'
                                        CssClass="comment-flag-link"
                                        Visible='<%# Session["Role"] != null && Session["Role"].ToString() == "Teacher" %>'>
                                        Report
                                    </asp:LinkButton>
                                </div>
                                <div class="comment-body"><%# Eval("Message") %></div>
                                    <asp:Repeater ID="rptReplies" runat="server"
                                                  OnItemCommand="rptThreads_ItemCommand">
                                        <ItemTemplate>
                                            <div class="reply-item">
                                                <div class="comment-meta">
                                                    <span class="comment-author"><%# Eval("AuthorName") %></span>
                                                    <span class="comment-time"><%# Eval("TimeAgo") %></span>

                                                    <asp:LinkButton ID="btnFlagReply" runat="server"
                                                        CommandName="Flag"
                                                        CommandArgument='<%# Eval("DiscussionID") %>'
                                                        CssClass="comment-flag-link"
                                                        CausesValidation="false"
                                                        Visible='<%# Session["Role"] != null && Session["Role"].ToString() == "Teacher" %>'>
                                                        Report
                                                    </asp:LinkButton>
                                                </div>

                                                <div class="comment-body"><%# Eval("Message") %></div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                <div class="reply-form">
                                    <asp:TextBox ID="txtReply" runat="server"
                                                 CssClass="comment-input"
                                                 TextMode="MultiLine"
                                                 Rows="2"
                                                 placeholder="Reply to this comment..."></asp:TextBox>

                                    <asp:LinkButton ID="btnReply" runat="server"
                                                    CommandName="Reply"
                                                    CommandArgument='<%# Eval("DiscussionID") %>'
                                                    CssClass="reply-button">
                                        Reply
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="comment-form">
                        <asp:TextBox ID="txtComment" runat="server"
                                     CssClass="comment-input"
                                     TextMode="MultiLine"
                                     Rows="3"
                                     placeholder="Share your thoughts or ask a question..."></asp:TextBox>

                        <asp:Button ID="btnPostComment" runat="server"
                                    Text="Post Comment"
                                    CssClass="comment-button"
                                    OnClick="btnPostComment_Click" />
                    </div>
                </div>

            </asp:Panel>

        </div>
    </form>
</body>
</html>