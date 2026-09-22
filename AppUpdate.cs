using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;

namespace InterfaceRecordImporter
{
    static class AppUpdate
    {
        private static string APIKey = "a6bae5be-6610-47ad-84bd-d598defea3db";
        public static bool CheckForNewVersion( )
        {
            System.Net.Http.HttpClient MyClient = new System.Net.Http.HttpClient();
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + "\\InterfaceRecordImporter_Old.exe"))
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\InterfaceRecordImporter_Old.exe"); }

                MyClient.BaseAddress = new Uri("https://Atlas.SmartAgent.nyc/");
                MyClient.DefaultRequestHeaders.Accept.Clear();
                MyClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                MyClient.DefaultRequestHeaders.Add("APIKey", APIKey);

            }
            catch (Exception ex)
            {

            }
            BinaryWriter FS2 = default(BinaryWriter);
            byte[] ByteArray2 = null;

            try
            {
                ByteArray2 = MyClient.GetByteArrayAsync("api/Interface/GetNewVersion?Version=" + GetProductVersion()).Result;
                File.Move(Directory.GetCurrentDirectory() + "\\InterfaceRecordImporter.exe", "InterfaceRecordImporter_Old.exe");
                FS2 = new BinaryWriter(new FileStream(Directory.GetCurrentDirectory() + "\\InterfaceRecordImporter.exe", FileMode.Create, FileAccess.Write));
                FS2.Write(ByteArray2);
                FS2.Dispose();
                MyClient.Dispose();

                System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\InterfaceRecordImporter.exe");
               
                System.Environment.Exit(0);
                return true;
            }
            catch (Exception ex)
            {
                //MsgBox(ex.StackTrace)
                MyClient.Dispose();
                return false;
            }
            

        }

        public static string GetProductVersion()
        {
            //var attribute = (AssemblyVersionAttribute)Assembly
            //  .GetExecutingAssembly()
            //  .GetCustomAttributes(typeof(AssemblyVersionAttribute), true)
            //  .Single();
            //return attribute.Version;
            return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public static string GetLastModifiedDate()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return lastModified.ToString("MM/dd/yy HH:mm:ss");

        }

    }

}

