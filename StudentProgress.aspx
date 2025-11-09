<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentProgress.aspx.cs" Inherits="BrainBlitz.StudentProgress" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Learning Progress - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="Content/Site.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
    body {
        background-color: #F3F3F3;
        font-family: 'Sansation', sans-serif;
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
        height: 40px;
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

    .progress-container {
        max-width: 1440px;
        margin: 0 auto;
        padding: 20px 50px 40px 50px;
        box-sizing: border-box;
    }

    .page-back {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 14px;
        color: #610099;
        text-decoration: none;
        margin-bottom: 10px;
    }

    .page-back i {
        font-size: 14px;
    }

    .page-title {
        font-size: 36px;
        font-weight: 700;
        margin: 0;
        color: #000;
    }

    .page-subtitle {
        font-size: 18px;
        font-weight: 400;
        color: #8D97AA;
        margin-top: 4px;
        margin-bottom: 24px;
    }

    .progress-grid {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 24px;
    }

    .progress-card {
        background: #FFFFFF;
        border-radius: 10px;
        padding: 20px 22px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.03);
        display: flex;
        flex-direction: column;
        gap: 16px;
        min-height: 260px;
    }

    .progress-card-header {
        display: flex;
        align-items: center;
        gap: 10px;
        font-size: 20px;
        font-weight: 700;
    }

    .progress-card-header i {
        color: #610099;
    }

    .empty-message {
        font-size: 14px;
        color: #8D97AA;
        text-align: center;
        margin-top: 30px;
    }

    .message {
        display: block;
        margin-top: 10px;
        color: #555;
        font-size: 14px;
    }

    .message.error {
        color: #B00020;
    }

    .message.success {
        color: #008000;
    }

    .quiz-list-item {
        width: 100%;
        text-align: left;
        border-radius: 10px;
        border: 2px solid #E0E0E0;
        padding: 12px 14px;
        margin-bottom: 10px;
        cursor: pointer;
        background: #FFFFFF;
        display: block;
        color: #000;
        text-decoration: none;
    }

    .quiz-list-item.selected {
        border-color: #610099;
        background: #F5EAFE;
    }

    .quiz-list-item:link,
    .quiz-list-item:visited,
    .quiz-list-item:hover,
    .quiz-list-item:active {
        color: inherit;
        text-decoration: none;
    }

    .quiz-title {
        font-size: 15px;
        font-weight: 600;
        margin-bottom: 4px;
    }

    .quiz-subject {
        font-size: 12px;
        color: #8D97AA;
    }

    .quiz-attempt-count {
        font-size: 12px;
        padding: 3px 8px;
        border-radius: 999px;
        background: #F3F3F3;
        color: #555;
        float: right;
    }

    .attempt-item {
        width: 100%;
        text-align: left;
        border-radius: 10px;
        border: 2px solid #E0E0E0;
        padding: 12px 14px;
        margin-bottom: 10px;
        cursor: pointer;
        background: #FFFFFF;
        display: block;
        color: #000;
        text-decoration: none;
    }

    .attempt-item.selected {
        border-color: #610099;
        background: #F5EAFE;
    }

    .attempt-item:link,
    .attempt-item:visited,
    .attempt-item:hover,
    .attempt-item:active {
        color: inherit;
        text-decoration: none;
    }

    .attempt-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
    }

    .attempt-left {
        display: flex;
        flex-direction: column;
    }

    .attempt-right {
        text-align: right;
        min-width: 70px;
    }

    .attempt-date {
        font-size: 12px;
        color: #8D97AA;
        margin-bottom: 2px;
    }

    .attempt-time {
        font-size: 12px;
        color: #8D97AA;
    }

    .attempt-score {
        font-size: 20px;
        font-weight: 700;
    }

    .attempt-score.good {
        color: #29CF48;
    }

    .attempt-score.bad {
        color: #FF9800;
    }

    .attempt-score-small {
        font-size: 12px;
        color: #8D97AA;
    }

    .attempt-summary {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 12px;
        background: #F5F5FF;
        border-radius: 10px;
        padding: 10px 12px;
        font-size: 13px;
    }

    .attempt-summary-label {
        font-size: 11px;
        color: #8D97AA;
        margin-bottom: 2px;
    }

    .attempt-summary-value {
        font-size: 16px;
        font-weight: 600;
    }

    .question-list {
        margin-top: 12px;
        max-height: 480px;
        overflow-y: auto;
        padding-right: 4px;
    }

    .question-block {
        border-radius: 10px;
        border: 2px solid #E0E0E0;
        padding: 10px 12px;
        margin-bottom: 10px;
        font-size: 13px;
    }

    .question-block.correct {
        border-color: rgba(41, 207, 72, 0.4);
        background: rgba(41, 207, 72, 0.05);
    }

    .question-block.wrong {
        border-color: rgba(255, 0, 0, 0.3);
        background: rgba(255, 0, 0, 0.05);
    }

    .question-text {
        font-weight: 600;
        margin-bottom: 6px;
    }

    .option-row {
        padding: 4px 6px;
        border-radius: 6px;
        margin-bottom: 4px;
    }

    .option-row.correct {
        background: rgba(41, 207, 72, 0.15);
        border: 1px solid rgba(41, 207, 72, 0.5);
        font-weight: 600;
    }

    .option-row.student {
        background: rgba(255, 0, 0, 0.12);
        border: 1px solid rgba(255, 0, 0, 0.5);
    }

    .option-label {
        font-weight: 600;
        margin-right: 4px;
    }

    .option-tag {
        font-size: 11px;
        margin-left: 6px;
    }

    .option-tag.correct {
        color: #29CF48;
    }

    .option-tag.student {
        color: #FF0000;
    }

    @media (max-width: 1024px) {
        .progress-container {
            padding: 20px 20px 40px 20px;
        }

        .progress-grid {
            grid-template-columns: 1fr;
        }
    }
</style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- Header reused from other student pages --%>
        <div class="header">
            <div class="header-content">
                <a href="UserDashboard.aspx.aspx" class="brainblitz">BrainBlitz</a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout"
                        runat="server"
                        CssClass="header-logout-btn"
                        OnClick="btnLogout_Click">
                        Log out
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="progress-container">
            <a href="StudentDashboard.aspx" class="page-back">
                <i class="fas fa-arrow-left"></i> Back to Dashboard
            </a>

            <h1 class="page-title">Learning Progress</h1>
            <p class="page-subtitle">Review your quiz attempts and track your improvement</p>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>

            <div class="progress-grid">
                <%-- LEFT: Quizzes --%>
                <div class="progress-card">
                    <div class="progress-card-header">
                        <i class="fas fa-trophy"></i>
                        <span>Your Quizzes</span>
                    </div>

                    <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false">
                        <div class="empty-message">
                            No quiz attempts yet. Start taking quizzes to track your progress!
                        </div>
                    </asp:Panel>

                    <asp:Repeater ID="rptQuizzes" runat="server" OnItemCommand="rptQuizzes_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkQuiz" runat="server"
                                CommandName="SelectQuiz"
                                CommandArgument='<%# Eval("QuizID") %>'
                                CssClass='<%# Eval("ItemCss") %>'>
                                <div class="quiz-title">
                                    <%# Eval("Title") %>
                                    <span class="quiz-attempt-count">
                                        <%# Eval("AttemptCount") %> attempt<%# (Convert.ToInt32(Eval("AttemptCount")) == 1 ? "" : "s") %>
                                    </span>
                                </div>
                                <div class="quiz-subject"><%# Eval("SubjectName") %></div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <%-- MIDDLE: Attempts --%>
                <div class="progress-card">
                    <div class="progress-card-header">
                        <i class="fas fa-clock"></i>
                        <span>Attempts</span>
                    </div>

                    <asp:Panel ID="pnlNoQuizSelected" runat="server">
                        <div class="empty-message">
                            Select a quiz to view your attempts
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlAttempts" runat="server" Visible="false">
                        <asp:Repeater ID="rptAttempts" runat="server" OnItemCommand="rptAttempts_ItemCommand">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkAttempt" runat="server"
                                    CommandName="SelectAttempt"
                                    CommandArgument='<%# Eval("AttemptID") %>'
                                    CssClass='<%# Eval("ItemCss") %>'>

                                    <div class="attempt-row">
                                        <div class="attempt-left">
                                            <div class="attempt-date"><%# Eval("AttemptDate") %></div>
                                            <div class="attempt-time"><%# Eval("AttemptTime") %></div>
                                        </div>

                                        <div class="attempt-right">
                                            <span class='<%# Eval("ScoreClass") %>'>
                                                <%# Eval("ScorePercent") %>
                                            </span>
                                            <div class="attempt-score-small">
                                                <%# Eval("ScoreDisplay") %>
                                            </div>
                                        </div>
                                    </div>

                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                </div>

                <%-- RIGHT: Details --%>
                <div class="progress-card">
                    <div class="progress-card-header">
                        <i class="fas fa-check-circle" style="color:#29CF48;"></i>
                        <span>Details</span>
                    </div>

                    <asp:Panel ID="pnlNoAttemptSelected" runat="server">
                        <div class="empty-message">
                            Select an attempt to view details
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlAttemptDetails" runat="server" Visible="false">
                        <div class="attempt-summary">
                            <div>
                                <div class="attempt-summary-label">Score</div>
                                <asp:Label ID="lblDetailScore" runat="server" CssClass="attempt-summary-value"></asp:Label>
                            </div>
                            <div>
                                <div class="attempt-summary-label">Accuracy</div>
                                <asp:Label ID="lblDetailAccuracy" runat="server" CssClass="attempt-summary-value"></asp:Label>
                            </div>
                        </div>

                        <div class="question-list">
                            <asp:Literal ID="litQuestionDetails" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>

    </form>
</body>
</html>