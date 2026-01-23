using System;
using RunAs.Common;
using RunAs.Common.Services;

namespace RunAs;

internal static class Program
{
    private static void Main(string[]? args)
    {
        if (args is { Length: 3 })
        {
            var programName = TryGetProgramNameOrExit(args[0]);
            var role = TryGetRoleOrExit(args[1]);
            var attributeFile = TryGetAttributeFileOrExit(args[2]);
            var isHiddenFile = IsHiddenFile(attributeFile);

            var executorService = new ExecutorService
            {
                Name = programName,
                Arguments = role,
                IsHiddenFile = isHiddenFile,
                IsWait = false,
                IsRemoveFile = false,
                UseExecutor = false
            };

            executorService.Execute();
        }
        else
        {
            ExitBecauseIncorrectParameters();
        }
    }

    private static string TryGetProgramNameOrExit(string programName)
    {
        if (string.IsNullOrWhiteSpace(programName))
        {
            ExitBecauseIncorrectParameters("The program name is not specified.");
        }

        return programName;
    }

    private static string TryGetRoleOrExit(string role)
    {
        if (!Enum.TryParse(role, true, out Role value))
        {
            ExitBecauseIncorrectParameters("The role is not specified.");
        }

        return $"{value}";
    }

    private static AttributeFile TryGetAttributeFileOrExit(string attributeFile)
    {
        if (!Enum.TryParse(attributeFile, true, out AttributeFile value))
        {
            ExitBecauseIncorrectParameters("The attribute file is not specified.");
        }

        return value;
    }

    private static bool IsHiddenFile(AttributeFile attributeFile) =>
        attributeFile == AttributeFile.Hidden;

    private static void ExitBecauseIncorrectParameters(string? message = null)
    {
        Console.WriteLine($"Incorrect parameters. {message}");
        Environment.Exit(-1);
    }
}
