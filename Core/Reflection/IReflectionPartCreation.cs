namespace MEFLight.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Defenitions;

    internal interface IReflectionPartCreation
    {
        Type GetPartType();

        Lazy<Type> GetLazyPartType();

        ConstructorInfo GetConstructor();

        IEnumerable<MemberInfo> GetExportMembers();

        IEnumerable<ExportDefinition> GetExports();

        IEnumerable<ImportDefinition> GetImports();
    }
}
