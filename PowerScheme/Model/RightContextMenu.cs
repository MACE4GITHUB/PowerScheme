using System.ComponentModel;

namespace PowerScheme.Model
{
    using Common;
    using PowerSchemeServiceAPI;
    using PowerSchemeServiceAPI.Model;
    using Properties;
    using RegistryManager;
    using RunAs.Common.Utils;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using static MenuLookup;
    using static Utility.TrayIcon;

    public sealed class RightContextMenu : ContextMainMenu
    {
        public RightContextMenu(IContainer components, IPowerSchemeService power) : base(components, power)
        { }

        protected override void BuildContextMenu()
        {
            AddMenuItemInfo();
            AddMenuItemSepatator();
            AddMenuItemStartWithWindows();
            AddMenuItemHibernate();
            AddMenuItemSleep();
            AddMenuItemLid();
            AddMenuItemSepatator();
            AddMenuItemSettings();
            AddMenuItemExit();
        }

        public override void ClearMenu()
        {
            if (Items.Count <= 0) return;

            Items[MenuItm.StartupOnWindows.ToString()].Click -= StartWithWindowsOnClick;
            Items[MenuItm.Hibernate.ToString()].Click -= HibernateOnClick;
            Items[MenuItm.Sleep.ToString()].Click -= SleepOnClick;
            Items[MenuItm.Exit.ToString()].Click -= ExitOnClick;

            if (!(Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            var dropDownItems = settingsToolStripMenuItem.DropDownItems;

            dropDownItems[MenuItm.RestoreDefaultPowerSchemes.ToString()].Click -= RestoreDefaultPowerSchemesOnClick;
            dropDownItems[MenuItm.ControlPanelSchemeWindows.ToString()].Click -= ItemCplSchemeOnClick;
            dropDownItems[MenuItm.CreateTypicalSchemes.ToString()].Click -= ItemCreateTypicalSchemesOnClick;
            dropDownItems[MenuItm.DeleteTypicalSchemes.ToString()].Click -= ItemDeleteTypicalSchemesOnClick;
            
            for (var index = dropDownItems.Count - 1; index >= 0; index--)
            {
                var item = dropDownItems[index];
                if (!(item is ToolStripMenuItem toolStripItem)) continue;

                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
                toolStripItem.Click -= ItemMenuActionPowerOnClick;
                toolStripItem.Dispose();
            }

            dropDownItems.Clear();
            Items.Clear();
        }

        public override void UpdateMenu()
        {
            CheckMenu(
                Items[MenuItm.StartupOnWindows.ToString()],
                RegistryService.IsRunOnStartup);

            if (Power.ExistsHibernate)
                CheckMenu(
                    Items[MenuItm.Hibernate.ToString()],
                    RegistryService.IsShowHibernateOption);

            CheckMenu(
                Items[MenuItm.Sleep.ToString()],
                RegistryService.IsShowSleepOption);

            if (Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)
            {
                settingsToolStripMenuItem.DropDownItems[MenuItm.DeleteTypicalSchemes.ToString()].Visible = Power.TypicalPowerSchemes.Any();
                settingsToolStripMenuItem.DropDownItems[MenuItm.CreateTypicalSchemes.ToString()].Visible = !Power.ExistsAllTypicalScheme;
                UpdateItemsTypicalScheme();
            }

            CheckLid();
        }

        private void AddMenuItemSettings()
        {
            var itemDropDownSetting = new ToolStripMenuItem()
            {
                Name = MenuItm.Settings.ToString(),
                Text = MenuItems[MenuItm.Settings].Name
            };

            #region CplSchemeWindows
            var itemCplScheme = new ToolStripMenuItem()
            {
                Name = MenuItm.ControlPanelSchemeWindows.ToString(),
                Text = MenuItems[MenuItm.ControlPanelSchemeWindows].Name,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(MenuItems[MenuItm.ControlPanelSchemeWindows].Picture)
            };
            itemCplScheme.Click += ItemCplSchemeOnClick;

            itemDropDownSetting.DropDownItems.Add(itemCplScheme);
            #endregion

            #region RestoreDefaultWindows
            var itemRestore = new ToolStripMenuItem()
            {
                Name = MenuItm.RestoreDefaultPowerSchemes.ToString(),
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
            var itemTypicalSchemes = new ToolStripMenuItem()
            {
                Name = MenuItm.CreateTypicalSchemes.ToString(),
                Text = MenuItems[MenuItm.CreateTypicalSchemes].Name,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(MenuItems[MenuItm.CreateTypicalSchemes].Picture)
            };
            itemTypicalSchemes.Click += ItemCreateTypicalSchemesOnClick;

            itemDropDownSetting.DropDownItems.Add(itemTypicalSchemes);
            #endregion

            #region DeleteTypicalSchemes

            var itemDeleteTypicalSchemes = new ToolStripMenuItem()
            {
                Name = MenuItm.DeleteTypicalSchemes.ToString(),
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
                Name = MenuItm.StartupOnWindows.ToString(),
                Text = MenuItems[MenuItm.StartupOnWindows].Name
            };

            item.Click += StartWithWindowsOnClick;
            Items.Add(item);
        }

        private void AddMenuItemInfo()
        {
            var info = new AppInfo();
            var item = new ToolStripMenuItem
            {
                Text = $@"{info.ProductName} {info.ProductVersion}"
            };

            Items.Add(item);
        }

        private void AddMenuItemHibernate()
        {
            if (!Power.ExistsHibernate) return;

            var item = new ToolStripMenuItem
            {
                Name = MenuItm.Hibernate.ToString(),
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
                Name = MenuItm.Sleep.ToString(),
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
                Name = MenuItm.Exit.ToString(),
                Text = MenuItems[MenuItm.Exit].Name
            };

            item.Click += ExitOnClick;
            Items.Add(item);
        }

        private void AddMenuItemSepatator()
            => Items.Add(new ToolStripSeparator());


        private bool HasLidValue(KeyValuePair<MenuItm, ViewMenu> mi, int i)
        {
            if (!(mi.Value.Tag is Lid value)) return false;
            return (int)value == i;
        }

        private void AddMenuItemLid()
        {
            if (!Power.ExistsMobilePlatformRole) return;

            var item = new ToolStripMenuItem
            {
                Name = MenuItm.Lid.ToString(),
                Text = MenuItems[MenuItm.Lid].Name
            };
            var itemsDropDown = item.DropDownItems;

            for (var i = 0; i < 4; i++)
            {
                var lidItem = new ToolStripMenuItem()
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
            if (!(sender is ToolStripMenuItem item)) return;
            if (!(item.Tag is Lid value)) return;

            Power.SetLid((int)value);
        }

        private void UpdateItemsTypicalScheme()
        {
            if (!(Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            foreach (var itemMenu in settingsToolStripMenuItem.DropDownItems)
            {
                if (!(itemMenu is ToolStripMenuItem item)) continue;
                if (!(item.Tag is StatePowerScheme tag)) continue;

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
            UACHelper.AttemptPrivilegeEscalation("powercfg.cpl");
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
            if (!GetCheckedOption(sender, out var isChecked)) return;

            RegistryService.SetStartup(isChecked);
        }

        private void HibernateOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? 1 : 0;
            RegistryService.SetHibernateOption(Resources.ResourceManager, value);
        }

        private void SleepOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? 1 : 0;
            RegistryService.SetSleepOption(Resources.ResourceManager, value);
        }

        private void ExitOnClick(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ItemMenuActionPowerOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;
            if (!(menu.Tag is StatePowerScheme tag)) return;
            Power.ActionPowerScheme(tag);
        }

        private static bool GetCheckedOption(object sender, out bool isChecked)
        {
            isChecked = false;
            var menu = (ToolStripMenuItem)sender;
            if (menu == null) return false;

            if (!(menu.Tag is bool b)) return false;
            isChecked = !b;
            menu.Tag = isChecked;
            menu.Image = isChecked ? GetImage(ImageItem.Check) : null;
            return true;
        }

        private void CheckLid()
        {
            if (!(Items[MenuItm.Lid.ToString()] is ToolStripMenuItem lidItems)) return;
            if (!Power.ExistsMobilePlatformRole) return;

            var any = Power.ActivePowerScheme.Guid;
            var valueLidOn = RegistryService.GetLidOption(any);
            var pictureName = ImageItem.Unknown;
            foreach (ToolStripMenuItem lidStripMenuItem in lidItems.DropDownItems)
            {
                var valueTag = (int)(Lid)lidStripMenuItem.Tag;
                var @checked = valueLidOn == valueTag;
                // Uncomment if you want to change the Image style
                //lidStripMenuItem.Image = GetImage(@checked ? ImageItem.RadioOn : ImageItem.RadioOff);
                if (@checked) pictureName = MenuItems.Where(mi =>
                    HasLidValue(mi, valueTag)).Select(mi => mi.Value.Picture).FirstOrDefault();
            }
            Items[MenuItm.Lid.ToString()].Image = GetImage(pictureName);
        }

        private static void CheckMenu(ToolStripItem item, bool @checked)
        {
            item.Tag = @checked;

            var addShield = item.Name == MenuItm.Hibernate.ToString() || item.Name == MenuItm.Sleep.ToString();

            item.Image = GetImageIfCheck(@checked, addShield);
        }

        private static Bitmap GetImageIfCheck(bool @checked, bool addShield)
        {
            var bitmap = GetImage(ImageItem.Check);
            if (!addShield) 
                return @checked ? bitmap : null;
            
            var shield = GetImage(ImageItem.Shield);
            return @checked ?  bitmap.CopyToSquareCanvas(Color.Transparent, shield): shield;
        }
    }
}
