using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ParkShark.Domain
{
    public static class Validations
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
