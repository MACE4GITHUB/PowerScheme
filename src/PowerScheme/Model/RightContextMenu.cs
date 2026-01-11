using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common;
using PowerScheme.Properties;
using PowerSchemeServiceAPI;
using PowerSchemeServiceAPI.Model;
using RegistryManager;
using RunAs.Common.Utils;
using static PowerScheme.MenuLookup;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Model;

public sealed class RightContextMenu(
    IContainer components,
    IPowerSchemeService power) :
    ContextMainMenu(components, power)
{
    protected override void BuildContextMenu()
    {
        AddMenuItemInfo();
        AddMenuItemSeparator();
        AddMenuItemStartWithWindows();
        AddMenuItemHibernate();
        AddMenuItemSleep();
        AddMenuItemLid();
        AddMenuItemSeparator();
        AddMenuItemSettings();
        AddMenuItemExit();
    }

    public override void ClearMenu()
    {
        if (Items.Count <= 0)
        {
            return;
        }

        Items[nameof(MenuItm.StartupOnWindows)].Click -= StartWithWindowsOnClick;
        Items[nameof(MenuItm.Sleep)].Click -= SleepOnClick;
        Items[nameof(MenuItm.Exit)].Click -= ExitOnClick;

        if (Power.ExistsHibernate)
        {
            Items[nameof(MenuItm.Hibernate)].Click -= HibernateOnClick;
        }

        #region SettingsMenu
        if (Items[nameof(MenuItm.Settings)] is ToolStripMenuItem settingsToolStripMenuItem)
        {
            var settingDropDownItems = settingsToolStripMenuItem.DropDownItems;

            settingDropDownItems[nameof(MenuItm.RestoreDefaultPowerSchemes)].Click -=
                RestoreDefaultPowerSchemesOnClick;
            settingDropDownItems[nameof(MenuItm.ControlPanelSchemeWindows)].Click -= ItemCplSchemeOnClick;
            settingDropDownItems[nameof(MenuItm.CreateTypicalSchemes)].Click -=
                ItemCreateTypicalSchemesOnClick;
            settingDropDownItems[nameof(MenuItm.DeleteTypicalSchemes)].Click -=
                ItemDeleteTypicalSchemesOnClick;

            for (var index = settingDropDownItems.Count - 1; index >= 0; index--)
            {
                var item = settingDropDownItems[index];
                if (item is not ToolStripMenuItem toolStripItem)
                {
                    continue;
                }

                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
                toolStripItem.Click -= ItemMenuActionPowerOnClick;
                toolStripItem.Dispose();
            }

            if (settingDropDownItems.Count > 0)
            {
                settingDropDownItems.Clear();
            }
        }
        #endregion

        #region LidMenu
        if (Items[nameof(MenuItm.Lid)] is ToolStripMenuItem lidToolStripMenuItem)
        {
            var lidDropDownItems = lidToolStripMenuItem.DropDownItems;

            for (var index = lidDropDownItems.Count - 1; index >= 0; index--)
            {
                var item = lidDropDownItems[index];
                if (item is not ToolStripMenuItem toolStripItem)
                {
                    continue;
                }

                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
                toolStripItem.Click -= LidOnClick;
                toolStripItem.Dispose();
            }

            if (lidDropDownItems.Count > 0)
            {
                lidDropDownItems.Clear();
            }
        }
        #endregion

        Items.Clear();
    }

    public override void UpdateMenu()
    {
        CheckMenu(
            Items[nameof(MenuItm.StartupOnWindows)],
            RegistryService.IsRunOnStartup);

        if (Power.ExistsHibernate)
        {
            CheckMenu(
                Items[nameof(MenuItm.Hibernate)],
                RegistryService.IsShowHibernateOption);
        }

        CheckMenu(
            Items[nameof(MenuItm.Sleep)],
            RegistryService.IsShowSleepOption);

        if (Items[nameof(MenuItm.Settings)] is ToolStripMenuItem settingsToolStripMenuItem)
        {
            settingsToolStripMenuItem.DropDownItems[nameof(MenuItm.DeleteTypicalSchemes)].Visible = Power.TypicalPowerSchemes.Any();
            settingsToolStripMenuItem.DropDownItems[nameof(MenuItm.CreateTypicalSchemes)].Visible = !Power.ExistsAllTypicalScheme;
            UpdateItemsTypicalScheme();
        }

        CheckLid();
    }

    private void AddMenuItemSettings()
    {
        var itemDropDownSetting = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Settings),
            Text = MenuItems[MenuItm.Settings].Name
        };

        #region CplSchemeWindows
        var itemCplScheme = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.ControlPanelSchemeWindows),
            Text = MenuItems[MenuItm.ControlPanelSchemeWindows].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = GetImage(MenuItems[MenuItm.ControlPanelSchemeWindows].Picture)
        };
        itemCplScheme.Click += ItemCplSchemeOnClick;

        itemDropDownSetting.DropDownItems.Add(itemCplScheme);
        #endregion

        #region RestoreDefaultWindows
        var itemRestore = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.RestoreDefaultPowerSchemes),
            Text = MenuItems[MenuItm.RestoreDefaultPowerSchemes].Name,
            ToolTipText = MenuItems[MenuItm.RestoreDefaultPowerSchemes].Description,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = GetImage(MenuItems[MenuItm.RestoreDefaultPowerSchemes].Picture)
        };
        itemRestore.Click += RestoreDefaultPowerSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemRestore);
        #endregion

        itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

        #region AddTypicalSchemes
        var itemTypicalSchemes = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.CreateTypicalSchemes),
            Text = MenuItems[MenuItm.CreateTypicalSchemes].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = GetImage(MenuItems[MenuItm.CreateTypicalSchemes].Picture)
        };
        itemTypicalSchemes.Click += ItemCreateTypicalSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemTypicalSchemes);
        #endregion

        #region DeleteTypicalSchemes

        var itemDeleteTypicalSchemes = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.DeleteTypicalSchemes),
            Text = MenuItems[MenuItm.DeleteTypicalSchemes].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = GetImage(MenuItems[MenuItm.DeleteTypicalSchemes].Picture)
        };
        itemDeleteTypicalSchemes.Click += ItemDeleteTypicalSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemDeleteTypicalSchemes);
        #endregion

        itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

        foreach (var powerScheme in Power.TypicalPowerSchemes)
        {
            var item = new ToolStripMenuItem
            {
                Tag = new StatePowerScheme(powerScheme, ActionWithPowerScheme.Create),
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(powerScheme.Picture)
            };

            item.Click += ItemMenuActionPowerOnClick;

            itemDropDownSetting.DropDownItems.Add(item);
        }

        Items.Add(itemDropDownSetting);
    }

    private void AddMenuItemStartWithWindows()
    {
        var item = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.StartupOnWindows),
            Text = MenuItems[MenuItm.StartupOnWindows].Name
        };

        item.Click += StartWithWindowsOnClick;
        Items.Add(item);
    }

    private void AddMenuItemInfo()
    {
        var item = new ToolStripMenuItem
        {
            Text = $@"{AppInfo.ProductName} {AppInfo.ProductVersion}"
        };

        Items.Add(item);
    }

    private void AddMenuItemHibernate()
    {
        if (!Power.ExistsHibernate)
        {
            return;
        }

        var item = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Hibernate),
            Text = MenuItems[MenuItm.Hibernate].Name,
            ToolTipText = MenuItems[MenuItm.Hibernate].Description
        };

        item.Click += HibernateOnClick;
        Items.Add(item);
    }

    private void AddMenuItemSleep()
    {
        var item = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Sleep),
            Text = MenuItems[MenuItm.Sleep].Name,
            ToolTipText = MenuItems[MenuItm.Sleep].Description
        };

        item.Click += SleepOnClick;
        Items.Add(item);
    }

    private void AddMenuItemExit()
    {
        var item = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Exit),
            Text = MenuItems[MenuItm.Exit].Name
        };

        item.Click += ExitOnClick;
        Items.Add(item);
    }

    private void AddMenuItemSeparator()
        => Items.Add(new ToolStripSeparator());


    private static bool HasLidValue(KeyValuePair<MenuItm, ViewMenu> mi, int i)
    {
        if (mi.Value.Tag is not Lid value)
        {
            return false;
        }

        return (int)value == i;
    }

    private void AddMenuItemLid()
    {
        if (!Power.ExistsMobilePlatformRole)
        {
            return;
        }

        var item = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Lid),
            Text = MenuItems[MenuItm.Lid].Name
        };
        var itemsDropDown = item.DropDownItems;

        for (var i = 0; i < 4; i++)
        {
            var lidItem = new ToolStripMenuItem
            {
                Tag = (Lid)i,
                Text = MenuItems.Where(mi =>
                    HasLidValue(mi, i)).Select(mi => mi.Value.Name).FirstOrDefault(),
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(MenuItems.Where(mi =>
                    HasLidValue(mi, i)).Select(mi => mi.Value.Picture).FirstOrDefault())
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);
        }

        Items.Add(item);
    }

    private void LidOnClick(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem item)
        {
            return;
        }

        if (item.Tag is not Lid value)
        {
            return;
        }

        Power.SetLid((int)value);
    }

    private void UpdateItemsTypicalScheme()
    {
        if (Items[nameof(MenuItm.Settings)] is not ToolStripMenuItem settingsToolStripMenuItem)
        {
            return;
        }

        foreach (var itemMenu in settingsToolStripMenuItem.DropDownItems)
        {
            if (itemMenu is not ToolStripMenuItem item)
            {
                continue;
            }

            if (item.Tag is not StatePowerScheme tag)
            {
                continue;
            }

            item.Text = Power.TextActionToggle(tag);
            item.Tag = Power.StatePowerSchemeToggle(tag);
        }
    }


    private void RestoreDefaultPowerSchemesOnClick(object sender, EventArgs e)
    {
        Power.RestoreDefaultPowerSchemes();
    }

    private static void ItemCplSchemeOnClick(object sender, EventArgs e)
    {
        UacHelper.AttemptPrivilegeEscalation("powercfg.cpl");
    }

    private void ItemCreateTypicalSchemesOnClick(object sender, EventArgs e)
    {
        Power.CreateTypicalSchemes();
    }

    private void ItemDeleteTypicalSchemesOnClick(object sender, EventArgs e)
    {
        void DeleteTypicalScheme()
        {
            Power.DeleteAllTypicalScheme();
        }

        Power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);
    }

    private static void StartWithWindowsOnClick(object sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        RegistryService.SetStartup(isChecked);
    }

    private void HibernateOnClick(object sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        var value = isChecked ? 1 : 0;
        RegistryService.SetHibernateOption(Resources.ResourceManager, value);
    }

    private void SleepOnClick(object sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        var value = isChecked ? 1 : 0;
        RegistryService.SetSleepOption(Resources.ResourceManager, value);
    }

    private static void ExitOnClick(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void ItemMenuActionPowerOnClick(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem menu)
        {
            return;
        }

        if (menu.Tag is not StatePowerScheme tag)
        {
            return;
        }

        Power.ActionPowerScheme(tag);
    }

    private static bool GetCheckedOption(object sender, out bool isChecked)
    {
        isChecked = false;
        var menu = (ToolStripMenuItem)sender;
        if (menu == null)
        {
            return false;
        }

        if (menu.Tag is not bool b)
        {
            return false;
        }

        isChecked = !b;
        menu.Tag = isChecked;
        menu.Image = isChecked ? GetImage(ImageItem.Check) : null;
        return true;
    }

    private void CheckLid()
    {
        if (Items[nameof(MenuItm.Lid)] is not ToolStripMenuItem lidItems)
        {
            return;
        }

        if (!Power.ExistsMobilePlatformRole)
        {
            return;
        }

        var any = Power.ActivePowerScheme.Guid;
        var valueLidOn = RegistryService.GetLidOption(any);
        var pictureName = ImageItem.Unknown;
        foreach (ToolStripMenuItem lidStripMenuItem in lidItems.DropDownItems)
        {
            var valueTag = (int)(Lid)lidStripMenuItem.Tag;
            var @checked = valueLidOn == valueTag;
            // Uncomment if you want to change the Image style
            //lidStripMenuItem.Image = GetImage(@checked ? ImageItem.RadioOn : ImageItem.RadioOff);
            if (@checked)
            {
                pictureName = MenuItems.Where(mi =>
                    HasLidValue(mi, valueTag)).Select(mi => mi.Value.Picture).FirstOrDefault();
            }
        }
        Items[nameof(MenuItm.Lid)].Image = GetImage(pictureName);
    }

    private static void CheckMenu(ToolStripItem item, bool @checked)
    {
        item.Tag = @checked;

        var addShield = item.Name == nameof(MenuItm.Hibernate) || item.Name == nameof(MenuItm.Sleep);

        item.Image = GetImageIfCheck(@checked, addShield);
    }

    private static Bitmap GetImageIfCheck(bool @checked, bool addShield)
    {
        var bitmap = GetImage(ImageItem.Check);
        if (!addShield)
        {
            return @checked ? bitmap : null;
        }

        var shield = GetImage(ImageItem.Shield);
        return @checked ? bitmap.CopyToSquareCanvas(Color.Transparent, shield) : shield;
    }

    protected override void Dispose(bool disposing)
    {
        Power = null;
        base.Dispose(disposing);
    }
}