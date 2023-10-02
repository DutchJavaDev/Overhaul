namespace Dbhaul.Core
{
    public static class AttributeCache
    {
        private static readonly Dictionary<Guid, List<Attribute>> Cache = new();

        public static bool ContainsKey(Guid key)
        {
            return Cache.ContainsKey(key);
        }

        public static void Add(Guid guid, IEnumerable<Attribute> items)
        {
            Cache[guid] = items.ToList();
        }

        public static List<Attribute> Get(Guid guid)
        {
            return Cache[guid];
        }
    }
}
