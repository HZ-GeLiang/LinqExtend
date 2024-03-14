using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LinqExtend.EF.ExtensionMethod
{
    public static class WhereExtension
    {
        /// <summary>
        /// 有值(不为空 IsNotBlank): COALESCE([t].[Name], N'') <> N''
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IQueryable<TSource> WhereHasValue<TSource, TProperty>(this IQueryable<TSource> query,
            Expression<Func<TSource, TProperty>> propertySelector)
        {
            //把 sql语句 IsNull(字段, "") != "" 对应的表达式 Expression<Func<TSource, bool>> exp = a => (a.字段 ?? "") != ""; 翻译成对应的表达式树

            string propertyName = GetPropertyName(propertySelector);

            var type_TEntity = typeof(TSource);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                throw new Exception($"{propertyName}不在{typeof(TSource).FullName}类型中");
            }

            if (prop_Info.PropertyType != typeof(string))
            {
                throw new Exception($"{propertyName}不在{typeof(TSource).FullName}类型中只能为string类型");
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var left = Expression.MakeBinary(
                ExpressionType.NotEqual,
                Expression.Coalesce(
                    Expression.MakeMemberAccess(parameterExp, prop_Info),
                    Expression.Constant("", typeof(string))
                ),
                Expression.Constant("")
            //dnspy 反编译没有这个2个, ExpressionTreeToString有
            //,false
            //,typeof(string).GetMethod("op_Inequality")
            );
            var lambda = Expression.Lambda<Func<TSource, bool>>(left, parameterExp);

            query = query.Where(lambda);
            return query;
        }

        /// <summary>
        /// 没有值(为空 IsBlank):  COALESCE([t].[Name], N'') = N''
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IQueryable<TSource> WhereNotValue<TSource, TProperty>(this IQueryable<TSource> query,
            Expression<Func<TSource, TProperty>> propertySelector)
        {
            //把 sql语句 IsNull(字段, "") = "" 对应的表达式 Expression<Func<TSource, bool>> exp = a => (a.字段 ?? "") == ""; 翻译成对应的表达式树

            string propertyName = GetPropertyName(propertySelector);

            var type_TEntity = typeof(TSource);
            var prop_Info = type_TEntity.GetProperty(propertyName);
            if (prop_Info == null)
            {
                throw new Exception($"{propertyName}不在{typeof(TSource).FullName}类型中");
            }

            if (prop_Info.PropertyType != typeof(string))
            {
                throw new Exception($"{propertyName}不在{typeof(TSource).FullName}类型中只能为string类型");
            }

            var parameterExp = Expression.Parameter(type_TEntity, "a");
            var left = Expression.MakeBinary(
                ExpressionType.Equal,
                Expression.Coalesce(
                    Expression.MakeMemberAccess(parameterExp, prop_Info),
                    Expression.Constant("", typeof(string))
                ),
                Expression.Constant("")
            //dnspy 反编译没有这个2个, ExpressionTreeToString有
            //,false
            //,typeof(string).GetMethod("op_Inequality")
            );
            var lambda = Expression.Lambda<Func<TSource, bool>>(left, parameterExp);

            query = query.Where(lambda);
            return query;
        }

        private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (propertySelector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
            {
                return operand.Member.Name;
            }
            else
            {
                throw new ArgumentException("Invalid property selector expression.");
            }
        }

        public static IQueryable<TSource> WhereIfNotValue<TSource, TProperty>(this IQueryable<TSource> query,
            bool condition, Expression<Func<TSource, TProperty>> propertySelector)
        {
            if (condition)
            {
                return query.WhereNotValue(propertySelector);
            }
            else
            {
                return query;
            }
        }

        public static IQueryable<TSource> WhereIfHasValue<TSource, TProperty>(this IQueryable<TSource> query,
           bool condition, Expression<Func<TSource, TProperty>> propertySelector)
        {
            if (condition)
            {
                return query.WhereHasValue(propertySelector);
            }
            else
            {
                return query;
            }
        }

    }


}
