using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Net.Http;

namespace InterfaceRecordImporter
{
    

    public static class ManageSettings
    {
        public static Settings ReadSettings()
        {
            
            GlobalSettings.MyAppSettings = new Settings();
            try
            {
                GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = Properties.Settings.Default.AmadeusFilesFolderLocation;
                GlobalSettings.MyAppSettings.SabreFilesFolderLocation = Properties.Settings.Default.SabreFilesFolderLocation;
                GlobalSettings.MyAppSettings.ApolloFilesFolderLocation = Properties.Settings.Default.ApolloFilesFolderLocation;
                GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation = Properties.Settings.Default.WorldspanFilesFolderLocation;
                GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation = Properties.Settings.Default.DuplicateFilesFolderLocation;

                GlobalSettings.MyAppSettings.ServerIP = Properties.Settings.Default.ServerIP;
            }
            catch (Exception ex)
            {

                GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = "";
                GlobalSettings.MyAppSettings.SabreFilesFolderLocation = "";
                GlobalSettings.MyAppSettings.ApolloFilesFolderLocation = "";
                GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation = "";
                GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation = "";
                GlobalSettings.MyAppSettings.ServerIP = "";

                GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = Properties.Settings.Default.AmadeusFilesFolderLocation;
                GlobalSettings.MyAppSettings.SabreFilesFolderLocation = Properties.Settings.Default.SabreFilesFolderLocation;
                GlobalSettings.MyAppSettings.ApolloFilesFolderLocation = Properties.Settings.Default.ApolloFilesFolderLocation;
                GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation = Properties.Settings.Default.WorldspanFilesFolderLocation;
                GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation = Properties.Settings.Default.DuplicateFilesFolderLocation;
                GlobalSettings.MyAppSettings.ServerIP = Properties.Settings.Default.ServerIP;
            }

            return GlobalSettings.MyAppSettings;

        }

        public static void ResetNewSettings()
        {
            GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = "";
            GlobalSettings.MyAppSettings.SabreFilesFolderLocation = "";
            GlobalSettings.MyAppSettings.ApolloFilesFolderLocation = "";
            GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation = "";
            GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation = "";
            GlobalSettings.MyAppSettings.ServerIP = "";
        }

        public static void StoreSettings()
        {
            try
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.AmadeusFilesFolderLocation = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                Properties.Settings.Default.SabreFilesFolderLocation = GlobalSettings.MyAppSettings.SabreFilesFolderLocation;
                Properties.Settings.Default.ApolloFilesFolderLocation = GlobalSettings.MyAppSettings.ApolloFilesFolderLocation;
                Properties.Settings.Default.WorldspanFilesFolderLocation = GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation;
                Properties.Settings.Default.DuplicateFilesFolderLocation = GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation;
                Properties.Settings.Default.ServerIP = GlobalSettings.MyAppSettings.ServerIP;
                //GlobalSettings.MyAppSettings.ServerIP= GlobalSettings.MyAppSettings.ServerIP;
                // GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                //GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                // Settings_POS.Default.SQLServer = Settings.SQLServer
                // Settings_POS.Default.SelectedPrinter = Settings.SelectedPrinter
                // Settings_POS.Default.ServerIP = Settings.ServerIP
                //Settings_POS.Default.Save()
                //Settings_POS.Default.Reload()
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }


        public static bool CheckSettings()
        {
            bool isReset = false;
            string MyPath = "";

            try
            {
                MyPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath + ".config";
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                if (!File.Exists(MyPath))
                {
                    isReset = true;
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                
                string filename = string.Empty;
                if (!string.IsNullOrEmpty(ex.Filename))
                {
                    filename = ex.Filename;
                }
                else
                {
                    dynamic innerEx = ex.InnerException as ConfigurationErrorsException;
                    if (innerEx != null && !string.IsNullOrEmpty(innerEx.Filename))
                    {
                        filename = innerEx.Filename;
                    }
                }

                if (!string.IsNullOrEmpty(filename))
                {
                    if (System.IO.File.Exists(filename))
                    {
                        dynamic fileInfo = new System.IO.FileInfo(filename);
                        dynamic watcher = new System.IO.FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);
                        System.IO.File.Delete(filename);
                        isReset = true;
                        if (System.IO.File.Exists(filename))
                        {
                            watcher.WaitForChanged(System.IO.WatcherChangeTypes.Deleted);
                        }
                    }
                }
            }

            return isReset;
        }

    }
    public class Settings
    {
       
        public string SabreFilesFolderLocation { get; set; }

        public string AmadeusFilesFolderLocation { get; set; }

        public string ApolloFilesFolderLocation { get; set; }

        public string WorldspanFilesFolderLocation { get; set; }

        public string DuplicateFilesFolderLocation { get; set; }

        public string ServerIP { get; set; }
    }
}
