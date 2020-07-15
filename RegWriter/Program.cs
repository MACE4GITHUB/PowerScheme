namespace RegWriter
{
    using RegistryManager;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2) return;

            var registrySaver = new RegistrySaver(args[0], args[1]);
            registrySaver.SaveToRegistry();
        }
    }
}
