using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrinchBook
{
    public partial class index : System.Web.UI.Page
    {
        private static Service service = Service.Instance;

        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!this.Page.IsPostBack)
            {
                InitDropDownArea();
            }

        }

        protected void InitDropDownArea()
        {

            service.getLibraries(listArea);

        }

        protected void listArea_SelectedIndexChanged(object sender, EventArgs e)
        {

            InitLibraryForArea(listArea.SelectedValue);
            passwordUserText.Attributes["type"] = "password";

        }
        protected void libraryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (service.checkIfSubscriptionRequired(libraryList.SelectedValue))
            {
                signinPopUp.Show();
                
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            ServiceReference2.User user = null;
            user = service.LogInWithCredentials(userNameText.Text, passwordUserText.Text, libraryList.SelectedValue);
            if (user != null)
            {

                Session["user"] = userNameText.Text;                
                Session["email"] = user.UserEmail;               
                Session["birth"] = user.UserBirth;
                Session["library"] = libraryList.SelectedItem;
                Session["libraryID"] = libraryList.SelectedValue;
                Response.Redirect("Default.aspx");

            }
            else
            {                
                signinPopUp.Show();
                errorValidation.Attributes["style"] = "visibility:visible";
            }
        }

        protected void InitLibraryForArea(string selecteArea)
        {
            libraryList.Visible = true;
            lblLibrary.Visible = true;
            service.getLibrariesForArea(selecteArea, libraryList);
        }

        protected void btnCancel1_Click(object sender, EventArgs e)
        {
            errorValidation.Attributes["style"] = "visibility:hidden";
        }


    }
}