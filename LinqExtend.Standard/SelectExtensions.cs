﻿using LinqExtend.ExtendMethods;
using LinqExtend.Handle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
        public static IEnumerable<TResult> Select<TResult>(this DataColumnCollection columnCollection, Func<DataColumn, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataColumn item in columnCollection)
            {
                list.Add(selector(item));
            }

            return list;
        }

        public static IEnumerable<TResult> Select<TResult>(this DataRowCollection rowCollection, Func<DataRow, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataRow item in rowCollection)
            {
                list.Add(selector(item));
            }

            return list;
        }

        /// <inheritdoc cref="SelectMap{TSource, TResult}(IEnumerable{TSource}, Expression{Func{TSource, TResult}})"/>
        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source)
            where TSource : class
            where TResult : class, new()
        {
            return SelectMap<TSource, TResult>(source, (Expression<Func<TSource, TResult>>)null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector">硬编码部分</param>
        /// <returns></returns>
        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source, Expression<Func<TSource, TResult>> selector)
            where TSource : class
            where TResult : class, new()
        {
            if (source == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var lambda = SelectMapMain.SelectMap_GetExpression<TSource, TResult>(selector, out var bindings);

            if (OnSelectMapLogTo != null)
            {
                string log = SelectMapMain.GetSelectMapLog(bindings);
                OnSelectMapLogTo.Invoke(log);
            }

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


        /// <summary>
        /// 获得SelectMap映射的日志情况
        /// </summary>
        public static Action<string> OnSelectMapLogTo { get; set; }

    }
}
