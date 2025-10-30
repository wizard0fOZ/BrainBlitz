<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentQuiz.aspx.cs" Inherits="BrainBlitz.StudentQuiz" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Available Quizzes - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="../Content/Site.css"> <%-- Adjusted path --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
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

        /* Page Specific Styles */
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .quiz-page-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
        }

        .page-header {
            margin-bottom: 30px;
        }
        .page-title {
            font-size: 36px; font-weight: 700; color: #000; margin-bottom: 5px;
        }
        .page-subtitle {
            font-size: 20px; color: #8D97AA; font-weight: 400; /* Lighter weight per design */
        }

        .top-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 40px;
            gap: 20px;
            background: #FFFFFF;
            padding: 12px 20px;
            border-radius: 12px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .search-input-wrapper {
            display: flex;
            align-items: center;
            flex-grow: 1; /* Search takes up available space */
            gap: 15px;
        }
        .search-input-wrapper i { color: #8D97AA; font-size: 20px; }
        .search-input {
            border: none;
            outline: none;
            font-family: 'Sansation';
            font-size: 16px; /* Per design */
            color: #333;
            width: 100%;
        }
        .search-input::placeholder { color: #8D97AA; }
        
        .filter-pills {
            display: flex;
            gap: 10px;
        }
        .filter-pill {
            padding: 10px 20px;
            border-radius: 8px;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 14px;
            background: #F3F3F3;
            color: #555;
            cursor: pointer;
            text-decoration: none;
            border: none;
        }
        .filter-pill.active {
            background: #610099; /* Main purple color */
            color: #FFFFFF;
        }

        /* NEW Quiz Card Grid */
        .quiz-card-container {
            display: flex;
            flex-direction: row;
            flex-wrap: wrap;
            justify-content: center;
            gap: 30px;
        }

        .quiz-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 24px;
            display: flex;
            flex-direction: column;
            gap: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
            flex-basis: 655px;
        }

        .quiz-card-header .quiz-title {
            font-size: 24px;
            font-weight: 700;
            color: #610099; /* Purple title */
        }
        
        .quiz-card-tags {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }
        .tag {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 8px;
            font-size: 12px;
            font-weight: 700;
            text-align: center;
        }
        .tag-subject-math { background-color: #E6B9FF; color: #6A009C; }
        .tag-subject-science { background-color: #B9E6FF; color: #006A9C; }
        .tag-subject-english { background-color: aquamarine; color: darkcyan; }
        .tag-subject-default { background-color: #610099; color: #E6B9FF; }

        .tag-difficulty-medium { background-color: #FFF3B9; color: #9C6F00; }
        .tag-difficulty-hard { background-color: #FFB9B9; color: #9C0000; }
        .tag-difficulty-easy { background-color: #B9FFC6; color: #008D1A; }

        .quiz-card-details {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            border-top: 1px solid #F3F3F3;
            padding-top: 20px;
            font-size: 14px;
            color: #555;
        }
        .detail-item {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .detail-item i { /* Font Awesome icons */
            font-size: 16px;
            color: #8D97AA;
            width: 20px; /* Align icons */
            text-align: center;
        }
        .detail-item-best-score {
            font-weight: 700;
            color: #28A745; /* Green for best score */
        }
        .detail-item-best-score i { color: #28A745; }


        .quiz-card-button {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 10px;
            width: 100%;
            padding: 12px;
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
        .quiz-card-button:hover {
             transform: translateY(-2px);
             box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }
        .quiz-card-button i { font-size: 14px; }

        .error-message { color: #FF0000; font-size: 16px; margin-bottom: 15px; }

    </style>
</head>
<body>
    <form id="form1" runat="server">

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
        <div class="quiz-page-container">

            <div class="page-header">
                <h1 class="page-title">Available Quizzes</h1>
                <p class="page-subtitle">Challenge yourself and track your progress</p>
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" Visible="false"></asp:Label>

            <%-- 3. Top Bar with Search and Filters --%>
            <div class="top-bar">
                <div class="search-input-wrapper">
                    <i class="fas fa-search"></i>
                    <asp:TextBox ID="txtSearchQuizzes" runat="server" CssClass="search-input" placeholder="Search Quizzes..." AutoPostBack="true" OnTextChanged="txtSearchQuizzes_TextChanged"></asp:TextBox>
                </div>
            </div>

            <%-- 4. Quiz Card Grid --%>
            <div class="quiz-card-container">
                <asp:Repeater ID="rptStudentQuizzes" runat="server">
                    <ItemTemplate>
                        <div class="quiz-card">
                            
                            <div class="quiz-card-header">
                                <span class="quiz-title"><%# Eval("QuizTitle") %></span>
                                <div class="quiz-card-tags">
                                    <span class='<%# GetSubjectTagClass(Eval("SubjectName").ToString()) %>'>
                                        <%# Eval("SubjectName") %>
                                    </span>
                                    <span class='<%# GetDifficultyTagClass(Eval("Difficulty").ToString()) %>'>
                                        <%# Eval("Difficulty") %>
                                    </span>
                                </div>
                            </div>

                            <div class="quiz-card-details">
                                <div class="detail-item">
                                    <i class="fas fa-list-ol"></i>
                                    <span><%# Eval("QuestionCount") %> Questions</span>
                                </div>
                                <div class="detail-item">
                                    <i class="fas fa-redo"></i>
                                    <span><%# Eval("AttemptCount") %> Attempts</span>
                                </div>
                                <div class="detail-item detail-item-best-score">
                                    <i class="fas fa-trophy"></i>
                                        <span>Best: <%# CalculatePercentage(Eval("BestScorePoints"), Eval("MaxPoints")) %></span>
                                </div>
                            </div>

                            <a href='TakeQuiz.aspx?quizID=<%# Eval("QuizID") %>' class="quiz-card-button">
                                <i class="fas fa-play"></i>
                                <%# Convert.ToInt32(Eval("AttemptCount")) > 0 ? "Retake Quiz" : "Start Quiz" %>
                            </a>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            
            <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false" Style="text-align: center; padding: 50px; color: #888; font-size: 18px;">
                No quizzes are available for you at this time.
            </asp:Panel>

        </div>
    </form>
</body>
</html>