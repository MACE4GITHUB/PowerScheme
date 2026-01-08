using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace RunAs.Common.Utils;

/// <summary>
/// Helper class for UAC based functions.
/// </summary>
public static class UACHelper
{
    /// <summary>
    /// Determines if current user has admin privileges.
    /// </summary>
    /// <returns>true if does, false if not.</returns>
    public static bool HasAdminPrivileges
    {
        get
        {
            bool isAdmin;
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                isAdmin = false;
            }

            return isAdmin;
        }
    }

    /// <summary>
    /// Attempts to run the given process as an admim process.
    /// </summary>
    /// <param name="path">Full path to process</param>
    /// <param name="args">Arguments to process</param>
    /// <param name="windowStyle"></param>
    /// <param name="runas">true will use runas verb, false assumes manifest is part of process</param>
    /// <param name="isWait"></param>
    public static void AttemptPrivilegeEscalation(string path, string args = "",
        ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
        bool runas = false,
        bool isWait = false)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
        {
            var fullPath = Path.Combine(Environment.SystemDirectory, path);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(fullPath);
            }

            path = fullPath;
        }
            
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true, 
            FileName = path, 
            WindowStyle = windowStyle
        };

        // only do this for Vista+ since xp has an older runas dialog
        // also, runas set to false will assume that the application has a manifest and so we don't need this
        if (IsValidOs && runas) 
        {
            startInfo.Verb = "runas"; // will bring up the UAC run-as menu when this ProcessStartInfo is used
        }

        if (!string.IsNullOrEmpty(args))
        {
            startInfo.Arguments = args;
        }

        try
        {
            using (var process = Process.Start(startInfo))
            {
                //block this UI until the launched process exits
                if (isWait)
                {
                    process?.WaitForExit();
                }
            }
        }

        catch (System.ComponentModel.Win32Exception) //occurs when the user has clicked Cancel on the UAC prompt.
        {
            // By returning, we are ignoring the user tried to get UAC priviliges but then hit cancel at the "Run-As" prompt.
        }
    }

    /// <summary>
    /// Deteremines if current operating system is Vista+.
    /// </summary>
    public static bool IsValidOs
        => Environment.OSVersion.Platform == PlatformID.Win32NT
           && Environment.OSVersion.Version.Major >= 6;
}