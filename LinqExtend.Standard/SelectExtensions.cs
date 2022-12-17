using LinqExtend.ExtendMethods;
using LinqExtend.Handle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
            return SelectMap<TSource, TResult>(source, (Expression<Func<TSource, TResult>>)null);
        }

        public static IEnumerable<TResult> SelectMap<TSource, TResult>(this IEnumerable<TSource> source, Expression<Func<TSource, TResult>> selector)
            where TSource : class
            where TResult : class, new()
        {
            if (source == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var lambda = SelectMap_GetExpression<TSource, TResult>(selector);

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

        private static Expression<Func<TSource, TResult>> SelectMap_GetExpression<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
            where TSource : class
            where TResult : class, new()
        {
            var process = new SelectMapProcess<TSource, TResult>();

            ParameterExpression parameterExp;
            if (selector == null)
            {
                parameterExp = Expression.Parameter(typeof(TSource), "a");
            }
            else
            {
                //需要外面丢进来 ,不然会提示 variable '' of type '' referenced from scope '', but it is not defined
                parameterExp = selector.Parameters[0];
            }

            List<MemberBinding> bindings = new List<MemberBinding>();

            /*计划支持
               一个类对象/一个匿名对象中的属性类型允许为自定义类, 
                    但是自定义中如果又出现了自定义类, 针对这种情况. 需要修改代码

               属性映射优先级:
               内置类型的属性 > 自定义类的类型属性
               相同组别的属性按顺序逐个处理
            */

            if (selector != null)
            {
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
            }

            var unmappedBuildInProperty = process.GetUnmappedBuildInProperty(rank: 1);

            //1等公民的处理
            foreach (var propertyName in unmappedBuildInProperty)
            {
                process.DealWithBuildInProperty(propertyName);

                //todo: 计划支持类型不一致时的情况: ToList<T> 的实现参考...

                // if (propertyInfo.PropertyType.IsEnum)
                // {
                //     propertyInfo.SetValue(model, Enum.ToObject(propertyInfo.PropertyType, colValue));
                //     continue;
                // }
                // if (propertyInfo.DeclaringType == colValue.GetType())
                // {
                //     propertyInfo.SetValue(model, colValue);
                //     continue;
                // }
                // if (propertyInfo.PropertyType == typeof(string))
                // {
                //     propertyInfo.SetValue(model, colValue.ToString());
                //     continue;
                // }

                // else //ChangeType 转换失败会异常
                // { 
                //                        
                //#if DEBUG
                //      var new_colValue = ChangeType(propertyInfo, colValue, propertyInfo.PropertyType);
                //#else
                //      var new_colValue = ChangeType(colValue, propertyInfo.PropertyType);
                //#endif
                //      propertyInfo.SetValue(model, new_colValue);
                // }


                var memberAssignment = Expression.Bind(
                    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                );

                bindings.Add(memberAssignment);

            }

            //2等公民的处理 
            var unmappedBuildInPropertyLv2 = process.GetUnmappedBuildInProperty(rank: 2);
            foreach (var propertyName in unmappedBuildInPropertyLv2)
            {
                Console.WriteLine(propertyName);
            }

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




}
