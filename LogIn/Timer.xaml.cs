using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace LogIn
{
    /// <summary>
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class TimerBox : Window
    {
        private TimeSpan timer;
        private int hours,min,sec;
        private System.Windows.Forms.Timer timer1;
        
        public TimerBox(TimeSpan time)
        {
            InitializeComponent();
            
            this.Topmost = true;
            this.Top = 0;
            this.Left = 0;
            timer = time;
            timer1 =new  System.Windows.Forms.Timer();
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(timer1_Tick);
            initiate();
            timer1.Start();
            
            
        }
        
        public void timer1_Tick(object o, EventArgs sender)
        {
            if (min == 0 && hours == 0 && sec == 0)
            {
                
                timerhour.Content = "00";
                timerminutes.Content = "00";
            }
            else
            {
                
                if (sec < 1)
                {
                    sec = 59;
                    if (min < 1)
                    {
                        min = 59;
                        
                        if (hours != 0)
                        {
                            hours -= 1;
                        }
                    }
                    else
                    {
                        if (min == 6 && hours==0)
                        {
                            this.Background = new SolidColorBrush(Colors.Orange);
                           
                            Window dialog = new MyDialog();
                            dialog.Show();
                        }
                        min -= 1;
                    }
                }
                else
                {                   
                     if (hours == 0 && min == 0)
                    {
                       
                        Restart();
                    }
                    sec -= 1;
                }

                if (hours > 9)
                    timerhour.Content =  hours.ToString();
                else
                    timerhour.Content = "0" + hours.ToString();
                if (min > 9)
                    timerminutes.Content = min.ToString();
                else
                    timerminutes.Content = "0" + min.ToString();
            }
            
        }

        public void initiate()
        {
            
            hours = timer.Hours;
            min = timer.Minutes;
        }
        private static void Restart()
        {
           
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -f -r";            
            Process.Start(proc);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            e.Cancel = true;
        }

        
    }
}
