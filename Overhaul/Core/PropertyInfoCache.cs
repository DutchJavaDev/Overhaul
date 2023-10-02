using System.Reflection;

namespace Dbhaul.Core
{
    public static class PropertyInfoCache
    {
        private static readonly Dictionary<Guid, List<PropertyInfo>> Cache = new();

        public static bool ContainsKey(Guid key)
        {
            return Cache.ContainsKey(key);
        }

        public static void Add(Guid guid, PropertyInfo[] items)
        {
            Cache[guid] = items.ToList();
        }

        public static List<PropertyInfo> Get(Guid guid)
        {
            return Cache[guid];
        }
    }
}
