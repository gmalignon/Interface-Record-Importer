using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InterfaceRecordImporter
{
    /// <summary>
    /// Interaction logic for Window_Settings.xaml
    /// </summary>
    public partial class Window_Settings : Window
    {
        private bool WasRunning = false;

        public Window_Settings()
        {
            InitializeComponent();
            
            this.Dispatcher.Invoke(() =>
            {
                if (((MainWindow)Application.Current.MainWindow).Status)
                {
                    ((MainWindow)Application.Current.MainWindow).StartProcess();
                    WasRunning = true;
                }

                TextBox_SQLServer.Text = GlobalSettings.MyAppSettings.ServerIP;
                TextBox_AmadeusFileLocation.Text = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                TextBox_SabreFileLocation.Text = GlobalSettings.MyAppSettings.SabreFilesFolderLocation;
                TextBox_ApolloFileLocation.Text = GlobalSettings.MyAppSettings.ApolloFilesFolderLocation;
                TextBox_WorldspanFileLocation.Text = GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation;
                TextBox_DuplicateFileLocation.Text = GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation;
            });

            this.Closing += new CancelEventHandler(MainWindow_Closing);
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            //Your code to handle the event
            this.Dispatcher.Invoke(() =>
            {
                this.Owner.IsEnabled = true;
                if (WasRunning)
                {
                    ((MainWindow)Application.Current.MainWindow).SetupFolders();
                    ((MainWindow)Application.Current.MainWindow).SetupFolderWatchers();
                    ((MainWindow)Application.Current.MainWindow).StartProcess();
                    ((MainWindow)Application.Current.MainWindow).Activate();
                }
            });
        }
        private void Button_AmadeusFileLocation_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
            dlg.ShowNewFolderButton = true;
            
            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;
                //TextBox_AmadeusFileLocation.
                this.Dispatcher.Invoke(() =>
                {
                    // TextBox_AmadeusFileLocation.Text = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                    //GlobalSettings.MyAppSettings.SabreFilesFolderLocation = path;
                    TextBox_AmadeusFileLocation.Text = path;
                });
               
                //this
            }
        }
        private void Button_SabreFolderLocation_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = GlobalSettings.MyAppSettings.SabreFilesFolderLocation;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {   string path = dlg.SelectedPath;
                //TextBox_SabreFileLocation.Text = path;
                //GlobalSettings.MyAppSettings.SabreFilesFolderLocation = path;
                this.Dispatcher.Invoke(() =>
                {
                    TextBox_SabreFileLocation.Text = path;
                });
            }
        }

        private void Button_ApolloFolderLocation_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = GlobalSettings.MyAppSettings.ApolloFilesFolderLocation;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;
                //TextBox_SabreFileLocation.Text = path;
                //GlobalSettings.MyAppSettings.SabreFilesFolderLocation = path;
                this.Dispatcher.Invoke(() =>
                {
                    TextBox_ApolloFileLocation.Text = path;
                });
            }
        }


        private void Button_WorldspanFolderLocation_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;
                //TextBox_SabreFileLocation.Text = path;
                //GlobalSettings.MyAppSettings.SabreFilesFolderLocation = path;
                this.Dispatcher.Invoke(() =>
                {
                    TextBox_WorldspanFileLocation.Text = path;
                });
            }
        }

        private void Button_DuplicateFolderLocation_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.SelectedPath = GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation;
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == true)
            {
                string path = dlg.SelectedPath;
                //TextBox_SabreFileLocation.Text = path;
                //GlobalSettings.MyAppSettings.SabreFilesFolderLocation = path;
                this.Dispatcher.Invoke(() =>
                {
                    TextBox_DuplicateFileLocation.Text = path;
                });
            }
        }

        private void Button_Try_Click(object sender, RoutedEventArgs e)
        {
            //Properties.Settings.Default.AmadeusFilesFolderLocation= TextBox_AmadeusFileLocation.Text;
           
            GlobalSettings.MyAppSettings.ServerIP = TextBox_SQLServer.Text;
            GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation = TextBox_AmadeusFileLocation.Text;
            GlobalSettings.MyAppSettings.SabreFilesFolderLocation = TextBox_SabreFileLocation.Text;
            GlobalSettings.MyAppSettings.ApolloFilesFolderLocation = TextBox_ApolloFileLocation.Text;
            GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation = TextBox_WorldspanFileLocation.Text;
            GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation = TextBox_DuplicateFileLocation.Text;

            ManageSettings.StoreSettings();
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_AmadeusFolderLocationClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_AmadeusFileLocation.Text = "";
        }

        private void Button_SabreFolderLocationClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_SabreFileLocation.Text = "";
        }

        private void Button_ApolloFolderLocationClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_ApolloFileLocation.Text = "";
        }

        private void Button_WorldspanFolderLocationClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_WorldspanFileLocation.Text = "";
        }

        private void Button_DuplicateFolderLocationClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DuplicateFileLocation.Text = "";
        }
    }
}
