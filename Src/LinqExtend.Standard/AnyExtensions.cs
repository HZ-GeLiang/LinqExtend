using System.Collections;
using System.Data;
using System.Text;

namespace LinqExtend;

public static class AnyExtensions
{
    //和public static bool AnyEnhance<TSource>(this IEnumerable<TSource> source) 有冲突
    //public static bool AnyEnhance(this ICollection source) => source != null && source.Count > 0;

    public static bool AnyEnhance(this StringBuilder source) => source != null && source.Length > 0;

    public static bool AnyEnhance(this string source) => source != null && source.Length > 0;

    public static bool AnyEnhance(this DataRowCollection source) => source != null && source.Count > 0;

    public static bool AnyEnhance(this ArrayList source) => source != null && source.Count > 0;

    /// <summary>
    /// Any方法的增强
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool AnyEnhance<TSource>(this IEnumerable<TSource> source) => source?.Any() ?? false;

    public static bool AnyEnhance<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        => source?.Any(predicate) ?? false;
}