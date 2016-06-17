using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrinchWebService1
{
    public class Library
    {
        private long libId;
        private string name;
        private string area;
        private bool reqProfile;


        public Library(string name, string area, bool reqProfile)
        {
            this.name = name;
            this.area = area;
            this.reqProfile = reqProfile;
        }

        public Library()
        {
            // TODO: Complete member initialization
        }
        public long ID {
            get { return libId; }
            set { libId = value; }
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
