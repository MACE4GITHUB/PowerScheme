namespace PowerScheme.Configuration
{
    using Ninject;
    using Ninject.Modules;

    /// <summary>
    /// Creates Composition Root for the application.
    /// </summary>
    internal static class CompositionRoot
    {
        private static IKernel _ninjectKernel;

        public static void Wire(INinjectModule module)
        {
            _ninjectKernel = new StandardKernel(module);
        }

        /// <summary>
        /// Gets the resolving class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _ninjectKernel.Get<T>();
        }
    }
}
