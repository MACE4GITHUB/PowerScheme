using PowerSchemes.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using PowerSchemes.Properties;
using static PowerSchemes.Program;
using static PowerSchemes.Utility.TrayIcon;

namespace PowerSchemes.Services
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

        private const string STARTUP_ON_WINDOWS_MENU = "StartupOnWindowsToolStripMenuItem";
        private const string SHOW_HIBERNATE_OPTION_MENU = "ShowHibernateOptionToolStripMenuItem";
        private const string SHOW_SLEEP_OPTION_MENU = "ShowSleepOptionToolStripMenuItem";
        private const string EXIT_MENU = "ExitToolStripMenuItem";

        private const string SETTINGS_DROP_DOWN_MENU = "SettingsToolStripMenuItem";

        private const string RESTORE_DEFAULT_POWER_SCHEMES_MENU = "RestoreDefaultPowerSchemesToolStripMenuItem";
        private const string CPL_SCHEME_WINDOWS_MENU = "CplSchemeWindowsToolStripMenuItem";
        private const string ADD_TYPICAL_SCHEMES_MENU = "AddTypicalSchemesToolStripMenuItem";
        private const string DELETE_TYPICAL_SCHEMES_MENU = "DeleteTypicalSchemesToolStripMenuItem";

        private const string ADD_STABLE_TYPICAL_SCHEME_MENU = "AddStableTypicalSchemeToolStripMenuItem";
        private const string ADD_MEDIA_TYPICAL_SCHEME_MENU = "AddMediaTypicalSchemeToolStripMenuItem";
        private const string ADD_SIMPLE_TYPICAL_SCHEME_MENU = "AddSimpleTypicalSchemeToolStripMenuItem";

        private void BuildRightMenu() =>
            _form.InvokeIfRequired(BuildContextRightMenu);

        private void BuildContextRightMenu()
        {
            ViewModel.ContextRightMenu.Items.Clear();

            var stripRightMenuItems = new List<ToolStripItem>();

            #region RunOnStartup
            var itemStartup = new ToolStripMenuItem
            {
                Name = STARTUP_ON_WINDOWS_MENU,
                Text = Language.StartupOnWindows,
            };

            itemStartup.Click += StartWithWindowsToolStripMenuItem_Click;
            stripRightMenuItems.Add(itemStartup);
            #endregion

            #region RunShowHibernateOption
            var itemShowHibernateOption = new ToolStripMenuItem
            {
                Name = SHOW_HIBERNATE_OPTION_MENU,
                Text = Language.ShowHibernateOptionName,
                ToolTipText = Language.ShowHibernateOptionDescription,
            };

            itemShowHibernateOption.Click += ItemShowHibernateOption_Click;
            stripRightMenuItems.Add(itemShowHibernateOption);
            #endregion

            #region RunShowSleepOption
            var itemShowSleepOption = new ToolStripMenuItem
            {
                Name = SHOW_SLEEP_OPTION_MENU,
                Text = Language.ShowSleepOptionName,
                ToolTipText = Language.ShowSleepOptionDescription,
            };

            itemShowSleepOption.Click += ItemShowSleepOption_Click;
            stripRightMenuItems.Add(itemShowSleepOption);
            #endregion

            //var itemCaptionCover = new ToolStripMenuItem
            //{
            //    Text = "Действие при закрытии крышки:",
            //    Enabled = false
            //};

            //stripRightMenuItems.Add(itemCaptionCover);

            //#region MobileCloseCover
            //if (_power.IsMobilePlatformRole())
            //{
            //    var itemCover = new ToolStripComboBox();
            //    {

            //        //Text = Language.StartupOnWindows,
            //        //Name = STARTUP_ON_WINDOWS_MENU,
            //        //Tag = isOnStartup,
            //        //Image = isOnStartup ? GetImage("Check") : null
            //    };

            //    //itemStartup.Click += StartWithWindowsToolStripMenuItem_Click;
            //    stripRightMenuItems.Add(itemCover);
            //}
            //#endregion

            #region DropDownSettings
            var itemDropDownSetting = new ToolStripMenuItem()
            {
                Name = SETTINGS_DROP_DOWN_MENU,
                Text = Language.Settings
            };

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

            #region CplSchemeWindows
            var itemCplScheme = new ToolStripMenuItem()
            {
                Name = CPL_SCHEME_WINDOWS_MENU,
                Text = Language.ShowCplName,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(CPL_WINDOWS_ICON)
            };
            itemCplScheme.Click += ItemCplScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemCplScheme);
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
                Name = ADD_STABLE_TYPICAL_SCHEME_MENU,
                Text = Language.CreateStableScheme,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(STABLE_ICON)
            };
            itemStableScheme.Click += ItemStableScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemStableScheme);
            #endregion

            #region AddMediaFromBalance
            var itemMediaScheme = new ToolStripMenuItem()
            {
                Name = ADD_MEDIA_TYPICAL_SCHEME_MENU,
                Text = Language.CreateMediaScheme,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(MEDIA_ICON)
            };
            itemMediaScheme.Click += ItemMediaScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemMediaScheme);
            #endregion

            #region AddSimpleFromLow
            var itemSimpleScheme = new ToolStripMenuItem()
            {
                Name = ADD_SIMPLE_TYPICAL_SCHEME_MENU,
                Text = Language.CreateSimpleScheme,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Image = GetImage(SIMPLE_ICON)
            };
            itemSimpleScheme.Click += ItemSimpleScheme_Click;

            itemDropDownSetting.DropDownItems.Add(itemSimpleScheme);
            #endregion

            stripRightMenuItems.Add(itemDropDownSetting);
            #endregion

            stripRightMenuItems.Add(new ToolStripSeparator());

            #region Exit
            var itemExit = new ToolStripMenuItem
            {
                Name = EXIT_MENU,
                Text = Language.Exit
            };

            itemExit.Click += ExitStripMenuItem_Click;
            stripRightMenuItems.Add(itemExit);
            #endregion

            var stripRightMenuItemsArray = stripRightMenuItems.ToArray();

            ViewModel.ContextRightMenu.Items.AddRange(stripRightMenuItemsArray);
        }

        private void ItemDeleteTypicalSchemes_Click(object sender, EventArgs e)
        {
            void DeleteTypicalScheme()
            {
                _power.DeleteTypicalScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);
        }

        private void ItemCreateTypicalSchemes_Click(object sender, EventArgs e)
        {
            void CreateTypicalSchemes()
            {
                CreateMediaPowerScheme();
                CreateStablePowerScheme();
                CreateSimplePowerScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(CreateTypicalSchemes);
        }

        private void ItemMediaScheme_Click(object sender, EventArgs e)
            => CreateMediaPowerScheme();

        private void ItemStableScheme_Click(object sender, EventArgs e)
            => CreateStablePowerScheme();

        private void ItemSimpleScheme_Click(object sender, EventArgs e)
            => CreateSimplePowerScheme();

        private void CreateMediaPowerScheme()
            => _power.CreateMediaPowerScheme(Language.MediaName, Language.MediaDescription);

        private void CreateStablePowerScheme()
            => _power.CreateStablePowerScheme(Language.StableName, Language.StableDescription);

        private void CreateSimplePowerScheme()
            => _power.CreateSimplePowerScheme(Language.SimpleName, Language.SimpleDescription);

        private static void ItemCplScheme_Click(object sender, EventArgs e)
        {
            UACHelper.AttemptPrivilegeEscalation("powercfg.cpl");
        }

        private void ExitStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewModel.NotifyIcon.Visible = false;
            Environment.Exit(0);
        }

        private void RestoreDefaultPowerSchemes_Click(object sender, EventArgs e)
        {
            _power.RestoreDefaultPowerSchemes();
        }

        private static void StartWithWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            RegistryService.SetStartup(isChecked);
        }

        private void ItemShowSleepOption_Click(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? "1" : "0";
            RegistryService.SetSleepOption(value);
        }

        private void ItemShowHibernateOption_Click(object sender, EventArgs e)
        {
            if (!GetCheckedOption(sender, out var isChecked)) return;

            var value = isChecked ? "1" : "0";
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
            ViewModel.ContextLeftMenu.Items[STARTUP_ON_WINDOWS_MENU].Click -= StartWithWindowsToolStripMenuItem_Click;
            ViewModel.ContextLeftMenu.Items[SHOW_HIBERNATE_OPTION_MENU].Click -= ItemShowHibernateOption_Click;
            ViewModel.ContextLeftMenu.Items[SHOW_SLEEP_OPTION_MENU].Click -= ItemShowSleepOption_Click;
            ViewModel.ContextLeftMenu.Items[EXIT_MENU].Click -= ExitStripMenuItem_Click;

            if (!(ViewModel.ContextRightMenu.Items[SETTINGS_DROP_DOWN_MENU] is ToolStripMenuItem settingsToolStripMenuItem)) return;

            settingsToolStripMenuItem.DropDownItems[RESTORE_DEFAULT_POWER_SCHEMES_MENU].Click -= RestoreDefaultPowerSchemes_Click;
            settingsToolStripMenuItem.DropDownItems[CPL_SCHEME_WINDOWS_MENU].Click -= ItemCplScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ADD_TYPICAL_SCHEMES_MENU].Click -= ItemCreateTypicalSchemes_Click;
            settingsToolStripMenuItem.DropDownItems[DELETE_TYPICAL_SCHEMES_MENU].Click -= ItemDeleteTypicalSchemes_Click;
            settingsToolStripMenuItem.DropDownItems[ADD_STABLE_TYPICAL_SCHEME_MENU].Click -= ItemStableScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ADD_MEDIA_TYPICAL_SCHEME_MENU].Click -= ItemMediaScheme_Click;
            settingsToolStripMenuItem.DropDownItems[ADD_SIMPLE_TYPICAL_SCHEME_MENU].Click -= ItemSimpleScheme_Click;
        }

    }
}
