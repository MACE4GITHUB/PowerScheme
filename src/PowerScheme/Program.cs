using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MessageForm;
using PowerScheme.Configuration;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme;

internal static class Program
{
    private static Mutex? _mutexObj;

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.SetDefaultFont(
            new Font(new FontFamily("Microsoft Sans Serif"),
            8.25f,
            0,
            (GraphicsUnit)3));

        using (var entry = new EntryService(
                   DiRoot.GetService<IPowerSchemeService>(),
                   DiRoot.GetService<IMainMessageBox>()))
        {
            entry.Validate();
            _mutexObj = entry.Mutex;
        }

        Application.Run(new ViewService(DiRoot.GetService<IViewModel>()));

        _mutexObj?.Dispose();
    }
}
