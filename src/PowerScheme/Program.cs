using System.Drawing;

namespace PowerScheme;

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
        var diConfigurator = new DiConfigurator();
        DiRoot.ConfigureServices(diConfigurator.Configure());

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