using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Logger;
using Updater.Common;

namespace Updater;

internal sealed class UpdaterService(
    ILogger logger,
    Arguments arguments,
    ReleaseInfo releaseInfo)
{
    internal Arguments Arguments => arguments;

    internal ReleaseInfo ReleaseInfo => releaseInfo;

    internal async Task ExecuteAsync()
    {
        logger.LogInfo($"Current version: {releaseInfo.LocalVersion}, Latest version: {releaseInfo.RemoteVersion}");

        if (releaseInfo.NewVersionAvailable)
        {
            KillOldVersion(arguments);

            logger.LogInfo("Downloading latest version...");
            await DownloadLatestAsync(releaseInfo.AssetUrl, arguments.DownloadFilePath);

            ReplaceOldVersion(arguments);

            LaunchAfterUpdate(arguments);
        }
        else
        {
            logger.LogInfo("Already up to date.");
        }
    }

    private void KillOldVersion(Arguments arguments)
    {
        if (arguments.KillOldVersion)
        {
            logger.LogInfo("Checking if old version is running...");
            KillRunningInstances(Path.GetFileNameWithoutExtension(arguments.LocalFilePath.Value));
        }
    }

    private void KillRunningInstances(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        foreach (var proc in processes)
        {
            try
            {
                logger.LogInfo($"Terminating process {proc.ProcessName} (PID {proc.Id})...");
                proc.Kill();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to terminate {proc.ProcessName}: {ex.Message}");
            }
        }
    }

    private static async Task DownloadLatestAsync(string url, DownloadFilePath downloadFilePath)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
        var data = await client.GetByteArrayAsync(url);
        File.WriteAllBytes(downloadFilePath.Value, data); // overwrite if exists
    }

    private void ReplaceOldVersion(Arguments arguments)
    {
        if (arguments.ReplaceOldVersion)
        {
            logger.LogInfo("Replacing old version with latest...");
            ReplaceFile(arguments.LocalFilePath, arguments.DownloadFilePath);
        }
        else
        {
            logger.LogInfo($"Latest version saved as '{arguments.Suffix}' file.");
        }
    }

    private static void ReplaceFile(LocalFilePath localFilePath, DownloadFilePath downloadFilePath)
    {
        if (File.Exists(localFilePath.Value))
        {
            File.Delete(localFilePath.Value);
        }

        File.Move(downloadFilePath.Value, localFilePath.Value);
    }

    private void LaunchAfterUpdate(Arguments arguments)
    {
        if (arguments.LaunchAfterUpdate)
        {
            logger.LogInfo("Launching latest version...");
            try
            {
                Process.Start(arguments.LocalFilePath.Value);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to launch application: {ex.Message}");
            }
        }
    }
}
