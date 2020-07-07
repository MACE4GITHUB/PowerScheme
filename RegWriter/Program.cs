using System;
using System.IO;
using RegistryManager;
using RegistryManager.Model;

namespace RegWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2) return;

            var registrySaver = new RegistrySaver(args[0], args[1]);
            registrySaver.SaveToRegistry();
        }
    }
}
