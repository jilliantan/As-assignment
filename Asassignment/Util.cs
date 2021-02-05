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
    public class Util
    {
        public Util() { }

        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AsAssConnection"].ConnectionString;

        // eg passwordHash, passwordSalt [for strings only
        public string getDBItemString(string userid, String dbItem)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select " + dbItem + " FROM Account WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[dbItem] != null) { if (reader[dbItem] != DBNull.Value) h = reader[dbItem].ToString(); }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { connection.Close(); }
            return h;
        } // getDBItemString

        // eg attempt
        public int getDBItemInt(String userid, String dbItem)
        {
            int ii = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select " + dbItem + " FROM ACCOUNT WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader[dbItem] != null)
                        {
                            if (reader[dbItem] != DBNull.Value) { ii = Convert.ToInt32(reader[dbItem]); }
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { connection.Close(); }
            return ii;
        } // getDBItemInt



        // eg lockout
        public bool getDBItemBool(String userid, String dbItem)
        {
            bool ret = false;
            int ii = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select " + dbItem + " FROM ACCOUNT WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())  {
                        if (reader[dbItem] != null)  { 
                            if (reader[dbItem] != DBNull.Value)  {  ii = Convert.ToInt32(reader[dbItem]);   if (ii == 1) ret = true; }
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { connection.Close(); }
            return ret;
        } // getDBItemBool

        // eg pwage, attempdt
        public DateTime getDBItemDate(String userid, String dbItem)
        {
            DateTime ret = DateTime.Now;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select " + dbItem + " FROM ACCOUNT WHERE email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) {
                        if (reader[dbItem] != null)  {
                            if (reader[dbItem] != DBNull.Value)  {  ret = Convert.ToDateTime(reader[dbItem]);  }
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { connection.Close(); }
            return ret;
        } // getDBItemDate



        public int UpdateAttempts(String userid, int att, int llock)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET attempt=@newattempt, attemptdt=@newattemptdt, lockout=@newlockout WHERE email=@USERID"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@USERID", userid);
                        cmd.Parameters.AddWithValue("@newattempt", att);
                        cmd.Parameters.AddWithValue("@newattemptdt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@newlockout", llock);

                        cmd.Connection = con;
                        con.Open();
                        ret = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return ret;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        } // UpdateAttempts


        public int UpdateDateTime(String userid)
        {
            int ret = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET pwage=@newpwage WHERE email=@USERID"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@USERID", userid);
                        cmd.Parameters.AddWithValue("@newpwage", DateTime.Now);

                        cmd.Connection = con;
                        con.Open();
                        ret = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return ret;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        } // UpdateAttempts

        public int UpdatePaswords(String userid, String pwdnew)
        {
            int ret = 0;
            SHA512Managed hashing = new SHA512Managed();
            string dbSalt = getDBItemString(userid, "passwordSalt");
            string pwd = pwdnew;
            string pwdWithSalt = pwd + dbSalt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            string pwdnewHash = Convert.ToBase64String(hashWithSalt);

            string pw1 = getDBItemString(userid, "passwordHash");
            string pw2 = getDBItemString(userid, "pw1");

            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET passwordHash=@PWDNEW, pw1=@PWD1, pw2=@PWD2, pwage=@PWAGE WHERE email=@USERID"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@USERID", userid);
                        cmd.Parameters.AddWithValue("@PWDNEW", pwdnewHash);
                        cmd.Parameters.AddWithValue("@PWD1", pw1);
                        cmd.Parameters.AddWithValue("@PWD2", pw2);
                        cmd.Parameters.AddWithValue("@PWAGE", DateTime.Now);

                        cmd.Connection = con;
                        con.Open();
                        ret = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return ret;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        } // UpdatePasswords

        public String validatePassword(String userid, String userInput)
        {
            if ((userInput.Length == 0) || (userInput == null))  { return "Invalid user input;false";  }//to validate if current password is null
            String retstr = String.Empty;
            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBItemString (userid, "passwordHash");
            string dbSalt = getDBItemString(userid, "passwordSalt");
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string pwd = userInput;
                    string pwdWithSalt = pwd + dbSalt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    string userHash = Convert.ToBase64String(hashWithSalt);
                    if (userHash.Equals(dbHash))
                    {
                        retstr = "Password matched, good;true";
                    }
                    else
                    {
                        retstr = "Password is incorrect;false";
                    }
                }
                else
                {
                    retstr = "User not found;false";
                }

            }
            catch (Exception ex) { retstr = "dbException;false";  throw new Exception(ex.ToString()); }
            finally { }
            return retstr;
        } // validatePwd



    }
}