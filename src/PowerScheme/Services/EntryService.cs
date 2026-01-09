namespace PowerScheme.Services;

using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Languages;
using MessageForm;
using Model;
using PowerSchemeServiceAPI;
using RegistryManager;
using RunAs.Common;
using RunAs.Common.Utils;

/// <summary>
/// Represents the application startup order.
/// </summary>
internal sealed class EntryService : IDisposable
{
    private const int RESTARTED_VALUE = 0;
    private IPowerSchemeService _power;
    private readonly IMainMessageBox _messageBox;
    private readonly string[] _args;

    private AppInfo _appInfo = new();

    /// <summary>
    /// Determines the application startup order.
    /// </summary>
    public EntryService(IPowerSchemeService power, IMainMessageBox messageBox)
    {
        ActionFirstStart = ShowFirstStartDialog;
        _power = power;
        _args = Environment.GetCommandLineArgs();
        _messageBox = messageBox;
    }

    public Action ActionFirstStart { get; }

    public void Validate()
        => ValidateOs().ValidateOnceApplication().ValidateAdmin().ValidateFirstStart();

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
        void ExitBecauseNotAdmin()
        {
            _messageBox.Show(Language.Current.CannotGetAdministratorRights,
                Language.Current.Error, MessageBoxButtons.OK, MessageBoxIcon.Error,
                isApplicationExit:true, timeout: 15);
        }

        if (!IsValidateAdmin) return this;

        var executorMainService = new ExecutorRunAsService($"{Role.Admin} {AttributeFile.Normal}");

        var isNeedAdminAccess = _power.IsNeedAdminAccessForChangePowerScheme;
        if (isNeedAdminAccess)
        {
            if (_args.Length == 1)
            {
                var isRole = Enum.TryParse(_args[0], true, out Role role);
                if (isRole)
                {
                    if (role == Role.Admin)
                    {
                        ExitBecauseNotAdmin();
                    }
                }
            }

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
        if (UacHelper.IsValidOs) return this;

        _messageBox.Show(Language.Current.ApplicationLatter,
            Language.Current.Error,
            icon:MessageBoxIcon.Error,
            isApplicationExit: true,
            timeout: 15);

        return this;
    }

    private void ShowFirstStartDialog()
    {
        var result = _messageBox.Show(Language.Current.FirstStartDescription, Language.Current.FirstStartCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;
        _power.CreateTypicalSchemes();
    }

    private void OnceApplication()
    {
        var guid = Assembly.GetExecutingAssembly().GetType().GUID.ToString();
        Mutex = new Mutex(true, guid, out var onceApp);

        if (onceApp) return;

        _messageBox.Show(Language.Current.AlreadyRunning,
            Language.Current.Information, isApplicationExit: true, timeout:5);
    }

    private EntryService ValidateOnceApplication()
    {
        if (!IsValidateOnceApplication) return this;

        OnceApplication();

        return this;
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
            _messageBox.Dispose();
        }
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }
    #endregion
}