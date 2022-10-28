using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace LinqExtend
{
    public static class OrderByExtensions
    {
        #region OrderByFunc

        /// <summary>
        /// 排序比较器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class DynamicSortComparer<T> : IComparer<T> where T : class
        {
            private readonly Func<T, T, int> _func;

            public DynamicSortComparer(Func<T, T, int> func)
            {
                _func = func;
            }

            public int Compare(T x, T y) => _func(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">数据源</param>
        /// <param name="comparer">比较器:传入一个委托,内部会自动创建比较器</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByFunc<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, int> comparer)
            where TSource : class
        {
            if (source is null)
            {
                //throw new ArgumentException($@"{nameof(source)} can not be null");
                return Enumerable.Empty<TSource>().OrderBy(a => a);
            }

            var sortComparer = new DynamicSortComparer<TSource>(comparer);
            var result = source.OrderBy(a => a, sortComparer); //需要 using System.Linq;
            return result;
        }


        #endregion

        #region OrderByExpression

        private static Type dynamicType = null;
        public static IEnumerable<TSource> OrderByExpression<TSource>(
          this IEnumerable<TSource> source,
          Expression<Func<TSource, dynamic>> orderExpression
        ) where TSource : class
        {
            dynamicType = orderExpression.Body.Type;

            if (source == null ||
                source.Count() <= 1 ||
                orderExpression == null
                )
            {
                return source;
            }

            //System.Collections.ObjectModel.ReadOnlyCollection<Expression> arguments;
            //System.Collections.ObjectModel.ReadOnlyCollection<MemberBinding> arguments2;
            IList arguments;

            var orderExpressionBodyType = orderExpression.Body.GetType();
            if (orderExpression.Body is System.Linq.Expressions.NewExpression newExpression)
            {
                // new 匿名类
                arguments = newExpression.Arguments; //ReadOnlyCollection<Expression>
            }
            else if (orderExpression.Body is System.Linq.Expressions.MemberInitExpression memberInitExpression)
            {
                // new 一个具体的类
                arguments = memberInitExpression.Bindings; //ReadOnlyCollection<MemberBinding> 
            }
            else
            {
                //未知
                throw new Exception("请修改代码, 未知的情况");
                //return source;
            }
            if (arguments.Count <= 0)
            {
                return source;
            }

            IOrderedEnumerable<TSource> orderedResult = null;

            var type_TEntity = typeof(TSource);
            foreach (dynamic argument in arguments)
            {
                // System.Linq.Expressions.PropertyExpression 
                if (argument.GetType().Name == "PropertyExpression")
                {
                    // s.OrderByCustom(a => new { a.Name }); 
                    var propertyName = argument.Member.Name;
                    var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"
                    var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName

                    //if (argument.Member.PropertyType == typeof(int))
                    //{
                    //    //typeof(Func<TSource, int>);

                    //    var lambda = Expression.Lambda<Func<TSource, int>>(
                    //        propertyExp,
                    //        new ParameterExpression[1] { parameterExp }
                    //    );

                    //    orderedResult = OrderByCustom_Order<TSource, int>(source, orderedResult, lambda);
                    //    continue;
                    //}
                    //if (argument.Member.PropertyType == typeof(string))
                    //{
                    //    var lambda =
                    //        Expression.Lambda<Func<TSource, string>>(
                    //            propertyExp,
                    //            new ParameterExpression[1] { parameterExp }
                    //        );
                    //    orderedResult = OrderByCustom_Order<TSource, string>(source, orderedResult, lambda);
                    //    continue;
                    //}

                    Type genericType_arg2 = (Type)argument.Member.PropertyType;
                    Type genericType = typeof(Func<,>);
                    Type[] templateTypeSet = new[] { typeof(TSource), genericType_arg2 };
                    Type implementType = genericType.MakeGenericType(templateTypeSet);

                    var lambda =
                        Expression.Lambda(
                            implementType,
                            propertyExp,
                            new ParameterExpression[1] { parameterExp }
                        );

                    orderedResult = OrderByCustom_Order<TSource>(source, orderedResult, lambda, genericType_arg2) as IOrderedEnumerable<TSource>;
                    continue;
                }

                // System.Linq.Expressions.MethodCallExpression
                if (argument is System.Linq.Expressions.MethodCallExpression methodCallExpression)
                {
                    if (methodCallExpression.NodeType == ExpressionType.Call)
                    {
                        if (methodCallExpression.Arguments.Count == 1)
                        {
                            if (methodCallExpression.Arguments[0].GetType().Name == "PropertyExpression")// System.Linq.Expressions.PropertyExpression
                            {
                                //s.OrderByCustom(a => new { Name = Convert.ToInt32(a.Name) });

                                var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"

                                var propertyName = ((dynamic)methodCallExpression.Arguments[0]).Member.Name; //UserNickName
                                var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName

                                var body =
                                    Expression.Call(
                                        null,
                                        methodCallExpression.Method,
                                        propertyExp
                                    );

                                //if (methodCallExpression.Method.ReturnType == typeof(int))
                                //{
                                //    var lambda =
                                //        Expression.Lambda<Func<TSource, int>>(
                                //            body,
                                //            new ParameterExpression[1] { parameterExp }
                                //        );

                                //    //注: 如果 在执行 methodCallExpression.Method 时发生错误, 这里不会体现
                                //    orderedResult = OrderByCustom_Order<TSource, int>(source, orderedResult, lambda);
                                //    continue;
                                //}

                                Type genericType_arg2 = methodCallExpression.Method.ReturnType;
                                Type genericType = typeof(Func<,>);
                                Type[] templateTypeSet = new[] { typeof(TSource), genericType_arg2 };
                                Type implementType = genericType.MakeGenericType(templateTypeSet);

                                var lambda =
                                    Expression.Lambda(
                                        implementType,
                                        body,
                                        new ParameterExpression[1] { parameterExp }
                                    );

                                //注: 如果 在执行 methodCallExpression.Method 时发生错误, 这里不会体现
                                orderedResult = OrderByCustom_Order<TSource>(source, orderedResult, lambda, genericType_arg2);
                                continue;

                            }
                        }
                    }
                }

                //System.Linq.Expressions.FullConditionalExpression
                if (argument.GetType().Name == "FullConditionalExpression")
                {
                    if (argument.NodeType == ExpressionType.Conditional)
                    {
                        //.OrderByCustom(a => new { Name = string.IsNullOrEmpty(a.Name) ? 0 : Convert.ToInt32(a.Name) })

                        var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"

                        //if (argument.Type == typeof(int))
                        //{
                        //    Expression test = GetExpression_IfTrueOrIfFalse<TSource, int>(argument.Test, parameterExp);
                        //    Expression ifTrue = GetExpression_IfTrueOrIfFalse<TSource, int>(argument.IfTrue, parameterExp);
                        //    Expression ifFalse = GetExpression_IfTrueOrIfFalse<TSource, int>(argument.IfFalse, parameterExp);
                        //    var body = Expression.Condition(test, ifTrue, ifFalse);
                        //    var lambda =
                        //        Expression.Lambda<Func<TSource, int>>(
                        //            body,
                        //            new ParameterExpression[1] { parameterExp }
                        //        );
                        //    orderedResult = OrderByCustom_Order(source, orderedResult, lambda);
                        //    continue;
                        //}

                        Type genericType_arg2 = argument.Type;
                        Type genericType = typeof(Func<,>);
                        Type[] templateTypeSet = new[] { typeof(TSource), genericType_arg2 };
                        Type implementType = genericType.MakeGenericType(templateTypeSet);

                        Expression test = GetExpression_IfTrueOrIfFalse(argument.Test, parameterExp, argument.Type);
                        Expression ifTrue = GetExpression_IfTrueOrIfFalse(argument.IfTrue, parameterExp, argument.Type);
                        Expression ifFalse = GetExpression_IfTrueOrIfFalse(argument.IfFalse, parameterExp, argument.Type);
                        var body = Expression.Condition(test, ifTrue, ifFalse);
                        var lambda =
                            Expression.Lambda(
                                implementType,
                                body,
                                new ParameterExpression[1] { parameterExp }
                            );
                        orderedResult = OrderByCustom_Order<TSource>(source, orderedResult, lambda, genericType_arg2);
                        continue;
                    }
                }

                //"System.Linq.Expressions.MemberAssignment"
                if (argument is System.Linq.Expressions.MemberAssignment)
                {
                    throw new Exception($"完善细节");
                }


                throw new Exception($"不支持的操作,请修改代码:{nameof(OrderByExpression)}");
            }

            if (orderedResult == null)
            {
                throw new Exception("请修改代码, orderedResult 不应该为 null");
            }

            //return orderedResult.ToList(); //注:如果 在执行 methodCallExpression.Method 时发生错误,  这里体现
            return orderedResult;
        }

        private static IOrderedEnumerable<TSource> OrderByCustom_Order<TSource, TOrderType>(
            IEnumerable<TSource> source,
            IOrderedEnumerable<TSource> orderedResult,
            Expression<Func<TSource, TOrderType>> lambda
        ) where TSource : class
        {
            return orderedResult == null
                ? System.Linq.Enumerable.OrderBy(source, lambda.Compile())
                : System.Linq.Enumerable.ThenBy(orderedResult, lambda.Compile());
        }

        private static IOrderedEnumerable<TSource> OrderByCustom_Order<TSource>(
           IEnumerable<TSource> source,
           IOrderedEnumerable<TSource> orderedResult,
           LambdaExpression lambda,
           Type TOrderType
        ) where TSource : class
        {
            var enumerableType = typeof(System.Linq.Enumerable);

            var bindingFlags = BindingFlags.Public | BindingFlags.Static;
            //var methodGenericTypes = new Type[] { typeof(TSource), TOrderType };  //注: typeof(TSource)  == typeof(Object), 此时 typeof(TSource) 必须为public的

            Type[] types = source.GetType().GetGenericArguments();
            Type[] methodGenericTypes;
            if (types.Length == 1)
            {
                methodGenericTypes = new Type[] { types[0], TOrderType };
            }
            else if (types.Length == 2)
            {
                methodGenericTypes = new Type[] { types[1], TOrderType };
            }
            else
            {
                throw new NotSupportedException("请完善代码,当前情况不在单元测试中 ");
            }

            if (orderedResult == null)
            {
                var methodPara = new object[] { source, lambda.Compile() };
                var orderByMehtod = enumerableType
                    .GetMethods(bindingFlags)
                    .First(mi => mi.Name == nameof(System.Linq.Enumerable.OrderBy) && mi.GetParameters().Length == 2);
                return (IOrderedEnumerable<TSource>)orderByMehtod.MakeGenericMethod(methodGenericTypes).Invoke(null, methodPara);
            }
            else
            {
                var methodPara = new object[] { orderedResult, lambda.Compile() };
                var thenByMehtod =
                     enumerableType
                    .GetMethods(bindingFlags)
                    .First(mi => mi.Name == nameof(System.Linq.Enumerable.ThenBy) && mi.GetParameters().Length == 2);
                return (IOrderedEnumerable<TSource>)thenByMehtod.MakeGenericMethod(methodGenericTypes).Invoke(null, methodPara);
            }
        }

        private static Expression GetExpression_IfTrueOrIfFalse(
            Expression expression,
            ParameterExpression parameterExp,
            Type TExpressionValueType
        )
        {
            if (expression is ConstantExpression constantExpression)
            {
                var exp = Expression.Constant(constantExpression.Value, TExpressionValueType); // typeof(int)
                return exp;
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Arguments.Count == 1)
                {
                    if (methodCallExpression.Arguments[0].GetType().Name == "PropertyExpression")// System.Linq.Expressions.PropertyExpression
                    {
                        //Expression<Func<test, int>> lambda = a => string.IsNullOrEmpty(a.Name) ? 0 : Convert.ToInt32(a.Name);

                        //下面的 a 要外面丢进来 ,不然会提示variable '' of type '' referenced from scope '', but it is not defined
                        //var type_TEntity = typeof(TSource);
                        //var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"

                        var propertyName = ((dynamic)methodCallExpression.Arguments[0]).Member.Name; //UserNickName
                        var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName
                        var exp = Expression.Call(
                            null,
                            methodCallExpression.Method, //ToInt32
                            propertyExp
                        );
                        return exp;
                    }
                }
            }

            throw new Exception($"不支持的操作,请修改代码:{nameof(OrderByExpression)}-{nameof(GetExpression_IfTrueOrIfFalse)}");
        }

        #endregion
    }
}
