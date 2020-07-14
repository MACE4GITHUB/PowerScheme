using PowerScheme.Configuration;
using PowerScheme.Services;
using System;
using System.Threading;
using System.Windows.Forms;
using PowerSchemeServiceAPI;

namespace PowerScheme
{
    static class Program
    {
        private static Mutex _mutexObj;

        [STAThread]
        static void Main(string[] args)
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

            IViewService viewService = CompositionRoot.Resolve<ViewService>();
            viewService.Start();

            Application.Run();
        }
    }
}
