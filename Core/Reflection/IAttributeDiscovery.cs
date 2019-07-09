namespace MEFLight.Reflection
{
    using System;
    using Defenitions;
    using Parts;

    internal interface IAttributeDiscovery
    {
        ComposablePartDefinition CreateDiscoverablePartDefinition(Type type);

        ReflectedComposablePartDefinition CreatePartDefinition(Type type);

        ReflectedComposablePart CreatePart(object obj);
    }
}
