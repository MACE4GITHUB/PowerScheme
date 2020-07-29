using System.ComponentModel;

namespace PowerScheme.Model
{
    using PowerSchemeServiceAPI;
    using PowerSchemeServiceAPI.Model;
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using static Utility.TrayIcon;

    public sealed class LeftContextMenu : ContextMainMenu
    {
        public LeftContextMenu(IContainer components, IPowerSchemeService power) : base(components, power)
        { }

        public override void UpdateMenu()
        {
            BuildMenu();
        }

        public override void ClearMenu()
        {
            if (Items.Count <= 0) return;

            for (var index = Items.Count - 1; index >= 0; index--)
            {
                var toolStripItem = Items[index];
                toolStripItem.Click -= ItemMenuPowerOnClick;
                toolStripItem.Tag = null;
                toolStripItem.Text = null;
                toolStripItem.Image = null;
                toolStripItem.Dispose();
            }
            Items.Clear();
        }

        protected override void BuildContextMenu()
        {
            ClearMenu();

            foreach (var powerScheme in Power.DefaultPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = new StatePowerScheme(powerScheme),
                    Text = powerScheme.Name,
                    Image = GetImage(powerScheme.Picture)
                };

                item.Click += ItemMenuPowerOnClick;

                Items.Add(item);
            }

            if (!Power.UserPowerSchemes.Any()) return;

            Items.Add(new ToolStripSeparator());

            foreach (var powerScheme in Power.UserPowerSchemes)
            {
                var item = new ToolStripMenuItem
                {
                    Tag = new StatePowerScheme(powerScheme),
                    Text = powerScheme.Name,
                    Image = GetImage(powerScheme.Picture)
                };

                item.Click += ItemMenuPowerOnClick;

                Items.Add(item);
            }

        }
        
        private void ItemMenuPowerOnClick(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem menu)) return;
            if (!(menu.Tag is StatePowerScheme statePowerScheme)) return;

            Power.SetActivePowerScheme(statePowerScheme.PowerScheme);
        }

        protected override void Dispose(bool disposing)
        {
            Power = null;
            base.Dispose(disposing);
        }
    }
}