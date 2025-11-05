<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentDetails.aspx.cs" Inherits="BrainBlitz.Teacher.StudentDetails" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Student Details - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="../Content/Site.css"> <%-- Adjust path if needed --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
    /* Dashboard Container - For content *below* the header */
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

        /* Page Specific Styles */
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .details-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            gap: 30px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .page-title { font-size: 36px; font-weight: 700; color: #000; margin-bottom: 5px; }
        .page-subtitle { font-size: 20px; color: #8D97AA; font-weight: 400; }
        
        .back-link {
            display: inline-flex; align-items: center; gap: 8px;
            padding: 10px 20px; background: #FFFFFF; border: 1px solid #8D97AA;
            border-radius: 8px; font-weight: 700; font-size: 14px;
            color: #333; text-decoration: none; cursor: pointer;
        }
        .back-link:hover { background-color: #f7f7f7; }
        .back-link i { font-size: 16px; }

        /* Summary Cards (can reuse from performance page) */
        .summary-container {
            display: grid;
            grid-template-columns: repeat(3, 1fr); /* 3 columns for student */
            gap: 25px;
        }
        .summary-card {
            display: flex; flex-direction: column; padding: 24px; 
            background: #FFFFFF; border-radius: 10px; 
            box-shadow: 0 2px 4px rgba(0,0,0,0.05); gap: 10px;
        }
        .summary-title { font-size: 16px; color: #8D97AA; font-weight: 700; }
        .summary-value { font-size: 32px; font-weight: 700; color: #000; }
        .summary-value.positive { color: #28A745; }
        .summary-value.neutral { color: #000; }

        /* List Table (can reuse from performance page) */
        .list-table {
            background: #FFFFFF; border-radius: 10px;
            padding: 20px 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }
        .table-header,
        .table-row {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr 1fr 1fr; /* 5 columns */
            gap: 20px; align-items: center; padding: 15px 0;
            border-bottom: 1px solid #EAEAEA; font-size: 16px;
        }
        .table-header {
            font-weight: 700; color: #8D97AA; font-size: 14px;
            text-transform: uppercase; border-bottom: 2px solid #DDD;
        }
        .table-row:last-child { border-bottom: none; }

        /* Column Classes */
        .col-quiz-title { grid-column: 1; font-weight: 700; }
        .col-subject { grid-column: 2; }
        .col-date { grid-column: 3; color: #555; }
        .col-score { grid-column: 4; text-align: center; font-weight: 700; }
        .col-percentage { grid-column: 5; text-align: center; font-weight: 700; }
        
        .score-good { color: #28A745; }
        .score-medium { color: #FFA500; }
        .score-bad { color: #DC3545; }
        
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
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- 2. Main Content Area --%>
        <div class="details-container">

            <div class="page-header">
                <div>
                    <h1 class="page-title">
                        <asp:Label ID="lblStudentName" runat="server" Text="Student Performance"></asp:Label>
                    </h1>
                    <p class="page-subtitle">Detailed report of all quiz attempts</p>
                </div>
                <asp:Button ID="btnBack" runat="server" Text="Back to Performance" 
                    CssClass="back-link" OnClick="btnBack_Click" CausesValidation="false" />
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            
            <%-- 3. Summary Cards --%>
            <div class="summary-container">
                <div class="summary-card">
                    <span class="summary-title">Overall Average</span>
                    <asp:Label ID="lblOverallAverage" runat="server" CssClass="summary-value positive" Text="--%"></asp:Label>
                </div>
                <div class="summary-card">
                    <span class="summary-title">Total Attempts</span>
                    <asp:Label ID="lblTotalAttempts" runat="server" CssClass="summary-value neutral" Text="--"></asp:Label>
                </div>
                <div class="summary-card">
                    <span class="summary-title">Quizzes Taken</span>
                    <asp:Label ID="lblQuizzesTaken" runat="server" CssClass="summary-value neutral" Text="--"></asp:Label>
                </div>
            </div>

            <%-- 4. Detailed List Table --%>
            <div class="list-table">
                <asp:Repeater ID="rptStudentAttempts" runat="server">
                    <HeaderTemplate>
                        <div class="table-header">
                            <span class="col-quiz-title">Quiz Title</span>
                            <span class="col-subject">Subject</span>
                            <span class="col-date">Date Taken</span>
                            <span class="col-score">Score</span>
                            <span class="col-percentage">Percentage</span>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="table-row">
                            <div class="col-quiz-title"><%# Eval("QuizTitle") %></div>
                            <div class="col-subject">
                                <span class='<%# GetSubjectTagClass(Eval("SubjectName").ToString()) %>'>
                                    <%# Eval("SubjectName") %>
                                </span>
                            </div>
                            <div class="col-date"><%# Eval("FinishedAt", "{0:dd MMM yyyy h:mm tt}") %></div>
                            <div class="col-score"><%# Eval("PointsEarned") %> / <%# Eval("MaxPoints") %></div>
                            <div class="col-percentage">
                                <span class='<%# GetScoreClass(Eval("Percentage")) %>'>
                                    <%# Eval("Percentage") %>%
                                </span>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Panel ID="pnlNoAttempts" runat="server" Visible="false" Style="text-align: center; padding: 40px; color: #888;">
                            This student has not completed any quizzes yet.
                        </asp:Panel>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

        </div>
    </form>
</body>
</html>