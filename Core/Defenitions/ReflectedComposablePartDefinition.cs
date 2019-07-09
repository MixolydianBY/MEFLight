namespace MEFLight.Defenitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Parts;
    using Reflection;

    internal class ReflectedComposablePartDefinition : ComposablePartDefinition
    {
        private object _lock = new object();
        private readonly IReflectionPartCreation _creationInfo;
        private volatile ImportDefinition[] _imports;
        private volatile ExportDefinition[] _exports;
        private volatile MemberInfo[] _members;
        private volatile IDictionary<string, object> _metadata;
        private volatile ConstructorInfo _constructor;

        public ReflectedComposablePartDefinition(IReflectionPartCreation creationInfo)
        {
            this._creationInfo = creationInfo;
        }

        public Type GetPartType()
        {
            return this._creationInfo.GetPartType();
        }

        public bool IsDelegate()
        {
            return this._creationInfo.GetPartType() is Delegate;
        }

        public Lazy<Type> GetLazyPartType()
        {
            return this._creationInfo.GetLazyPartType();
        }

        public ConstructorInfo GetConstructor()
        {
            if (this._constructor == (ConstructorInfo)null)
            {
                ConstructorInfo constructor = this._creationInfo.GetConstructor();
                lock (this._lock)
                {
                    if (this._constructor == (ConstructorInfo)null)
                        this._constructor = constructor;
                }
            }
            return this._constructor;
        }

        private ExportDefinition[] ExportDefinitionsInternal
        {
            get
            {
                if (this._exports == null)
                {
                    ExportDefinition[] array = this._creationInfo.GetExports().ToArray<ExportDefinition>();
                    lock (this._lock)
                    {
                        if (this._exports == null)
                            this._exports = array;
                    }
                }
                return this._exports;
            }
        }

        private MemberInfo[] MembersInternal
        {
            get
            {
                if (this._members == null)
                {
                    MemberInfo[] array = this._creationInfo.GetExportMembers().ToArray();
                    lock (this._lock)
                    {
                        if (this._members == null)
                            this._members = array;
                    }
                }
                return this._members;
            }
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get
            {
                return (IEnumerable<ExportDefinition>)this.ExportDefinitionsInternal;
            }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get
            {
                if (this._imports == null)
                {
                    ImportDefinition[] array = this._creationInfo.GetImports().ToArray<ImportDefinition>();
                    lock (this._lock)
                    {
                        if (this._imports == null)
                            this._imports = array;
                    }
                }
                return (IEnumerable<ImportDefinition>)this._imports;
            }
        }

        public override IEnumerable<MemberInfo> ExportMemberInfos
        {
            get
            {
                return (IEnumerable<MemberInfo>)this.MembersInternal;
            }
        }

        public override string OrigTypeName     
        {
            get
            {
               return this.GetPartType().FullName;
            }
        }

        public bool ParameterlessConstructor
        {
            get
            {
                try
                {
                    return !this.GetConstructor().GetParameters().Any();
                }
                catch (NullReferenceException e)
                {
                    return false;
                }
            }
        }

        internal override ComposablePart CreatePart()
        {
            return (ComposablePart) new ReflectedComposablePart(this);
        }
    }
}
