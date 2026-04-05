using System;
using System.Windows.Forms;
using PowerScheme.Themes;

namespace PowerScheme.Forms;

public partial class IdleDisplayForm : Form
{
    public IdleDisplayForm()
    {
        InitializeComponent();
        KeyPreview = true;

        Activated += OnActivated;
        Deactivate += OnDeactivate;
        KeyDown += OnKeyDown;

        titleLabel.Text = displayControl.Title;
    }

    private void OnActivated(object sender, EventArgs e)
    {
        ThemeService.ApplyControlTheme(this);
        displayControl.OnShown();
    }

    private void OnDeactivate(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleDisplayForm>();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            WindowManager.HideForm<IdleDisplayForm>();
        }
    }
}
