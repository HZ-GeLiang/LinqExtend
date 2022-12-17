using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public PropertyGroup(Type type)
        {
            BuildIn = new List<PropertyInfo>();
            Custom = new List<PropertyInfo>();
            All = type.GetProperties();
            foreach (var item in All)
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

            BuildInPropertyProcessList = BuildIn.ToDictionary(a => a.Name, a => false, StringComparer.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 属性处理列表(目前只处理内置类型的属性)
        /// </summary>
        public Dictionary<string, bool> BuildInPropertyProcessList { get; }

        /// <summary>
        /// 内置类型
        /// </summary>
        public List<PropertyInfo> BuildIn { get; }

        /// <summary>
        /// 定义类型类型
        /// </summary>
        public List<PropertyInfo> Custom { get; }

        /// <summary>
        /// 所有类型
        /// </summary>
        public PropertyInfo[] All { get; }

        /// <inheritdoc cref="DealWithBuildInProperty(string, bool)"/>
        public void DealWithBuildInProperty(string propertyName)
        {
            DealWithBuildInProperty(propertyName, true);
        }

        /// <summary>
        /// 添加处理过的内置类型的属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="check"></param>
        public void DealWithBuildInProperty(string propertyName, bool check)
        {
            if (check && SensitiveField.Default.Contains(propertyName))
            {
                //场景: 比例Password 等字段, 一般是不需要接口返回的, 此时需要注意
                var msg = $@"Warning:{propertyName}可能是敏感字段,敏感字段一般是不需要通过接口返回的";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg);
                Console.ResetColor();
            }

            if (BuildInPropertyProcessList.ContainsKey(propertyName))
            {
                BuildInPropertyProcessList[propertyName] = true;
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
}
