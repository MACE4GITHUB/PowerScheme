using System;
using System.Windows.Forms;
using Languages;
using PowerScheme.Themes;

namespace PowerScheme.Forms;

public partial class IdleForm : Form
{
    public IdleForm()
    {
        InitializeComponent();
        KeyPreview = true;

        Activated += OnActivated;
        Deactivate += OnDeactivate;
        KeyDown += OnKeyDown;

        titleLabel.Text = thresholdControl.Title;
    }

    private void OnActivated(object sender, EventArgs e)
    {
        ThemeService.ApplyControlTheme(this);
        thresholdControl.OnShown();
    }

    private void OnDeactivate(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleForm>();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            WindowManager.HideForm<IdleForm>();
        }
    }
}
