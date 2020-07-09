using PowerScheme.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PowerScheme.Model;
using static PowerScheme.Utility.TrayIcon;

namespace PowerScheme.Services
{
    partial class ViewService
    {
        private void BuildLeftMenu() =>
            _form.InvokeIfRequired(BuildContextLeftMenu);

        private void BuildContextLeftMenu()
        {
            ClearContextLeftMenu();

            var stripLeftMenuItems = new List<ToolStripItem>();
            foreach (var powerScheme in _power.DefaultPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = powerScheme,
                    Text = powerScheme.Name,
                    Image = GetImage(powerScheme.Image)
                };

                item.Click += ItemMenuPowerOnClick;

                stripLeftMenuItems.Add(item);
            }

            if (_power.UserPowerSchemes.Any())
            {
                stripLeftMenuItems.Add(new ToolStripSeparator());

                foreach (var powerScheme in _power.UserPowerSchemes)
                {
                    var item = new ToolStripMenuItem
                    {
                        Tag = powerScheme,
                        Text = powerScheme.Name,
                        Image = GetImage(powerScheme.Image)
                    };

                    item.Click += ItemMenuPowerOnClick;

                    stripLeftMenuItems.Add(item);
                }
            }

            var stripLeftMenuItemsArray = stripLeftMenuItems.ToArray();
            ViewModel.ContextLeftMenu.Items.AddRange(stripLeftMenuItemsArray);
        }

        private void ClearContextLeftMenu()
        {
            if (ViewModel.ContextLeftMenu.Items.Count <= 0) return;

            UnsubscribeFromContextLeftMenu();

            ViewModel.ContextLeftMenu.Items.Clear();
        }

        private void UnsubscribeFromContextLeftMenu()
        {
            foreach (ToolStripItem toolStripItem in ViewModel.ContextLeftMenu.Items)
            {
                toolStripItem.Click -= ItemMenuPowerOnClick;
                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
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
