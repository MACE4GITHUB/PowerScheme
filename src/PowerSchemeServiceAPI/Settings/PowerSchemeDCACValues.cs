namespace PowerSchemeServiceAPI.Settings;

public class PowerSchemeDcAcValues(int dCSettings, int aCSettings)
{
    public virtual int DcSettings { get; } = dCSettings;

    public virtual int AcSettings { get; } = aCSettings;
}
