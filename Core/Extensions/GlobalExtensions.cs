namespace MEFLight.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Attributes;
    using Defenitions;

    public static class GlobalExtensions
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                                      | BindingFlags.Public | BindingFlags.Instance);
        }

        private static Expression CreateContractConstraintBody(string contractName, ParameterExpression parameter)
        {
            return (Expression)Expression.Equal((Expression)Expression.Property((Expression)parameter, typeof(ExportDefinition).GetProperty("ContractName")), (Expression)Expression.Constant((object)(contractName ?? string.Empty), typeof(string)));
        }

        public static Expression<Func<ExportDefinition, bool>> CreateConstraint(string contractName, ParameterExpression parameter)
        {
            Expression exp = CreateContractConstraintBody(contractName, parameter);

            return Expression.Lambda<Func<ExportDefinition, bool>>(exp, new ParameterExpression[1]
            {
                parameter
            });
        }

        public static List<T> FastAppendToListAllowNulls<T>(this List<T> source, T value, IEnumerable<T> second)
        {
            source = second != null ? source.FastAppendToListAllowNulls<T>(second) : source.FastAppendToListAllowNulls<T>(value);
            return source;
        }

        public static List<T> FastAppendToListAllowNulls<T>(this List<T> source, IEnumerable<T> second)
        {
            if (second == null)
                return source;
            if (source == null || source.Count == 0)
                return second.AsList<T>();
            List<T> objList = second as List<T>;
            if (objList != null)
            {
                if (objList.Count == 0)
                    return source;
                if (objList.Count == 1)
                {
                    source.Add(objList[0]);
                    return source;
                }
            }
            source.AddRange(second);
            return source;
        }

        private static List<T> FastAppendToListAllowNulls<T>(this List<T> source, T value)
        {
            if (source == null)
                source = new List<T>();
            source.Add(value);
            return source;
        }

        public static List<T> AsList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable as List<T> ?? enumerable.ToList<T>();
        }

        internal static void GetContractInfoFromExport(this MemberInfo member, ExportAttribute export, out Type typeIdentityType, out string contractName, out string memberName)
        {
            typeIdentityType = member.GetTypeIdentityTypeFromExport(export);
            if (!string.IsNullOrEmpty(export.ContractName))
                contractName = export.ContractName;
            else
                contractName = member.GetTypeIdentityFromExport(typeIdentityType);

            memberName = member?.Name;
        }

        internal static string GetTypeIdentityFromExport(this MemberInfo member, Type typeIdentityType)
        {
            string originalTypeIdentity = string.Empty;

            if (typeIdentityType != (Type)null && member.MemberType != MemberTypes.Method)
            {
                Type[] contracts = typeIdentityType.GetInterfaces();

                if (contracts.Length == 1)
                {
                   originalTypeIdentity = contracts[0].FullName;
                }
                else if(contracts.Length == 0)
                {
                    originalTypeIdentity = typeIdentityType.FullName;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Multiple Interfaces were detected, consider mark Export attribute with explicit type to export");
                }
        
                return originalTypeIdentity;
            }
            else if (typeIdentityType != (Type)null)
            {
                return typeIdentityType.FullName;
            }

            MethodInfo method = member as MethodInfo;
            return method?.ReturnType.FullName;
        }

        private static Type GetTypeIdentityTypeFromExport(this MemberInfo member, ExportAttribute export)
        {
            if (export.ContractType != (Type)null)
                return export.ContractType.AdjustSpecifiedTypeIdentityType(member);
            if (member.MemberType == MemberTypes.Method)
                return (Type)null;
            return member.GetDefaultTypeFromMember();
        }

        internal static Type GetDefaultTypeFromMember(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return (Type)member;
                default:
                    return ((FieldInfo)member).FieldType;
            }
        }

        internal static Type AdjustSpecifiedTypeIdentityType(this Type specifiedContractType, Type memberType)
        {
            if (memberType != (Type)null && memberType.IsGenericType && specifiedContractType.IsGenericType)
            {
                if (specifiedContractType.ContainsGenericParameters && !memberType.ContainsGenericParameters)
                {
                    Type[] genericArguments1 = memberType.GetGenericArguments();
                    Type[] genericArguments2 = specifiedContractType.GetGenericArguments();
                    if (genericArguments1.Length == genericArguments2.Length)
                        return specifiedContractType.MakeGenericType(genericArguments1);
                }
                //else if (specifiedContractType.ContainsGenericParameters && memberType.ContainsGenericParameters)
                //{
                //    IList<Type> genericParameters = memberType.GetPureGenericParameters();
                //    if (specifiedContractType.GetPureGenericArity() == genericParameters.Count)
                //        return specifiedContractType.GetGenericTypeDefinition().MakeGenericType(genericParameters.ToArray<Type>());
                //}
            }
            return specifiedContractType;
        }

        internal static Type AdjustSpecifiedTypeIdentityType(this Type specifiedContractType, MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method)
                return specifiedContractType;
            return AdjustSpecifiedTypeIdentityType(specifiedContractType, member.GetDefaultTypeFromMember());
        }

        internal static bool HasBaseclassOf(this Type type, Type baseClass)
        {
            if (type == baseClass)
                return false;
            for (; type != (Type)null; type = type.BaseType)
            {
                if (type == baseClass)
                    return true;
            }
            return false;
        }

     
    }
}
