using System.ComponentModel;
using System.Windows.Forms;
using PowerScheme.Model.Menu;
using PowerScheme.Model.Menu.Hibernate;
using PowerScheme.Model.Menu.IdleMonitoring;
using PowerScheme.Model.Menu.Lid;
using PowerScheme.Model.Menu.Quit;
using PowerScheme.Model.Menu.Settings;
using PowerScheme.Model.Menu.Sleep;
using PowerScheme.Model.Menu.Version;
using PowerScheme.Model.Menu.WindowsStartup;
using PowerScheme.Services;
using PowerScheme.Themes;
using PowerSchemeServiceAPI;
using static PowerScheme.MenuLookup;

namespace PowerScheme.Model;

internal sealed class RightContextMenu(
    IContainer components,
    IPowerSchemeService power,
    IUpdateService updateService)
    : ContextMainMenu(components, power)
{
    protected override void BuildContextMenu()
    {
        if (Items.Count != 0)
        {
            return;
        }

        Items.Add(VersionMenuBuilder.Empty);
        Items.Add(new ToolStripSeparator());
        Items.Add(WindowsStartupMenuBuilder.Empty);
        if (Power.ExistsHibernate)
        {
            Items.Add(HibernateMenuBuilder.Empty);
        }
        Items.Add(SleepMenuBuilder.Empty);
        if (Power.ExistsMobilePlatformRole)
        {
            Items.Add(LidMenuBuilder.Empty);
        }
        Items.Add(IdleMonitoringMenuBuilder.Empty);
        Items.Add(new ToolStripSeparator());
        Items.Add(SettingsMenuBuilder.Empty);
        Items.Add(QuitMenuBuilder.Empty);
    }

    internal override void ClearMenu()
    {
        if (Items.Count == 0)
        {
            return;
        }

        Items.RemoveMenu();

        Items.Clear();
    }

    internal override void UpdateMenu()
    {
        try
        {
            var menu = new MenuItemUpdater(Items);
            new VersionMenuBuilder(updateService, menu.Item(MenuItm.Version)).Build();
            menu.Separator();
            new WindowsStartupMenuBuilder(menu.Item(MenuItm.StartupOnWindows)).Build();
            if (Power.ExistsHibernate)
            {
                new HibernateMenuBuilder(menu.Item(MenuItm.Hibernate)).Build();
            }
            new SleepMenuBuilder(menu.Item(MenuItm.Sleep)).Build();
            if (Power.ExistsMobilePlatformRole)
            {
                new LidMenuBuilder(Power, menu.Item(MenuItm.Lid)).Build();
            }
            new IdleMonitoringMenuBuilder(Power, menu.Item(MenuItm.IdleMonitoring)).Build();
            menu.Separator();
            new SettingsMenuBuilder(Power, menu.Item(MenuItm.Parameters)).Build();
            new QuitMenuBuilder(menu.Item(MenuItm.Quit)).Build();
            menu.Complete();
            ThemeService.ApplyToolStripTheme(this);
        }
        catch
        {
            ClearMenu();
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearMenu();
        }

        base.Dispose(disposing);
    }
}
