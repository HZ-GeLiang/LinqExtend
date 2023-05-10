using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace LinqExtend.EF
{
    /// <summary>
    /// 基于<see cref="ExpressionHelper"/>使用而产生的扩展方法
    /// </summary>
    public static class ExpressionExtesion
    {
        /// <inheritdoc cref="SelectMap{TSource, TResult}(IQueryable{TSource}, Expression{Func{TSource, TResult}})" />
        public static IQueryable<TResult> SelectMap<TSource, TResult>(
                this IQueryable<TSource> query,
                Expression<Func<TSource, TResult>> selector
            )
          where TSource : class
          where TResult : class, new()
        {
            var exp = ExpressionHelper.SelectMap<TSource, TResult>(selector);
            IQueryable<TResult> querySelect = query.Select(exp);
            return querySelect;
        }
    }
}
