<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentResources.aspx.cs" Inherits="BrainBlitz.StudentResources" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Learning Resources</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="../Content/Site.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        body {
            background-color: #F3F3F3;
            font-family: 'Sansation', sans-serif;
        }

        /* Main page wrapper (similar to quiz page) */
        .resources-page-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
        }

        .page-header {
            margin-bottom: 30px;
        }
        .page-title {
            font-size: 36px;
            font-weight: 700;
            color: #000;
            margin-bottom: 5px;
        }
        .page-subtitle {
            font-size: 20px;
            color: #8D97AA;
            font-weight: 400;
        }

        /* Top bar with search + subject filter */
        .top-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 40px;
            gap: 20px;
            background: #FFFFFF;
            padding: 12px 20px;
            border-radius: 12px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .search-input-wrapper {
            display: flex;
            align-items: center;
            flex-grow: 1;
            gap: 15px;
        }

        .search-input-wrapper i {
            color: #8D97AA;
            font-size: 20px;
        }

        .search-input {
            border: none;
            outline: none;
            font-family: 'Sansation';
            font-size: 16px;
            color: #333;
            width: 100%;
        }

        .search-input::placeholder {
            color: #8D97AA;
        }

        .subject-filter-wrapper {
            display: flex;
            align-items: center;
        }

        .subject-filter {
            border-radius: 8px;
            padding: 10px 16px;
            border: 1px solid #E0E0E0;
            font-family: 'Sansation';
            font-size: 14px;
            cursor: pointer;
            background-color: #F3F3F3;
            min-width: 180px;
        }

        .subject-filter:focus {
            outline: none;
            border-color: #610099;
        }

        .search-button {
            margin-left: 10px;
            padding: 10px 18px;
            border-radius: 8px;
            border: none;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
            font-family: 'Sansation';
            font-weight: 700;
            cursor: pointer;
        }

        .search-button:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        /* Resource card grid */
        .resource-card-container {
            display: flex;
            flex-direction: row;
            flex-wrap: wrap;
            justify-content: center;
            gap: 30px;
        }

        .resource-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 20px;
            display: flex;
            flex-direction: column;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            flex-basis: 350px;
            max-width: 360px;
            box-sizing: border-box;
        }

        .resource-card-header {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 8px;
        }

        .resource-icon {
            width: 40px;
            height: 40px;
            border-radius: 12px;
            background-color: #F5EAFE;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            color: #610099;
        }

        .resource-title {
            font-size: 20px;
            font-weight: 700;
            color: #000;
        }

        .subject-pill {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            background-color: #E6D4FF;
            color: #610099;
            margin: 4px 0 10px 0;
        }

        .resource-description {
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            color: #757575;
            margin-bottom: 18px;
            min-height: 40px;
        }

        .resource-card-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
            margin-top: auto;
        }

        .view-resource-btn {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 8px;
            flex: 1;
            padding: 10px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            color: #FFFFFF;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 14px;
            text-decoration: none;
            border: none;
            text-align: center;
        }

        .view-resource-btn:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        .bookmark-btn {
            border: none;
            background: none;
            cursor: pointer;
            font-size: 20px;
            padding: 4px 6px;
        }

        .bookmark-btn.bookmarked {
            color: #FFC107;
        }

        .bookmark-btn:not(.bookmarked) {
            color: #9E9E9E;
        }

        .empty-state {
            text-align: center;
            padding: 50px;
            color: #888;
            font-size: 18px;
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

        @media (max-width: 900px) {
            .resources-page-container {
                padding: 20px;
            }
            .resource-card {
                flex-basis: 100%;
                max-width: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <%-- Reuse same header style as StudentQuiz --%>
        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="resources-page-container">

            <div class="page-header">
                <h1 class="page-title">Learning Resources</h1>
                <p class="page-subtitle">Access study materials and guides</p>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>

            <%-- top bar: search + subject filter --%>
            <div class="top-bar">
                <div class="search-input-wrapper">
                    <i class="fas fa-search"></i>
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="search-input"
                        Placeholder="Search resources..."
                        AutoPostBack="true"
                        OnTextChanged="txtSearch_TextChanged" />
                </div>

                <div class="subject-filter-wrapper">
                    <asp:DropDownList ID="ddlSubjectFilter" runat="server"
                        CssClass="subject-filter"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlSubjectFilter_SelectedIndexChanged">
                    </asp:DropDownList>

                    <asp:Button ID="btnClear" runat="server"
                        Text="Clear"
                        CssClass="search-button"
                        OnClick="btnClear_Click" />
                </div>
            </div>

            <%-- card grid --%>
            <div class="resource-card-container">
                <asp:Repeater ID="rptResources" runat="server" OnItemCommand="rptResources_ItemCommand">
                    <ItemTemplate>
                        <div class="resource-card">
                            <div class="resource-card-header">
                                <div class="resource-icon">
                                    <i class="fas fa-book"></i>
                                </div>
                                <div class="resource-title"><%# Eval("Title") %></div>
                            </div>

                            <span class="subject-pill"><%# Eval("SubjectName") %></span>

                            <div class="resource-description">
                                <%# Eval("ShortDescription") %>
                            </div>

                            <div class="resource-card-footer">
                                <asp:HyperLink ID="lnkView" runat="server"
                                    CssClass="view-resource-btn"
                                    NavigateUrl='<%# "~/StudentResourceView.aspx?id=" + Eval("ResourceId") %>'>
                                    <i class="fas fa-eye"></i>
                                    <span>View Resource</span>
                                </asp:HyperLink>

                                <asp:LinkButton ID="btnBookmark" runat="server"
                                    CommandName="ToggleBookmark"
                                    CommandArgument='<%# Eval("ResourceId") %>'
                                    CssClass='<%# "bookmark-btn " + (Convert.ToBoolean(Eval("IsBookmarked")) ? "bookmarked" : "") %>'
                                    ToolTip="Bookmark">
                                    <%# Convert.ToBoolean(Eval("IsBookmarked")) ? "🔖" : "📑" %>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                No resources found for this filter.
            </asp:Panel>

        </div>
    </form>
</body>
</html>