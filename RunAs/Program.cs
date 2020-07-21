using RunAs.Common;
using System;

namespace RunAs
{
    using Common.Services;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 3) ExitBecauseWrongParameters();

            var isRole = Enum.TryParse(args[1], true, out Role role);
            if (!isRole) ExitBecauseWrongParameters("The role is not specified.");

            var isAttributeFile = Enum.TryParse(args[2], true, out AttributeFile attributeFile);
            if (!isAttributeFile) ExitBecauseWrongParameters("The attribute file is not specified.");
            var isHidden = attributeFile == AttributeFile.Hidden;

            var programName = args[0];

            var executorService = new ExecutorService()
            {
                Name = programName,
                Arguments = role.ToString(),
                UseExecutor = false,
                IsWait = false,
                IsRemoveFile = false,
                IsHiddenFile = isHidden
            };
            executorService.Execute();
        }

        private static void ExitBecauseWrongParameters(string message = null)
        {
            Console.WriteLine($"Wrong parameters. {message}");
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}
