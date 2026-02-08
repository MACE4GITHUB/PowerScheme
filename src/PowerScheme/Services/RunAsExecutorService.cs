using System;
using PowerScheme.Properties;
using RegistryManager;
using RunAs.Common.Services;

namespace PowerScheme.Services;

public class RunAsExecutorService(
    string arguments = "") :
    ExecutorService(
        NAME,
        string.IsNullOrEmpty(arguments)
            ? NameMainProgram
            : $"{NameMainProgram} {arguments}",
        Resources.ResourceManager)
{
    private const string NAME = "RunAs";
    private bool _isExecuted;

    private static string NameMainProgram
        => Paths.ApplicationNameWithoutExtension;

    public override void Execute()
    {
        if (_isExecuted)
        {
            throw new InvalidOperationException($"{NAME} has already been executed.");
        }

        base.Execute();
        _isExecuted = true;
    }
}
