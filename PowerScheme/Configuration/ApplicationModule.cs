namespace PowerScheme.Configuration
{
    using Model;
    using Ninject.Modules;
    using PowerSchemeServiceAPI;
    using Services;

    /// <summary>
    /// Binding Interfaces with Classes.
    /// </summary>
    internal sealed class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPowerSchemeService>().To<PowerSchemeService>().InSingletonScope();
            Bind<IViewModel>().To<ViewModel>().InSingletonScope();
        }
    }
}
