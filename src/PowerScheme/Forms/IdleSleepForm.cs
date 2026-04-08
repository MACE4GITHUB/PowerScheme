using System;
using System.Windows.Forms;
using PowerScheme.Themes;

namespace PowerScheme.Forms;

public partial class IdleSleepForm : Form
{
    public IdleSleepForm()
    {
        InitializeComponent();
        KeyPreview = true;

        Activated += OnActivated;
        Deactivate += OnDeactivate;
        KeyDown += OnKeyDown;

        titleLabel.Text = sleepControl.Title;
    }

    private void OnActivated(object sender, EventArgs e)
    {
        ThemeService.ApplyControlTheme(this);
        sleepControl.OnShown();
    }

    private void OnDeactivate(object sender, EventArgs e)
    {
        WindowManager.HideForm<IdleSleepForm>();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            WindowManager.HideForm<IdleSleepForm>();
        }
    }
}
