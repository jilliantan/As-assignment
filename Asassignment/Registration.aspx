<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Asassignment.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register</title>
     <script src=": https://www.google.com/recaptcha/api.js"></script>
     <script src="https://www.google.com/recaptcha/api.js?render="></script>
     <script type="text/javascript">
         function validate() {
             var str = document.getElementById('<%=tb_pwd.ClientID%>').value;
             if (str.length < 8) {
                 document.getElementById("pwdchecker").innerHTML = "Password Length Must be at least 8 characters";
                 document.getElementById("pwdchecker").style.color = "Red";
                 return ("too_short");
             }
             else if (str.search(/[0-9]/) == -1) {
                 document.getElementById("pwdchecker").innerHTML = "Password require at least 1 number";
                 document.getElementById("pwdchecker").style.color = "Red";
                 return ("no_number");
             }
             else if (str.search(/[A-Z]/) == -1) {
                 document.getElementById("pwdchecker").innerHTML = "Password require at least 1 Upper Case";
                 document.getElementById("pwdchecker").style.color = "Red";
                 return ("no_upper");
             }
             else if (str.search(/[a-z]/) == -1) {
                 document.getElementById("pwdchecker").innerHTML = "Password require at least 1 Lower Case";
                 document.getElementById("pwdchecker").style.color = "Red";
                 return ("no_lower");
             }
             else if (str.search(/[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/) == -1) {
                 document.getElementById("pwdchecker").innerHTML = "Password require at least 1 Special Character";
                 document.getElementById("pwdchecker").style.color = "Red";
                 return ("no_special");
             }
             else {
                 document.getElementById("pwdchecker").innerHTML = " ";
             }
         }

         function cdnumval() {
             var str = document.getElementById('<%=tb_cdnum.ClientID%>').value;
             if (str.length != 16) {
                  
                  document.getElementById("cdnumchecker").innerHTML = "Must be 16 characters";
                  document.getElementById("cdnumchecker").style.color = "Red";
                  return ("too_short");
              }
             else {
                 var ret = true;
                 for (var ii = 0; ii < 16; ii++) {
                     var ss = str.slice(ii, ii + 1);
                     var jj = parseInt(ss);
                     if (isNaN(jj)) {
                         ret = false; break;
                     }
                 }
                 if (ret) {
                     document.getElementById("cdnumchecker").innerHTML = " ";
                 }
                 else {
                     document.getElementById("cdnumchecker").innerHTML = "Must be numbers";
                     document.getElementById("cdnumchecker").style.color = "Red";
                 }
             }
         }

         function exdateErr() {
             document.getElementById("exdatechecker").innerHTML = "Format Error, please see hint";
             document.getElementById("exdatechecker").style.color = "Red";
         }

         function exdateval() {
             var str = document.getElementById('<%=tb_exdate.ClientID%>').value;
             var ret = true;
             if (str.length != 5) { exdateErr(); return; }
             var res = str.split("/");
             if (res.length != 2) { exdateErr(); return; }
             var ii = res[0];
             var jj = res[1];
             if ((ii < 1) || (ii > 12)) { exdateErr(); return; }
             if ((jj < 20) || (jj > 30)) { exdateErr(); return; }
             for (var ii = 0; ii < 2; ii++) {
                 var ss = res[0].slice(ii, ii + 1);
                 var dd = res[1].slice(ii, ii + 1);
                 var jj = parseInt(ss);
                 var kk = parseInt(dd);
                 if (isNaN(jj)) {
                     ret = false; break;
                 }
                 if (isNaN(kk)) {
                     ret = false; break;
                 }
             }
             if (!ret) {exdateErr(); return; }
             document.getElementById("exdatechecker").innerHTML = " ";
         }

         function cvcval() {
            
             var str = document.getElementById('<%=tb_cvc.ClientID%>').value;
             if (str.length != 3) {
                 document.getElementById("cvcchecker").innerHTML = "Must be length of 3";
                 document.getElementById("cvcchecker").style.color = "Red";
             }
             else {
                 var cvcOk = true
                 for (var ii = 0; ii < 3; ii++) {
                     var ss = str.slice(ii, ii + 1);
                     var jj = parseInt(ss);
                     if (isNaN(jj)) {
                         cvcOk = false; break;
                     }
                 }
                 if (cvcOk) {
                     document.getElementById("cvcchecker").innerHTML = " ";
                 }
                 else {
                     document.getElementById("cvcchecker").innerHTML = "Must be numbers";
                     document.getElementById("cvcchecker").style.color = "Red";
                 }
             }
         }

         function mailval() {

             var str = document.getElementById('<%=tb_email.ClientID%>').value;
             var emailformat = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
             if (str.match(emailformat)) {
                 document.getElementById("mailchecker").innerHTML = " ";
             }
             else {
                 document.getElementById("mailchecker").innerHTML = "Wrong email format, follow hint";
                 document.getElementById("mailchecker").style.color = "Red";
             }
         }

         function dobErr() {
             document.getElementById("dobchecker").innerHTML = "Format Error, please follow dd/mm/yyyy format";
             document.getElementById("dobchecker").style.color = "Red";
         }


         function dobval() {
             var str = document.getElementById('<%=tb_dob.ClientID%>').value;
             var ret = true;
             if (str.length != 10) { dobErr(); return; }
             var div = str.split("/");
             var ii = div[0];
             var jj = div[1];
             if (div.length < 3) { dobErr(); return; }
             if (ii.length != 2 && jj.length != 2) { dobErr(); return; }
             if (div[2].length != 4) { dobErr(); return; }
             if ((ii < 1) || (ii > 31)) { dobErr(); return; }
             if ((jj < 1) || (jj > 12)) { dobErr(); return; }
             for (var ii = 0; ii < 2; ii++) {
                 var dd = div[0].slice(ii, ii + 1);
                 var mm = div[1].slice(ii, ii + 1);
                 var ddd = parseInt(dd);
                 var mmm = parseInt(mm);
                 if (isNaN(ddd)) {
                     ret = false; break;
                 }
                 if (isNaN(mmm)) {
                     ret = false; break;
                 }
                
             }
             for (var ii = 0; ii < 4; ii++) {
                 var yy = div[2].slice(ii, ii + 1);
                 var yyy = parseInt(yy);
                 if (isNaN(yyy)) {
                     ret = false; break;
                 }
             }

             if (!ret) { dobErr(); return; }
             document.getElementById("dobchecker").innerHTML = " ";
        }
         

     </script>
   
</head>
<body style="height: 1067px">
   
    <form id="form1" runat="server">
        <div>
            <br />
            <h3>Account Registration</h3>
            <br />
            <br />
            First Name:
            <asp:TextBox ID="tb_fname" runat="server" Height="22px"></asp:TextBox>
            <br />
            <br />
            Last Name:
            <asp:TextBox ID="tb_lname" runat="server" Height="22px"></asp:TextBox>
            <br />
            <br />
            Password:
            <asp:TextBox ID="tb_pwd" runat="server" Height="22px" onkeyup="javascript:validate()" TextMode="Password" ></asp:TextBox>
            <asp:Label ID="pwdchecker" runat="server"></asp:Label>
            <br />
            <br />
            Credit Card Number:
            <asp:TextBox ID="tb_cdnum" runat="server" Height="22px" onkeyup="javascript:cdnumval()"></asp:TextBox>
            <asp:Label ID="cdnumchecker" runat="server"></asp:Label>
            <br />
            <br />
            Credit Card Expiry Date:
            <asp:TextBox ID="tb_exdate" runat="server" Height="22px" onkeyup="javascript:exdateval()" placeholder="MM/YY"></asp:TextBox>
            <asp:Label ID="exdatechecker" runat="server"></asp:Label>
            <br />
            <br />
            CVC Number:
            <asp:TextBox ID="tb_cvc" runat="server" Height="22px" onkeyup="javascript:cvcval()"></asp:TextBox>
            <asp:Label ID="cvcchecker" runat="server"></asp:Label>
            <br />
            <br />
            Email Address:
            <asp:TextBox ID="tb_email" runat="server" Height="22px" onkeyup="javascript:mailval()" placeholder="xxx@mail.com" ></asp:TextBox>
            <asp:Label ID="mailchecker" runat="server"></asp:Label>
            <br />
            <br />
            Date Of Birth
            <asp:TextBox ID="tb_dob" runat="server" Height="22px" onkeyup="javascript:dobval()" placeholder="DD/MM/YYYY"></asp:TextBox>
            <asp:Label ID="dobchecker" runat="server"></asp:Label>
            <br />
            <br />
            <br />
            <asp:Button ID="btn_Submit" runat="server" Height="33px" Text="Submit" OnClick="btn_Submit_Click" />
       
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <asp:Label ID="lblMessage" runat="server" EnableViewState="False"></asp:Label>
            <asp:Label ID="lbl_gScore" runat="server" Visible="False"></asp:Label>
    
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