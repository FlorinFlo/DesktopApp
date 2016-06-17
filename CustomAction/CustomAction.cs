using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            session.Log("Begin CustomAction1");
            MessageBox.Show("Custom1");
            //Sourcedir=  //targetdir
            string targetDir = session["INSTALLFOLDER"];
            string sourceDir = targetDir + "LogIn.exe";

           string path = targetDir + "loginPath.txt";

            System.IO.File.Create(path).Close();
            StreamWriter sw = new StreamWriter(path, false);
            sw.WriteLine(sourceDir);
            sw.Flush();
            sw.Close();
            return ActionResult.Success;
        }

        //modify reg
        [CustomAction]
        public static ActionResult CustomAction2(Session session)
        {
            string targetDir = session["INSTALLFOLDER"];
            string sourceDir = targetDir + "LogIn.exe";
            
                bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator) ? true : false;
                if (isAdmin)
                {
                    MessageBox.Show("you are an administrator");
                }
                else
                {
                    MessageBox.Show("You are not an administrator");
                }

                //check if is 64bit OS
                if (System.Environment.Is64BitOperatingSystem)
                {
                    RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                    localKey = localKey.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);

                    try
                    {
                        // MessageBox.Show("Modifie reg" + localKey.Name + localKey.GetSubKeyNames() + localKey.GetValue("Userinit"));
                        localKey.SetValue("Userinit", sourceDir, RegistryValueKind.String);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex + " ?????????");
                    }
                }
                else
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", RegistryKeyPermissionCheck.ReadWriteSubTree)) //must dispose key or use "using" keyword
                    {
                        if (key != null)  //must check for null key
                        {
                            RegistrySecurity regSecurity = key.GetAccessControl();

                            // MessageBox.Show("Modifie reg" + key.Name + key.GetSubKeyNames() + key.GetValue("Userinit"));
                            try
                            {
                                key.SetValue("Userinit", sourceDir, RegistryValueKind.String);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex + " ?????????");
                            }


                        }
                    }

                }




            
            return ActionResult.Success;
        }
    }
}
