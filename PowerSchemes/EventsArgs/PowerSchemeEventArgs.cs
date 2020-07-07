using System;
using PowerSchemes.Model;

namespace PowerSchemes.EventsArgs
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
