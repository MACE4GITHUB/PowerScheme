using System;
using System.IO;
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

            var executorMainService = new ExecutorRunAsService("admin");

            _power = new PowerSchemeService();
            var isNeedAdminAccess = _power.IsNeedAdminAccessForChangePowerScheme();
            if (isNeedAdminAccess)
            {
                executorMainService.Execute();
                Environment.Exit(-3);
            }
            else
            {
                executorMainService.RemoveIfExists();
            }

            var viewModel =
                new ViewModel(notifyIcon, contextLeftMenuStrip, contextRightMenuStrip);

            _viewService = new ViewService(this, viewModel, _power);
            _viewService.Start();

        }
    }
}
