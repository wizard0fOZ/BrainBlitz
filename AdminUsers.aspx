<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminUsers.aspx.cs" Inherits="BrainBlitz.AdminUsers" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>User Management - BrainBlitz</title>
    <link href="https://fonts.googleapis.com/css2?family=Sansation:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="Content/Site.css">
<style>
    .header-buttons-wrapper {
        display: flex;
        align-items: center;
        gap: 48px;
    }
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

    /* User Management Styles */
    .content-container {
        padding: 40px 60px;
        max-width: 1440px;
        margin: 0 auto;
    }

    .page-title {
        font-family: 'Sansation', sans-serif;
        font-size: 36px;
        font-weight: 700;
        color: #333;
        margin-bottom: 30px;
    }

    .users-grid-container {
        background: white;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .users-grid {
        width: 100%;
        border-collapse: collapse;
        font-family: 'Sansation', sans-serif;
    }

    .users-grid thead {
        background: linear-gradient(90deg, #610099 0%, #FF00D9 100%);
        color: white;
    }

    .users-grid th {
        padding: 15px;
        text-align: left;
        font-weight: 700;
        font-size: 16px;
    }

    .users-grid td {
        padding: 15px;
        font-size: 14px;
        color: #333;
    }

    .users-grid tr:hover {
        background: #f9f9f9;
    }

    .status-badge {
        display: inline-block;
        padding: 5px 15px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 700;
    }

    .status-active {
        background: #4CAF50;
        color: white;
    }

    .status-inactive {
        background: #f44336;
        color: white;
    }

    .action-btn {
        padding: 6px 15px;
        border-radius: 6px;
        border: none;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
        font-size: 12px;
        cursor: pointer;
        margin-right: 8px;
        transition: all 0.3s ease;
    }

    .btn-deactivate {
        background: #FF9800;
        color: white;
    }

    .btn-activate {
        background: #4CAF50;
        color: white;
    }

    .btn-delete {
        background: #f44336;
        color: white;
    }

    .action-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(0,0,0,0.2);
    }

    .message-box {
        padding: 15px 20px;
        border-radius: 8px;
        margin-bottom: 20px;
        font-family: 'Sansation', sans-serif;
        font-weight: 700;
    }

    .message-success {
        background: #d4edda;
        color: #155724;
        border: 1px solid #c3e6cb;
    }

    .message-error {
        background: #f8d7da;
        color: #721c24;
        border: 1px solid #f5c6cb;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <a href="AdminDashboard.aspx" class="brainblitz"></a>
                <div class="header-buttons-wrapper">
                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="header-logout-btn" OnClick="btnLogout_Click">
                        <span>Log out</span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="content-container">
            <h1 class="page-title">User Management</h1>

            <!-- Message Display -->
            <asp:Panel ID="pnlMessage" runat="server" Visible="false">
                <div class="message-box" id="messageBox" runat="server">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
            </asp:Panel>

            <!-- Users Grid -->
            <div class="users-grid-container">
                <asp:GridView ID="gvUsers" runat="server" 
                    CssClass="users-grid" 
                    AutoGenerateColumns="False" 
                    OnRowCommand="gvUsers_RowCommand"
                    DataKeyNames="userID"
                    EmptyDataText="No users found.">
                    <Columns>
                        <asp:BoundField DataField="userID" HeaderText="ID" />
                        <asp:BoundField DataField="fullName" HeaderText="Full Name" />
                        <asp:BoundField DataField="email" HeaderText="Email" />
                        <asp:BoundField DataField="role" HeaderText="Role" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# (bool)Eval("isActive") ? "status-badge status-active" : "status-badge status-inactive" %>'>
                                    <%# (bool)Eval("isActive") ? "Active" : "Inactive" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="createdAt" HeaderText="Created At" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnToggleStatus" runat="server" 
                                    CssClass='<%# (bool)Eval("isActive") ? "action-btn btn-deactivate" : "action-btn btn-activate" %>'
                                    Text='<%# (bool)Eval("isActive") ? "Deactivate" : "Activate" %>'
                                    CommandName="ToggleStatus" 
                                    CommandArgument='<%# Eval("userID") %>'
                                    OnClientClick="return confirm('Are you sure you want to change this user\'s status?');" />
                                <asp:Button ID="btnDelete" runat="server" 
                                    CssClass="action-btn btn-delete"
                                    Text="Delete" 
                                    CommandName="DeleteUser" 
                                    CommandArgument='<%# Eval("userID") %>'
                                    OnClientClick="return confirm('Are you sure you want to permanently delete this user? This action cannot be undone.');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>