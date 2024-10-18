using LinqExtend.EF;

namespace LinqExtend;

/// <summary>
/// GroupBy的扩展
/// </summary>
public static class GroupByExtensions
{
    public static IQueryable<IGrouping<TKey, TSource>> GroupByMap<TSource, TKey>(this IQueryable<TSource> source)
        where TSource : class
        where TKey : class, new()
    {
        var keySelector = ExpressionHelper.SelectMap<TSource, TKey>();
        return source.GroupBy(keySelector);
    }

    public static IQueryable<IGrouping<TKey, TSource>> GroupByMap<TSource, TKey>(this IQueryable<TSource> source, TKey _)
       where TSource : class
       where TKey : class, new()
    {
        var keySelector = ExpressionHelper.SelectMap<TSource, TKey>();
        return source.GroupBy(keySelector);
    }
}