using RegistryManager;

namespace PowerScheme.Services;

using System;
using Properties;
using RunAs.Common.Services;

public class ExecutorRunAsService : ExecutorService
{
    private const string NAME = "RunAs";
    private bool _isExecuted;

    private static string NameMainProgram
        => Paths.ApplicationNameWithoutExtension;

    public ExecutorRunAsService(string arguments = "")
    {
        Name = NAME;
        Arguments = string.IsNullOrEmpty(arguments)
            ? NameMainProgram : $"{NameMainProgram} {arguments}";

        ResourceManager = Resources.ResourceManager;
    }

    public override void Execute()
    {
        if (_isExecuted)
        {
            throw new ArgumentNullException(nameof(NAME));
        }

        base.Execute();
        _isExecuted = true;
    }
}