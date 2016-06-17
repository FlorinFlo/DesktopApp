using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchWebService1
{
    public class Credentials
    {
        private string clientID;
        private string clientPass;

        public Credentials(string clientID, string clientPass)
        {
            this.clientID = clientID;
            this.clientPass = clientPass;
        }
        public Credentials()
        {
            
        }
        public string ClientID { get; set; }
        public string ClientPass { get; set; }

    }
}