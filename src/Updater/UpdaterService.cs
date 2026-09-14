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

            logger.LogInfo("Downloading installer...");
            await DownloadLatestAsync(releaseInfo.GetAssetDownloadUrl(arguments.ApiUrl), arguments.DownloadFilePath);

            RunInstaller(arguments);

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
            KillProcessById(arguments.ProcessId);
        }
    }

    private void KillProcessById(int processId)
    {
        if (processId <= 0)
        {
            logger.LogInfo("No valid process ID provided, skipping.");
            return;
        }

        try
        {
            var proc = Process.GetProcessById(processId);
            logger.LogInfo($"Terminating process {proc.ProcessName} (PID {proc.Id})...");
            proc.Kill();
            proc.WaitForExit();
        }
        catch (ArgumentException)
        {
            logger.LogInfo($"Process with PID {processId} not found (already stopped).");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to terminate process PID {processId}: {ex.Message}");
        }
    }

    private static async Task DownloadLatestAsync(string url, DownloadFilePath downloadFilePath)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/octet-stream");
        var data = await client.GetByteArrayAsync(url);
        File.WriteAllBytes(downloadFilePath.Value, data); // overwrite if exists

        if (!new SignatureVerifier().Verify(downloadFilePath.Value))
        {
            throw new InvalidOperationException("Downloaded file failed signature verification.");
        }
    }

    private void RunInstaller(Arguments arguments)
    {
        logger.LogInfo("Running silent installer (VERYSILENT)...");
        var startInfo = new ProcessStartInfo
        {
            FileName = arguments.DownloadFilePath.Value,
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART"
        };
        try
        {
            using var process = Process.Start(startInfo);
            process?.WaitForExit();
            if (process is not null && process.ExitCode != 0)
            {
                logger.LogError($"Installer exited with code {process.ExitCode}.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to run installer: {ex.Message}");
        }
    }

    private void LaunchAfterUpdate(Arguments arguments)
    {
        if (arguments.LaunchAfterUpdate)
        {
            logger.LogInfo("Launching application...");
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
