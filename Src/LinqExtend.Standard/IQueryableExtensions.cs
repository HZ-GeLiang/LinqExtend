using System.Linq.Expressions;

namespace LinqExtend;

public static class IQueryableExtensions
{
    #region 是否已应用OrderBy/OrderByDescending

    // 场景:在尝试Skip()和Take()之前检查IQueryable<T> 是否已应用OrderBy

    /// <summary>
    /// query是否包含了OrderBy/OrderByDescending
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryable"></param>
    /// <returns></returns>
    public static bool IsOrdered<T>(this IQueryable<T> queryable)
    {
        var expression = queryable.Expression;

        while (expression is MethodCallExpression)
        {
            var method = (MethodCallExpression)expression;
            if (method.Method.Name == "OrderBy" || method.Method.Name == "OrderByDescending")
            {
                return true;
            }

            expression = method.Arguments[0];
        }

        return false;
    }

    /// <summary>
    /// query是否包含了OrderBy/OrderByDescending
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static bool IsOrdered<T, TKey>(this IQueryable<T> queryable, Expression<Func<T, TKey>> keySelector)
    {
        // 获取查询表达式树
        var expression = queryable.Expression;

        // 查找 OrderBy 或 OrderByDescending 方法的调用
        var methodCallExpression = expression as MethodCallExpression;
        while (methodCallExpression != null &&
                (methodCallExpression.Method.Name == "OrderBy" || methodCallExpression.Method.Name == "OrderByDescending"))
        {
            // 检查调用的参数是否与提供的键选择器匹配
            if (methodCallExpression.Arguments[1] is UnaryExpression unaryExpression &&
                unaryExpression.Operand is LambdaExpression lambdaExpression &&
                lambdaExpression.Body.ToString() == keySelector.Body.ToString())
            {
                return true;
            }

            // 递归处理表达式树中的下一个方法调用
            methodCallExpression = methodCallExpression.Arguments[0] as MethodCallExpression;
        }

        // 如果没有找到 OrderBy 或 OrderByDescending 方法的调用，则查询没有进行排序
        return false;
    }

    #endregion
}