using System.Diagnostics;
using System.IO;

namespace RegistryManager;

public static class Paths
{
    private const string EXE_EXTENSION = ".exe";
    private const string UPDATER = "Updater";
    private const string RUNAS = "RunAs";
    private const string REGWRITER = "RegWriter";

    public static string ApplicationPath =>
        Path.GetDirectoryName(ApplicationFileName)!;

    public static string ApplicationFileName =>
        MainModule.FileName;

    public static string ApplicationName =>
        MainModule.ModuleName;

    public static string ApplicationNameWithoutExtension =>
        Path.GetFileNameWithoutExtension(ApplicationName);

    public static string UpdaterFileName =>
        Path.Combine(ApplicationPath, $"{UPDATER}{EXE_EXTENSION}");

    public static string RunasFileName =>
        Path.Combine(ApplicationPath, $"{RUNAS}{EXE_EXTENSION}");

    public static string RegWriterFileName =>
        Path.Combine(ApplicationPath, $"{REGWRITER}{EXE_EXTENSION}");

    private static ProcessModule MainModule =>
        Process.GetCurrentProcess().MainModule;
}
