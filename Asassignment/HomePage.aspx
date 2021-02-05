<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="Asassignment.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <fieldset>
    <legend>HomePage</legend>

    <br />
    Email: 
    <asp:Label ID="lbl_userID" runat="server" Text="Email : "></asp:Label>
    <br />
    <br />
    Card Number:
    <asp:Label ID="lbl_cnum" runat="server" Text="Card Number : "></asp:Label>    
    <br />
    <br />
    Card Expiry:
    <asp:Label ID="lbl_exdate" runat="server" Text="Card Expiry :"></asp:Label>
    <br />
    <br />
    Card CVC Number:
    <asp:Label ID="lbl_cvc" runat="server" Text="CVC : "></asp:Label>
    <br />
    <br />
    <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="LogoutMe" Visible="false" />
    <br />
    <br />
    <asp:Button ID="btnChangePwd" runat="server" Text="PwdChange" OnClick="ChangePwd" Visible="true" />
    
    <p />


    </fieldset>
    </div>
    </form>
</body>
</html>

