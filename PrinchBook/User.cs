using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinchBook
{
   public class User
    {
        private string name;       
        private string password;

        public User(string name, string password)
        {
            this.name=name;
            this.password = password;

        }

        public User()
        {
            // TODO: Complete member initialization
        }
        public string Name{
            get { return name; }
            set { name = value; }
        }
        
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
