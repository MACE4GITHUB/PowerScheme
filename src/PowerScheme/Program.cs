using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MessageForm;
using PowerScheme.Configuration;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme;

internal static class Program
{
    private static Mutex? _mutexObj;

    [STAThread]
    private static void Main()
    {
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
            _mutexObj = entry.Mutex;
        }

        Application.Run((ApplicationContext)DiRoot.GetService<IViewService>());

        _mutexObj?.Dispose();
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
