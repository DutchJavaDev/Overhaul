﻿namespace Dbhaul.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OneToMany : Attribute
    {
        public Type Type { get; init; }
        public string KeyName { get; init; }

        public OneToMany(Type type, string keyName)
        {
            Type = type;
            KeyName = keyName;
        }
    }
}
