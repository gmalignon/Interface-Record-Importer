using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InterfaceRecordImporter
{
    public class CleanupFolders
    {

        public void StartCleanup(string FolderPath, int NumberOfDays, bool OnlyMainFolder)
        {
            // First Cleanup Main folder
            Cleanup(FolderPath, NumberOfDays, OnlyMainFolder);
            // Cleans up error files (not processed within a week TO DO
            //if (OnlyMainFolder == false)
            //{ Cleanup(FolderPath + "/SmartAgent/Processed", NumberOfDays, OnlyMainFolder); }

            // then processed folder
            if (OnlyMainFolder == false)
            { Cleanup(FolderPath + "\\SmartAgent\\Processed", NumberOfDays, OnlyMainFolder); }
        }


        private void Cleanup(string FolderPath, int NumberOfDays, bool OnlyMainFolder)
        {
            string[] FileList = Directory.GetFiles(FolderPath);
            int Year = 0;
            int Month = 0;
            int Day = 0;
            FileInfo fi;

            if (!System.IO.Directory.Exists(FolderPath + "\\Archive"))
            {   System.IO.Directory.CreateDirectory(FolderPath + "\\Archive"); }

            foreach (string file in FileList)
            {
                try
                {
                    fi= new FileInfo(file);
                 
                    if ((fi.CreationTime < DateTime.Now.AddDays(-NumberOfDays))  )
                    {
                        Year = fi.CreationTime.Year;
                        Month = fi.CreationTime.Month;
                        Day = fi.CreationTime.Day;

                        if (!System.IO.Directory.Exists(FolderPath + "\\Archive"))
                        {   System.IO.Directory.CreateDirectory(FolderPath + "\\Archive"); }
                        if (!System.IO.Directory.Exists(FolderPath + "\\Archive\\" + Year))
                        {   System.IO.Directory.CreateDirectory(FolderPath + "\\Archive\\" + Year); }
                        if (!System.IO.Directory.Exists(FolderPath + "\\Archive\\" + Year + "\\" + Month))
                        {   System.IO.Directory.CreateDirectory(FolderPath + "\\Archive\\" + Year + "\\" + Month); }
                        File.Move(file, FolderPath + "\\Archive\\" + Year + "\\" + Month + "\\"  + fi.Name);
                    }
                }
                catch (Exception ex)
                {

                }
            }


            // To do: Move All records to "/Archive" Folder older than 10 days, Create Year/Month/day for all the files
            // To do: Move All records in "/SmartAgent" Folder older than 10 days to "/Smartagent/Error" folder
            // To do: Move All records in "/SmartAgent/Processed" Folder older than 30 days to "SmartAgent/Processed/Archive"
        }
    }
}
