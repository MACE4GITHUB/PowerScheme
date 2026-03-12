using System;
using RegistryManager.Common;

namespace RegistryManager.EventsArgs;

public sealed class RegistryChangedEventArgs(
    RegChangeNotifyFilter regChangeNotifyFilter,
    RegistryParam previous,
    RegistryParam current,
    ChangeType changeType,
    string[]? subKeys)
    : EventArgs
{
    public RegistryChangedEventArgs(
        RegChangeNotifyFilter regChangeNotifyFilter,
        RegistryParam registryParam,
        ChangeType changeType,
        string[] subKeys) :
        this(
            regChangeNotifyFilter,
            registryParam,
            registryParam,
            changeType,
            subKeys)
    { }

    public RegistryChangedEventArgs(
        RegChangeNotifyFilter regChangeNotifyFilter,
        RegistryParam previous,
        RegistryParam current) :
        this(
            regChangeNotifyFilter,
            previous,
            current,
            ChangeType.Updated,
            null)
    { }

    public RegChangeNotifyFilter RegChangeNotifyFilter { get; } = regChangeNotifyFilter;

    public RegistryParam Previous { get; } = previous;

    public RegistryParam Current { get; } = current;

    public string[]? SubKeys { get; } = subKeys;

    public ChangeType ChangeType { get; } = changeType;
}

public enum ChangeType
{
    Added,
    Updated,
    Deleted
}
