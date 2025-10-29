<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Auth.aspx.cs" Inherits="BrainBlitz.Auth" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>BrainBlitz - Auth</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <style>
        body {
            min-height: 1120px;
        }

        .auth-header {
            display: flex;
            flex-direction: column; 
            align-items: center;
            padding-top: 80px;
            margin-bottom: 30px;
        }

        .toggle-div {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 5px;
            gap: 5px;
            margin: 0 auto;
            width: 600px;
            height: 50px;
            background: #FFFFFF;
            border-radius: 10px;
            margin-bottom: 10px;
        }

        .toggle-btn {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 9px 20px;
            gap: 10px;
            width: 290px;
            height: 40px;
            background: #EEEEEE;
            border-radius: 10px;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
            cursor: pointer;
            transition: background 0.3s;
        }

        .toggle-btn.active {
            background: #9F00FB;
            color: #FFFFFF;
        }

        .form-container {
            display: flex;
            justify-content: center;
            position: relative;
            width: 100%;
        }

        .signin-div {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            padding: 27px;
            gap: 10px;
            width: 600px;
            height: 400px;
            background: #FFFFFF;
            border-radius: 10px;
            visibility: visible;
        }

        .signup-div {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            padding: 27px;
            gap: 10px;
            position: absolute;
            width: 600px;
            height: 600px;
            top: 0;
            background: #FFFFFF;
            border-radius: 10px;
            visibility: hidden;
        }

        .title {
            font-weight: 700;
            font-size: 30px;
            line-height: 34px;
            color: #000000;
        }

        .subtitle {
            font-weight: 400;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
        }

        .input-container {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: flex-start;
            padding: 20px 0px;
            gap: 10px;
            width: 546px;
        }

        .input-label {
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
        }

        .input-field {
            display: flex;
            flex-direction: row;
            align-items: center;
            padding: 0px 20px;
            gap: 10px;
            width: 546px;
            height: 50px;
            background: #EEEEEE;
            border-radius: 15px;
            border: none;
            font-family: 'Sansation', sans-serif;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
        }

        .input-field::placeholder {
            color: rgba(0, 0, 0, 0.5);
        }

        .submit-btn {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 9px 20px;
            gap: 10px;
            width: 546px;
            height: 40px;
            background: #9F00FB;
            border-radius: 10px;
            border: none;
            font-family: 'Sansation', sans-serif;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #FFFFFF;
            cursor: pointer;
        }

        .submit-btn:hover {
            background: #8A00DC;
        }

        select.input-field {
            appearance: none;
            cursor: pointer;
        }

        .success-message {
            color: #28a745;
            font-size: 16px;
            margin: 10px 0;
            display: none;
        }

        .error-message {
            color: #dc3545;
            font-size: 14px;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Logo and Name Header -->
        <div class="auth-header">
            <div class="logo"></div>
            <div class="name"></div>
        </div>

        <!-- Sign In / Sign Up Toggle -->
        <div class="toggle-div">
            <div class="toggle-btn active" id="btnSignInToggle">Sign In</div>
            <div class="toggle-btn" id="btnSignUpToggle">Sign Up</div>
        </div>

        <!-- Forms Container -->
        <div class="form-container">
            <!-- Sign In Form -->
            <div class="signin-div" id="signinDiv">
                <div class="title">Welcome Back</div>
                <div class="subtitle">Sign in to your account to continue</div>
                
                <asp:Label ID="lblSignInSuccess" runat="server" CssClass="success-message"></asp:Label>
                
                <div class="input-container">
                    <div class="input-label">Email</div>
                    <asp:TextBox ID="txtSignInEmail" runat="server" CssClass="input-field" 
                                 placeholder="your@email.com" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSignInEmail" runat="server" 
                                                ControlToValidate="txtSignInEmail"
                                                ErrorMessage="Email is required" 
                                                CssClass="error-message"
                                                Display="Dynamic"
                                                ValidationGroup="SignIn"></asp:RequiredFieldValidator>
                    <asp:Label ID="lblSignInEmailError" runat="server" CssClass="error-message"></asp:Label>
                    
                    <div class="input-label">Password</div>
                    <asp:TextBox ID="txtSignInPassword" runat="server" CssClass="input-field" 
                                 placeholder="****************************" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSignInPassword" runat="server" 
                                                ControlToValidate="txtSignInPassword"
                                                ErrorMessage="Password is required" 
                                                CssClass="error-message"
                                                Display="Dynamic"
                                                ValidationGroup="SignIn"></asp:RequiredFieldValidator>
                    <asp:Label ID="lblSignInPasswordError" runat="server" CssClass="error-message"></asp:Label>
                </div>

                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="submit-btn" 
                            OnClick="btnSignIn_Click" ValidationGroup="SignIn" />
            </div>

            <!-- Sign Up Form -->
            <div class="signup-div" id="signupDiv">
                <div class="title">Create Account</div>
                <div class="subtitle">Sign up to get started with BrainBlitz</div>
                
                <asp:Label ID="lblSignUpSuccess" runat="server" CssClass="success-message"></asp:Label>
                
                <div class="input-container">
                    <div class="input-label">Full Name</div>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="input-field" 
                                 placeholder="Full Name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFullName" runat="server" 
                                                ControlToValidate="txtFullName"
                                                ErrorMessage="Full name is required" 
                                                CssClass="error-message"
                                                Display="Dynamic"
                                                ValidationGroup="SignUp"></asp:RequiredFieldValidator>
                    <asp:Label ID="lblSignUpNameError" runat="server" CssClass="error-message"></asp:Label>
                    
                    <div class="input-label">Email</div>
                    <asp:TextBox ID="txtSignUpEmail" runat="server" CssClass="input-field" 
                                 placeholder="your@email.com" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSignUpEmail" runat="server" 
                                                ControlToValidate="txtSignUpEmail"
                                                ErrorMessage="Email is required" 
                                                CssClass="error-message"
                                                Display="Dynamic"
                                                ValidationGroup="SignUp"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revSignUpEmail" runat="server"
                                                    ControlToValidate="txtSignUpEmail"
                                                    ErrorMessage="Invalid email format"
                                                    CssClass="error-message"
                                                    Display="Dynamic"
                                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                    ValidationGroup="SignUp"></asp:RegularExpressionValidator>
                    <asp:Label ID="lblSignUpEmailError" runat="server" CssClass="error-message"></asp:Label>
                    
                    <div class="input-label">Password</div>
                    <asp:TextBox ID="txtSignUpPassword" runat="server" CssClass="input-field" 
                                 placeholder="****************************" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSignUpPassword" runat="server" 
                                                ControlToValidate="txtSignUpPassword"
                                                ErrorMessage="Password is required" 
                                                CssClass="error-message"
                                                Display="Dynamic"
                                                ValidationGroup="SignUp"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revSignUpPassword" runat="server"
                                                    ControlToValidate="txtSignUpPassword"
                                                    ErrorMessage="Password must be at least 6 characters"
                                                    CssClass="error-message"
                                                    Display="Dynamic"
                                                    ValidationExpression=".{6,}"
                                                    ValidationGroup="SignUp"></asp:RegularExpressionValidator>
                    <asp:Label ID="lblSignUpPasswordError" runat="server" CssClass="error-message"></asp:Label>
                    
                    <div class="input-label">I am a...</div>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="input-field">
                        <asp:ListItem Value="Student" Selected="True">Student</asp:ListItem>
                        <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" CssClass="submit-btn" 
                            OnClick="btnSignUp_Click" ValidationGroup="SignUp" />
            </div>
        </div>

        <asp:HiddenField ID="hfCurrentForm" runat="server" Value="signin" />
    </form>
    <script>
        // Form Toggle Functionality
        function toggleForm(formType) {
            const signinDiv = document.getElementById('signinDiv');
            const signupDiv = document.getElementById('signupDiv');
            const btnSignInToggle = document.getElementById('btnSignInToggle');
            const btnSignUpToggle = document.getElementById('btnSignUpToggle');
            const hfCurrentForm = document.getElementById('<%= hfCurrentForm.ClientID %>');

            if (formType === 'signin') {
                signinDiv.style.visibility = 'visible';
                signupDiv.style.visibility = 'hidden';
                btnSignInToggle.classList.add('active');
                btnSignUpToggle.classList.remove('active');
                hfCurrentForm.value = 'signin';
            } else {
                signinDiv.style.visibility = 'hidden';
                signupDiv.style.visibility = 'visible';
                btnSignInToggle.classList.remove('active');
                btnSignUpToggle.classList.add('active');
                hfCurrentForm.value = 'signup';
            }
        }

        // Toggle Button Event Listener
        document.addEventListener('DOMContentLoaded', function () {
            const btnSignInToggle = document.getElementById('btnSignInToggle');
            const btnSignUpToggle = document.getElementById('btnSignUpToggle');

            btnSignInToggle.addEventListener('click', () => toggleForm('signin'));
            btnSignUpToggle.addEventListener('click', () => toggleForm('signup'));

            // URL parameter check to determine which form to show
            const urlParams = new URLSearchParams(window.location.search);
            const mode = urlParams.get('mode');

            if (mode === 'signup') {
                toggleForm('signup');
            } else if (mode === 'signin') {
                toggleForm('signin');
            }
        });
    </script>
</body>
</html>