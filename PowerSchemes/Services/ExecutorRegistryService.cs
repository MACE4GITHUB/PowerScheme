using System;
using RegistryManager;
using RegistryManager.Model;

namespace PowerSchemes.Services
{
    public class ExecutorRegistryService : ExecutorService
    {
        private const string NAME = "RegWriter";
        private bool isExecuted;

        public ExecutorRegistryService(RegistryParam registryParam, RegistryAdminAction registryAdminAction, object fileName)
        {
            RegistryAdminAction = registryAdminAction;
            if (fileName is Guid guidFileName)
            {
                RegistryFileName = guidFileName.ToString();
            }
            else
            {
                RegistryFileName = (string) fileName;
            }

            RegistrySaver = new RegistrySaver(registryAdminAction, RegistryFileName)
            {
                RegistryParam = registryParam
            };

            Name = NAME;
            Arguments = RegistrySaver.Arguments;
        }

        public RegistrySaver RegistrySaver { get; }

        public RegistryAdminAction RegistryAdminAction { get; }

        public string RegistryFileName { get; }

        public override void Execute()
        {
            if (isExecuted) 
                throw new ArgumentNullException(nameof(RegistrySaver));

            RegistrySaver.SaveToStore();
            base.Execute();
            RegistrySaver.Dispose();
            isExecuted = true;
        }
    }
}