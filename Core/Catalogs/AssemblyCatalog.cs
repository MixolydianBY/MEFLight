namespace MEFLight.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Defenitions;
    using Resources;

    public class AssemblyCatalog : CompositionCatalog
    {
        private volatile Assembly _assembly;
        private volatile CompositionCatalog _innerCatalog;
        private ReflectionContext _reflectionContext;
        private  object _lock = new object();

        public AssemblyCatalog(string codeBase)
        {
            this.InitializeAssemblyCatalog(LoadAssembly(codeBase));
        }

        public AssemblyCatalog(string codeBase, ReflectionContext reflectionContext)
        {
            this.InitializeAssemblyCatalog(LoadAssembly(codeBase));
            this._reflectionContext = reflectionContext;
        }

        public AssemblyCatalog()
        {

        }

        public Assembly Assembly
        {
            get
            {
                return this._assembly;
            }
        }

        private void InitializeAssemblyCatalog(Assembly assembly)
        {
            if (assembly.ReflectionOnly)
            {
                throw new ArgumentException(string.Format(
                    (IFormatProvider)CultureInfo.CurrentCulture,
                    Messages.AssemblyReflectionOnlyEx, new object[1]
                {
                    (object) nameof (assembly)
                }), nameof(assembly));
            }
               
            this._assembly = assembly;
        }

        private static Assembly LoadAssembly(string codeBase)
        {
            AssemblyName assemblyRef;
            try
            {
                assemblyRef = AssemblyName.GetAssemblyName(codeBase);
            }
            catch (ArgumentException ex)
            {
                assemblyRef = new AssemblyName();
                assemblyRef.CodeBase = codeBase;
            }
            return Assembly.Load(assemblyRef);
        }

        private CompositionCatalog InnerCatalog
        {
            get
            {
                if (this._innerCatalog == null)
                {
                    lock (this._lock)
                    {
                        if (this._innerCatalog == null)
                        {
                            TypeCatalog typeCatalog =  new TypeCatalog((IEnumerable<Type>)this._assembly.GetTypes());

                            Thread.MemoryBarrier();
                            this._innerCatalog = (CompositionCatalog)typeCatalog;
                        }
                    }
                }
                return this._innerCatalog;
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return this.InnerCatalog.GetExports(definition);
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return this.InnerCatalog.GetEnumerator();
        }
    }
}
