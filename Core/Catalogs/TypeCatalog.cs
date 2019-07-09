namespace MEFLight.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Defenitions;
    using Reflection;
    using Resources;

    public class TypeCatalog : CompositionCatalog
    {
        private readonly object _thisLock = new object();
        private Type[] _types;
        private volatile List<ComposablePartDefinition> _parts;

        public TypeCatalog(params Type[] types)
            : this((IEnumerable<Type>)types)
        {

        }

        public TypeCatalog(IEnumerable<Type> types)
        {
            this.InitializeTypeCatalog(types);  
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return this.PartsInternal.GetEnumerator();
        }

        private void InitializeTypeCatalog(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type == (Type)null)
                    throw new ArgumentNullException();  //todo make exception
                if (type.Assembly.ReflectionOnly)
                    throw new ArgumentException(string.Format(
                        (IFormatProvider)CultureInfo.CurrentCulture,
                        Messages.AssemblyReflectionOnlyEx, new object[1]
                    {
                        (object) nameof (types)
                    }), nameof(types));
            }
            this._types = types.ToArray<Type>();
        }

        private IEnumerable<ComposablePartDefinition> PartsInternal
        {
            get
            {
                if (this._parts == null)
                {
                    lock (this._thisLock)
                    {
                        if (this._parts == null)
                        {
                            List<ComposablePartDefinition> composablePartDefinitionList = new List<ComposablePartDefinition>();
                            foreach (Type type in this._types)
                            {
                                ComposablePartDefinition definitionIfDiscoverable = AttributeDiscovery.Instance.CreateDiscoverablePartDefinition(type);
                                if (definitionIfDiscoverable != null)
                                    composablePartDefinitionList.Add(definitionIfDiscoverable);
                            }
                            Thread.MemoryBarrier();
                            this._types = (Type[])null;
                            this._parts = composablePartDefinitionList;
                        }
                    }
                }
                return (IEnumerable<ComposablePartDefinition>)this._parts;
            }
        }
    }
}
