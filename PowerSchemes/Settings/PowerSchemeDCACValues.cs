using PowerManagerAPI;

namespace PowerSchemes.Settings
{
    public class PowerSchemeDCACValues
    {
        public PowerSchemeDCACValues(int dCSettings, int aCSettings)
        {
            DCSettings = dCSettings;
            ACSettings = aCSettings;
        }

        public virtual int DCSettings { get; }

        public virtual int ACSettings { get; }
    }
}
