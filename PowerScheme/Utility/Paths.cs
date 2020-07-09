using System.Diagnostics;
using System.IO;

namespace PowerScheme.Utility
{
    internal static class Paths
    {
        public static string ApplicationPath
            => Path.GetDirectoryName(ApplicationFullName);

        public static string ApplicationFullName 
            => Process.GetCurrentProcess().MainModule?.FileName;

        public static string ApplicationName
            => Process.GetCurrentProcess().MainModule?.ModuleName;

        public static string ApplicationNameWithoutExtension
            => Path.GetFileNameWithoutExtension(ApplicationName);
    }
}
