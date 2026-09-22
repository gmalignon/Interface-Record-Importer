using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Threading;

namespace InterfaceRecordImporter
{
    public class ProcessFolder
    {

        public void StartProcess(string [] FileList, string GDS, string FolderPath)
        {
            _ProcessFolder(FileList, GDS, FolderPath);
        }

        public void _ProcessFolder(string[] FileList, string GDS, string FolderPath)
        {
           // int FileSuccessFullySent = 0;
            SendDataToSmartAgent MyClass = null;
            Task<String> task;
            MyClass = new SendDataToSmartAgent();
            string FileContent="";
            string Response = "";
            FileStream fs = null;
            FileInfo fileinfo;
            string ProcessedFolderPath = FolderPath + "\\SmartAgent\\Processed\\";

            foreach (string FileName in FileList)
            {
                if (File.Exists(FileName))
                {
                    try
                    {

                        fileinfo = new FileInfo(FileName);

                        fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);

                        using (StreamReader reader = new StreamReader(fs))
                        {
                            FileContent = reader.ReadToEnd();
                        }
                        fs.Dispose();
                        task = MyClass.SendFileToSmartAgent(FileContent, fileinfo.Name);
                        task.Wait();
                        Response = task.Result;

                        if (Response == "OK")
                        {
                          
                            if (File.Exists(ProcessedFolderPath  + fileinfo.Name))
                            { File.Delete(ProcessedFolderPath + fileinfo.Name); }
                            File.Move(FileName, ProcessedFolderPath + fileinfo.Name);
                        }

                        task.Dispose();
                    }
                    catch( Exception ex)
                    {

                    }
                }
            }

        }
            /*
            public void MyProcessFolder(string FolderPath, string Interface)
            {
                string[] FileList;
                int Counter = 0;
                string MyFileName = "";
                string TFileName = "";
                string TStatus = "";

                int StartOverCount = 0;
                string Response = "";
                bool EmptyFolder = true;
                int FileSuccessFullySent = 0;
                bool NewRecord = false;
                Task<String> task;
                string CopyPath = GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation;
                SendDataToSmartAgent MyClass = null;

                try
                {
                    if (Interface == "A")
                    {
                        //AmadeusFileWatcher.EnableRaisingEvents = false;
                        if (CopyPath != "")
                        { CopyPath += "/Amadeus"; }
                       // UpdateStatus("New Amadeus Records...");
                    }
                    else if (Interface == "S")
                    {
                       // SabreFileWatcher.EnableRaisingEvents = false;
                        if (CopyPath != "")
                        { CopyPath += "/Sabre"; }
                       // UpdateStatus("New Sabre Records...");
                    }
                    else if (Interface == "P")
                    {
                        //ApolloFileWatcher.EnableRaisingEvents = false;
                        if (CopyPath != "")
                        { CopyPath += "/Apollo"; }
                       // UpdateStatus("New Apollo Records...");
                    }
                    else if (Interface == "W")
                    {
                        //WorldspanFileWatcher.EnableRaisingEvents = false;
                        if (CopyPath != "")
                        { CopyPath += "/Worldspan"; }
                       // UpdateStatus("New Worldspan Records...");
                    }

                    //SetupFolders();

                StartOver:
                    FileList = Directory.GetFiles(FolderPath);
                    if (FileList.Length > 0)
                    {
                        EmptyFolder = false;
                        MyClass = new SendDataToSmartAgent();
                        foreach (string FileName in FileList)
                        {
                            if (File.Exists(FileName))
                            {
                                MyFileName = new FileInfo(FileName).Name;

                                if (!File.Exists(FolderPath + "/SmartAgent/Processed/" + MyFileName))
                                {
                                    if (!File.Exists(FolderPath + "/SmartAgent/" + MyFileName))
                                    { File.Copy(FileName, FolderPath + "/SmartAgent/" + MyFileName); }
                                    // Copies to selected folder
                                    if (GlobalSettings.MyAppSettings.DuplicateFilesFolderLocation != "")
                                    {
                                        try
                                        {
                                            File.Copy(FileName, CopyPath + '/' + MyFileName);
                                        }
                                        catch (Exception ex)
                                        {
                                            UpdateStatus("Error Copying Files: " + ex.Message);
                                            if (ex.InnerException != null)
                                            {
                                                StoreError(ex.HResult, ex.Message + ex.StackTrace + "/n" + ex.InnerException.Message + ex.InnerException.StackTrace, "Copy File " + ex.Source, "UR", "PF", "Process Folder");
                                            }
                                            else
                                            {
                                                StoreError(ex.HResult, ex.Message + ex.StackTrace, "Process Folder " + ex.Source, "UR", "PF", "Process Folder");
                                            }
                                        }

                                    }
                                    NewRecord = true;
                                }
                            }
                        }
                        FileList = Directory.GetFiles(FolderPath + "/SmartAgent");

                    }
                    if (NewRecord)
                    {
                        foreach (string FileName in FileList)
                        {
                        TryAgain:
                            try
                            {   //Console.WriteLine(name);
                                if (File.Exists(FileName))
                                {

                                   // FileStream fs = new FileStream(@”c:\test.txt”, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                   // StreamReader sr = new StreamReader(fs);
                                   // txtContents.Text = sr.ReadToEnd();
                                   // sr.Close();


                                    // Create a file to write to.
                                    string FileContent = File.ReadAllText(FileName);
                                    // if (FileContent.Substring(0, 2) == "AA" || FileContent.Substring(0, 3) == "AIR")
                                    // {
                                    if (FileContent.Substring(0, 2) == "AA")
                                    { Interface = "S"; }
                                    else if (FileContent.Substring(0, 3) == "AIR")
                                    { Interface = "A"; }

                                    Dispatcher.Invoke(() =>
                                    {
                                        this.ShowRecord(FileContent);
                                        ChangeTaskBarIcon("B");
                                    });
                                    // MyClass = new SendDataToSmartAgent();
                                    MyFileName = new FileInfo(FileName).Name;

                                    if (Interface == "A")
                                    {
                                        TFileName = "Amadeus";
                                        TStatus = "Sending Amadeus File: " + MyFileName;
                                    }
                                    else if (Interface == "S")
                                    {
                                        TFileName = MyFileName;
                                        TStatus = "Sending Sabre File: " + MyFileName;
                                    }
                                    else if (Interface == "P")
                                    {
                                        TFileName = MyFileName;
                                        TStatus = "Sending Apollo File: " + MyFileName;
                                    }
                                    else if (Interface == "W")
                                    {
                                        TFileName = MyFileName;
                                        TStatus = "Sending Worldspan File: " + MyFileName;
                                    }

                                    UpdateStatus(TStatus);
                                    task = MyClass.SendFileToSmartAgent(FileContent, FileName, FolderPath, TFileName);
                                    task.Wait();
                                    Response = task.Result;


                                    if (Response == "OK")
                                    {
                                        FileSuccessFullySent++;
                                        if (File.Exists(FolderPath + "/SmartAgent/Processed/" + MyFileName))
                                        { File.Delete(FolderPath + "/SmartAgent/Processed/" + MyFileName); }
                                        File.Move(FileName, FolderPath + "/SmartAgent/Processed/" + MyFileName);
                                    }

                                    task.Dispose();
                                    //}
                                    //else
                                    // {
                                    //     File.Move(FileName, FolderPath + "/SmartAgent/Processed/" + new FileInfo(FileName).Name);
                                    // }
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateStatus("Error Processing Files: " + ex.Message);
                                if (ex.InnerException != null)
                                { StoreError(ex.HResult, ex.Message + ex.StackTrace + "/n" + ex.InnerException.Message + ex.InnerException.StackTrace, "Process Folder " + ex.Source, "UR", "PF", "Process Folder"); }
                                else
                                { StoreError(ex.HResult, ex.Message + ex.StackTrace, "Process Folder " + ex.Source, "UR", "PF", "Process Folder"); }

                                Counter++;
                                if (Counter < 5)
                                {
                                    System.Threading.Thread.Sleep(2000);
                                    goto TryAgain;
                                }
                            }

                        }
                    }
                    FileList = Directory.GetFiles(FolderPath);
                    if (FileList.Length > 0 && StartOverCount <= 10)
                    {
                        StartOverCount++;
                        goto StartOver;
                    }
                    if (MyClass != null)
                    { MyClass.Dispose(); }
                    if (Interface == "A")
                    { AmadeusFileWatcher.EnableRaisingEvents = true; }
                    else if (Interface == "S")
                    { SabreFileWatcher.EnableRaisingEvents = true; }
                    else if (Interface == "P")
                    { ApolloFileWatcher.EnableRaisingEvents = true; }
                    else if (Interface == "W")
                    { WorldspanFileWatcher.EnableRaisingEvents = true; }
                    ChangeTaskBarIcon("G");
                    MyClass = null;
                    if (EmptyFolder)
                    { UpdateStatus("Done Checking Folder, No New Records Found... "); }
                    else
                    { UpdateStatus("Done Sending Records, " + FileSuccessFullySent.ToString() + " Records Were Transmitted... "); }
                }
                catch (Exception ex)
                {
                    UpdateStatus("Error: " + ex.Message);
                    if (ex.InnerException != null)
                    { StoreError(ex.HResult, ex.Message + ex.StackTrace + "/n" + ex.InnerException.Message + ex.InnerException.StackTrace, "Process Folder (Outer try) " + ex.Source, "UR", "PF", "Process Folder"); }
                    else
                    { StoreError(ex.HResult, ex.Message + ex.StackTrace, "Process Folder (Outer Try) " + ex.Source, "UR", "PF", "Process Folder"); }

                }
            }*/

        }
}
