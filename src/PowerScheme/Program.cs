using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Languages;
using MessageForm;
using PowerScheme.Configuration;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerScheme.Utility;
using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme;

internal static class Program
{
    public static Mutex? OnceAppMutex { get; set; }

    internal static void LogMonitoringError(string stage, Exception exception) =>
        ErrorLog.Write($"[IdleMonitor:{stage}] {exception}");

    [STAThread]
    private static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.ThreadException += Application_ThreadException;

        if (!TryRunStartup(RunApplication, ShowErrorMessage))
        {
            Environment.ExitCode = 1;
        }
    }

    internal static bool TryRunStartup(Action startup, Action<string> reportError)
    {
        try
        {
            startup();
            return true;
        }
        catch (Exception exception)
        {
            reportError($"[Startup] {exception}");
            return false;
        }
        finally
        {
            OnceAppMutex?.Dispose();
            OnceAppMutex = null;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RunApplication()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        var savedLanguage = RegistryService.GetLanguageCode(AppInfo.CompanyName, AppInfo.ProductName);
        if (savedLanguage != null)
        {
            try
            {
                Language.SetLanguage(savedLanguage);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or XmlException or ArgumentException)
            {
                ErrorLog.Write($"Cannot restore language '{savedLanguage}': {exception}");
            }
        }

        _ = Language.Current.Parameters;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (var entry = new EntryService(
                   DiRoot.GetService<IPowerSchemeService>(),
                   DiRoot.GetService<IMainMessageBox>()))
        {
            entry.Validate();
            OnceAppMutex = entry.Mutex;
        }

        DiRoot.GetService<IPowerSchemeService>().RenameTypicalSchemes();

        Application.Run((ApplicationContext)DiRoot.GetService<IViewService>());
    }

    #region Exception Handlers

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) =>
        ShowError("ThreadException", e.Exception);

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
        ShowError("UnhandledException", e.ExceptionObject as Exception);

    private static void ShowError(string type, Exception? ex) =>
        ShowErrorMessage($"[{type}] {ex}");

    private static void ShowErrorMessage(string message)
    {
        ErrorLog.Write(message);
#if DEBUG
        MessageBox.Show(message, "Application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
    }

    #endregion
}
