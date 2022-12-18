using LinqExtend.ExtendMethods;
using LinqExtend.Handle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace LinqExtend
{
    /// <summary>
    /// Select的扩展
    /// </summary>
    public static class SelectExtensions
    {
        public static IEnumerable<TResult> Select<TResult>(this DataColumnCollection dataColumns, Func<DataColumn, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataColumn item in dataColumns)
            {
                list.Add(selector(item));
            }

            return list;
        }

        public static IEnumerable<TResult> Select<TResult>(this DataRowCollection rows, Func<DataRow, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataRow item in rows)
            {
                list.Add(selector(item));
            }

            return list;
        }

        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source)
            where TSource : class
            where TResult : class, new()
        {
            return SelectMap<TSource, TResult>(source, (Expression<Func<TSource, TResult>>)null, false);
        }

        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source, Expression<Func<TSource, TResult>> selector)
            where TSource : class
            where TResult : class, new()
        {
            return SelectMap<TSource, TResult>(source, selector, false);
        }

        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source, Expression<Func<TSource, TResult>> selector, bool autoMap)
            where TSource : class
            where TResult : class, new()
        {
            if (source == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var selectorLast = autoMap ? SelectMapHelper.GetSelectorLast<TSource, TResult>() : null;
            var lambda = SelectMapHelper.SelectMap_GetExpression<TSource, TResult>(selector, selectorLast, out var bindings);

            var log = SelectMapHelper.GetSelectMapLog(bindings);

            var methodPara = new object[] { source, lambda.Compile() };

            var SelectMehtod =
                    typeof(System.Linq.Enumerable)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(mi => mi.Name == "Select"
                        && mi.GetParameters().Length == 2
                        && mi.GetParameters().Last().ParameterType.GenericTypeArguments.Length == 2);

            IEnumerable<TResult> list = (IEnumerable<TResult>)
                SelectMehtod.MakeGenericMethod(
                    new Type[] { typeof(TSource), typeof(TResult) }
                ).Invoke(null, methodPara);

            return list;
        }


    }
}
