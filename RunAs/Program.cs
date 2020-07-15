using System;
using RunAs.Common;

namespace RunAs
{
    using Common.Services;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var program = args[0];
            var arguments = args[1];
            var isHidden = string.Equals(args[2], AttributeFile.Hidden.ToString(), StringComparison.InvariantCultureIgnoreCase);

            var executorService = new ExecutorService()
            {
                Name = program,
                Arguments = arguments,
                UseExecutor = false,
                IsWait = false,
                IsRemoveFile = false,
                IsHiddenFile = isHidden
            };
            executorService.Execute();
        }
    }
}
