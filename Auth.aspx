<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>BrainBlitz - Auth</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <style>
        .toggle-div {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 5px;
            gap: 5px;
            position: absolute;
            width: 600px;
            height: 50px;
            left: 420px;
            top: 311px;
            background: #FFFFFF;
            border-radius: 10px;
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

        .signin-div {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            padding: 27px;
            gap: 10px;
            position: absolute;
            width: 600px;
            height: 400px;
            left: 420px;
            top: 371px;
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
            left: 420px;
            top: 371px;
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
    <!-- Logo -->
    <div class="logo"></div>

    <!-- BrainStorm Title -->
    <div class="name"></div>

    <!-- Sign In / Sign Up Toggle -->
    <div class="toggle-div">
        <div class="toggle-btn active">Sign In</div>
        <div class="toggle-btn">Sign Up</div>
    </div>

    <!-- Sign In Form -->
    <div class="signin-div">
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
    <div class="signup-div">
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
                <option value="parent">Parent</option>
            </select>
        </div>

        <button class="submit-btn">Sign Up</button>
    </div>
</body>
</html>