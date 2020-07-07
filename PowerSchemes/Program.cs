using FormAutoClose;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using PowerSchemes.Languages;

namespace PowerSchemes
{
    static class Program
    {
        private static Mutex _mutexObj;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OnceApplication();

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
                // LogsManager.Info("Приложение запущено");
            }
            else
            {
                IFormAutoClose formAutoClose = new MessageBoxAutoClose("Приложение уже было запущено.", "Ошибка.", 5);
                formAutoClose.Show();

                //LogsManager.Info("Предотвращена попытка запуска второй копии приложения");
                Environment.Exit(-1);
            }
        }

        public static Lang Language => Lang.SetLanguage();
    }
}
