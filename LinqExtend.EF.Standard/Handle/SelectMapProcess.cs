using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExtend.EF.Handle
{
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


        public void DealWithBuildInProperty(string propertyName)
        {
            DealWithBuildInProperty(propertyName, true);
        }

        public void DealWithBuildInProperty(string propertyName, bool check)
        {
            Result.DealWithBuildInProperty(propertyName, check);
        }

        /// <summary>
        /// 未映射过的内置类型的属性
        /// </summary>
        /// <returns></returns>
        public List<string> GetUnmappedBuildInProperty()
        {
            var list = new List<string>();
            var PropertyList = GetUnmappedProperty();

            //一个差集的 foreach , 获得 BuildInCommon 中独有的
            var BuildInCommon = GetBuildInCommonPropertyName(rank: 1);
#if DEBUG
            var debugView = BuildInCommon.ToList();
#endif
            foreach (var item in BuildInCommon)
            {
                if (!Result.BuildInPropertyProcessList.ContainsKey(item))
                {
                    continue;
                }

                var isProcess = Result.BuildInPropertyProcessList[item]; // 不区分大小写的
                if (isProcess)
                {
                    continue;
                }

                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 未映射的属性
        /// </summary>
        public List<string> GetUnmappedProperty()
        {
            var list = new List<string>();
            foreach (var item in Result.BuildInPropertyProcessList)
            {
                if (item.Value == false)
                {
                    list.Add(item.Key);
                }
            }
            return list;
        }

        /// <summary>
        /// 相同的内置属性名
        /// </summary>
        /// <param name="rank">{rank}等公民</param>
        /// <returns></returns>
        private IEnumerable<string> GetBuildInCommonPropertyName(int rank)
        {
            var source = Source.GetBuildInPropertyName(rank);
            var result = Result.GetBuildInPropertyName(rank);
            var common = source.Intersect(result); //TSource 和 TResult 的相同属性
            return common;
        }

        //private bool IsInignoreCase(string thisValue, IEnumerable<string> values)
        //{
        //    return values != null &&
        //           values.Any(value =>
        //           {
        //               return string.Compare(
        //                            thisValue, value, StringComparison.OrdinalIgnoreCase
        //                        ) == 0;
        //           });
        //}

    }

}
