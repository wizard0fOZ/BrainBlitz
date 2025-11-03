<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSubjectsTeachers.aspx.cs" Inherits="BrainBlitz.AdminSubjectsTeachers" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Subject Teachers Management - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
<style>
    .header-buttons-wrapper {
        display: flex;
        align-items: center;
        gap: 48px;
    }
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

    .content-container {
        padding: 40px 60px;
        max-width: 1440px;
        margin: 0 auto;
        background: #F3F3F3;
        min-height: calc(100vh - 75px);
    }

    .page-header {
        margin-bottom: 30px;
    }

    .page-title {
        font-family: 'Sansation', sans-serif;
        font-size: 36px;
        font-weight: 700;
        color: #000;
        margin-bottom: 8px;
    }

    .page-subtitle {
        font-family: 'Sansation', sans-serif;
        font-size: 20px;
        font-weight: 700;
        color: #8D97AA;
    }

    .back-link {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        font-family: 'Sansation', sans-serif;
        font-size: 16px;
        font-weight: 700;
        color: #610099;
        text-decoration: none;
        margin-bottom: 20px;
        transition: transform 0.2s ease;
    }

    .back-link:hover {
        transform: translateX(-5px);
    }

    .main-content {
        background: white;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .message-box {
        padding: 15px 20px;
        border-radius: 8px;
        margin-bottom: 20px;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
    }

    .message-success {
        background: #d4edda;
        color: #155724;
        border: 1px solid #c3e6cb;
    }

    .message-error {
        background: #f8d7da;
        color: #721c24;
        border: 1px solid #f5c6cb;
    }

    .subjects-grid {
        width: 100%;
        border-collapse: collapse;
        font-family: 'Sansation', sans-serif;
    }

    .subjects-grid thead {
        background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
        color: white;
    }

    .subjects-grid th {
        padding: 15px;
        text-align: left;
        font-weight: 700;
        font-size: 16px;
    }

    .subjects-grid td {
        padding: 15px;
        font-size: 14px;
        color: #333;
        border-bottom: 1px solid #f0f0f0;
    }

    .subjects-grid tr:hover {
        background: #f9f9f9;
    }

    .teacher-dropdown {
        width: 100%;
        padding: 8px 12px;
        margin-bottom: 10px;
        font-family: 'Sansation', sans-serif;
        font-size: 14px;
        border: 1px solid #8D97AA;
        border-radius: 6px;
        background: white;
        cursor: pointer;
    }

    .teacher-dropdown:focus {
        outline: none;
        border-color: #610099;
    }

    .action-btn {
        padding: 8px 20px;
        border-radius: 6px;
        border: none;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
        font-size: 14px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .btn-assign {
        background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
        color: white;
    }

    .btn-remove {
        background: #f44336;
        color: white;
    }

    .action-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(0,0,0,0.2);
    }

    .action-btn:disabled {
        background: #ccc;
        cursor: not-allowed;
        transform: none;
    }

    .teacher-info {
        display: flex;
        flex-direction: column;
        gap: 4px;
    }

    .teacher-name {
        font-weight: 700;
        color: #000;
    }

    .teacher-email {
        font-size: 12px;
        color: #8D97AA;
    }

    .no-teacher {
        color: #8D97AA;
        font-style: italic;
    }

    .empty-state {
        text-align: center;
        padding: 40px;
        color: #8D97AA;
        font-family: 'Sansation', sans-serif;
        font-size: 16px;
    }

    .teacher-cell {
        min-width: 200px;
    }

    .action-cell {
        min-width: 250px;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <a href="AdminDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">
                        <span>Log out</span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="content-container">
            <a href="AdminSubjects.aspx" class="back-link">← Back to Subjects</a>
            
            <div class="page-header">
                <h1 class="page-title">Manage Subject Teachers</h1>
                <p class="page-subtitle">Assign teachers to subjects</p>
            </div>

            <!-- Message Display -->
            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                <div class="message-box" id="messageBox" runat="server">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
            </asp:Panel>

            <div class="main-content">
                <asp:GridView ID="gvSubjects" runat="server" 
                    CssClass="subjects-grid" 
                    AutoGenerateColumns="False" 
                    OnRowCommand="gvSubjects_RowCommand"
                    DataKeyNames="subjectID"
                    EmptyDataText="No subjects found.">
                    <Columns>
                        <asp:BoundField DataField="subjectID" HeaderText="ID" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="name" HeaderText="Subject Name" ItemStyle-Width="200px" />
                        <asp:TemplateField HeaderText="Current Teacher">
                            <ItemTemplate>
                                <div class="teacher-cell">
                                    <asp:Panel ID="pnlTeacher" runat="server" Visible='<%# Eval("assignedTo") != DBNull.Value %>'>
                                        <div class="teacher-info">
                                            <span class="teacher-name"><%# Eval("teacherName") %></span>
                                            <span class="teacher-email"><%# Eval("teacherEmail") %></span>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlNoTeacher" runat="server" Visible='<%# Eval("assignedTo") == DBNull.Value %>'>
                                        <span class="no-teacher">No teacher assigned</span>
                                    </asp:Panel>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-cell">
                                    <asp:DropDownList ID="ddlTeachers" runat="server" CssClass="teacher-dropdown"
                                        DataTextField="TeacherDisplay" 
                                        DataValueField="userID">
                                    </asp:DropDownList>
                                    <asp:Button ID="btnAssign" runat="server" 
                                        CssClass="action-btn btn-assign"
                                        Text="Assign" 
                                        CommandName="AssignTeacher" 
                                        CommandArgument='<%# Eval("subjectID") %>' />
                                    <asp:Button ID="btnRemove" runat="server" 
                                        CssClass="action-btn btn-remove"
                                        Text="Remove" 
                                        CommandName="RemoveTeacher" 
                                        CommandArgument='<%# Eval("subjectID") %>'
                                        Visible='<%# Eval("assignedTo") != DBNull.Value %>'
                                        OnClientClick="return confirm('Are you sure you want to remove this teacher from the subject?');" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            No subjects available. Please create subjects first.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>