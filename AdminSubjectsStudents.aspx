<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminSubjectsStudents.aspx.cs" Inherits="BrainBlitz.AdminSubjectsStudents" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Subject Students Management - BrainBlitz</title>
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

    .student-dropdown {
        width: 100%;
        padding: 8px 12px;
        font-family: 'Sansation', sans-serif;
        font-size: 14px;
        border: 1px solid #8D97AA;
        border-radius: 6px;
        background: white;
        cursor: pointer;
        margin-bottom: 10px;
    }

    .student-dropdown:focus {
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
        margin-right: 8px;
    }

    .btn-enroll {
        background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
        color: white;
    }

    .btn-view {
        background: #4CAF50;
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

    .enrolled-count {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        font-weight: 700;
        color: #610099;
        background: #f0e6ff;
        padding: 6px 12px;
        border-radius: 20px;
        font-size: 14px;
    }

    .no-students {
        color: #8D97AA;
        font-style: italic;
    }
    .action-cell {
        min-width: 300px;
    }

    /* Modal Styles */
    .modal-overlay {
        display: none;
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        z-index: 1000;
        justify-content: center;
        align-items: center;
    }

    .modal-overlay.active {
        display: flex;
    }

    .modal-content {
        background: white;
        border-radius: 10px;
        padding: 30px;
        max-width: 600px;
        width: 90%;
        max-height: 80vh;
        overflow-y: auto;
        box-shadow: 0 4px 20px rgba(0,0,0,0.3);
    }

    .modal-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 20px;
        padding-bottom: 15px;
        border-bottom: 2px solid #f0f0f0;
    }

    .modal-title {
        font-family: 'Sansation', sans-serif;
        font-size: 24px;
        font-weight: 700;
        color: #000;
    }

    .modal-close {
        font-size: 28px;
        color: #8D97AA;
        cursor: pointer;
        background: none;
        border: none;
        line-height: 1;
    }

    .modal-close:hover {
        color: #610099;
    }

    .enrolled-list {
        list-style: none;
        padding: 0;
        margin: 0;
    }

    .enrolled-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 15px;
        border-bottom: 1px solid #f0f0f0;
        font-family: 'Sansation', sans-serif;
    }

    .enrolled-item:hover {
        background: #f9f9f9;
    }

    .student-info {
        display: flex;
        flex-direction: column;
        gap: 4px;
    }

    .student-name {
        font-weight: 700;
        color: #000;
        font-size: 16px;
    }

    .student-email {
        font-size: 14px;
        color: #8D97AA;
    }

    .btn-remove-student {
        background: #f44336;
        color: white;
        padding: 6px 16px;
        border-radius: 6px;
        border: none;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
        font-size: 13px;
        cursor: pointer;
        transition: all 0.3s ease;
    }

    .btn-remove-student:hover {
        background: #d32f2f;
        transform: translateY(-2px);
    }

    .no-enrollments {
        text-align: center;
        padding: 40px;
        color: #8D97AA;
        font-family: 'Sansation', sans-serif;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        
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
                <h1 class="page-title">Manage Subject Students</h1>
                <p class="page-subtitle">Enroll students in subjects</p>
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
                        <asp:TemplateField HeaderText="Enrolled Students">
                            <ItemTemplate>
                                <asp:Panel ID="pnlEnrolled" runat="server" Visible='<%# Convert.ToInt32(Eval("enrolledCount")) > 0 %>'>
                                    <span class="enrolled-count">
                                        👥 <%# Eval("enrolledCount") %> student(s)
                                    </span>
                                </asp:Panel>
                                <asp:Panel ID="pnlNoStudents" runat="server" Visible='<%# Convert.ToInt32(Eval("enrolledCount")) == 0 %>'>
                                    <span class="no-students">No students enrolled</span>
                                </asp:Panel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-cell">
                                    <asp:DropDownList ID="ddlStudents" runat="server" CssClass="student-dropdown"
                                        DataTextField="StudentDisplay" 
                                        DataValueField="userID">
                                    </asp:DropDownList>
                                    <div>
                                        <asp:Button ID="btnEnroll" runat="server" 
                                            CssClass="action-btn btn-enroll"
                                            Text="Enroll Student" 
                                            CommandName="EnrollStudent" 
                                            CommandArgument='<%# Eval("subjectID") %>' />
                                        <asp:Button ID="btnViewEnrolled" runat="server" 
                                            CssClass="action-btn btn-view"
                                            Text="View Enrolled" 
                                            CommandName="ViewEnrolled" 
                                            CommandArgument='<%# Eval("subjectID") %>'
                                            Visible='<%# Convert.ToInt32(Eval("enrolledCount")) > 0 %>' />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Modal for Viewing Enrolled Students -->
        <div id="enrolledModal" class="modal-overlay">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="modal-title">Enrolled Students</h2>
                    <button type="button" class="modal-close" onclick="closeModal()">&times;</button>
                </div>
                <asp:UpdatePanel ID="upEnrolledList" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnlEnrolledList" runat="server">
                            <asp:Repeater ID="rptEnrolledStudents" runat="server" OnItemCommand="rptEnrolledStudents_ItemCommand">
                                <HeaderTemplate>
                                    <ul class="enrolled-list">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="enrolled-item">
                                        <div class="student-info">
                                            <span class="student-name"><%# Eval("fullName") %></span>
                                            <span class="student-email"><%# Eval("email") %></span>
                                        </div>
                                        <asp:Button ID="btnRemove" runat="server" 
                                            CssClass="btn-remove-student"
                                            Text="Remove" 
                                            CommandName="RemoveEnrollment" 
                                            CommandArgument='<%# Eval("enrollmentID") %>'
                                            OnClientClick="return confirm('Remove this student from the subject?');" />
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:Panel ID="pnlNoEnrollments" runat="server" Visible="false">
                                <div class="no-enrollments">
                                    <p>No students enrolled in this subject.</p>
                                </div>
                            </asp:Panel>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvSubjects" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>

        <asp:HiddenField ID="hfCurrentSubjectID" runat="server" />
    </form>

    <script>
        function showModal() {
            document.getElementById('enrolledModal').classList.add('active');
        }

        function closeModal() {
            document.getElementById('enrolledModal').classList.remove('active');
        }

        // Close modal when clicking outside
        document.getElementById('enrolledModal').addEventListener('click', function(e) {
            if (e.target === this) {
                closeModal();
            }
        });
    </script>
</body>
</html>