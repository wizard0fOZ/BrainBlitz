<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherQuiz.aspx.cs" Inherits="BrainBlitz.TeacherQuiz" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>My Quizzes - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
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
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;} /* Ensure background */

        .quiz-page-container {
            width: 100%;
            max-width: 1440px; /* Consistent max width */
            margin: 0 auto; /* Center content */
            padding: 30px 50px; /* Consistent side padding, adjust top/bottom */
            box-sizing: border-box;
        }

        .page-header {
            margin-bottom: 30px;
        }

        .page-title {
            font-size: 36px;
            font-weight: 700;
            color: #000;
            margin-bottom: 5px;
        }

        .page-subtitle {
            font-size: 20px;
            color: #8D97AA;
            font-weight: 700;
        }

        .top-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 40px;
            gap: 20px; /* Add gap between search and button */
        }

        .search-input-wrapper {
            display: flex;
            align-items: center;
            background: #FFFFFF;
            border: 1px solid #8D97AA;
            border-radius: 12px;
            padding: 15px 20px; /* Adjust padding */
            flex-grow: 1; /* Allow search to take available space */
            gap: 15px;
        }
        .search-input-wrapper i { /* Style for Font Awesome icon */
            color: #8D97AA;
            font-size: 24px;
        }
        .search-input {
            border: none;
            outline: none;
            font-family: 'Sansation';
            font-size: 20px; /* Adjust size */
            color: #8D97AA;
            width: 100%;
        }
        .search-input::placeholder {
            color: #8D97AA;
        }

        .create-quiz-button {
            display: inline-flex; /* Use inline-flex for button */
            align-items: center;
            gap: 10px;
            padding: 15px 30px; /* Adjust padding */
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            color: #FFFFFF;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            text-decoration: none;
            cursor: pointer;
            border: none; /* If using asp:Button */
            white-space: nowrap; /* Prevent button text wrapping */
        }
         .create-quiz-button i { /* Style for Font Awesome icon */
            font-size: 18px;
         }
        .create-quiz-button:hover {
             transform: translateY(-2px);
             box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
         }


        .quiz-list-table {
            background: #FFFFFF;
            border-radius: 10px;
            padding: 20px 30px; /* Adjust padding */
            box-shadow: 0 2px 4px rgba(0,0,0,0.05); /* Optional shadow */
        }

        .quiz-table-header,
        .quiz-table-row {
            display: grid; /* Use Grid for column alignment */
            /* Define column template - adjust fractions/widths as needed */
            grid-template-columns: 2fr 1fr 1fr 1fr 1fr 1fr 1.2fr;
            gap: 20px; /* Gap between columns */
            align-items: center;
            padding: 15px 0;
            border-bottom: 1px solid #EAEAEA; /* Separator line */
            font-size: 16px; /* Base font size for rows */
        }

        .quiz-table-header {
            font-weight: 700;
            color: #555; /* Header text color */
            font-size: 14px; /* Slightly smaller header text */
            text-transform: uppercase;
             border-bottom: 2px solid #DDD; /* Stronger header border */
        }
        .quiz-table-row:last-child {
            border-bottom: none; /* Remove border on last row */
        }

        .quiz-title-col .quiz-subtitle {
            font-size: 14px;
            color: #8D97AA;
            margin-top: 4px;
        }

        /* Column Specific Alignment/Styling */
        .col-quiz-title { grid-column: 1; }
        .col-subject { grid-column: 2; text-align: left; }
        .col-questions { grid-column: 3; text-align: center; }
        .col-attempts { grid-column: 4; text-align: center; }
        .col-avg-score { grid-column: 5; text-align: center; font-weight: 700;}
        .col-status { grid-column: 6; text-align: center;}
        .col-actions { grid-column: 7; text-align: right; }

        .tag-status-active { background-color: #82F797; color: #006D1A; }
        .tag-status-inactive { background-color: #E0E0E0; color: #555; }


         /* Score Coloring */
        .score-good { color: #28A745; }
        .score-medium { color: #FFA500; } 
        .score-bad { color: #DC3545; } 


        /* Action Buttons */
        .action-buttons {
            display: flex;
            justify-content: flex-end; /* Align icons to the right */
            gap: 15px; /* Space between icons */
        }
        .action-button {
            color: #555;
            font-size: 20px; /* Icon size */
            cursor: pointer;
            text-decoration: none;
            transition: color 0.2s ease;
        }
        .action-button:hover { color: #610099; }
        .action-button.delete:hover { color: #DC3545; } /* Red for delete hover */

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- Reusable Header --%>
        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- Main Content Area for this Page --%>
        <div class="quiz-page-container">

            <div class="page-header">
                <h1 class="page-title">My Quizzes</h1>
                <p class="page-subtitle">Create and manage your quizzes</p>
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="error-message" ForeColor="Red" Visible="false" EnableViewState="false"></asp:Label>

            <div class="top-bar">
                <div class="search-input-wrapper">
                    <i class="fas fa-search"></i>
                    <asp:TextBox ID="txtSearchQuizzes" runat="server" CssClass="search-input" placeholder="Search Quizzes..." AutoPostBack="true" OnTextChanged="txtSearchQuizzes_TextChanged"></asp:TextBox>
                </div>
                <asp:LinkButton ID="btnCreateQuiz" runat="server" CssClass="create-quiz-button" OnClick="btnCreateQuiz_Click">
                     <i class="fas fa-plus"></i> Create Quiz
                </asp:LinkButton>
            </div>

            <div class="quiz-list-table">
                <%-- Repeater for the Quiz List --%>
                <asp:Repeater ID="rptQuizzes" runat="server" OnItemCommand="rptQuizzes_ItemCommand">
                    <HeaderTemplate>
                        <div class="quiz-table-header">
                            <span class="col-quiz-title">Quiz Title</span>
                            <span class="col-subject">Subject</span>
                            <span class="col-questions">Questions</span>
                            <span class="col-attempts">Attempts</span>
                            <span class="col-avg-score">Avg Score</span>
                            <span class="col-status">Status</span>
                            <span class="col-actions">Actions</span>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="quiz-table-row">
                            <div class="col-quiz-title">
                                <%# Eval("QuizTitle") %>
                            </div>
                            <div class="col-subject">
                                <span class='<%# GetSubjectTagClass(Eval("SubjectName") == DBNull.Value ? "" : Eval("SubjectName").ToString()) %>'><%# Eval("SubjectName") == DBNull.Value ? "N/A" : Eval("SubjectName") %></span>
                            </div>
                            <div class="col-questions"><%# Eval("QuestionCount") %></div>
                            <div class="col-attempts"><%# Eval("AttemptCount") %></div>
                            <div class="col-avg-score">
                                <span class='<%# GetScoreClass(Eval("AverageScorePoints"), Eval("MaxPoints")) %>'><%#CalculatePercentage(Eval("AverageScorePoints"), Eval("MaxPoints")) %> </span>
                            </div>
                            <div class="col-status">
                                <asp:LinkButton ID="btnToggleStatus" runat="server"
                                    CommandName="ToggleStatus"
                                    CommandArgument='<%# Eval("QuizID") + "," + Convert.ToString(Eval("Status") ?? "Draft") %>'
                                    CssClass='<%# GetStatusTagClass(Convert.ToString(Eval("Status") ?? "Draft")) %>'
                                    ToolTip='<%# (Convert.ToString(Eval("Status") ?? "Draft") == "Active") ? "Click to set as Draft" : "Click to set as Active" %>'>
                                    <%# Convert.ToString(Eval("Status") ?? "Draft") %>
                                </asp:LinkButton>
                            </div>
                            <div class="col-actions action-buttons">
                                <asp:LinkButton ID="btnView" runat="server" CssClass="action-button" CommandName="View" CommandArgument='<%# Eval("QuizID") %>' ToolTip="View Quiz Details">
                                    <i class="fas fa-eye"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="action-button" CommandName="Edit" CommandArgument='<%# Eval("QuizID") %>' ToolTip="Edit Quiz">
                                     <i class="fas fa-pencil-alt"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="action-button delete" CommandName="Delete" CommandArgument='<%# Eval("QuizID") %>' OnClientClick="return confirm('Are you sure you want to delete this quiz?');" ToolTip="Delete Quiz">
                                     <i class="fas fa-trash-alt"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                     <FooterTemplate>
                         <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false" Style="text-align: center; padding: 40px; color: #888; border-top: 1px solid #EAEAEA;">
                             No quizzes found matching your criteria.
                         </asp:Panel>
                     </FooterTemplate>
                </asp:Repeater>
            </div>

        </div>

    </form>
</body>
</html>