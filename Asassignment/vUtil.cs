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
    public class vUtil
    {
        public vUtil() { }


        private int checkPwdScore(string password)
        {
            int score = 0;
            if (password.Length < 8) return 1;
            else score = 1;
            if (Regex.IsMatch(password, "[a-z]"))  score++;
            if (Regex.IsMatch(password, "[A-Z]"))  score++;
            if (Regex.IsMatch(password, "[0-9]"))  score++;
            if (Regex.IsMatch(password, "[^A-Za-z0-9]")) score++;
            return score;
        }
        public String checkPwdStrength(string userInput)
        {
            String retstr = String.Empty;
            int scores = checkPwdScore(userInput);
            string status = "";
            switch (scores)
            {
                case 1: status = "very weak password, please try again";  break;
                case 2: status = "weak password, please try again";  break;
                case 3: status = "medium strength, please try again"; break;
                case 4:  status = "fairy strong password, please try again";  break;
                case 5:   status = "good"; break;
                default:  break;
            }
            retstr = status;
            if (scores < 4) retstr += ";false";
            else retstr += ";true";
            return retstr;
        }
    }
}