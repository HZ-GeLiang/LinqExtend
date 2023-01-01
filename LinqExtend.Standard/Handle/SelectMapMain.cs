using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqExtend.Handle
{


    internal class GetExpressionArgs<TSource, TResult>
        where TSource : class
        where TResult : class, new()
    {


#if IEnumerableSource
        public GetExpressionArgs(
            Expression<Func<TSource, TResult>> selector,
            Action<string> OnSelectMapLogTo,
            bool isAutoFill
            )
        {
            this.selector = selector;
            this.OnSelectMapLogTo = OnSelectMapLogTo;
            this.IsAutoFill = isAutoFill;
        }
#elif IQuerableSource
        public GetExpressionArgs(
            Expression<Func<TSource, TResult>> selector,
            Action<string> OnSelectMapLogTo
            )
        {
            this.selector = selector;
            this.OnSelectMapLogTo = OnSelectMapLogTo;
        }
#else

        private GetExpressionArgs(){
          throw new Exception("未知的DefineConstants")
        }

#endif



        /// <summary>
        /// 硬编码部分
        /// </summary>
        public Expression<Func<TSource, TResult>> selector { get; set; }

        /// <summary>
        /// 获得SelectMap映射的日志情况
        /// </summary>
        public Action<string> OnSelectMapLogTo { get; set; }

#if IEnumerableSource
        /// <summary>
        /// 自动填充
        /// </summary>
        public bool IsAutoFill { get; }
#endif  
    }

    internal class SelectMapMain
    {
        ///// <summary>
        ///// 获得SelectMap映射的日志情况
        ///// </summary>
        //public static Action<string> OnSelectMapLogTo { get; set; }

        /// <summary>
        /// 返回一个Expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Expression<Func<TSource, TResult>> SelectMap_GetExpression<TSource, TResult>(
               GetExpressionArgs<TSource, TResult> args
            )
            where TSource : class
            where TResult : class, new()
        {
            var parameterExp = args.selector == null
                  ? Expression.Parameter(typeof(TSource), "a")
                  : args.selector.Parameters[0];  //需要外面丢进来 ,不然会提示 variable '' of type '' referenced from scope '', but it is not defined

            var bindings = SelectMap_GetExpression_GetBindings(args, parameterExp);

            if (args.OnSelectMapLogTo != null)
            {
                string log = SelectMapMain.GetSelectMapLog(bindings);
                args.OnSelectMapLogTo.Invoke(log);
            }

            var lambda = GetLambda<TSource, TResult>(bindings, parameterExp);
            return lambda;
        }

        /// <summary>
        /// 获得映射关系
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="args"></param> 
        /// <param name="parameterExp"></param>
        /// <returns></returns>
        public static List<MemberBinding> SelectMap_GetExpression_GetBindings<TSource, TResult>(
            GetExpressionArgs<TSource, TResult> args,
            ParameterExpression parameterExp
         )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();
            var process = new SelectMapProcess<TSource, TResult>();

            var selector_bindings = GetBindings(args, process);//step1:硬编码
            if (selector_bindings != null)
            {
                bindings.AddRange(selector_bindings);
            }

            var autoMap_bindings = GetBindings(parameterExp, process);//step2:根据名字自动映射(目前只处理内置类型的)
            if (autoMap_bindings != null)
            {
                bindings.AddRange(autoMap_bindings);
            }


#if IEnumerableSource
            {

                //todo:SelectMap_Enumerable_支持值对象
                //1等公民的处理(自定义类型)
                //var unmappedCustomProperty = process.GetUnmappedPropertyWithCustom();

                //foreach (var propertyName in unmappedCustomProperty)
                //{
                //    process.DealPropertyWithCustom(propertyName);

                //    //var memberAssignment = Expression.Bind(
                //    //    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                //    //    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                //    //);

                //    //bindings.Add(memberAssignment);
                //}

            }

#endif

#if IEnumerableSource
            var _isAutoFill = args.IsAutoFill;
#elif IQuerableSource
            var _isAutoFill = true;
#else
            throw new Exception("未知的DefineConstants")
#endif
            if (_isAutoFill)//step3
            {
                //var == Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>>
                var selectorLast = GetSelectorLast<TSource, TResult>();

                var last_bindings = GetBindings(parameterExp, process, selectorLast);
                if (last_bindings != null)
                {
                    bindings.AddRange(last_bindings);
                }
            }

            return bindings;

        }


        /// <summary>
        /// step1, 硬编码部分
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="args"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static List<MemberBinding> GetBindings<TSource, TResult>(
                GetExpressionArgs<TSource, TResult> args,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();

            if (args.selector == null)
            {
                return bindings;
            }

            var body = args.selector.Body;
            if (body is System.Linq.Expressions.MemberInitExpression memberInitExpression)
            {
                foreach (var item in memberInitExpression.Bindings)
                {
                    var propertyName = item.Member.Name;
                    process.DealPropertyWithBuildIn(propertyName);
                }

                bindings.AddRange(memberInitExpression.Bindings);
            }
            else
            {
                throw new NotSupportedException("当前selector的写法暂不支持,请修改程序或提issue");
            }

            return bindings;

        }

        /// <summary>
        /// step2,根据名字自动映射(目前只处理内置类型的)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="parameterExp"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public static List<MemberBinding> GetBindings<TSource, TResult>(
                ParameterExpression parameterExp,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();
            //1等公民的处理(内置类型)
            var unmappedBuildInProperty = process.GetUnmappedPropertyWithBuildIn(rank: 1);

            foreach (var propertyName in unmappedBuildInProperty)
            {
                process.DealPropertyWithBuildIn(propertyName);

                var memberAssignment = Expression.Bind(
                    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                );

                bindings.Add(memberAssignment);
            }


            return bindings;
        }

        /// <summary>
        /// step3,最后兜底部分的处理(自动映射,二等公民)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="parameterExp"></param>
        /// <param name="process"></param>
        /// <param name="selectorLast"></param>
        /// <returns></returns>
        public static List<MemberBinding> GetBindings<TSource, TResult>(
                ParameterExpression parameterExp,
                SelectMapProcess<TSource, TResult> process,
                Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast
            )
            where TSource : class
            where TResult : class, new()
        {
            if (selectorLast == null)
            {
                return new List<MemberBinding>();
            }
            var bindings = selectorLast.Invoke(parameterExp, process);
            return bindings;
        }

        /// <summary>
        /// 第三部分的处理逻辑:自动映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> GetSelectorLast<TSource, TResult>()
            where TSource : class
            where TResult : class, new()
        {
            Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast = (parameterExp, process) =>
            {
                //最后未处理的部分(目前只处理内置类型的)
                var bindings = new List<MemberBinding>();
                var unmappedPropertyNameList = process.GetUnmappedProperty();

                //if (unmappedPropertyNameList.Count == 0)
                //{
                //    return bindings;
                //}
                var collection = process.Source.Custom;

                var dict = collection.ToDictionary(a => a, a => new PropertyGroup(a.PropertyType));

                foreach (var propertyName in unmappedPropertyNameList)
                {
#if DEBUG
                    Console.WriteLine(propertyName);
#endif

                    foreach (var kv in dict)
                    {
                        var objProcess = kv.Value;

                        if (
                            (!objProcess.BuildInPropertyProcessList.ContainsKey(propertyName)) ||
                            objProcess.BuildInPropertyProcessList[propertyName] == true //objProcess中propertyName是未处理过的
                        )
                        {
                            continue;
                        }

                        PropertyInfo prop = kv.Value.BuildIn.First(a => string.Compare(a.Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0);

                        if (prop != null)
                        {
                            var objType = kv.Key.PropertyType;

                            dict[kv.Key].DealWithBuildInProperty(propertyName);
                            process.DealPropertyWithBuildIn(propertyName, check: false);

#if DEBUG
                            var debugTxt = $@"{objType}:{propertyName}";
                            Console.WriteLine(debugTxt);
#endif
                            var objName = kv.Key.Name; //order

                            var exp = Expression.Property(parameterExp, objName);//a.order

                            //添加 binding
                            var memberAssignment = Expression.Bind(
                               typeof(TResult).GetProperty(propertyName),
                               Expression.Property(exp, propertyName)// TSource 的 a.order.Id
                            );

                            bindings.Add(memberAssignment);
                            break;// 防止被下一个 kv对象 处理
                        }
                    }
                }
                return bindings;
            };

            return selectorLast;
        }

        public static Expression<Func<TSource, TResult>> GetLambda<TSource, TResult>(List<MemberBinding> bindings, ParameterExpression parameterExp)
          where TSource : class
          where TResult : class, new()
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

        public static string GetSelectMapLog(List<MemberBinding> bindings)
        {
            if (bindings == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (MemberBinding binding in bindings)
            {
                sb.AppendLine($@"{binding}");
            }
            var log = sb.ToString();
            return log;
        }

    }
}
