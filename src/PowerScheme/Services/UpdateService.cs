using System.Threading.Tasks;
using Common.Paths;
using PowerScheme.Properties;
using RunAs.Common.Services;
using Updater.Common;

namespace PowerScheme.Services;

public sealed class UpdateService : ExecutorService, IUpdateService
{
    private const string NAME = "Updater";

    private static readonly string[] _arguments = [
                           "--api:\"https://api.github.com/repos/MACE4GITHUB/PowerScheme/releases/latest\"",
                           $"--path:\"{Default.ApplicationFileName}\"",
                           "--suffix:\"_new\"",
                           "--quit",
                           "--replace",
                           "--launchAfterUpdate"
                           ];

    public UpdateService() :
        base(
        NAME,
        string.Join(" ", _arguments),
        Resources.ResourceManager)
    {
        Runas = false;
    }

    public async Task<ReleaseInfo> GetReleaseInfoAsync()
    {
        var releaseInfo = await GetReleaseInfoAsync(_arguments);

        ReleaseInfo = releaseInfo;

        return releaseInfo;
    }

    public ReleaseInfo ReleaseInfo { get; private set; } = ReleaseInfo.Empty;

    public void Update() =>
        Execute();

    private static async Task<ReleaseInfo> GetReleaseInfoAsync(string[] args)
    {
        try
        {
            var arguments = Updater.Common.Arguments.Create(args);
            var releaseInfo = await ReleaseInfo.CreateAsync(arguments);

            return releaseInfo;
        }
        catch
        {
            return ReleaseInfo.Empty;
        }
    }
}
