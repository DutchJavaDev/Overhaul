namespace Dbhaul.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PrecisionAttribute : Attribute
    {
        public string Precision { get; }

        public PrecisionAttribute(int precision)
        {
            Precision = precision.ToString();
        }
    }
}
