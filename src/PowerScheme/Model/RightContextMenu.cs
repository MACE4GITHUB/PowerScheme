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
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

        if (Items.Count <= 0)
        {
            return;
        }

        // Unsubscribe handlers and dispose images/items deterministically.
        UnsubscribeAndDisposeItem(Items[nameof(MenuItm.StartupOnWindows)], StartWithWindowsOnClick);
        UnsubscribeAndDisposeItem(Items[nameof(MenuItm.Sleep)], SleepOnClick);
        UnsubscribeAndDisposeItem(Items[nameof(MenuItm.Exit)], ExitOnClick);

        if (Power.ExistsHibernate)
        {
            UnsubscribeAndDisposeItem(Items[nameof(MenuItm.Hibernate)], HibernateOnClick);
        }

        #region SettingsMenu
        if (Items[nameof(MenuItm.Settings)] is ToolStripMenuItem settingsToolStripMenuItem)
        {
            var settingDropDownItems = settingsToolStripMenuItem.DropDownItems;

            UnsubscribeAndDisposeItem(settingDropDownItems[nameof(MenuItm.RestoreDefaultPowerSchemes)],
                RestoreDefaultPowerSchemesOnClick);
            UnsubscribeAndDisposeItem(settingDropDownItems[nameof(MenuItm.ControlPanelSchemeWindows)],
                ItemCplSchemeOnClick);
            UnsubscribeAndDisposeItem(settingDropDownItems[nameof(MenuItm.CreateTypicalSchemes)],
                ItemCreateTypicalSchemesOnClick);
            UnsubscribeAndDisposeItem(settingDropDownItems[nameof(MenuItm.DeleteTypicalSchemes)],
                ItemDeleteTypicalSchemesOnClick);

            for (var index = settingDropDownItems.Count - 1; index >= 0; index--)
            {
                var item = settingDropDownItems[index];
                if (item is not ToolStripMenuItem toolStripItem)
                {
                    item.Dispose();
                    continue;
                }

                toolStripItem.Click -= ItemMenuActionPowerOnClick;
                // Dispose image if any.
                var img = toolStripItem.Image;
                if (img is not null)
                {
                    toolStripItem.Image = null;
                    img.Dispose();
                }

                toolStripItem.Tag = null;
                toolStripItem.Text = null;
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
                    item.Dispose();
                    continue;
                }

                toolStripItem.Click -= LidOnClick;

                var img = toolStripItem.Image;
                if (img is not null)
                {
                    toolStripItem.Image = null;
                    img.Dispose();
                }

                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Dispose();
            }

            if (lidDropDownItems.Count > 0)
            {
                lidDropDownItems.Clear();
            }
        }
        #endregion

        // Dispose images for root-level items and items themselves.
        for (var i = Items.Count - 1; i >= 0; i--)
        {
            var root = Items[i];
            if (root is ToolStripMenuItem tsm)
            {
                var img = tsm.Image;
                if (img is not null)
                {
                    tsm.Image = null;
                    img.Dispose();
                }
            }

            root.Dispose();
        }

        Items.Clear();
    }

    public override void UpdateMenu()
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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
            settingsToolStripMenuItem.DropDownItems[nameof(MenuItm.DeleteTypicalSchemes)]?.Visible = Power.TypicalPowerSchemes.Any();
            settingsToolStripMenuItem.DropDownItems[nameof(MenuItm.CreateTypicalSchemes)]?.Visible = !Power.ExistsAllTypicalScheme;
            UpdateItemsTypicalScheme();
        }

        CheckLid();
    }

    private void AddMenuItemSettings()
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

        var itemDropDownSetting = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.Settings),
            Text = MenuItems[MenuItm.Settings].Name
        };

        #region CplSchemeWindows
        var srcCpl = GetImage(MenuItems[MenuItm.ControlPanelSchemeWindows].Picture);
        var itemCplScheme = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.ControlPanelSchemeWindows),
            Text = MenuItems[MenuItm.ControlPanelSchemeWindows].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = CloneBitmap(srcCpl)
        };
        itemCplScheme.Click += ItemCplSchemeOnClick;

        itemDropDownSetting.DropDownItems.Add(itemCplScheme);
        #endregion

        #region RestoreDefaultWindows
        var srcRestore = GetImage(MenuItems[MenuItm.RestoreDefaultPowerSchemes].Picture);
        var itemRestore = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.RestoreDefaultPowerSchemes),
            Text = MenuItems[MenuItm.RestoreDefaultPowerSchemes].Name,
            ToolTipText = MenuItems[MenuItm.RestoreDefaultPowerSchemes].Description,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = CloneBitmap(srcRestore)
        };
        itemRestore.Click += RestoreDefaultPowerSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemRestore);
        #endregion

        itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

        #region AddTypicalSchemes
        var srcTypical = GetImage(MenuItems[MenuItm.CreateTypicalSchemes].Picture);
        var itemTypicalSchemes = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.CreateTypicalSchemes),
            Text = MenuItems[MenuItm.CreateTypicalSchemes].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = CloneBitmap(srcTypical)
        };
        itemTypicalSchemes.Click += ItemCreateTypicalSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemTypicalSchemes);
        #endregion

        #region DeleteTypicalSchemes
        var srcDelete = GetImage(MenuItems[MenuItm.DeleteTypicalSchemes].Picture);
        var itemDeleteTypicalSchemes = new ToolStripMenuItem
        {
            Name = nameof(MenuItm.DeleteTypicalSchemes),
            Text = MenuItems[MenuItm.DeleteTypicalSchemes].Name,
            ImageScaling = ToolStripItemImageScaling.SizeToFit,
            Image = CloneBitmap(srcDelete)
        };
        itemDeleteTypicalSchemes.Click += ItemDeleteTypicalSchemesOnClick;

        itemDropDownSetting.DropDownItems.Add(itemDeleteTypicalSchemes);
        #endregion

        itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

        foreach (var powerScheme in Power.TypicalPowerSchemes)
        {
            var src = GetImage(powerScheme.Picture);
            var item = new ToolStripMenuItem
            {
                Tag = new StatePowerScheme(powerScheme, ActionWithPowerScheme.Create),
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = CloneBitmap(src)
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
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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
            var src = GetImage(MenuItems.Where(mi =>
                HasLidValue(mi, i)).Select(mi => mi.Value.Picture).FirstOrDefault());

            var lidItem = new ToolStripMenuItem
            {
                Tag = (Lid)i,
                Text = MenuItems.Where(mi =>
                    HasLidValue(mi, i)).Select(mi => mi.Value.Name).FirstOrDefault(),
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = CloneBitmap(src)
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);
        }

        Items.Add(item);
    }

    private void LidOnClick(object? sender, EventArgs e)
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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


    private void RestoreDefaultPowerSchemesOnClick(object? sender, EventArgs e)
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

        Power.RestoreDefaultPowerSchemes();
    }

    private static void ItemCplSchemeOnClick(object? sender, EventArgs e)
    {
        UacHelper.AttemptPrivilegeEscalation("powercfg.cpl");
    }

    private void ItemCreateTypicalSchemesOnClick(object? sender, EventArgs e)
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

        Power.CreateTypicalSchemes();
    }

    private void ItemDeleteTypicalSchemesOnClick(object? sender, EventArgs e)
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

        Power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);

        return;

        void DeleteTypicalScheme()
        {
            Power.DeleteAllTypicalScheme();
        }
    }

    private static void StartWithWindowsOnClick(object? sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        RegistryService.SetStartup(isChecked);
    }

    private static void HibernateOnClick(object? sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        var value = isChecked ? 1 : 0;
        RegistryService.SetHibernateOption(Resources.ResourceManager, value);
    }

    private static void SleepOnClick(object? sender, EventArgs e)
    {
        if (!GetCheckedOption(sender, out var isChecked))
        {
            return;
        }

        var value = isChecked ? 1 : 0;
        RegistryService.SetSleepOption(Resources.ResourceManager, value);
    }

    private static void ExitOnClick(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    private void ItemMenuActionPowerOnClick(object? sender, EventArgs e)
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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

    private static bool GetCheckedOption(object? sender, out bool isChecked)
    {
        isChecked = false;
        var menu = sender as ToolStripMenuItem;

        if (menu?.Tag is not bool b)
        {
            return false;
        }

        isChecked = !b;
        menu.Tag = isChecked;

        // Dispose previous image before assigning a new one to avoid leaking.
        var prev = menu.Image;
        var next = isChecked ? GetImage(ImageItem.Check) : null;
        menu.Image = next;
        prev?.Dispose();

        return true;
    }

    private void CheckLid()
    {
        if (Power == null)
        {
            throw new ArgumentNullException(nameof(Power));
        }

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
            var valueTag = (int)(Lid)lidStripMenuItem.Tag!;
            var @checked = valueLidOn == valueTag;
            // Uncomment if you want to change the Image style
            //lidStripMenuItem.Image = GetImage(@checked ? ImageItem.RadioOn : ImageItem.RadioOff);
            if (@checked)
            {
                pictureName = MenuItems
                    .Where(mi => HasLidValue(mi, valueTag))
                    .Select(mi => mi.Value.Picture)
                    .FirstOrDefault();
            }
        }

        var prev = Items[nameof(MenuItm.Lid)]?.Image;
        var nextImg = GetImage(pictureName);
        Items[nameof(MenuItm.Lid)]?.Image = CloneBitmap(nextImg);
        prev?.Dispose();
    }

    private static void CheckMenu(ToolStripItem? item, bool @checked)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.Tag = @checked;

        var addShield = item.Name is nameof(MenuItm.Hibernate) or nameof(MenuItm.Sleep);

        var prev = item.Image;
        var next = GetImageIfCheck(@checked, addShield);
        item.Image = next;
        prev?.Dispose();
    }

    private static Bitmap? GetImageIfCheck(bool @checked, bool addShield)
    {
        var bitmap = GetImage(ImageItem.Check);
        if (!addShield)
        {
            return @checked ? bitmap : null;
        }

        var shield = GetImage(ImageItem.Shield);
        return @checked
            ? bitmap.CopyToSquareCanvas(Color.Transparent, shield)
            : shield;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearMenu();
            Power = null;
        }

        base.Dispose(disposing);
    }

    // Helper: clone a potentially shared Bitmap so the caller owns disposal.
    private static Bitmap? CloneBitmap(Bitmap? src) => src is null ? null : new Bitmap(src);

    // Helper: unsubscribe click and dispose a ToolStripItem (if exists).
    private static void UnsubscribeAndDisposeItem(ToolStripItem? item, EventHandler handler)
    {
        if (item is null)
        {
            return;
        }

        if (item is ToolStripMenuItem menuItem)
        {
            menuItem.Click -= handler;

            var img = menuItem.Image;
            if (img is not null)
            {
                menuItem.Image = null;
                img.Dispose();
            }

            menuItem.Tag = null;
            menuItem.Text = null;
            menuItem.Dispose();
            return;
        }

        item.Click -= handler;
        item.Dispose();
    }
}
