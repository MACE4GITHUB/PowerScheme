using System.Windows.Forms;

namespace PowerScheme.Forms;

public partial class IdleDisplayForm : Form
{
    public IdleDisplayForm()
    {
        InitializeComponent();
        PopupFormBehavior.Attach(this, displayControl.OnShown, WindowManager.HideForm<IdleDisplayForm>);

        titleLabel.Text = displayControl.Title;
    }
}
