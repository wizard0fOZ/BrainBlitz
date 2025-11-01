<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TakeQuiz.aspx.cs" Inherits="BrainBlitz.Student.TakeQuiz" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Take Quiz - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="../Content/Site.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        .header {
            position: relative; width: 100%; height: 75px; background: #FFFFFF;
            box-sizing: border-box; z-index: 10;
        }
        .header-content {
            display: flex; justify-content: space-between; align-items: center;
            max-width: 1440px; height: 100%; margin: 0 auto; padding: 0 50px;
            box-sizing: border-box;
        }
        .brainblitz {
             position: relative; left: auto; top: auto; width: 312px; height: 60px;
             background: url('../Images/BrainBlitz.png') no-repeat center center / contain;
             margin: 0; display: block; text-decoration: none;
        }
        .header-buttons-wrapper { display: flex; align-items: center; gap: 48px; }
        .header-home-btn, .header-logout-btn {
            display: flex; justify-content: center; align-items: center; padding: 9px 38px;
            position: relative; height: 40px; background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px; cursor: pointer; color: #fff; font-family: 'Sansation';
            font-weight: 700; font-size: 20px; text-align: center; text-decoration: none;
        }
        .header-home-btn { width: 132px; }
        .header-logout-btn { width: 145px; }

        /* Page Specific Styles */
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .quiz-take-container {
            width: 100%;
            max-width: 1000px; /* Page is narrower */
            margin: 30px auto;
            padding: 0 15px;
            box-sizing: border-box;
        }

        /* Top header bar */
        .quiz-header-bar {
            display: block; 
            padding: 20px 30px;
            background: #FFFFFF;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }
        .quiz-header-info {
            width: 100%;
        }
        .quiz-header-title {
            font-size: 24px;
            font-weight: 700;
            color: #000;
        }
        .quiz-header-details {
            font-size: 16px;
            color: #555;
            margin-top: 4px;
        }

        /* Progress Bar */
        .progress-bar-container {
            width: 100%; /* Spans full width of card */
            height: 8px;
            background: #E0E0E0;
            border-radius: 4px;
            margin-top: 15px;
            overflow: hidden;
        }
        .progress-bar-fill {
            height: 100%;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 4px;
            width: 0%; /* Start at 0 */
            transition: width 0.3s ease;
        }

        /* Main Question Card */
        .question-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 30px 40px;
            margin-top: 30px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }
        
        .question-header {
            display: flex;
            align-items: baseline;
            gap: 10px;
        }
        .question-tag {
            display: inline-block;
            padding: 4px 12px;
            background: #F5F3FE;
            color: #610099;
            font-weight: 700;
            font-size: 14px;
            border-radius: 6px;
        }
        .question-points {
            font-size: 14px;
            font-weight: 400;
            color: #8D97AA;
        }

        .question-text {
            font-size: 28px;
            font-weight: 700;
            color: #000;
            margin-top: 15px;
            margin-bottom: 30px;
        }

        /* Answer Options */
        .options-list { display: flex; flex-direction: column; gap: 15px; }

        /* This is the clickable block */
        .options-list label {
            display: flex; 
            align-items: center; 
            padding: 18px 20px;
            border: 2px solid #E0E0E0;
            border-radius: 10px;
            cursor: pointer; 
            transition: border-color 0.2s, background-color 0.2s, box-shadow 0.2s;
        }

        .options-list label:hover { 
            border-color: #9F00FB;
            background: #FDF9FF; 
        }

        .options-list input[type="radio"] {
            display: none;
        }

        /* 2. This styles the label WHEN the hidden radio button is checked */
        .options-list input[type="radio"]:checked + label {
            border-color: #610099;
            background: #F5F3FE;
            box-shadow: 0 0 8px rgba(97, 0, 153, 0.2);
        }

        /* 3. This highlights the "A, B, C" when selected (optional) */
        .options-list input[type="radio"]:checked + label .option-letter {
            background-color: #610099;
            color: #FFFFFF;
            border-color: #610099;
        }

        .option-letter {
            font-weight: 700; color: #333; border: 1px solid #AAA;
            border-radius: 50%; width: 24px; height: 24px;
            display: inline-flex; justify-content: center; align-items: center;
            margin-right: 15px; font-size: 14px; flex-shrink: 0;
            transition: background-color 0.2s, color 0.2s; /* Add transition */
        }

        /* Bottom Navigation */
        .quiz-navigation {
            display: flex; justify-content: space-between; align-items: center;
            margin-top: 30px;
        }
        .nav-button {
            display: inline-flex; align-items: center; justify-content: center;
            gap: 8px; padding: 12px 30px; border-radius: 10px;
            font-family: 'Sansation'; font-weight: 700; font-size: 16px;
            text-decoration: none; cursor: pointer; border: 1px solid #8D97AA;
            background: #FFFFFF; color: #333;
        }
        .nav-button:hover { background: #f7f7f7; }
        .nav-button.next-submit {
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF; border: none;
        }
        .nav-button.next-submit:hover { opacity: 0.9; }
        .nav-button:disabled {
            opacity: 0.6; cursor: not-allowed; background: #E0E0E0;
        }
        
        /* ============================
           NEW MODAL POPUP STYLES
        ============================ */
        .modal-backdrop {
            position: fixed; /* Sits on top of the whole screen */
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.6); /* Dark overlay */
            z-index: 100;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .modal-content {
            width: 100%;
            max-width: 500px; /* Width of the popup */
            padding: 40px;
            background: #FFFFFF;
            border-radius: 12px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.3);
            text-align: center;
        }

        .modal-title {
            font-size: 32px;
            font-weight: 700;
            color: #000;
        }

        .quiz-title-modal {
            font-size: 24px;
            font-weight: 400;
            color: #555;
            margin-top: 5px;
        }

        .score-display-modal {
            font-size: 72px;
            font-weight: 700;
            color: #610099; /* Your theme color */
            margin: 30px 0 10px;
        }

        .score-total-modal {
            font-size: 24px;
            font-weight: 400;
            color: #8D97AA;
        }

        .modal-button {
             display: inline-block;
             padding: 12px 30px;
             margin-top: 40px;
             background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
             border-radius: 10px;
             color: #FFFFFF;
             font-family: 'Sansation';
             font-weight: 700;
             font-size: 16px;
             text-decoration: none;
             cursor: pointer;
             border: none;
        }
        .modal-button:hover {
             opacity: 0.9;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <%-- Add ScriptManager for potential future partial-page updates --%>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <%-- 1. Reusable Header --%>
        <div class="header">
            <div class="header-content">
                <a href="UserDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- 2. Main Content Area --%>
        <div class="quiz-take-container">
            <%-- Top Info Bar --%>
            <div class="quiz-header-bar">
                <div class="quiz-header-info"> 
                    <div class="quiz-header-title">
                        <asp:Label ID="lblQuizTitle" runat="server" Text="Mathematics: Algebra Fundamentals"></asp:Label>
                    </div>
                    <div class="quiz-header-details">
                        <asp:Label ID="lblQuestionProgress" runat="server" Text="Question 1 of 3"></asp:Label>
                    </div>
                </div>
                <div class="progress-bar-container">
                    <asp:Panel ID="pnlProgressBarFill" runat="server" CssClass="progress-bar-fill"></asp:Panel>
                </div>
            </div>

            <%-- Question Card --%>
            <div class="question-card">
                <div class="question-header">
                    <asp:Label ID="lblQuestionTag" runat="server" CssClass="question-tag" Text="Question 1"></asp:Label>
                    <asp:Label ID="lblQuestionPoints" runat="server" CssClass="question-points" Text="(10 Points)"></asp:Label>
                </div>
                <h2 class="question-text">
                    <asp:Label ID="lblQuestionText" runat="server" Text="What is the value of x in the equation: 2x + 5 = 13?"></asp:Label>
                </h2>
                <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="options-list" RepeatLayout="Flow" RepeatDirection="Vertical">
                    <%-- Bound from code-behind --%>
                </asp:RadioButtonList>
            </div>

            <%-- Bottom Navigation --%>
            <div class="quiz-navigation">
                <asp:Button ID="btnPrevious" runat="server" Text="Previous" CssClass="nav-button" OnClick="btnPrevious_Click" />
                <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="nav-button next-submit" OnClick="btnNext_Click" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit Quiz" CssClass="nav-button next-submit" OnClick="btnSubmit_Click" Visible="false" />
            </div>

            <asp:HiddenField ID="hfCurrentQuestionIndex" runat="server" Value="0" />

            <asp:Panel ID="pnlModalBackdrop" runat="server" CssClass="modal-backdrop" Visible="false">
                <div class="modal-content">
                    <h1 class="modal-title">Quiz Submitted!</h1>
                    <h2 class="quiz-title-modal">
                        <asp:Label ID="lblModalQuizTitle" runat="server" Text="Quiz Title"></asp:Label>
                    </h2>
                    <div class="score-display-modal">
                        <asp:Label ID="lblModalPercentage" runat="server" Text="85%"></asp:Label>
                    </div>
                    <div class="score-total-modal">
                        <asp:Label ID="lblModalScoreDisplay" runat="server" Text="You scored 17 / 20"></asp:Label>
                    </div>
                    <%-- Use an asp:Button so it triggers a postback --%>
                    <asp:Button ID="btnBackToQuizzes" runat="server" Text="Back to Quizzes" 
                        CssClass="modal-button" OnClick="btnBackToQuizzes_Click" />
                </div>
            </asp:Panel>
            </div> <%-- End of .quiz-take-container --%>
    </form>
</body>
</html>