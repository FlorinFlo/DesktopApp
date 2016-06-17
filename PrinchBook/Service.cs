using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace PrinchBook
{
    public class Service
    {
        private static Service instance;
        private static List<PrinchBook.ServiceReference2.Resource> resourceList = new List<PrinchBook.ServiceReference2.Resource>();

        public Service()
        {

        }

        public static Service Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Service();
                }

                return instance;
            }
        }


        //internal List<Tuple<string, string>> createSchedule(int hourOpen, int hourClose, int minuteClose, int minuteOpen)
        //{
        //    string minutes, hours;

        //    List<Tuple<string, string>> schedule = new List<Tuple<string, string>>();
        //    while (hourOpen <= hourClose)
        //    {
        //        if (minuteOpen < 10)
        //        {
        //            minutes = "0" + minuteOpen;
        //        }
        //        else if (minuteOpen == 60)
        //        {
        //            minutes = "00";
        //            minuteOpen = 0;
        //            hourOpen++;
        //        }
        //        else
        //        {
        //            minutes = "" + minuteOpen;
        //        }
        //        if (hourOpen < 10)
        //        {
        //            hours = "0" + hourOpen;
        //        }
        //        else
        //        {
        //            hours = "" + hourOpen;
        //        }


        //        schedule.Add(Tuple.Create(hours, minutes));


        //        minuteOpen += 30;


        //    }

        //    return schedule;
        //}

        //internal List<string> createListTime(List<Tuple<string, string>> times)
        //{
        //    List<String> listTime = new List<string>();
        //    foreach (Tuple<string, string> t in times)
        //    {
        //        string time = t.Item1 + ":" + t.Item2;
        //        listTime.Add(time);
        //    }
        //    return listTime;
        //}

        //internal void sendEmail(String email, StringBuilder emailBody)
        //{
        //    try
        //    {
        //        var fromAddress = new MailAddress("florea.flo@gmail.com", "florea.flo@gmail.com");
        //        var toAddress = new MailAddress(email, email);
        //        const string fromPassword = "David151991";
        //        const string subject = "Booking PC through Princh";
        //        string body = emailBody.ToString();

        //        var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
        //            Timeout = 20000
        //        };
        //        using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
        //        {
        //            Subject = subject,
        //            Body = body
        //        })
        //        {

        //            smtp.Send(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex + "?????????????");
        //    }

        //}

        //internal StringBuilder createInfo(StringBuilder info, ListItem area, ListItem library, string date, ListItem time, string station, string user, string password, bool prevEnd)
        //{

        //    info.Clear();
        //    info.Append("You have booked a pc in the area ");
        //    info.Append(area);
        //    info.Append(" at the ");
        //    info.Append(library);
        //    info.Append(" on the date ");
        //    info.Append(date);
        //    info.Append(" at the hour ");
        //    info.Append(time);
        //    info.Append(" the station ");
        //    info.Append(station);
        //    if (prevEnd)
        //    {
        //        info.Append(".<br/> After pressing the finish button you will get a confirmation on your email");
        //    }
        //    else
        //    {
        //        info.Append(". Your user is ");
        //        info.Append(user);
        //        info.Append(" and password for loging in is ");
        //        info.Append(password);
        //    }

        //    return info;
        //}

        //internal void getAvailableResourcesForSelectedHour(string time, string selectedDate, long libId)
        //{
        //    PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
        //    DateTime selectedTime = getDateTimeFromStringPDate(time, selectedDate);
        //    resourceList.Clear();
        //    resourceList = soap.getResources(selectedTime, libId);



        //}

        //internal DateTime getDateTimeFromStringPDate(string time, string selectedDate)
        //{
        //    DateTime returnDate = new DateTime();
        //    try
        //    {
        //        string[] hM = time.Split(new[] { ':' });
        //        TimeSpan ts = new TimeSpan(Int32.Parse(hM[0].ToString()), Int32.Parse(hM[1]), 0);
        //        returnDate = Convert.ToDateTime(selectedDate);
        //        returnDate = returnDate.Date + ts;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex + "");
        //    }


        //    return returnDate;
        //}

        //internal List<PrinchBook.ServiceReference2.Resource> initiateResourcesTables(string time, string selectedDate, bool adult, string type, long libId)
        //{
        //    getAvailableResourcesForSelectedHour(time, selectedDate, libId);
        //    List<PrinchBook.ServiceReference2.Resource> resources = new List<PrinchBook.ServiceReference2.Resource>();
        //    foreach (PrinchBook.ServiceReference2.Resource r in resourceList)
        //    {
        //        if (adult && r.Adult & r.ResourceType.Equals(type))
        //        {
        //            resources.Add(r);
        //        }
        //        else if (!adult && !r.Adult && r.ResourceType.Equals(type))
        //        {
        //            resources.Add(r);
        //        }
        //    }
        //    //Debug.WriteLine("Initiate tables Pc" + resourceList.Count);
        //    return resources;
        //}

        internal bool checkIfSubscriptionRequired(string p)
        {
            long libraryId = Int64.Parse(p);
            return getRequiresSub(libraryId);

        }

        
        internal ServiceReference2.User LogInWithCredentials(string username, string password, string libraListSelectedValue)
        {
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            return soap.validateUser(username, password, Convert.ToInt64(libraListSelectedValue));

        }

        //private string[] getTime(string time)
        //{
        //    string[] hM = time.Split(':');
        //    return hM;

        //}

        internal void getLibraries(DropDownList areaList)
        {
            List<string> areas = new List<string>();

            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            areas = soap.getAreas();
            areaList.DataSource = areas;
            areaList.DataBind();
            areaList.Items.Insert(0, "-Select-");

        }

        internal void getLibrariesForArea(string area, DropDownList libraryList)
        {
            List<PrinchBook.ServiceReference2.Library> libList = new List<PrinchBook.ServiceReference2.Library>();

            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            libList = soap.getLibraryFor(area);
            libraryList.DataSource = libList;
            libraryList.DataValueField = "ID";
            libraryList.DataTextField = "Name";
            libraryList.DataBind();
            libraryList.Items.Insert(0, "-Select-");

        }
              

        internal bool getRequiresSub(long libraryId)
        {
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();

            return soap.requireSub(libraryId);

        }


        internal Tuple<byte[], byte[]> getHashCredentials(string plainText)
        {
            

            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

            int max_lenght = 32;

            byte[] salt = new byte[max_lenght];
            random.GetNonZeroBytes(salt);


            byte[] plainTextByte = Encoding.UTF8.GetBytes(plainText);
            byte[] plainTextWithSaltBytes = new byte[plainTextByte.Length + salt.Length];
            for (int i = 0; i < plainTextByte.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextByte[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainTextByte.Length + i] = salt[i];
            }

            HashAlgorithm hash = new SHA1Managed();

            byte[] hashBytesUserId = hash.ComputeHash(plainTextWithSaltBytes);

            

            return new Tuple<byte[],byte[]>(hashBytesUserId,salt);
        }

        //internal void createBooking(long libraryId, byte[] clientId, byte[] clientPass, string resourceId, string startDate, string time)
        //{
        //    int hour = Int32.Parse(getTime(time)[0]);
        //    int minutes = Int32.Parse(getTime(time)[1]);

        //    TimeSpan timeSpan = new TimeSpan(hour, minutes, 0);

        //    DateTime startDt = Convert.ToDateTime(startDate);
        //    startDt = startDt.Date + timeSpan;
        //    DateTime endDate = startDt.AddHours(1);

        //    PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
        //    soap.createBookingFromWeb(libraryId, clientId, clientPass, resourceId, startDt, endDate);
        //}

       //internal List<ServiceReference2.Resource> getResourcesForTables1(long libID)
       // {
       //    PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
       //    return soap.getAllResources(libID);
       // }

        //internal Dictionary<ServiceReference2.Resource, List<string>> getInformationForResourceTable(List <ServiceReference2.Resource> resourceList ,List<string> times)
        //{
        //    Dictionary<ServiceReference2.Resource, List<string>> dict = new Dictionary<ServiceReference2.Resource, List<string>>();

        //    foreach (ServiceReference2.Resource res in resourceList)
        //    {
        //        dict.Add(res, times);
        //    }
        //    return dict;
        //}

        internal Tuple<TimeSpan, TimeSpan> getTimesFromString(string duration)
        {
            int startHour=0;
            int startMinutes=0;
            int endHour=0;
            int endMinutes=0;

            string [] times = duration.Split('-');
            string [] startTimes = times[0].Split(':');
            string[] endTimes = times[1].Split(':');
            startHour = int.Parse(startTimes[0]);
            startMinutes = int.Parse(startTimes[1]);
            endHour = int.Parse(endTimes[0]);
            endMinutes = int.Parse(endTimes[1]);          

            return Tuple.Create(new TimeSpan(startHour, startMinutes,0), new TimeSpan(endHour, endMinutes,0));

        }

        internal Tuple<TimeSpan, TimeSpan> getLibrarySchedule(long libraryId,DateTime date)
        {
            TimeSpan openingHour = new TimeSpan();
            TimeSpan closingHour = new TimeSpan();
            int dayNoOfWeek = (int)date.DayOfWeek;
            Debug.WriteLine(dayNoOfWeek + "Day of week");
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            PrinchBook.ServiceReference2.Schedule schedule = soap.getScheduleForLibraryOnDay(libraryId, dayNoOfWeek);
            if (schedule != null)
            {
                
                 openingHour = schedule.OpeningHour;
                 closingHour = schedule.ClosingHour;
            }
            else
            {
                
                openingHour = new TimeSpan(0, 0, 0, 0);
                closingHour = new TimeSpan(0, 0, 0, 0);
            }
            
            

            return Tuple.Create(openingHour, closingHour);
        }

        //get intervals for timepicker having 15 minutes intervals
        internal List<TimeSpan> getIntervalsForTimePicker(Tuple<TimeSpan,TimeSpan> librarySchedule){

            TimeSpan pivotInterval=librarySchedule.Item1;

            List<TimeSpan> intervalList = new List<TimeSpan>();

           int intervals = Convert.ToInt32(librarySchedule.Item2.Subtract(librarySchedule.Item1).TotalMinutes/15);

            Debug.WriteLine(intervals + "INTERVALE"+librarySchedule.Item1+"????????"+librarySchedule.Item2);
            TimeSpan quarter = TimeSpan.FromMinutes(15);


            for (int i = 0; i <= intervals; i++)
            {

                intervalList.Add(pivotInterval);
                var time = pivotInterval.Add(quarter);
                pivotInterval = time;
            }
            return intervalList;

        }


        internal List<TimeSpan> getUnavailableTimes(long libraryID,DateTime openingHour, int intervals,bool adult,string function)
        {
            List<TimeSpan> times=new List<TimeSpan>();
            times.Clear();

            List<DateTime> dateTimeList = new List<DateTime>();
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();

            dateTimeList = soap.getUnavailableTimes(libraryID, openingHour, intervals, adult, function);

            foreach (DateTime time in dateTimeList)
            {
                times.Add(time.TimeOfDay);
                Debug.WriteLine(time.TimeOfDay + "ITS ADDED"+openingHour);
            }

            return times;
        }

        internal bool isAdult(string userBirth,int ageRestriction)
        {
            
            int year = Int32.Parse(userBirth.Substring(0, 4));
           
            int month = Int32.Parse(userBirth.Substring(4, 2));
            int day = Int32.Parse(userBirth.Substring(6, 2));
           
            DateTime date = new DateTime(year, month, day);
            DateTime now = DateTime.Now;
            int age = (now - date).Days / 365;
            if (age < ageRestriction)
            {
                return false;
            }


            return true;
        }

        internal List<string> getFunctions(long librarID, bool adult)
        {
            List<string> returnFunctions = new List<string>();
            List<string> function = new List<string>();
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            function=soap.getFunctions(librarID, adult);

            foreach (string f in function)
            {
                string[] func=null;
                if (f.Contains(","))
                {
                     func = f.Split(',');
                     if (func != null && func.Length > 0)
                     {
                         for (int i = 0; i < func.Length; i++)
                         {
                             if (!returnFunctions.Contains(func[i]))
                             {
                                 returnFunctions.Add(func[i]);
                             }
                         }
                     }

                }
                else
                {
                    returnFunctions.Add(f);
                }
                

            }


            return returnFunctions;
        }


        internal ServiceReference2.Resource getOneResourceForSelections(long LIBRARY_ID, string FUNCTION, bool ADULT,DateTime startDate,DateTime endBooking)
        {
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            return soap.getOneResourceForSelections(LIBRARY_ID, FUNCTION, ADULT, startDate, endBooking);
            
        }

        internal bool createBooking(long LIBRARY_ID,string user,string resourceID, DateTime dateStart, DateTime dateEnd)
        {
            byte []clientID = getHashCredentials(user).Item1;
            byte[] clientPass = getHashCredentials(user).Item2;
            PrinchBook.ServiceReference2.PrinchService1SoapClient soap = new ServiceReference2.PrinchService1SoapClient();
            return soap.createBookingFromWeb(LIBRARY_ID, clientID, clientPass, resourceID, dateStart, dateEnd);

            
        }
    }
}