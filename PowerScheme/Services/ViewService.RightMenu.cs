using PowerScheme.Properties;
using PowerScheme.Utility;
using RegistryManager;
using RunAs.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Languages;
using PowerSchemeServiceAPI;
using static PowerScheme.Utility.TrayIcon;
using PowerSchemeServiceAPI.Model;

namespace PowerScheme.Services
{
    partial class ViewService
    {
        private const string RESTORE_ICON = "Restore";
        private const string CPL_WINDOWS_ICON = "Panel";
        private const string ADD_ICON = "Add";
        private const string DELETE_ICON = "Delete";

        private const string RADIO_ON_ICON = "RadioOn";
        private const string RADIO_OFF_ICON = "RadioOff";

        private const string STARTUP_ON_WINDOWS_MENU = "StartupOnWindowsToolStripMenuItem";
        private const string SHOW_HIBERNATE_OPTION_MENU = "ShowHibernateOptionToolStripMenuItem";
        private const string SHOW_SLEEP_OPTION_MENU = "ShowSleepOptionToolStripMenuItem";
        private const string EXIT_MENU = "ExitToolStripMenuItem";

        private const string LIDON_DROP_DOWN_MENU = "LidOnToolStripMenuItem";
        private const string LID_NOTHING_MENU = "LidNothingToolStripMenuItem";
        private const string LID_SLEEP_MENU = "LidSleepToolStripMenuItem";
        private const string LID_HIBERNATE_MENU = "LidHibernateToolStripMenuItem";
        private const string LID_SHUTDOWN_MENU = "LidShutdownToolStripMenuItem";

        private const string SETTINGS_DROP_DOWN_MENU = "SettingsToolStripMenuItem";

        private const string RESTORE_DEFAULT_POWER_SCHEMES_MENU = "RestoreDefaultPowerSchemesToolStripMenuItem";
        private const string CPL_SCHEME_WINDOWS_MENU = "CplSchemeWindowsToolStripMenuItem";
        private const string ADD_TYPICAL_SCHEMES_MENU = "AddTypicalSchemesToolStripMenuItem";
        private const string DELETE_TYPICAL_SCHEMES_MENU = "DeleteTypicalSchemesToolStripMenuItem";

        private static readonly Dictionary<string, string> IMAGE_LID_ON = new Dictionary<string, string>  {
            { LID_NOTHING_MENU, "Nothing" },
            { LID_SLEEP_MENU, "Sleep" },
            { LID_HIBERNATE_MENU, "Hibernate" },
            { LID_SHUTDOWN_MENU, "Shutdown" },
        };

        private void BuildRightMenu() =>
            _viewModel.ContextRightMenu.InvokeIfRequired(BuildContextRightMenu);

        private void BuildContextRightMenu()
        {
            _viewModel.ContextRightMenu.Items.Clear();

            _viewModel.ContextRightMenu.Items.Add(MenuItemSettings());
            _viewModel.ContextRightMenu.Items.Add(MenuItemSepatator());

            _viewModel.ContextRightMenu.Items.Add(MenuItemStartWithWindows());

            if (_power.ExistsHibernate)
                _viewModel.ContextRightMenu.Items.Add(MenuItemHibernate());

            _viewModel.ContextRightMenu.Items.Add(MenuItemSleep());

            if (_power.ExistsMobilePlatformRole)
                _viewModel.ContextRightMenu.Items.Add(MenuItemLidOn());

            _viewModel.ContextRightMenu.Items.Add(MenuItemSepatator());
            _viewModel.ContextRightMenu.Items.Add(MenuItemExit());
        }


        #region MenuItems

        private ToolStripMenuItem MenuItemSettings()
        {
            var itemDropDownSetting = new ToolStripMenuItem()
            {
                Name = SETTINGS_DROP_DOWN_MENU,
                Text = Language.Current.Settings
            };

            #region CplSchemeWindows
            var itemCplScheme = new ToolStripMenuItem()
            {
                Name = CPL_SCHEME_WINDOWS_MENU,
                Text = Language.Current.PowerOptions,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(CPL_WINDOWS_ICON)
            };
            itemCplScheme.Click += ItemCplScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemCplScheme);
            #endregion

            #region RestoreDefaultWindows
            var itemRestore = new ToolStripMenuItem()
            {
                Name = RESTORE_DEFAULT_POWER_SCHEMES_MENU,
                Text = Language.Current.RestoreDefaultPowerSchemesName,
                ToolTipText = Language.Current.RestoreDefaultPowerSchemesDescription,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(RESTORE_ICON)
            };
            itemRestore.Click += RestoreDefaultPowerSchemes_Click;

            itemDropDownSetting.DropDownItems.Add(itemRestore);
            #endregion

            itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

            #region AddTypicalSchemes
            var itemTypicalSchemes = new ToolStripMenuItem()
            {
                Name = ADD_TYPICAL_SCHEMES_MENU,
                Text = Language.Current.CreateTypicalSchemes,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(ADD_ICON)
            };
            itemTypicalSchemes.Click += ItemCreateTypicalSchemes_Click;

            itemDropDownSetting.DropDownItems.Add(itemTypicalSchemes);
            #endregion

            #region DeleteTypicalSchemes

            var itemDeleteTypicalSchemes = new ToolStripMenuItem()
            {
                Name = DELETE_TYPICAL_SCHEMES_MENU,
                Text = Language.Current.DeleteTypicalSchemes,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(DELETE_ICON)
            };
            itemDeleteTypicalSchemes.Click += ItemDeleteTypicalSchemes_Click;

            itemDropDownSetting.DropDownItems.Add(itemDeleteTypicalSchemes);
            #endregion

            itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

            foreach (var powerScheme in _power.TypicalPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = new StatePowerScheme(powerScheme, ActionWithPowerScheme.Create),
                    ImageScaling = ToolStripItemImageScaling.SizeToFit,
                    Image = GetImage(powerScheme.PictureName)
                };

                item.Click += ItemMenuActionPowerOnClick;

                itemDropDownSetting.DropDownItems.Add(item);
            }

            return itemDropDownSetting;
        }

        private void ItemMenuActionPowerOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;
            if (!(menu.Tag is StatePowerScheme tag)) return;
            _power.ActionPowerScheme(tag);
        }

        private ToolStripMenuItem MenuItemStartWithWindows()
        {
            var item = new ToolStripMenuItem
            {
                Name = STARTUP_ON_WINDOWS_MENU,
                Text = Language.Current.StartupOnWindows
            };

            item.Click += StartWithWindowsOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemHibernate()
        {
            var item = new ToolStripMenuItem
            {
                Name = SHOW_HIBERNATE_OPTION_MENU,
                Text = Language.Current.Hibernate,
                ToolTipText = Language.Current.HibernateDescription
            };

            item.Click += HibernateOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemSleep()
        {
            var item = new ToolStripMenuItem
            {
                Name = SHOW_SLEEP_OPTION_MENU,
                Text = Language.Current.Sleep,
                ToolTipText = Language.Current.SleepDescription
            };

            item.Click += SleepOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemExit()
        {
            var item = new ToolStripMenuItem
            {
                Name = EXIT_MENU,
                Text = Language.Current.Exit
            };

            item.Click += ExitOnClick;
            return item;
        }

        private ToolStripSeparator MenuItemSepatator()
            => new ToolStripSeparator();

        private ToolStripMenuItem MenuItemLidOn()
        {
            var item = new ToolStripMenuItem
            {
                Name = LIDON_DROP_DOWN_MENU,
                Text = Language.Current.WhenICloseTheLid
            };
            var itemsDropDown = item.DropDownItems;

            var lidItem = new ToolStripMenuItem()
            {
                Name = LID_NOTHING_MENU,
                Tag = 0,
                Text = Language.Current.DoNothing,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SLEEP_MENU,
                Tag = 1,
                Text = Language.Current.Sleep,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_HIBERNATE_MENU,
                Tag = 2,
                Text = Language.Current.Hibernate,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SHUTDOWN_MENU,
                Tag = 3,
                Text = Language.Current.ShutDown,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            return item;
        }

        private void LidOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem item)) return;
            if (!(item.Tag is int value)) return;
            var values = new[] { 0, 1, 2, 3 };

            if (!values.Contains(value)) return;

            _power.SetLid(value);
        }

        #endregion

        private void ItemDeleteTypicalSchemes_Click(object sender, EventArgs e)
        {
            void DeleteTypicalScheme()
            {
                _power.DeleteAllTypicalScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);
        }

        private void UpdateItemsTypicalScheme()
        {
            if (!(_viewModel.ContextRightMenu.Items[SETTINGS_DROP_DOWN_MENU] is ToolStripMenuItem settingsToolStripMenuItem)
            ) return;

            foreach (var itemMenu in settingsToolStripMenuItem.DropDownItems)
            {
                if (!(itemMenu is ToolStripMenuItem item)) continue;
                if (!(item.Tag is StatePowerScheme tag)) continue;

                item.Text = _power.TextActionToggle(tag);
                item.Tag = _power.StatePowerSchemeToggle(tag);
            }
        }

        private void ItemCreateTypicalSchemes_Click(object sender, EventArgs e)
        {
            _power.CreateTypicalSchemes();
        }
        
        private static void ItemCplScheme_Click(object sender, EventArgs e)
        {
            UACHelper.AttemptPrivilegeEscalation("powercfg.cpl");
        }

        private void ExitOnClick(object sender, EventArgs e)
        {
            _viewModel.NotifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void RestoreDefaultPowerSchemes_Click(object sender, EventArgs e)
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
            menu.Image = isChecked ? GetImage(CHECK_ICON) : null;
            return true;
        }

        private void UnsubscribeFromContextRightMenu()
        {
            _viewModel.ContextLeftMenu.Items[STARTUP_ON_WINDOWS_MENU].Click -= StartWithWindowsOnClick;
            _viewModel.ContextLeftMenu.Items[SHOW_HIBERNATE_OPTION_MENU].Click -= HibernateOnClick;
            _viewModel.ContextLeftMenu.Items[SHOW_SLEEP_OPTION_MENU].Click -= SleepOnClick;
            _viewModel.ContextLeftMenu.Items[EXIT_MENU].Click -= ExitOnClick;

            if (!(_viewModel.ContextRightMenu.Items[SETTINGS_DROP_DOWN_MENU] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            settingsToolStripMenuItem.DropDownItems[RESTORE_DEFAULT_POWER_SCHEMES_MENU].Click -= RestoreDefaultPowerSchemes_Click;
            settingsToolStripMenuItem.DropDownItems[CPL_SCHEME_WINDOWS_MENU].Click -= ItemCplScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ADD_TYPICAL_SCHEMES_MENU].Click -= ItemCreateTypicalSchemes_Click;
            settingsToolStripMenuItem.DropDownItems[DELETE_TYPICAL_SCHEMES_MENU].Click -= ItemDeleteTypicalSchemes_Click;

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
