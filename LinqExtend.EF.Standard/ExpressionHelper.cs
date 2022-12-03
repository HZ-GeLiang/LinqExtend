using LinqExtend.EF.Consts;
using LinqExtend.EF.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExtend.EF
{
    /// <summary>
    /// 生成Expression的帮助类
    /// </summary>
    public sealed class ExpressionHelper
    {
        /// <summary>
        /// 2个对象是否相等, 一般用在比较值对象上
        /// </summary>
        /// <typeparam name="T1">对象1</typeparam>
        /// <typeparam name="T2">对象2</typeparam>
        /// <param name="propAccessor">要比较的值对象</param>
        /// <param name="compareObject">比较的值对象</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static Expression<Func<T1, bool>> MakeEqual<T1, T2>(Expression<Func<T1, T2>> propAccessor, T2 compareObject)
           where T1 : class
           where T2 : class
        {
            //这个代码来自 杨中科 ,  之所以是 internal, 是因为 单元测试没通过, 觉得应该可以调整, 当作一个todo

            BinaryExpression conditionalExpr = null;
            foreach (var prop in typeof(T2).GetProperties())
            {
                BinaryExpression equalExpr;
                //other的prop属性的值
                object otherValue = null;
                if (compareObject != null)
                {
                    otherValue = prop.GetValue(compareObject);
                }
                Type propType = prop.PropertyType;
                //访问待比较的属性
                var leftExpr = Expression.MakeMemberAccess(
                    propAccessor.Body,//要取出来Body部分，不能带参数
                    prop
                );
                Expression rightExpr = Expression.Convert(Expression.Constant(otherValue), propType);
                if (propType.IsPrimitive)//基本数据类型和复杂类型比较方法不一样
                {
                    equalExpr = Expression.Equal(leftExpr, rightExpr);
                }
                else
                {
                    equalExpr = Expression.MakeBinary(ExpressionType.Equal,
                        leftExpr, rightExpr, false,
                        prop.PropertyType.GetMethod("op_Equality")
                    );
                }
                if (conditionalExpr == null)
                {
                    conditionalExpr = equalExpr;
                }
                else
                {
                    conditionalExpr = Expression.AndAlso(conditionalExpr, equalExpr);
                }
            }
            if (conditionalExpr == null)
            {
                throw new ArgumentException("There should be at least one property.");
            }
            var e1 = propAccessor.Parameters.Single();//提取出来参数
            return Expression.Lambda<Func<T1, bool>>(conditionalExpr, e1);
        }

        /// <summary>
        /// 没有值 (这个没有值是语义上的)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Expression<Func<TEntity, bool>> IsNullOrEmpty<TEntity>(Expression<Func<TEntity, string>> propAccessor)
            where TEntity : class
        {
            //把 string.IsNullOrEmpty 翻译成对应的表达式树

            var expression = propAccessor.Body;
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                var type_TEntity = typeof(TEntity);
                //var p1 = Expression.Parameter(type_TEntity,"b");                
                var p1 = propAccessor.Parameters.Single();//等价上面的写法 

                string propName = null;

                if (expression.NodeType == ExpressionType.MemberAccess)
                {
                    propName = ((dynamic)expression).Member.Name;
                }
                else
                {
                    DebuggerHelper.Break();
                    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
                }

                var lambda =
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Call(
                        typeof(string).GetMethod("IsNullOrEmpty"),
                        Expression.MakeMemberAccess(p1,
                           type_TEntity.GetProperty(propName)//"AuthorName"
                        )
                    ),
                    p1
                );

                return lambda;
            }
            else
            {
                throw new ArgumentException("propAccessor 的写法暂不被支持.");
            }
        }


        // 删除状态的
        public static Expression<Func<TEntity, bool>> IsDeleted<TEntity>(Expression<Action<TEntity>> propAccessor)
        {
            throw new NotImplementedException();
        }

        // 软删除状态的
        public static Expression<Func<TEntity, bool>> IsSoftDelete<TEntity>(Expression<Action<TEntity>> propAccessor)
        {
            throw new NotImplementedException();
        }

        // 没有被删除的
        public static Expression<Func<TEntity, bool>> IsNotDeleted<TEntity>(Expression<Action<TEntity>> propAccessor)
        {
            throw new NotImplementedException();
        }

        // 没有被软删除的
        public static Expression<Func<TEntity, bool>> IsNotSoftDelete<TEntity>(Expression<Action<TEntity>> propAccessor)
        {
            throw new NotImplementedException();
        }
    }


    /// <inheritdoc cref="ExpressionHelper"/>
    public sealed class ExpressionHelper<TEntity> where TEntity : class
    {
        /// <inheritdoc cref="ExpressionHelper.MakeEqual{T1, T2}(Expression{Func{T1, T2}}, T2)" />
        internal static Expression<Func<TEntity, bool>> MakeEqual<T2>(Expression<Func<TEntity, T2>> propAccessor, T2 compareObject)
                where T2 : class
        {
            return ExpressionHelper.MakeEqual<TEntity, T2>(propAccessor, compareObject);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNullOrEmpty{TEntity}(Expression{Func{TEntity, string}})"/>     
        public static Expression<Func<TEntity, bool>> IsNullOrEmpty(Expression<Func<TEntity, string>> propAccessor)
        {
            return ExpressionHelper.IsNullOrEmpty<TEntity>(propAccessor);
        }

    }
}
