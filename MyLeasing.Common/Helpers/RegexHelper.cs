using System;
using System.Net.Mail;

namespace MyLeasing.Common.Helpers
{
    public class RegexHelper
    {
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress mail = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

    }
}
