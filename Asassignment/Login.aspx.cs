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
    public partial class Login : System.Web.UI.Page
    {
        int UNLOCKDELAY = 2;
        int attempts = 0;
        int iattempt = 0;
        Util uu = new Util();
    
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(" https://www.google.com/recaptcha/api/siteverify?secret=&response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        lbl_gScore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }

        }


        protected void LoginMe(object sender, EventArgs e)
        {
            if (!ValidateCaptcha())
            {
                //lbl_captcha.ForeColor = Color.Red;
                //lbl_captcha.Text = "Please validate all fields";
                return;
            }
           attempts = Convert.ToInt32(ViewState["attempts"]);
            msg1.ForeColor = Color.Red;
            msg2.ForeColor = Color.Red;
            string pwd = tb_pwd.Text.ToString().Trim();
            string userid = tb_userid.Text.ToString().Trim();

            if (attempts >= 3)//denial of service
            {
                msg1.Text = "Acct Locked, Attempts=" + attempts.ToString();
                return;
            }

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = uu.getDBItemString(userid, "passwordHash");
            string dbSalt = uu.getDBItemString(userid, "passwordSalt");
           
            try
            {

                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {

                    if (uu.getDBItemBool(userid, "lockout"))
                    {

                        DateTime dd2 = DateTime.Now;
                        DateTime dd1 = uu.getDBItemDate(userid, "attemptdt");
                        TimeSpan ts = dd2.Subtract(dd1);
                        double ttlmin = ts.TotalMinutes;
                        Int64 jj = Convert.ToInt64(ttlmin);

                        if (jj < UNLOCKDELAY)
                        {
                            msg1.Text = "account is locked, try again in min =" + (UNLOCKDELAY - jj).ToString(); return;
                        }
                        else
                        {
                            uu.UpdateAttempts(userid, 0, 0);
                            attempts = 0;
                            ViewState["attempts"] = 0;
                            msg1.Text = "account is unlocked, you may now attempt to login again";
                            msg1.ForeColor = Color.Blue; 
                            return;
                        }
                    }


                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {

                        DateTime pwexp1 = DateTime.Now;
                        DateTime pwexp2 = uu.getDBItemDate(userid, "pwage");
                        TimeSpan pwts = pwexp1.Subtract(pwexp2);
                        double ttlmin = pwts.TotalMinutes;
                        Int64 kk = Convert.ToInt64(ttlmin);

                        Session["UserID"] = userid;
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                        uu.UpdateAttempts(userid, 0, 0);//refresh attempts and lockout state
                        if (kk > 15) Response.Redirect("ChangePwd.aspx", false);
                        else Response.Redirect("HomePage.aspx", false);
                    }
                    else
                    {
                        iattempt = uu.getDBItemInt(userid, "attempt");
                        iattempt++;
                        if (uu.UpdateAttempts(userid, iattempt, 0) > 0) msg2.Text = "upd iattempt ok";
                        else msg2.Text = "upd iattempt failed";

                        if (iattempt >= 3)
                        {
                            msg1.Text = "Acct Locked, iAttempts=" + iattempt.ToString();
                            uu.UpdateAttempts(userid, iattempt, 1); // lockout
                        }
                        else
                        {
                            msg1.Text = "Login Error, iAttempts="  + iattempt.ToString();
                        }
                    }

                }
                else
                {
                    msg1.Text = "no such user exists";
                    ViewState["attempts"] = attempts + 1;
                }

            }
            catch (Exception ex) { msg1.Text = ex.ToString(); throw new Exception(ex.ToString()); }
            finally { }
        }

        protected void register_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx", false);
        }
    }
}