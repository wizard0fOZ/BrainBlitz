<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherResources.aspx.cs" Inherits="BrainBlitz.TeacherResources" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Learning Resources - Teacher - BrainBlitz</title>

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

        .teacher-resources-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 30px;
            gap: 20px;
        }

        .page-header-text h1 {
            font-size: 36px;
            font-weight: 700;
            margin-bottom: 5px;
            color: #000;
        }

        .page-header-text p {
            font-size: 20px;
            color: #8D97AA;
        }

        .btn-upload {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            padding: 10px 20px;
            border-radius: 10px;
            border: none;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
            font-weight: 700;
            font-size: 16px;
            cursor: pointer;
            text-decoration: none;
        }

        .btn-upload i {
            font-size: 14px;
        }

        .btn-upload:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        .search-card {
            background: #FFFFFF;
            padding: 16px 20px;
            border-radius: 12px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            margin-bottom: 30px;
        }

        .search-input-wrapper {
            position: relative;
        }

        .search-input-wrapper i {
            position: absolute;
            left: 12px;
            top: 50%;
            transform: translateY(-50%);
            color: #8D97AA;
            font-size: 18px;
        }

        .search-input {
            width: 100%;
            padding: 10px 12px 10px 36px;
            border-radius: 8px;
            border: none;
            outline: none;
            font-size: 14px;
            font-family: 'Sansation';
        }

        .search-input::placeholder {
            color: #8D97AA;
        }

        .resources-grid {
            display: flex;
            flex-wrap: wrap;
            gap: 24px;
        }

        .resource-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            flex-basis: 350px;
            max-width: 360px;
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
        }

        .resource-card-header {
            display: flex;
            align-items: flex-start;
            gap: 14px;
            margin-bottom: 10px;
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
            flex-shrink: 0;
        }

        .resource-title {
            font-size: 18px;
            font-weight: 700;
            margin-bottom: 4px;
        }

        .subject-pill {
            display: inline-block;
            padding: 3px 10px;
            border-radius: 999px;
            background-color: #E6D4FF;
            color: #610099;
            font-size: 11px;
            font-weight: 700;
        }

        .type-label {
            font-size: 11px;
            color: #757575;
            margin-left: 6px;
        }

        .resource-meta-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            border-top: 1px solid #F0F0F0;
            border-bottom: 1px solid #F0F0F0;
            padding: 10px 0;
            margin: 10px 0;
            font-size: 12px;
            color: #757575;
        }

        .meta-label {
            color: #9E9E9E;
            margin-bottom: 2px;
        }

        .meta-value {
            font-weight: 600;
            color: #424242;
        }

        .resource-actions {
            display: flex;
            gap: 8px;
            margin-top: 8px;
        }

        .btn-view,
        .btn-edit {
            flex: 1;
            border-radius: 8px;
            border: 1px solid #E0E0E0;
            background: #FFFFFF;
            font-size: 13px;
            padding: 8px 10px;
            cursor: pointer;
            text-align: center;
            text-decoration: none;
            color: #333;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-delete {
            flex: 0 0 40px;
            padding: 8px;
            border-radius: 8px;
            border: 1px solid #E0E0E0;
            background: #FFFFFF;
            font-size: 13px;
            cursor: pointer;
            text-align: center;
            text-decoration: none;
            color: #C62828;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-delete {
            flex: 0 0 40px;
            padding: 8px;
            color: #C62828;
        }

        .btn-edit i, .btn-delete i {
            font-size: 14px;
        }

        .empty-state {
            text-align: center;
            padding: 40px 0;
            color: #888888;
            font-size: 16px;
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
            .teacher-resources-container {
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

        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="teacher-resources-container">
            <div class="page-header">
                <div class="page-header-text">
                    <h1>Learning Resources</h1>
                    <p>Manage materials for your students</p>
                </div>

                <asp:Button ID="btnUploadResource" runat="server"
                    CssClass="btn-upload"
                    Text="+ Upload Resource"
                    OnClick="btnUploadResource_Click" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>

            <div class="search-card">
                <div class="search-input-wrapper">
                    <i class="fas fa-search"></i>
                    <asp:TextBox ID="txtSearch" runat="server"
                        CssClass="search-input"
                        Placeholder="Search resources..."
                        AutoPostBack="true"
                        OnTextChanged="txtSearch_TextChanged" />
                </div>
            </div>

            <div class="resources-grid">
                <asp:Repeater ID="rptResources" runat="server" OnItemCommand="rptResources_ItemCommand">
                    <ItemTemplate>
                        <div class="resource-card">
                            <div class="resource-card-header">
                                <div class="resource-icon">
                                    <i class="fas fa-book"></i>
                                </div>
                                <div class="resource-card-header-text">
                                    <div class="resource-title"><%# Eval("Title") %></div>
                                    <span class="subject-pill"><%# Eval("SubjectName") %></span>
                                    <span class="type-label"><%# Eval("Type") %></span>
                                </div>
                            </div>

                            <div class="resource-meta-row">
                                <div>
                                    <div class="meta-label">Created</div>
                                    <div class="meta-value"><%# Eval("CreatedAtFormatted") %></div>
                                </div>
                                <div>
                                    <div class="meta-label">ID</div>
                                    <div class="meta-value">#<%# Eval("ResourceId") %></div>
                                </div>
                            </div>

                            <div class="resource-actions">
                                <asp:HyperLink ID="hlView" runat="server"
                                    CssClass="btn-view"
                                    NavigateUrl='<%# "~/StudentResourceView.aspx?id=" + Eval("ResourceId") + "&from=teacher" %>'
                                    Target="_blank">
                                    <i class="fas fa-eye"></i>
                                    <span>View</span>
                                </asp:HyperLink>

                                <asp:HyperLink ID="hlEdit" runat="server"
                                    CssClass="btn-edit"
                                    NavigateUrl='<%# "~/TeacherResourceEditor.aspx?id=" + Eval("ResourceId") %>'>
                                    <i class="fas fa-edit"></i>
                                    <span>Edit</span>
                                </asp:HyperLink>

                                <asp:LinkButton ID="btnDelete" runat="server"
                                    CssClass="btn-delete"
                                    CommandName="DeleteResource"
                                    CommandArgument='<%# Eval("ResourceId") %>'
                                    OnClientClick="return confirm('Delete this resource?');">
                                    <i class="fas fa-trash-alt"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                No resources found. Create your first resource to get started!
            </asp:Panel>

        </div>
    </form>
</body>
</html>