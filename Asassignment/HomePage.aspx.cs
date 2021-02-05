using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Asassignment
{
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AsAssConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] cnum = null;
        byte[] ccvc = null;
        byte[] cexdate = null;

        string wKey = String.Empty;
        string wIV = String.Empty;
        string wcnum = String.Empty;
        string wccvc = String.Empty;
        string wcexdate = String.Empty;

        string userID = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)

            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    //lblMessage.Text = "Congratulations !, you are logged in.";
                    //lblMessage.ForeColor = System.Drawing.Color.Green;
                    btnLogout.Visible = true;
                    btnChangePwd.Visible = true;
                    userID = Session["UserID"].ToString();
                    displayUserProfile(userID);
                    
                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }

        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected void displayUserProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE email=@userId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader["email"] != DBNull.Value) lbl_userID.Text = reader["email"].ToString();
                        if (reader["cnum"] != DBNull.Value) wcnum = reader["cnum"].ToString();
                        if (reader["cexdate"] != DBNull.Value) wcexdate = reader["cexdate"].ToString();
                        if (reader["ccvc"] != DBNull.Value) wccvc = reader["ccvc"].ToString();
                        if (reader["IV"] != DBNull.Value) wIV = reader["IV"].ToString();
                        if (reader["Key"] != DBNull.Value) wKey = reader["Key"].ToString();
                    }
                    else return;

                    try
                    {
                        IV = Convert.FromBase64String(wIV);
                        Key = Convert.FromBase64String(wKey);

                        cnum = Convert.FromBase64String(wcnum);
                        ccvc = Convert.FromBase64String(wccvc);
                        cexdate = Convert.FromBase64String(wcexdate);
                    }
                    catch (Exception ex) { throw new Exception(ex.ToString()); }

                    lbl_cnum.Text = decryptData(cnum);
                    lbl_exdate.Text = decryptData(cexdate);
                    lbl_cvc.Text = decryptData(ccvc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected void LogoutMe(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.Net_SessionId"].Value = string.Empty;
                Response.Cookies["Asp.Net_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void ChangePwd(object sender, EventArgs e)
        {
            Response.Redirect("ChangePwd.aspx", false);
        }
    }
}