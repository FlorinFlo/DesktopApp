using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchWebService1
{
    public class User
    {
        
        private string sms;
        private string email;
        private string birthDate;

        public User(string userSms, string userEmail, string userBirth)
        {
            
            this.sms = userSms;
            this.email = userEmail;
            this.birthDate = userBirth;
        }

        public User()
        {
            // TODO: Complete member initialization
        }
        public User(string userName, string userPass)
        {
            this.sms = "0";
            this.email = userName;
            this.birthDate = userPass;
        }

        public string UserSMS
        {
            get { return sms; }
            set { sms = value; }
        }
        public string UserEmail
        {
            get { return email; }
            set { email = value; }
        }

        public string UserBirth
        {
            get { return birthDate; }
            set { birthDate = value; }
        }

        
    }
}