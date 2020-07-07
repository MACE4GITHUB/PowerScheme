﻿using PowerSchemes.Model;
using PowerSchemes.Utility;
using RegistryManager.EventsArgs;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static PowerSchemes.Utility.TrayIcon;

namespace PowerSchemes.Services
{
    public partial class ViewService : IDisposable
    {
        private const string SHOW_CONTEXT_MENU = "ShowContextMenu";
        private const string CHECK_ICON = "Check";
        private const string STOP_ICON = "Stop";

        private readonly IPowerSchemeService _power;
        private readonly Form _form;

        public ViewService(Form form, ViewModel viewModel, IPowerSchemeService power)
        {
            _form = form;
            _power = power;
            ViewModel = viewModel;
        }

        public ViewModel ViewModel { get; }

        public void Start()
        {
            _power.Watchers.PowerSchemes.Changed += ChangedPowerSchemes;
            _power.Watchers.ActivePowerScheme.Changed += ChangedActivePowerScheme;

            _form.InvokeIfRequired(() =>
            {
                ViewModel.NotifyIcon.MouseClick += NotifyIcon_MouseClick;
                ViewModel.NotifyIcon.Visible = true;
            });

            BuildMenu();
            UpdateIcon();

            _power.Watchers.Start();
        }

        public void Stop()
        {
            _power.Watchers.PowerSchemes.Changed -= ChangedPowerSchemes;
            _power.Watchers.ActivePowerScheme.Changed -= ChangedActivePowerScheme;

            _form.InvokeIfRequired(() =>
            {
                _form.Icon = GetIcon(STOP_ICON);
                ViewModel.NotifyIcon.Icon = null;
                ViewModel.NotifyIcon.Text = string.Empty;
                ViewModel.NotifyIcon.Visible = false;
                ViewModel.NotifyIcon.MouseClick -= NotifyIcon_MouseClick;
            });

            _power.Watchers.Stop();
        }

        private void ChangedActivePowerScheme(object sender, RegistryChangedEventArgs e)
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            var activePowerScheme = _power.ActivePowerScheme;
            var image = activePowerScheme.Image;
            var icon = GetIcon(image);

            _form.InvokeIfRequired(() =>
            {
                _form.Icon = icon;
                ViewModel.NotifyIcon.Icon = icon;
                ViewModel.NotifyIcon.Text = activePowerScheme.Name;
            });
        }

        private void ChangedPowerSchemes(object sender, RegistryChangedEventArgs e)
        {
            BuildLeftMenu();
        }

        private void BuildMenu()
        {
            BuildLeftMenu();
            BuildRightMenu();
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            ViewModel.NotifyIcon.ContextMenuStrip =
                e.Button == MouseButtons.Right ?
                    ViewModel.ContextRightMenu :
                    ViewModel.ContextLeftMenu;

            if (e.Button == MouseButtons.Right)
            {
                CheckMenu(
                    ViewModel.ContextRightMenu.Items[STARTUP_ON_WINDOWS_MENU], 
                    RegistryService.IsRunOnStartup);

                CheckMenu(
                    ViewModel.ContextRightMenu.Items[SHOW_HIBERNATE_OPTION_MENU],
                    RegistryService.IsShowHibernateOption);

                CheckMenu(
                    ViewModel.ContextRightMenu.Items[SHOW_SLEEP_OPTION_MENU],
                    RegistryService.IsShowSleepOption);
            }

            var mi = typeof(NotifyIcon).GetMethod(SHOW_CONTEXT_MENU, BindingFlags.Instance | BindingFlags.NonPublic);
            mi?.Invoke(ViewModel.NotifyIcon, null);
            ViewModel.NotifyIcon.ContextMenuStrip = null;
        }

        private void CheckMenu(ToolStripItem item, bool @checked)
        {
            item.Tag = @checked;
            item.Image = GetImageIfCheck(@checked);
        }

        private Bitmap GetImageIfCheck(bool @checked)
        {
            return @checked ? GetImage(CHECK_ICON) : null;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) { return; }
            if (disposing)
            {
                Stop();
                UnsubscribeFromContextLeftMenu();
                UnsubscribeFromContextRightMenu();
            }

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);

            //No destructor so isn't required (yet)            
            // GC.SuppressFinalize(this); 
        }

        #endregion
    }
}
