using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace RunAs.Common.Utils
{
    /// <summary>
    /// Helper class for UAC based functions.
    /// </summary>
    public class UACHelper
    {
        [DllImport("user32")]
        private static extern UInt32 SendMessage
            (IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

        internal const int BCM_FIRST = 0x1600; //Normal button
        internal const int BCM_SETSHIELD = (BCM_FIRST + 0x000C); //Elevated button

        /// <summary>
        /// Add the UAC shield to the given Button.
        /// </summary>
        /// <param name="b">Button to add shield</param>
        public static void AddShieldToButton(Button b)
        {
            if (!IsWindowsVistaOrLater) return;
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BCM_SETSHIELD, 0, 0xFFFFFFFF);
        }

        /// <summary>
        /// Removes the UAC shield from the given Button.
        /// </summary>
        /// <param name="b">Button to remove shield</param>
        public static void RemoveShieldFromButton(Button b)
        {
            if (!IsWindowsVistaOrLater) return;
            b.FlatStyle = FlatStyle.System;
            SendMessage(b.Handle, BCM_SETSHIELD, 0, 0x0);
        }

        /// <summary>
        /// Determines if current user has admin privileges.
        /// </summary>
        /// <returns>true if does, false if not.</returns>
        public static bool HasAdminPrivileges()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Attempts to run the given process as an admim process.
        /// </summary>
        /// <param name="path">Full path to process</param>
        /// <param name="args">Arguments to process</param>
        /// <param name="windowStyle"></param>
        /// <param name="runas">true will use runas verb, false assumes manifest is part of process</param>
        public static void AttemptPrivilegeEscalation(string path, string args = "",
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal,
            bool runas = false,
            bool isWait = false)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!File.Exists(path))
            {
                var fullPath = Path.Combine(Environment.SystemDirectory, path);
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException("path");
                }

                path = fullPath;
            }


            // commented out so that we can call it no matter what
            //if (HasAdminPrivileges())
            //   throw new SecurityException("Already have administrator privileges.");


            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true, 
                FileName = path, 
                WindowStyle = windowStyle
            };

            // only do this for Vista+ since xp has an older runas dialog
            // also, runas set to false will assume that the application has a manifest and so we don't need this
            if (IsWindowsVistaOrLater && runas)// System.Environment.OSVersion.Version.Major >= 6 && runas)
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
                return; // By returning, we are ignoring the user tried to get UAC priviliges but then hit cancel at the "Run-As" prompt.
            }
        }

        /// <summary>
        /// Deteremines if current operating system is Vista+.
        /// </summary>
        public static bool IsWindowsVistaOrLater
            => Environment.OSVersion.Platform == PlatformID.Win32NT
               && Environment.OSVersion.Version >= new Version(6, 0, 6000);
    }
}
