<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSubjects.aspx.cs" Inherits="BrainBlitz.AdminSubjects" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Subject Management - BrainBlitz</title>
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

    .main-content {
        display: flex;
        gap: 30px;
        margin-bottom: 30px;
    }

    .create-subject-card {
        background: white;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        flex: 1;
        max-width: 500px;
    }

    .card-title {
        font-family: 'Sansation', sans-serif;
        font-size: 24px;
        font-weight: 700;
        color: #000;
        margin-bottom: 20px;
    }

    .form-group {
        margin-bottom: 20px;
    }

    .form-label {
        display: block;
        font-family: 'Sansation', sans-serif;
        font-size: 16px;
        font-weight: 700;
        color: #333;
        margin-bottom: 8px;
    }

    .form-input {
        width: 100%;
        padding: 12px 15px;
        font-family: 'Sansation', sans-serif;
        font-size: 14px;
        border: 1px solid #8D97AA;
        border-radius: 8px;
        box-sizing: border-box;
    }

    .form-textarea {
        width: 100%;
        padding: 12px 15px;
        font-family: 'Sansation', sans-serif;
        font-size: 14px;
        border: 1px solid #8D97AA;
        border-radius: 8px;
        box-sizing: border-box;
        resize: vertical;
        min-height: 100px;
    }

    .form-input:focus, .form-textarea:focus {
        outline: none;
        border-color: #610099;
    }

    .btn-create {
        width: 100%;
        padding: 12px;
        background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
        border: none;
        border-radius: 10px;
        color: white;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
        font-size: 18px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .btn-create:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
    }

    .subjects-list-card {
        background: white;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        flex: 2;
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

    .action-btn {
        padding: 6px 15px;
        border-radius: 6px;
        border: none;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
        font-size: 12px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .btn-delete {
        background: #f44336;
        color: white;
    }

    .action-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(0,0,0,0.2);
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

    .action-cards-container {
        display: flex;
        flex-direction: row;
        justify-content: center;
        gap: 30px;
        margin-top: 30px;
    }

    .action-card {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        padding: 24px;
        background: #FFFFFF;
        border-radius: 10px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
        text-decoration: none;
        color: inherit;
        width: 426px;
        min-width: 0;
        gap: 8px;
        transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

    .action-card:hover {
        transform: translateY(-3px);
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }

    .action-card-icon {
        width: 40px;
        height: 40px;
        background-size: contain;
        background-repeat: no-repeat;
        background-position: center;
        margin-bottom: 8px;
    }

    .manage-teachers-icon { background-image: url('/Images/Teachers.png'); }
    .manage-students-icon { background-image: url('/Images/Students.png'); }

    .action-card-title {
        font-family: 'Sansation';
        font-weight: 700;
        font-size: 20px;
        color: #000000;
    }

    .action-card-subtitle {
        font-family: 'Sansation';
        font-weight: 400;
        font-size: 14px;
        color: #8D97AA;
        line-height: 1.4;
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
            <div class="page-header">
                <h1 class="page-title">Subject Management</h1>
                <p class="page-subtitle">Create and manage subjects for your courses</p>
            </div>

            <!-- Message Display -->
            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                <div class="message-box" id="messageBox" runat="server">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
            </asp:Panel>

            <div class="main-content">
                <!-- Create Subject Form -->
                <div class="create-subject-card">
                    <h2 class="card-title">Create New Subject</h2>
                    <div class="form-group">
                        <label class="form-label">Subject Name</label>
                        <asp:TextBox ID="txtSubjectName" runat="server" CssClass="form-input" 
                            placeholder="e.g., Mathematics, Physics, Chemistry" MaxLength="100"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvSubjectName" runat="server" 
                            ControlToValidate="txtSubjectName" 
                            ErrorMessage="Subject name is required" 
                            ForeColor="Red" 
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-textarea" 
                            TextMode="MultiLine" 
                            placeholder="Enter a brief description of the subject..." 
                            MaxLength="255"></asp:TextBox>
                    </div>
                    <asp:Button ID="btnCreateSubject" runat="server" CssClass="btn-create" 
                        Text="Create Subject" OnClick="btnCreateSubject_Click" />
                </div>

                <!-- Subjects List -->
                <div class="subjects-list-card">
                    <h2 class="card-title">Existing Subjects</h2>
                    <asp:GridView ID="gvSubjects" runat="server" 
                        CssClass="subjects-grid" 
                        AutoGenerateColumns="False" 
                        OnRowCommand="gvSubjects_RowCommand"
                        DataKeyNames="subjectID"
                        EmptyDataText="No subjects found. Create your first subject!">
                        <Columns>
                            <asp:BoundField DataField="subjectID" HeaderText="ID" />
                            <asp:BoundField DataField="name" HeaderText="Subject Name" />
                            <asp:BoundField DataField="description" HeaderText="Description" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnDelete" runat="server" 
                                        CssClass="action-btn btn-delete"
                                        Text="Delete" 
                                        CommandName="DeleteSubject" 
                                        CommandArgument='<%# Eval("subjectID") %>'
                                        OnClientClick="return confirm('Are you sure you want to delete this subject? This action cannot be undone.');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <!-- Action Cards for Teachers and Students -->
            <div class="action-cards-container">
                <a href="AdminSubjectsTeachers.aspx" class="action-card">
                    <div class="action-card-icon manage-teachers-icon"></div>
                    <span class="action-card-title">Manage Teachers Subjects</span>
                    <span class="action-card-subtitle">Assign teachers to subjects</span>
                </a>
                <a href="AdminSubjectsStudents.aspx" class="action-card">
                    <div class="action-card-icon manage-students-icon"></div>
                    <span class="action-card-title">Manage Students Subjects</span>
                    <span class="action-card-subtitle">Enroll students in subjects</span>
                </a>
            </div>
        </div>
    </form>
</body>
</html>