<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPasswordSecurity.aspx.cs" Inherits="BrainBlitz.ForgotPasswordSecurity" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <title>Reset Password - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="Content/Site.css" />
    <style>
        body {
            min-height: 100vh;
            background: #F3F3F3;
            font-family: 'Sansation', sans-serif;
        }

        .auth-header {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding-top: 60px;
            margin-bottom: 30px;
        }

        .reset-container {
            display: flex;
            justify-content: center;
            width: 100%;
        }

        .reset-card {
            width: 600px;
            background: #FFFFFF;
            border-radius: 10px;
            padding: 27px;
            box-sizing: border-box;
            display: flex;
            flex-direction: column;
            gap: 15px;
        }

        .title {
            font-weight: 700;
            font-size: 28px;
            color: #000000;
        }

        .subtitle {
            font-weight: 400;
            font-size: 18px;
            color: #555555;
        }

        .input-container {
            display: flex;
            flex-direction: column;
            gap: 10px;
            margin-top: 10px;
        }

        .input-label {
            font-weight: 700;
            font-size: 16px;
            color: #000000;
        }

        .input-field {
            display: flex;
            align-items: center;
            padding: 0 20px;
            width: 100%;
            height: 46px;
            background: #EEEEEE;
            border-radius: 12px;
            border: none;
            font-size: 16px;
            font-weight: 600;
        }

        .input-field::placeholder {
            color: rgba(0,0,0,0.5);
        }

        .submit-btn {
            margin-top: 10px;
            width: 100%;
            height: 40px;
            border-radius: 10px;
            border: none;
            background: #9F00FB;
            color: #FFFFFF;
            font-size: 18px;
            font-weight: 700;
            cursor: pointer;
        }

        .submit-btn:hover {
            background: #8A00DC;
        }

        .error-message {
            color: #FF0000;
            font-size: 14px;
            margin-top: 5px;
        }

        .success-message {
            color: #00A86B;
            font-size: 14px;
            margin-top: 5px;
        }

        .back-link {
            display: inline-block;
            margin-top: 10px;
            font-size: 14px;
            color: #610099;
            text-decoration: underline;
        }

        .question-text {
            font-weight: 600;
            font-size: 16px;
            color: #333333;
            margin-bottom: 4px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="auth-header">
            <div class="logo"></div>
            <div class="name"></div>
        </div>

        <div class="reset-container">
            <div class="reset-card">
                <div class="title">Reset Your Password</div>
                <div class="subtitle">Verify your identity with your security question.</div>

                <!-- Step 1: Email -->
                <div class="input-container">
                    <div class="input-label">Account Email</div>
                    <asp:TextBox ID="txtEmailLookup" runat="server" CssClass="input-field"
                                 placeholder="your@email.com" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmailLookup" runat="server"
                        ControlToValidate="txtEmailLookup"
                        ErrorMessage="Email is required"
                        CssClass="error-message"
                        Display="Dynamic"
                        ValidationGroup="Lookup"></asp:RequiredFieldValidator>

                    <asp:Button ID="btnFindAccount" runat="server"
                                Text="Find Account"
                                CssClass="submit-btn"
                                OnClick="btnFindAccount_Click"
                                ValidationGroup="Lookup" />

                    <asp:Label ID="lblLookupMessage" runat="server"
                               Visible="false"
                               CssClass="error-message"></asp:Label>
                </div>

                <asp:HiddenField ID="hfUserId" runat="server" />

                <!-- Step 2: Question + New Password -->
                <asp:Panel ID="pnlStep2" runat="server" Visible="false">
                    <hr />
                    <div class="input-container">
                        <div class="input-label">Security Question</div>
                        <div class="question-text">
                            <asp:Label ID="lblSecurityQuestion" runat="server" Text=""></asp:Label>
                        </div>

                        <div class="input-label">Your Answer</div>
                        <asp:TextBox ID="txtSecurityAnswer" runat="server" CssClass="input-field"
                                     placeholder="Enter your answer"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvSecurityAnswer" runat="server"
                            ControlToValidate="txtSecurityAnswer"
                            ErrorMessage="Answer is required"
                            CssClass="error-message"
                            Display="Dynamic"
                            ValidationGroup="Reset"></asp:RequiredFieldValidator>

                        <div class="input-label">New Password</div>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="input-field"
                                     TextMode="Password"
                                     placeholder="New password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server"
                            ControlToValidate="txtNewPassword"
                            ErrorMessage="New password is required"
                            CssClass="error-message"
                            Display="Dynamic"
                            ValidationGroup="Reset"></asp:RequiredFieldValidator>

                        <div class="input-label">Confirm New Password</div>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="input-field"
                                     TextMode="Password"
                                     placeholder="Confirm new password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ErrorMessage="Please confirm your new password"
                            CssClass="error-message"
                            Display="Dynamic"
                            ValidationGroup="Reset"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvPasswords" runat="server"
                            ControlToValidate="txtConfirmPassword"
                            ControlToCompare="txtNewPassword"
                            ErrorMessage="Passwords do not match"
                            CssClass="error-message"
                            Display="Dynamic"
                            ValidationGroup="Reset"></asp:CompareValidator>

                        <asp:Button ID="btnResetPassword" runat="server"
                                    Text="Reset Password"
                                    CssClass="submit-btn"
                                    OnClick="btnResetPassword_Click"
                                    ValidationGroup="Reset" />

                        <asp:Label ID="lblResetMessage" runat="server"
                                   Visible="false"
                                   CssClass="error-message"></asp:Label>
                    </div>
                </asp:Panel>

                <a href="Auth.aspx?mode=signin" class="back-link">Back to sign in</a>
            </div>
        </div>
    </form>
</body>
</html>
