using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceRecordImporter
{
    public class MoveAllFiles
    {
        public void StartMovingFiles(string FolderPath, string[] FileList)
        {
            _MoveFiles(FolderPath, FileList);
        }

        private void _MoveFiles(string FolderPath, string[] FileList)
        {
            FileInfo fi;
            foreach (string FileName in FileList)
            {
                try
                {
                    fi = new FileInfo(FileName);
                    // updated from 250 to 60 to include Sabre Void records
                    if (fi.Length >= 60)
                    {
                        if (!File.Exists(FolderPath + "\\SmartAgent\\" + fi.Name))
                        { File.Move(FileName, FolderPath + "\\SmartAgent\\" + fi.Name); }
                    }
                }
                catch (Exception ex)
                { }
            }
        }
    }
}
