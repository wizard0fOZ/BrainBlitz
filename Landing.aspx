<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>
<html>
<head>
    <title>BrainBlitz - Landing Page</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Sansation', sans-serif;
            background: #F2F2F2;
            width: 1440px;
            height: 1560px;
            position: relative;
        }

        .header {
            position: absolute;
            width: 1440px;
            height: 75px;
            left: 0px;
            top: 0px;
            background: #FFFFFF;
        }

        .logo {
            position: absolute;
            width: 312px;
            height: 60px;
            left: 32px;
            top: 7px;
            background: url(image.png);
        }

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
            left: 1084px;
            top: 17px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

        .header-signin-btn span {
            width: 63px;
            height: 22px;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #FFFFFF;
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
            left: 1264px;
            top: 17px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

        .header-getstarted-btn span {
            width: 102px;
            height: 22px;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #FFFFFF;
        }

        .slogan-div {
            position: absolute;
            width: 848px;
            height: 477px;
            left: 296px;
            top: 75px;
        }

        .main-title {
            position: absolute;
            width: 733px;
            height: 168px;
            left: 57px;
            top: 57px;
            font-weight: 700;
            font-size: 75px;
            line-height: 84px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #000000;
        }

        .subtitle {
            position: absolute;
            width: 656px;
            height: 54px;
            left: 96px;
            top: 248px;
            font-weight: 700;
            font-size: 24px;
            line-height: 27px;
            display: flex;
            align-items: center;
            text-align: center;
            color: #8D97AA;
        }

        .hero-getstarted-btn {
            display: flex;
            flex-direction: row;
            align-items: center;
            padding: 14px 20px;
            gap: 7px;
            position: absolute;
            width: 213px;
            height: 50px;
            left: 96px;
            top: 370px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

        .hero-getstarted-btn span {
            width: 146px;
            height: 22px;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #FFFFFF;
        }

        .hero-signin-btn {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 14px 56px;
            gap: 10px;
            position: absolute;
            width: 175px;
            height: 50px;
            left: 567px;
            top: 370px;
            background: #FFFFFF;
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

        .hero-signin-btn span {
            width: 63px;
            height: 22px;
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
        }

        .info-div {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 75px 0px;
            gap: 30px;
            position: absolute;
            width: 1428px;
            height: 406px;
            left: 6px;
            top: 552px;
        }

        .feature-card {
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
            align-items: center;
            padding: 20px 56px;
            gap: 20px;
            width: 400px;
            height: 256px;
            background: #FFFFFF;
            border-radius: 10px;
        }

        .feature-icon {
            width: 87px;
            height: 87px;
            background: url(image.png);
        }

        .feature-title {
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            text-align: center;
            color: #000000;
        }

        .feature-description {
            width: 284px;
            font-weight: 400;
            font-size: 14px;
            line-height: 16px;
            text-align: center;
            color: #000000;
        }

        .user-div {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 75px 0px;
            gap: 30px;
            position: absolute;
            width: 1428px;
            height: 528px;
            left: 6px;
            top: 971px;
        }

        .section-title {
            width: 297px;
            height: 40px;
            font-weight: 700;
            font-size: 36px;
            line-height: 40px;
            text-align: center;
            color: #000000;
        }

        .section-subtitle {
            width: 571px;
            height: 22px;
            font-weight: 400;
            font-size: 20px;
            line-height: 22px;
            text-align: center;
            color: #000000;
        }

        .userinfo-div {
            display: flex;
            flex-direction: row;
            align-items: flex-start;
            padding: 0px;
            gap: 30px;
            width: 1260px;
            height: 256px;
        }

        .user-card {
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: flex-start;
            padding: 20px 30px;
            gap: 20px;
            width: 400px;
            height: 256px;
            background: #FFFFFF;
            border-radius: 10px;
        }

        .user-icon {
            width: 50px;
            height: 46px;
            background: url(image.png);
        }

        .user-type-title {
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #000000;
        }

        .user-features {
            width: 284px;
            font-weight: 400;
            font-size: 14px;
            line-height: 16px;
            color: #000000;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="logo"></div>
            <button class="header-signin-btn">
                <span>Sign In</span>
            </button>
            <button class="header-getstarted-btn">
                <span>Get Started</span>
            </button>
        </div>

        <div class="slogan-div">
            <div class="main-title">Learn Smarter with BrainBlitz</div>
            <div class="subtitle">Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey.</div>
            <button class="hero-getstarted-btn">
                <span>Get Started Free</span>
            </button>
            <button class="hero-signin-btn">
                <span>Sign In</span>
            </button>
        </div>

        <div class="info-div">
            <div class="feature-card">
                <div class="feature-icon"></div>
                <div class="feature-title">Interactive Quizzes</div>
                <div class="feature-description">Engage with animated quizzes, track your progress, and improve with unlimited attempts.</div>
            </div>
            <div class="feature-card">
                <div class="feature-icon"></div>
                <div class="feature-title">Rich Resources</div>
                <div class="feature-description">Access a comprehensive library of learning materials, bookmark favorites, and track completion.</div>
            </div>
            <div class="feature-card">
                <div class="feature-icon"></div>
                <div class="feature-title">Track Progress</div>
                <div class="feature-description">Monitor your learning journey with detailed analytics and performance insights.</div>
            </div>
        </div>

        <div class="user-div">
            <div class="section-title">Built For Everyone</div>
            <div class="section-subtitle">Tailored experiences for students, teachers, and administrators</div>
            <div class="userinfo-div">
                <div class="user-card">
                    <div class="user-icon"></div>
                    <div class="user-type-title">Students</div>
                    <div class="user-features">-Take interactive quizzes<br/>-Access learning resources<br/>-Track your progress<br/>-Join discussions</div>
                </div>
                <div class="user-card">
                    <div class="user-icon"></div>
                    <div class="user-type-title">Teachers</div>
                    <div class="user-features">-Create and manage quizzes<br/>-Upload learning materials<br/>-Review student performance<br/>-Subject-based access</div>
                </div>
                <div class="user-card">
                    <div class="user-icon"></div>
                    <div class="user-type-title">Admin</div>
                    <div class="user-features">-Manage users and roles<br/>-Assign subjects to teachers<br/>-View analytics dashboard<br/>-System-wide control</div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>