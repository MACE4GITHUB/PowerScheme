namespace PowerScheme.Configuration
{
    using MessageForm;
    using Model;
    using Ninject.Modules;
    using PowerSchemeServiceAPI;

    /// <summary>
    /// Binding Interfaces with Classes.
    /// </summary>
    internal sealed class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPowerSchemeService>().To<PowerSchemeService>().InSingletonScope();
            Bind<IViewModel>().To<ViewModel>().InSingletonScope();
            Bind<IMainMessageBox>().To<MainMessageBox>();
        }
    }
}
