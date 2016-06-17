
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.Security.Principal;
using System.Threading;
using System.Reflection;
using LogIn.Service;
using System.IO;
using System.Windows;
using LogIn.ServiceReference1;
using LogIn.Model;
using Microsoft.Win32;
using System.Runtime.InteropServices;



namespace LogIn
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {

        private Service.Service service = Service.Service.InstanceService;
        private static long libID = 0;
        private static string resourceID = string.Empty;

        private BrushConverter bc = new BrushConverter();
        private static GlobalKeyboardHook gkh = new GlobalKeyboardHook();
        private BackgroundWorker bw = new BackgroundWorker();
        private System.Threading.Timer timer;
        private static TimeSpan timeToLogIn = new TimeSpan(0, 15, 0);
        private System.Windows.Forms.Timer timerToLogIn;
        private int min = 0, sec = 0;
        private static LogIn.ServiceReference1.Booking booking = null;
        private static ServiceReference1.Credentials knownUser = null;

       
        private static bool REGISTERED = false;
        private static bool AVAILABLE = false;

        private static bool CONTROLPANEL_STATUS = false;
        private static bool CMD_STATUS = false;
        private static int AGE_RESTRICTION = 12;
        private static string QUICK_BOOK = "QUICK_BOOK";
        private static TimeSpan TIME_TO_NEXT_BOOKING=new TimeSpan(0,0,0);





        
        public MainWindow()
        {



            InitializeComponent();
            CheckConnection();
            initiate();

            toggleTsk(false);
            

            pasBox.KeyDown += pasBox_KeyDown;

            //Timercallback  each minute
            timer = new System.Threading.Timer(TimerCallback, null, 0, 100000);
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;

            //Add hock windows key for shortcut combinations with win-key
            gkh.HookedKeys.Add(Keys.LWin);
            gkh.KeyDown += new System.Windows.Forms.KeyEventHandler(gkh_KeyDown);


            if (booking != null)
            {
                loginAsUser(service.getTimeRemaining(booking));
            }

        }

        private void initiateTimer()
        {
            min = timeToLogIn.Minutes;
            sec = timeToLogIn.Seconds;
        }

        private void timerToLogIn_Tick(object sender, EventArgs e)
        {
            if (min == 0 && sec == 0)
            {

                timermin.Content = "00";
                timersec.Content = "00";
            }
            else
            {

                if (sec < 1)
                {
                    sec = 59;
                    if (min < 1)
                    {
                        min = 0;

                    }
                    else
                    {
                        if (min == 1 && sec == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("Youre booking will be deleted");
                        }
                        min -= 1;
                    }
                }
                else
                {

                    sec -= 1;
                }



                if (min > 9)
                    timermin.Content = min.ToString();
                else
                    timermin.Content = "0" + min.ToString();
                if (sec > 9)
                    timersec.Content = sec.ToString();
                else
                    timersec.Content = "0" + sec.ToString();
            }
        }

        private void gkh_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void TimerCallback(object state)
        {
            if (!bw.IsBusy)
                bw.RunWorkerAsync();
        }
        //change color blue(available) #169ED9 orange(booked) #FFFFA500
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (REGISTERED)
            {
                if (AVAILABLE)
                {

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        this.Background = (Brush)bc.ConvertFrom("#169ED9");
                    }

                    ));
                }
                
                else
                {

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        this.Background = (Brush)bc.ConvertFrom("#FFFFA500");
                    }

                    ));
                }
            }



        }
        // Check if new booking in background
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (REGISTERED)
            {
                checkAvailability();
                
            }
            else if (bw.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            LogIn();

        }


        //Start user initiation Windows
        public void startUser()
        {
            System.Diagnostics.Process.Start(@"C:\Windows\system32\userinit.exe");
        }

        private bool CheckConnection()
        {
            WebClient client = new WebClient();
            try
            {
                using (client.OpenRead("http://www.google.com"))
                {
                }

                return true;
            }
            catch (WebException)
            {
                System.Windows.MessageBox.Show("You do not  have connection to the internet");
                return false;
            }
        }
        // prevent from closing application   
        private void Window_Closing(object sender, CancelEventArgs e)
        {

            e.Cancel = true;
        }

        private void pasBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogIn();
            }
        }

        private void LogIn()
        {
            LogIn.Model.User user = new LogIn.Model.User(TextBox.Text, pasBox.Password);

            if (service.LogIn(user, knownUser, booking).Item1)
            {

                loginAsUser(service.LogIn(user, knownUser, booking).Item2);
            }
            else if (user.UserName.Equals("admin") && (user.UserPass).Equals("admin"))
            {
                service.toggleTaskManager(true);
                service.toggleControlPanel(true);
                service.toggleCMD(true);
                bw.CancelAsync();
                startUser();
                this.Hide();
                System.Windows.Forms.Application.Exit();
            }
            else if (service.validateUser(user.UserName, user.UserPass, libID) != null)
            {
                System.Windows.Forms.MessageBox.Show("You will have access later :)" + service.validateUser(user.UserName, user.UserPass, libID).UserBirth);
                //loginAsUser()
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Access denied");
            }


        }

        private void initiate()
        {
            try
            {
                libID = service.readInfo();
                resourceID = service.getMyID(libID, service.getMacAddress());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("The system crashed in attemp to find Identifier");
            }
            

            


            //if not registered is part of network installation use global configurations and default configurations
            if (!service.checkIfRegistered(libID, resourceID))
            {
                string pcName = service.getNameForStation(libID);
                if (service.registerStation(resourceID, "Station", libID, string.Empty, true, true, pcName, 0))
                {
                    REGISTERED = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("There has been a problem in attemp to register the station");
                }

            }

           if(REGISTERED)
            {
                REGISTERED = true;
                checkAvailability();
            }
                      
            service.getSettings(resourceID,libID);
           


            CONTROLPANEL_STATUS = service.controlPanelStatus();
            CMD_STATUS = service.cmdStatus();
            AGE_RESTRICTION = service.getAgeRestriction();


        }


        private void checkAvailability()
        {
            
            booking = service.checkIfAvailable(libID, resourceID);

            if (booking == null)
            {
                AVAILABLE = true;
               
            }
            else
            {
                try
                {
                    // if more than two hours returns 2 hours
                TIME_TO_NEXT_BOOKING= service.getTimeToBooking(booking);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("There has been a problem in calculating the time to the next booking");
                }
               


                if (TIME_TO_NEXT_BOOKING.Minutes == -15)
                {
                    System.Windows.Forms.MessageBox.Show("In 15 minutes a new booking will start ");
                }
                else if (TIME_TO_NEXT_BOOKING.Minutes == -30)
                {
                    System.Windows.Forms.MessageBox.Show("In 30 minutes a new booking will start ");
                }
                else if (TIME_TO_NEXT_BOOKING.Minutes > -10)// preebooking prevented 10 min before start of other booking
                {
                    btnStart.Visibility = Visibility.Hidden;
                    knownUser = service.getUserFBooking(booking, libID);

                    if (TIME_TO_NEXT_BOOKING.Minutes > -1)
                    {
                        timerPanel.Visibility = Visibility.Visible;
                        timerToLogIn = new System.Windows.Forms.Timer();
                        timerToLogIn.Interval = 1000;
                        timerToLogIn.Tick += new EventHandler(timerToLogIn_Tick);
                        initiateTimer();
                        timerToLogIn.Start();
                    }
                    AVAILABLE = false;
                }

            }


        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            checkAvailability();
            if (AVAILABLE)
            {
                TimeSpan timeToBooking=service.getTimeToBooking(booking);
                var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                DateTime startTime = DateTime.UtcNow + offset;                
                DateTime endTime=startTime.Add(timeToBooking);

                if (service.createDirectBooking(libID, QUICK_BOOK, string.Empty, resourceID,startTime,endTime))
                {
                    loginAsUser(service.getTimeToBooking(booking));
                }

            }

        }

        private void loginAsUser(TimeSpan time)
        {
            service.toggleControlPanel(CONTROLPANEL_STATUS);
            service.toggleCMD(CMD_STATUS);
            startUser();
            bw.CancelAsync();
            this.Hide();            
            System.Windows.Window window = new TimerBox(time);
            window.Show();
        }

         
        private void toggleTsk(bool enable)
        {
           
            service.toggleTaskManager(enable);

        }
       

        
    }
}
