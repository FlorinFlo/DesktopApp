using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinchBook
{
    public class Library 
    {
        private long libId;
        private string name;
        private string area;
        private bool reqProfile;
        

        public Library(string name, string area,bool reqProfile)
        {
            this.name = name;
            this.area = area;
            this.reqProfile = reqProfile;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Area
        {
            get { return area; }
            set { area = value; }
        }
        public bool ReqProfile
        {
            get { return reqProfile; }
            set { reqProfile = value; }
        }

    }
}