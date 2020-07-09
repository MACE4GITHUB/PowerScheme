namespace PowerScheme
{
    using Model;
    using Services;
    using System;
    using System.Windows.Forms;

    public partial class FormMain : Form
    {
        private readonly IPowerSchemeService _power;
        private readonly ViewService _viewService;

        public FormMain()
        {
            InitializeComponent();
            _power = new PowerSchemeService();

            var executorMainService = new ExecutorRunAsService("admin");

            var isNeedAdminAccess = _power.IsNeedAdminAccessForChangePowerScheme();
            if (isNeedAdminAccess)
            {
                executorMainService.Execute();
                Environment.Exit(0);
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
