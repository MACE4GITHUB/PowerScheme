using System.Collections.Generic;
using Common;
using Languages;
using PowerScheme.Model;

namespace PowerScheme;

internal static class MenuLookup
{
    internal static readonly Dictionary<MenuItm, ViewMenu> MenuItems = new()
    {
        { MenuItm.StartupOnWindows, new ViewMenu(Language.Current.StartupOnWindows) },
        { MenuItm.Hibernate, new ViewMenu(Language.Current.HibernateName, Language.Current.HibernateDescription) },
        { MenuItm.Sleep, new ViewMenu(Language.Current.SleepName, Language.Current.SleepDescription) },
        { MenuItm.Lid, new ViewMenu(Language.Current.WhenICloseTheLid) },
        { MenuItm.LidNothing, new ViewMenu(Language.Current.DoNothing, ImageItem.Nothing, Lid.Nothing) },
        { MenuItm.LidSleep, new ViewMenu(Language.Current.SleepName, ImageItem.Sleep, Lid.Sleep) },
        { MenuItm.LidHibernate, new ViewMenu(Language.Current.HibernateName, ImageItem.Hibernate, Lid.Hibernate) },
        { MenuItm.LidShutdown, new ViewMenu(Language.Current.ShutDown, ImageItem.Shutdown, Lid.Shutdown) },
        { MenuItm.Settings, new ViewMenu(Language.Current.Settings) },
        { MenuItm.RestoreDefaultPowerSchemes, new ViewMenu(Language.Current.RestoreDefaultPowerSchemesName, ImageItem.Restore, Language.Current.RestoreDefaultPowerSchemesDescription) },
        { MenuItm.ControlPanelSchemeWindows, new ViewMenu(Language.Current.PowerOptions, ImageItem.Panel) },
        { MenuItm.CreateTypicalSchemes, new ViewMenu(Language.Current.CreateTypicalSchemes, ImageItem.Add) },
        { MenuItm.DeleteTypicalSchemes, new ViewMenu(Language.Current.DeleteTypicalSchemes, ImageItem.Delete) },
        { MenuItm.Exit, new ViewMenu(Language.Current.Exit) }
    };

    internal enum MenuItm
    {
        StartupOnWindows,
        Hibernate,
        Sleep,
        Lid,
        LidNothing,
        LidSleep,
        LidHibernate,
        LidShutdown,
        Settings,
        RestoreDefaultPowerSchemes,
        ControlPanelSchemeWindows,
        CreateTypicalSchemes,
        DeleteTypicalSchemes,
        Exit
    }

    /// <summary>
    /// Order is important
    /// </summary>
    internal enum Lid
    {
        Nothing,
        Sleep,
        Hibernate,
        Shutdown
    }
}
