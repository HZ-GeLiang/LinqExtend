namespace LinqExtend.Test.ExtendMethods
{
    internal static class TypeExtensionMethod
    {
        public static List<T> MakeList<T>(this T data)
        {
            Type type = typeof(List<>).MakeGenericType(data.GetType());
            List<T> list = (List<T>)Activator.CreateInstance(type);
            list.Add(data);
            return list;
        }
    }
}
