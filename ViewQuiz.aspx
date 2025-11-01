<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewQuiz.aspx.cs" Inherits="BrainBlitz.ViewQuiz" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>View Quiz - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="../Content/Site.css" />

    <style>
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif; }

        /* HEADER */
        .header { height: 75px; background: #fff; display: flex; justify-content: center; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
        .header-content { display: flex; justify-content: space-between; align-items: center; width: 100%; max-width: 1200px; padding: 0 40px; }
        .brainblitz { width: 200px; height: 60px; background: url('../Images/BrainBlitz.png') no-repeat center / contain; display: block; }
        .header-buttons-wrapper { display: flex; gap: 24px; }
        .header-home-btn, .header-logout-btn {
            padding: 9px 30px; border-radius: 10px; background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #fff; font-weight: 700; font-size: 18px; text-decoration: none; text-align: center;
        }

        /* QUIZ */
        .quiz-take-container { max-width: 1000px; margin: 30px auto; padding: 0 15px; }
        .quiz-header-bar { background: #fff; border-radius: 12px; padding: 20px 30px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
        .quiz-header-title { font-size: 24px; font-weight: 700; }
        .progress-bar-container { height: 8px; background: #E0E0E0; border-radius: 4px; margin-top: 15px; }
        .progress-bar-fill { height: 100%; border-radius: 4px; background: linear-gradient(90deg, #610099 0%, #FF00D9 100%); transition: width 0.3s ease; }

        .question-card { background: #fff; border-radius: 12px; padding: 30px 40px; margin-top: 30px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
        .question-header { display: flex; align-items: baseline; gap: 10px; }
        .question-tag { background: #F5F3FE; color: #610099; padding: 4px 12px; border-radius: 6px; font-weight: 700; }
        .question-text { font-size: 26px; font-weight: 700; margin: 20px 0; }

        /* OPTIONS */
        .options-list { display: flex; flex-direction: column; gap: 15px; }

        /* This is the clickable block */
        .options-list label {
            display: flex; 
            align-items: center; 
            padding: 18px 20px;
            border: 2px solid #E0E0E0;
            border-radius: 10px;
            cursor: default;
            transition: border-color 0.2s, background-color 0.2s;
        }

        .option-letter {
            font-weight: 700; color: #333; border: 1px solid #AAA;
            border-radius: 50%; width: 24px; height: 24px;
            display: inline-flex; justify-content: center; align-items: center;
            margin-right: 15px; font-size: 14px; flex-shrink: 0;
        }

        /* Hide radio buttons */
        .options-list input[type="radio"] {
            display: none;
        }

        /* Correct answer highlight */
        .correct-answer-highlight {
            display: flex !important;
            width: 100%;
        }
        
        .correct-answer-highlight label {
            border-color: #28A745 !important;
            background: #F0FFF4 !important;
        }
        
        .correct-answer-highlight label .option-letter {
            background-color: #28A745 !important;
            color: #fff !important;
            border-color: #28A745 !important;
        }
        
        /* Alternative selector based on checked state */
        .options-list input[type="radio"]:checked + label {
            border-color: #28A745 !important;
            background: #F0FFF4 !important;
        }
        
        .options-list input[type="radio"]:checked + label .option-letter {
            background-color: #28A745 !important;
            color: #fff !important;
            border-color: #28A745 !important;
        }

        .options-list[disabled] {
            pointer-events: none;
        }

        /* NAV BUTTONS */
        .quiz-navigation { display: flex; justify-content: space-between; align-items: center; margin-top: 30px; }
        .nav-button {
            padding: 12px 30px; border-radius: 10px; border: 1px solid #8D97AA;
            background: #fff; font-weight: 700; cursor: pointer;
        }
        .nav-button.next-submit {
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #fff; border: none;
        }
        .nav-button:disabled { opacity: 0.6; cursor: not-allowed; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnHome" runat="server" CssClass="header-home-btn" OnClick="btnHome_Click">Home</asp:LinkButton>
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="quiz-take-container">
            <div class="quiz-header-bar">
                <div class="quiz-header-title">
                    <asp:Label ID="lblQuizTitle" runat="server" Text="Quiz Preview"></asp:Label>
                </div>
                <div class="quiz-header-details">
                    <asp:Label ID="lblQuestionProgress" runat="server" Text="Question 1 of X"></asp:Label>
                </div>
                <div class="progress-bar-container">
                    <asp:Panel ID="pnlProgressBarFill" runat="server" CssClass="progress-bar-fill"></asp:Panel>
                </div>
            </div>

            <div class="question-card">
                <div class="question-header">
                    <asp:Label ID="lblQuestionTag" runat="server" CssClass="question-tag" Text="Question X"></asp:Label>
                    <asp:Label ID="lblQuestionPoints" runat="server" CssClass="question-points" Text="(X Points)"></asp:Label>
                </div>
                <h2 class="question-text">
                    <asp:Label ID="lblQuestionText" runat="server" Text="Question text will load here."></asp:Label>
                </h2>
                <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="options-list" RepeatLayout="Flow" RepeatDirection="Vertical"></asp:RadioButtonList>
            </div>

            <div class="quiz-navigation">
                <asp:Button ID="btnPrevious" runat="server" Text="Previous" CssClass="nav-button" OnClick="btnPrevious_Click" CausesValidation="false" />
                <div>
                    <asp:Button ID="btnBack" runat="server" Text="Back to Quiz List" CssClass="nav-button" OnClick="btnBack_Click" CausesValidation="false" />
                    <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="nav-button next-submit" OnClick="btnNext_Click" CausesValidation="false" />
                </div>
            </div>

            <asp:HiddenField ID="hfCurrentQuestionIndex" runat="server" Value="0" />
        </div>
    </form>
</body>
</html>
