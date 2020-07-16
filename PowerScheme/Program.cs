using PowerScheme.Configuration;
using PowerScheme.Services;
using PowerSchemeServiceAPI;
using System;
using System.Threading;
using System.Windows.Forms;

namespace PowerScheme
{
    internal static class Program
    {
        private static Mutex _mutexObj;

        [STAThread]
        private static void Main(string[] args)
        {
            var applicationModule = new ApplicationModule();
            CompositionRoot.Wire(applicationModule);

            using (var entry = new EntryService(args)
            {
                Power = CompositionRoot.Resolve<IPowerSchemeService>()
            })
            {
                entry.Start();
                _mutexObj = entry.Mutex;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var viewService = CompositionRoot.Resolve<IViewService>())
            {
                viewService.Start();
                Application.Run();
                _mutexObj.Dispose();
            }
        }
    }
}
