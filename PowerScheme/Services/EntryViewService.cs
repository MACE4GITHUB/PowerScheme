namespace PowerScheme.Services
{
    using Model;
    using System;

    public class EntryViewService
    {
        private const string RESTARTED_NAME = "ShowFirstDialog";
        private const string RESTARTED_VALUE = "false";

        private readonly AppInfo _appInfo = new AppInfo();

        public EntryViewService()
        {
            IsFirstStart = RegistryService.IsFirstStart(_appInfo.CompanyName, _appInfo.ProductName);
        }

        public bool IsFirstStart { get; }

        public Action ActionIsFirstStart { get; set; }

        public void ExecuteActionIsFirstStart()
        {
            if (!IsFirstStart) return;

            ActionIsFirstStart?.Invoke();

            RegistryService.SetAppSettings(_appInfo.CompanyName, _appInfo.ProductName, RESTARTED_NAME, RESTARTED_VALUE);
        }
    }
}
