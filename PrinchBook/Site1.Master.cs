using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrinchBook
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        //returns to first page and clears session variables
        protected void logout_Click(object sender, EventArgs e)
        {
            Response.Redirect("index.aspx");
            logedInAs.Visible = false;
            logedInAs.Text = "";
            logout.Visible = false;
            Session.Clear();
            
        }
    }
}