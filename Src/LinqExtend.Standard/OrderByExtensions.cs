using LinqExtend.Consts;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExtend;

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
    /// 自定义Func来实现OrderBy
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

    /// <inheritdoc cref="OrderByExpression{TSource}(IEnumerable{TSource}, Expression{Func{TSource, dynamic}}, HashSet{string})"/>
    public static IEnumerable<TSource> OrderByExpression<TSource>(
        this IEnumerable<TSource> source,
        Expression<Func<TSource, dynamic>> orderExpression
    ) where TSource : class
    {
        return OrderByExpression(source, orderExpression, (HashSet<string>)null);
    }

    /// <inheritdoc cref="OrderByExpression{TSource}(IEnumerable{TSource}, Expression{Func{TSource, dynamic}}, HashSet{string})"/>
    public static IEnumerable<TSource> OrderByExpression<TSource>(
        this IEnumerable<TSource> source,
        Expression<Func<TSource, dynamic>> orderExpression,
        Expression<Func<TSource, dynamic>> descending
    ) where TSource : class
    {
        HashSet<string> ht_desc = GetDesc(descending);
        return OrderByExpression(source, orderExpression, ht_desc);
    }

    private static HashSet<string> GetDesc<TSource>(Expression<Func<TSource, dynamic>> descending)
        where TSource : class
    {
        HashSet<string> ht_desc = null;
        if (descending != null)
        {
            ht_desc = new HashSet<string>(StringComparer.Ordinal);

            if (descending.Body is System.Linq.Expressions.UnaryExpression bodyUnaryExpression)
            {
                if (bodyUnaryExpression.Operand is System.Linq.Expressions.Expression operandExpression)
                {
                    var name = ((dynamic)operandExpression).Member.Name as string;
                    ht_desc.Add(name);
                }
                else
                {
                    throw new Exception("请修改代码, 未知的情况");
                }
            }
            else if (descending.Body.GetType().FullName == ConstTypeFullName.PropertyExpression)
            {
                var name = ((dynamic)descending.Body).Member.Name as string;
                ht_desc.Add(name);
            }
            else if (descending.Body is System.Linq.Expressions.NewExpression bodyNewExpression)
            {
                var arguments = bodyNewExpression.Arguments;
                foreach (var item in arguments)
                {
                    if (item.GetType().FullName == ConstTypeFullName.PropertyExpression)
                    {
                        var name = ((dynamic)item).Member.Name as string;
                        ht_desc.Add(name);
                    }
                    else
                    {
                        throw new Exception("请修改代码, 未知的情况");
                    }
                }
            }
            else
            {
                throw new Exception("请修改代码, 未知的情况");
            }
        }
        return ht_desc;
    }

    /// <summary>
    /// 自定义Expression来实现OrderBy
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">数据源</param>
    /// <param name="orderExpression">用来写  new 匿名类</param>
    /// <param name="descending">倒序排序的字段</param>
    /// <returns>排好序的对象</returns>
    /// <exception cref="Exception">未考虑到的情况时会触发</exception>
    /// <exception cref="ArgumentException">orderExpression 中 new 一个自定义类时会触发</exception>
    public static IEnumerable<TSource> OrderByExpression<TSource>(
        this IEnumerable<TSource> source,
        Expression<Func<TSource, dynamic>> orderExpression,
        HashSet<string> descending
    ) where TSource : class
    {
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
        IList members;

        var orderExpressionBodyType = orderExpression.Body.GetType();
        if (orderExpression.Body is System.Linq.Expressions.NewExpression newExpression)
        {
            // new 匿名类
            arguments = newExpression.Arguments; //ReadOnlyCollection<Expression>
            members = newExpression.Members;//ReadOnlyCollection<System.Reflection.MemberInfo>
        }
        else if (orderExpression.Body is System.Linq.Expressions.MemberInitExpression memberInitExpression)
        {
            // new 一个具体的类
            arguments = memberInitExpression.Bindings; //ReadOnlyCollection<MemberBinding>
            members = null;
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

        var eachCount = -1;
        foreach (dynamic argument in arguments)
        {
            eachCount++;

            // System.Linq.Expressions.PropertyExpression
            if (argument.GetType().FullName == ConstTypeFullName.PropertyExpression)
            {
                // s.OrderByCustom(a => new { a.Name });
                var propertyName = argument.Member.Name;
                var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"
                var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName

                var isDescending = false;
                if (descending != null && descending.Contains(propertyName))
                {
                    isDescending = true;
                }
                //if (argument.Member.PropertyType == typeof(int))
                //{
                //    //typeof(Func<TSource, int>);

                //    var lambda = Expression.Lambda<Func<TSource, int>>(
                //        propertyExp,
                //        new ParameterExpression[1] { parameterExp }
                //    );

                //    orderedResult = GetIOrderedEnumerable<TSource, int>(source, orderedResult, lambda, isDescending);
                //    continue;
                //}
                //if (argument.Member.PropertyType == typeof(string))
                //{
                //    var lambda =
                //        Expression.Lambda<Func<TSource, string>>(
                //            propertyExp,
                //            new ParameterExpression[1] { parameterExp }
                //        );
                //    orderedResult = GetIOrderedEnumerable<TSource, string>(source, orderedResult, lambda, isDescending);
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

                orderedResult = GetIOrderedEnumerable<TSource>(source, orderedResult, lambda, genericType_arg2, isDescending) as IOrderedEnumerable<TSource>;
                continue;
            }

            // System.Linq.Expressions.MethodCallExpression
            if (argument is System.Linq.Expressions.MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.NodeType == ExpressionType.Call)
                {
                    if (methodCallExpression.Arguments.Count == 1)
                    {
                        // System.Linq.Expressions.PropertyExpression
                        if (methodCallExpression.Arguments[0].GetType().FullName == ConstTypeFullName.PropertyExpression)
                        {
                            var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"
                            var propertyName = ((dynamic)methodCallExpression.Arguments[0]).Member.Name; //UserNickName
                            var propertyExp = Expression.Property(parameterExp, propertyName);//a.UserNickName
                            var isDescending = false;
                            if (descending != null && descending.Contains(propertyName))
                            {
                                isDescending = true;
                            }

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
                            //    orderedResult = GetIOrderedEnumerable<TSource, int>(source, orderedResult, lambda, isDescending);
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
                            orderedResult = GetIOrderedEnumerable<TSource>(source, orderedResult, lambda, genericType_arg2, isDescending);
                            continue;
                        }
                    }
                }
            }

            //System.Linq.Expressions.FullConditionalExpression
            if (argument.GetType().FullName == "System.Linq.Expressions.FullConditionalExpression")
            {
                if (argument.NodeType == ExpressionType.Conditional)
                {
                    var parameterExp = Expression.Parameter(type_TEntity, orderExpression.Parameters[0].Name);// "a"

                    var isDescending = false;
                    if (members != null)
                    {
                        foreach (dynamic item in members)
                        {
                            if (descending != null && descending.Contains(item.Name))
                            {
                                isDescending = true;
                                break;
                            }
                        }
                    }

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
                    //    orderedResult = GetIOrderedEnumerable(source, orderedResult, lambda, isDescending);
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
                    orderedResult = GetIOrderedEnumerable<TSource>(source, orderedResult, lambda, genericType_arg2, isDescending: false);
                    continue;
                }
            }

            //"System.Linq.Expressions.MemberAssignment"
            if (argument is System.Linq.Expressions.MemberAssignment)
            {
                //AnonymousType
                throw new ArgumentException($@"{nameof(OrderByExpression)}的参数{nameof(orderExpression)}只支持匿名类型");
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

    private static IOrderedEnumerable<TSource> GetIOrderedEnumerable<TSource, TOrderType>(
        IEnumerable<TSource> source,
        IOrderedEnumerable<TSource> orderedResult,
        Expression<Func<TSource, TOrderType>> lambda,
        bool isDescending
    ) where TSource : class
    {
        if (isDescending)
        {
            return orderedResult == null
               ? Enumerable.OrderByDescending(source, lambda.Compile())
               : Enumerable.ThenByDescending(orderedResult, lambda.Compile());
        }
        else
        {
            return orderedResult == null
                ? Enumerable.OrderBy(source, lambda.Compile())
                : Enumerable.ThenBy(orderedResult, lambda.Compile());
        }
    }

    private static IOrderedEnumerable<TSource> GetIOrderedEnumerable<TSource>(
        IEnumerable<TSource> source,
        IOrderedEnumerable<TSource> orderedResult,
        LambdaExpression lambda,
        Type TOrderType,
        bool isDescending
    ) where TSource : class
    {
        var enumerableType = typeof(Enumerable);

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
            var methodName = isDescending ? nameof(Enumerable.OrderByDescending) : nameof(Enumerable.OrderBy);

            var methodPara = new object[] { source, lambda.Compile() };
            var orderByMehtod = enumerableType
                .GetMethods(bindingFlags)
                .First(mi => mi.Name == methodName && mi.GetParameters().Length == 2);
            return (IOrderedEnumerable<TSource>)orderByMehtod.MakeGenericMethod(methodGenericTypes).Invoke(null, methodPara);
        }
        else
        {
            var methodName = isDescending ? nameof(Enumerable.ThenByDescending) : nameof(Enumerable.ThenBy);

            var methodPara = new object[] { orderedResult, lambda.Compile() };
            var thenByMehtod =
                 enumerableType
                .GetMethods(bindingFlags)
                .First(mi => mi.Name == methodName && mi.GetParameters().Length == 2);
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
                // System.Linq.Expressions.PropertyExpression
                if (methodCallExpression.Arguments[0].GetType().FullName == ConstTypeFullName.PropertyExpression)
                {
                    //Expression<Func<test, int>> lambda = a => string.IsNullOrEmpty(a.Name) ? 0 : Convert.ToInt32(a.Name);

                    //下面的 a 要外面丢进来 ,不然会提示 variable '' of type '' referenced from scope '', but it is not defined
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