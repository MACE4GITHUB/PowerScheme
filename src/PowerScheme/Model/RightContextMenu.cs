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

namespace PowerScheme.Model;

internal sealed class RightContextMenu(
    IContainer components,
    IPowerSchemeService power,
    IUpdateService updateService)
    : ContextMainMenu(components, power)
{
    protected override void BuildContextMenu()
    {
        AddVersionMenu();
        AddSeparatorItem();
        AddWindowsStartupMenu();

        if (Power.ExistsHibernate)
        {
            AddHibernateMenu();
        }

        AddSleepMenu();

        if (Power.ExistsMobilePlatformRole)
        {
            AddLidMenu();
        }

        AddIdleMonitoringMenu();
        AddSeparatorItem();
        AddSettingsMenu();
        AddExitMenu();
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
        Items.ReplaceMenu(GetVersionMenu);
        Items.ReplaceMenu(GetWindowsStartupMenu);
        Items.ReplaceMenu(GetSettingsMenu);
        Items.ReplaceMenu(GetExitMenu);
        Items.ReplaceMenu(GetSleepMenu);
        Items.ReplaceMenu(GetIdleMonitoringMenu);

        if (Power.ExistsHibernate)
        {
            Items.ReplaceMenu(GetHibernateMenu);
        }

        if (Power.ExistsMobilePlatformRole)
        {
            Items.ReplaceMenu(GetLidMenu);
        }

        ThemeService.ApplyToolStripTheme(this);
    }

    #region MenuBuilders

    private void AddWindowsStartupMenu() =>
        Items.Add(WindowsStartupMenuBuilder.Empty);

    private static ToolStripMenuItem GetWindowsStartupMenu() =>
        new WindowsStartupMenuBuilder().Build();

    private void AddVersionMenu() =>
        Items.Add(VersionMenuBuilder.Empty);

    private ToolStripMenuItem GetVersionMenu() =>
        new VersionMenuBuilder(updateService).Build();

    private void AddHibernateMenu() =>
        Items.Add(HibernateMenuBuilder.Empty);

    private static ToolStripMenuItem GetHibernateMenu() =>
        new HibernateMenuBuilder().Build();

    private void AddSleepMenu() =>
        Items.Add(SleepMenuBuilder.Empty);

    private static ToolStripMenuItem GetSleepMenu() =>
        new SleepMenuBuilder().Build();

    private void AddExitMenu() =>
        Items.Add(QuitMenuBuilder.Empty);

    private static ToolStripMenuItem GetExitMenu() =>
        new QuitMenuBuilder().Build();

    private void AddSeparatorItem()
        => Items.Add(new ToolStripSeparator());

    private void AddSettingsMenu() =>
        Items.Add(SettingsMenuBuilder.Empty);

    private ToolStripMenuItem GetSettingsMenu() =>
        new SettingsMenuBuilder(Power).Build();

    private void AddLidMenu() =>
        Items.Add(LidMenuBuilder.Empty);

    private ToolStripMenuItem GetLidMenu() =>
        new LidMenuBuilder(Power).Build();

    private void AddIdleMonitoringMenu() =>
        Items.Add(IdleMonitoringMenuBuilder.Empty);

    private ToolStripMenuItem GetIdleMonitoringMenu() =>
        new IdleMonitoringMenuBuilder(Power).Build();

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearMenu();
        }

        base.Dispose(disposing);
    }
}
