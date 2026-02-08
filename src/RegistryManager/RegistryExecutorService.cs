using System;
using System.Resources;
using RegistryManager.Common;
using RegistryManager.Savers;
using RunAs.Common.Services;

namespace RegistryManager;

public class RegistryExecutorService : ExecutorService
{
    private const string NAME = "RegWriter";
    private bool _isExecuted;

    public RegistryExecutorService(
        ResourceManager resourceManager,
        RegistryParam registryParam,
        RegistryAction registryAdminAction,
        object fileName) :
        base(NAME, "", resourceManager)
    {
        RegistryAdminAction = registryAdminAction;

        RegistryFileName = fileName is Guid guidFileName
            ? $"{guidFileName}"
            : (string)fileName;

        RegistrySaver = new RegistrySaver(registryAdminAction, RegistryFileName)
        {
            RegistryParam = registryParam
        };

        Arguments = RegistrySaver.Arguments;
    }

    public RegistrySaver RegistrySaver { get; }

    public RegistryAction RegistryAdminAction { get; }

    public string RegistryFileName { get; }

    public override void Execute()
    {
        if (_isExecuted)
        {
            throw new ArgumentException(nameof(RegistrySaver));
        }

        RegistrySaver.SaveToStore();
        base.Execute();
        RegistrySaver.Dispose();
        _isExecuted = true;
    }
}
