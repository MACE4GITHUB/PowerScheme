using System;
using System.Threading.Tasks;
using Logger;
using Updater.Common;

namespace Updater;

internal static class GitHubUpdater
{
    public static async Task Main(string[] args)
    {
        IWriter writer = new ConsoleWriter();
        ILogger logger = new Log(writer);
        try
        {
            var arguments = Arguments.Create(args);
            var releaseInfo = await ReleaseInfo.CreateAsync(arguments);

            var executorService = new UpdaterService(logger, arguments, releaseInfo);
            await executorService.ExecuteAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex.Message);
        }
    }
}
