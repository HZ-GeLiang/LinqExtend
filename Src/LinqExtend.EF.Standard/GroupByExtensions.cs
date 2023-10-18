using LinqExtend.EF;
using System.Linq;

namespace LinqExtend
{
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
    }
}
