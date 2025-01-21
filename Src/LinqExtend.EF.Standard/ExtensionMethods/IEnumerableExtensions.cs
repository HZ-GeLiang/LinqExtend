namespace LinqExtend.EF.ExtensionMethods;

internal static partial class IEnumerableExtensions
{
    internal static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException($"{nameof(selector)}参数不能为空", nameof(selector));
        }

        HashSet<TResult> hashSet = new HashSet<TResult>();
        if (source == null)
        {
            return hashSet;
        }

        foreach (TSource sourceItem in source)
        {
            hashSet.Add(selector.Invoke(sourceItem));
        }
        return hashSet;
    }
}