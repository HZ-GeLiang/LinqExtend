using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExtend.ExtendMethods
{
    internal static class ClassPropertyCompareExtensions
    {
        /// <summary>
        /// 2个类相同的属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static List<string> GetCommonProps<TSource, TResult>() where TSource : class where TResult : class, new()
        {
            var propsSource = typeof(TSource).GetProperties().ToHashSet(a => a.Name);
            var propsResult = typeof(TResult).GetProperties().ToHashSet(a => a.Name);
            var propsCommon = propsSource.Intersect(propsResult);
            return propsCommon.ToList();
        }

        /// <summary>
        /// TResult 类 没有映射的属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static List<string> GetExceptProps<TSource, TResult>() where TSource : class where TResult : class, new()
        {
            var propsSource = typeof(TSource).GetProperties().ToHashSet(a => a.Name);
            var propsResult = typeof(TResult).GetProperties().ToHashSet(a => a.Name);
            var resultPropsExcept = propsResult.Except(propsSource); //propsResult 的 差集
            return resultPropsExcept.ToList();
        }

        public static string Demo_映射情况分析<TSource, TResult>() where TSource : class where TResult : class, new()
        {
            List<string> list1 = ClassPropertyCompareExtensions.GetCommonProps<TSource, TResult>();
            List<string> list2 = ClassPropertyCompareExtensions.GetExceptProps<TSource, TResult>();

            StringBuilder sb = new StringBuilder();

            var txt = list1.AggregateToString(a =>
            {
                return $"r.{a} = s.{a};";
            }, Environment.NewLine);
            if (txt.Length > 0)
            {
                txt += Environment.NewLine;
            }
            if (list2.Any())
            {
                txt += $"下列属性需要人工映射 {Environment.NewLine}";
                //txt += $"需要人工映射, = 号后面的是值方便智能提示{Environment.NewLine}";
                txt += list2.AggregateToString(a =>
                {
                    return $"r.{a} = s.{a};";
                }, Environment.NewLine);
            }

            return txt;
        }

    }
}
