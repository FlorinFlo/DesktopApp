using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.ServiceModel.Channels;
using System.Threading;
using System.ServiceModel;
using System.Net.NetworkInformation;
using System.Net;
using System.Linq;
using System.IO;




namespace CustomAction
{
    public class CustomActions
    {
        private static long libraryId;
        private static string area;
        private static string name;
        private static string installationType;
        private static string pcName;
        private static bool adult;
        private static string pcFunctions;
        private static string installationPath;


        private static bool controlPanel;
        private static bool Cmd;
        private static int days;
        private static int ageRestriction;
        private static string imagePath;
        private static string URL;
        

        private static string resourceId;
        private static int setupResource;
        private static int globalSetup;
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        private static CustomAction.ServiceReference1.Setup setupObject = null;



        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {


            initiate1(session);

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CustomAction2(Session session)
        {


            initiate2(session);

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult CustomAction3(Session session)
        {

            System.Windows.Forms.OpenFileDialog openFileDialog = null;

            try
            {

                var thread = new Thread(
                    () =>
                    {

                        using (openFileDialog = new System.Windows.Forms.OpenFileDialog())
                            openFileDialog.CheckPathExists = true;
                        openFileDialog.CheckFileExists = true;
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;";
                        openFileDialog.DefaultExt = "*. *";

                        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            session["IMGPATH"] = openFileDialog.FileName;

                        }

                        if (openFileDialog != null)
                        {
                            openFileDialog.Dispose();
                        }
                    });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }


            return ActionResult.Success;

        }

        [CustomAction]
        public static ActionResult CustomAction4(Session session)
        {
            
            getInformationForInstallation(session);
          
            imagePath = session["IMGPATH"];


            byte[] img = getImageData(imagePath);

            if (libraryId == 0)
            {

                if (registerLibrary(name, area, URL, installationType))
                {
                    libraryId = getLibraryID(area, name);
                    session["LIBID"] = libraryId.ToString();
                    createGlobalSetup(days, ageRestriction, controlPanel, Cmd, img, libraryId);
                    setupResource = 0;
                   
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("There has been a problem in register the library");

                }

               
            }

            
            resourceId = createStationID(libraryId, getMacAdd());
            bool registeredStation=getSOAPClient().checkIfRegistered(libraryId, resourceId);
           
            if ( registeredStation && installationType.Equals("Manual"))// check if resource registered allready
            {

                System.Windows.Forms.MessageBox.Show("This resource allready exists and will be updated with selected setup");

            }
            else if (registeredStation && installationType.Equals("Network"))
            {
                System.Windows.Forms.MessageBox.Show("This is a new image of a system and global configurations will be used");
            }
            else
            {
                if (installationType.Equals("Manual"))
                {
                    if (session["Edit"].Equals("1"))
                    {                        
                        setupResource = getStationSetup(controlPanel, Cmd, days, ageRestriction);
                    }
                    else if (session["Edit"].Equals("0"))
                    {                        
                        setupResource = 0;
                    }
                }
                else if (installationType.Equals("Network"))
                {
                   
                    if (pcName.Equals(string.Empty))
                    {                        
                        pcName = getStationName(libraryId);
                        setupResource = 0;
                    }
                    
                }
                registerStation(resourceId, "Station", libraryId, pcFunctions, adult, true, pcName, setupResource);  
            }
        
            return ActionResult.Success;
        }

        private static string getStationName(long libraryId)
        {
            string stationName = getSOAPClient().getStationName(libraryId);
            if (stationName.Equals("404"))
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in finding a name for the resource and it will be called 404");
            }
            return stationName;

        }

        [CustomAction]
        public static ActionResult CustomAction5(Session session)
        {

            installationPath = session.CustomActionData["INSTALLFOLDER"];
            string lib = session.CustomActionData["LIBID"];

            try
            {
                string libFileRead = installationPath + "Library.txt";

                System.IO.File.Create(libFileRead).Close();
                StreamWriter sw = new StreamWriter(libFileRead, false);
                sw.WriteLine(lib);
                sw.Flush();
                sw.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex + "");
            }
            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult CustomAction6(Session session)
        {
            session["Edit"] = "1";
            return ActionResult.Success;
        }



        private static long getLibraryID(string libArea, string libName)
        {
           
            try
            {               
                return getSOAPClient().getLibraryID(libArea, libName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex + "???");
            }

            return -1;
        }

        private static void registerStation(string resourceID, string resourceType, long libraryID, string funct, bool adult, bool available, string name, int setup)
        {
            try
            {
                

                getSOAPClient().registerStation(resourceID, resourceType, libraryID, funct, adult, available, name, setup);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex + "...");
            }


        }


        private static CustomAction.ServiceReference1.Setup getGlobalSetupForLibrary(long libraryId)
        {
            
            return getSOAPClient().getGlobalSetup(libraryId);

        }

        private static bool registerLibrary(string libName, string libArea, string connectionString, string installationType)
        {
            try
            {
               
                return getSOAPClient().registerLibrary(libName, libArea, connectionString, installationType);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Register Library" + ex);
            }
            return false;
        }

        private static string getMacAdd()
        {

            string macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                              where nic.OperationalStatus == OperationalStatus.Up
                              select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            return macAddr;

        }

        private static bool CheckConnection()
        {
            WebClient client = new WebClient();
            try
            {
                using (client.OpenRead("http://www.google.com"))
                {
                }
                System.Windows.Forms.MessageBox.Show("You have connection to the internet");
                return true;
            }
            catch (WebException)
            {
                System.Windows.Forms.MessageBox.Show("You do not  have connection to the internet");
                return false;
            }
        }

        private static string createStationID(long p, string macAdd)
        {
            StringBuilder resourceID = new StringBuilder();
            resourceID.Append(p);
            resourceID.Append(macAdd);
            return resourceID.ToString();
        }

        private static int getStationSetup(bool controlPanel, bool Cmd, int daysInAdvance, int ageRestriciton)
        {
            int setup = 0;
            try
            {

                setup = getSOAPClient().getStationSetup(controlPanel, Cmd, daysInAdvance, ageRestriciton);

                if (setup == 0)
                {
                    getSOAPClient().createSetup(controlPanel, Cmd, daysInAdvance, ageRestriciton);
                }
                setup = getSOAPClient().getStationSetup(controlPanel, Cmd, daysInAdvance, ageRestriciton);
                return setup;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex + "??");
            }

            return setup;
        }

        private static void getGlobalSetup(long libraryID)
        {

            
            CustomAction.ServiceReference1.Setup setup = getSOAPClient().getGlobalSetup(libraryId);


        }

        private static byte[] getImageData(string path)
        {

            byte[] imageData = null;
            try
            {

                if (checkIfPicture(Path.GetExtension(path)))
                {
                    imageData = File.ReadAllBytes(path);

                    return imageData;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Parse Image" + ex);
                return null;
            }


            return imageData;
        }

        private static bool checkIfPicture(string ext)
        {
            if (ImageExtensions.Contains(ext.ToUpper()))
            {

                return true;
            }


            return false;
        }

        private static void initiate1(Session session)
        {
            area = session["area"];
            name = session["name"];


            if (area.Equals(string.Empty) || name.Equals(string.Empty))
            {
                System.Windows.Forms.MessageBox.Show("Please  press back and fill out the area and name of the library");
            }
            else
            {
                libraryId = getLibraryID(area, name);
                session["Library_ID"] = Convert.ToString(libraryId);

                
                if (libraryId != 0)
                {
                   
                    setupObject = getGlobalSetupForLibrary(libraryId);
                  
                    session["Connection"] = getConnectionString(libraryId);
                    session["Setup_Text"] = "The global setting are ";





                    if (setupObject.ControlPanel)
                    {

                        session["Control_Panel"] = "Control panel disabled ";
                        session["Control_Panel_Status"] = "true";
                    }
                    else
                    {
                        session["Control_Panel"] = "Control panel enabled";
                        session["Control_Panel_Status"] = "false";
                    }
                    if (setupObject.CmdStatus)
                    {
                        session["Cmd_Status"] = "Command prompt disabled ";
                        session["CommandPrompt_Status"] = "true";
                    }
                    else
                    {
                        session["Cmd_Status"] = "Command prompt enabled";
                        session["CommandPrompt_Status"] = "false";
                    }

                    days = setupObject.PrebookingDays;
                    session["Preebooking_day"] = "Preebooking days : " + days;
                    session["Preebooking_Day_No"] = days.ToString();
                    session["preebooking"] = days.ToString();

                    ageRestriction = setupObject.AgeRestriction;
                    session["Age_rest"] = "Age restriction : " + ageRestriction;
                    session["Age_Rest_No"] = ageRestriction.ToString();
                    session["age"] = ageRestriction.ToString();
                              
                    
                    session["Edit"] = "0";

                }
                else if (libraryId != 0 && installationType.Equals("Network"))
                {
                    session["Edit"] = "0";

                }
                else
                {
                    session["Edit"] = "1";
                }

                session["URLSERVICE"] = session["Connection"];
            }

            



        }

        private static void initiate2(Session session)
        {
            
            if (session["Edit"].Equals("1"))
            {
                
                if (getControlPanelStatus(session) == 0)
                {
                    
                    session["Control_Panel_Status"] = "false";
                }

                else
                {
                    
                    session["Control_Panel_Status"] = "true";
                }

                
                if (getCmdStatus(session)== 0)
                {
                   
                    session["CommandPrompt_Status"] = "false";
                }
                else
                {
                   
                    session["CommandPrompt_Status"] = "true";
                }
                try
                {
                   
                    session["Preebooking_Day_No"] = session["preebooking"];
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Please fill out the days field with numeric" + ex);
                }

                try
                {
                    
                    session["Age_Rest_No"] = session["age"];
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Please fill out the age field with numeric" + ex);
                }
            }
            


        }

        private static string getConnectionString(long libraryId)
        {

            return getSOAPClient().getConnectionString(libraryId);

        }

        private static CustomAction.ServiceReference1.PrinchService1SoapClient getSOAPClient()
        {
            Binding binding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress("http://52.38.37.58/PrinchService1.asmx?WSDL");
            CustomAction.ServiceReference1.PrinchService1SoapClient soap = new CustomAction.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
            return soap;
        }

        private static bool createGlobalSetup(int preebookingDays, int ageRestriction, bool controlPanel, bool cmdStatus, byte[] image, long libraryID)
        {
            return getSOAPClient().createGlobalSetup(preebookingDays, ageRestriction, controlPanel, cmdStatus, image, libraryId);
        }

        private static int  getInstallationType(Session session)
        {
            return Int32.Parse(session["InstallType_Prop"]);
        }

        private static int getControlPanelStatus(Session session)
        {
            return Int32.Parse(session["controlPanelStatus_Prop"]);
        }

        private static int getCmdStatus(Session session)
        {
            return Int32.Parse(session["lblRadioButton_Prop"]);
        }

        private static bool isAdult(Session session)
        {
            int adult = Int32.Parse(session["AdultRadio_Prop"]);
            if (adult == 0)
            {
                return false;
            }else{
                return true;
            }
        }

        private static void getInformationForInstallation(Session session)
        {
            URL = session["URLSERVICE"];
            name = session["name"];
            area = session["area"];
            if(getInstallationType(session)==0){
                installationType = "Manual";                
                pcName = session["pcName"];
                pcFunctions = session["function"];
                adult = isAdult(session);
            }
            else
            {
                installationType = "Network";
                pcName = string.Empty;
                pcFunctions = string.Empty;
                adult = true;
            }
            days=Int32.Parse(session["Preebooking_Day_No"] );
            ageRestriction=Int32.Parse(session["Age_Rest_No"]); 
            controlPanel=Convert.ToBoolean(session["Control_Panel_Status"]);
            Cmd = Convert.ToBoolean(session["CommandPrompt_Status"]);
            libraryId = Int64.Parse(session["Library_ID"]);
            System.Windows.Forms.MessageBox.Show(installationType + "" + pcName + "" + pcFunctions + name + ">>" + Cmd + controlPanel+libraryId);
        }
    }
}
