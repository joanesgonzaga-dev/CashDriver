using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Data
{
    public class DatabaseConstants
    {
        public const string DatabaseFileName = "cashdriver.db3";

#if WINDOWS
        public static string DatabasePath =  $@"C:\Temp\{DatabaseFileName}";
#else
        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
#endif
    }
}
