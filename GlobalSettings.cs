using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceRecordImporter
{
    static class GlobalSettings
    {
        public static Settings MyAppSettings { get; set; }

      
        public static System.Security.SecureString PassPhrase = new System.Security.SecureString();
    }
}
