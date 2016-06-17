
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.Win32;
using System.Security.Permissions;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Net.NetworkInformation;
using LogIn.ServiceReference1;
using LogIn.Model;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


using System.Data;
using System.Drawing;
using System.Security.Cryptography;


namespace LogIn.Service
{
    class Service
    {
        private static Service instanceService;
        //private static TimeSpan time;        
        private static LogIn.ServiceReference1.Setup setup = null;
        private static LogIn.ServiceReference1.Booking booking1 = null;
        private static Binding binding = new BasicHttpBinding();
        private static EndpointAddress endpointAddress = new EndpointAddress("http://52.38.37.58/PrinchService1.asmx?WSDL");

        private Service()
        {

        }

        internal static Service InstanceService
        {
            get
            {
                if (instanceService == null)
                {
                    instanceService = new Service();
                }

                return instanceService;
            }
        }

        internal Tuple<bool, TimeSpan> LogIn(LogIn.Model.User userAtComputer, ServiceReference1.Credentials knownUser, LogIn.ServiceReference1.Booking booking)
        {
            TimeSpan time = new TimeSpan(0, 0, 0);

            if (knownUser != null)
            {
                if (isSameUser(userAtComputer, knownUser))
                {

                    return Tuple.Create<bool, TimeSpan>(true, getTimeForLogIn(booking));
                }
            }


            return Tuple.Create<bool, TimeSpan>(false, time);
        }

        internal void toggleTaskManager(bool enable)
        {

            if (enable)
            {
                try
                {


                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System!DisableTaskMgr", "0", RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Trying to enable task manager has failed");
                }

            }
            else
            {
                try
                {

                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System!DisableTaskMgr", "1", RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Trying to disable task manager has failed");
                }

            }

        }

        internal void toggleCMD(bool enable)
        {

            if (enable)
            {
                try
                {
                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Policies\\Microsoft\\Windows\\System!DisableCMD", "0", RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Trying to enable Command prommpt has failed");
                }

            }
            else
            {
                try
                {
                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Policies\\Microsoft\\Windows\\System!DisableCMD", "1", RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Trying to disable Command prommpt  has failed");
                }

            }

        }

        internal void toggleControlPanel(bool enable)
        {
            if (enable)
            {
                try
                {
                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Programs!NoProgramsCPL", "0", RegistryValueKind.DWord);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Trying to enable Control pannel has failed");
                }


            }
            else
            {
                try
                {
                    GroupPoliciesMod.ComputerGroupPolicyObject.SetPolicySetting("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Programs!NoProgramsCPL", "1", RegistryValueKind.DWord);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Trying to disable Control pannel has failed");
                }


            }

        }


        internal long readInfo()
        {
            long id = 0;
            try
            {
                string executionPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string libIDPath = Path.Combine(executionPath, "Library.txt");
                StreamReader sr = new StreamReader(libIDPath);

                id = Int64.Parse(sr.ReadLine());
                sr.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Trying to read info about pc has failed");
            }

            return id;

        }

        internal string getMacAddress()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Trying to read pc address has failed");
            }
            return string.Empty;

        }

        internal string getMyID(long libraryID, string macAddress)
        {
            string myId = libraryID + macAddress;

            return myId;

        }

        internal ServiceReference1.Credentials getUserFBooking(LogIn.ServiceReference1.Booking booking, long libId)
        {
            if (booking != null)
            {

                if (!booking.ClientPass.Equals(string.Empty))
                {

                    LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                    return soap.getUserForBooking(booking.BookingID, libId);
                }

            }
            return null;

        }

        internal bool isSameUser(LogIn.Model.User userAtPC, ServiceReference1.Credentials theUser)
        {
            byte[] hashSalt = Convert.FromBase64String(theUser.ClientPass);
            return validate(userAtPC.UserName, hashSalt, theUser.ClientID);
        }

        internal TimeSpan getTimeForLogIn(LogIn.ServiceReference1.Booking booking)
        {
            DateTime now = DateTime.Now;
            TimeSpan t = now.Subtract(booking.BookingEnd);
            return t;

        }

        internal TimeSpan getTimeToBooking(ServiceReference1.Booking booking)
        {
            if (booking != null)
            {
                DateTime now = DateTime.Now;

                return now - booking.BookingStart;
            }
            else
            {
                return new TimeSpan(2, 0, 0);
            }

        }

        internal bool createDirectBooking(long libraryId, string clienId, string clientPass, string resourceId, DateTime startDate, DateTime endDate)
        {
            try
            {
                LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                return soap.createBooking(libraryId, clienId, clientPass, resourceId, startDate, endDate);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in creating a direct booking");
            }

            return false;
        }

        internal LogIn.ServiceReference1.Setup getSettings(string resourceId,long libraryID)
        {
            int setupId = 0;
            try
            {
                LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                setupId = soap.getSetupId(resourceId);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in geting the settings");
            }

            if (setupId == 0)
            {
                try
                {
                    LogIn.ServiceReference1.PrinchService1SoapClient soap1 = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                    setupId = soap1.getGlobalSetupId(libraryID);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Get global settings" + ex);
                }
                
            }

            try
            {


                LogIn.ServiceReference1.PrinchService1SoapClient soap1 = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                setup = soap1.getSetup(setupId);
                


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Get settings" + ex);
            }


            if (setup == null)
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in reading the congiguration of this pc Default configurations will be used");
               
            }
            return setup;
        }

        internal int getAgeRestriction()
        {
            if (setup != null)
            {
                return setup.AgeRestriction;
            }
            else
            {
                //Default
                return 12;
            }
        }

        internal bool controlPanelStatus()
        {
            if (setup != null)
            {
                return setup.ControlPanel;
            }
            else
            {
                //Default
                return false;
            }
        }

        internal bool cmdStatus()
        {
            if (setup != null)
            {
                return setup.CmdStatus;
            }
            else
            {
                //Default
                return false;
            }
        }

        internal bool checkIfRegistered(long libID, string resourceID)
        {
            try
            {
                LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);

                return soap.checkIfRegistered(libID, resourceID);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in checking if this pc is registered to the system");
            }
            return false;
        }
        //returns booking if time difference -15 minutes until start of other booking or time.now-time_booking_end=>0
        internal LogIn.ServiceReference1.Booking checkIfAvailable(long libID, string resourceID)
        {
            try
            {

                DateTime now = DateTime.Now;
                
                LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                booking1 = soap.checkIfAvailable(libID, resourceID, now);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Check if available??" + ex);
            }


            return booking1;
        }

        internal LogIn.ServiceReference1.Booking getBooking()
        {
            if (booking1.BookingID.Equals(string.Empty))
            {
                return null;
            }
            return booking1;
        }

        internal ServiceReference1.User validateUser(string user, string password, long libraryId)
        {
            try
            {
                LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
                return soap.validateUser(user, password, libraryId);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There has been a problem in validation of the user");
            }

            return null;
        }


        internal TimeSpan getTimeRemaining(ServiceReference1.Booking booking)
        {
            DateTime dateNow = DateTime.Now;
            TimeSpan time = dateNow - booking.BookingEnd;
            TimeSpan returnTime = new TimeSpan(Math.Abs(time.Hours), Math.Abs(time.Minutes), Math.Abs(time.Seconds));
            return returnTime;
        }

        internal string GetHAsh(string plainText, byte[] salt)
        {

            if (salt == null)
            {
                RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

                int max_lenght = 32;

                salt = new byte[max_lenght];
                random.GetNonZeroBytes(salt);
            }
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

            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);


            return Convert.ToBase64String(hashBytes);
        }

        internal bool validate(string plainText, byte[] hashSalt, string hash)
        {
            //SHA1           
            byte[] hashWithSaltBytes = Convert.FromBase64String(hash);
            byte[] saltBytes = hashSalt;
            string expectedHash = GetHAsh(plainText, hashSalt);
            return (hash == expectedHash);

        }

        internal bool registerStation(string resourceID, string resourceType, long libraryID, string funct, bool adult, bool available, string name, int setup)
        {
            LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
            return soap.registerStation(resourceID, resourceType, libraryID, funct, adult, available, name, setup);
        }
        
        internal string getNameForStation(long libraryID)
        {
            LogIn.ServiceReference1.PrinchService1SoapClient soap = new LogIn.ServiceReference1.PrinchService1SoapClient(binding, endpointAddress);
            return soap.getStationName(libraryID);
        }
    }
}
