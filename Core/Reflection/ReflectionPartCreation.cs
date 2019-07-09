namespace MEFLight.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Attributes;
    using Defenitions;
    using Extensions;
    using Items;

    public class ReflectionPartCreation : IReflectionPartCreation, ICompositionElement
    {
        private ConstructorInfo _constructor;
        private MethodInfo _exportDelegate;
        private IEnumerable<ExportDefinition> _exports;
        private IEnumerable<ImportDefinition> _imports;
        private IEnumerable<MemberInfo> _members;
        private HashSet<string> _contractNamesOnNonInterfaces;
        private readonly Type _type;
        private readonly bool _ignoreConstructorImports;
        private readonly ICompositionElement _origin;

        public ReflectionPartCreation(Type type, bool ignoreConstructorImports)
        {
            this._type = type;
            this._ignoreConstructorImports = ignoreConstructorImports;
        }

        ICompositionElement ICompositionElement.Origin
        {
            get
            {
                return this._origin;
            }
        }

        string ICompositionElement.DisplayName
        {
            get
            {
                return this.GetDisplayName();
            }
        }

        public override string ToString()
        {
            return this.GetDisplayName();
        }

        private string GetDisplayName()
        {
            //return this.GetPartType().GetDisplayName(); 
            return string.Empty;
        }

        public Type GetPartType()
        {
            return this._type;
        }

        public Lazy<Type> GetLazyPartType()
        {
            return new Lazy<Type>(new Func<Type>(this.GetPartType), LazyThreadSafetyMode.PublicationOnly);
        }

        public ConstructorInfo GetConstructor()
        {
            if (this._constructor == (ConstructorInfo)null && !this._ignoreConstructorImports)
                this._constructor = ReflectionPartCreation.SelectPartConstructor(this._type);
            return this._constructor;
        }

        public IEnumerable<MemberInfo> GetExportMembers()
        {
            this.DiscoverExportsAndImports();
            return this._members;
        }

        public IEnumerable<ExportDefinition> GetExports()
        {
            this.DiscoverExportsAndImports();
            return this._exports;
        }

        public IEnumerable<ImportDefinition> GetImports()
        {
            this.DiscoverExportsAndImports();
            return this._imports;
        }

        public bool IsPartDiscoverable()
        {         
            if (!this.HasExports())
            {
                return false;
            }

            return true;
        }

        private static ConstructorInfo SelectPartConstructor(Type type)
        {
            if (type.IsAbstract)
                return (ConstructorInfo)null;
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ConstructorInfo[] constructors = type.GetConstructors(bindingAttr);
            if (constructors.Length == 0)
                return (ConstructorInfo)null;
            if (constructors.Length == 1 && constructors[0].GetParameters().Length == 0)
                return constructors[0];
            ConstructorInfo constructorInfo1 = (ConstructorInfo)null;
            ConstructorInfo constructorInfo2 = (ConstructorInfo)null;
            foreach (ConstructorInfo attributeProvider in constructors)
            {
                if (attributeProvider.IsAttributeDefined<ImportingConstructorAttribute>())
                {
                    if (constructorInfo1 != (ConstructorInfo)null)
                        return (ConstructorInfo)null;
                    constructorInfo1 = attributeProvider;
                }
                else if (constructorInfo2 == (ConstructorInfo)null && attributeProvider.GetParameters().Length == 0)
                    constructorInfo2 = attributeProvider;
            }
            ConstructorInfo constructorInfo3 = constructorInfo1;
            if ((object)constructorInfo3 != null)
                return constructorInfo3;
            return constructorInfo2;
        }

        private void DiscoverExportsAndImports()
        {
            if (this._exports != null && this._imports != null)
                return;
            this._exports = this.GetExportDefinitions(out _members);
            this._imports = this.GetImportDefinitions();
        }

        private IEnumerable<ExportDefinition> GetExportDefinitions(out IEnumerable<MemberInfo> members)
        {
            List<ExportDefinition> exportDefinitionList = new List<ExportDefinition>();
            this._contractNamesOnNonInterfaces = new HashSet<string>();

            members = this.GetExportMembers(this._type).ToList();

            foreach (MemberInfo exportMember in members)
            {
                foreach (ExportAttribute attribute in exportMember.GetAttributes<ExportAttribute>())
                {
                    ExportDefinition exportDefinition = this.CreateExportDefinition(exportMember, attribute);             
                    exportDefinitionList.Add(exportDefinition);
                }
            }
            this._contractNamesOnNonInterfaces = (HashSet<string>)null;
            return (IEnumerable<ExportDefinition>)exportDefinitionList;
        }

        private IEnumerable<MemberInfo> GetExportMembers(Type type)
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            if (type.IsAbstract)
                flags &= ~BindingFlags.Instance;
            else if (ReflectionPartCreation.IsExport((ICustomAttributeProvider)type) && !this._type.ContainsGenericParameters)
                yield return (MemberInfo)type;
            FieldInfo[] fieldInfoArray = type.GetFields(flags);
            int index;
            for (index = 0; index < fieldInfoArray.Length; ++index)
            {
                FieldInfo fieldInfo = fieldInfoArray[index];
                if (ReflectionPartCreation.IsExport((ICustomAttributeProvider)fieldInfo) &&  !this._type.ContainsGenericParameters)
                    yield return (MemberInfo)fieldInfo;
            }
            fieldInfoArray = (FieldInfo[])null;
            PropertyInfo[] propertyInfoArray = type.GetProperties(flags);
            for (index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo propertyInfo = propertyInfoArray[index];
                if (ReflectionPartCreation.IsExport((ICustomAttributeProvider)propertyInfo) && !this._type.ContainsGenericParameters)
                    yield return (MemberInfo)propertyInfo;
            }
            propertyInfoArray = (PropertyInfo[])null;
            MethodInfo[] methodInfoArray = type.GetMethods(flags);
            for (index = 0; index < methodInfoArray.Length; ++index)
            {
                MethodInfo methodInfo = methodInfoArray[index];
                if (ReflectionPartCreation.IsExport((ICustomAttributeProvider)methodInfo) && !this._type.ContainsGenericParameters)
                    yield return (MemberInfo)methodInfo;
            }
            methodInfoArray = (MethodInfo[])null;
        }

        private static bool IsExport(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.IsAttributeDefined<ExportAttribute>(false);
        }

        private ExportDefinition CreateExportDefinition(MemberInfo member, ExportAttribute exportAttribute)
        {
            string contractName = (string)null;
            string exportName = (string)null;
            Type typeIdentityType = (Type)null;
            member.GetContractInfoFromExport(exportAttribute, out typeIdentityType, out contractName, out exportName);
            return new ExportDefinition(typeIdentityType, contractName, member);
        }

        private bool HasExports()
        {
            if (!this.GetExportMembers(this._type).Any<MemberInfo>())
                return false;
            return true;
        }

        private IEnumerable<ImportDefinition> GetImportDefinitions()
        {
            List<ImportDefinition> importDefinitionList = new List<ImportDefinition>();
            foreach (MemberInfo importMember in this.GetImportMembers(this._type))
            {
                ImportDefinition importDefinition = ReflectionPartCreation.CreateMemberImport(importMember, (ICompositionElement)this);
                importDefinitionList.Add((ImportDefinition)importDefinition);
            }
            ConstructorInfo constructor = this.GetConstructor();
            if (constructor != (ConstructorInfo)null)
            {
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    ImportDefinition importDefinition = ReflectionPartCreation.CreateParameterImport(parameter, (ICompositionElement)this);
                    importDefinitionList.Add((ImportDefinition)importDefinition);
                }
            }
            return (IEnumerable<ImportDefinition>)importDefinitionList;
        }

        private IEnumerable<MemberInfo> GetImportMembers(Type type)
        {
            if (!type.IsAbstract)
            {
                foreach (MemberInfo onlyImportMember in this.GetDeclaredOnlyImportMembers(type))
                    yield return onlyImportMember;
                if (type.BaseType != (Type)null)
                {
                    Type baseType;
                    for (baseType = type.BaseType; baseType != (Type)null && baseType.UnderlyingSystemType != typeof(object); baseType = baseType.BaseType)
                    {
                        foreach (MemberInfo onlyImportMember in this.GetDeclaredOnlyImportMembers(baseType))
                            yield return onlyImportMember;
                    }
                    baseType = (Type)null;
                }
            }
        }

        private IEnumerable<MemberInfo> GetDeclaredOnlyImportMembers(Type type)
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fieldInfoArray = type.GetFields(flags);
            int index;
            for (index = 0; index < fieldInfoArray.Length; ++index)
            {
                FieldInfo fieldInfo = fieldInfoArray[index];
                if (ReflectionPartCreation.IsImport((ICustomAttributeProvider)fieldInfo) && !fieldInfo.ReflectedType.ContainsGenericParameters)
                    yield return (MemberInfo)fieldInfo;
            }
            fieldInfoArray = (FieldInfo[])null;
            PropertyInfo[] propertyInfoArray = type.GetProperties(flags);
            for (index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo propertyInfo = propertyInfoArray[index];
                if (ReflectionPartCreation.IsImport((ICustomAttributeProvider)propertyInfo) && !propertyInfo.ReflectedType.ContainsGenericParameters)
                    yield return (MemberInfo)propertyInfo;
            }
            propertyInfoArray = (PropertyInfo[])null;
        }

        private static bool IsImport(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.IsAttributeDefined<IAttributedImport>(false);
        }


        public static ImportDefinition CreateParameterImport(ParameterInfo parameter, ICompositionElement origin)
        {
            ReflectionParameter reflectionParameter = new ReflectionParameter(parameter);
            IAttributedImport attributedImport = ReflectionPartCreation.GetAttributedImport((ICustomAttributeProvider)parameter);

            if (string.IsNullOrEmpty(attributedImport.ContractName))
            {
                return new ImportDefinition(parameter.Name, parameter.ParameterType);
            }
            
            return new ContractBasedImportDefinition(parameter.Name, parameter.ParameterType, attributedImport.ContractName);
        }

        private static IAttributedImport GetAttributedImport(ICustomAttributeProvider attributeProvider)
        {
            IAttributedImport[] attributes = attributeProvider.GetAttributes<IAttributedImport>(false);
            if (attributes.Length == 0)
                return (IAttributedImport)new ImportAttribute();
            if (attributes.Length > 1)
                throw new Exception(); //MULTIPLE [Import] and [ImportMany]
            return attributes[0];
        }

        public static ImportDefinition CreateMemberImport(MemberInfo member, ICompositionElement origin)
        {
            IAttributedImport attributedImport = ReflectionPartCreation.GetAttributedImport((ICustomAttributeProvider)member);

            if (string.IsNullOrEmpty(attributedImport.ContractName))
            {
                return new ImportDefinition(member.Name, member.GetDefaultTypeFromMember());
            }

            return new ContractBasedImportDefinition(member.Name, member.GetDefaultTypeFromMember(), attributedImport.ContractName);
        }
    }
}
