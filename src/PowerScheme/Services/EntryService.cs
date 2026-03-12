using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Common.Paths;
using Languages;
using MessageForm;
using PowerScheme.Model;
using PowerSchemeServiceAPI;
using RegistryManager;
using RunAs.Common;
using RunAs.Common.Utils;

namespace PowerScheme.Services;

/// <summary>
/// Represents the application startup order.
/// </summary>
internal sealed class EntryService : IDisposable
{
    private const int RESTARTED_VALUE = 0;
    private readonly IPowerSchemeService _power;
    private readonly IMainMessageBox _messageBox;
    private readonly string[] _args;

    /// <summary>
    /// Determines the application startup order.
    /// </summary>
    public EntryService(
        IPowerSchemeService power,
        IMainMessageBox messageBox)
    {
        _power = power ?? throw new ArgumentNullException(nameof(power)); ;
        _args = Environment.GetCommandLineArgs();
        _messageBox = messageBox;
    }

    public void Validate() =>
        ValidateOs()
        .ValidateOnceApplication()
        .ValidateDirectoryPermissions()
        .ValidateRestartLimiter()
        .ValidateAdmin()
        .ValidateResetRestartLimiter()
        .ValidateFirstStart()
        .ValidateExistArtifacts();

    /// <summary>
    /// Gets Mutex to start one application instance.
    /// </summary>
    public Mutex? Mutex { get; private set; }

    public bool IsValidateOs { get; set; } = true;

    public bool IsValidateAdmin { get; set; } = true;

    public bool IsValidateOnceApplication { get; set; } = true;

    public bool IsValidateFirstStart { get; set; } = true;

    private EntryService ValidateFirstStart()
    {
        if (!IsValidateFirstStart)
        {
            return this;
        }

        var isFirstStart = RegistryService.IsFirstStart(AppInfo.CompanyName, AppInfo.ProductName);
        if (!isFirstStart)
        {
            return this;
        }

        ShowFirstStartDialog();

        RegistryService.SetAppSettings(AppInfo.CompanyName, AppInfo.ProductName, RESTARTED_VALUE);

        return this;
    }

    private EntryService ValidateAdmin()
    {
        if (!IsValidateAdmin)
        {
            return this;
        }

        var executorMainService = new RunAsExecutorService($"{Role.Admin} {AttributeFile.Normal}");

        var isNeedAdminAccess = _power.IsNeedAdminAccessForChangePowerScheme;

        if (isNeedAdminAccess)
        {
            if (_args.Length == 1)
            {
                var isRole = Enum.TryParse(_args[0], true, out Role role);
                if (isRole && role == Role.Admin)
                {
                    ExitBecauseNotAdmin();
                }
            }

            executorMainService.Execute();

            Environment.Exit(0);
        }
        else
        {
            executorMainService.RemoveFileIfExists();
        }

        return this;

        void ExitBecauseNotAdmin()
        {
            _messageBox.Show(
                Language.Current.CannotGetAdministratorRights,
                Language.Current.Error,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                isApplicationExit: true,
                timeout: 15);
        }
    }

    private EntryService ValidateOs()
    {
        if (!IsValidateOs || UacHelper.IsValidOs)
        {
            return this;
        }

        _messageBox.Show(Language.Current.ApplicationLatter,
            Language.Current.Error,
            icon: MessageBoxIcon.Error,
            isApplicationExit: true,
            timeout: 15);

        return this;
    }

    private EntryService ValidateRestartLimiter()
    {
        if (RestartLimiterService.CanRestart())
        {
            return this;
        }

        RestartLimiterService.Reset();

        Environment.Exit(0);

        return this;
    }

    private EntryService ValidateResetRestartLimiter()
    {
        RestartLimiterService.Reset();

        return this;
    }

    private void ShowFirstStartDialog()
    {
        var result = _messageBox.Show(
            Language.Current.FirstStartDescription,
            Language.Current.FirstStartCaption,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        _power.CreateTypicalSchemes();
    }

    private void OnceApplication()
    {
        var guid = AppInfo.ProductGuid;
        Mutex = new Mutex(true, guid, out var onceApp);

        if (onceApp)
        {
            return;
        }

        _messageBox.Show(
            Language.Current.AlreadyRunning,
            Language.Current.Information,
            isApplicationExit: true,
            timeout: 5);
    }

    private EntryService ValidateOnceApplication()
    {
        if (!IsValidateOnceApplication)
        {
            return this;
        }

        OnceApplication();

        return this;
    }

    private EntryService ValidateExistArtifacts()
    {
        string[] artifacts = [Default.UpdaterFileName, Default.RegWriterFileName];

        foreach (var artifact in artifacts.Where(File.Exists))
        {
            File.Delete(artifact);
        }

        return this;
    }

    private EntryService ValidateDirectoryPermissions()
    {
        try
        {
            var folderPath = Default.ApplicationPath;
            if (string.IsNullOrWhiteSpace(folderPath))
                return this;

            var root = Path.GetPathRoot(folderPath);
            if (string.IsNullOrEmpty(root))
                return this;

            var drive = new DriveInfo(root);

            if (!drive.IsReady)
                return this;

            var format = drive.DriveFormat;
            if (!SupportsAcl(format))
                return this;

            var dInfo = new DirectoryInfo(folderPath);
            if (!dInfo.Exists)
                return this;

            var dSecurity = dInfo.GetAccessControl();
            var authUserSid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            var rules = dSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

            var groupExists = rules
                .Cast<FileSystemAccessRule>()
                .Any(r => r.IdentityReference == authUserSid &&
                          (r.FileSystemRights & FileSystemRights.Modify) != 0 &&
                          r.AccessControlType == AccessControlType.Allow);

            if (!groupExists)
            {
                var accessRule = new FileSystemAccessRule(
                    authUserSid,
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                dSecurity.AddAccessRule(accessRule);
                dInfo.SetAccessControl(dSecurity);
            }
        }
        catch (UnauthorizedAccessException)
        {
            _messageBox.Show(
                Language.Current.RunAsAdministrator,
                Language.Current.Error,
                icon: MessageBoxIcon.Warning,
                isApplicationExit: true,
                timeout: 15);
        }
        catch (Exception)
        {
            // Do nothing
        }

        return this;
    }

    private static bool SupportsAcl(string driveFormat) =>
        driveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase) ||
        driveFormat.Equals("ReFS", StringComparison.OrdinalIgnoreCase);

    public void Dispose()
    {
        Mutex = null;
        _messageBox.Dispose();
    }
}
