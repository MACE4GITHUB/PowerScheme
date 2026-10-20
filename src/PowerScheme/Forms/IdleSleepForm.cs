using System.Windows.Forms;

namespace PowerScheme.Forms;

public partial class IdleSleepForm : Form
{
    public IdleSleepForm()
    {
        InitializeComponent();
        PopupFormBehavior.Attach(this, sleepControl.OnShown, WindowManager.HideForm<IdleSleepForm>);

        titleLabel.Text = sleepControl.Title;
    }
}
