using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtend
{
    /// <summary>
    /// IEnumerable的扩展
    /// </summary>
    public static class IEnumerableExtensions
    { 

#if !NET6_0_OR_GREATER && !NETSTANDARD1_0_OR_GREATER 
        // 注: net 6.0 自带了 DistinctBy,

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source is null)
            {
                //return Enumerable.Empty<TSource>();
                throw new ArgumentException($@"{nameof(source)} can not be null");
            }

            var hashSet = new HashSet<TKey>();
            foreach (TSource item in source)
            {
                if (hashSet.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

#endif

    }
}
