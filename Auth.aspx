<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>BrainBlitz - Auth</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <style>
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
        <div class="signin-div" id="signinForm">
            <div class="title">Welcome Back</div>
            <div class="subtitle">Sign in to your account to continue</div>
            
            <div class="input-container">
                <div class="input-label">Email</div>
                <input type="email" class="input-field" placeholder="your@email.com">
                
                <div class="input-label">Password</div>
                <input type="password" class="input-field" placeholder="****************************">
            </div>

            <button class="submit-btn">Sign In</button>
        </div>

        <!-- Sign Up Form -->
        <div class="signup-div" id="signupForm">
            <div class="title">Create Account</div>
            <div class="subtitle">Sign up to get started with BrainBlitz</div>
            
            <div class="input-container">
                <div class="input-label">Full Name</div>
                <input type="text" class="input-field" placeholder="Full Name">
                
                <div class="input-label">Email</div>
                <input type="email" class="input-field" placeholder="your@email.com">
                
                <div class="input-label">Password</div>
                <input type="password" class="input-field" placeholder="****************************">
                
                <div class="input-label">I am a...</div>
                <select class="input-field">
                    <option value="">Student</option>
                    <option value="teacher">Teacher</option>
                </select>
            </div>

            <button class="submit-btn">Sign Up</button>
        </div>
    </div>

    <script>
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

        // Add click event listeners to toggle buttons
        document.addEventListener('DOMContentLoaded', function () {
            const toggleBtns = document.querySelectorAll('.toggle-btn');
            toggleBtns[0].addEventListener('click', () => toggleForm('signin'));
            toggleBtns[1].addEventListener('click', () => toggleForm('signup'));
        });
    </script>
</body>
</html>