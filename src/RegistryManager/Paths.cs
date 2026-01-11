using System;
using System.Diagnostics;
using System.IO;

namespace RegistryManager;

public static class Paths
{
    public static string ApplicationPath
        => Path.GetDirectoryName(ApplicationFullName)!;

    public static string ApplicationFullName
        => Environment.ProcessPath!;

    public static string ApplicationName
        => Process.GetCurrentProcess().MainModule!.ModuleName;

    public static string ApplicationNameWithoutExtension
        => Path.GetFileNameWithoutExtension(ApplicationName);
}
