using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Languages;
using MessageForm;
using PowerScheme.Configuration;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;
using RegistryManager;

namespace PowerScheme;

internal static class Program
{
    public static Mutex? OnceAppMutex { get; set; }

    [STAThread]
    private static void Main()
    {
        var savedLanguage = RegistryService.GetLanguage(AppInfo.CompanyName, AppInfo.ProductName);
        if (savedLanguage is { } languageKind)
        {
            Language.SetLanguage((LanguageKind)languageKind);
        }

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        Application.ThreadException += Application_ThreadException;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using (var entry = new EntryService(
                   DiRoot.GetService<IPowerSchemeService>(),
                   DiRoot.GetService<IMainMessageBox>()))
        {
            entry.Validate();
            OnceAppMutex = entry.Mutex;
        }

        Application.Run((ApplicationContext)DiRoot.GetService<IViewService>());

        OnceAppMutex?.Dispose();
    }

    #region Exception Handlers

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) =>
        ShowError("ThreadException", e.Exception);

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) =>
        ShowError("UnhandledException", e.ExceptionObject as Exception);

    private static void ShowError(string type, Exception? ex)
    {
        var message = $"[{type}] {ex?.Message}\n\n{ex?.StackTrace}";
#if DEBUG
        MessageBox.Show(message, "Application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        File.AppendAllText("errors.log", $"{DateTime.Now}: {message}{Environment.NewLine}");
    }

    #endregion
}
