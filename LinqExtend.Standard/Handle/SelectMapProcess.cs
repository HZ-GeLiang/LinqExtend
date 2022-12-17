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
        }

        public PropertyGroup<TSource> Source { get; set; }
        public PropertyGroup<TResult> Result { get; set; }


        /// <summary>
        /// 已经处理过的内置类型的属性
        /// </summary>
        public SortedSet<string> BuildInPropertyProcessList { get; } = new SortedSet<string>();

        /// <summary>
        /// 添加处理过的内置类型的属性
        /// </summary>
        /// <param name="propertyName"></param>
        public void DealWithBuildInProperty(string propertyName)
        {
            if (SensitiveField.Default.Contains(propertyName))
            {
                //场景: 比例Password 等字段, 一般是不需要接口返回的, 此时需要注意
                var msg = $@"Warning:{propertyName}可能是敏感字段,敏感字段一般是不需要通过接口返回的";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg);
                Console.ResetColor();
            }

            BuildInPropertyProcessList.Add(propertyName);
        }

        /// <summary>
        /// 未映射过的内置类型的属性
        /// </summary>
        /// <param name="rank">{rank}等公民</param>
        /// <returns></returns>
        public List<string> GetUnmappedBuildInProperty(int rank)
        {
            var list = new List<string>();

            //差集, 获得 BuildInCommon 中独有的
            var BuildInCommon = GetBuildInCommonPropertyName(rank);
            foreach (var item in BuildInCommon)
            {
                var isProcess = BuildInPropertyProcessList.Contains(item); // 不区分大小写的
                if (isProcess)
                {
                    continue;
                }
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 相同的内置属性名
        /// </summary>
        /// <param name="rank"></param>
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

    /// <summary>
    /// 敏感的
    /// </summary>
    public class SensitiveField
    {
        /// <summary>
        /// 敏感字符
        /// </summary> 
        public static SortedSet<string> Default = new SortedSet<string>()
        {
            "Password",
            "pwd",
            "salt",
        };//集合中区分大小写
    }


    internal class PropertyGroup
    {
        public PropertyGroup(Type type)
        {
            BuildIn = new List<PropertyInfo>();
            Custom = new List<PropertyInfo>();
            foreach (var item in type.GetProperties())
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

        /// <inheritdoc cref="GetBuildInPropertyName(int)"/>
        public IEnumerable<string> GetBuildInPropertyName()
        {
            return GetBuildInPropertyName(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rank">从1开始</param>
        /// <returns></returns>
        public IEnumerable<string> GetBuildInPropertyName(int rank)
        {
            if (rank <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rank) + "是从1开始的");
            }
            if (rank > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(rank) + "目前最大支持2级");
            }
            if (rank == 1)
            {
                var list = BuildIn.Select(a => a.Name);
                return list;
            }

            if (rank == 2)
            {
                IEnumerable<string> list = new List<string>(Custom.Count);
                foreach (var item in Custom)
                {
                    var collection = new PropertyGroup(item.PropertyType).GetBuildInPropertyName();
                    ((List<string>)list).AddRange(collection);
                }

                list = list.Distinct();
                return list;
            }

            throw new ArgumentOutOfRangeException(nameof(rank) + "当前值无效");
        }
    }

    /// <summary>
    /// 属性分类解析
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PropertyGroup<T> : PropertyGroup where T : class
    {
        public PropertyGroup() : base(typeof(T))
        {
        }
    }

}
