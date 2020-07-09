using System;
using PowerScheme.Model;

namespace PowerScheme.EventsArgs
{
    public class PowerSchemeEventArgs: EventArgs
    {
        
        public PowerSchemeEventArgs(IPowerScheme powerScheme)
        {
            PowerScheme = powerScheme;
        }

        public IPowerScheme PowerScheme { get; }
    }
}
