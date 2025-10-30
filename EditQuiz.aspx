<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditQuiz.aspx.cs" Inherits="BrainBlitz.EditQuiz" Async="true" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Edit  Quiz - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css"> <%-- Link to global CSS --%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" /> <%-- Font Awesome --%>

    <style>
    .header {
        position: relative;
        width: 100%;
        height: 75px;
        background: #FFFFFF;
        box-sizing: border-box;
        z-index: 10;
    }

    /* Inner container for alignment within the header */
    .header-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
        max-width: 1440px;
        height: 100%;
        margin: 0 auto;
        padding: 0 50px;
        box-sizing: border-box;
    }

    /* Logo styling */
    .brainblitz {
        position: relative;
        left: auto;
        top: auto;
        width: 312px;
        height: 60px;
        background: url('../Images/BrainBlitz.png');
        background-size: contain;
        background-repeat: no-repeat;
        margin: 0;
        display: block;
        text-decoration: none;
    }
    .brainblitz:hover {
        transform: none;
        box-shadow: none;
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

        /* --- End Header --- */

        /* Page Specific Styles */
        body { background-color: #F3F3F3; font-family: 'Sansation', sans-serif;}

        .create-quiz-container {
            width: 100%;
            max-width: 1440px; /* Consistent max width */
            margin: 0 auto; /* Center content */
            padding: 30px 50px 50px; /* Consistent side padding, adjust top/bottom */
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            gap: 30px; /* Space between sections */
        }

        .page-header {
            margin-bottom: 10px; /* Reduced margin */
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
            gap: 20px; /* Space inside card */
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .card-title {
            font-family: 'Sansation'; font-weight: 700; font-size: 24px;
            color: #000000; margin-bottom: 10px; /* Add some space below title */
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
            gap: 4px; /* Space between label and input */
            flex: 1; /* Allow fields to grow equally */
             min-width: 0; /* Necessary for flex-grow */
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
            padding: 10px 15px; /* Adjust padding */
            font-family: 'Sansation'; font-size: 16px; color: #333;
        }
         .input-field::placeholder, .textarea-field::placeholder { color: #8D97AA; }
         .textarea-field {
            height: 175px; /* Height for question text */
            resize: vertical; /* Allow vertical resize */
         }


        /* Answer Options Specifics */
        .options-list {
            display: flex;
            flex-direction: column;
            gap: 15px; /* Increased gap */
            margin-top: 10px;
        }
        .option-row {
            display: flex;
            align-items: center;
            gap: 12px; /* Gap between radio and input */
        }
        .option-row input[type="radio"] { /* Basic styling for radio */
             width: 20px;
             height: 20px;
             accent-color: #610099; /* Match theme color */
             cursor: pointer;
        }
        .option-row .input-field {
             flex-grow: 1; /* Input takes remaining space */
             height: 50px; /* Match other inputs */
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
            margin-top: 10px; /* Space above buttons */
        }

        /* --- Ensure these styles for the Delete Button are included --- */
        .question-title-row {
            display: flex;
            justify-content: space-between; /* Pushes title left, button right */
            align-items: center; /* Vertically aligns them */
            width: 100%; /* Ensure it takes full width */
            margin-bottom: 10px; /* Add space below title row if needed */
        }

        .delete-question-button {
            color: #DC3545; /* Red color */
            text-decoration: none;
            font-size: 14px; /* Smaller font */
            font-weight: normal;
            background: none;
            border: none;
            cursor: pointer;
            padding: 5px; /* Add some clickable area */
            margin-left: auto; /* Push button to the far right */
        }
        .delete-question-button i { margin-right: 4px;} /* Space between icon and text */

        .delete-question-button:hover {
            text-decoration: underline;
            color: #a71d2a; /* Darker red on hover */
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
            height: 42px; /* Match Figma height */
             min-width: 170px; /* Match Figma width */
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
             border: none; /* Remove border for gradient button */
        }
         .submit-quiz-button i { color: #FFFFFF; }

         .form-button:hover {
              transform: translateY(-2px);
              box-shadow: 0 4px 8px rgba(0,0,0,0.1);
         }
    .action-buttons-row {
        justify-content: space-between; 
    }

    .submit-buttons-group {
        display: flex;
        gap: 15px;
    }

    .cancel-button {
         background: #F3F3F3;
         color: #555;
         border: 1px solid #8D97AA;
    }
    .cancel-button:hover {
         background: #e9e9e9;
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
                    <asp:LinkButton ID="btnHome" runat="server" CssClass="header-home-btn" OnClick="btnHome_Click">Home</asp:LinkButton>
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <%-- Main Content Area --%>
        <div class="create-quiz-container">

            <div class="page-header">
                <h1 class="page-title">Edit Quiz</h1>
                <p class="page-subtitle">Make changes to your quizzes here.</p>
            </div>

            <%-- Quiz Info Card --%>
            <div class="form-card">
                <h2 class="card-title">Quiz Information</h2>
                <div class="form-row">
                    <div class="form-group">
                        <asp:Label ID="lblQuizTitle" runat="server" AssociatedControlID="txtQuizTitle" CssClass="form-label">Quiz Title</asp:Label>
                        <asp:TextBox ID="txtQuizTitle" runat="server" CssClass="input-field" placeholder="e.g., Algebra Fundamentals"></asp:TextBox>
                        <%-- Add Validation controls if needed --%>
                        <asp:RequiredFieldValidator ID="rfvQuizTitle" runat="server" ControlToValidate="txtQuizTitle"
                             ErrorMessage="Quiz Title is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblSubject" runat="server" AssociatedControlID="ddlSubject" CssClass="form-label">Subject</asp:Label>
                        <asp:DropDownList ID="ddlSubject" runat="server" CssClass="dropdown-field">
                            <%-- Populated from code-behind --%>
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

            <%-- Placeholder for Dynamically Added Question Cards --%>
            <asp:PlaceHolder ID="phQuestions" runat="server"></asp:PlaceHolder>


            <div class="action-buttons-row">
                <asp:Button ID="btnAddQuestion" runat="server" Text="Add Question" CssClass="form-button add-question-button" OnClick="btnAddQuestion_Click" CausesValidation="false"/>
        
        <div class="submit-buttons-group">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="form-button cancel-button" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnUpdateQuiz" runat="server" Text="Update Quiz" CssClass="form-button submit-quiz-button" OnClick="btnUpdateQuiz_Click" />
        </div>
    </div>

             <%-- For displaying error messages --%>
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="Red" HeaderText="Please fix the following issues:" />


        </div>

    </form>
</body>
</html>