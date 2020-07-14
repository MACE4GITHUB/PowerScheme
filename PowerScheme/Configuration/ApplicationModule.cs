using Ninject.Modules;
using PowerScheme.Model;
using PowerScheme.Services;
using PowerSchemeServiceAPI;

namespace PowerScheme.Configuration
{
    public class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPowerSchemeService>().To<PowerSchemeService>().InSingletonScope();
            Bind<IViewService>().To<ViewService>().InSingletonScope();
            Bind<IViewModel>().To<ViewModel>().InSingletonScope();
        }
    }
}
