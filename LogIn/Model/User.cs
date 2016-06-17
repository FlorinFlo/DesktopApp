using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogIn.Model
{
    class User
    {
        
        private string userName;
        private string userPass;

        public User(string userName,string userPass)
        {
            
            this.userName = userName;
            this.userPass = userPass;
        }

        public User()
        {
            // TODO: Complete member initialization
        }
       

        
        public string UserName{
            get { return userName; }
            set { userName= value; }
        }

        public string UserPass {
            get { return userPass; }
            set { userPass = value; }
        }
    }
}
