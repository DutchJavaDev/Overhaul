namespace Overhaul.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PrecisionAttribute : Attribute
    {
        public string Precision { get; }

        public PrecisionAttribute(object precision)
        {
            Precision = precision.ToString().Replace(".",",");
        }
    }
}
