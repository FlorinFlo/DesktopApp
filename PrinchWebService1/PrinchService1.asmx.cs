using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace PrinchWebService1
{
    /// <summary>
    /// Summary description for PrinchService1
    /// </summary>
    [WebService(Namespace = "http://princh.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PrinchService1 : System.Web.Services.WebService
    {
        private static string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;

/*
 * Functions used in the WebApp
 * 
 * 
 * */
        [WebMethod(EnableSession = true)]
        public List<string> getAreas()
        {

            // string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select distinct lib_area from library ", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> areaList = new List<string>();

                while (reader.Read())
                {
                    areaList.Add(reader["lib_area"].ToString());
                }

                reader.Close();
                conn.Close();
                return areaList;


            }

        }

        [WebMethod(EnableSession = true)]
        public List<Library> getLibraryFor(string area)
        {
            // string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select lib_id,lib_name,lib_area,req_profile from library where lib_area=@lib_area", conn);
                cmd.Parameters.AddWithValue("@lib_area", area);
                SqlDataReader reader = cmd.ExecuteReader();
                List<Library> libraryList = new List<Library>();


                while (reader.Read())
                {
                    Library library = new Library();

                    library.ID = Convert.ToInt64(reader["lib_id"]);
                    library.Name = reader["lib_name"].ToString();
                    library.Area = reader["lib_area"].ToString();
                    library.ReqProfile = Convert.ToBoolean(reader["req_profile"]);

                    libraryList.Add(library);
                }

                reader.Close();
                conn.Close();
                return libraryList;


            }



        }

        [WebMethod(EnableSession = true)]
        public List<Resource> getResources(DateTime selectedDateAndHour, long libraryId)
        {
            // string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from resource where library_id=@lib_id and resource_id not in (select resource_id from booking where library_id=@lib_id AND datediff(day,@now,booking_date_start)=0 AND ABS(datediff(minute,@now,booking_date_start))>=0 and ABS(datediff(minute,@now,booking_date_start))<=30 AND datediff(minute,@now,booking_date_end)>=30) ", conn);
                cmd.Parameters.AddWithValue("@lib_id", libraryId);
                cmd.Parameters.AddWithValue("@now", selectedDateAndHour);

                SqlDataReader reader = cmd.ExecuteReader();
                List<Resource> resourceList = new List<Resource>();


                while (reader.Read())
                {
                    Resource resource = new Resource();

                    resource.ResourceID = reader["resource_id"].ToString();
                    resource.ResourceType = reader["resource_type"].ToString();
                    resource.LibraryID = Convert.ToInt64(reader["library_id"]);
                    resource.Funct = reader["funct"].ToString();
                    resource.Adult = Convert.ToBoolean(reader["adult"]);
                    resource.Available = Convert.ToBoolean(reader["available"]);
                    resource.Name = reader["name"].ToString();


                    resourceList.Add(resource);
                }

                reader.Close();
                conn.Close();
                return resourceList;

            }
        }

        [WebMethod]
        public bool requireSub(long libId)
        {
            bool temp = false;
            //string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select req_profile from library where lib_id=@lib_id ", connection);

                cmd.Parameters.AddWithValue("lib_id", libId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    temp = reader.GetBoolean(0);
                }

                reader.Close();
                if (temp)
                {
                    return true;
                }
                else
                {
                    return false;
                }



            }


        }
        [WebMethod]
        public string getUrlForValidation(long libraryId)
        {
            string validationString = string.Empty;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select connection_string from library where lib_id=@lib_id", conn);
                cmd.Parameters.AddWithValue("@lib_id", libraryId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    validationString = reader["connection_string"].ToString();


                }
                reader.Close();

            }


            return validationString;
        }

        [WebMethod]
        public Booking checkIfAvailable(long libID, string resourceID, DateTime now)
        {

            Booking booking = null;


            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from booking where library_id=@lib_id AND resource_id=@resource_id AND datediff(day,@now,booking_date_start)=0 AND datediff(minute,@now,booking_date_end)>=0 AND datediff(minute,@now,booking_date_start)>=-15", conn);
                cmd.Parameters.AddWithValue("@lib_id", libID);
                cmd.Parameters.AddWithValue("@resource_id", resourceID);
                cmd.Parameters.AddWithValue("@now", now);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    booking = new Booking();
                    booking.BookingID = Convert.ToInt64(reader["booking_id"]);
                    booking.LibraryID = Convert.ToInt64(reader["library_id"]);
                    booking.ClientID = reader["client_id"].ToString();
                    if (!reader.IsDBNull(3))
                    {
                        booking.ClientPass = reader.GetString(3);
                    }
                    else
                    {
                        booking.ClientPass = string.Empty;
                    }


                    booking.ResourceID = reader["resource_id"].ToString();
                    booking.BookingStart = Convert.ToDateTime(reader["booking_date_start"]);
                    booking.BookingEnd = Convert.ToDateTime(reader["booking_date_end"]);
                    // MessageBox.Show("Create booking"+booking1.BookingID+"....."+booking1.BookingStart);

                }
                reader.Close();


            }




            return booking;
        }

        [WebMethod]
        public Schedule getScheduleForLibraryOnDay(long libraryID, int dayofWeek)
        {
            Schedule schedule = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select schedule_open_day"+dayofWeek+",schedule_close_day"+dayofWeek+" from schedule_table  where schedule_id=(select  schedule from resource_setup where library=@library)", conn);
                    cmd.Parameters.AddWithValue("@library", libraryID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {

                            return schedule;
                        }
                        else
                        {
                            schedule = new Schedule();
                            schedule.OpeningHour = reader.GetTimeSpan(0);
                            schedule.ClosingHour = reader.GetTimeSpan(1);
                            
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return schedule;
            }
            return schedule;
        }

        [WebMethod]
        public bool createBookingFromWeb(long libraryId, byte[] clientId, byte[] clientPass, string resourceId, DateTime startDate, DateTime endDate)
        {

            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into booking(library_id,client_id,client_pas,resource_id,booking_date_start,booking_date_end) Values (@library_id,@client_id,@client_pass,@resource_id,@booking_date_start,@booking_date_end)", conn);
                    cmd.Parameters.AddWithValue("@library_id", libraryId);
                    //hashed clientid
                    SqlParameter sqlParamID = cmd.Parameters.AddWithValue("@client_id", clientId);
                    sqlParamID.DbType = DbType.Binary;
                    //salt
                    SqlParameter sqlParamPas = cmd.Parameters.AddWithValue("@client_pass", clientPass);
                    sqlParamPas.DbType = DbType.Binary;

                    cmd.Parameters.AddWithValue("@resource_id", resourceId);
                    cmd.Parameters.AddWithValue("@booking_date_start", startDate);
                    cmd.Parameters.AddWithValue("@booking_date_end", endDate);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public List<Resource> getAllResources(long libID)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from resource where library_id=@lib_id and available=1", conn);
                    cmd.Parameters.AddWithValue("@lib_id", libID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Resource> resourceList = new List<Resource>();
                    while (reader.Read())
                    {
                        Resource resource = new Resource();

                        resource.ResourceID = reader["resource_id"].ToString();
                        resource.ResourceType = reader["resource_type"].ToString();
                        resource.LibraryID = Convert.ToInt64(reader["library_id"]);
                        resource.Funct = reader["funct"].ToString();
                        resource.Adult = Convert.ToBoolean(reader["adult"]);
                        resource.Available = Convert.ToBoolean(reader["available"]);
                        resource.Name = reader["name"].ToString();
                        resource.Setup = Convert.ToInt32(reader["setup_res"]);
                        resourceList.Add(resource);
                    }

                    reader.Close();
                    conn.Close();
                    return resourceList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        [WebMethod]
        public int getAdultChildResource(bool adult, long libraryID)
        {
            int count = 0;
            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select count(resource_id) from resource where adult=@adult and library_id=@library_id", conn);
                    if (adult)
                    {
                        cmd.Parameters.AddWithValue("@adult", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@adult", 0);
                    }
                    cmd.Parameters.AddWithValue("@library_id", libraryID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        count = reader.GetInt32(0);

                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return count;
        }


        [WebMethod]
        public List<DateTime> getUnavailableTimes(long libraryID, DateTime openingHour, int intervals, bool adult, string function)
        {
            List<DateTime> times = new List<DateTime>();
            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("unavailable", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@library", SqlDbType.BigInt).Value = libraryID;
                    cmd.Parameters.Add("@start_time", SqlDbType.DateTime2).Value = openingHour;
                    cmd.Parameters.Add("@count_intervals", SqlDbType.Int).Value = intervals;
                    if (adult)
                    {
                        cmd.Parameters.Add("@adult", SqlDbType.Bit).Value = 1;
                    }
                    else
                    {
                        cmd.Parameters.Add("@adult", SqlDbType.Bit).Value = 0;
                    }
                    if (function.Equals(string.Empty))
                    {
                        cmd.Parameters.Add("@function", SqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@function", SqlDbType.VarChar).Value = function;
                    }


                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime time = Convert.ToDateTime(reader["hour_available"]);
                        bool available = Convert.ToBoolean(reader["available"]);
                        if (!available)
                        {
                            times.Add(time);
                        }

                    }
                    reader.Close();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }





            return times;
        }
        

        [WebMethod]
        public List<string> getFunctions(long libraryID, bool adult)
        {
            List<string> functionsList = new List<string>();

            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("select distinct funct from resource where library_id=@library_id and adult=@adult", conn);
                    cmd.Parameters.Add("@library_id", libraryID);
                    if (adult)
                    {
                        cmd.Parameters.Add("@adult", 1);
                    }
                    else
                    {
                        cmd.Parameters.Add("@adult", 0);
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        functionsList.Add(reader["funct"].ToString());
                    }
                    reader.Close();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return functionsList;
        }


        [WebMethod]
        public Resource getOneResourceForSelections(long libraryID, string function, bool adult, DateTime startBooking, DateTime endBooking)
        {
            Resource resource = null;
            string selectQuerry = string.Empty;
            DateTime start = startBooking.AddMinutes(+1);
            DateTime end = endBooking.AddMinutes(-1);
            if (function.Equals(string.Empty))
            {
                selectQuerry = "Select TOP 1 * from resource where library_id=@libraryID AND adult=@adult AND resource_id NOT IN ( SELECT resource_id from booking where library_id=1 AND @startBooking between booking_date_start AND booking_date_end AND @endBooking between booking_date_start AND booking_date_end)";
            }
            else
            {
                selectQuerry = "Select TOP 1 * from resource where library_id=@libraryID AND funct like'%'+@function+'%' AND adult=@adult AND resource_id NOT IN ( SELECT resource_id from booking where library_id=@libraryID AND @startBooking between booking_date_start AND booking_date_end AND @endBooking between booking_date_start AND booking_date_end )";
            }

            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(selectQuerry, conn);
                    cmd.Parameters.AddWithValue("@libraryID", libraryID);
                    cmd.Parameters.AddWithValue("@function", function);
                    if (adult)
                    {
                        cmd.Parameters.AddWithValue("@adult", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@adult", 0);
                    }
                    cmd.Parameters.AddWithValue("@startBooking", start);
                    cmd.Parameters.AddWithValue("@endBooking", end);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        resource = new Resource();
                        resource.ResourceID = Convert.ToString(reader["resource_id"]);
                        resource.ResourceType = Convert.ToString(reader["resource_type"]);
                        resource.LibraryID = Convert.ToInt64(reader["library_id"]);
                        resource.Funct = Convert.ToString(reader["funct"]);
                        resource.Adult = Convert.ToBoolean(reader["adult"]);
                        resource.Available = Convert.ToBoolean(reader["available"]);
                        resource.Name = reader["name"].ToString();

                    }
                    reader.Close();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resource;
        }


        [WebMethod]
        public User validateUser(string user, string password, long libraryId)
        {
            User returnUser = null;

            Dictionary<string, string> dict = null;
            try
            {
                string validationUrl = modifieUrl(getUrlForValidation(libraryId), user, password);
                WebRequest webRequest = WebRequest.Create(validationUrl);
                WebResponse webresponse = webRequest.GetResponse();
                dict = ReadFrom(webresponse.GetResponseStream());
                string status;
                if (dict != null)
                {
                    if (dict.TryGetValue("status", out status))
                    {
                        if (Int32.Parse(status) == 0)
                        {
                            string sms;
                            string email;
                            string birth;

                            dict.TryGetValue("sms", out sms);
                            dict.TryGetValue("email", out email);
                            dict.TryGetValue("birth", out birth);
                            returnUser = new User(sms, email, birth);

                        }
                    }

                }

            }
            catch (System.Net.WebException)
            {

                return returnUser;
            }

            return returnUser;

        }


/*
 * Functions used for Desktop app
 * */


        [WebMethod]
        public bool createBooking(long libraryId, string clientId, string clientPass, string resourceId, DateTime startDate, DateTime endDate)
        {           
            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into booking(library_id,client_id,client_pas,resource_id,booking_date_start,booking_date_end) Values (@library_id,@client_id,@client_pass,@resource_id,@booking_date_start,@booking_date_end)", conn);
                    cmd.Parameters.AddWithValue("@library_id", libraryId);
                    if (clientId.Equals("QUICK_BOOK"))
                    {
                        cmd.Parameters.AddWithValue("@client_id", clientId);
                        cmd.Parameters.AddWithValue("@client_pass", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@resource_id", resourceId);
                    cmd.Parameters.AddWithValue("@booking_date_start", startDate);
                    cmd.Parameters.AddWithValue("@booking_date_end", endDate);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public Credentials getUserForBooking(long bookingID, long libraryID)
        {
            Credentials credentials = null;
            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select client_id,client_pas from booking where booking_id=@booking_id and library_id=@library_id", conn);
                    cmd.Parameters.AddWithValue("@booking_id", bookingID);
                    cmd.Parameters.AddWithValue("@library_id", libraryID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        credentials = new Credentials();
                        credentials.ClientID = reader["client_id"].ToString();
                        credentials.ClientPass = reader["client_pas"].ToString();

                    }
                    reader.Close();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return credentials;
        }

        [WebMethod]
        public int getGlobalSetupId(long libraryID)
        {
            int setupID = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd=new SqlCommand("Select setup_id from resource_setup where library=@library",conn);
                    cmd.Parameters.AddWithValue("@library", libraryID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while(reader.Read()){
                        setupID = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
               
                return setupID;
            }
            return setupID;
        }
       

    /*
     * Functions used for installer
     * */

        [WebMethod]
        public bool checkIfRegistered(long libID, string resourceID)
        {
            int count = 0;
            
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select count(resource_id) from resource where resource_id=@resource_id and library_id=@library_id", conn);
                cmd.Parameters.AddWithValue("@resource_id", resourceID);
                cmd.Parameters.AddWithValue("@library_id", libID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    count = reader.GetInt32(0);

                }
                reader.Close();

                if (count == 0)
                    return false;
                else
                    return true;


            }

        }

        [WebMethod]
        public bool registerLibrary(string libName, string libArea, string connectionString, string installationType)
        {

            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into library(lib_name,lib_area,connection_string,req_profile,installation_type) Values(@lib_name,@lib_area,@connection_string,@req_profile,@installation_type)", conn);
                    cmd.Parameters.AddWithValue("@lib_name", libName);
                    cmd.Parameters.AddWithValue("@lib_area", libArea);
                    if (connectionString.Equals(string.Empty))
                    {
                        cmd.Parameters.AddWithValue("@connection_string", DBNull.Value);

                    }

                    else
                    {
                        cmd.Parameters.AddWithValue("@connection_string", connectionString);

                    }

                    if (connectionString.Equals(string.Empty))
                    {
                        cmd.Parameters.AddWithValue("@req_profile", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@req_profile", 1);
                    }

                    cmd.Parameters.AddWithValue("@installation_type", installationType);


                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;

            }
        }

        [WebMethod]
        public bool registerStation(string resourceID, string resourceType, long libraryID, string funct, bool adult, bool available, string name, int setup)
        {
            int succsefull = 0;
            try
            {


                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into resource(resource_id,resource_type,library_id,funct,adult,available,name,setup_res) Values(@resource_id,@resource_type,@library_id,@funct,@adult,@available,@name,@setup_res)", conn);
                    cmd.Parameters.AddWithValue("@resource_id", resourceID);
                    cmd.Parameters.AddWithValue("@resource_type", resourceType);
                    cmd.Parameters.AddWithValue("@library_id", libraryID);
                    cmd.Parameters.AddWithValue("@funct", funct);
                    cmd.Parameters.AddWithValue("@adult", adult);
                    cmd.Parameters.AddWithValue("@available", available);
                    cmd.Parameters.AddWithValue("@name", name);
                    if (setup == 0)
                    {
                        cmd.Parameters.AddWithValue("@setup_res", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@setup_res", setup);
                    }
                    succsefull = cmd.ExecuteNonQuery();
                }
            }catch(Exception ex){

            }

            if (succsefull == 0)
            {
                return false;
            }else{
                return true;
            }


            
        }

        [WebMethod]
        public int getStationSetup(bool controlPanel, bool Cmd, int daysInAdvance, int ageRestriciton)
        {
            int setupId = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select Top 1 setup_id from resource_setup where preebooking_days=@prebooking_days and age_restriction=@age_restriction and control_panel=@control_panel and cmd_status=@cmd  and library is NULL and back_img is NULL", conn);
                cmd.Parameters.AddWithValue("@prebooking_days", daysInAdvance);
                cmd.Parameters.AddWithValue("@age_restriction", ageRestriciton);
                if (controlPanel)
                {
                    cmd.Parameters.AddWithValue("@control_panel", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@control_panel", 0);
                }

                if (Cmd)
                {
                    cmd.Parameters.AddWithValue("@cmd", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@cmd", 0);
                }


                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    setupId = reader.GetInt32(0);
                }
                reader.Close();
            }

            return setupId;
        }


        [WebMethod]
        public void createSetup(bool controlPanel, bool Cmd, int daysInAdvance, int ageRestriciton)
        {

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Insert into resource_setup (preebooking_days,age_restriction,control_panel,cmd_status,schedule,back_img,library) Values(@prebooking,@age_restriction,@control_panel,@cmd_status,@schedule,@back_img,@library)", conn);
                cmd.Parameters.AddWithValue("@prebooking", daysInAdvance);
                cmd.Parameters.AddWithValue("@age_restriction", ageRestriciton);
                cmd.Parameters.AddWithValue("@control_Panel", controlPanel);
                cmd.Parameters.AddWithValue("@cmd_status", Cmd);
                cmd.Parameters.AddWithValue("@schedule", DBNull.Value);
                cmd.Parameters.AddWithValue("@back_img", DBNull.Value);
                cmd.Parameters.AddWithValue("@library", DBNull.Value);
                cmd.ExecuteNonQuery();


            }


        }


        [WebMethod]
        public Setup getSetup(int id)
        {
            Setup setup = null;
            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select * from resource_setup where setup_id=@setup_id", conn);
                    cmd.Parameters.AddWithValue("@setup_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        setup = new Setup();
                        setup.SetupID = Convert.ToInt32(reader["setup_id"]);
                        setup.PrebookingDays = Convert.ToInt32(reader["preebooking_days"]);
                        setup.AgeRestriction = Convert.ToInt32(reader["age_restriction"]);
                        setup.ControlPanel = Convert.ToBoolean(reader["control_panel"]);
                        setup.CmdStatus = Convert.ToBoolean(reader["cmd_status"]);
                        setup.Schedule = Convert.ToInt32(reader["schedule"]);



                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return setup;

        }


        [WebMethod]
        public int getSetupId(string resourceId)
        {
            int setup = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select setup_res from resource where resource_id=@resource_id", conn);
                cmd.Parameters.AddWithValue("@resource_id", resourceId);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                    {
                        setup = 0;
                    }
                    else
                    {
                        setup = Convert.ToInt32(reader["setup_res"]);
                    }
                    

                }
                reader.Close();
            }

            return setup;
        }


        [WebMethod]
        public Setup getGlobalSetup(long libraryID)
        {
            Setup setup = null;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * from resource_setup where library=@lib_id", conn);
                cmd.Parameters.AddWithValue("@lib_id", libraryID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    setup = new Setup();
                    setup.SetupID = Convert.ToInt32(reader["setup_id"]);
                    setup.PrebookingDays = Convert.ToInt32(reader["preebooking_days"]);
                    setup.AgeRestriction = Convert.ToInt32(reader["age_restriction"]);
                    setup.ControlPanel = Convert.ToBoolean(reader["control_panel"]);
                    setup.CmdStatus = Convert.ToBoolean(reader["cmd_status"]);
                    setup.Schedule = Convert.ToInt32(reader["schedule"]);

                }
                reader.Close();
            }

            return setup;
        }


        [WebMethod]
        public bool createGlobalSetup(int preebookingDays, int ageRestriction, bool controlPanel, bool cmdStatus, byte[] image, long library)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Insert into  resource_setup(preebooking_days,age_restriction,control_panel,cmd_status,schedule,back_img,library) values(@preebooking_days,@age_restriction,@control_panel,@cmd_status,1,@back_img,@library)", conn);
                    cmd.Parameters.AddWithValue("@preebooking_days", preebookingDays);
                    cmd.Parameters.AddWithValue("@age_restriction", ageRestriction);
                    if (controlPanel)
                    {
                        cmd.Parameters.AddWithValue("@control_panel", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@control_panel", 0);
                    }

                    if (cmdStatus)
                    {
                        cmd.Parameters.AddWithValue("@cmd_status", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@cmd_status", 0);
                    }

                    if (image != null)
                    {
                        SqlParameter sqlParam = cmd.Parameters.AddWithValue("@back_img", image);
                        sqlParam.DbType = DbType.Binary;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@back_img", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@library", library);


                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        [WebMethod]
        public string getStationName(long libraryID)
        {

            string stationName = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select Top 1 name from resource where library_id=@library_id order by name desc", conn);

                    cmd.Parameters.AddWithValue("@library_id", libraryID);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        stationName = reader.GetString(0);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }


            string stationNameNumber = "1";
            try
            {
                stationNameNumber = (Int32.Parse(Regex.Match(stationName, @"\d+").Value) + 1).ToString();

            }
            catch
            {
                return stationNameNumber;
            }

            return stationNameNumber;

        }

        [WebMethod]
        public long getLibraryID(string libArea, string libName)
        {
            long libID = 0;
            // string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select lib_id from library where lib_name=@lib_name AND lib_area=@lib_area", conn);
                cmd.Parameters.AddWithValue("@lib_name", libName);
                cmd.Parameters.AddWithValue("@lib_area", libArea);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    libID = reader.GetInt64(0);
                }
                reader.Close();
            }
            return libID;
        }

        

        //[WebMethod]
        //private Library getLibrary(string area, string name)
        //{
        //    // GetConnectionFPrinch();
        //    // MessageBox.Show(" GEt Library Lib area" + area + "Libname" + libName);
        //    Library lib=null;

        //    string connString = ConfigurationManager.ConnectionStrings["PrinchDB"].ConnectionString;
        //        using (SqlConnection conn = new SqlConnection(connString))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand("Select lib_id,connection_string,provider,lib_name,lib_area from library where lib_name=@lib_name AND lib_area=@lib_area", conn);
        //            cmd.Parameters.AddWithValue("@lib_name", name);
        //            cmd.Parameters.AddWithValue("@lib_area", area);
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                lib=new Library();
        //                lib.ID = reader.GetInt64(0);
        //                lib. = reader.GetString(1);
        //                lib.Provider = reader.GetString(2);
        //                lib.Name = reader.GetString(3);
        //                lib.Area = reader.GetString(4);
        //            }
        //            reader.Close();
        //        }
        //    }

       


       

        

        

       

        private static string modifieUrl(string url, string name, string password)
        {

            string userReplace = url.Replace("[USER]", name);
            string replacedComplete = userReplace.Replace("[PASSWORD]", password);
            return replacedComplete;


        }

        private static Dictionary<string, string> ReadFrom(Stream stream)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            XmlTextReader reader = new XmlTextReader(stream);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.                         
                        // Read the attributes.
                        while (reader.MoveToNextAttribute())
                        {

                            dict.Add(reader.Name.ToString(), reader.Value.ToString());

                        }


                        break;

                }
            }
            reader.Close();
            return dict;
        }
        
            
        [WebMethod]
        public  string getConnectionString(long libraryID)
        {
            string connection = string.Empty;
            try{

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("Select connection_string from library where lib_id=@lib_id ", conn);
                    cmd.Parameters.AddWithValue("@lib_id", libraryID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        connection = Convert.ToString(reader["connection_string"]);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                
                return string.Empty;
            }

            return connection;
        }

               
    }


}
