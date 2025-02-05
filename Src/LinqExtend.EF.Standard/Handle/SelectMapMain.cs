using LinqExtend.EF.ExtensionMethods;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#if IQuerableSource

#endif

namespace LinqExtend.EF.Handle;

internal class GetExpressionArgs<TSource, TResult>
    where TSource : class
    where TResult : class
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
    private GetExpressionArgs()
    {
        throw new Exception("未知的DefineConstants");
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
        where TResult : class
    {
        var parameterExp = args.selector == null
              ? Expression.Parameter(typeof(TSource), "a")
              : args.selector.Parameters[0];  //需要外面丢进来 ,不然会提示 variable '' of type '' referenced from scope '', but it is not defined

        var bindings = SelectMap_GetExpression_GetBindings(args, parameterExp);

        if (args.OnSelectMapLogTo != null)
        {
            string log = GetSelectMapLog(bindings);
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
        where TResult : class
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
        throw new Exception("未知的DefineConstants");
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
        where TResult : class
    {
        var bindings = new List<MemberBinding>();

        if (args.selector == null)
        {
            return bindings;
        }

        var body = args.selector.Body;
        if (body is MemberInitExpression memberInitExpression)
        {
            foreach (var item in memberInitExpression.Bindings)
            {
                var propertyName = item.Member.Name;
                //process.DealPropertyWithBuildIn(propertyName);
                process.DealPropertyWithAuto(propertyName);
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
        where TResult : class
    {
        var bindings = new List<MemberBinding>();
        //1等公民的处理(内置类型)
        var unmappedBuildInProperty = process.GetUnmappedPropertyWithBuildIn(rank: 1);

        foreach (var propertyName in unmappedBuildInProperty)
        {
            try
            {
                process.DealPropertyWithBuildIn(propertyName);
                var memberAssignment = Expression.Bind(
                    typeof(TResult).GetProperty(propertyName),   //  TResult 的 set_UserNickName()
                    Expression.Property(parameterExp, propertyName)// TSource 的 a.UserNickName
                );

                bindings.Add(memberAssignment);
            }
            catch (System.ArgumentException argumentException)
            {
                throw new ArgumentException($"{argumentException.Message}:{propertyName}", argumentException);
            }
            catch (System.Exception ex)
            {
                throw new Exception($"{argumentException.Message}:{propertyName}", ex);
            }

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
        where TResult : class
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
        where TResult : class
    {
        Func<ParameterExpression, SelectMapProcess<TSource, TResult>, List<MemberBinding>> selectorLast = (parameterExp, process) =>
        {
            var bindings = new List<MemberBinding>();
            var dict_PropertyListWithSourceCustom =
                process.Source.Custom
                .ToDictionary(a => a, a => new PropertyGroup(a.PropertyType));

            //最后未处理的部分(内置类型的)
            var unmappedPropertyNameList = process.Result.BuildInPropertyProcessList
                                            .Where(a => a.Value.IsProcess == false)
                                            .Select(a => a.Value)
                                            ;

            foreach (var property in unmappedPropertyNameList)
            {
                var propertyName = property.Name;
                foreach (var kv in dict_PropertyListWithSourceCustom)
                {
                    var objProcess = kv.Value;

                    if (
                        objProcess.BuildInPropertyProcessList.ContainsKey(propertyName) == false ||
                        objProcess.BuildInPropertyProcessList[propertyName].IsProcess == true //objProcess中propertyName是未处理过的
                    )
                    {
                        continue;
                    }

                    PropertyInfo prop = kv.Value.BuildIn.First(a => string.Compare(a.Name, propertyName, StringComparison.OrdinalIgnoreCase) == 0);

                    if (prop != null)
                    {
                        var objType = kv.Key.PropertyType;

                        dict_PropertyListWithSourceCustom[kv.Key].DealPropertyWithBuildIn(propertyName);
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

#if IQuerableSource
            //最后未处理的部分(自定义类型的)
            var unmappedPropertyNameList_Custom = process.Result.CustomPropertyProcessList
                                            .Where(a => a.Value.IsProcess == false)
                                            .Select(a => a.Value)
                                            ;

            foreach (var customProperty in unmappedPropertyNameList_Custom)
            {
                /*
                //NickName = new MultilingualString(a.b.NickName.Chinese)
                //{
                //    English = a.b.NickName.English
                //}
                */

                var propertyName = customProperty.Name; //NickName

                foreach (var kv in dict_PropertyListWithSourceCustom)
                {
                    var customPropertyInfo = kv.Key.PropertyType.GetProperty(propertyName);//NickName
                    if (customPropertyInfo == null)
                    {
                        continue;
                    }

                    MemberExpression exp = Expression.Property(parameterExp, kv.Key);//a.b
                    MemberExpression exp_customPropertyName = Expression.Property(exp, propertyName);//a.b.NickName
                    Type exp_customPropertyType = exp_customPropertyName.Type;

                    var customPropertyType = customProperty.PropertyInfo.PropertyType; //MultilingualString
                    //var customPropertyType2 = customPropertyInfo.PropertyType; //和上面等价
                    var ctors = customPropertyType.GetConstructors();//获得 public的
                    ConstructorInfo MemberInit_new_left = null;
                    List<Expression> MemberInit_new_right = new List<Expression>();

                    if (ctors.Length == 0) // 没有 pulic 的构造器
                    {
                        throw new Exception($@"暂不支持没有构造函数的类. 类:{customPropertyType.FullName}");
                        //  object instance  = FormatterServices.GetUninitializedObject(classType);
                    }
                    else
                    {
                        ConstructorInfo constructor = ctors.First();
                        ParameterInfo[] parameters = constructor.GetParameters();

                        if (parameters.Length == 0)
                        {
                            MemberInit_new_left = customPropertyType.GetConstructor(new Type[] { });
                        }
                        else
                        {
                            var _ConstructorTypes = new List<Type>();

                            //bool allPropertyIsExists = true;// 构造函数中的所有参数名都能按名字匹配到属性名(忽略大小写)
                            //for (int i = 0; i < parameters.Length; i++)
                            //{
                            //    ParameterInfo parameter = parameters[i];
                            //    string parameterName = process.Result.GetPropertyInfoWithCustom(propertyName, parameter.Name)?.Name; // 因为下面是区分大小写的, 所以要根据构造参数参数名去换取真正的属性名

                            //    if (parameterName == null)
                            //    {
                            //        allPropertyIsExists = false;
                            //        break;
                            //    }
                            //}

                            for (int i = 0; i < parameters.Length; i++)
                            {
                                ParameterInfo parameter = parameters[i];
                                _ConstructorTypes.Add(parameter.ParameterType);

                                string parameterName = process.Result.GetPropertyInfoWithCustom(propertyName, parameter.Name)?.Name; // 因为下面是区分大小写的, 所以要根据构造参数参数名去换取真正的属性名

                                if (parameterName != null)
                                {
                                    MemberInit_new_right.Add(
                                        Expression.MakeMemberAccess(
                                            Expression.MakeMemberAccess(exp,
                                                exp.Type.GetProperty(propertyName)
                                            ),
                                            exp_customPropertyType.GetProperty(parameterName) //要注意大小写
                                        )
                                    );
                                    process.Result.DealPropertyWithCustom(propertyName, parameter.Name);
                                }
                                else
                                {
                                    if (parameter.ParameterType.IsClass)
                                    {
                                        var ConstantValue = parameter.ParameterType.GetDefaultValue();
                                        MemberInit_new_right.Add(Expression.Constant(ConstantValue, parameter.ParameterType));
                                    }
                                    else
                                    {
                                        var ConstantValue = parameter.ParameterType.GetDefaultValue();
                                        MemberInit_new_right.Add(Expression.Constant(ConstantValue));
                                    }
                                }
                            }

                            MemberInit_new_left = customPropertyType.GetConstructor(_ConstructorTypes.ToArray());
                        }
                    }

                    NewExpression MemberInit_Left =
                        Expression.New(
                            MemberInit_new_left,
                            MemberInit_new_right
                        );

                    var MemberInit_Right = new List<MemberAssignment>();

                    //遍历当前自定义类中被赋值的属性

                    var list = process.Result.CustomPropertyProcessList[propertyName]
                                            .PropertyGroup.BuildInPropertyProcessList;

                    var exp_customProperty_props = exp_customPropertyType.GetProperties();

                    foreach (var itemProperty in list)
                    {
                        if (itemProperty.Value.IsProcess == true)
                        {
                            continue;
                        }

                        var exp_customProperty = exp_customPropertyType.GetProperty(itemProperty.Value.Name);
                        if (exp_customProperty == null)
                        {
                            //如果 exp_customPropertyName 中没有 itemProperty.Value.Name, 那么需要跳过
                            //也就是实体类的属性在数据源中不存在
                            continue;
                        }

                        var propInit_left = customPropertyType.GetProperty(itemProperty.Value.Name);//"English"
                        var propInit_right =
                            Expression.MakeMemberAccess(
                                exp_customPropertyName,
                                exp_customProperty //"English"
                            );

                        var propInit = Expression.Bind(
                            propInit_left,
                            propInit_right
                        );

                        MemberInit_Right.Add(propInit);
                        process.Result.DealPropertyWithCustom(propertyName, itemProperty.Value.Name);
                    }
                    //添加 binding
                    var bind_right =
                        Expression.MemberInit(
                            MemberInit_Left,
                            MemberInit_Right
                        );

                    MemberAssignment memberAssignment = Expression.Bind(
                       typeof(TResult).GetProperty(propertyName),
                       bind_right
                    );

                    bindings.Add(memberAssignment);
                }
                break;
            }
#endif
            return bindings;
        };
        return selectorLast;
    }

    public static Expression<Func<TSource, TResult>> GetLambda<TSource, TResult>(List<MemberBinding> bindings, ParameterExpression parameterExp)
      where TSource : class
      where TResult : class
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