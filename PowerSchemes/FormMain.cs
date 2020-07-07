using PowerSchemes.Model;
using PowerSchemes.Services;
using System.Windows.Forms;

namespace PowerSchemes
{
    public partial class FormMain : Form
    {
        private readonly IPowerSchemeService _power;
        private readonly ViewService _viewService;

        public FormMain()
        {
            InitializeComponent();

            _power = new PowerSchemeService();

            var viewModel =
                new ViewModel(notifyIcon, contextLeftMenuStrip, contextRightMenuStrip);

            _viewService = new ViewService(this, viewModel, _power);
            _viewService.Start();

        }
    }
}
