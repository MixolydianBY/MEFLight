namespace MEFLight.Extensions
{
    using System.Reflection;

    public static class AttributeExtensions
    {
        public static bool IsAttributeDefined<T>(this ICustomAttributeProvider attributeProvider) where T : class
        {
            return attributeProvider.IsDefined(typeof(T), false);
        }

        public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
            return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
        }

        public static bool IsAttributeDefined<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
            return attributeProvider.IsDefined(typeof(T), inherit);
        }

        public static T[] GetAttributes<T>(this ICustomAttributeProvider attributeProvider) where T : class
        {
            return (T[])attributeProvider.GetCustomAttributes(typeof(T), false);
        }
    }
}
