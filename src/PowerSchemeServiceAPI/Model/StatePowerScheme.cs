namespace PowerSchemeServiceAPI.Model;

public class StatePowerScheme(
    IPowerScheme powerScheme,
    object value = null)
{
    public IPowerScheme PowerScheme { get; } = powerScheme;

    public object Value { get; } = value;
}
