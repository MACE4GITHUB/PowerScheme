using PowerScheme.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RunAs.Common.Utils;
using static PowerScheme.Program;
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

        private const string ADD_STABLE_TYPICAL_SCHEME_MENU = "AddStableTypicalSchemeToolStripMenuItem";
        private const string ADD_MEDIA_TYPICAL_SCHEME_MENU = "AddMediaTypicalSchemeToolStripMenuItem";
        private const string ADD_SIMPLE_TYPICAL_SCHEME_MENU = "AddSimpleTypicalSchemeToolStripMenuItem";

        private readonly Dictionary<string, string> ImageLidOn = new Dictionary<string, string>  { 
            { LID_NOTHING_MENU, "Nothing" },  
            { LID_SLEEP_MENU, "Sleep" },  
            { LID_HIBERNATE_MENU, "Hibernate" },  
            { LID_SHUTDOWN_MENU, "Shutdown" },  
        };

        private void BuildRightMenu() =>
            _form.InvokeIfRequired(BuildContextRightMenu);

        private void BuildContextRightMenu()
        {
            ViewModel.ContextRightMenu.Items.Clear();

            ViewModel.ContextRightMenu.Items.Add(MenuItemSettings());
            ViewModel.ContextRightMenu.Items.Add(MenuItemSepatator());

            ViewModel.ContextRightMenu.Items.Add(MenuItemStartWithWindows());

            if (_power.IsHibernate())
                ViewModel.ContextRightMenu.Items.Add(MenuItemHibernate());

            ViewModel.ContextRightMenu.Items.Add(MenuItemSleep());

            if (_power.IsMobilePlatformRole())
                ViewModel.ContextRightMenu.Items.Add(MenuItemLidOn());

            ViewModel.ContextRightMenu.Items.Add(MenuItemSepatator());
            ViewModel.ContextRightMenu.Items.Add(MenuItemExit());
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

            var lidItems = new List<ToolStripMenuItem>();
            var lidItem = new ToolStripMenuItem()
            {
                Name = LID_NOTHING_MENU,
                Tag = 0,
                Text = Language.DoNothing,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            lidItems.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SLEEP_MENU,
                Tag = 1,
                Text = Language.Sleep,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            lidItems.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_HIBERNATE_MENU,
                Tag = 2,
                Text = Language.Hibernate,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            lidItems.Add(lidItem);

            lidItem = new ToolStripMenuItem()
            {
                Name = LID_SHUTDOWN_MENU,
                Tag = 3,
                Text = Language.ShutDown,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            lidItem.Click += LidOnClick;
            lidItems.Add(lidItem);

            var lidItemsArray = lidItems.ToArray();

            item.DropDownItems.AddRange(lidItemsArray);

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
                _power.DeleteTypicalScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(DeleteTypicalScheme);
        }

        private void ItemCreateTypicalSchemes_Click(object sender, EventArgs e)
        {
            CreateTypicalSchemes();
        }

        private void CreateTypicalSchemes()
        {
            void CreateSchemes()
            {
                CreateMediaPowerScheme();
                CreateStablePowerScheme();
                CreateSimplePowerScheme();
            }

            _power.Watchers.RaiseActionWithoutWatchers(CreateSchemes);
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

        private void ExitOnClick(object sender, EventArgs e)
        {
            ViewModel.NotifyIcon.Visible = false;
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
            ViewModel.ContextLeftMenu.Items[STARTUP_ON_WINDOWS_MENU].Click -= StartWithWindowsOnClick;
            ViewModel.ContextLeftMenu.Items[SHOW_HIBERNATE_OPTION_MENU].Click -= HibernateOnClick;
            ViewModel.ContextLeftMenu.Items[SHOW_SLEEP_OPTION_MENU].Click -= SleepOnClick;
            ViewModel.ContextLeftMenu.Items[EXIT_MENU].Click -= ExitOnClick;

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
