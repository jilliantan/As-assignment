<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePwd.aspx.cs" Inherits="Asassignment.ChangePwd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script type="text/javascript">
         function validate() {
             var str = document.getElementById('<%=tb_newpwd.ClientID%>').value;
             if (str.length < 8) {
                 document.getElementById("msg2").innerHTML = "Password Length Must be at least 8 characters";
                 document.getElementById("msg2").style.color = "Red";
                 return ("too_short");
             }
             else if (str.search(/[0-9]/) == -1) {
                 document.getElementById("msg2").innerHTML = "Password require at least 1 number";
                 document.getElementById("msg2").style.color = "Red";
                 return ("no_number");
             }
             else if (str.search(/[A-Z]/) == -1) {
                 document.getElementById("msg2").innerHTML = "Password require at least 1 Upper Case";
                 document.getElementById("msg2").style.color = "Red";
                 return ("no_upper");
             }
             else if (str.search(/[a-z]/) == -1) {
                 document.getElementById("msg2").innerHTML = "Password require at least 1 Lower Case";
                 document.getElementById("msg2").style.color = "Red";
                 return ("no_lower");
             }
             else if (str.search(/[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/) == -1) {
                 document.getElementById("msg2").innerHTML = "Password require at least 1 Special Character";
                 document.getElementById("msg2").style.color = "Red";
                 return ("no_special");
             }
             else {
                 document.getElementById("msg2").innerHTML = " ";
             }
         }
     </script>
    <style type="text/css">
        .auto-style1 {
            margin-left: 23px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <h3>Change Password</h3>
            <br />
            <br />
            Current Password:
            <asp:TextBox ID="tb_pwd" runat="server" Height="22px"></asp:TextBox>
            <asp:Label ID="msg1" runat="server" ></asp:Label>
            <br />
            <br />
            Password:
            <asp:TextBox ID="tb_newpwd" runat="server" Height="22px" onkeyup="javascript:validate()" ></asp:TextBox>
            <asp:Label ID="msg2" runat="server" ></asp:Label>
            <br />
            <br />
            <p>
                <asp:Button ID="btnhmpg" runat="server" OnClick="btnhmpg_Click" Text="Return to Homepage" Height="46px" Width="189px" />
                <asp:Button ID="btnsubmit" runat="server" Text="Change Password" OnClick="submit" Height="46px" Width="189px" CssClass="auto-style1" /></p>
            <br />
            <asp:Label ID="pwderror" runat="server"></asp:Label>
            <br />
             <br />
             <br />
            <br />
        </div>
    </form>
</body>
</html>
