using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InterfaceRecordImporter
{
    public class RenameFiles
    {
        public void StartRenamingFiles(string[] FileList)
        {
           _RenameFiles(FileList); 
        }

        private void _RenameFiles(string[] FileList)
        {
           
            string Year = "";
            string Month = "";
            string Day = "";
            bool Rename = false;
            FileInfo fi;

            foreach (string file in FileList)
            {
                Rename = false;
                try
                {
                    
                    fi = new FileInfo(file);
                    Year = fi.CreationTime.Year.ToString();
                    Month = fi.CreationTime.Month.ToString().PadLeft(2,'0');
                    Day = fi.CreationTime.Day.ToString().PadLeft(2,'0');

                    if (fi.Length >= 60)
                    {
                        if (fi.Name.Length < 9)
                        {    Rename = true; }
                        else if (fi.Name.Substring(0, 4) != Year)
                        {   Rename = true;  }

                        if (Rename)
                        {
                            File.Move(file, fi.DirectoryName + '\\' + Year + Month + Day + "_" + fi.Name);
                        }
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
