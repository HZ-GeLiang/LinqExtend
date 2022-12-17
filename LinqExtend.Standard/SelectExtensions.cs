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

            var selectorLast = autoMap ? GetSelectorLast<TSource, TResult>() : null;
            var lambda = SelectMap_GetExpression<TSource, TResult>(selector, selectorLast);

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
        /// 返回一个Expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector">硬编码部分</param>
        /// <param name="selectorLast">最后兜底部分的处理</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private static Expression<Func<TSource, TResult>> SelectMap_GetExpression<TSource, TResult>(
                Expression<Func<TSource, TResult>> selector,
                Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast
            )
            where TSource : class
            where TResult : class, new()
        {
            List<MemberBinding> bindings = new List<MemberBinding>();
            var process = new SelectMapProcess<TSource, TResult>();

            /*计划支持
               一个类对象/一个匿名对象中的属性类型允许为自定义类, 
                    但是自定义中如果又出现了自定义类, 针对这种情况. 需要修改代码

               属性映射优先级:
               内置类型的属性 > 自定义类的类型属性
               相同组别的属性按顺序逐个处理
            */

            var selector_bindings = GetBindings(selector, process);
            if (selector_bindings != null)
            {
                bindings.AddRange(selector_bindings);
            }

            var parameterExp = selector == null
                  ? Expression.Parameter(typeof(TSource), "a")
                  : selector.Parameters[0];  //需要外面丢进来 ,不然会提示 variable '' of type '' referenced from scope '', but it is not defined

            var autoMap_bindings = GetBindings(parameterExp, process);
            if (autoMap_bindings != null)
            {
                bindings.AddRange(autoMap_bindings);
            }

            var last_bindings = GetBindings(selectorLast, parameterExp, process);
            if (last_bindings != null)
            {
                bindings.AddRange(last_bindings);
            }

            var lambda = GetLambda<TSource, TResult>(bindings, parameterExp);
            return lambda;
        }


        //第一部分, 硬编码部分
        private static List<MemberBinding> GetBindings<TSource, TResult>(
                Expression<Func<TSource, TResult>> selector,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();

            if (selector == null)
            {
                return bindings;
            }

            var body = selector.Body;
            if (body is System.Linq.Expressions.MemberInitExpression memberInitExpression)
            {
                foreach (var item in memberInitExpression.Bindings)
                {
                    var propertyName = item.Member.Name;
                    process.DealWithBuildInProperty(propertyName);
                }

                bindings.AddRange(memberInitExpression.Bindings);
            }
            else
            {
                throw new NotSupportedException("当前selector的写法暂不支持,请修改程序或提issue");
            }

            return bindings;

        }

        //第二部分,根据名字自动映射(目前只处理内置类型的)
        private static List<MemberBinding> GetBindings<TSource, TResult>(
                ParameterExpression parameterExp,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();
            //1等公民的处理(目前只处理内置类型的)
            var unmappedBuildInProperty = process.GetUnmappedBuildInProperty();
            foreach (var propertyName in unmappedBuildInProperty)
            {
                process.DealWithBuildInProperty(propertyName);

                var memberAssignment = Expression.Bind(
                    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                );

                bindings.Add(memberAssignment);
            }

            return bindings;
        }

        //第三部分 
        private static List<MemberBinding> GetBindings<TSource, TResult>(
                Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast,
                ParameterExpression parameterExp,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            if (selectorLast == null)
            {
                return new List<MemberBinding>();
            }
            var bindings = selectorLast.Invoke(parameterExp, process);
            return bindings;
        }

        //第三部分的处理逻辑
        private static Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> GetSelectorLast<TSource, TResult>()
         where TSource : class
         where TResult : class, new()
        {
            Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast = (parameterExp, process) =>
            {
                //最后未处理的部分(目前只处理内置类型的)
                var bindings = new List<MemberBinding>();
                var unmappedPropertyNameList = process.GetUnmappedProperty();
                var customCollection = process.Source.Custom;

                var customDict = customCollection.ToDictionary(a => a, a => new PropertyGroup(a.PropertyType));

                foreach (var propertyName in unmappedPropertyNameList)
                {
                    Console.WriteLine(propertyName);

                    foreach (var kv in customDict)
                    {
                        var objProcess = kv.Value;

                        //objProcess中propertyName是未处理过的
                        if (
                            (!objProcess.BuildInPropertyProcessList.ContainsKey(propertyName)) ||
                            objProcess.BuildInPropertyProcessList[propertyName] == true
                        )
                        {
                            continue;
                        }

                        PropertyInfo prop = kv.Value.BuildIn.First(a => string.Compare(a.Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0);

                        if (prop != null)
                        {
                            var objType = kv.Key.PropertyType;

                            customDict[kv.Key].DealWithBuildInProperty(propertyName);
                            process.DealWithBuildInProperty(propertyName, check: false);

                            Console.WriteLine($@"{objType}:{propertyName}");

                            var objName = kv.Key.Name; //order

                            var exp = Expression.Property(parameterExp, objName);//a.order

                            //添加 binding
                            var memberAssignment = Expression.Bind(
                               typeof(TResult).GetProperty(propertyName),
                               Expression.Property(exp, propertyName)// TSource 的 a.order.Id
                            );

                            bindings.Add(memberAssignment);
                        }
                    }
                }
                return bindings;
            };

            return selectorLast;
        }

        private static Expression<Func<TSource, TResult>> GetLambda<TSource, TResult>(List<MemberBinding> bindings, ParameterExpression parameterExp)
          where TSource : class
          where TResult : class, new()
        {
            MemberInitExpression memberInitExpression =
                Expression.MemberInit(
                    Expression.New(typeof(TResult)),
                    bindings
                );

            Type genericType_arg2 = typeof(TResult);
            Type genericType = typeof(Func<,>);
            Type[] templateTypeSet = new[] { typeof(TSource), genericType_arg2 };
            Type implementType = genericType.MakeGenericType(templateTypeSet);

            var lambda = (Expression<Func<TSource, TResult>>)
                Expression.Lambda(
                    implementType,
                    memberInitExpression,
                    new ParameterExpression[1] { parameterExp }
                );
            return lambda;
        }

    }
}
