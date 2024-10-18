using System.Reflection;

namespace LinqExtend.ExtensionMethods;

internal static class TypeExtensions
{
    /// <summary>
    /// 获得泛型类型的第一个T的 Type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetFirstGenericType(this Type type)
    {
        if (!type.IsGenericType || type.GenericTypeArguments.Length <= 0)
        {
            return null;
        }
        return type.GenericTypeArguments[0];
    }

    public static bool IsGenericType(this Type type)
    {
#if NETCOREAPP
        return type.GetTypeInfo().IsGenericType;
#else
        return type.IsGenericType;
#endif
    }

    public static object GetDefaultValue(this Type type)
    {
        Type d1 = typeof(TypeHelper<>);
        Type[] typeArgs = { type };
        Type constructed = d1.MakeGenericType(typeArgs);
        MethodInfo method = constructed.GetMethod("GetValueOrDefault");
        var value = method.Invoke(null, null);

        //如果是非静态方法 这样写
        //object instance = Activator.CreateInstance(constructed);
        //var value = method.Invoke(instance, null);

        return value;
    }

    internal class TypeHelper<T>
    {
        private static readonly T value;

        public static T GetValueOrDefault()
        {
            return value;
        }
    }
}