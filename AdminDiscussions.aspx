<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDiscussions.aspx.cs" Inherits="BrainBlitz.AdminDiscussions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Discussion Moderation - BrainBlitz</title>

    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="../Content/Site.css" />
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

        .admin-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px 40px 50px;
            box-sizing: border-box;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
            margin-bottom: 24px;
        }

        .page-header-text h1 {
            font-size: 36px;
            font-weight: 700;
            margin: 0 0 6px 0;
            color: #000;
        }

        .page-header-text p {
            font-size: 18px;
            color: #8D97AA;
            margin: 0;
        }

        .filters-bar {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            margin-bottom: 20px;
        }

        .filter-select,
        .filter-search {
            background: #FFFFFF;
            border-radius: 10px;
            padding: 10px 12px;
            border: 1px solid #E0E0E0;
            font-size: 14px;
            font-family: 'Sansation', sans-serif;
        }

        .filter-search {
            min-width: 260px;
        }

        .filter-select {
            min-width: 180px;
        }

        .btn-filter {
            padding: 9px 16px;
            border-radius: 10px;
            border: none;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
            font-weight: 700;
            font-size: 14px;
            cursor: pointer;
        }

        .moderation-list {
            display: flex;
            flex-direction: column;
            gap: 16px;
            margin-top: 10px;
        }

        .moderation-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 16px 18px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.04);
            display: flex;
            flex-direction: column;
            gap: 8px;
            border-left: 4px solid #FF9800;
        }

        .moderation-card-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 10px;
        }

        .moderation-resource-title {
            font-size: 16px;
            font-weight: 700;
            margin-bottom: 2px;
        }

        .moderation-subject-pill {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 999px;
            background-color: #E6D4FF;
            color: #610099;
            font-size: 11px;
            font-weight: 700;
            margin-right: 6px;
        }

        .moderation-type-tag {
            font-size: 11px;
            color: #757575;
        }

        .moderation-status-badge {
            font-size: 11px;
            padding: 3px 8px;
            border-radius: 999px;
            background: #FFF3E0;
            color: #F57C00;
            font-weight: 700;
            white-space: nowrap;
        }

        .moderation-comment {
            font-size: 14px;
            color: #444;
        }

        .moderation-meta-row {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            font-size: 12px;
            color: #757575;
        }

        .moderation-meta-row span {
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .moderation-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 6px;
        }

        .btn-small {
            padding: 6px 10px;
            border-radius: 8px;
            border: 1px solid #E0E0E0;
            background: #FFFFFF;
            font-size: 12px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            text-decoration: none;
            color: #333;
        }

        .btn-small.view {
            border-color: #61009933;
        }

        .btn-small.clear {
            border-color: #4CAF5033;
            color: #2E7D32;
        }

        .btn-small.delete {
            border-color: #E5393533;
            color: #C62828;
        }

        .btn-small i {
            font-size: 12px;
        }

        .empty-state {
            text-align: center;
            padding: 40px 0;
            color: #888888;
            font-size: 15px;
        }

        .message {
            display: block;
            margin-bottom: 10px;
        }
        .message.error { color: #B00020; }
        .message.success { color: #008000; }

        @media (max-width: 900px) {
            .admin-container {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- header reused style --%>
        <div class="header">
            <div class="header-content">
                <a href="AdminDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server"
                        CssClass="header-logout-btn"
                        OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="admin-container">

            <div class="page-header">
                <div class="page-header-text">
                    <h1>Discussion Moderation</h1>
                    <p>Review and manage flagged comments across all resources</p>
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>

            <%-- Filters --%>
            <div class="filters-bar">
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-select">
                    <asp:ListItem Text="Flagged only" Value="flagged" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="All comments" Value="all"></asp:ListItem>
                </asp:DropDownList>

                <asp:TextBox ID="txtSearch" runat="server"
                    CssClass="filter-search"
                    Placeholder="Search by resource or comment text..."></asp:TextBox>

                <asp:Button ID="btnApplyFilters" runat="server"
                    CssClass="btn-filter"
                    Text="Apply"
                    OnClick="btnApplyFilters_Click" />
            </div>

            <%-- Moderation list --%>
            <asp:Repeater ID="rptDiscussions" runat="server" OnItemCommand="rptDiscussions_ItemCommand">
                <ItemTemplate>
                    <div class="moderation-card">
                        <div class="moderation-card-header">
                            <div>
                                <div class="moderation-resource-title">
                                    <%# Eval("ResourceTitle") %>
                                </div>
                                <div>
                                    <span class="moderation-subject-pill"><%# Eval("SubjectName") %></span>
                                    <span class="moderation-type-tag"><%# Eval("Type") %></span>
                                </div>
                            </div>
                            <asp:Label ID="lblStatus" runat="server"
                                       CssClass="moderation-status-badge"
                                       Text='<%# (Convert.ToBoolean(Eval("IsFlagged")) ? "Flagged" : "Active") %>'>
                            </asp:Label>
                        </div>

                        <div class="moderation-comment">
                            <strong><%# Eval("AuthorName") %>:</strong>
                            <%# Eval("MessageShort") %>
                        </div>

                        <div class="moderation-meta-row">
                            <span><i class="fas fa-clock"></i><%# Eval("CreatedAtDisplay") %></span>
                            <asp:PlaceHolder ID="phFlagInfo" runat="server" Visible='<%# Convert.ToBoolean(Eval("IsFlagged")) %>'>
                                <span><i class="fas fa-flag"></i>Flagged by <%# Eval("FlaggedByName") %></span>
                                <span><i class="fas fa-calendar"></i><%# Eval("FlaggedAtDisplay") %></span>
                            </asp:PlaceHolder>
                        </div>

                        <div class="moderation-actions">
                            <asp:HyperLink ID="hlView" runat="server"
                                CssClass="btn-small view"
                                NavigateUrl='<%# "~/StudentResourceView.aspx?id=" + Eval("ResourceId") + "&from=admin" %>'>
                                <i class="fas fa-eye"></i>
                                <span>View resource</span>
                            </asp:HyperLink>

                            <asp:LinkButton ID="btnClearFlag" runat="server"
                                CssClass="btn-small clear"
                                CommandName="ClearFlag"
                                CommandArgument='<%# Eval("DiscussionId") %>'
                                Visible='<%# Convert.ToBoolean(Eval("IsFlagged")) %>'>
                                <i class="fas fa-check"></i>
                                <span>Clear flag</span>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnDelete" runat="server"
                                CssClass="btn-small delete"
                                CommandName="DeleteComment"
                                CommandArgument='<%# Eval("DiscussionId") %>'
                                OnClientClick="return confirm('Delete this comment and its replies?');">
                                <i class="fas fa-trash-alt"></i>
                                <span>Delete</span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                No comments matching the current filters.
            </asp:Panel>

        </div>
    </form>
</body>
</html>