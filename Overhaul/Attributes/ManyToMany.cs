namespace Dbhaul.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ManyToMany : Attribute
    {
        public Type Type { get; init; }
        public ManyToMany(Type type)
        {
            Type = type;
        }
    }
}
