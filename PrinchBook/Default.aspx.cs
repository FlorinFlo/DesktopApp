
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace PrinchBook
{
    public partial class Default : System.Web.UI.Page
    {
        //private static bool area = false;
        //private static bool library = false;
        private static Service service = Service.Instance;
        private static string PC;
        private static string USER_NAME;
        private static string USER_EMAIL;
        private static string USER_BIRTH;
        // private static StringBuilder info = new StringBuilder();
        //private static bool logedIn = false;
        private static string USER_PC;
        private static List<PrinchBook.ServiceReference2.Resource> resourcesAdult;
        private static List<PrinchBook.ServiceReference2.Resource> resourceUnder;
        private static long LIBRARY_ID;
        private static string LIBRARY_NAME;
        private static int START_HOUR = 9;
        private static int END_Hour = 20;
        private static string EMAIL = string.Empty;
        private static List<string> timesList = new List<string>();
        private static Dictionary<ServiceReference2.Resource, List<string>> tableRows = new Dictionary<ServiceReference2.Resource, List<string>>();
        private static DateTime SELECTED_DATE = DateTime.Now;
        private static int selectedDuration = 2;// 30 minutes
        // private static string selectedTime = string.Empty;
        private Button nextButton = null;
        private bool created = false;
        private bool showNext = false;
        private static DateTime startBooking;
        private static DateTime endBooking;
        private static string FUNCTION = String.Empty;
        private static bool ADULT = false;
        private static StringBuilder infoString = new StringBuilder();
        private static ServiceReference2.Resource resource = null;




        protected void Page_Load(object sender, EventArgs e)
        {
           

            
            if (!IsPostBack)
            {
                LIBRARY_NAME = Session["library"].ToString();
                LIBRARY_ID = Convert.ToInt64(Session["libraryID"]);
                USER_NAME = Session["user"].ToString();
                USER_EMAIL = Session["email"].ToString();
                USER_BIRTH = Session["birth"].ToString();
                ADULT = service.isAdult(USER_BIRTH, 12);

                Site1 site = this.Master as Site1;
                Label logedInAs = site.FindControl("logedInAs") as Label;
                Button logout = site.FindControl("logout") as Button;

                logedInAs.Text = "Loged in at " + LIBRARY_NAME;
                logedInAs.Visible = true;
                logout.Visible = true;
            }
            //if (sender == calendar)
            //{
            //    created = true;
            //}

            if (!created)
            {
                setFunction();
                createTimePicker(LIBRARY_ID, ADULT, FUNCTION);

            }

            if (showNext)
            {
                InfoCell.Visible = true;
                FinishLabel.Text = infoString.ToString();

            }


        }


        protected string checkforEmail()
        {
            EMAIL = Session["email"].ToString();
            if (EMAIL.Equals(string.Empty))
            {
                return string.Empty;
            }
            return EMAIL;
        }





        protected void calendar_DayRender(object sender, DayRenderEventArgs e)
        {
            string onmouseoverStyle = "this.style.backgroundColor='#D4EDFF'";
            string onWeekendStyle = "this.style.backgroundColor='#FF0000'";
            string onDayBefore = string.Empty;
            string onmouseoutStyle = "this.style.backgroundColor='@BackColor'";
            string rowBackColor = string.Empty;


            if (!e.Day.IsToday)
            {

                e.Cell.Attributes.Add("onmouseover", onmouseoverStyle);
                e.Cell.Attributes.Add("onmouseout", onmouseoutStyle.Replace("@BackColor", rowBackColor));
            }


            if (e.Day.IsWeekend && e.Day.Date.CompareTo(DateTime.Now) > 0)
            {
                e.Cell.Attributes.Add("onmouseover", onWeekendStyle);
                e.Cell.Attributes.Add("onmouseout", onmouseoutStyle.Replace("@BackColor", rowBackColor));
                e.Cell.Attributes.Add("onclick", "dateSelected(this)");
            }
            if (e.Day.Date.CompareTo(DateTime.Today) < 0)
            {
                e.Day.IsSelectable = false;
                e.Cell.Attributes.Add("onmouseover", onDayBefore);
                e.Cell.Attributes.Add("onmouseout", onmouseoutStyle.Replace("@BackColor", onDayBefore));
            }



        }

        protected void calendar_SelectionChanged(object sender, EventArgs e)
        {

            SELECTED_DATE = calendar.SelectedDate;
            timepicker.Style["visibility"] = "visible";
            redrawTimePicker(sender);

        }

        protected int MyValue
        {
            get
            {
                return selectedDuration;
            }
            set
            {
                selectedDuration = value;
            }

        }


        protected Tuple<TimeSpan, TimeSpan> getScheduleLibrary(long libraryId)
        {

            return service.getLibrarySchedule(libraryId,SELECTED_DATE);
        }

        protected void createTimePicker(long libraryID, bool adult, string function)
        {

            DateTime date = SELECTED_DATE;
            Tuple<TimeSpan, TimeSpan> librarySchedule = service.getLibrarySchedule(libraryID, SELECTED_DATE);
            
            TimeSpan time = librarySchedule.Item1;//Opening hour
           
            DateTime finalTime = date.Date + time;

            List<TimeSpan> intervalList = service.getIntervalsForTimePicker(librarySchedule);
            
            if (intervalList.Count > 1)
            {
            List<TimeSpan> unavailableIntervals = service.getUnavailableTimes(libraryID, finalTime, intervalList.Count, adult, function);
            
            for (int i = 0; i < intervalList.Count; i++)
            {
                bool colored = false;
                Button b = new Button();
                b.ID = i + "";
                b.Width = 5;
                b.Height = 25;

                b.Style["padding"] = "0px";
                b.Style["border"] = "none";


                foreach (TimeSpan t in unavailableIntervals)
                {
                    if ((t - intervalList[i]).TotalMinutes == 0)
                    {
                        if (!colored)
                        {

                            b.Style["background-color"] = "red";
                            colored = true;
                        }
                    }


                }

                if (!colored)
                {
                    b.Style["background-color"] = "lightgreen";
                    colored = true;
                }




                if (i == 0)
                {
                    b.Style["border-top-left-radius"] = "35px";
                    b.Style["border-bottom-left-radius"] = "35px";
                }
                if (i == intervalList.Count - 1)
                {
                    b.Style["border-top-right-radius"] = "35px";
                    b.Style["border-bottom-right-radius"] = "35px";
                }
                b.ToolTip = intervalList[i].ToString();


                b.Attributes.Add("onmouseover", "mouseOver(this)");
                b.Attributes.Add("onmouseout", "mouseOut(this)");
                b.OnClientClick = "timePick(this)";
                b.Click += new EventHandler(this.MyButtonHandler);

                b.Style["outline"] = "0";


                clock.Controls.Add(b);


            }
            created = true;
            }
        }

        public void MyButtonHandler(object sender, EventArgs e)
        {

            Tuple<TimeSpan, TimeSpan> bookingTimeSpans = null;
            string time = hidden.Value.ToString();
            




            if (!time.Equals("Unavailable"))
            {

                bookingTimeSpans = service.getTimesFromString(time);
                startBooking = SELECTED_DATE.Add(bookingTimeSpans.Item1);
                endBooking = SELECTED_DATE.Add(bookingTimeSpans.Item2);
                infoString.Clear();

                infoString.AppendLine("If this information is correct please press Finish button.<br/>");
                infoString.AppendLine("Date : " + startBooking.Date.ToShortDateString() + "<br/>");

                infoString.AppendLine("Start time : " + startBooking.TimeOfDay + "<br/>");

                infoString.AppendLine("End time : " + endBooking.TimeOfDay + "<br/>");

                if (!FUNCTION.Equals(string.Empty))
                {
                    infoString.AppendLine("Function : " + FUNCTION);
                }

                
                showNext = true;

                redrawTimePicker(sender);

                this.Page_Load(sender, e);

            }
            else
            {
                infoString.Clear();
                infoString.Append("The choosen period is not available, please choose another period of time to book a pc");
                

            }


        }

        protected void calendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            timepicker.Style["visibility"] = "hidden";
            redrawTimePicker(null);
        }

        protected void duration_Click(object sender, EventArgs e)
        {

            Button buton = (Button)sender;
            if (buton.ID.Equals("duration1"))
            {
                MyValue = 2;
                duration2.Style["background-color"] = string.Empty;
                duration3.Style["background-color"] = string.Empty;
            }
            else if (buton.ID.Equals("duration2"))
            {
                MyValue = 4;
                buton.Style["background-color"] = "lightgreen";
                duration3.Style["background-color"] = string.Empty;

            }
            else if (buton.ID.Equals("duration3"))
            {
                MyValue = 8;
                duration2.Style["background-color"] = "lightgreen";
                buton.Style["background-color"] = "lightgreen";
            }
            redrawTimePicker(null);

        }

        protected void redrawTimePicker(object sender)
        {

            int firstButton = 0;
            int lastButton = 0;

            
            foreach (Control c in clock.Controls)
            {

                Button b = (Button)c;


                int currentButton = int.Parse(b.ID);

                if (currentButton > firstButton && currentButton <= lastButton)
                {
                    b.Style["background-color"] = "blue";
                }
                else if (!b.Style["background-color"].Equals("red"))
                {
                    b.Style["background-color"] = "lightgreen";
                }

                if (b == sender && !b.Style["background-color"].Equals("red"))
                {

                    b.Style["background-color"] = "blue";
                    firstButton = int.Parse(b.ID);
                    lastButton = firstButton + selectedDuration;

                }

            }
            if (sender == calendar)
            {//if date change recreate timepicker with selected date
                clock.Controls.Clear();
                createTimePicker(LIBRARY_ID, ADULT, FUNCTION);
            }


        }

        protected void PcType_Click(object sender, EventArgs e)
        {

            List<string> functionsList = service.getFunctions(LIBRARY_ID, ADULT);

            Functions.DataSource = functionsList;
            Functions.DataBind();

            PCTypeTable.Show();

            redrawTimePicker(null);
        }

        protected void setFunction()
        {

            string savedFunction = hidden_functions.Value.ToString();
            if (!savedFunction.Equals(string.Empty))
            {
                FUNCTION = savedFunction;
            }

            if (!FUNCTION.Equals(string.Empty))
            {
                PcType.Text = FUNCTION;
            }
        }

        protected void FinishButton_Click(object sender, EventArgs e)
        {
            resource = service.getOneResourceForSelections(LIBRARY_ID, FUNCTION, ADULT, startBooking, endBooking);
            FinishInfo.Text = "You are going to book the PC " + resource.Name + "<br/> Please press book to finish.";
            FinishPopupExtender.Show();
            
        }

        protected void BooKPC_Click(object sender, EventArgs e)
        {
            if (service.createBooking(LIBRARY_ID, USER_NAME, resource.ResourceID, startBooking, endBooking))
            {
                Response.Redirect("index.aspx");
            }
            else
            {
                FinishInfo.Text = "There has been a problem in booking the PC.<br/>Please try again." ;
                FinishPopupExtender.Show();
            }
            
            
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            FinishPopupExtender.Hide();
            
        }

    }

}