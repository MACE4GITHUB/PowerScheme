using System.Reflection;
using System.Runtime.InteropServices;

namespace PowerScheme.Model;

public static class AppInfo
{
    private static Assembly GetExecutingAssembly { get; } =
        typeof(Program).Assembly;

    public static string CompanyName { get; } = GetExecutingAssembly
        .GetCustomAttribute<AssemblyCompanyAttribute>()?
        .Company ?? "BULAVA";

    public static string ProductName { get; } = GetExecutingAssembly
        .GetCustomAttribute<AssemblyProductAttribute>()?
        .Product ?? "PowerScheme";

    public static string ProductVersion { get; } = $"{GetExecutingAssembly
        .GetName()
        .Version}";

    public static string ProductGuid { get; } = GetExecutingAssembly
        .GetCustomAttribute<GuidAttribute>()?
        .Value ?? "23610885-8476-4388-afc8-bc564f6be09f";
}
