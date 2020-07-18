using PowerScheme.Model;

namespace PowerScheme
{
    using Configuration;
    using PowerSchemeServiceAPI;
    using Services;
    using System;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        private static Mutex _mutexObj;

        [STAThread]
        private static void Main()
        {
            var applicationModule = new ApplicationModule();
            CompositionRoot.Wire(applicationModule);

            using (var entry = new EntryService(CompositionRoot.Resolve<IPowerSchemeService>()))
            {
                entry.Validate();
                _mutexObj = entry.Mutex;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ViewService(CompositionRoot.Resolve<IViewModel>()));
            
            _mutexObj.Dispose();
        }
    }
}
