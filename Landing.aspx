<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Landing.aspx.cs" Inherits="BrainBlitz.Landing" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta charset="utf-8" />
    <meta name="description" content="BrainBlitz - Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey." />
    <title>BrainBlitz - Learn Smarter</title>
    
    <style type="text/css">
        @import url("https://cdnjs.cloudflare.com/ajax/libs/meyer-reset/2.0/reset.min.css");
        
        * {
            -webkit-font-smoothing: antialiased;
            box-sizing: border-box;
        }
        
        html, body {
            margin: 0px;
            height: 100%;
            display: flex;
            flex-direction: column;
        }
        
        button:focus-visible, a:focus-visible {
            outline: 2px solid #4a90e2 !important;
            outline: -webkit-focus-ring-color auto 5px !important;
        }
        
        a {
            text-decoration: none;
        }

        .landing-page {
            background-color: #ffffff;
            width: 100%;
            min-width: 1440px;
            min-height: 1560px;
            display: flex;
            flex-direction: column;
        }

        .landing-page .header {
            width: 1440px;
            height: 75px;
            display: flex;
            background-color: transparent;
        }

        .landing-page .brainblitz-logo {
            margin-top: 7px;
            width: 312px;
            height: 60px;
            margin-left: 32px;
            aspect-ratio: 5.17;
            object-fit: cover;
        }

        .landing-page .header-nav {
            display: flex;
            align-items: center;
            margin-left: auto;
            gap: 41px;
            margin-top: 17px;
        }

        .landing-page .sign-in-btn {
            display: inline-flex;
            width: 139px;
            height: 40px;
            gap: 10px;
            padding: 9px 38px;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            position: relative;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            cursor: pointer;
            transition: opacity 0.2s ease;
            border: none;
        }

        .landing-page .sign-in-btn:hover,
        .landing-page .sign-in-btn:focus {
            opacity: 0.9;
        }

        .landing-page .text-wrapper {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: fit-content;
            margin-top: -1.00px;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
        }

        .landing-page .get-started-btn {
            display: inline-flex;
            width: 140px;
            height: 40px;
            gap: 10px;
            padding: 9px 19px;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            position: relative;
            align-items: center;
            justify-content: center;
            text-decoration: none;
            cursor: pointer;
            border: none;
            transition: opacity 0.2s ease;
        }

        .landing-page .get-started-btn:hover,
        .landing-page .get-started-btn:focus {
            opacity: 0.9;
        }

        .landing-page .body {
            width: 1440px;
            height: 1485px;
            display: flex;
            flex-direction: column;
            background-color: #f2f2f2;
        }

        .landing-page .slogal-div {
            margin-left: 296px;
            width: 848px;
            height: 477px;
            position: relative;
        }

        .landing-page .div-wrapper {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            padding: 14px 56px;
            position: absolute;
            top: 370px;
            left: 567px;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            text-decoration: none;
            cursor: pointer;
            transition: background-color 0.2s ease;
        }

        .landing-page .div-wrapper:hover,
        .landing-page .div-wrapper:focus {
            background-color: #f5f5f5;
        }

        .landing-page .div {
            display: flex;
            width: fit-content;
            margin-top: -1.00px;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #000000;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
            position: relative;
            align-items: center;
            justify-content: center;
        }

        .landing-page .get-started-btn-2 {
            display: inline-flex;
            align-items: center;
            gap: 7px;
            padding: 14px 20px;
            position: absolute;
            top: 370px;
            left: 96px;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            text-decoration: none;
            cursor: pointer;
            transition: opacity 0.2s ease;
        }

        .landing-page .get-started-btn-2:hover,
        .landing-page .get-started-btn-2:focus {
            opacity: 0.9;
        }

        .landing-page .arrow-right {
            position: relative;
            width: 20px;
            height: 20px;
            aspect-ratio: 1;
        }

        .landing-page .p {
            position: absolute;
            top: 248px;
            left: 96px;
            width: 656px;
            height: 54px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #8d97aa;
            font-size: 24px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
        }

        .landing-page .info-div {
            display: flex;
            margin-left: 6px;
            width: 1428px;
            height: 406px;
            position: relative;
            align-items: center;
            justify-content: center;
            gap: 30px;
            padding: 75px 0px;
        }

        .landing-page .interactive-quizzes {
            flex-direction: column;
            width: 400px;
            height: 256px;
            justify-content: flex-end;
            gap: 20px;
            padding: 20px 56px;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            display: flex;
            align-items: center;
            position: relative;
        }

        .landing-page .image {
            width: 87px;
            height: 87px;
            position: relative;
            aspect-ratio: 1;
            object-fit: cover;
        }

        .landing-page .text-wrapper-2 {
            justify-content: center;
            width: fit-content;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #000000;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
            display: flex;
            align-items: center;
            position: relative;
        }

        .landing-page .text-wrapper-3 {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 284px;
            font-family: "Sansation-Regular", Helvetica;
            font-weight: 400;
            color: #000000;
            font-size: 14px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
        }

        .landing-page .rich-resources {
            display: flex;
            flex-direction: column;
            width: 400px;
            height: 256px;
            align-items: center;
            justify-content: flex-end;
            gap: 20px;
            padding: 20px 73px;
            position: relative;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
        }

        .landing-page .img {
            position: relative;
            width: 89px;
            height: 89px;
            aspect-ratio: 1;
            object-fit: cover;
        }

        .landing-page .text-wrapper-4 {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 284px;
            margin-left: -15.00px;
            margin-right: -15.00px;
            font-family: "Sansation-Regular", Helvetica;
            font-weight: 400;
            color: #000000;
            font-size: 14px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
        }

        .landing-page .interactive-quizzes-2 {
            flex-direction: column;
            width: 400px;
            height: 256px;
            justify-content: flex-end;
            gap: 20px;
            padding: 20px 75px;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
            display: flex;
            align-items: center;
            position: relative;
        }

        .landing-page .text-wrapper-5 {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 284px;
            margin-left: -17.00px;
            margin-right: -17.00px;
            font-family: "Sansation-Regular", Helvetica;
            font-weight: 400;
            color: #000000;
            font-size: 14px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
        }

        .landing-page .user-div {
            display: flex;
            margin-left: 6px;
            width: 1428px;
            height: 528px;
            position: relative;
            margin-top: 13px;
            flex-direction: column;
            align-items: center;
            gap: 30px;
            padding: 75px 0px;
        }

        .landing-page .built-for-everyone {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: fit-content;
            margin-top: -1.00px;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #000000;
            font-size: 36px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
        }

        .landing-page .text-wrapper-6 {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            width: fit-content;
            font-family: "Sansation-Regular", Helvetica;
            font-weight: 400;
            color: #000000;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
        }

        .landing-page .userinfo-div {
            display: inline-flex;
            align-items: flex-start;
            gap: 30px;
            position: relative;
            flex: 0 0 auto;
        }

        .landing-page .div-2 {
            display: flex;
            flex-direction: column;
            width: 400px;
            height: 256px;
            align-items: flex-start;
            justify-content: center;
            gap: 20px;
            padding: 20px 30px;
            position: relative;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
        }

        .landing-page .img-2 {
            position: relative;
            width: 50px;
            height: 46px;
            aspect-ratio: 1.09;
            object-fit: cover;
        }

        .landing-page .text-wrapper-7 {
            position: relative;
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            justify-content: center;
            width: 284px;
            font-family: "Sansation-Regular", Helvetica;
            font-weight: 400;
            color: #000000;
            font-size: 14px;
            letter-spacing: 0;
            line-height: normal;
            list-style: none;
            padding: 0;
            margin: 0;
            gap: 4px;
        }

        .landing-page .text-wrapper-7 li {
            position: relative;
            padding-left: 0;
        }

        .landing-page .text-wrapper-7 li::before {
            content: "-";
            position: absolute;
            left: -12px;
        }

        .landing-page .image-2 {
            width: 50px;
            height: 50px;
            position: relative;
            aspect-ratio: 1;
            object-fit: cover;
        }

        .landing-page .visually-hidden {
            position: absolute;
            width: 1px;
            height: 1px;
            padding: 0;
            margin: -1px;
            overflow: hidden;
            clip: rect(0, 0, 0, 0);
            white-space: nowrap;
            border-width: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="landing-page">
            <header class="header">
                <img class="brainblitz-logo" src="img/brainblitz-logo.png" alt="BrainBlitz Logo" />
                <nav class="header-nav" role="navigation" aria-label="Main navigation">
                    <asp:HyperLink ID="hlSignIn" runat="server" NavigateUrl="#signin" CssClass="sign-in-btn" role="button" aria-label="Sign in to your account">
                        <span class="text-wrapper">Sign In</span>
                    </asp:HyperLink>
                    <asp:HyperLink ID="hlGetStarted" runat="server" NavigateUrl="#getstarted" CssClass="get-started-btn" role="button" aria-label="Get started for free">
                        <span class="text-wrapper">Get Started</span>
                    </asp:HyperLink>
                </nav>
            </header>
            <main class="body">
                <section class="slogal-div" aria-labelledby="hero-heading">
                    <h1 id="hero-heading" class="visually-hidden">Learn Smarter with BrainBlitz</h1>
                    <asp:HyperLink ID="hlSignIn2" runat="server" NavigateUrl="#signin" CssClass="div-wrapper" role="button" aria-label="Sign in to your account">
                        <span class="div">Sign In</span>
                    </asp:HyperLink>
                    <asp:HyperLink ID="hlGetStartedFree" runat="server" NavigateUrl="#getstarted" CssClass="get-started-btn-2" role="button" aria-label="Get started for free">
                        <span class="text-wrapper">Get Started Free</span>
                        <img class="arrow-right" src="img/arrow-right.svg" alt="" aria-hidden="true" />
                    </asp:HyperLink>
                    <p class="p">
                        Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey.
                    </p>
                </section>
                <section class="info-div" aria-labelledby="features-heading">
                    <h2 id="features-heading" class="visually-hidden">Key Features</h2>
                    <article class="interactive-quizzes">
                        <img class="image" src="img/image-1.png" alt="Interactive quizzes icon" />
                        <h3 class="text-wrapper-2">Interactive Quizzes</h3>
                        <p class="text-wrapper-3">
                            Engage with animated quizzes, track your progress, and improve with unlimited attempts.
                        </p>
                    </article>
                    <article class="rich-resources">
                        <img class="img" src="img/image-2.png" alt="Rich resources icon" />
                        <h3 class="text-wrapper-2">Rich Resources</h3>
                        <p class="text-wrapper-4">
                            Access a comprehensive library of learning materials, bookmark favorites, and track completion.
                        </p>
                    </article>
                    <article class="interactive-quizzes-2">
                        <img class="img" src="img/image-3.png" alt="Track progress icon" />
                        <h3 class="text-wrapper-2">Track Progress</h3>
                        <p class="text-wrapper-5">
                            Monitor your learning journey with detailed analytics and performance insights.
                        </p>
                    </article>
                </section>
                <section class="user-div" aria-labelledby="users-heading">
                    <h2 id="users-heading" class="built-for-everyone">Built For&nbsp;&nbsp;Everyone</h2>
                    <p class="text-wrapper-6">Tailored experiences for students, teachers, and administrators</p>
                    <div class="userinfo-div">
                        <article class="div-2">
                            <img class="img-2" src="img/students.png" alt="Students icon" />
                            <h3 class="text-wrapper-2">Students</h3>
                            <ul class="text-wrapper-7">
                                <li>Take interactive quizzes</li>
                                <li>Access learning resources</li>
                                <li>Track your progress</li>
                                <li>Join discussions</li>
                            </ul>
                        </article>
                        <article class="div-2">
                            <img class="img-2" src="img/teachers.png" alt="Teachers icon" />
                            <h3 class="text-wrapper-2">Teachers</h3>
                            <ul class="text-wrapper-7">
                                <li>Create and manage quizzes</li>
                                <li>Upload learning materials</li>
                                <li>Review student performance</li>
                                <li>Subject-based access</li>
                            </ul>
                        </article>
                        <article class="div-2">
                            <img class="image-2" src="img/image-4.png" alt="Admin icon" />
                            <h3 class="text-wrapper-2">Admin</h3>
                            <ul class="text-wrapper-7">
                                <li>Manage users and roles</li>
                                <li>Assign subjects to teachers</li>
                                <li>View analytics dashboard</li>
                                <li>System-wide control</li>
                            </ul>
                        </article>
                    </div>
                </section>
            </main>
        </div>
    </form>
</body>
</html>