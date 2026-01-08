using System;
using PowerSchemeServiceAPI.Model;

namespace PowerSchemeServiceAPI.EventsArgs;

public class PowerSchemeEventArgs: EventArgs
{
        
    public PowerSchemeEventArgs(IPowerScheme powerScheme)
    {
        PowerScheme = powerScheme;
    }

    public IPowerScheme PowerScheme { get; }
}