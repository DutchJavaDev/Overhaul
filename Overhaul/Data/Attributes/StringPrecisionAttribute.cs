namespace Overhaul.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringPrecisionAttribute : Attribute
    {
        public string Precision { get; init; }
        public StringPrecisionAttribute(int maxCharacters)
        {
            Precision = $"({maxCharacters})";
        }
    }
}
