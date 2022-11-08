namespace Overhaul.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrecisionAttribute : Attribute
    {
        public string Precision { get; init; }

        public PrecisionAttribute(int precision)
        {
            Precision = $"({precision})";
        }
    }
}
