<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateQuiz.aspx.cs" Inherits="BrainBlitz.CreateQuiz" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Create New Quiz - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css"> <%-- Link to global CSS --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" /> <%-- Font Awesome --%>

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

        /* --- End Header --- */

        /* Page Specific Styles */
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .create-quiz-container {
            width: 100%;
            max-width: 1440px;
            margin: 0 auto;
            padding: 30px 50px 50px;
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            gap: 30px;
        }

        .page-header {
            margin-bottom: 10px;
        }
        .page-title {
            font-size: 36px; font-weight: 700; color: #000; margin-bottom: 5px;
        }
        .page-subtitle {
            font-size: 20px; color: #8D97AA; font-weight: 700;
        }

        /* Card Styling */
        .form-card {
            background: #FFFFFF;
            border-radius: 8px;
            padding: 24px;
            display: flex;
            flex-direction: column;
            gap: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .card-title {
            font-family: 'Sansation'; font-weight: 700; font-size: 24px;
            color: #000000; margin-bottom: 10px;
        }

        /* Form Layout */
        .form-row {
            display: flex;
            gap: 25px;
            align-items: flex-start;
        }
        .form-group {
            display: flex;
            flex-direction: column;
            gap: 4px;
            flex: 1;
             min-width: 0;
        }
        .form-label {
            font-family: 'Sansation'; font-weight: 700; font-size: 20px;
            color: #000000;
        }
        /* Common Input Styling */
        .input-field, .dropdown-field, .textarea-field {
            box-sizing: border-box;
            width: 100%;
            height: 50px;
            background: #F5F3FE;
            border: 1px solid #8D97AA;
            border-radius: 8px;
            padding: 10px 15px;
            font-family: 'Sansation'; font-size: 16px; color: #333;
        }
         .input-field::placeholder, .textarea-field::placeholder { color: #8D97AA; }
         .textarea-field {
            height: 175px;
            resize: vertical;
         }


        /* Answer Options Specifics */
        .options-list {
            display: flex;
            flex-direction: column;
            gap: 15px;
            margin-top: 10px;
        }
        .option-row {
            display: flex;
            align-items: center;
            gap: 12px;
        }
        .option-row input[type="radio"] {
             width: 20px;
             height: 20px;
             accent-color: #610099;
             cursor: pointer;
        }
        .option-row .input-field {
             flex-grow: 1;
             height: 50px;
        }

        .helper-text {
            font-size: 14px;
            color: #8D97AA;
            margin-top: 5px;
        }

        /* Buttons Section */
        .action-buttons-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 10px;
        }

        .question-title-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            width: 100%;
            margin-bottom: 10px;
        }

        .delete-question-button {
            color: #DC3545;
            text-decoration: none;
            font-size: 14px;
            font-weight: normal;
            background: none;
            border: none;
            cursor: pointer;
            padding: 5px;
            margin-left: auto
        }
        .delete-question-button i { margin-right: 4px;}

        .delete-question-button:hover {
            text-decoration: underline;
            color: #a71d2a;
        }
        /* --- End Delete Button Styles --- */

        /* Base button style */
        .form-button {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 10px 24px;
            gap: 8px;
            border-radius: 8px;
            font-family: 'Sansation'; font-weight: 700; font-size: 16px;
            border: 1px solid #8D97AA;
            cursor: pointer;
            text-decoration: none;
            height: 42px;
             min-width: 170px;
             box-sizing: border-box;
        }
        .add-question-button {
             background: #FFFFFF;
             color: #000000;
        }
        .add-question-button i { color: #1E1E1E; }

        .submit-quiz-button {
             background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
             color: #FFFFFF;
             border: none;
        }
         .submit-quiz-button i { color: #FFFFFF; }

         .form-button:hover {
              transform: translateY(-2px);
              box-shadow: 0 4px 8px rgba(0,0,0,0.1);
         }

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- Reusable Header --%>
        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click" CausesValidation="false">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- Main Content Area --%>
        <div class="create-quiz-container">

            <div class="page-header">
                <h1 class="page-title">Create New Quiz</h1>
                <p class="page-subtitle">Build an engaging quiz for your students</p>
            </div>

            <%-- Quiz Info Card --%>
            <div class="form-card">
                <h2 class="card-title">Quiz Information</h2>
                <div class="form-row">
                    <div class="form-group">
                        <asp:Label ID="lblQuizTitle" runat="server" AssociatedControlID="txtQuizTitle" CssClass="form-label">Quiz Title</asp:Label>
                        <asp:TextBox ID="txtQuizTitle" runat="server" CssClass="input-field" placeholder="e.g., Algebra Fundamentals"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvQuizTitle" runat="server" ControlToValidate="txtQuizTitle"
                             ErrorMessage="Quiz Title is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblSubject" runat="server" AssociatedControlID="ddlSubject" CssClass="form-label">Subject</asp:Label>
                        <asp:DropDownList ID="ddlSubject" runat="server" CssClass="dropdown-field">

                        </asp:DropDownList>
                         <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="ddlSubject"
                             ErrorMessage="Subject is required." ForeColor="Red" Display="Dynamic" InitialValue=""></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblDifficulty" runat="server" AssociatedControlID="ddlDifficulty" CssClass="form-label">Difficulty</asp:Label>
                        <asp:DropDownList ID="ddlDifficulty" runat="server" CssClass="dropdown-field">
                            <asp:ListItem Text="Easy" Value="Easy"></asp:ListItem>
                            <asp:ListItem Text="Medium" Value="Medium" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Hard" Value="Hard"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <asp:PlaceHolder ID="phQuestions" runat="server"></asp:PlaceHolder>


            <%-- Action Buttons Row --%>
            <div class="action-buttons-row">
                 <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" CssClass="form-button add-question-button" OnClick="btnAddQuestion_Click" CausesValidation="false"/>

                <asp:Button ID="btnCreateQuizSubmit" runat="server" Text="Create Quiz" CssClass="form-button submit-quiz-button" OnClick="btnCreateQuizSubmit_Click" />
            </div>

             <%-- For displaying error messages --%>
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" HeaderText="Please fix the following issues:" />


        </div>

    </form>
</body>
</html>