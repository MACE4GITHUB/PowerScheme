using System;
using PowerSchemeServiceAPI.Model;

namespace PowerSchemeServiceAPI.EventsArgs;

public class PowerSchemeEventArgs(
    IPowerScheme powerScheme) :
    EventArgs
{
    public IPowerScheme PowerScheme { get; } = powerScheme;
}
