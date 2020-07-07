using System.Diagnostics;

namespace RegistryManager
{
    internal static class Paths
    {
        public static string ApplicationPath
            => System.IO.Path.GetDirectoryName(ApplicationFullName);

        public static string ApplicationFullName 
            => Process.GetCurrentProcess().MainModule?.FileName;

        public static string ApplicationName
            => Process.GetCurrentProcess().MainModule?.ModuleName;
    }
}
