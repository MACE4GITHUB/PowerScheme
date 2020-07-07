using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSchemes.Properties;
using PowerSchemes.Utility;

namespace PowerSchemes.Services
{

    public class ExecutorService
    {
        private const string EXE_EXTENTION = ".exe";

        public ExecutorService()
        { }

        public ExecutorService(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; set; }

        public string FullName
            => Path.Combine(Paths.ApplicationPath, $"{Name}{EXE_EXTENTION}");

        public string Arguments { get; set; }

        public ProcessWindowStyle ProcessWindowStyle { get; set; } = ProcessWindowStyle.Hidden;

        public bool Runas { get; set; } = true;

        public bool IsWait { get; set; } = true;

        public virtual void Execute()
        {
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
                if (File.Exists(FullName)) File.Delete(FullName);
            }
        }

        private byte[] Executor
            => (byte[])Resources.ResourceManager.GetObject(Name)
               ?? throw new ArgumentNullException(nameof(Name));
    }
}
