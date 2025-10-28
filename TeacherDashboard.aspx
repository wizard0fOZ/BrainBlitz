<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherDashboard.aspx.cs" Inherits="BrainBlitz.TeacherDashboard" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Teacher Dashboard - BrainBlitz</title>

    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">

<style>
        /* Dashboard Container - For content *below* the header */
        .dashboard-container {
            position: relative;
            width: 100%;
            min-height: calc(1024px - 75px); /* Adjust height since header is outside */
            background: #F3F3F3;
            margin: 0 auto; /* Centers the container */
            padding-bottom: 40px;
            max-width: 1440px; /* Max width of the content area */
        }

        /* ============================
           FULL WIDTH HEADER STYLES
        ============================ */
        .header {
            position: relative; /* Normal flow, not absolute */
            width: 100%;
            height: 75px;
            background: #FFFFFF;
            box-sizing: border-box;
             /* Ensures it sits above the light gray background */
             z-index: 10;
        }

        /* Inner container for alignment within the header */
        .header-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            max-width: 1440px; /* Matches dashboard container width */
            height: 100%;
            margin: 0 auto;    /* Centers this inner container */
            padding: 0 50px;   /* Consistent side padding */
            box-sizing: border-box;
        }

        /* Logo styling */
        .brainblitz {
            position: relative; /* Not absolute */
            left: auto;
            top: auto;
            width: 312px;
            height: 60px;
            background: url('../Images/BrainBlitz.png');
            background-size: contain;
            background-repeat: no-repeat;
            margin: 0; /* Remove default margins */
            display: block; /* Ensure it behaves like a block element */
        }
         /* Optional: Remove global hover effect if desired */
        .brainblitz:hover {
            transform: none;
            box-shadow: none;
        }

        /* NEW: Wrapper for buttons for flex alignment */
        .header-buttons-wrapper {
            display: flex;
            align-items: center;
            gap: 48px; /* Spacing between buttons */
        }

        /* Update Buttons - Remove absolute positioning */
        .header-home-btn,
        .header-logout-btn {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 9px 38px;
            position: relative; /* REMOVED absolute */
            height: 40px;
            top: auto;          /* REMOVED top */
            right: auto;         /* REMOVED right */
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
            color: #fff;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            text-align: center;
            text-decoration: none;
             width: 132px; /* Set width for Home */
        }
        .header-logout-btn {
             width: 107px; /* Set width for Logout */
        }


        .header-home-btn:hover,
        .header-logout-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
        }
        /* ============================
           END HEADER STYLES
        ============================ */

        /* Slogan Section */
        .slogan-div {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            align-items: flex-start;
            padding: 5px 50px 20px; /* Consistent 50px side padding */
            box-sizing: border-box;
            margin-top: 15px; /* Spacing below header */
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

        /* ===== Summary Section ===== */
        .summary-container {
            display: flex;
            flex-direction: row;
            justify-content: space-between; /* Spreads cards to edges */
            align-items: center;
            padding: 0 50px; /* Consistent 50px side padding */
            box-sizing: border-box;
            margin-top: 30px; /* Position below slogan */
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

        /* =================================
         MY SUBJECTS & RECENT ACTIVITY
        =================================
        */

        /* Main 2-Column Container */
        .main-content-container {
            display: flex;
            flex-direction: row;
            gap: 30px;
            padding: 0 50px; /* Consistent 50px side padding */
            box-sizing: border-box;
            justify-content: center;
            margin-top: 30px; /* Position below summary */
        }

        /* Base Card for both columns */
        .list-card {
            background: #FFFFFF;
            border-radius: 10px;
            padding: 20px;
            display: flex;
            flex-direction: column;
            gap: 20px;
            font-family: 'Sansation';
        }

        /* Column 1: My Subjects */
        .subjects-container {
            flex-basis: 655px; /* Corrected width for alignment */
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
             /* --- ADDED FOR ICON --- */
            display: inline-flex;
            align-items: center;
            gap: 10px;
            /* --- END ICON --- */
        }

        /* --- ADDED ICON STYLES --- */
        /* Icon for "My Subjects" */
        .subjects-container .list-header-title::before {
             content: "";
             display: inline-block;
             width: 32px;
             height: 32px;
             background-image: url('/Images/Teachers.png'); /* <<<--- PATH TO YOUR ICON */
             background-size: contain;
             background-repeat: no-repeat;
             background-position: center;
        }

        /* Icon for "Recent Activity" */
        .activity-container .list-header-title::before {
             content: "";
             display: inline-block;
             width: 32px;
             height: 32px;
             background-image: url('/Images/barchart.png'); /* <<<--- PATH TO YOUR ICON */
             background-size: contain;
             background-repeat: no-repeat;
             background-position: center;
        }
        /* --- END ICON STYLES --- */


        .subject-card {
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            padding: 20px;
            gap: 20px;
            background: #FFFFFF;
            border: 1px solid #8D97AA;
            border-radius: 10px;
        }

        .subject-card-title {
            font-weight: 700;
            font-size: 28px;
            line-height: 31px;
            color: #000000;
        }

        .subject-stats-row {
            display: flex;
            flex-direction: row;
            align-items: center;
            justify-content: space-between; /* Better distribution */
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


        /* Column 2: Recent Activity */
        .activity-container {
            flex-basis: 655px; /* Corrected width for alignment */
        }

        .activity-list {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }

        .activity-item {
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            align-items: center;
            padding: 16px;
            background: #F5F3FE;
            border-radius: 8px;
        }

        .activity-info {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .activity-name {
            font-weight: 700;
            font-size: 16px;
            color: #000000;
        }

        .activity-details {
            font-weight: 400;
            font-size: 14px;
            color: #555;
        }

        .activity-time {
            font-weight: 400;
            font-size: 14px;
            color: #8D97AA;
        }

        .activity-score {
            font-weight: 700;
            font-size: 18px;
        }

        .activity-score-good { color: #29CF48; }
        .activity-score-bad { color: #FF0000; }

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
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <div class="header-home-btn"><span>Home</span></div>
                    <div class="header-logout-btn"><span>Log out</span></div>
                </div>
            </div>
        </div>

        <div class="dashboard-container">

            <div class="slogan-div">
                <div class="dashboard-title">Teacher Dashboard</div>
                <div class="dashboard-subtitle">Manage your courses and track student progress</div>
            </div>

            <div class="summary-container">
                <div class="summary-card">
                    <div class="summary-title">Total Students</div>
                    <asp:Label ID="lblTotalStudents" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <div class="summary-title">Active Quizzes</div>
                    <asp:Label ID="lblActiveQuizzes" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <div class="summary-title">Resources</div>
                    <asp:Label ID="lblResources" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <div class="summary-title">Average Score</div>
                    <asp:Label ID="lblAverageScore" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
            </div>
            
            <div class="main-content-container">
                
                <div class="list-card subjects-container">
                    <div class="list-header">
                        <span class="list-header-title">My Subjects</span>
                    </div>
                    <asp:Repeater ID="rptSubjects" runat="server">
                        <ItemTemplate>
                            <div class="subject-card">
                                <span class="subject-card-title"><%# Eval("SubjectName") %></span>
                                <div class="subject-stats-row">
                                    <div class="stat-group">
                                        <span class="stat-group-label">Quizzes</span>
                                        <span><%# Eval("QuizCount") %></span>
                                    </div>
                                    <div class="stat-group">
                                        <span class="stat-group-label">Resources</span>
                                        <span><%# Eval("ResourceCount") %></span>
                                    </div>
                                    <div class="stat-group">
                                        <span class="stat-group-label">Students</span>
                                        <span><%# Eval("StudentCount") %></span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="list-card activity-container">
                    <div class="list-header">
                        <span class="list-header-title">Recent Activity</span>
                    </div>
                    <div class="activity-list">
                        <asp:Repeater ID="rptRecentActivity" runat="server">
                            <ItemTemplate>
                                <div class="activity-item">
                                    <div class="activity-info">
                                        <span class="activity-name"><%# Eval("StudentName") %></span>
                                        <span class="activity-details"><%# Eval("QuizTitle") %></span>
                                        <span class="activity-time"><%# Eval("TimeAgo") %></span>
                                    </div>
                                    <span class='<%# Eval("ScoreClass") %>'><%# Eval("ScoreDisplay") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <a href="Performance.aspx" class="view-all-button">View All Performance</a>
                </div>
                
            </div>
        </div> </form>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Home button click
            // Check if element exists before adding listener
            const homeBtn = document.querySelector('.header-home-btn');
            if (homeBtn) {
                homeBtn.addEventListener('click', function () {
                    window.location.href = 'Default.aspx';
                });
            }

            // Logout button click
            // Check if element exists before adding listener
            const logoutBtn = document.querySelector('.header-logout-btn');
            if (logoutBtn) {
                logoutBtn.addEventListener('click', function () {
                    window.location.href = 'Logout.aspx';
                });
            }
        });
    </script>
</body>
</html>