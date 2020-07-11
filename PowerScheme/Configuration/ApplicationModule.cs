using Ninject.Modules;
using PowerScheme.Services;

namespace PowerScheme.Configuration
{
    public class ApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<FormMain>().ToSelf().InSingletonScope();
            Bind<IPowerSchemeService>().To<PowerSchemeService>().InSingletonScope();
        }
    }
}
