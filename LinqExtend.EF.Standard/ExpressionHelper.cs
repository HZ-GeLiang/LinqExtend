using LinqExtend.EF.Consts;
using LinqExtend.EF.ExtendMethods;
using LinqExtend.EF.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                object toCompareValue = null;
                if (compareObject != null)
                {
                    toCompareValue = prop.GetValue(compareObject);
                }
                Type propType = prop.PropertyType;
                //访问待比较的属性
                var leftExpr = Expression.MakeMemberAccess(
                    propAccessor.Body,//要取出来Body部分，不能带参数
                    prop
                );
                Expression rightExpr = Expression.Convert(Expression.Constant(toCompareValue), propType);
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
        /// 是空的,没有值 (这个没有值是语义上的) 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Expression<Func<TEntity, bool>> IsEmpty<TEntity>(Expression<Func<TEntity, string>> propAccessor)
            where TEntity : class
        {
            //把 string.IsNullOrEmpty 翻译成对应的表达式树

            var expression = propAccessor.Body;
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                var type_TEntity = typeof(TEntity);
                //var p1 = Expression.Parameter(type_TEntity,"b");                
                var p1 = propAccessor.Parameters.Single();//等价上面的写法 

                string propName;

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

        /// <summary>
        /// 不为空,有值 (这个有值是语义上的)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Expression<Func<TEntity, bool>> IsNotEmpty<TEntity>(Expression<Func<TEntity, string>> propAccessor)
              where TEntity : class
        {
            //把 !string.IsNullOrEmpty 翻译成对应的表达式树

            var expression = propAccessor.Body;
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                var type_TEntity = typeof(TEntity);
                //var p1 = Expression.Parameter(type_TEntity,"b");                
                var p1 = propAccessor.Parameters.Single();//等价上面的写法 

                string propName;

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
                        Expression.Not(
                            Expression.Call(
                                typeof(string).GetMethod("IsNullOrEmpty"),
                                Expression.MakeMemberAccess(p1,
                                   type_TEntity.GetProperty(propName)//"AuthorName"
                                )
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

        private static Dictionary<Type, ConstantExpression> trueValue = new Dictionary<Type, ConstantExpression>()
        {
            {typeof(bool),     Expression.Constant((bool)true,  typeof(bool))},
            {typeof(bool?),    Expression.Constant((bool?)true, typeof(bool?))},
            {typeof(char),     Expression.Constant((char)'1',   typeof(char))},
            {typeof(char?),    Expression.Constant((char?)'1',   typeof(char?))},

            //11种数字
            {typeof(sbyte),    Expression.Constant((sbyte)1,    typeof(sbyte))}, //-128,127
            {typeof(byte),     Expression.Constant((byte)1,      typeof(byte))}, //0,255
            {typeof(short),    Expression.Constant((short)1,    typeof(short))}, //-32768,32767
            {typeof(ushort),   Expression.Constant((ushort)1,   typeof(ushort))},
            {typeof(int),      Expression.Constant((int)1,      typeof(int))},
            {typeof(uint),     Expression.Constant((uint)1,      typeof(uint))},
            {typeof(long),     Expression.Constant((long)1,     typeof(long))},
            {typeof(ulong),    Expression.Constant((ulong)1,   typeof(ulong))},
            {typeof(float),    Expression.Constant((float)1,    typeof(float))},
            {typeof(double),   Expression.Constant((double)1,   typeof(double))},
            {typeof(decimal),  Expression.Constant((decimal)1,  typeof(decimal))},

            {typeof(sbyte?),    Expression.Constant((sbyte?)1,    typeof(sbyte?))},
            {typeof(byte?),     Expression.Constant((byte?)1,      typeof(byte?))},
            {typeof(short?),    Expression.Constant((short?)1,    typeof(short?))},
            {typeof(ushort?),   Expression.Constant((ushort?)1,   typeof(ushort?))},
            {typeof(int?),      Expression.Constant((int?)1,      typeof(int?))},
            {typeof(uint?),     Expression.Constant((uint?)1,      typeof(uint?))},
            {typeof(long?),     Expression.Constant((long?)1,     typeof(long?))},
            {typeof(ulong?),    Expression.Constant((ulong?)1,   typeof(ulong?))},
            {typeof(float?),    Expression.Constant((float?)1,    typeof(float?))},
            {typeof(double?),   Expression.Constant((double?)1,   typeof(double?))},
            {typeof(decimal?),  Expression.Constant((decimal?)1,  typeof(decimal?))},
        };

        /// <summary>
        /// 删除状态的,即软删除的
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPropType"></typeparam>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static Expression<Func<TEntity, bool>> IsDeleted<TEntity, TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            /*
               .Where(b => b.IsDel.HasValue && b.IsDel == true)
               .Where(b => b.IsDel == true)    //同时兼顾了 bool? 和 bool
            */

            var expression = propAccessor.Body;
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                var type_TEntity = typeof(TEntity);
                var p1 = propAccessor.Parameters.Single();//等价上面的写法 

                string propName;

                if (expression.NodeType == ExpressionType.MemberAccess)
                {
                    propName = ((dynamic)expression).Member.Name;
                }
                else
                {
                    DebuggerHelper.Break();
                    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
                }

                var prop = type_TEntity.GetProperty(propName);
                //ConstantExpression propValue =
                //    prop.PropertyType == typeof(Nullable<bool>)
                //    //TypeHelper.IsNullableType(prop.PropertyType) //  未做 性能测试, 这种更加通用
                //    ? Expression.Constant(true, typeof(Nullable<bool>))
                //    : Expression.Constant(true);

                if (!trueValue.ContainsKey(prop.PropertyType))
                {
                    throw new ArgumentException($"Type暂不被支持.{prop.PropertyType}");
                }
                ConstantExpression propValue = trueValue[prop.PropertyType];

                var lambda =
                    Expression.Lambda<Func<TEntity, bool>>(
                        Expression.Equal(
                            Expression.MakeMemberAccess(p1, prop),
                            propValue
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

        /// <inheritdoc cref="IsDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsSoftDelete<TEntity, TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return IsDeleted(propAccessor);
        }

        /// <summary>
        /// 没有被删除的,,即未标记为软删除的
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPropType"></typeparam>
        /// <param name="propAccessor"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Expression<Func<TEntity, bool>> IsNotDeleted<TEntity, TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            /*
                .Where(b => b.IsDel == null || b.IsDel == false)
                .Where(b => b.IsDel != true)  //同时兼顾了 bool? 和 bool
             */
            var expression = propAccessor.Body;
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                var type_TEntity = typeof(TEntity);
                var p1 = propAccessor.Parameters.Single();//等价上面的写法 

                string propName;

                if (expression.NodeType == ExpressionType.MemberAccess)
                {
                    propName = ((dynamic)expression).Member.Name;
                }
                else
                {
                    DebuggerHelper.Break();
                    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
                }

                var prop = type_TEntity.GetProperty(propName);
                //ConstantExpression propValue =
                //    prop.PropertyType == typeof(Nullable<bool>)
                //    //TypeHelper.IsNullableType(prop.PropertyType) //  未做 性能测试, 这种更加通用
                //    ? Expression.Constant(true, typeof(Nullable<bool>))
                //    : Expression.Constant(true);

                if (!trueValue.ContainsKey(prop.PropertyType))
                {
                    throw new ArgumentException($"Type暂不被支持.{prop.PropertyType}");
                }
                ConstantExpression propValue = trueValue[prop.PropertyType];

                var lambda =
                    Expression.Lambda<Func<TEntity, bool>>(
                        Expression.NotEqual(
                            Expression.MakeMemberAccess(p1, prop),
                            propValue
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

        /// <inheritdoc cref="IsNotDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})" />
        public static Expression<Func<TEntity, bool>> IsNotSoftDelete<TEntity, TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return IsNotDeleted(propAccessor);
        }


        /// <inheritdoc cref="SelectMap{TSource, TResult}(Func{TSource, TResult})" />
        public static Expression<Func<TSource, TResult>> SelectMap<TSource, TResult>()
            where TSource : class
            where TResult : class, new()
        {
            return SelectMap((Func<TSource, TResult>)null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, TResult>> SelectMap<TSource, TResult>(Func<TSource, TResult> selector)
         where TSource : class
         where TResult : class, new()
        {
            var lambda = SelectExtensions.SelectMap_GetExpression<TSource, TResult>(selector);
            return lambda;
        }

    }

    /// <inheritdoc cref="ExpressionHelper"/>
    public static class ExpressionExtesion
    {
        /// <inheritdoc cref="ExpressionHelper.SelectMap{TSource, TResult}(Func{TSource, TResult})" />
        public static IQueryable<TResult> SelectMap<TSource, TResult>(this IQueryable<TSource> query, Func<TSource, TResult> selector)
          where TSource : class
          where TResult : class, new()
        {
            var exp = ExpressionHelper.SelectMap<TSource, TResult>(selector);
            IQueryable<TResult> querySelect = query.Select(exp);
            return querySelect;
        }
    }


    #region 泛型类
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

        /// <inheritdoc cref="ExpressionHelper.IsSoftDelete{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsSoftDelete<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsSoftDelete<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNotDeleted{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsNotDeleted<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsNotDeleted<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.IsNotSoftDelete{TEntity, TPropType}(Expression{Func{TEntity, TPropType}})"/>
        public static Expression<Func<TEntity, bool>> IsNotSoftDelete<TPropType>(Expression<Func<TEntity, TPropType>> propAccessor)
        {
            return ExpressionHelper.IsNotSoftDelete<TEntity, TPropType>(propAccessor);
        }

        /// <inheritdoc cref="ExpressionHelper.SelectMap{TSource, TResult}(Func{TSource, TResult})" />
        public static Expression<Func<TEntity, TResult>> SelectMap<TResult>()
            where TResult : class, new()
        {
            return ExpressionHelper.SelectMap<TEntity, TResult>();
        }
    }

    #endregion
}
