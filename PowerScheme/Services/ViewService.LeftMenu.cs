using PowerScheme.Utility;
using PowerSchemeServiceAPI.Model;
using System;
using System.Linq;
using System.Windows.Forms;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Services
{
    partial class ViewService
    {
        private void BuildLeftMenu() 
            => _viewModel.ContextLeftMenu.InvokeIfRequired(BuildContextLeftMenu);

        private void BuildContextLeftMenu()
        {
            ClearContextLeftMenu();

            foreach (var powerScheme in _power.DefaultPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = powerScheme,
                    Text = powerScheme.Name,
                    Image = GetImage(powerScheme.Image)
                };

                item.Click += ItemMenuPowerOnClick;

                _viewModel.ContextLeftMenu.Items.Add(item);
            }

            if (!_power.UserPowerSchemes.Any()) return;

            _viewModel.ContextLeftMenu.Items.Add(new ToolStripSeparator());

            foreach (var powerScheme in _power.UserPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = powerScheme,
                    Text = powerScheme.Name,
                    Image = GetImage(powerScheme.Image)
                };

                item.Click += ItemMenuPowerOnClick;

                _viewModel.ContextLeftMenu.Items.Add(item);
            }

        }

        private void ClearContextLeftMenu()
        {
            if (_viewModel.ContextLeftMenu.Items.Count <= 0) return;

            UnsubscribeFromContextLeftMenu();

            _viewModel.ContextLeftMenu.Items.Clear();
        }

        private void UnsubscribeFromContextLeftMenu()
        {
            for (var index = _viewModel.ContextLeftMenu.Items.Count - 1; index >= 0; index--)
            {
                ToolStripItem toolStripItem = _viewModel.ContextLeftMenu.Items[index];
                toolStripItem.Click -= ItemMenuPowerOnClick;
                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
                toolStripItem.Dispose();
            }
        }

        private void ItemMenuPowerOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;
            if (!(menu.Tag is IPowerScheme powerScheme)) return;

            _power.SetActivePowerScheme(powerScheme);
        }
    }
}
