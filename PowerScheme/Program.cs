namespace PowerScheme
{
    using Configuration;
    using MessageForm;
    using Model;
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var entry = new EntryService(
                CompositionRoot.Resolve<IPowerSchemeService>(),
                CompositionRoot.Resolve<IMainMessageBox>()))
            {
                entry.Validate();
                _mutexObj = entry.Mutex;
            }

            Application.Run(new ViewService(CompositionRoot.Resolve<IViewModel>()));

            _mutexObj?.Dispose();
        }
    }
}
