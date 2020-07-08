using FormAutoClose;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using PowerSchemes.Languages;
using RunAs;

namespace PowerSchemes
{
    static class Program
    {
        private static Mutex _mutexObj;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ValidateOs();
            if (args.Length == 0 || args[0] != "admin") OnceApplication();

            var roleAdmin = User.IsUserAdministrator();
            if (roleAdmin) OnceApplication();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        internal static void OnceApplication()
        {
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            _mutexObj = new Mutex(true, guid, out var onceApp);

            if (onceApp)
            {
                // 
            }
            else
            {
                IFormAutoClose formAutoClose = new MessageBoxAutoClose(Language.AlreadyRunning, Language.Error, 5);
                formAutoClose.Show();

                Environment.Exit(-1);
            }
        }

        private static void ValidateOs()
        {
            if (IsValidOs()) return;

            IFormAutoClose formAutoClose = new MessageBoxAutoClose(Language.ApplicationLatter, Language.Error, 15);
            formAutoClose.Show();

            Environment.Exit(-2);
        }

        private static bool IsValidOs()
        {
            return
                Environment.OSVersion.Platform == PlatformID.Win32NT
                && Environment.OSVersion.Version.Major >= 6;
        }

        public static Lang Language => Lang.SetLanguage();
    }
}
