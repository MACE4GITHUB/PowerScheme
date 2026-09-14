using System;
using System.Net;
using System.Threading.Tasks;
using Logger;
using Updater.Common;

namespace Updater;

internal static class GitHubUpdater
{
    public static async Task Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

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
