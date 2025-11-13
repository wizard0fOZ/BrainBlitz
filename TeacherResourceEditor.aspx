<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherResourceEditor.aspx.cs" Inherits="BrainBlitz.TeacherResourceEditor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Upload Resource - BrainBlitz</title>

    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="../Content/Site.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <style>
        .header-logout-btn {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 9px 38px;
            position: relative;
            height: 40px;
            top: auto;
            right: auto;
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            border-radius: 10px;
            cursor: pointer;
            color: #fff;
            font-family: 'Sansation';
            font-weight: 700;
            font-size: 20px;
            text-align: center;
            text-decoration: none;
            width: 145px;
        }

        .header-logout-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 16px rgba(97, 0, 153, 0.3);
        }

        body {
            background-color: #F3F3F3;
            font-family: 'Sansation', sans-serif;
        }

        .editor-container {
            width: 100%;
            max-width: 900px;
            margin: 0 auto;
            padding: 30px 50px;
            box-sizing: border-box;
        }

        .page-title {
            font-size: 36px;
            font-weight: 700;
            margin-bottom: 4px;
        }

        .page-subtitle {
            font-size: 18px;
            color: #8D97AA;
            margin-bottom: 20px;
        }

        .editor-card {
            background: #FFFFFF;
            border-radius: 12px;
            padding: 24px 28px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 700;
            margin-bottom: 6px;
        }

        .input-field,
        .textarea-field,
        .dropdown-field {
            width: 100%;
            box-sizing: border-box;
            border-radius: 8px;
            border: 1px solid #E0E0E0;
            padding: 10px 12px;
            font-family: 'Sansation', sans-serif;
            font-size: 14px;
            outline: none;
        }

        .input-field:focus,
        .textarea-field:focus,
        .dropdown-field:focus {
            border-color: #610099;
        }

        .textarea-field {
            min-height: 100px;
            resize: vertical;
        }

        .row-2 {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }

        .upload-box {
            border: 2px dashed #E0E0E0;
            border-radius: 10px;
            padding: 24px;
            text-align: center;
            color: #8D97AA;
            font-size: 14px;
        }

        .upload-box i {
            font-size: 32px;
            margin-bottom: 8px;
            color: #8D97AA;
        }

        .button-row {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 20px;
        }

        .btn-secondary,
        .btn-primary {
            border-radius: 10px;
            padding: 10px 18px;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            border: none;
        }

        .btn-secondary {
            background: #FFFFFF;
            border: 1px solid #E0E0E0;
            color: #333333;
        }

        .btn-primary {
            background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
            color: #FFFFFF;
        }

        .btn-primary:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(97, 0, 153, 0.3);
        }

        .existing-file {
            font-size: 12px;
            color: #757575;
            margin-top: 4px;
        }

        .message {
            display: block;
            margin-bottom: 10px;
        }
        .message.error {
            color: #B00020;
        }
        .message.success {
            color: #008000;
        }

        @media (max-width: 900px) {
            .editor-container {
                padding: 20px;
            }
            .row-2 {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%-- header reused from other teacher pages --%>
        <div class="header">
            <div class="header-content">
                <a href="TeacherDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">Log out</asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="editor-container">
            <h1 class="page-title">
                <asp:Label ID="lblPageTitle" runat="server" Text="Upload Resource"></asp:Label>
            </h1>
            <p class="page-subtitle">
                <asp:Label ID="lblPageSubtitle" runat="server" Text="Add new learning material for students"></asp:Label>
            </p>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" EnableViewState="false"></asp:Label>

            <asp:HiddenField ID="hfResourceId" runat="server" />
            <asp:HiddenField ID="hfExistingPath" runat="server" />

            <div class="editor-card">

                <div class="form-group">
                    <label for="txtTitle" class="form-label">Resource Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="input-field"
                        placeholder="e.g., Calculus Fundamentals Guide"></asp:TextBox>
                </div>

                <div class="form-group row-2">
                    <div>
                        <label for="ddlSubject" class="form-label">Subject</label>
                        <asp:DropDownList ID="ddlSubject" runat="server" CssClass="dropdown-field">
                        </asp:DropDownList>
                    </div>
                    <div>
                        <label for="ddlType" class="form-label">Resource Type</label>
                        <asp:DropDownList ID="ddlType" runat="server" CssClass="dropdown-field">
                            <asp:ListItem Text="PDF" Value="pdf"></asp:ListItem>
                            <asp:ListItem Text="Image" Value="img"></asp:ListItem>
                            <asp:ListItem Text="Video" Value="video"></asp:ListItem>
                            <asp:ListItem Text="Link" Value="link"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-group">
                    <label for="txtDescription" class="form-label">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="textarea-field"
                        TextMode="MultiLine"
                        placeholder="Describe what this resource covers..."></asp:TextBox>
                </div>

                <div class="form-group">
                    <label class="form-label">Upload File</label>
                    <div class="upload-box">
                        <i class="fas fa-upload"></i>
                        <p>Click the button below to upload a file</p>
                        <p style="font-size:12px;">PDF, DOC, PPT, images or video files (reasonable size)</p>
                        <br />
                        <asp:FileUpload ID="fuResource" runat="server" />
                        <asp:Label ID="lblExistingFile" runat="server" CssClass="existing-file"></asp:Label>
                    </div>
                </div>

                <div class="form-group">
                    <label for="txtExternalUrl" class="form-label">Or external URL (optional)</label>
                    <asp:TextBox ID="txtExternalUrl" runat="server" CssClass="input-field"
                        placeholder="https://example.com/resource.pdf"></asp:TextBox>
                    <span class="existing-file">
                        If you upload a file, the file will be used and this URL is optional.
                    </span>
                </div>

                <div class="button-row">
                    <asp:Button ID="btnCancel" runat="server" CssClass="btn-secondary"
                        Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" />
                    <asp:Button ID="btnSave" runat="server" CssClass="btn-primary"
                        Text="Upload Resource" OnClick="btnSave_Click" />
                </div>

            </div>
        </div>

    </form>
</body>
</html>
