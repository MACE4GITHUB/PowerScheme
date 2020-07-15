using System.Collections.Generic;
using PowerScheme.Model;

namespace PowerScheme.Services
{
    using Common;
    using PowerSchemeServiceAPI;
    using PowerSchemeServiceAPI.Model;
    using Properties;
    using RegistryManager;
    using RunAs.Common.Utils;
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Utility;
    using static MenuLookup;
    using static Utility.TrayIcon;

    partial class ViewService
    {
        private void BuildRightMenu() =>
            _viewModel.ContextRightMenu.InvokeIfRequired(BuildContextRightMenu);

        private void BuildContextRightMenu()
        {
            ClearContextRightMenu();

            AddMenuItemSettings();
            AddMenuItemSepatator();
            AddMenuItemStartWithWindows();
            AddMenuItemHibernate();
            AddMenuItemSleep();
            AddMenuItemLid();
            AddMenuItemSepatator();
            AddMenuItemExit();
        }

        private void ClearContextRightMenu()
        {
            if (_viewModel.ContextRightMenu.Items.Count <= 0) return;

            UnsubscribeFromContextRightMenu();
        }
        #region MenuItems

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

            foreach (var powerScheme in _power.TypicalPowerSchemes)
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

            _viewModel.ContextRightMenu.Items.Add(itemDropDownSetting);
        }

        private void ItemMenuActionPowerOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;
            if (!(menu.Tag is StatePowerScheme tag)) return;
            _power.ActionPowerScheme(tag);
        }

        private void AddMenuItemStartWithWindows()
        {
            var item = new ToolStripMenuItem
            {
                Name = MenuItm.StartupOnWindows.ToString(),
                Text = MenuItems[MenuItm.StartupOnWindows].Name
            };

            item.Click += StartWithWindowsOnClick;
            _viewModel.ContextRightMenu.Items.Add(item);
        }

        private void AddMenuItemHibernate()
        {
            if (!_power.ExistsHibernate) return;

            var item = new ToolStripMenuItem
            {
                Name = MenuItm.Hibernate.ToString(),
                Text = MenuItems[MenuItm.Hibernate].Name,
                ToolTipText = MenuItems[MenuItm.Hibernate].Description
            };

            item.Click += HibernateOnClick;
            _viewModel.ContextRightMenu.Items.Add(item);
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
            _viewModel.ContextRightMenu.Items.Add(item);
        }

        private void AddMenuItemExit()
        {
            var item = new ToolStripMenuItem
            {
                Name = MenuItm.Exit.ToString(),
                Text = MenuItems[MenuItm.Exit].Name
            };

            item.Click += ExitOnClick;
            _viewModel.ContextRightMenu.Items.Add(item);
        }

        private void AddMenuItemSepatator()
            => _viewModel.ContextRightMenu.Items.Add(new ToolStripSeparator());


        private bool HasLidValue(KeyValuePair<MenuItm, ViewMenu> mi, int i)
        {
            if (!(mi.Value.Tag is Lid value)) return false;
            return (int)value == i;
        }

        private void AddMenuItemLid()
        {
            if (!_power.ExistsMobilePlatformRole) return;

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

            _viewModel.ContextRightMenu.Items.Add(item);
        }

        private void LidOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item)) return;
            if (!(item.Tag is Lid value)) return;
            
            _power.SetLid((int)value);
        }

        #endregion

        private void ItemDeleteTypicalSchemesOnClick(object sender, EventArgs e)
        {
            void DeleteTypicalScheme()
            {
                _power.DeleteAllTypicalScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);
        }

        private void UpdateItemsTypicalScheme()
        {
            if (!(_viewModel.ContextRightMenu.Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            foreach (var itemMenu in settingsToolStripMenuItem.DropDownItems)
            {
                if (!(itemMenu is ToolStripMenuItem item)) continue;
                if (!(item.Tag is StatePowerScheme tag)) continue;

                item.Text = _power.TextActionToggle(tag);
                item.Tag = _power.StatePowerSchemeToggle(tag);
            }
        }

        private void ItemCreateTypicalSchemesOnClick(object sender, EventArgs e)
        {
            _power.CreateTypicalSchemes();
        }

        private static void ItemCplSchemeOnClick(object sender, EventArgs e)
        {
            UACHelper.AttemptPrivilegeEscalation("powercfg.cpl");
        }

        private void ExitOnClick(object sender, EventArgs e)
        {
            _viewModel.NotifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void RestoreDefaultPowerSchemesOnClick(object sender, EventArgs e)
        {
            _power.RestoreDefaultPowerSchemes();
        }

        private static void StartWithWindowsOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            RegistryService.SetStartup(isChecked);
        }

        private void SleepOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? 1 : 0;
            RegistryService.SetSleepOption(Resources.ResourceManager, value);
        }

        private void HibernateOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? 1 : 0;
            RegistryService.SetHibernateOption(Resources.ResourceManager, value);
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

        private void UnsubscribeFromContextRightMenu()
        {
            _viewModel.ContextLeftMenu.Items[MenuItm.StartupOnWindows.ToString()].Click -= StartWithWindowsOnClick;
            _viewModel.ContextLeftMenu.Items[MenuItm.Hibernate.ToString()].Click -= HibernateOnClick;
            _viewModel.ContextLeftMenu.Items[MenuItm.Sleep.ToString()].Click -= SleepOnClick;
            _viewModel.ContextLeftMenu.Items[MenuItm.Exit.ToString()].Click -= ExitOnClick;

            if (!(_viewModel.ContextRightMenu.Items[MenuItm.Settings.ToString()] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            settingsToolStripMenuItem.DropDownItems[MenuItm.RestoreDefaultPowerSchemes.ToString()].Click -= RestoreDefaultPowerSchemesOnClick;
            settingsToolStripMenuItem.DropDownItems[MenuItm.ControlPanelSchemeWindows.ToString()].Click -= ItemCplSchemeOnClick;
            settingsToolStripMenuItem.DropDownItems[MenuItm.CreateTypicalSchemes.ToString()].Click -= ItemCreateTypicalSchemesOnClick;
            settingsToolStripMenuItem.DropDownItems[MenuItm.DeleteTypicalSchemes.ToString()].Click -= ItemDeleteTypicalSchemesOnClick;

            foreach (var itemMenu in settingsToolStripMenuItem.DropDownItems)
            {
                if (!(itemMenu is ToolStripMenuItem item)) continue;

                item.Text = null;
                item.Tag = null;
                item.Click -= ItemMenuActionPowerOnClick;
                item.Dispose();
            }

            settingsToolStripMenuItem.DropDownItems.Clear();
            _viewModel.ContextLeftMenu.Items.Clear();
        }
    }
}
