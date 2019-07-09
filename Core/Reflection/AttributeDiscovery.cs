

namespace MEFLight.Reflection
{
    using System;
    using System.Reflection;
    using Defenitions;
    using Parts;

    internal class AttributeDiscovery : IAttributeDiscovery
    {
        private static readonly Lazy<AttributeDiscovery> _singleton = new Lazy<AttributeDiscovery>(() => new AttributeDiscovery());

        private AttributeDiscovery()
        {

        }

        public static AttributeDiscovery Instance
        {
            get
            {
               return _singleton.Value;
            }
        }

        public ComposablePartDefinition CreateDiscoverablePartDefinition(Type type)
        {
            ReflectionPartCreation partCreation = new ReflectionPartCreation(type, false);
            if (!partCreation.IsPartDiscoverable())
                return (ComposablePartDefinition)null;
            return (ComposablePartDefinition)new ReflectedComposablePartDefinition((IReflectionPartCreation)partCreation);
        }

        public ReflectedComposablePart CreatePart(object obj)
        {
            var definition = CreatePartDefinition(obj.GetType());
            return new ReflectedComposablePart(definition, obj);
        }

        public ReflectedComposablePart CreatePart(Type objType)
        {
            var definition = CreatePartDefinition(objType);
            return new ReflectedComposablePart(definition);
        }

        public ExportDelegatePart CreatePart(Type delType, MemberInfo memberInfo)
        {
            return new ExportDelegatePart(delType, memberInfo);
        }

        public ReflectedComposablePartDefinition CreatePartDefinition(Type type)
        {
            ReflectionPartCreation partCreation = new ReflectionPartCreation(type, false);
            return new ReflectedComposablePartDefinition((IReflectionPartCreation)partCreation);
        }
    }
}
