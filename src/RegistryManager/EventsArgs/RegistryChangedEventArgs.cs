namespace RegistryManager.EventsArgs;

using Model;
using System;

public sealed class RegistryChangedEventArgs : EventArgs
{
    public RegistryChangedEventArgs(RegChangeNotifyFilter regChangeNotifyFilter, RegistryParam registryParam) :
        this(regChangeNotifyFilter, registryParam, registryParam, null)
    { }

    public RegistryChangedEventArgs(RegChangeNotifyFilter regChangeNotifyFilter, RegistryParam registryParam, string[] subKeys) :
        this(regChangeNotifyFilter, registryParam, registryParam, subKeys)
    { }
        
    public RegistryChangedEventArgs(RegChangeNotifyFilter regChangeNotifyFilter, RegistryParam previous, RegistryParam current) :
        this(regChangeNotifyFilter, previous, current, null)
    { }

    public RegistryChangedEventArgs(RegChangeNotifyFilter regChangeNotifyFilter,  RegistryParam previous, RegistryParam current, string[] subKeys)
    {
        RegChangeNotifyFilter = regChangeNotifyFilter;
        Previous = previous;
        Current = current;
        NewSubKeys = subKeys;
    }

    public RegChangeNotifyFilter RegChangeNotifyFilter { get; }

    public RegistryParam Previous { get; }

    public RegistryParam Current { get; }
        
    public string[] NewSubKeys { get; }
}