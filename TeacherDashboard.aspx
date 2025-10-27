<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherDashboard.aspx.cs" Inherits="BrainBlitz.TeacherDashboard" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Teacher Dashboard - BrainBlitz</title>

    <!-- Fonts and Global CSS -->
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">

    <style>
        /* Dashboard Container */
        .dashboard-container {
            position: relative;
            width: 100%;
            min-height: 1024px;
            background: #F3F3F3;
            margin: 0 auto;
            padding-bottom: 40px;
        }

        /* Buttons */
        .header-home-btn,
        .header-logout-btn {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 9px 38px;
            position: absolute;
            height: 40px;
            top: 17px;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
            color: #fff;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            text-align: center;
        }

        .header-home-btn {
            width: 132px;
            right: 225px;
        }

        .header-logout-btn {
            width: 107px;
            right: 70px;
        }

        .header-home-btn:hover,
        .header-logout-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
        }

        /* Slogan Section */
        .slogan-div {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            align-items: flex-start;
            padding: 5px 50px 20px;
            position: absolute;
            width: 100%;
            height: 95px;
            top: 75px;
        }

        .dashboard-title {
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 36px;
            line-height: 40px;
            color: #000;
        }

        .dashboard-subtitle {
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            line-height: 22px;
            color: #8D97AA;
        }

        /* ===== Summary Section ===== */
        .summary-container {
            display: flex;
            flex-direction: row;
            justify-content: center;
            align-items: center;
            padding: 0px 60px 0px 20px;
            gap: 25px;
            position: absolute;
            width: 100%;
            height: 177px;
            top: 200px;
        }

        .summary-card {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            padding: 30px 56px 20px 40px;
            gap: 20px;
            width: 307px;
            height: 177px;
            background: #FFFFFF;
            border-radius: 10px;
        }

        .summary-title {
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 24px;
            line-height: 27px;
            color: #8D97AA;
            margin: 0;
        }

        .summary-value {
            font-family: 'Sansation';
            font-weight: 400;
            font-size: 36px;
            line-height: 40px;
            color: #000000;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <!-- Dashboard Container -->
        <div class="dashboard-container">

            <!-- Header -->
            <div class="header">
                <!-- BrainBlitz Logo (from global CSS) -->
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>

                <!-- Home Button -->
                <div class="header-home-btn">
                    <span>Home</span>
                </div>

                <!-- Logout Button -->
                <div class="header-logout-btn">
                    <span>Log out</span>
                </div>
            </div>

            <!-- Slogan -->
            <div class="slogan-div">
                <div class="dashboard-title">Teacher Dashboard</div>
                <div class="dashboard-subtitle">Manage your courses and track student progress</div>
            </div>

            <!-- Summary Cards -->
            <div class="summary-container">
                <div class="summary-card">
                    <div class="summary-title">Total Students</div>
                    <asp:Label ID="lblTotalStudents" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-title">Active Quizzes</div>
                    <asp:Label ID="lblActiveQuizzes" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-title">Resources</div>
                    <asp:Label ID="lblResources" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>

                <div class="summary-card">
                    <div class="summary-title">Average Score</div>
                    <asp:Label ID="lblAverageScore" runat="server" CssClass="summary-value" Text="--"></asp:Label>
                </div>
            </div>

        </div>
    </form>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Home button click
            document.querySelector('.header-home-btn').addEventListener('click', function () {
                window.location.href = 'Default.aspx';
            });

            // Logout button click
            document.querySelector('.header-logout-btn').addEventListener('click', function () {
                window.location.href = 'Logout.aspx';
            });
        });
    </script>
</body>
</html>
