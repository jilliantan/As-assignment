<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Asassignment.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <title>Login</title>
     <script src=": https://www.google.com/recaptcha/api.js"></script>
     <script src="https://www.google.com/recaptcha/api.js?render="></script>
     <script>
         function pwdShow() {
             var x = document.getElementById("cb_pwd");
             if (x.type === "password") {
                 x.type = "text";
             } else {
                 x.type = "password";
             }
         }
     </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <h3>Login</h3>
            <br />
            <br />
            Email:
            <asp:TextBox ID="tb_userid" runat="server" Height="22px"></asp:TextBox>
            <br />
            <br />
            Password:
            <asp:TextBox ID="tb_pwd" runat="server" Height="22px" TextMode="Password" ></asp:TextBox>
            &nbsp;
            <br />
            <br />
            <input type="checkbox" onchange="document.getElementById('tb_pwd').type = this.checked ? 'text' : 'password'" />Show password
            <p><asp:Button ID="btnsubmit" runat="server" Text="Login" OnClick="LoginMe"/>
                <asp:Label ID="lbl_captcha" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Button ID="register" runat="server" OnClick="register_Click" Text="Register" />
            </p>
    
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <asp:Label ID="msg1" runat="server" ></asp:Label>
             <br />
            <asp:Label ID="msg2" runat="server"></asp:Label>
            <br />
            <asp:Label ID="lbl_gScore" runat="server" Visible="False"></asp:Label>
    
            <br />
    
        </div>
      
    </form>
     <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
     </script>
</body>
</html>
