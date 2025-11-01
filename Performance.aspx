<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Performance.aspx.cs" Inherits="BrainBlitz.Performance" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Student Performance - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="../Content/Site.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        /* --- (Copy/paste your full header styles here) --- */
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

    /* Wrapper for buttons for flex alignment */
    .header-buttons-wrapper {
        display: flex;
        align-items: center;
        gap: 48px;
    }

    /* Update Buttons */
    .header-home-btn,
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

    .header-home-btn:hover,
    .header-logout-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
    }
        /* --- End Header Styles --- */

        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .performance-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            gap: 30px; /* Space between elements */
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .page-title { font-size: 36px; font-weight: 700; color: #000; margin-bottom: 5px; }
        .page-subtitle { font-size: 20px; color: #8D97AA; font-weight: 400; }
        
        .export-button {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 20px; background: #FFFFFF; border: 1px solid #8D97AA;
            border-radius: 8px; font-weight: 700; font-size: 14px;
            color: #333; text-decoration: none; cursor: pointer;
        }
        .export-button i { font-size: 16px; }

        /* Summary Cards */
        .summary-container {
            display: grid;
            grid-template-columns: repeat(4, 1fr); /* 4 equal columns */
            gap: 25px;
        }
        .summary-card {
            display: flex; flex-direction: column;
            padding: 24px; background: #FFFFFF;
            border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);
            gap: 10px;
        }
        .summary-title {
            font-size: 16px; color: #8D97AA; font-weight: 700;
        }
        .summary-value {
            font-size: 32px; font-weight: 700; color: #000;
        }
        .summary-value.positive { color: #28A745; }
        .summary-value.neutral { color: #000; }

        /* Filter Bar */
        .filter-bar {
            display: flex; justify-content: space-between; align-items: center;
            gap: 20px; background: #FFFFFF; padding: 12px 20px;
            border-radius: 12px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .search-input-wrapper {
            display: flex; align-items: center; flex-grow: 1; gap: 15px;
        }
        .search-input-wrapper i { color: #8D97AA; font-size: 20px; }
        .search-input {
            border: none; outline: none; font-family: 'Sansation';
            font-size: 16px; color: #333; width: 100%;
        }
        .search-input::placeholder { color: #8D97AA; }
        
        .filter-pills { display: flex; gap: 10px; }
        .filter-pill {
            padding: 10px 20px; border-radius: 8px; font-weight: 700;
            font-size: 14px; background: #F3F3F3; color: #555;
            cursor: pointer; text-decoration: none; border: none;
        }
        .filter-pill.active { background: #610099; color: #FFFFFF; }

        /* Performance List Table */
        .performance-list-table {
            background: #FFFFFF; border-radius: 10px;
            padding: 20px 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .performance-table-header,
        .performance-table-row {
            display: grid;
            /* Adjusted columns, removed Trend */
            grid-template-columns: 2fr 1fr 1fr 1fr 1fr 1fr;
            gap: 20px; align-items: center; padding: 15px 0;
            border-bottom: 1px solid #EAEAEA; font-size: 16px;
        }
        .performance-table-header {
            font-weight: 700; color: #8D97AA; font-size: 14px;
            text-transform: uppercase; border-bottom: 2px solid #DDD;
        }
        .performance-table-row:last-child { border-bottom: none; }

        /* Column Classes */
        .col-student { grid-column: 1; font-weight: 700; }
        .col-subject { grid-column: 2; }
        .col-quizzes-taken { grid-column: 3; text-align: center; }
        .col-avg-score { grid-column: 4; text-align: center; font-weight: 700; }
        .col-last-activity { grid-column: 5; color: #555; }
        .col-actions { grid-column: 6; text-align: right; }
        
        /* Tags */
        .tag {
            display: inline-block; padding: 4px 10px; border-radius: 8px;
            font-size: 12px; font-weight: 700; text-align: center;
        }
        .tag-subject-math { background-color: #E6B9FF; color: #6A009C; }
        .tag-subject-physics { background-color: #B9E6FF; color: #006A9C; }
        .tag-subject-default { background-color: #E0E0E0; color: #555; }
        
        /* Scores */
        .score-good { color: #28A745; }
        .score-medium { color: #FFA500; }
        .score-bad { color: #DC3545; }
        
        .action-link {
            font-weight: 700; color: #610099; text-decoration: none;
        }
        .action-link:hover { text-decoration: underline; }

        .error-message { color: #FF0000; font-size: 16px; margin-bottom: 15px; }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <%-- 1. Reusable Header --%>
        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnHome" runat="server" CssClass="header-home-btn" OnClick="btnHome_Click">Home</asp:LinkButton>
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- 2. Main Content Area --%>
        <div class="performance-container">

            <div class="page-header">
                <div>
                    <h1 class="page-title">Student Performance</h1>
                    <p class="page-subtitle">Track and analyze student progress</p>
                </div>
                <asp:Button ID="btnExportReport" runat="server" Text="Export Report" CssClass="export-button" OnClick="btnExportReport_Click" />
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            
            <%-- 3. Summary Cards --%>
            <div class="summary-container">
                <div class="summary-card">
                    <span class="summary-title">Total Students</span>
                    <asp:Label ID="lblTotalStudents" runat="server" CssClass="summary-value neutral" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <span class="summary-title">Class Average</span>
                    <asp:Label ID="lblClassAverage" runat="server" CssClass="summary-value positive" Text="--%"></asp:Label>
                </div>
                <div class="summary-card">
                    <span class="summary-title">Active This Week</span>
                    <asp:Label ID="lblActiveThisWeek" runat="server" CssClass="summary-value neutral" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <span class="summary-title">Total Attempts</span>
                    <asp:Label ID="lblTotalAttempts" runat="server" CssClass="summary-value neutral" Text="--"></asp:Label>
                </div>
            </div>

            <%-- 4. Filter Bar --%>
            <div class="filter-bar">
                <div class="search-input-wrapper">
                    <i class="fas fa-search"></i>
                    <asp:TextBox ID="txtSearchStudents" runat="server" CssClass="search-input" placeholder="Search students..." AutoPostBack="true" OnTextChanged="txtSearchStudents_TextChanged"></asp:TextBox>
                </div>
            </div>

            <%-- 5. Performance List Table --%>
            <div class="performance-list-table">
                <asp:Repeater ID="rptPerformance" runat="server">
                    <HeaderTemplate>
                        <div class="performance-table-header">
                            <span class="col-student">Student Name</span>
                            <span class="col-subject">Subject</span>
                            <span class="col-quizzes-taken">Quizzes Taken</span>
                            <span class="col-avg-score">Avg Score</span>
                            <span class="col-last-activity">Last Activity</span>
                            <span class="col-actions">Actions</span>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="performance-table-row">
                            <div class="col-student"><%# Eval("StudentName") %></div>
                            <div class="col-subject">
                                <span class='<%# GetSubjectTagClass(Eval("LastSubjectName").ToString()) %>'>
                                    <%# Eval("LastSubjectName") %>
                                </span>
                            </div>
                            <div class="col-quizzes-taken"><%# Eval("QuizzesTaken") %></div>
                            <div class="col-avg-score">
                                <span class='<%# GetScoreClass(Eval("AvgPercentage")) %>'>
                                    <%# Eval("AvgPercentage") %>%
                                </span>
                            </div>
                            <div class="col-last-activity"><%# Eval("LastActivityTimeAgo") %></div>
                            <div class="col-actions">
                                <a href='StudentDetails.aspx?studentID=<%# Eval("UserID") %>' class="action-link">
                                    View Details
                                </a>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Panel ID="pnlNoStudents" runat="server" Visible="false" Style="text-align: center; padding: 40px; color: #888; border-top: 1px solid #EAEAEA;">
                            No student performance data found.
                        </asp:Panel>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

        </div>
    </form>
</body>
</html>