namespace MEFLight.Reflection.Items
{
    using System;

    internal abstract class ReflectionItem
    {
        public abstract string Name { get; }

        public abstract string GetDisplayName();

        public abstract Type ReturnType { get; }

        public abstract ReflectionItemType ItemType { get; }
    }

    internal enum ReflectionItemType
    {
        Parameter,
        Field,
        Property,
        Method,
        Type,
    }
}
