using System.Diagnostics;
using System.IO;

namespace RunAs.Common.Utils
{
    public static class Paths
    {
        public static string ApplicationPath
            => System.IO.Path.GetDirectoryName(ApplicationFullName);

        public static string ApplicationFullName 
            => Process.GetCurrentProcess().MainModule?.FileName;

        public static string ApplicationName
            => Process.GetCurrentProcess().MainModule?.ModuleName;
        
        public static string ApplicationNameWithoutExtension
            => Path.GetFileNameWithoutExtension(ApplicationName);
    }
}
