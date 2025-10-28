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
    </style>
</head>
<body>
    <!-- Logo and Name Header -->
    <div class="auth-header">
        <div class="logo"></div>
        <div class="name"></div>
    </div>

    <!-- Sign In / Sign Up Toggle -->
    <div class="toggle-div">
        <div class="toggle-btn active">Sign In</div>
        <div class="toggle-btn">Sign Up</div>
    </div>

    <!-- Forms Container -->
    <div class="form-container">
        <!-- Sign In Form -->
        <form class="signin-div visible" id="signinForm" method="POST" action="/api/signin">
            <div class="title">Welcome Back</div>
            <div class="subtitle">Sign in to your account to continue</div>
            
            <div class="success-message" id="signinSuccess"></div>
            
            <div class="input-container">
                <div class="input-label">Email</div>
                <input type="email" name="email" class="input-field" placeholder="your@email.com" required>
                <div class="error-message" id="signinEmailError"></div>
                
                <div class="input-label">Password</div>
                <input type="password" name="password" class="input-field" placeholder="****************************" required>
                <div class="error-message" id="signinPasswordError"></div>
            </div>

            <button type="submit" class="submit-btn">Sign In</button>
        </form>

        <!-- Sign Up Form -->
        <form class="signup-div hidden" id="signupForm" method="POST" action="/api/signup">
            <div class="title">Create Account</div>
            <div class="subtitle">Sign up to get started with BrainBlitz</div>
            
            <div class="success-message" id="signupSuccess"></div>
            
            <div class="input-container">
                <div class="input-label">Full Name</div>
                <input type="text" name="fullName" class="input-field" placeholder="Full Name" required>
                <div class="error-message" id="signupNameError"></div>
                
                <div class="input-label">Email</div>
                <input type="email" name="email" class="input-field" placeholder="your@email.com" required>
                <div class="error-message" id="signupEmailError"></div>
                
                <div class="input-label">Password</div>
                <input type="password" name="password" class="input-field" placeholder="****************************" required minlength="6">
                <div class="error-message" id="signupPasswordError"></div>
                
                <div class="input-label">I am a...</div>
                <select name="role" class="input-field" required>
                    <option value="Student">Student</option>
                    <option value="Teacher">Teacher</option>
                </select>
            </div>

            <button type="submit" class="submit-btn">Sign Up</button>
        </form>
    </div>

    <script>
        // Form Toggle Functionality
        function toggleForm(formType) {
            const signinForm = document.getElementById('signinForm');
            const signupForm = document.getElementById('signupForm');
            const toggleBtns = document.querySelectorAll('.toggle-btn');

            if (formType === 'signin') {
                signinForm.style.visibility = 'visible';
                signupForm.style.visibility = 'hidden';
                toggleBtns[0].classList.add('active');
                toggleBtns[1].classList.remove('active');
            } else {
                signinForm.style.visibility = 'hidden';
                signupForm.style.visibility = 'visible';
                toggleBtns[0].classList.remove('active');
                toggleBtns[1].classList.add('active');
            }
        }

        // Toggle Button Event Listener
        document.addEventListener('DOMContentLoaded', function () {
            const toggleBtns = document.querySelectorAll('.toggle-btn');
            toggleBtns[0].addEventListener('click', () => toggleForm('signin'));
            toggleBtns[1].addEventListener('click', () => toggleForm('signup'));

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