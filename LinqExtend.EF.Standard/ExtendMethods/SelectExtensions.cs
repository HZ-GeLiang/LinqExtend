using LinqExtend.EF.ExtendMethods;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExtend
{
    /// <summary>
    /// Select的扩展
    /// </summary>
    internal static class SelectExtensions
    {

        //这段代码要同步于 LinqExtend.Standard 的 SelectExtensions.cs 的 SelectMap_GetExpression 方法
        public static Expression<Func<TSource, TResult>> SelectMap_GetExpression<TSource, TResult>(Func<TSource, TResult> selector)
            where TSource : class
            where TResult : class, new()
        {

            var propsSource = typeof(TSource).GetProperties().ToHashSet(a => a.Name);
            var propsResult = typeof(TResult).GetProperties().ToHashSet(a => a.Name);
            var propsCommon = propsSource.Intersect(propsResult); //TSource 和 TResult 的相同属性

            var parameterExp = Expression.Parameter(typeof(TSource), "a");
            MemberBinding[] bindings = new MemberBinding[propsCommon.Count()];

            var i = 0;
            foreach (var propertyName in propsCommon)
            {
                //if (selector != null)
                //{
                //   todo: 部分属性按配置的来, 没有配置的按约定的走
                //}

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

                bindings[i++] =
                    Expression.Bind(
                        typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                        Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                    );
            }

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
