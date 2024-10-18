namespace LinqExtend.Handle;

internal class SelectMapProcess<TSource, TResult>
    where TSource : class
    where TResult : class, new()
{
    public SelectMapProcess()
    {
        Source = new PropertyGroup<TSource>();
        Result = new PropertyGroup<TResult>();
    }

    public PropertyGroup<TSource> Source { get; set; }
    public PropertyGroup<TResult> Result { get; set; }

    /// <summary>
    /// 处理属性
    /// </summary>
    /// <param name="propertyName"></param>
    public void DealPropertyWithAuto(string propertyName)
    {
        //if (Result.IsBuildIn(propertyName))
        //{
        //    DealPropertyWithBuildIn(propertyName);
        //}
        //else
        //{
        //    DealPropertyWithCustom(propertyName);
        //}

        Result.DealPropertyWithAuto(propertyName);
    }

    /// <inheritdoc cref="DealPropertyWithBuildIn(string, bool)"/>
    public void DealPropertyWithBuildIn(string propertyName)
    {
        DealPropertyWithBuildIn(propertyName, true);
    }

    /// <summary>
    /// 处理内置类型的属性
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="check"></param>
    public void DealPropertyWithBuildIn(string propertyName, bool check)
    {
        Result.DealPropertyWithBuildIn(propertyName, check);
    }

    /// <inheritdoc cref="DealPropertyWithCustom(string, bool)"/>
    public void DealPropertyWithCustom(string propertyName)
    {
        DealPropertyWithCustom(propertyName, true);
    }

    /// <summary>
    /// 处理自定义类型的属性
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="check"></param>
    public void DealPropertyWithCustom(string propertyName, bool check)
    {
        Result.DealPropertyWithCustom(propertyName, check);
    }

    /// <summary>
    /// 未映射过的属性(内置类型)
    /// </summary>
    /// <returns></returns>
    public List<string> GetUnmappedPropertyWithBuildIn(int rank)
    {
        var BuildInCommon = GetCommonPropertyNameWithBuildIn(rank);
#if DEBUG
        var debugView = BuildInCommon.ToList();
#endif
        var result = new List<string>();
        foreach (var item in BuildInCommon)//获得差集数据
        {
            if (!Result.BuildInPropertyProcessList.ContainsKey(item))
            {
                continue;
            }

            var isProcess = Result.BuildInPropertyProcessList[item].IsProcess; // 不区分大小写的
            if (isProcess)
            {
                continue;
            }

            result.Add(item);
        }
        return result;
    }

    /// <summary>
    /// 未映射过的属性(自定义类型)
    /// </summary>
    /// <returns></returns>
    public List<string> GetUnmappedPropertyWithCustom()
    {
        var customCommon = GetCommonPropertyNameWithCustom();
#if DEBUG
        var debugView = customCommon.ToList();
#endif
        var result = new List<string>();
        foreach (var item in customCommon)//获得差集数据
        {
            if (!Result.CustomPropertyProcessList.ContainsKey(item))
            {
                continue;
            }

            var isProcess = Result.CustomPropertyProcessList[item].IsProcess; // 不区分大小写的
            if (isProcess)
            {
                continue;
            }

            result.Add(item);
        }
        return result;
    }

    /// <summary>
    /// 相同的内置属性名
    /// </summary>
    /// <param name="rank">{rank}等公民</param>
    /// <returns></returns>
    private IEnumerable<string> GetCommonPropertyNameWithBuildIn(int rank)
    {
        var source = Source.GetPropertyNameWithBuildIn(rank);
        var result = Result.GetPropertyNameWithBuildIn(rank);
        var common = source.Intersect(result); //TSource 和 TResult 的相同属性
        return common;
    }

    /// <summary>
    /// 相同的内置属性名
    /// </summary>
    /// <returns></returns>
    private IEnumerable<string> GetCommonPropertyNameWithCustom()
    {
        var source = Source.GetPropertyNameWithCustom();
        var result = Result.GetPropertyNameWithCustom();
        var common = source.Intersect(result); //TSource 和 TResult 的相同属性
        return common;
    }
}