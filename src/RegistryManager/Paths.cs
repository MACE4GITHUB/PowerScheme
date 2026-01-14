using System.Diagnostics;
using System.IO;

namespace RegistryManager;

public static class Paths
{
    public static string ApplicationPath
        => Path.GetDirectoryName(ApplicationFullName)!;

    public static string ApplicationFullName
        => Process.GetCurrentProcess().MainModule!.ModuleName;

    public static string ApplicationNameWithoutExtension
        => Path.GetFileNameWithoutExtension(ApplicationFullName);
}
