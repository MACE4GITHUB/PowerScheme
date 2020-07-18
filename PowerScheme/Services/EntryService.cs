namespace PowerScheme.Services
{
    using FormAutoClose;
    using Languages;
    using Model;
    using Ninject;
    using PowerSchemeServiceAPI;
    using RegistryManager;
    using RunAs.Common;
    using RunAs.Common.Utils;
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Represents the application startup order. 
    /// </summary>
    internal sealed class EntryService : IDisposable
    {
        private const int RESTARTED_VALUE = 0;
        private IPowerSchemeService _power;

        private AppInfo _appInfo = new AppInfo();

        /// <summary>
        /// Determines the application startup order.
        /// </summary>
        public EntryService(IPowerSchemeService power)
        {
            ActionFirstStart = ShowFirstStartDialog;
            _power = power;
        }

        public Action ActionFirstStart { get; }

        public void Validate() 
            => ValidateOs().ValidateAdmin().ValidateOnceApplication().ValidateFirstStart();

        /// <summary>
        /// Gets Mutex to start one application instance.
        /// </summary>
        public Mutex Mutex { get; private set; }

        public bool IsValidateOs { get; set; } = true;

        public bool IsValidateAdmin { get; set; } = true;

        public bool IsValidateOnceApplication { get; set; } = true;

        public bool IsValidateFirstStart { get; set; } = true;
        
        private EntryService ValidateFirstStart()
        {
            if (!IsValidateFirstStart) return this;

            var isFirstStart = RegistryService.IsFirstStart(_appInfo.CompanyName, _appInfo.ProductName);
            if (!isFirstStart) return this;

            ActionFirstStart?.Invoke();

            RegistryService.SetAppSettings(_appInfo.CompanyName, _appInfo.ProductName, RESTARTED_VALUE);

            return this;
        }

        private EntryService ValidateAdmin()
        {
            if (!IsValidateAdmin) return this;

            var executorMainService = new ExecutorRunAsService($"{AttributeFile.Normal}");

            var isNeedAdminAccess = _power.IsNeedAdminAccessForChangePowerScheme;
            if (isNeedAdminAccess)
            {
                executorMainService.Execute();
                Environment.Exit(0);
            }
            else
            {
                executorMainService.RemoveIfExists();
            }

            return this;
        }

        private EntryService ValidateOs()
        {
            if (!IsValidateOs) return this;
            if (UACHelper.IsValidOs) return this;

            IFormAutoClose formAutoClose = new MessageBoxAutoClose(Language.Current.ApplicationLatter, Language.Current.Error, 15);
            formAutoClose.Show();

            Environment.Exit(-2);
            return this;
        }

        private void ShowFirstStartDialog()
        {
            var result = MessageBox.Show(Language.Current.FirstStartDescription, Language.Current.FirstStartCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;
            _power.CreateTypicalSchemes();
        }

        private EntryService ValidateOnceApplication()
        {
            if (!IsValidateOnceApplication) return this;

            OnceApplication();

            return this;
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

        #region IDisposable imlementation
        private bool _isDisposed;

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                Mutex = null;
                _power = null;
                _appInfo = null;
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
