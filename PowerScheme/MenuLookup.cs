namespace PowerScheme
{
    using Common;
    using Languages;
    using Model;
    using System.Collections.Generic;

    internal class MenuLookup
    {
        internal static readonly Dictionary<MenuItm, ViewMenu> MenuItems = new Dictionary<MenuItm, ViewMenu>
        {
            { MenuItm.StartupOnWindows, new ViewMenu(Language.Current.StartupOnWindows) },
            { MenuItm.Hibernate, new ViewMenu(Language.Current.HibernateName, Language.Current.HibernateDescription) },
            { MenuItm.Sleep, new ViewMenu(Language.Current.SleepName, Language.Current.SleepDescription) },
            { MenuItm.Lid, new ViewMenu(Language.Current.WhenICloseTheLid) },
            { MenuItm.Lid_Nothing, new ViewMenu(Language.Current.DoNothing, ImageItem.Nothing, 0) },
            { MenuItm.Lid_Sleep, new ViewMenu(Language.Current.SleepName, ImageItem.Sleep, 1) },
            { MenuItm.Lid_Hibernate, new ViewMenu(Language.Current.HibernateName, ImageItem.Hibernate, 2) },
            { MenuItm.Lid_Shutdown, new ViewMenu(Language.Current.ShutDown, ImageItem.Shutdown, 3) },
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
            Lid_Nothing,
            Lid_Sleep,
            Lid_Hibernate,
            Lid_Shutdown,
            Settings,
            RestoreDefaultPowerSchemes,
            ControlPanelSchemeWindows,
            CreateTypicalSchemes,
            DeleteTypicalSchemes,
            Exit
        }
    }
}
