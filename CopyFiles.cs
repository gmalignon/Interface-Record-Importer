using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceRecordImporter
{
    public class CopyFiles
    {
        public void StartCopyingFiles(string CopyFolderPath, string[] FileList)
        {
            _CopyFiles(CopyFolderPath, FileList);
        }

        private void _CopyFiles(string CopyFolderPath, string[] FileList)
        {
            FileInfo fileinfo;
            foreach (string FileName in FileList)
            {
                try
                { fileinfo = new FileInfo(FileName);
                    if (fileinfo.Length > 250)
                    {
                        if (!File.Exists(CopyFolderPath + "\\" + fileinfo.Name))
                        { File.Copy(FileName, CopyFolderPath + "\\" + fileinfo.Name); }
                    }
                }
                catch (Exception ex)
                { }
            }
        }
    }
}
