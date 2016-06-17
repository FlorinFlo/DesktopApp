using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartLogIn
{
    class Program
    {
        static void Main(string[] args)
        {

          //  Console.WriteLine(readExecution());
           // Console.ReadKey();
            RunElevated(readExecution());
        }

        private static bool RunElevated(string fileName)
        {

            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception)
            {
                //Do nothing. 
            }
            return false;
        }

        private static string getPath()
        {
            string c=Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ;
            return Path.Combine(c, "loginPath.txt");
        }

        private static string readExecution()
        {   string path="";
            try
            {
               
                StreamReader sw = new StreamReader(getPath());
                path = sw.ReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "");
            }
            


           return path;
        }

    }
}
