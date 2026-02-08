using System;
using System.Resources;
using RegistryManager.Model;
using RunAs.Common.Services;

namespace RegistryManager;

public class RegistryExecutorService : ExecutorService
{
    private const string NAME = "RegWriter";
    private bool _isExecuted;

    public RegistryExecutorService(
        ResourceManager resourceManager,
        RegistryParam registryParam,
        RegistryAdminAction registryAdminAction,
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

    public RegistryAdminAction RegistryAdminAction { get; }

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
