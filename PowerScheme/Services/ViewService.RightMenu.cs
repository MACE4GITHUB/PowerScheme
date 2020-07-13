using PowerScheme.Utility;
using RunAs.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static PowerScheme.Languages.Lang;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Services
{
    partial class ViewService
    {
        private const string RESTORE_ICON = "Restore";
        private const string CPL_WINDOWS_ICON = "Panel";
        private const string ADD_ICON = "Add";
        private const string DELETE_ICON = "Delete";

        private const string STABLE_ICON = "Stable";
        private const string MEDIA_ICON = "Media";
        private const string SIMPLE_ICON = "Simple";
        private const string EXTREME_ICON = "Extreme";

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

        private const string ACTION_STABLE_TYPICAL_SCHEME_MENU = "ActionStableTypicalSchemeToolStripMenuItem";
        private const string ACTION_MEDIA_TYPICAL_SCHEME_MENU = "ActionMediaTypicalSchemeToolStripMenuItem";
        private const string ACTION_SIMPLE_TYPICAL_SCHEME_MENU = "ActionSimpleTypicalSchemeToolStripMenuItem";
        private const string ACTION_EXTREME_TYPICAL_SCHEME_MENU = "ActionExtremeTypicalSchemeToolStripMenuItem";

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
                Text = Language.Settings
            };

            #region CplSchemeWindows
            var itemCplScheme = new ToolStripMenuItem()
            {
                Name = CPL_SCHEME_WINDOWS_MENU,
                Text = Language.PowerOptions,
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
                Text = Language.RestoreDefaultPowerSchemesName,
                ToolTipText = Language.RestoreDefaultPowerSchemesDescription,
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
                Text = Language.CreateTypicalSchemes,
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
                Text = Language.DeleteTypicalSchemes,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(DELETE_ICON)
            };
            itemDeleteTypicalSchemes.Click += ItemDeleteTypicalSchemes_Click;

            itemDropDownSetting.DropDownItems.Add(itemDeleteTypicalSchemes);
            #endregion

            itemDropDownSetting.DropDownItems.Add(new ToolStripSeparator());

            #region AddStableFromHigh
            var itemStableScheme = new ToolStripMenuItem()
            {
                Name = ACTION_STABLE_TYPICAL_SCHEME_MENU,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(STABLE_ICON)
            };
            itemStableScheme.Click += ItemStableScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemStableScheme);
            #endregion

            #region AddMediaFromBalance
            var itemMediaScheme = new ToolStripMenuItem()
            {
                Name = ACTION_MEDIA_TYPICAL_SCHEME_MENU,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(MEDIA_ICON)
            };
            itemMediaScheme.Click += ItemMediaScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemMediaScheme);
            #endregion

            #region AddSimpleFromLow
            var itemSimpleScheme = new ToolStripMenuItem()
            {
                Name = ACTION_SIMPLE_TYPICAL_SCHEME_MENU,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(SIMPLE_ICON)
            };
            itemSimpleScheme.Click += ItemSimpleScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemSimpleScheme);
            #endregion

            #region AddExtremeFromUltimate
            if (_power.CanCreateExtremePowerScheme)
            {
                var itemExtremeScheme = new ToolStripMenuItem()
                {
                    Name = ACTION_EXTREME_TYPICAL_SCHEME_MENU,
                    ImageScaling = ToolStripItemImageScaling.SizeToFit,
                    Image = GetImage(EXTREME_ICON)
                };
                itemExtremeScheme.Click += ItemExtremeScheme_Click;

                itemDropDownSetting.DropDownItems.Add(itemExtremeScheme);
            }
            #endregion

            return itemDropDownSetting;
        }

        private ToolStripMenuItem MenuItemStartWithWindows()
        {
            var item = new ToolStripMenuItem
            {
                Name = STARTUP_ON_WINDOWS_MENU,
                Text = Language.StartupOnWindows
            };

            item.Click += StartWithWindowsOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemHibernate()
        {
            var item = new ToolStripMenuItem
            {
                Name = SHOW_HIBERNATE_OPTION_MENU,
                Text = Language.Hibernate,
                ToolTipText = Language.HibernateDescription
            };

            item.Click += HibernateOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemSleep()
        {
            var item = new ToolStripMenuItem
            {
                Name = SHOW_SLEEP_OPTION_MENU,
                Text = Language.Sleep,
                ToolTipText = Language.SleepDescription
            };

            item.Click += SleepOnClick;
            return item;
        }

        private ToolStripMenuItem MenuItemExit()
        {
            var item = new ToolStripMenuItem
            {
                Name = EXIT_MENU,
                Text = Language.Exit
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
                Text = Language.WhenICloseTheLid
            };
            var itemsDropDown = item.DropDownItems;

            var lidItem = new ToolStripMenuItem()
            {
                Name = LID_NOTHING_MENU,
                Tag = 0,
                Text = Language.DoNothing,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SLEEP_MENU,
                Tag = 1,
                Text = Language.Sleep,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_HIBERNATE_MENU,
                Tag = 2,
                Text = Language.Hibernate,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            itemsDropDown.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SHUTDOWN_MENU,
                Tag = 3,
                Text = Language.ShutDown,
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
            var values = new int[] { 0, 1, 2, 3 };

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

            var existScheme = _power.ExistsStablePowerScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_STABLE_TYPICAL_SCHEME_MENU].Text =
                existScheme ? Language.DeleteStableScheme : Language.CreateStableScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_STABLE_TYPICAL_SCHEME_MENU].Tag =
                existScheme ? 0 : 1;

            existScheme = _power.ExistsMediaPowerScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_MEDIA_TYPICAL_SCHEME_MENU].Text =
                existScheme ? Language.DeleteMediaScheme : Language.CreateMediaScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_MEDIA_TYPICAL_SCHEME_MENU].Tag =
                existScheme ? 0 : 1;

            existScheme = _power.ExistsSimplePowerScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_SIMPLE_TYPICAL_SCHEME_MENU].Text =
                existScheme ? Language.DeleteSimpleScheme : Language.CreateSimpleScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_SIMPLE_TYPICAL_SCHEME_MENU].Tag =
                existScheme ? 0 : 1;

            existScheme = _power.ExistsExtremePowerScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_EXTREME_TYPICAL_SCHEME_MENU].Text =
                existScheme ? Language.DeleteExtremeScheme : Language.CreateExtremeScheme;
            settingsToolStripMenuItem.DropDownItems[ACTION_SIMPLE_TYPICAL_SCHEME_MENU].Tag =
                existScheme ? 0 : 1;
        }

        private void ItemCreateTypicalSchemes_Click(object sender, EventArgs e)
        {
            _power.CreateTypicalSchemes();
        }

        private void ItemMediaScheme_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;

            ToggleSchemeClick(ref menu,
                _power.CreateMediaPowerScheme,
                _power.DeleteMediaPowerScheme
            );
        }

        private void ItemStableScheme_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;

            ToggleSchemeClick(ref menu,
                _power.CreateStablePowerScheme,
                _power.DeleteStablePowerScheme
            );
        }

        private void ItemSimpleScheme_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;

            ToggleSchemeClick(ref menu,
                _power.CreateSimplePowerScheme,
                _power.DeleteSimplePowerScheme
            );
        }

        private void ItemExtremeScheme_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;

            ToggleSchemeClick(ref menu,
                _power.CreateExtremePowerScheme,
                _power.DeleteExtremePowerScheme
            );
        }

        private void ToggleSchemeClick(
            ref ToolStripMenuItem menu, 
            Action createScheme, Action deleteScheme)
        {
            var create = (int)menu.Tag == 1;
            if (create)
            {
                createScheme();
            }
            else
            {
                deleteScheme();
            }
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
            RegistryService.SetSleepOption(value);
        }

        private void HibernateOnClick(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? 1 : 0;
            RegistryService.SetHibernateOption(value);
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
            settingsToolStripMenuItem.DropDownItems[ACTION_STABLE_TYPICAL_SCHEME_MENU].Click -= ItemStableScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ACTION_MEDIA_TYPICAL_SCHEME_MENU].Click -= ItemMediaScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ACTION_SIMPLE_TYPICAL_SCHEME_MENU].Click -= ItemSimpleScheme_Click;

            if (_power.CanCreateExtremePowerScheme)
                settingsToolStripMenuItem.DropDownItems[ACTION_EXTREME_TYPICAL_SCHEME_MENU].Click -= ItemExtremeScheme_Click;
        }
    }
}
