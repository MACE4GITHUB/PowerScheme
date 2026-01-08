using System.Diagnostics;
using System.Reflection;

namespace PowerScheme.Model;

public class AppInfo
{
    public string CompanyName { get; }
    public string ProductName { get; }
    public string ProductVersion { get; }

    public AppInfo()
    {
        CompanyName = "MACE";
        ProductName = "PowerScheme";
        ProductVersion = "2.1";
    }
}
