using System.Windows.Forms;

namespace PowerScheme.Forms;

public partial class IdleForm : Form
{
    public IdleForm()
    {
        InitializeComponent();
        PopupFormBehavior.Attach(this, thresholdControl.OnShown, WindowManager.HideForm<IdleForm>);

        titleLabel.Text = thresholdControl.Title;
    }
}
