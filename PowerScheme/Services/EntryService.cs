using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme.Services
{
    using FormAutoClose;
    using Languages;
    using Model;
    using Ninject;
    using RunAs;
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class EntryService : IDisposable
    {
        private const string RESTARTED_NAME = "ShowDialogFirstStart";
        private const string ADMIN = "admin";
        private const int RESTARTED_VALUE = 0;

        private readonly string[] _args;

        private AppInfo _appInfo = new AppInfo();

        public EntryService(string[] args)
        {
            _args = args;
            ActionFirstStart = ShowFirstStartDialog;
        }

        [Inject]
        public IPowerSchemeService Power { get; set; }

        public Action ActionFirstStart { get; }

        public void Start()
        {
            ValidateOs();
            ValidateOnceApplication();
            ValidateAdmin();
            ValidateFirstStart();
        }

        public bool IsValidateOs { get; set; } = true;

        public bool IsValidateOnceApplication { get; set; } = true;

        public bool IsValidateFirstStart { get; set; } = true;

        public bool IsValidateAdmin { get; set; } = true;

        public Mutex Mutex { get; private set; }

        private void ValidateFirstStart()
        {
            if (!IsValidateFirstStart) return;

            var isFirstStart = RegistryService.IsFirstStart(_appInfo.CompanyName, _appInfo.ProductName);
            if (!isFirstStart) return;

            ActionFirstStart?.Invoke();

            RegistryService.SetAppSettings(_appInfo.CompanyName, _appInfo.ProductName, RESTARTED_NAME, RESTARTED_VALUE);
        }

        private void ValidateAdmin()
        {
            if (!IsValidateAdmin) return;

            var executorMainService = new ExecutorRunAsService("admin");

            var isNeedAdminAccess = Power.IsNeedAdminAccessForChangePowerScheme;
            if (isNeedAdminAccess)
            {
                executorMainService.Execute();
                Environment.Exit(0);
            }
            else
            {
                executorMainService.RemoveIfExists();
            }
        }

        private void ValidateOs()
        {
            if (!IsValidateOs) return;
            if (IsValidOs) return;

            IFormAutoClose formAutoClose = new MessageBoxAutoClose(Language.Current.ApplicationLatter, Language.Current.Error, 15);
            formAutoClose.Show();

            Environment.Exit(-2);
        }

        private static bool IsValidOs =>
            Environment.OSVersion.Platform == PlatformID.Win32NT
            && Environment.OSVersion.Version.Major >= 6;

        private void ShowFirstStartDialog()
        {
            var result = MessageBox.Show(Language.Current.FirstStartDescription, Language.Current.FirstStartCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;
            Power.CreateTypicalSchemes();
        }

        private void ValidateOnceApplication()
        {
            if (!IsValidateOnceApplication) return;

            if (_args.Length == 0 || _args[0].ToLowerInvariant() != ADMIN) OnceApplication();

            var roleAdmin = User.IsUserAdministrator();
            if (roleAdmin) OnceApplication();
        }

        private void OnceApplication()
        {
            var guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            Mutex = new Mutex(true, guid, out var onceApp);

            if (onceApp)
            {
                // do nothing
            }
            else
            {
                IFormAutoClose formAutoClose = new MessageBoxAutoClose(Language.Current.AlreadyRunning, Language.Current.Error, 5);
                formAutoClose.Show();

                Environment.Exit(-1);
            }
        }

        private bool _isDisposed;

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                Mutex = null;
                Power = null;
                _appInfo = null;
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
