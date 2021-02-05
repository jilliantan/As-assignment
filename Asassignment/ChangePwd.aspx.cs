using System;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

using System.Drawing;

using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;
using System.Data;

namespace Asassignment
{
    public partial class ChangePwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))  Response.Redirect("Login.aspx", false);
                else
                {
                    // msg1.ForeColor = Color.Blue;
                   //msg2.ForeColor = Color.Red;
                   String ss1 = (string)Session["userID"];
                   //pwderror.Text = ss1 + " has entered the change password page";
                }

            }
            else  Response.Redirect("Login.aspx", false);
        }


        protected String NewPwdExist(string userid, string userInput)
        {
            Util uu = new Util();

            String retstr = String.Empty;
            string newpwd = userInput;
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = uu.getDBItemString(userid, "passwordHash");
            string dbSalt = uu.getDBItemString(userid, "passwordSalt");

            string pw1 = uu.getDBItemString(userid, "pw1");
            string pw2 = uu.getDBItemString(userid, "pw2");
            string userHash = String.Empty;


            try
            {
                if (dbSalt != null && dbSalt.Length > 0)
                {
                    string pwdWithSalt = newpwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    userHash = Convert.ToBase64String(hashWithSalt);
                }
                else
                    return ("dbSalt is null;true");

                if (dbHash != null && dbHash.Length > 0) { if (userHash.Equals(dbHash)) return ("matched original password;true"); }
                if (pw1 != null && pw1.Length > 0) { if (userHash.Equals(pw1)) return ("matched pw1;true"); }
                if (pw2 != null && pw2.Length > 0) { if (userHash.Equals(pw2)) return ("matched pw2;true"); }
            }
            catch (Exception ex) { retstr = "exeption "; throw new Exception(ex.ToString()); }
            finally { }
            retstr += ";false";
            return retstr;
        }



        protected void submit(object sender, EventArgs e)
        {
            Util uu = new Util();
            vUtil vv = new vUtil();
            String[] ss;
            String tmp = String.Empty;
            int ALLOWPWDCHANGE = 5;
            string userid = (string)Session["userID"];
            DateTime pwexp1 = DateTime.Now;
            DateTime pwexp2 = uu.getDBItemDate(userid, "pwage");
            TimeSpan pwts = pwexp1.Subtract(pwexp2);
            double ttlmin = pwts.TotalMinutes;
            Int64 kk = Convert.ToInt64(ttlmin);
            if (kk<ALLOWPWDCHANGE) { pwderror.Text = "You cannot change your password yet, you can change it only in " + (ALLOWPWDCHANGE-kk).ToString() + " minutes time"; pwderror.ForeColor = Color.Red; }
          
          
            if ((userid.Length == 0) || (userid == null))
            {
                msg2.ForeColor = Color.Red;
                msg2.Text = "userid is null";
                return;
            }
            
            tmp =  vv.checkPwdStrength(tb_newpwd.Text);
            ss = tmp.Split(';');
            if (ss[1].Equals("false"))  { msg2.Text = ss[0];  msg2.ForeColor = Color.Red; return; }
            msg2.Text = ss[0];
            msg2.ForeColor = Color.Green;

            tmp = uu.validatePassword(userid, tb_pwd.Text);
            ss = tmp.Split(';');
            if (!ss[1].Equals("true")) { msg1.Text = "incorrect password"; msg1.ForeColor = Color.Red; return; }
            msg1.Text = ss[0];
            msg1.ForeColor = Color.Green;

            tmp = NewPwdExist(userid, tb_newpwd.Text);
            ss = tmp.Split(';');
            if (ss[1].Equals("true")) { 
                msg2.Text = ss[0]; 
                pwderror.Text = "Password already exists, please retry";
                pwderror.ForeColor = Color.Red;
                msg2.ForeColor = Color.Red;
            }
            else
            {
                msg2.Text = ss[0];
                msg2.ForeColor = Color.Green;
                //pwderror.Text = "Password is good.";
                //pwderror.ForeColor = Color.Green;

                // TODO UPDATE NEW PASSWORD INTO DB.
                int ii = uu.UpdatePaswords(userid, tb_newpwd.Text);
                int jj =  uu.UpdateDateTime(userid);
                if (ii == 1 && jj==1) 
                {
                    pwderror.Text = "Password updated successfully!";
                    pwderror.ForeColor = Color.Green;
                }
                else {
                    pwderror.Text = "Password update failed! rows=" + ii.ToString();
                    pwderror.ForeColor = Color.Red;
                }
            }
        }

        protected void btnhmpg_Click(object sender, EventArgs e)
        {
            Response.Redirect("HomePage.aspx", false);
        }
    }
}