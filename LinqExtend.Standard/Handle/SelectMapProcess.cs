using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LinqExtend;

namespace LinqExtend.Handle
{
    internal class SelectMapProcess<TSource, TResult>
        where TSource : class
        where TResult : class, new()
    {
        public SelectMapProcess()
        {
            Source = new PropertyGroup<TSource>();
            Result = new PropertyGroup<TResult>();
            BuildInCommon = GetBuildInCommonPropertyName();
        }

        public PropertyGroup<TSource> Source { get; set; }
        public PropertyGroup<TResult> Result { get; set; }

        /// <summary>
        /// 相同的内置属性名
        /// </summary>
        public IEnumerable<string> BuildInCommon { get; set; }

        /// <summary>
        /// 内置的公共属性名
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetBuildInCommonPropertyName()
        {
            var source = Source.GetBuildInPropertyName();
            var result = Result.GetBuildInPropertyName();
            var common = source.Intersect(result); //TSource 和 TResult 的相同属性
            return common;
        }

        /// <summary>
        /// 已经处理过的内置类型的属性
        /// </summary>
        public List<string> BuildInPropertyProcessList { get; } = new List<string>();

        /// <summary>
        /// 添加处理过的内置类型的属性
        /// </summary>
        /// <param name="PropertyName"></param>
        public void DealWithBuildInProperty(string PropertyName)
        {
            BuildInPropertyProcessList.Add(PropertyName);
        }

        /// <summary>
        /// 未映射过的内置类型的属性
        /// </summary>
        /// <returns></returns>
        public List<string> GetUnmappedBuildInProperty()
        {
            //差集, 获得 BuildInCommon 中独有的
            var list = new List<string>();
            foreach (var item in BuildInCommon)
            {
                var isProcess = IsInignoreCase(item, BuildInPropertyProcessList);
                if (isProcess)
                {
                    continue;
                }
                list.Add(item);

            }
            return list;
        }

        private bool IsInignoreCase(string thisValue, IEnumerable<string> values)
        {
            return values != null &&
                   values.Any(value =>
                   {
                       return string.Compare(
                                    thisValue, value, StringComparison.OrdinalIgnoreCase
                                ) == 0;
                   });
        }

    }

    internal class PropertyGroup<T> where T : class
    {
        private static HashSet<Type> _buildInType = new HashSet<Type>()  {

            typeof(bool),
            typeof(bool?),
            typeof(char),
            typeof(char?),  
            
            //11种数字
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),

            typeof(sbyte?),
            typeof(byte?),
            typeof(short?),
            typeof(ushort?),
            typeof(int?),
            typeof(uint?),
            typeof(long?),
            typeof(ulong?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?),

            typeof(string),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(Guid),
            typeof(Guid?),
        };

        public PropertyGroup()
        {
            BuildIn = new List<PropertyInfo>();
            Custom = new List<PropertyInfo>();
            foreach (var item in typeof(T).GetProperties())
            {
                if (_buildInType.Contains(item.PropertyType))
                {
                    BuildIn.Add(item);
                }
                else
                {
                    Custom.Add(item);
                }
            }
        }

        /// <summary>
        /// 内置类型
        /// </summary>
        public List<PropertyInfo> BuildIn { get; }

        /// <summary>
        /// 定义类型类型
        /// </summary>
        public List<PropertyInfo> Custom { get; }

        public bool IsEmpty()
        {
            if (!BuildIn.Any() && !Custom.Any())
            {
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetBuildInPropertyName()
        {
            var list = BuildIn.Select(a => a.Name);
            return list;
        }
    }

}
