<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs" Inherits="BrainBlitz.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Student Dashboard - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="Content/Site.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif; }

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

        /* header button wrapper same as teacher */
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

        /* welcome section */
        .slogan-div {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            align-items: flex-start;
            padding: 5px 50px 20px;
            box-sizing: border-box;
            margin-top: 15px;
        }

        .student-name-title {
            font-weight: 400;
            font-size: 32px;
            color: #555;
        }

        .dashboard-title {
            font-weight: 700;
            font-size: 36px;
            line-height: 40px;
            color: #000;
        }

        .dashboard-subtitle {
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #8D97AA;
        }

        /* stats row */
        .summary-container {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 20px;
            padding: 0 50px;
            box-sizing: border-box;
            margin-top: 25px;
        }

        .summary-card {
            display: flex;
            flex-direction: column;
            padding: 20px 24px;
            gap: 10px;
            background: #FFFFFF;
            border-radius: 10px;
            box-sizing: border-box;
            box-shadow: 0 2px 4px rgba(0,0,0,0.03);
        }

        .summary-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .summary-title {
            font-weight: 700;
            font-size: 18px;
            color: #8D97AA;
        }

        .summary-icon {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            background: #F5EAFE;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #610099;
        }

        .summary-value {
            font-weight: 700;
            font-size: 28px;
            color: #000000;
        }

        .summary-trend {
            font-size: 12px;
            color: #28A745;
        }

        .summary-trend.bad {
            color: #C62828;
        }

        /* main 2-column area */
        .main-content-container {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 30px;
            padding: 30px 50px 0 50px;
            box-sizing: border-box;
        }

        .list-card {
            background: #FFFFFF;
            border-radius: 10px;
            padding: 20px 22px;
            display: flex;
            flex-direction: column;
            gap: 18px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.03);
        }

        .list-header-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .list-header-title {
            font-weight: 700;
            font-size: 24px;
            color: #000;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .list-header-title i {
            color: #610099;
            font-size: 20px;
        }

        .list-header-link {
            font-size: 14px;
            color: #610099;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .quiz-item {
            padding: 14px 14px;
            border-radius: 10px;
            border: 1px solid #E0E0E0;
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .quiz-top-row {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 10px;
        }

        .quiz-title {
            font-weight: 600;
            font-size: 16px;
        }

        .quiz-subject {
            font-size: 12px;
            color: #8D97AA;
        }

        .quiz-score {
            font-weight: 700;
            font-size: 20px;
            text-align: right;
        }

        .quiz-score-good { color: #29CF48; }
        .quiz-score-bad { color: #FF9800; }

        .quiz-time {
            font-size: 11px;
            color: #8D97AA;
        }

        .btn-primary-full {
            margin-top: 10px;
            width: 100%;
            padding: 10px 0;
            text-align: center;
            border-radius: 10px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
            font-weight: 700;
            font-size: 14px;
            text-decoration: none;
            display: inline-block;
        }

        .resource-item {
            padding: 14px 14px;
            border-radius: 10px;
            border: 1px solid #E0E0E0;
            display: flex;
            gap: 10px;
        }

        .resource-icon {
            width: 36px;
            height: 36px;
            border-radius: 10px;
            background: #F5EAFE;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #610099;
            flex-shrink: 0;
        }

        .resource-title {
            font-weight: 600;
            font-size: 15px;
        }

        .resource-meta {
            font-size: 12px;
            color: #8D97AA;
            margin-top: 4px;
        }

        .badge-type {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 999px;
            background: #E6D4FF;
            font-size: 11px;
            margin-right: 6px;
        }

        /* quick actions */
        .action-cards-container {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 30px;
            padding: 30px 50px 0 50px;
            box-sizing: border-box;
        }

        .action-card {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            padding: 20px 22px;
            background: #FFFFFF;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            text-decoration: none;
            color: inherit;
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
            border-radius: 10px;
            background-size: contain;
            background-repeat: no-repeat;
            background-position: center;
            margin-bottom: 8px;
        }

        .action-quiz-icon { background-image: url('/Images/pluspink.png'); }
        .action-resource-icon { background-image: url('/Images/bookicon.png'); }
        .action-progress-icon { background-image: url('/Images/chartgreen.png'); }

        .action-card-title {
            font-weight: 700;
            font-size: 20px;
        }

        .action-card-subtitle {
            font-weight: 400;
            font-size: 14px;
            color: #8D97AA;
            line-height: 1.4;
        }

        .message {
            display: block;
            margin: 0 50px;
            margin-top: 10px;
        }
        .message.error { color: #B00020; }
        .message.success { color: #008000; }

        @media (max-width: 1024px) {
            .summary-container { grid-template-columns: repeat(2, minmax(0, 1fr)); }
            .main-content-container { grid-template-columns: 1fr; }
            .action-cards-container { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- header same structure as teacher --%>
        <div class="header">
            <div class="header-content">
                <a href="StudentDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:Button ID="btnLogout" runat="server" Text="Log out" CssClass="header-logout-btn" OnClick="btnLogout_Click" />
                </div>
            </div>
        </div>

        <div class="dashboard-container">
            <div class="slogan-div">
                <asp:Label ID="lblStudentName" runat="server" CssClass="student-name-title"></asp:Label>
                <div class="dashboard-title">Student Dashboard</div>
                <div class="dashboard-subtitle">Ready to continue your learning journey?</div>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>

            <%-- stats cards --%>
            <div class="summary-container">
                <div class="summary-card">
                    <div class="summary-card-header">
                        <span class="summary-title">Quizzes Completed</span>
                        <div class="summary-icon"><i class="fas fa-trophy"></i></div>
                    </div>
                    <asp:Label ID="lblQuizzesCompleted" runat="server" CssClass="summary-value" Text="0"></asp:Label>
                    <asp:Label ID="lblQuizzesTrend" runat="server" CssClass="summary-trend"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-card-header">
                        <span class="summary-title">Average Score</span>
                        <div class="summary-icon"><i class="fas fa-bullseye"></i></div>
                    </div>
                    <asp:Label ID="lblAverageScore" runat="server" CssClass="summary-value" Text="0%"></asp:Label>
                    <asp:Label ID="lblAverageTrend" runat="server" CssClass="summary-trend"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-card-header">
                        <span class="summary-title">Learning Streak</span>
                        <div class="summary-icon"><i class="fas fa-chart-line"></i></div>
                    </div>
                    <asp:Label ID="lblStreak" runat="server" CssClass="summary-value" Text="0 days"></asp:Label>
                    <asp:Label ID="lblStreakInfo" runat="server" CssClass="summary-trend"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-card-header">
                        <span class="summary-title">Hours Studied</span>
                        <div class="summary-icon"><i class="fas fa-clock"></i></div>
                    </div>
                    <asp:Label ID="lblHoursStudied" runat="server" CssClass="summary-value" Text="0"></asp:Label>
                    <asp:Label ID="lblHoursInfo" runat="server" CssClass="summary-trend"></asp:Label>
                </div>
            </div>

            <%-- main two columns --%>
            <div class="main-content-container">
                <%-- recent quizzes --%>
                <div class="list-card">
                    <div class="list-header-row">
                        <div class="list-header-title">
                            <i class="fas fa-trophy"></i>
                            <span>Recent Quiz Attempts</span>
                        </div>
                        <a href="StudentQuiz.aspx" class="list-header-link">
                            View All <i class="fas fa-arrow-right"></i>
                        </a>
                    </div>

                    <asp:Repeater ID="rptRecentQuizzes" runat="server">
                        <ItemTemplate>
                            <div class="quiz-item">
                                <div class="quiz-top-row">
                                    <div>
                                        <div class="quiz-title"><%# Eval("QuizTitle") %></div>
                                        <div class="quiz-subject"><%# Eval("SubjectName") %></div>
                                    </div>
                                    <div>
                                        <div class='<%# Eval("ScoreClass") %>'>
                                            <span class="quiz-score"><%# Eval("ScoreDisplay") %></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="quiz-time"><%# Eval("TimeAgo") %></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <a href="StudentQuiz.aspx" class="btn-primary-full">Take a New Quiz</a>
                </div>

                <%-- bookmarked resources --%>
                <div class="list-card">
                    <div class="list-header-row">
                        <div class="list-header-title">
                            <i class="fas fa-book-open"></i>
                            <span>Bookmarked Resources</span>
                        </div>
                        <a href="StudentResources.aspx" class="list-header-link">
                            Browse All <i class="fas fa-arrow-right"></i>
                        </a>
                    </div>

                    <asp:Repeater ID="rptRecommendedResources" runat="server">
                        <ItemTemplate>
                            <div class="resource-item">
                                <div class="resource-icon">
                                    <i class="fas fa-book"></i>
                                </div>
                                <div>
                                    <div class="resource-title"><%# Eval("Title") %></div>
                                    <div class="resource-meta">
                                        <span class="badge-type"><%# Eval("Type") %></span>
                                        <span><%# Eval("SubjectName") %></span>
                                        <span> · <%# Eval("BookmarkedAgo") %></span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <a href="StudentResources.aspx" class="btn-primary-full"
                       style="background:#FFFFFF;border:1px solid #8D97AA;color:#000;">
                        Explore Resources
                    </a>
                </div>
            </div>


            <%-- quick actions --%>
            <div class="action-cards-container">
                <a href="StudentQuiz.aspx" class="action-card">
                    <div class="action-card-icon action-quiz-icon"></div>
                    <span class="action-card-title">Take a Quiz</span>
                    <span class="action-card-subtitle">Test your knowledge and improve your skills</span>
                </a>
                <a href="StudentResources.aspx" class="action-card">
                    <div class="action-card-icon action-resource-icon"></div>
                    <span class="action-card-title">Browse Resources</span>
                    <span class="action-card-subtitle">Access learning materials and study guides</span>
                </a>
                <a href="StudentProgress.aspx" class="action-card">
                    <div class="action-card-icon action-progress-icon"></div>
                    <span class="action-card-title">View Progress</span>
                    <span class="action-card-subtitle">Track your learning achievements</span>
                </a>
            </div>

        </div>
    </form>
</body>
</html>