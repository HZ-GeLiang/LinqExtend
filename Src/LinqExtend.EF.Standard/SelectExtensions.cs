using System.Linq.Expressions;

namespace LinqExtend.EF;

/// <summary>
/// Select的扩展
/// </summary>
public static class SelectExtensions
{
    /// <summary>
    /// 获得SelectMap映射的日志情况
    /// </summary>
    public static Action<string> OnSelectMapLogTo { get; set; }

    /*public static Action<string> OnSelectMapLogTo
        get
        {
            return SelectMapMain.OnSelectMapLogTo;
        }
        set
        {
            SelectMapMain.OnSelectMapLogTo = value;
        }
    }*/

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