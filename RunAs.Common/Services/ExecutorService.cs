using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using RunAs.Common.Utils;

namespace RunAs.Common.Services
{
    public class ExecutorService
    {
        private const string EXE_EXTENTION = ".exe";

        public ExecutorService()
        { }

        public ExecutorService(string name, string arguments, ResourceManager resourceManager)
        {
            Name = name;
            Arguments = arguments;
            ResourceManager = resourceManager;
        }

        public string Name { get; set; }

        public string FullName
            => Path.Combine(Paths.ApplicationPath, $"{Name}{EXE_EXTENTION}");

        public string Arguments { get; set; }

        public ResourceManager ResourceManager { get; set; }

        public ProcessWindowStyle ProcessWindowStyle { get; set; } = ProcessWindowStyle.Hidden;

        public bool Runas { get; set; } = true;

        public bool IsWait { get; set; } = true;

        public bool UseExecutor { get; set; } = true;
        
        public bool IsRemoveFile { get; set; } = true;

        public virtual void Execute()
        {
            if (UseExecutor)
                File.WriteAllBytes(FullName, Executor);

            try
            {
                UACHelper.AttemptPrivilegeEscalation(
                    FullName,
                    Arguments,
                    ProcessWindowStyle,
                    Runas,
                    IsWait);
            }
            finally
            {
                if (IsRemoveFile) RemoveIfExists();
            }
        }

        public virtual void RemoveIfExists()
        {
            if (File.Exists(FullName)) File.Delete(FullName);
        }

        private byte[] Executor
            => (byte[])ResourceManager.GetObject(Name)
               ?? throw new ArgumentNullException(nameof(Name));
    }
}
