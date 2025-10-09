<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BrainBlitz.Default" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta charset="utf-8" />
    <title>BrainBlitz - Interactive Learning Platform</title>
    <meta name="description" content="Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey." />
    <style>
        @import url("https://cdnjs.cloudflare.com/ajax/libs/meyer-reset/2.0/reset.min.css");
        
        * {
            -webkit-font-smoothing: antialiased;
            box-sizing: border-box;
        }
        
        html,
        body {
            margin: 0px;
            height: 100%;
        }
        
        button:focus-visible {
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
            min-height: 1024px;
            display: flex;
            flex-direction: column;
        }

        .landing-page .header {
            width: 100%;
            max-width: 1440px;
            height: 75px;
            display: flex;
            background-color: transparent;
            align-items: center;
            padding: 0 32px;
            box-sizing: border-box;
        }

        .landing-page .brainblitz-logo {
            width: 312px;
            height: 60px;
            aspect-ratio: 5.17;
            object-fit: cover;
        }

        .landing-page .header-nav {
            display: flex;
            gap: 40px;
            margin-left: auto;
            align-items: center;
        }

        .landing-page .sign-in-btn {
            width: 140px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            text-decoration: none;
            cursor: pointer;
            transition: opacity 0.3s ease;
            border: none;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
        }

        .landing-page .sign-in-btn:hover {
            opacity: 0.9;
        }

        .landing-page .sign-in-btn:focus {
            outline: 2px solid #4a90e2;
            outline-offset: 2px;
        }

        .landing-page .text-wrapper {
            display: flex;
            align-items: center;
            justify-content: center;
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
            width: 140px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            text-decoration: none;
            cursor: pointer;
            transition: opacity 0.3s ease;
            border: none;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
        }

        .landing-page .get-started-btn:hover {
            opacity: 0.9;
        }

        .landing-page .get-started-btn:focus {
            outline: 2px solid #4a90e2;
            outline-offset: 2px;
        }

        .landing-page .div {
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
        }

        .landing-page .body {
            width: 100%;
            max-width: 1440px;
            height: 949px;
            position: relative;
            background-color: #f2f2f2;
        }

        .landing-page .slogal-div {
            position: absolute;
            top: 0;
            left: 200px;
            width: 1040px;
            height: 598px;
            display: flex;
            flex-direction: column;
            gap: 68px;
        }

        .landing-page .p {
            display: flex;
            align-items: center;
            justify-content: center;
            margin-left: 192px;
            width: 656px;
            height: 54px;
            margin-top: 272px;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #8d97aa;
            font-size: 24px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
        }

        .landing-page .home-button {
            margin-left: 192px;
            width: 220px;
            height: 50px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 7px;
            border-radius: 10px;
            overflow: hidden;
            background: linear-gradient(90deg, rgba(97, 0, 153, 1) 0%, rgba(255, 0, 217, 1) 100%);
            text-decoration: none;
            cursor: pointer;
            transition: opacity 0.3s ease;
            border: none;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
        }

        .landing-page .home-button:hover {
            opacity: 0.9;
        }

        .landing-page .home-button:focus {
            outline: 2px solid #4a90e2;
            outline-offset: 2px;
        }

        .landing-page .text-wrapper-2 {
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #ffffff;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
        }

        .landing-page .arrow-right {
            width: 20px;
            height: 20px;
            aspect-ratio: 1;
        }

        .landing-page .div-wrapper {
            position: absolute;
            top: 394px;
            left: 863px;
            width: 175px;
            height: 50px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: #ffffff;
            border-radius: 10px;
            overflow: hidden;
        }

        .landing-page .text-wrapper-3 {
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: "Sansation-Bold", Helvetica;
            font-weight: 700;
            color: #000000;
            font-size: 20px;
            text-align: center;
            letter-spacing: 0;
            line-height: normal;
            white-space: nowrap;
            text-decoration: none;
            cursor: pointer;
            transition: color 0.3s ease;
            border: none;
            background: transparent;
        }

        .landing-page .text-wrapper-3:hover {
            color: #610099;
        }

        .landing-page .text-wrapper-3:focus {
            outline: 2px solid #4a90e2;
            outline-offset: 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="landing-page">
            <header class="header" role="banner">
                <img class="brainblitz-logo" src="img/brainblitz-logo.png" alt="BrainBlitz Logo" />
                <nav class="header-nav" role="navigation" aria-label="Main navigation">
                    <asp:Button ID="btnSignInHeader" runat="server" Text="Sign In" CssClass="sign-in-btn" OnClick="btnSignInHeader_Click" />
                    <asp:Button ID="btnGetStartedHeader" runat="server" Text="Get Started" CssClass="get-started-btn" OnClick="btnGetStartedHeader_Click" />
                </nav>
            </header>
            <main class="body" role="main">
                <section class="slogal-div" aria-labelledby="hero-heading">
                    <p class="p">
                        Interactive quizzes, rich learning resources, and powerful analytics to turbocharge your educational journey.
                    </p>
                    <asp:Button ID="btnGetStartedHero" runat="server" Text="Get Started Free →" CssClass="home-button" OnClick="btnGetStartedHero_Click" />
                </section>
                <aside class="div-wrapper">
                    <asp:Button ID="btnSignInHero" runat="server" Text="Sign In" CssClass="text-wrapper-3" OnClick="btnSignInHero_Click" />
                </aside>
            </main>
        </div>
    </form>
</body>
</html>