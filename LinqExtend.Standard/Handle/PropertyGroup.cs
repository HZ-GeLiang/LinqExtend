
#if IEnumerableSource
using LinqExtend.Config;
#elif IQuerableSource
using LinqExtend.EF.Config;
#else
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace LinqExtend.Handle
{
    ///<inheritdoc />
    internal class PropertyGroup<T> : PropertyGroup where T : class
    {
        public PropertyGroup() : base(typeof(T))
        {
        }
    }

    /// <summary>
    /// 属性分组解析
    /// </summary>
    internal class PropertyGroup
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

        /// <summary>
        /// is内置类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsBuildInType(Type type)
        {

            if (type.IsEnum)
            {
                return true;
            }
            if (_buildInType.Contains(type))
            {
                return true;
            }
            return false;
        }

        public PropertyGroup(Type type)
        {
            BuildIn = new List<PropertyInfo>();
            Custom = new List<PropertyInfo>();
            All = type.GetProperties();
            foreach (var item in All)
            {
                if (IsBuildInType(item.PropertyType))
                {
                    BuildIn.Add(item);
                }
                else
                {
                    Custom.Add(item);
                }
            }

            BuildInPropertyProcessList = BuildIn.ToDictionary(a => a.Name, a => false, StringComparer.OrdinalIgnoreCase);

            CustomPropertyProcessList = Custom.ToDictionary(a => a.Name, a => false, StringComparer.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 内置类型的属性处理列表
        /// </summary>
        public Dictionary<string, bool> BuildInPropertyProcessList { get; }

        /// <summary>
        /// 自定义类型的属性处理列表
        /// </summary>
        public Dictionary<string, bool> CustomPropertyProcessList { get; }

        /// <summary>
        /// 内置类型
        /// </summary>
        public List<PropertyInfo> BuildIn { get; }

        /// <summary>
        /// 自定义类型
        /// </summary>
        public List<PropertyInfo> Custom { get; }

        /// <summary>
        /// 所有类型
        /// </summary>
        public PropertyInfo[] All { get; }

        /// <inheritdoc cref="DealPropertyWithBuildIn(string, bool)"/>
        public void DealWithBuildInProperty(string propertyName)
        {
            DealPropertyWithBuildIn(propertyName, true);
        }

        /// <summary>
        /// 添加处理过的内置类型的属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="check"></param>
        public void DealPropertyWithBuildIn(string propertyName, bool check)
        {
#if IEnumerableSource
            var checkSensitiveField = check && LinqExtendConfig.EnabledSensitiveField;
#elif IQuerableSource
            var checkSensitiveField = check && LinqExtendEFConfig.EnabledSensitiveField;
#else
            throw new Exception("未知的DefineConstants");
#endif
            if (checkSensitiveField)
            {
                if (SensitiveField.Default.Contains(propertyName))
                {
                    //场景: 比例Password 等字段, 一般是不需要接口返回的, 此时需要注意
                    var msg = $@"Warning:{propertyName}可能是敏感字段,敏感字段一般是不需要通过接口返回的";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    //Debug.WriteLine(msg);
                    Console.ResetColor();
                }
            }

            if (BuildInPropertyProcessList.ContainsKey(propertyName))
            {
                BuildInPropertyProcessList[propertyName] = true;
            }
        }


        /// <summary>
        /// 添加处理过的自定义类型的属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="check"></param>
        public void DealPropertyWithCustom(string propertyName, bool check)
        {
#if IEnumerableSource
            var checkSensitiveField = check && LinqExtendConfig.EnabledSensitiveField;
#elif IQuerableSource
            var checkSensitiveField = check && LinqExtendEFConfig.EnabledSensitiveField;
#else
            throw new Exception("未知的DefineConstants");
#endif
            if (checkSensitiveField)
            {
                if (SensitiveField.Default.Contains(propertyName))
                {
                    //场景: 比例Password 等字段, 一般是不需要接口返回的, 此时需要注意
                    var msg = $@"Warning:{propertyName}可能是敏感字段,敏感字段一般是不需要通过接口返回的";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    //Debug.WriteLine(msg);
                    Console.ResetColor();
                }
            }

            if (CustomPropertyProcessList.ContainsKey(propertyName))
            {
                CustomPropertyProcessList[propertyName] = true;
            }
        }

        public bool IsEmpty()
        {
            if (!BuildIn.Any() && !Custom.Any())
            {
                return true;
            }
            return false;
        }

        /// <inheritdoc cref="GetPropertyNameWithBuildIn(int)"/>
        public IEnumerable<string> GetBuildInPropertyName()
        {
            return GetPropertyNameWithBuildIn(rank: 1);
        }

        /// <summary>
        /// 获得内置的属性
        /// </summary>
        /// <param name="rank">从1开始</param>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNameWithBuildIn(int rank)
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

        /// <summary>
        /// 获得自定义类型的属性
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPropertyNameWithCustom()
        {
            var list = Custom.Select(a => a.Name);
            return list;
        }
    }
}
