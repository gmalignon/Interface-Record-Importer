using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics;

namespace InterfaceRecordImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static FileSystemWatcher AmadeusFileWatcher { get; set; }
        private static FileSystemWatcher SabreFileWatcher { get; set; }
        private static FileSystemWatcher ApolloFileWatcher { get; set; }
        private static FileSystemWatcher WorldspanFileWatcher { get; set; }

        public bool Status { get; set; } = false;
        private ImageSource ImageSourceRed = new BitmapImage(new Uri("pack://application:,,,/Icon_Red.ico", UriKind.RelativeOrAbsolute)) as ImageSource;
        private ImageSource ImageSourceBlue = new BitmapImage(new Uri("pack://application:,,,/Icon_Blue.ico", UriKind.RelativeOrAbsolute)) as ImageSource;
        private ImageSource ImageSourceGreen = new BitmapImage(new Uri("pack://application:,,,/Icon_Green.ico", UriKind.RelativeOrAbsolute)) as ImageSource;

        private static string VersionNumber { get; set; } = "";
        private static string LastModified { get; set; } = "";
        // Checks for update once a day
        private System.Timers.Timer AppUpdateTimer { get; set; }

        // Checks folder every minute
        private System.Timers.Timer FolderTimer { get; set; }
        // Cleans up all folders, once a day
        private System.Timers.Timer CleanupTimer { get; set; }

        private int NumberOfDays { get; set; } = 7;

        private bool AmadeusFolderChanged { get; set; } = false;
        private bool SabreFolderChanged { get; set; } = false;
        private bool ApolloFolderChanged { get; set; } = false;
        private bool WorldspanFolderChanged { get; set; } = false;

        private bool AmadeusFolderCompleted { get; set; } =true;
        private bool SabreFolderCompleted { get; set; } = true;
        private bool ApolloFolderCompleted { get; set; } = true;
        private bool WorldspanFolderCompleted { get; set; } = true;

        private bool Processing { get; set; } = false;

       // private ProcessFolder MyProcessFolder=null;
        public   MainWindow()
        {
            try
            {
                InitializeComponent();
                // Checks if app is already running
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    MessageBox.Show("An instance is already running! Exit it and try again.");
                    Application.Current.Shutdown();
                }

               // TestClient test = new TestClient();
                //test.GetPNRList();
                //test.GetCommission();

                //this.OnClosed
                this.Closed += new EventHandler(MainWindow_Closed);
                this.ContentRendered += new EventHandler(Window_Activated);
                //TasbarIcon.LeftClickCommandTarget=
                TasbarIcon.TrayLeftMouseDown += new RoutedEventHandler(MouseDownHandler);
                this.MouseDown += new MouseButtonEventHandler(Window_MouseDown);
                VersionNumber = AppUpdate.GetProductVersion();
                LastModified = AppUpdate.GetLastModifiedDate();

                //this.Closing += new CancelEventHandler(MainWindow_Closing);

                this.Dispatcher.Invoke(() =>
                {
                    Label_ProductInfo.Content = "Version: v" + VersionNumber + "\n" + LastModified;
                });

                GlobalSettings.MyAppSettings = ManageSettings.ReadSettings();
                //GlobalSettings.MyAppSettings.ServerIP = "http://localhost:33193/";
                if (GlobalSettings.MyAppSettings.ServerIP == "")
                {GlobalSettings.MyAppSettings.ServerIP = "https://Atlas.SmartAgent.nyc/";}
                
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x53).ToCharArray()[0]);  
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x6D).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x40).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x72).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x74).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x40).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x67).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x33).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x6E).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x74).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x39).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x31).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x33).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x40).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x2B).ToCharArray()[0]);
                GlobalSettings.PassPhrase.AppendChar(Char.ConvertFromUtf32(0x2B).ToCharArray()[0]);
                GlobalSettings.PassPhrase.MakeReadOnly();


                // Checks for new version
                //UpdateStatus("Checking for Update...");
                //AppUpdate.CheckForNewVersion();
                //UpdateStatus("Done Checking for Update.");



                // first cleanup when starting
               // Cleanup(NumberOfDays);

                SetupFolders();
                SetupFolderWatchers();

                

               


                // Starts update timer
                // warning disabled
                AppUpdateTimer = new System.Timers.Timer();
                AppUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(onTimedUpdateEvent);
                AppUpdateTimer.Interval = 1000 * 60 * 60 * 1; // 1 Hours
                                                              //AppUpdateTimer.Interval = 30000; // 1 minute
                AppUpdateTimer.AutoReset = true;
               // AppUpdateTimer.Start();

                // Starts Cleanup Timer
                // Starts update timer
                CleanupTimer = new System.Timers.Timer();
                CleanupTimer.Elapsed += new System.Timers.ElapsedEventHandler(onTimedCleanupEvent);
                CleanupTimer.Interval = 1000 * 60 * 60 * 24; // Every 24 hours
                                                              
                CleanupTimer.AutoReset = true;
                CleanupTimer.Start();


               
                // Starts process timer
                FolderTimer = new System.Timers.Timer();
                FolderTimer.Elapsed += new System.Timers.ElapsedEventHandler(onTimedFolderEvent);
                FolderTimer.Interval = 1000 * 15 ; // Every 15 seconds

                FolderTimer.AutoReset = true;
                FolderTimer.Start();

                this.Dispatcher.Invoke(() =>
                {
                    StartProcess();
                });


                //StoreError(10, "TESTING", "InterfaceRecordImport", "IR", "IR", "MainWindow");

                this.Activate();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                { StoreError(ex.HResult, ex.Message + ex.StackTrace + "/n" + ex.InnerException.Message + ex.InnerException.StackTrace, "MainWindow " + ex.Source, "UR", "PF", "MainWindow"); }
                else
                { StoreError(ex.HResult, ex.Message + ex.StackTrace, "MainWindow " + ex.Source, "UR", "MW", "MainWindow"); }
                Application.Current.Shutdown();
            }
        }

        private void onTimedUpdateEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            bool Reset = false;
            if (Status)
            {
                Reset = true;
                StartProcess();
            }
            UpdateStatus("Checking for Update...");
            AppUpdate.CheckForNewVersion();
            UpdateStatus("Done Checking for Update.");
            if (Reset)
            {
                Reset = true;
                StartProcess();
            }
        }

        private void onTimedCleanupEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Cleanup(NumberOfDays);
        }

        private void onTimedFolderEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            ProcessFiles(false);
        }

        private void ProcessFiles(bool Bypass)
        {
            if (Processing == false)
            {
                string[] FileList;
                string[] UnprocessedFileList;

                Processing = true;
                Task task = null;
                string CurrentGDSFolder = "";
                string DuplicateFolder = GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation ;
                // First Amadeus
                if ((AmadeusFolderChanged && AmadeusFolderCompleted) || (Bypass && GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation != ""))
                {
                    CurrentGDSFolder = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                    FileList = Directory.GetFiles(CurrentGDSFolder);
                    UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");

                    if (FileCount(FileList) == 0 && FileCount(UnprocessedFileList) == 0)
                    {
                        AmadeusFolderChanged = false;
                        AmadeusFolderCompleted = true;
                    }
                    else
                    {
                        AmadeusFolderCompleted = false;

                        ChangeTaskBarIcon("B");
                        UpdateStatus("Processing Amadeus Records...");

                        // First we rename the files to avoid conflicts
                        task = Task.Factory.StartNew(() =>
                        {
                            new RenameFiles().StartRenamingFiles(FileList);
                        });
                        task.Wait();
                        // gets new list of files
                        FileList = Directory.GetFiles(CurrentGDSFolder);
                        // if a folder to copy to is set, then copy all files
                        if (DuplicateFolder != "")
                        {
                            task = Task.Factory.StartNew(() =>
                            {
                                new CopyFiles().StartCopyingFiles(DuplicateFolder + "\\Amadeus", FileList);
                            });
                            task.Wait();
                        }
                        // we then move the files to the SmartAgent sub folder 
                        task = Task.Factory.StartNew(() =>
                        {
                            new MoveAllFiles().StartMovingFiles(CurrentGDSFolder, FileList);
                        });
                        task.Wait();
                        UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                        task = Task.Factory.StartNew(() =>
                        {
                            new ProcessFolder()._ProcessFolder(UnprocessedFileList, "A", CurrentGDSFolder);
                        });
                        task.Wait();
                        AmadeusFolderCompleted = true;
                        UpdateStatus("Finished Processing Amadeus Records.");
                    }
                }

                // Then Sabre
                if ((SabreFolderChanged && SabreFolderCompleted) || (Bypass && GlobalSettings.MyAppSettings.SabreFilesFolderLocation != ""))
                {
                    CurrentGDSFolder = GlobalSettings.MyAppSettings.SabreFilesFolderLocation;
                    FileList = Directory.GetFiles(CurrentGDSFolder);
                    UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");

                    if (FileCount(FileList) == 0 && FileCount(UnprocessedFileList) == 0)
                    {
                        SabreFolderChanged = false;
                        SabreFolderCompleted = true;
                    }
                    else
                    {
                        SabreFolderCompleted = false;

                        ChangeTaskBarIcon("B");
                        UpdateStatus("Processing Sabre Records...");

                        // We don't need to rename files for Sabre
                        // First we rename the files to avoid conflicts
                        //task = Task.Factory.StartNew(() => {
                        //    new RenameFiles().StartRenamingFiles(FileList);
                        //});
                        // task.Wait();

                        // if a folder to copy to is set, then copy all files
                        if (DuplicateFolder != "")
                        {
                            task = Task.Factory.StartNew(() =>
                            {
                                new CopyFiles().StartCopyingFiles(DuplicateFolder + "\\Sabre", FileList);
                            });
                            task.Wait();
                        }
                        // we then move the files to the SmartAgent sub folder 
                        task = Task.Factory.StartNew(() =>
                        {
                            new MoveAllFiles().StartMovingFiles(CurrentGDSFolder, FileList);
                        });
                        task.Wait();
                        UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                        task = Task.Factory.StartNew(() =>
                        {
                            new ProcessFolder()._ProcessFolder(UnprocessedFileList, "S", CurrentGDSFolder);
                        });
                        task.Wait();
                        SabreFolderCompleted = true;
                        UpdateStatus("Finished Processing Sabre Records.");
                    }
                }

                // Apollo
                if ((ApolloFolderChanged && ApolloFolderCompleted) || (Bypass && GlobalSettings.MyAppSettings.ApolloFilesFolderLocation != ""))
                {
                    CurrentGDSFolder = GlobalSettings.MyAppSettings.ApolloFilesFolderLocation;
                    FileList = Directory.GetFiles(CurrentGDSFolder);
                    UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                    if (FileCount(FileList) == 0 && FileCount(UnprocessedFileList)==0)
                    {
                        ApolloFolderChanged = false;
                        ApolloFolderCompleted = true;
                    }
                    else
                    {
                        ApolloFolderCompleted = false;

                        ChangeTaskBarIcon("B");
                        UpdateStatus("Processing Apollo Records...");

                        // First we rename the files to avoid conflicts
                        task = Task.Factory.StartNew(() =>
                        {
                            new RenameFiles().StartRenamingFiles(FileList);
                        });
                        task.Wait();
                        // gets new list of files
                        FileList = Directory.GetFiles(CurrentGDSFolder);
                        // if a folder to copy to is set, then copy all files
                        if (DuplicateFolder != "")
                        {
                            task = Task.Factory.StartNew(() =>
                            {
                                new CopyFiles().StartCopyingFiles(DuplicateFolder + "\\Apollo", FileList);
                            });
                            task.Wait();
                        }
                        // we then move the files to the SmartAgent sub folder 
                        task = Task.Factory.StartNew(() =>
                        {
                            new MoveAllFiles().StartMovingFiles(CurrentGDSFolder, FileList);
                        });
                        task.Wait();
                        UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                        task = Task.Factory.StartNew(() =>
                        {
                            new ProcessFolder()._ProcessFolder(UnprocessedFileList, "P", CurrentGDSFolder);
                        });
                        task.Wait();
                        ApolloFolderCompleted = true;
                        UpdateStatus("Finished Processing Apollo Records.");
                    }
                }

                // Worldspan
                if ((WorldspanFolderChanged && WorldspanFolderCompleted) || (Bypass && GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation != ""))
                {
                    CurrentGDSFolder = GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation;
                    FileList = Directory.GetFiles(CurrentGDSFolder);
                    UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                    if (FileCount(FileList) == 0 && FileCount(UnprocessedFileList) == 0)
                    {
                        WorldspanFolderChanged = false;
                        WorldspanFolderCompleted = true;
                    }
                    else
                    {
                        WorldspanFolderCompleted = false;
                        ChangeTaskBarIcon("B");
                        UpdateStatus("Processing Worldspan Records...");
                        // First we rename the files to avoid conflicts
                        task = Task.Factory.StartNew(() =>
                        {
                            new RenameFiles().StartRenamingFiles(FileList);
                        });
                        task.Wait();
                        // gets new list of files
                        FileList = Directory.GetFiles(CurrentGDSFolder);
                        // if a folder to copy to is set, then copy all files
                        if (DuplicateFolder != "")
                        {
                            task = Task.Factory.StartNew(() =>
                            {
                                new CopyFiles().StartCopyingFiles(DuplicateFolder + "\\Worldspan", FileList);
                            });
                            task.Wait();
                        }
                        // we then move the files to the SmartAgent sub folder 
                        task = Task.Factory.StartNew(() =>
                        {
                            new MoveAllFiles().StartMovingFiles(CurrentGDSFolder, FileList);
                        });
                        task.Wait();
                        UnprocessedFileList = Directory.GetFiles(CurrentGDSFolder + "\\SmartAgent");
                        task = Task.Factory.StartNew(() =>
                        {
                            new ProcessFolder()._ProcessFolder(UnprocessedFileList, "W", CurrentGDSFolder);
                        });
                        task.Wait();

                        WorldspanFolderCompleted = true;
                        UpdateStatus("Finished Processing Worldspan Records.");
                    }
                }
                Processing = false;
                ChangeTaskBarIcon("G");
                
            }
        }

        private int FileCount(string [] FileList)
        {
            int Count = 0;
            
            foreach (string FileName in FileList)
            {
                try
                {
                    if (new FileInfo(FileName).Length > 250)
                    {
                        Count++;
                    }
                }
                catch (Exception ex)
                { }
            }
            return Count;
        }

        private bool Cleanup(int NumberOfdays)
        {

            try
            {

                CleanupFolders MyCleanupFolders = new CleanupFolders();
                UpdateStatus("Cleaning Up Folders...");
                Task[] TaskList = new Task[4];
                if (GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation != "")
                {
                    TaskList[0] = Task.Factory.StartNew(() => MyCleanupFolders.StartCleanup(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation, 7,false));
                }
                if (GlobalSettings.MyAppSettings.SabreFilesFolderLocation != "")
                {
                    TaskList[1] = Task.Factory.StartNew(() => MyCleanupFolders.StartCleanup(GlobalSettings.MyAppSettings.SabreFilesFolderLocation, 7,false));
                }
                if (GlobalSettings.MyAppSettings.ApolloFilesFolderLocation != "")
                {
                    TaskList[2] = Task.Factory.StartNew(() => MyCleanupFolders.StartCleanup(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation, 7,false));
                }
                if (GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation != "")
                {
                    TaskList[3] = Task.Factory.StartNew(() => MyCleanupFolders.StartCleanup(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation, 7,false));
                }
                Task.WaitAll(TaskList.ToArray());
                return true;

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                { StoreError(ex.HResult, ex.Message + ex.StackTrace + "/n" + ex.InnerException.Message + ex.InnerException.StackTrace, "MainWindow " + ex.Source, "UR", "PF", "MainWindow"); }
                else
                { StoreError(ex.HResult, ex.Message + ex.StackTrace, "MainWindow " + ex.Source, "UR", "MW", "MainWindow"); }
                //Application.Current.Shutdown();
                return false;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProcessFiles(true);
            });
        }

        void ChangeTaskBarIcon(string Color)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (Color == "B")
                { TasbarIcon.IconSource = ImageSourceBlue; }
                else if (Color == "R")
                { TasbarIcon.IconSource = ImageSourceRed; }
                else if (Color == "G")
                { TasbarIcon.IconSource = ImageSourceGreen; }
            });
        }

        void UpdateStatus(string Message)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBox_Status.Text = Message;
                TextBox_Status.UpdateLayout();
            });
        }

        void MouseDownHandler(object sender, RoutedEventArgs e)
        {
            if (this.WindowState==WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            this.Activate();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            //Your code to handle the event
            this.Dispatcher.Invoke(() =>
            {
                TasbarIcon.Dispose();
            });
        }

        public  void StartProcess()
        {
            if (Status)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (AmadeusFileWatcher != null)
                    { AmadeusFileWatcher.EnableRaisingEvents = false;   }
                    if (SabreFileWatcher != null)
                    { SabreFileWatcher.EnableRaisingEvents = false;     }
                    if (ApolloFileWatcher != null)
                    { ApolloFileWatcher.EnableRaisingEvents = false;    }
                    if (WorldspanFileWatcher != null)
                    { WorldspanFileWatcher.EnableRaisingEvents = false; }
                    Border_Start.BorderBrush = Brushes.Red;
                    Button_Start.Content = "Start";
                    ChangeTaskBarIcon("R");
                    TasbarIcon.ToolTipText = "SmartAgent AIR Process: Stopped";
                    UpdateStatus("Not Checking New Records.");

                    Status = false;
                });
                
                
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (AmadeusFileWatcher!=null)
                    {   AmadeusFileWatcher.EnableRaisingEvents = true; }
                    if (SabreFileWatcher != null)
                    {   SabreFileWatcher.EnableRaisingEvents = true; }
                    if (ApolloFileWatcher != null)
                    { ApolloFileWatcher.EnableRaisingEvents = true; }
                    if (WorldspanFileWatcher != null)
                    { WorldspanFileWatcher.EnableRaisingEvents = true; }
                    Border_Start.BorderBrush = Brushes.Green;
                    Button_Start.Content = "Stop";
                    TasbarIcon.ToolTipText = "SmartAgent AIR Process: Started";
                    ChangeTaskBarIcon("G");
                    Status = true;
                    UpdateStatus("Started...");

                });
            }
        }
        /*
        public void ProcessAllFiles()
        {
            if (GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation != "")
            {
                MyProcessFolder = new ProcessFolder();

                //ProcessFolder(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation, "A");
            }

            if (GlobalSettings.MyAppSettings.SabreFilesFolderLocation != "")
            {
                ProcessFolder(GlobalSettings.MyAppSettings.SabreFilesFolderLocation, "S");
            }

            if (GlobalSettings.MyAppSettings.ApolloFilesFolderLocation != "")
            {
                ProcessFolder(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation, "P");
            }

            if (GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation != "")
            {
                ProcessFolder(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation, "W");
            }
        }
        */
        

        public void SetupFolderWatchers()
        {
            
            if (GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation != "")
            {
                AmadeusFileWatcher = new FileSystemWatcher();
                AmadeusFileWatcher.Path = GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation;
                AmadeusFileWatcher.NotifyFilter =NotifyFilters.LastWrite;
                AmadeusFileWatcher.Filter = "*.*";
                AmadeusFileWatcher.Changed += new FileSystemEventHandler(OnAmadeusFolderChanged);
                //AmadeusFileWatcher.Created += new FileSystemEventHandler(OnAmadeusFolderChanged);
                //AmadeusFileWatcher.EnableRaisingEvents = true;
            }
            if (GlobalSettings.MyAppSettings.SabreFilesFolderLocation != "")
            {
                SabreFileWatcher = new FileSystemWatcher();
                SabreFileWatcher.Path = GlobalSettings.MyAppSettings.SabreFilesFolderLocation;
                SabreFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                SabreFileWatcher.Filter = "*.*";
                //SabreFileWatcher.Created += new FileSystemEventHandler(OnSabreFolderChanged);
                //SabreFileWatcher.Changed = null;
                SabreFileWatcher.Changed += new FileSystemEventHandler(OnSabreFolderChanged);
                // SabreFileWatcher.EnableRaisingEvents = true;
            }
            if (GlobalSettings.MyAppSettings.ApolloFilesFolderLocation != "")
            {
                ApolloFileWatcher = new FileSystemWatcher();
                ApolloFileWatcher.Path = GlobalSettings.MyAppSettings.ApolloFilesFolderLocation;
                ApolloFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                ApolloFileWatcher.Filter = "*.*";
                //SabreFileWatcher.Created += new FileSystemEventHandler(OnSabreFolderChanged);
                //SabreFileWatcher.Changed = null;
                ApolloFileWatcher.Changed += new FileSystemEventHandler(OnApolloFolderChanged);
                // SabreFileWatcher.EnableRaisingEvents = true;
            }
            if (GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation != "")
            {
                WorldspanFileWatcher = new FileSystemWatcher();
                WorldspanFileWatcher.Path = GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation;
                WorldspanFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                WorldspanFileWatcher.Filter = "*.*";
                //SabreFileWatcher.Created += new FileSystemEventHandler(OnSabreFolderChanged);
                //SabreFileWatcher.Changed = null;
                WorldspanFileWatcher.Changed += new FileSystemEventHandler(OnWorldspanFolderChanged);
                // SabreFileWatcher.EnableRaisingEvents = true;
            }
        }

        private  void OnAmadeusFolderChanged(object source, FileSystemEventArgs e)
        {  // ProcessFolder(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation,"A");

            AmadeusFolderChanged = true;
        }

        private  void OnSabreFolderChanged(object source, FileSystemEventArgs e)
        {
            SabreFolderChanged = true;
            //ProcessFolder(GlobalSettings.MyAppSettings.SabreFilesFolderLocation,"S");
        }

        private void OnApolloFolderChanged(object source, FileSystemEventArgs e)
        {
            ApolloFolderChanged = true;
            //ProcessFolder(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation, "P");
        }

        private void OnWorldspanFolderChanged(object source, FileSystemEventArgs e)
        {
            WorldspanFolderChanged = true;
            //ProcessFolder(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation, "W");
        }

        private  void ShowRecord(string Record)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBlock_Record.Text = Record;
                //TextBlock_Record.
                //TextBlock_Record.in
                //TextBlock_Record.re
            });

            /*  Task.Factory.StartNew(() =>
        {
                       /// do all your logic here

                       //Update Text on the UI thread 
                       Application.Current.Dispatcher.BeginInvoke( DispatcherPriority.Input,
                      new Action(() => { statusTextBox.Text = "newValue";}));

                       //continue with the rest of the logic that take a long time
                    });*/
        }
        

        public void SetupFolders()
        {
            if (GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation != "")
            {
                if (System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation))
                {
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent");
                    }
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Processed"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Processed");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Processed/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Processed/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Error"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.AmadeusFilesFolderLocation + "/SmartAgent/Error");
                    }

                }
                        
            }
            if (GlobalSettings.MyAppSettings.SabreFilesFolderLocation != "")
            {
                if (System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation))
                {
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Processed")) 
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Processed");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Processed/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Processed/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Error"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.SabreFilesFolderLocation + "/SmartAgent/Error");
                    }
                }

            }
            if (GlobalSettings.MyAppSettings.ApolloFilesFolderLocation != "")
            {
                if (System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation))
                {
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Processed"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Processed");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Processed/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Processed/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Error"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.ApolloFilesFolderLocation + "/SmartAgent/Error");
                    }
                }

            }
            if (GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation != "")
            {
                if (System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation))
                {
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Processed"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Processed");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Processed/Archive"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Processed/Archive");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Error"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.WorldspanFilesFolderLocation + "/SmartAgent/Error");
                    }
                }

            }

            if (GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation != "")
            {
                if (System.IO.Directory.Exists(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation))
                {
                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Amadeus"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Amadeus");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Sabre"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Sabre");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Apollo"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Apollo");
                    }

                    if (!System.IO.Directory.Exists(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Worldspan"))
                    {
                        System.IO.Directory.CreateDirectory(GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation + "/Worldspan");
                    }

                }

            }
        }


       /* private void ArchiveFiles(String Path)
        {
            DirectoryInfo SourceDirectory = new DirectoryInfo(Path);


        } */

        private String GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window_Settings MyWindow = new Window_Settings();
            MyWindow.Owner = this;
            this.IsEnabled = false;
            //MyWindow.
            MyWindow.Show();
        }

        public void OnWindowClosed(object sender, CancelEventArgs e)
        {
            // Handle closing logic, set e.Cancel as needed
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void TaskBarIconLeftClick()
        {
            WindowState = WindowState.Normal;
        }

        public class ShowWindowCommand : ICommand
        {
            //Displays MainWindow after clicking on TaskIcon
            public void Execute(object parameter)
            {
                //ShowClass.ShowMain();
                //TaskBarIconLeftClick();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged { add { } remove { } }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            StartProcess();
        }

        public void StoreError(int ErrorNumber, string ErrorDescription, string ErrorSource, string ProgramCode, string ProgramMode, string FunctionName)
        {
            try
            {
                Task t = Task.Run(() =>
                {
                    SendDataToSmartAgent MyClass = new SendDataToSmartAgent();
                    MyClass.SendErrorToSmartAgent(ErrorNumber, ErrorDescription, ErrorSource, ProgramCode, ProgramMode, FunctionName).Wait();
                    MyClass.Dispose();
                    MyClass = null;
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}


