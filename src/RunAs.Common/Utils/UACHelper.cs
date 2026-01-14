using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace RunAs.Common.Utils;

/// <summary>
/// Helper class for UAC based functions.
/// </summary>
public static class UacHelper
{
    /// <summary>
    /// Attempts to run the given process as an admin process.
    /// </summary>
    /// <param name="path">Full path to process</param>
    /// <param name="args">Arguments to process</param>
    /// <param name="windowStyle"></param>
    /// <param name="runas">true will use runas verb, false assumes manifest is part of process.</param>
    /// <param name="isWait"></param>
    public static void AttemptPrivilegeEscalation(
        string path,
        string args = "",
        ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
        bool runas = false,
        bool isWait = false)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var fullPath = GetFullPath(path);

        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = fullPath,
            WindowStyle = windowStyle,
            Verb = IsValidOs && runas
                ? "runas"
                : string.Empty,
            Arguments = !string.IsNullOrWhiteSpace(args)
                ? args
                : string.Empty
        };

        try
        {
            using var process = Process.Start(startInfo);

            // Block until the launched process exits.
            if (isWait)
            {
                process?.WaitForExit();
            }
        }
        catch (Win32Exception) //occurs when the user has clicked Cancel on the UAC prompt.
        {
            // By returning, we are ignoring the user tried
            // to get UAC privileges but then hit cancel at the "Run-As" prompt.
        }
    }

    /// <summary>
    /// Determines if current operating system is Vista and higher.
    /// </summary>
    /// <remarks>Only do this for Vista and higher since XP has an older runas dialog
    /// also, runas set to false will assume that the application has a manifest,
    /// and so we don't need this.</remarks>
    public static bool IsValidOs
        => Environment.OSVersion.Platform == PlatformID.Win32NT &&
           Environment.OSVersion.Version.Major >= 6;

    private static string GetFullPath(string path)
    {
        if (File.Exists(path))
        {
            return path;
        }

        var fullPath = Path.Combine(Environment.SystemDirectory, path);

        return File.Exists(fullPath)
            ? path
            : throw new FileNotFoundException(fullPath);
    }
}
