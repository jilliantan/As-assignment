using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Asassignment
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AsAssConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        bool allOk = true;
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

        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }

        private bool checkExDate(string exdate)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dt = DateTime.ParseExact(exdate, "MM/yy", provider);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private int checkCvc(string cvc)
        {
            int point = 0;
            if (cvc.Length != 3)
            {
                return 1;
            }
            else
            {
                point = 1;
            }
            if (Regex.IsMatch(cvc, "[0-9][0-9][0-9]"))
            {
                point += 1;
            }
            return point;
        }

        private bool checkCardNum(string cardnum)
        {

            if (cardnum.Length != 16)
            {
                return false;
            }
            try
            {
                Convert.ToInt64(cardnum);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool checkEmail(string email)
        {

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool checkDob(string dob)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dt = DateTime.ParseExact(dob, "dd/MM/yyyy", provider);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsAccountExist()
        {
            bool exists = false;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE email=@INPUTEMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@INPUTEMAIL", tb_email.Text);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())  {  if (reader["email"] != DBNull.Value) exists = true;  }
                }
            }
            catch  {}
            finally
            {
                connection.Close();
            }
            return exists;
        }
        public bool createAccount()
        {
            bool ret = false;
           
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@fname,@lname,@passwordHash,@passwordSalt,@cnum,@cexdate,@ccvc,@email,@dob,@IV,@Key,@pwage,@pw1,@pw2,@pwptr,@attempt,@attemptdt,@lockout)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@fname", tb_fname.Text.Trim());
                            cmd.Parameters.AddWithValue("@lname", tb_lname.Text.Trim());
                            cmd.Parameters.AddWithValue("@passwordHash", finalHash);
                            cmd.Parameters.AddWithValue("@passwordSalt", salt);
                            cmd.Parameters.AddWithValue("@cnum", Convert.ToBase64String(encryptData(tb_cdnum.Text.Trim())));
                            cmd.Parameters.AddWithValue("@cexdate", Convert.ToBase64String(encryptData(tb_exdate.Text.Trim())));
                            cmd.Parameters.AddWithValue("@ccvc", Convert.ToBase64String(encryptData(tb_cvc.Text.Trim())));
                            cmd.Parameters.AddWithValue("@email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@dob", DateTime.ParseExact(tb_dob.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));

                            cmd.Parameters.AddWithValue("@pwage", DateTime.Now);
                            cmd.Parameters.AddWithValue("@pw1", finalHash);
                            cmd.Parameters.AddWithValue("@pw2", finalHash);
                            cmd.Parameters.AddWithValue("@pwptr", 1);
                            cmd.Parameters.AddWithValue("@attempt",0);
                            cmd.Parameters.AddWithValue("@attemptdt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@lockout", 0);


                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            ret = true;
                        }
                    }
                }
                return ret;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }


       
        protected void btn_Submit_Click(object sender, EventArgs e)
        {

            bool exd = checkExDate(tb_exdate.Text);
            if (!exd)
            {
                allOk = false;
                exdatechecker.Text = "Incorrect date format";
                exdatechecker.ForeColor = Color.Red;
            }
            else
            {
                exdatechecker.Text = "";
            }

            int points = checkCvc(tb_cvc.Text);
            if (points < 2)
            {
                allOk = false;
                cvcchecker.Text = "cvc number invalid!";
                cvcchecker.ForeColor = Color.Red;
            }
            else
            {
                cvcchecker.Text = "";
            }

            bool num = checkCardNum(tb_cdnum.Text);
            if (!num)
            {
                allOk = false;
                cdnumchecker.Text = "invalid card number!";
                cdnumchecker.ForeColor = Color.Red;
            }
            else
            {
                cdnumchecker.Text = "";
            }

            bool eml = checkEmail(tb_email.Text);
            if (!eml)
            {
                allOk = false;
                mailchecker.Text = "invalid email!";
                mailchecker.ForeColor = Color.Red;
            }
            else
            {
                mailchecker.Text = "";
            }


            bool dd = checkDob(tb_dob.Text);
            if (!dd)
            {
                allOk = false;
                dobchecker.Text = "invalid dob";
                dobchecker.ForeColor = Color.Red;
            }
            else
            {
                dobchecker.Text = "";
            }

            int scores = checkPassword(tb_pwd.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "very weak password, please try again";
                    break;
                case 2:
                    status = "weak password, please try again";
                    break;
                case 3:
                    status = "medium strength, please try again";
                    break;
                case 4:
                    status = "fairy strong password, please try again";
                    break;
                case 5:
                    status = "";
                    break;
                default:
                    break;
            }
            pwdchecker.Text = status;
            if (scores < 4)
            {
                allOk = false;
                pwdchecker.ForeColor = Color.Red;
                return;
            }
            pwdchecker.ForeColor = Color.Green;

            if (allOk)
            {
                if (ValidateCaptcha())
                {
                    string pwd = tb_pwd.Text.ToString().Trim(); ;
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                    byte[] saltByte = new byte[8];
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + salt;

                    // byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    if (IsAccountExist()) lblMessage.Text = "account already exists, please retry";
                    else
                    {
                        if (createAccount()) lblMessage.Text = "account successfully created!";
                        else lblMessage.Text = "create account fail";
                    }
                }
            }
        }
    }
}