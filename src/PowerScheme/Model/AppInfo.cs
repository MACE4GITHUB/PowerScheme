using System.Reflection;
using System.Runtime.InteropServices;

namespace PowerScheme.Model;

public static class AppInfo
{
    private static Assembly GetExecutingAssembly { get; } =
        Assembly.GetExecutingAssembly();

    public static string CompanyName { get; } = GetExecutingAssembly
        .GetCustomAttribute<AssemblyCompanyAttribute>()?
        .Company;

    public static string ProductName { get; } = GetExecutingAssembly
        .GetCustomAttribute<AssemblyProductAttribute>()?
        .Product;

    public static string ProductVersion { get; } = $"{GetExecutingAssembly
        .GetName()
        .Version}";

    public static string ProductGuid { get; } = GetExecutingAssembly
        .GetCustomAttribute<GuidAttribute>()?
        .Value;
}
