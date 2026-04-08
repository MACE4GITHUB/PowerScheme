using System.Collections.Generic;
using Common;
using Languages;
using PowerScheme.Model;
using static PowerSchemeServiceAPI.SettingSchemeLookup;

namespace PowerScheme;

internal static class MenuLookup
{
    internal static readonly Dictionary<MenuItm, ViewMenu> MenuItems = new()
    {
        { MenuItm.High, new ViewMenu(SettingSchemes[SettingScheme.High].Name, ImageItem.High, SettingSchemes[SettingScheme.High].Guid, SettingSchemes[SettingScheme.High].Description, MenuItmKind.PowerScheme) },
        { MenuItm.Balance, new ViewMenu(SettingSchemes[SettingScheme.Balance].Name, ImageItem.Balance, SettingSchemes[SettingScheme.Balance].Guid, SettingSchemes[SettingScheme.Balance].Description, MenuItmKind.PowerScheme) },
        { MenuItm.Low, new ViewMenu(SettingSchemes[SettingScheme.Low].Name, ImageItem.Low, SettingSchemes[SettingScheme.Low].Guid, SettingSchemes[SettingScheme.Low].Description, MenuItmKind.PowerScheme) },
        { MenuItm.Stable, new ViewMenu(SettingSchemes[SettingScheme.Stable].Name, ImageItem.Stable, SettingSchemes[SettingScheme.Stable].Guid, SettingSchemes[SettingScheme.Stable].Description, MenuItmKind.PowerScheme)  },
        { MenuItm.Media, new ViewMenu(SettingSchemes[SettingScheme.Media].Name, ImageItem.Media, SettingSchemes[SettingScheme.Media].Guid, SettingSchemes[SettingScheme.Media].Description, MenuItmKind.PowerScheme) },
        { MenuItm.Simple, new ViewMenu(SettingSchemes[SettingScheme.Simple].Name, ImageItem.Simple, SettingSchemes[SettingScheme.Simple].Guid, SettingSchemes[SettingScheme.Simple].Description, MenuItmKind.PowerScheme) },
        { MenuItm.Extreme, new ViewMenu(SettingSchemes[SettingScheme.Extreme].Name, ImageItem.Extreme, SettingSchemes[SettingScheme.Extreme].Guid, SettingSchemes[SettingScheme.Extreme].Description, MenuItmKind.PowerScheme) },
        { MenuItm.StartupOnWindows, new ViewMenu(Language.Current.StartupOnWindows, ImageItem.Empty) },
        { MenuItm.NewVersionIsAvailable, new ViewMenu(Language.Current.NewVersionIsAvailable) },
        { MenuItm.UpdateApp, new ViewMenu(Language.Current.UpdateApp, ImageItem.Update) },
        { MenuItm.Hibernate, new ViewMenu(Language.Current.HibernateName, Language.Current.HibernateDescription) },
        { MenuItm.Sleep, new ViewMenu(Language.Current.SleepName, Language.Current.SleepDescription) },
        { MenuItm.Lid, new ViewMenu(Language.Current.WhenICloseTheLid) },
        { MenuItm.LidNothing, new ViewMenu(Language.Current.DoNothing, ImageItem.Nothing, Lid.Nothing, MenuItmKind.Lid) },
        { MenuItm.LidSleep, new ViewMenu(Language.Current.SleepName, ImageItem.Sleep, Lid.Sleep, MenuItmKind.Lid) },
        { MenuItm.LidHibernate, new ViewMenu(Language.Current.HibernateName, ImageItem.Hibernate, Lid.Hibernate, MenuItmKind.Lid) },
        { MenuItm.LidShutdown, new ViewMenu(Language.Current.ShutDown, ImageItem.Shutdown, Lid.Shutdown, MenuItmKind.Lid) },
        { MenuItm.IdleMonitoring, new ViewMenu(Language.Current.SwitchPowerSchemeWhenIdle) },
        { MenuItm.IdleDoNothing, new ViewMenu(Language.Current.DoNothing, ImageItem.Nothing) },
        { MenuItm.Settings, new ViewMenu(Language.Current.Settings) },
        { MenuItm.RestoreDefaultPowerSchemes, new ViewMenu(Language.Current.RestoreDefaultPowerSchemesName, ImageItem.Restore, Language.Current.RestoreDefaultPowerSchemesDescription) },
        { MenuItm.ControlPanelSchemeWindows, new ViewMenu(Language.Current.PowerOptions, ImageItem.Panel) },
        { MenuItm.CreateTypicalSchemes, new ViewMenu(Language.Current.CreateTypicalSchemes, ImageItem.Add) },
        { MenuItm.DeleteTypicalSchemes, new ViewMenu(Language.Current.DeleteTypicalSchemes, ImageItem.Delete) },
        { MenuItm.Quit, new ViewMenu(Language.Current.Quit, ImageItem.Empty) },
        { MenuItm.Version, new ViewMenu(Language.Current.Version, ImageItem.Empty) },
        { MenuItm.PowerSchemes, new ViewMenu(Language.Current.PowerSchemes, ImageItem.Empty) },
        { MenuItm.KeepBrightness, new ViewMenu(Language.Current.KeepBrightness, ImageItem.Empty, description: Language.Current.KeepBrightnessDescription) },
        { MenuItm.ShowIdleOptions, new ViewMenu(Language.Current.IdleOptions, ImageItem.Empty) },
        { MenuItm.ShowIdleDisplayOptions, new ViewMenu(Language.Current.IdleDisplayOptions, ImageItem.Empty) },
        { MenuItm.ShowIdleSleepOptions, new ViewMenu(Language.Current.IdleSleepOptions, ImageItem.Empty) },
        { MenuItm.Themes, new ViewMenu(Language.Current.Themes, ImageItem.Themes) },
        { MenuItm.Dark, new ViewMenu(Language.Current.Dark, ImageItem.Dark) },
        { MenuItm.Light, new ViewMenu(Language.Current.Light, ImageItem.Light) },
        { MenuItm.Blue, new ViewMenu(Language.Current.Blue, ImageItem.Blue) },
        { MenuItm.Green, new ViewMenu(Language.Current.Green, ImageItem.Green) },
    };

    internal enum MenuItm
    {
        Version,
        NewVersionIsAvailable,
        UpdateApp,
        StartupOnWindows,
        Hibernate,
        Sleep,
        Lid,
        LidNothing,
        LidSleep,
        LidHibernate,
        LidShutdown,
        IdleMonitoring,
        IdleDoNothing,
        KeepBrightness,
        Settings,
        RestoreDefaultPowerSchemes,
        ControlPanelSchemeWindows,
        CreateTypicalSchemes,
        DeleteTypicalSchemes,
        PowerSchemes,
        Quit,
        Stable,
        Media,
        Simple,
        Extreme,
        High,
        Balance,
        Low,
        ShowIdleOptions,
        ShowIdleDisplayOptions,
        ShowIdleSleepOptions,
        Themes,
        Dark,
        Light,
        Blue,
        Green
    }

    internal enum MenuItmKind
    {
        Unspecified = 0,
        PowerScheme = 1,
        Lid = 2
    }

    /// <summary>Order is important</summary>
    internal enum Lid
    {
        Nothing = 0,
        Sleep = 1,
        Hibernate = 2,
        Shutdown = 3
    }
}
