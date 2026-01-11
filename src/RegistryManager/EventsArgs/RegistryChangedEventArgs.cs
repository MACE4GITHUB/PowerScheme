using System;
using RegistryManager.Model;

namespace RegistryManager.EventsArgs;

public sealed class RegistryChangedEventArgs(
    RegChangeNotifyFilter regChangeNotifyFilter,
    RegistryParam previous,
    RegistryParam current,
    string[]? subKeys)
    : EventArgs
{
    public RegistryChangedEventArgs(
        RegChangeNotifyFilter regChangeNotifyFilter, 
        RegistryParam registryParam) :
        this(
            regChangeNotifyFilter, 
            registryParam,
            registryParam, 
            null)
    { }

    public RegistryChangedEventArgs(
        RegChangeNotifyFilter regChangeNotifyFilter, 
        RegistryParam registryParam, 
        string[] subKeys) :
        this(
            regChangeNotifyFilter, 
            registryParam, 
            registryParam, 
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
            null)
    { }

    public RegChangeNotifyFilter RegChangeNotifyFilter { get; } = regChangeNotifyFilter;

    public RegistryParam Previous { get; } = previous;

    public RegistryParam Current { get; } = current;

    public string[]? NewSubKeys { get; } = subKeys;
}
