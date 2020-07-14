namespace PowerSchemeServiceAPI.Model
{
    public class StatePowerScheme
    {
        public StatePowerScheme(IPowerScheme powerScheme, object value = null)
        {
            PowerScheme = powerScheme;
            Value = value;
        }

        public IPowerScheme PowerScheme { get; }

        public object Value { get; }
    }
}
