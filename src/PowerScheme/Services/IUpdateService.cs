using System.Threading.Tasks;
using Updater.Common;

namespace PowerScheme.Services;

public interface IUpdateService
{
    Task<ReleaseInfo> GetReleaseInfoAsync();
    ReleaseInfo ReleaseInfo { get; }
    void Update();
}
