using System;
using PowerSchemeServiceAPI.Model;

namespace PowerSchemeServiceAPI.EventsArgs;

public class PowerSchemeEventArgs(
    IPowerScheme previousPowerScheme,
    IPowerScheme activePowerScheme) :
    EventArgs
{
    public IPowerScheme PreviousPowerScheme { get; } = previousPowerScheme;
    public IPowerScheme ActivePowerScheme { get; } = activePowerScheme;
}
