using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtend
{
    public static class AnyExtensions
    {
        public static bool AnyEnhance(this ICollection source) => source != null && source.Count > 0;
        public static bool AnyEnhance(this StringBuilder source) => source != null && source.Length > 0;
        public static bool AnyEnhance(this string source) => source != null && source.Length > 0;

        /// <summary>
        /// Any方法的增强
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool AnyEnhance<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return false;
            }
            return source.Any();
        }
    }
}
