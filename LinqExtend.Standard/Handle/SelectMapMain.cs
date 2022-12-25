using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqExtend.Handle
{
    /// <summary>
    /// source数据源类型
    /// </summary>
    internal enum SelectMapSourceType
    {
        IEnumerable = 1,
        IQuerable = 2,
    }

    internal class GetExpressionArgs<TSource, TResult>
            where TSource : class
            where TResult : class, new()
    {

        public GetExpressionArgs(
            Expression<Func<TSource, TResult>> selector,
            Action<string> OnSelectMapLogTo,
            SelectMapSourceType sourceType,
            bool isAutoFill
            )
        {
            this.selector = selector;
            this.OnSelectMapLogTo = OnSelectMapLogTo;
            this.SourceType = sourceType;
            this.IsAutoFill = isAutoFill;
        }

        /// <summary>
        /// 硬编码部分
        /// </summary>
        public Expression<Func<TSource, TResult>> selector { get; set; }

        /// <summary>
        /// 获得SelectMap映射的日志情况
        /// </summary>
        public Action<string> OnSelectMapLogTo { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public SelectMapSourceType SourceType { get; }

        /// <summary>
        /// 自动填充
        /// </summary>
        public bool IsAutoFill { get; }
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


            /*计划支持
               一个类对象/一个匿名对象中的属性类型允许为自定义类, 
                    但是自定义中如果又出现了自定义类, 针对这种情况. 需要修改代码

               属性映射优先级:
               内置类型的属性 > 自定义类的类型属性
               相同组别的属性按顺序逐个处理
            */

            var selector_bindings = GetBindings(args, process);
            if (selector_bindings != null)
            {
                bindings.AddRange(selector_bindings);
            }

            var autoMap_bindings = GetBindings(parameterExp, process);
            if (autoMap_bindings != null)
            {
                bindings.AddRange(autoMap_bindings);
            }

            if (args.IsAutoFill)
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


        //第一部分, 硬编码部分
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
                    process.DealWithBuildInProperty(propertyName);
                }

                bindings.AddRange(memberInitExpression.Bindings);
            }
            else
            {
                throw new NotSupportedException("当前selector的写法暂不支持,请修改程序或提issue");
            }

            return bindings;

        }

        //第二部分,根据名字自动映射(目前只处理内置类型的)
        public static List<MemberBinding> GetBindings<TSource, TResult>(
                ParameterExpression parameterExp,
                SelectMapProcess<TSource, TResult> process
            )
            where TSource : class
            where TResult : class, new()
        {
            var bindings = new List<MemberBinding>();
            //1等公民的处理(目前只处理内置类型的)
            var unmappedBuildInProperty = process.GetUnmappedBuildInProperty();
            foreach (var propertyName in unmappedBuildInProperty)
            {
                process.DealWithBuildInProperty(propertyName);

                var memberAssignment = Expression.Bind(
                    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                );

                bindings.Add(memberAssignment);
            }

            return bindings;
        }

        //第三部分,最后兜底部分的处理(自动映射,二等公民)
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
                var customCollection = process.Source.Custom;

                var customDict = customCollection.ToDictionary(a => a, a => new PropertyGroup(a.PropertyType));

                foreach (var propertyName in unmappedPropertyNameList)
                {
#if DEBUG
                    Console.WriteLine(propertyName);
#endif

                    foreach (var kv in customDict)
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

                            customDict[kv.Key].DealWithBuildInProperty(propertyName);
                            process.DealWithBuildInProperty(propertyName, check: false);

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
