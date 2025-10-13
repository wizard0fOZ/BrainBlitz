<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>BrainBlitz - Learn Smarter</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
    <style>
        .header-signin-btn {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 9px 38px;
            gap: 10px;
            position: absolute;
            width: 139px;
            height: 40px;
            right: 180px;
            top: 17px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
        }

        .header-signin-btn span {
            width: 63px;
            height: 22px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #FFFFFF;
            flex: none;
            order: 0;
            flex-grow: 0;
        }

        .header-getstarted-btn {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 9px 19px;
            gap: 10px;
            position: absolute;
            width: 140px;
            height: 40px;
            right: 20px;
            top: 17px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
        }

        .header-getstarted-btn span {
            width: 102px;
            height: 22px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #FFFFFF;
            flex: none;
            order: 0;
            flex-grow: 0;
        }

        /* Slogan Div */
        .slogan-div {
            position: absolute;
            width: 60%;
            max-width: 848px;
            height: auto;
            left: 50%;
            transform: translateX(-50%);
            top: 75px;
            padding: 20px;
        }

        .main-heading {
            position: relative;
            width: 100%;
            left: 0;
            top: 0;
            margin: 57px auto 0;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: clamp(40px, 5.2vw, 75px);
            line-height: 1.12;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            justify-content: center;
        }

        .sub-heading {
            position: relative;
            width: 100%;
            max-width: 656px;
            left: 0;
            margin: 23px auto 0;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: clamp(18px, 1.7vw, 24px);
            line-height: 1.12;
            display: flex;
            align-items: center;
            text-align: center;
            color: #8D97AA;
            justify-content: center;
        }

        .hero-buttons {
            display: flex;
            gap: 30px;
            justify-content: center;
            margin-top: 65px;
            flex-wrap: wrap;
        }

        .hero-getstarted-btn {
            display: flex;
            flex-direction: row;
            align-items: center;
            padding: 14px 20px;
            gap: 7px;
            width: 213px;
            height: 50px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
        }

        .hero-getstarted-btn span {
            width: 146px;
            height: 22px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #FFFFFF;
            flex: none;
            order: 0;
            flex-grow: 0;
        }

        .arrow-icon {
            width: 20px;
            height: 20px;
            flex: none;
            order: 1;
            flex-grow: 0;
            border: 4px solid #F3F3F3;
            border-left: 0;
            border-bottom: 0;
            transform: rotate(45deg);
        }

        .hero-signin-btn {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 14px 56px;
            gap: 10px;
            width: 175px;
            height: 50px;
            background: #FFFFFF;
            border-radius: 10px;
            cursor: pointer;
        }

        .hero-signin-btn span {
            width: 63px;
            height: 22px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            flex: none;
            order: 0;
            flex-grow: 0;
        }

        /* Info Div */
        .info-div {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 75px 20px;
            gap: 30px;
            position: absolute;
            width: 100%;
            left: 0;
            top: 552px;
            flex-wrap: wrap;
        }

        .feature-card {
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
            align-items: center;
            gap: 20px;
            width: 100%;
            max-width: 400px;
            min-height: 256px;
            background: #FFFFFF;
            border-radius: 10px;
            flex: 1 1 300px;
            padding: 20px 56px;
        }


        .feature-icon {
            width: 87px;
            height: 87px;
            flex: none;
            order: 0;
            flex-grow: 0;
            background-size: contain;
            background-repeat: no-repeat;
        }


        .feature-icon.quiz {
            background: url('Images/Interactive_Quizzes.png');
        }

        .feature-icon.resources {
            width: 89px;
            height: 89px;
            background: url('Images/Rich_Resources.png');
        }

        .feature-icon.progress {
            width: 89px;
            height: 89px;
            background: url('Images/Track_Progress.png');
        }

        .feature-title {
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            flex: none;
            order: 1;
            flex-grow: 0;
        }

        .feature-description {
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 400;
            font-size: 14px;
            line-height: 16px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            width: 100%;
            max-width: 284px;
            flex: none;
            order: 2;
            flex-grow: 0;
        }

        /* User Div */
        .user-div {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 75px 20px;
            gap: 30px;
            position: absolute;
            width: 100%;
            left: 0;
            top: 971px;
        }

        .user-section-title {
            width: auto;
            height: 40px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: clamp(28px, 2.5vw, 36px);
            line-height: 40px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            flex: none;
            order: 0;
            flex-grow: 0;
        }

        .user-section-subtitle {
            width: 100%;
            max-width: 571px;
            height: auto;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 400;
            font-size: clamp(16px, 1.4vw, 20px);
            line-height: 1.1;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            flex: none;
            order: 1;
            flex-grow: 0;
        }

        .userinfo-div {
            display: flex;
            flex-direction: row;
            align-items: flex-start;
            padding: 0px;
            gap: 30px;
            width: 100%;
            max-width: 1260px;
            flex: none;
            order: 2;
            flex-grow: 0;
            justify-content: center;
            flex-wrap: wrap;
        }

        .user-card {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: flex-start;
            padding: 20px 30px;
            gap: 20px;
            width: 100%;
            max-width: 400px;
            min-height: 256px;
            background: #FFFFFF;
            border-radius: 10px;
            flex: 1 1 300px;
        }

        .user-icon {
            width: 50px;
            height: 46px;
            flex: none;
            order: 0;
            flex-grow: 0;
            background-size: contain;
            background-repeat: no-repeat;
        }

        .user-icon.student {
            background: url('Images/Students.png');
        }

        .user-icon.teacher {
            background: url('Images/Teachers.png');
        }

        .user-icon.admin {
            height: 50px;
            background: url('Images/Admins.png');
        }

        .user-role {
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
            flex: none;
            order: 1;
            flex-grow: 0;
        }

        .user-features {
            width: 100%;
            max-width: 284px;
            font-family: 'Sansation';
            font-style: normal;
            font-weight: 400;
            font-size: 14px;
            line-height: 16px;
            display: flex;
            align-items: center;
            color: #000000;
            flex: none;
            order: 2;
            flex-grow: 0;
        }

        .header-signin-btn:hover,
        .header-getstarted-btn:hover,
        .hero-getstarted-btn:hover,
        .hero-signin-btn:hover{
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
        }
    </style>
</head>
<body>
    <!-- Header -->
    <div class="header">
        <div class="brainblitz"></div>
        <div class="header-signin-btn">
            <span>Sign In</span>
        </div>
        <div class="header-getstarted-btn">
            <span>Get Started</span>
        </div>
    </div>

    <!-- Slogan Div -->
    <div class="slogan-div">
        <div class="main-heading">Learn Smarter with BrainBlitz</div>
        <div class="sub-heading">Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey.</div>
        <div class="hero-buttons">
            <div class="hero-getstarted-btn">
                <span>Get Started Free</span>
                <div class="arrow-icon"></div>
            </div>
            <div class="hero-signin-btn">
                <span>Sign In</span>
            </div>
        </div>
    </div>

    <!-- Info Div -->
    <div class="info-div">
        <div class="feature-card quiz">
            <div class="feature-icon quiz"></div>
            <div class="feature-title">Interactive Quizzes</div>
            <div class="feature-description">Engage with animated quizzes, track your progress, and improve with unlimited attempts.</div>
        </div>
        <div class="feature-card resources">
            <div class="feature-icon resources"></div>
            <div class="feature-title">Rich Resources</div>
            <div class="feature-description">Access a comprehensive library of learning materials, bookmark favorites, and track completion.</div>
        </div>
        <div class="feature-card progress">
            <div class="feature-icon progress"></div>
            <div class="feature-title">Track Progress</div>
            <div class="feature-description">Monitor your learning journey with detailed analytics and performance insights.</div>
        </div>
    </div>

    <!-- User Div -->
    <div class="user-div">
        <div class="user-section-title">Built For Everyone</div>
        <div class="user-section-subtitle">Tailored experiences for students, teachers, and administrators</div>
        <div class="userinfo-div">
            <div class="user-card student">
                <div class="user-icon student"></div>
                <div class="user-role">Students</div>
                <div class="user-features">-Take interactive quizzes<br>-Access learning resources<br>-Track your progress<br>-Join discussions</div>
            </div>
            <div class="user-card teacher">
                <div class="user-icon teacher"></div>
                <div class="user-role">Teachers</div>
                <div class="user-features">-Create and manage quizzes<br>-Upload learning materials<br>-Review student performance<br>-Subject-based access</div>
            </div>
            <div class="user-card admin">
                <div class="user-icon admin"></div>
                <div class="user-role">Admin</div>
                <div class="user-features">-Manage users and roles<br>-Assign subjects to teachers<br>-View analytics dashboard<br>-System-wide control</div>
            </div>
        </div>
    </div>
</body>
</html>