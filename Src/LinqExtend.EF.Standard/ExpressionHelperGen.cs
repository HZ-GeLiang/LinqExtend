using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LinqExtend.EF
{
    /// <inheritdoc cref="ExpressionHelper"/>
    public sealed class ExpressionHelper<TEntity> where TEntity : class
    {
        /// <inheritdoc cref="ExpressionHelper.MakeEqual{T1, T2}(Expression{Func{T1, T2}}, T2)" />
        internal static Expression<Func<TEntity, bool>> MakeEqual<T2>(Expression<Func<TEntity, T2>> propAccessor, T2 compareObject)
             where T2 : class
        {
            return ExpressionHelper.MakeEqual<TEntity, T2>(propAccessor, compareObject);
        }

        /// <inheritdoc cref="ExpressionHelper.IsEmpty{TEntity}(Expression{Func{TEntity, string}})"/>
        public static Expression<Func<TEntity, bool>> IsEmpty(Expression<Func<TEntity, string>> propAccessor)
        {
            return ExpressionHelper.IsEmpty<TEntity>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNotEmpty{TEntity}(Expression{Func{TEntity, string}})"/>
        public static Expression<Func<TEntity, bool>> IsNotEmpty(Expression<Func<TEntity, string>> propAccessor)
        {
            return ExpressionHelper.IsNotEmpty<TEntity>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsDeleted<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsDeleted<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsSoftDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsSoftDelete<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsSoftDeleted<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNotDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsNotDeleted<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsNotDeleted<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNotSoftDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsNotSoftDelete<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsNotSoftDeleted<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.SelectMap{TSource, TResult}()" />
        public static Expression<Func<TEntity, TResult>> SelectMap<TResult>() where TResult : class, new()
        {
            return ExpressionHelper.SelectMap<TEntity, TResult>();
        }
    }

}
