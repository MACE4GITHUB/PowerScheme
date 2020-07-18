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
            var isHidden = string.Equals(args[1], AttributeFile.Hidden.ToString(), StringComparison.InvariantCultureIgnoreCase);

            var executorService = new ExecutorService()
            {
                Name = program,
                UseExecutor = false,
                IsWait = false,
                IsRemoveFile = false,
                IsHiddenFile = isHidden
            };
            executorService.Execute();
        }
    }
}
