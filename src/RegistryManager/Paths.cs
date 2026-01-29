using System.Diagnostics;
using System.IO;

namespace RegistryManager;

public static class Paths
{
    public static string ApplicationPath =>
        Path.GetDirectoryName(ApplicationFileName)!;

    public static string ApplicationFileName =>
        MainModule.FileName;

    public static string ApplicationName =>
        MainModule.ModuleName;

    public static string ApplicationNameWithoutExtension =>
        Path.GetFileNameWithoutExtension(ApplicationName);

    private static ProcessModule MainModule =>
        Process.GetCurrentProcess().MainModule;
}
