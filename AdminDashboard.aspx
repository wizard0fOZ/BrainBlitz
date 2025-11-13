<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="BrainBlitz.AdminDashboard" Async="true" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Dashboard - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

<style>
    .dashboard-container {
        position: relative;
        width: 100%;
        min-height: calc(1024px - 75px);
        background: #F3F3F3;
        margin: 0 auto;
        padding-bottom: 40px;
        max-width: 1440px;
        box-sizing: border-box;
    }

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
        width: 132px;
    }
    .header-logout-btn {
        width: 145px;
    }

    .header-logout-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
    }

    .slogan-div {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        align-items: flex-start;
        padding: 5px 50px 20px;
        box-sizing: border-box;
        margin-top: 15px;
    }

    .dashboard-title {
        font-family: 'Sansation';
        font-weight: 700;
        font-size: 36px;
        line-height: 40px;
        color: #000;
    }

    .dashboard-subtitle {
        font-family: 'Sansation';
        font-weight: 700;
        font-size: 20px;
        line-height: 22px;
        color: #8D97AA;
    }

    .summary-container {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        padding: 0 50px;
        box-sizing: border-box;
        margin-top: 30px;
    }

    .summary-card {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        padding: 30px 40px;
        gap: 20px;
        width: 307px;
        height: 177px;
        background: #FFFFFF;
        border-radius: 10px;
        box-sizing: border-box;
    }

    .summary-title {
        font-family: 'Sansation';
        font-weight: 700;
        font-size: 24px;
        line-height: 27px;
        color: #8D97AA;
        margin: 0;
    }

    .summary-value {
        font-family: 'Sansation';
        font-weight: 400;
        font-size: 36px;
        line-height: 40px;
        color: #000000;
    }

    .summary-card-link {
    text-decoration: none;
    color: inherit;
    display: flex;
    transition: transform 0.2s ease;
    }

    .summary-card-link:hover {
     transform: translateY(-3px);
     box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    }

    .main-content-container {
        display: flex;
        flex-direction: row;
        gap: 30px;
        padding: 0 50px;
        box-sizing: border-box;
        justify-content: center;
        margin-top: 30px;
    }

    .list-card {
        background: #FFFFFF;
        border-radius: 10px;
        padding: 20px;
        display: flex;
        flex-direction: column;
        gap: 20px;
        font-family: 'Sansation';
    }

    .subjects-container {
        flex-basis: 655px;
    }

    .list-header {
        display: flex;
        flex-direction: row;
        align-items: center;
        gap: 10px;
        padding: 0 10px;
    }

    .list-header-title {
        font-weight: 700;
        font-size: 32px;
        line-height: 36px;
        color: #000000;
        display: inline-flex;
        align-items: center;
        gap: 10px;
    }

    .subjects-container .list-header-title::before {
        content: "";
        display: inline-block;
        width: 32px;
        height: 32px;
        background-image: url('../Images/Students.png');
        background-size: contain;
        background-repeat: no-repeat;
        background-position: center;
    }

    .user-container .list-header-title::before {
        content: "";
        display: inline-block;
        width: 32px;
        height: 32px;
        background-image: url('../Images/barchart.png');
        background-size: contain;
        background-repeat: no-repeat;
        background-position: center;
    }

    .user-card {
        box-sizing: border-box;
        display: flex;
        flex-direction: column;
        padding: 20px;
        gap: 20px;
        background: #FFFFFF;
        border: 1px solid #8D97AA;
        border-radius: 10px;
    }

    .user-card-title {
        font-weight: 700;
        font-size: 28px;
        line-height: 31px;
        color: #000000;
    }

    .subject-stats-row {
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: space-between;
        padding-right: 20px;
    }

    .stat-group {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        gap: 5px;
        font-size: 24px;
        font-weight: 700;
        color: #000000;
    }

    .stat-group-label {
        font-size: 16px;
        font-weight: 400;
        color: #8D97AA;
    }

    .user-container {
        flex-basis: 655px;
    }

    .user-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
    }

    .user-item {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        align-items: center;
        padding: 16px;
        background: #F5F3FE;
        border-radius: 8px;
    }

    .user-info {
        display: flex;
        flex-direction: column;
        gap: 4px;
    }

    .user-name {
        font-weight: 700;
        font-size: 16px;
        color: #000000;
    }

    .user-details {
        font-weight: 400;
        font-size: 14px;
        color: #555;
    }

    .user-time {
        font-weight: 400;
        font-size: 14px;
        color: #8D97AA;
    }

    .user-type {
        font-weight: 700;
        font-size: 18px;
    }

    .view-all-button {
        box-sizing: border-box;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 9px 20px;
        width: 100%;
        height: 40px;
        background: #F3F3F3;
        border: 1px solid #8D97AA;
        border-radius: 10px;
        font-size: 20px;
        color: #000000;
        text-decoration: none;
        cursor: pointer;
    }
    .view-all-button:hover {
        background: #e9e9e9;
    }

    .action-cards-container {
        display: flex;
        flex-direction: row;
        justify-content: center;
        gap: 30px;
        padding: 0 50px;
        margin-top: 30px;
        box-sizing: border-box;
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

    .manage-users-icon { background-image: url('/Images/Students.png'); }
    .manage-subjects-icon { background-image: url('/Images/Teachers.png'); }
    .manage-discussions-icon {
        background: #F5EAFE;
        color: #610099;
        font-size: 28px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 12px;
        width: 50px;
        height: 50px;
    }


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


        <!-- BrainBlitz Info -->
        <div class="dashboard-container">
            <div class="slogan-div">
                <div class="dashboard-title">Admin Dashboard</div>
                <div class="dashboard-subtitle">Manage your courses and track student progress</div>
            </div>

            <div class="summary-container">
                <a href="AdminUsers.aspx" class="summary-card summary-card-link">
                    <div class="summary-title">Total Users</div>
                    <asp:Label ID="lblTotalStudents" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </a>
                <a href="AdminSubjects.aspx" class="summary-card summary-card-link">
                    <div class="summary-title">Total Subjects</div>
                        <asp:Label ID="lblTotalSubjects" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </a>
                <div class="summary-card">
                    <div class="summary-title">Total Resources</div>
                    <asp:Label ID="lblTotalResources" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <div class="summary-title">Total Quizzes</div>
                    <asp:Label ID="lblTotalQuizzes" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
            </div>
            
            <!-- User Overview and Recent Activities -->
            <div class="main-content-container">
                <div class="list-card subjects-container">
                    <div class="list-header">
                        <span class="list-header-title">User Overview</span>
                    </div>
                    <div class="user-card">
                        <span class="user-card-title">Students</span>
                        <div class="subject-stats-row">
                            <div class="stat-group">
                                <span class="stat-group-label">Total Count</span>
                                <asp:Label ID="lblStudentsCount" runat="server" Text="--"></asp:Label>
                            </div>
                            <div class="stat-group">
                                <span class="stat-group-label">Active</span>
                                <asp:Label ID="lblActiveStudents" runat="server" Text="--"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="user-card">
                        <span class="user-card-title">Teachers</span>
                        <div class="subject-stats-row">
                            <div class="stat-group">
                                <span class="stat-group-label">Total Count</span>
                                <asp:Label ID="lblTeachersCount" runat="server" Text="--"></asp:Label>
                            </div>
                            <div class="stat-group">
                                <span class="stat-group-label">Active</span>
                                <asp:Label ID="lblActiveTeachers" runat="server" Text="--"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="user-card">
                        <span class="user-card-title">Admins</span>
                        <div class="subject-stats-row">
                            <div class="stat-group">
                                <span class="stat-group-label">Total Count</span>
                                <asp:Label ID="lblAdminsCount" runat="server" Text="--"></asp:Label>
                            </div>
                            <div class="stat-group">
                                <span class="stat-group-label">Active</span>
                                <asp:Label ID="lblActiveAdmins" runat="server" Text="--"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="list-card user-container">
                    <div class="list-header">
                        <span class="list-header-title">Recently Created Users</span>
                    </div>
                    <div class="user-list">
                        <asp:Repeater ID="rptRecentUsers" runat="server">
                            <ItemTemplate>
                                <div class="user-item">
                                    <div class="user-info">
                                        <span class="user-name"><%# Eval("UserName") %></span>
                                        <span class="user-details"><%# Eval("Email") %></span>
                                        <span class="user-time"><%# Eval("TimeAgo") %></span>
                                    </div>
                                    <span class="user-type"><%# Eval("RoleDisplay") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <a href="AdminUsers.aspx" class="view-all-button">View All Users</a>
                </div>
            </div>

            <!-- Action Buttons -->
            <div class="action-cards-container">
                <a href="AdminUsers.aspx" class="action-card">
                    <div class="action-card-icon manage-users-icon"></div>
                    <span class="action-card-title">Manage Users</span>
                    <span class="action-card-subtitle">Manager Users here</span>
                </a>
                <a href="AdminSubjects.aspx" class="action-card">
                    <div class="action-card-icon manage-subjects-icon"></div>
                    <span class="action-card-title">Manage Subjects</span>
                    <span class="action-card-subtitle">Manage Subjects for Student & Teachers</span>
                </a>
                <a href="AdminDiscussions.aspx" class="action-card">
                    <div class="action-card-icon manage-discussions-icon">
                         <i class="fas fa-comments"></i></div>
                    <span class="action-card-title">Discussion Moderation</span>
                    <span class="action-card-subtitle">Moderate and review flagged discussions</span>
                </a>
            </div>
        </div>
    </form>
</body>
</html>