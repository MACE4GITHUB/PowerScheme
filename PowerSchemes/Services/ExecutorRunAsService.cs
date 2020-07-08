using PowerSchemes.Properties;
using PowerSchemes.Utility;
using RunAs.Common.Services;
using System;
using System.Diagnostics;
using System.IO;

namespace PowerSchemes.Services
{
    public class ExecutorRunAsService : ExecutorService
    {
        private const string NAME = "RunAs";
        private bool isExecuted;

        public string NameMainProgram
            => Paths.ApplicationNameWithoutExtension;

        public ExecutorRunAsService() : this("")
        { }

        public ExecutorRunAsService(string arguments)
        {
            Name = NAME;
            Arguments = string.IsNullOrEmpty(arguments) 
                ? NameMainProgram : $"{NameMainProgram} {arguments}";

            ResourceManager = Resources.ResourceManager;
        }
        
        public override void Execute()
        {
            if (isExecuted)
                throw new ArgumentNullException(nameof(NAME));

            base.Execute();
            isExecuted = true;
        }
    }
}
