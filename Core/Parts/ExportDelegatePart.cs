namespace MEFLight.Parts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Defenitions;
    using Services;

    public class ExportDelegatePart : IComposablePart
    {
        private volatile object _cachedInstance;
        private Type _delType;
        private MemberInfo _member;

        public ExportDelegatePart(Type delegateType, MemberInfo member)
        {
            _delType = delegateType;
            _member = member;
        }

        public IEnumerable<ExportDefinition> ExportDefinitions { get; }

        public IEnumerable<ImportDefinition> ImportDefinitions { get; }

        public void Activate()
        {
            object obj = CustomActivator.CreateInstance(_member.DeclaringType);
            MethodInfo method = obj.GetType().GetMethod(_member.Name);

            _cachedInstance = Delegate.CreateDelegate(_delType, obj, method);
        }

        public object GetActivatedInstance()
        {
            if (_cachedInstance != null)
                return _cachedInstance;
            throw new InvalidOperationException("Could not load instance");
        }
    }
}
