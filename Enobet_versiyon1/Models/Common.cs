using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public static class Common
    {
        public static object GetDBNullOrValue<T>(this T val)
        {
            bool isDbNull = true;
            Type t = typeof(T);

            if (Nullable.GetUnderlyingType(t) != null)
                isDbNull = EqualityComparer<T>.Default.Equals(default(T), val);
            else if (t.IsValueType)
                isDbNull = false;
            else
                isDbNull = val == null;

            return isDbNull ? DBNull.Value : (object)val;
        }

        public static void MailGonder(string konu, string strBody, string kime)
        {
            string mailAdres = ConfigurationManager.AppSettings["EMailAdres"];
            string mailSifre = ConfigurationManager.AppSettings["Password"];
            var myMailMessage = new MailMessage(mailAdres, kime, konu, strBody) { IsBodyHtml = true };

            int mailPortNumber = Convert.ToInt32(ConfigurationManager.AppSettings["MailPortNumber"]);
            string mailUrl = ConfigurationManager.AppSettings["MailURL"];
            var mailAuthentication = new NetworkCredential(mailAdres, mailSifre);
            var mailClient = new SmtpClient(mailUrl, mailPortNumber)
            {
                EnableSsl = false,
                UseDefaultCredentials = false,
                Credentials = mailAuthentication
            };
            mailClient.Send(myMailMessage);
        }

        public static void HotmailMailSend()
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.live.com"); //111.120.2.20
                var mail = new MailMessage(); 
                mail.From = new MailAddress("kendimail@hotmail.com");
                mail.To.Add("gonderilecekmail@gmail.com");
                mail.Subject = "Test Mail - 1";
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = "Write some HTML code here";
                mail.Body = htmlBody; //
                SmtpServer.Port = 587; // 25
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("kendimail@hotmail.com", "şifre"); // 
                SmtpServer.EnableSsl = true; // fALSE
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public static string HotmailMailSend2(string GonderilicekMailAdresi,string mailBaslik,string mailAciklamaHtml)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("2.20"); //2.20
                var mail = new MailMessage();
                mail.From = new MailAddress("enobet@");
                mail.To.Add(GonderilicekMailAdresi);//.Add("mert.t2@dzkk.tsk");
                mail.Subject = mailBaslik;//"Test Mail - 1";
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = mailAciklamaHtml;//"Write some HTML code here";
                mail.Body = htmlBody; //
                SmtpServer.Port = 25; // 25
                SmtpServer.UseDefaultCredentials = true;
                //SmtpServer.Credentials = new System.Net.NetworkCredential("kendimail@hotmail.com", "şifre"); // 
                SmtpServer.EnableSsl = false; // fALSE
                SmtpServer.Send(mail);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
                //throw;
            }

        }

        public static string HotmailMailSend3(string GonderilicekMailAdresi, string mailBaslik, string mailAciklamaHtml)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.live.com"); 
                var mail = new MailMessage();
                mail.From = new MailAddress("azizhudaikaratas@hotmail.com");

                /**/
                /*foreach (var address in GonderilicekMailAdresi.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(address);
                }*/

                mail.To.Add(GonderilicekMailAdresi);
                mail.Subject = mailBaslik;
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = mailAciklamaHtml;
                mail.Body = htmlBody; //
                SmtpServer.Port = 587; // 25
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("azizhudaikaratas@hotmail.com", "1994azizz"); // 
                SmtpServer.EnableSsl = true; // fALSE
                SmtpServer.Send(mail);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        [Obsolete]
        public static string GetIpAddress()
        {
            string bilAdi = Dns.GetHostName();
            string ipAdr = Dns.GetHostByName(bilAdi).AddressList[0].ToString();
            return ipAdr;
        }


        public static string EncodeStr(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public static string DecodeStr(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /*public static bool passwordValidates(string pass)
        {
            int count = 0;

            if (8 <= pass.Length && pass.length() <= 32)
            {
                if (pass.matches(".*\\d.*"))
                    count++;
                if (pass.matches(".*[a-z].*"))
                    count++;
                if (pass.matches(".*[A-Z].*"))
                    count++;
                if (pass.matches(".*[*.!@#$%^&(){}[]:"; '<>,.?/~`_+-=|\\].*") )
                  count++;
            }

            return count >= 3;
        }*/

        public enum PasswordScore
        {
            //Blank = 0,
            //VeryWeak = 1,
            //Weak = 2,
            //Medium = 3,
            //Strong = 4,
            //VeryStrong = 5
            Geçersiz = 0,
            ÇokZayıf = 1,
            Zayıf = 2,
            Orta = 3,
            Güçlü = 4,
            ÇokGüçlü = 5
        }
        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Geçersiz;
            if (password.Length < 4)
                return PasswordScore.ÇokZayıf;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }
        //public class PasswordAdvisor
        //{

        //}


        public static bool ValidatePasswordRegular(string password)
        {
            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 15;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        ;
            return isValid;

        }

        public static bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one lower case letter.";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one upper case letter.";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be lesser than 8 or greater than 15 characters.";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one numeric value.";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one special case character.";
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}