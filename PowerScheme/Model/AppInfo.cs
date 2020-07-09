using System.Diagnostics;
using System.Reflection;

namespace PowerScheme.Model
{
    public class AppInfo
    {
        private FileVersionInfo _fileVersionInfo;

        public string CompanyName { get; }
        public string ProductName { get; }
        public string ProductVersion { get; }

        public AppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            CompanyName = _fileVersionInfo.CompanyName;
            ProductName = _fileVersionInfo.ProductName;
            ProductVersion = _fileVersionInfo.ProductVersion;
        }

    }
}
