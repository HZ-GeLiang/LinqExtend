#if IEnumerableSource
using LinqExtend;
using LinqExtend.Config;
using LinqExtend.ExtensionMethod;
#elif IQuerableSource
using LinqExtend.EF.Config;
using LinqExtend.EF.ExtendMethods;
#else
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace LinqExtend.Handle
{
    /// <summary>
    /// 属性处理信息
    /// </summary>
    internal class PropertyProcessInfo
    {
        public PropertyProcessInfo(string Name, PropertyInfo PropertyInfo)
        {
            this.Name = Name;
            this.PropertyInfo = PropertyInfo;
            IsProcess = false;
            PropertyGroup = null;
        }

        /// <summary>
        /// 属性的名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 是否处理过了
        /// </summary>
        public bool IsProcess { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyGroup PropertyGroup { get; private set; }

        public void SetIsProcess()
        {
            IsProcess = true;
        }

        public void SetPropertyGroup(Type customType)
        {
            PropertyGroup = new PropertyGroup(customType);
        }
    }

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

        public Type Type { get; private set; }
        public PropertyGroup(Type type)
        {
            Type = type;
            BuildIn = new List<PropertyInfo>();
            Custom = new List<PropertyInfo>();
            BuildInPropertyProcessList = new Dictionary<string, PropertyProcessInfo>(StringComparer.OrdinalIgnoreCase);
            CustomPropertyProcessList = new Dictionary<string, PropertyProcessInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo item in type.GetProperties())
            {
                //这个if 不能添加
                //if (item.CanWrite == false)
                //{
                //    continue;
                //}

                if (IsBuildInType(item.PropertyType))
                {
                    BuildIn.Add(item);
                    AddToPropertyProcessList(BuildInPropertyProcessList, item);
                }
                else
                {
                    Custom.Add(item);
                    var propertyProcessInfo = AddToPropertyProcessList(CustomPropertyProcessList, item);
                    propertyProcessInfo.SetPropertyGroup(item.PropertyType);
                }
            }
        }

        private PropertyProcessInfo AddToPropertyProcessList(
                Dictionary<string, PropertyProcessInfo> dict,
                PropertyInfo property)
        {
            var propertyProcessInfo = new PropertyProcessInfo(property.Name, property);
            try
            {
                dict.Add(property.Name, propertyProcessInfo);
            }
            catch (ArgumentException ex)
            {
                throw new Exception($@"框架约束: 类中的属性不允许出现重名的(不区分大小写).类:{GetClassName()},属性:{property.Name}", ex);
            }
            return propertyProcessInfo;
        }


        /// <summary>
        /// 获得类名
        /// </summary>
        /// <returns></returns>
        private string GetClassName()
        {
            var type = Type;
            if (Type.IsGenericType() && Type.GenericTypeArguments.Length == 1)
            {
                type = Type.GenericTypeArguments[0];
            }
            var className = type.FullName;
            return className;
        }

        /// <summary>
        /// 内置类型的属性处理列表
        /// </summary>
        public Dictionary<string, PropertyProcessInfo> BuildInPropertyProcessList { get; }

        /// <summary>
        /// 自定义类型的属性处理列表
        /// </summary>
        public Dictionary<string, PropertyProcessInfo> CustomPropertyProcessList { get; }

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


        /// <summary>
        /// is内置属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsBuildIn(string propertyName)
        {
            if (BuildInPropertyProcessList.ContainsKey(propertyName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// is自定义属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsCustom(string propertyName)
        {
            if (CustomPropertyProcessList.ContainsKey(propertyName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理属性
        /// </summary>
        /// <param name="propertyName"></param>
        public void DealPropertyWithAuto(string propertyName)
        {
            if (IsBuildIn(propertyName))
            {
                DealPropertyWithBuildIn(propertyName);
            }
            else
            {
                DealPropertyWithCustom(propertyName);
            }
        }


        /// <inheritdoc cref="DealPropertyWithBuildIn(string, bool)"/>
        public void DealPropertyWithBuildIn(string propertyName)
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
                BuildInPropertyProcessList[propertyName].SetIsProcess();
            }
        }

        /// <inheritdoc cref="DealPropertyWithCustom(string, bool)"/>
        public void DealPropertyWithCustom(string propertyName)
        {
            DealPropertyWithCustom(propertyName, true);
        }

        public void DealPropertyWithCustom(string CustomPropertyName, string buildInPropertyName)
        {
            if (CustomPropertyProcessList.ContainsKey(CustomPropertyName))
            {
                //CustomPropertyProcessList[CustomPropertyName].IsProcess = true;
                var PropertyGroup = CustomPropertyProcessList[CustomPropertyName].PropertyGroup;
                if (PropertyGroup.BuildInPropertyProcessList.ContainsKey(buildInPropertyName))
                {
                    PropertyGroup.BuildInPropertyProcessList[buildInPropertyName].SetIsProcess();
                }
            }
        }


        /// <summary>
        /// 从自定义类中获得属性信息
        /// </summary>
        /// <param name="CustomPropertyName"></param>
        /// <param name="buildInPropertyName"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfoWithCustom(string CustomPropertyName, string buildInPropertyName)
        {
            if (CustomPropertyProcessList.ContainsKey(CustomPropertyName))
            {
                var PropertyGroup = CustomPropertyProcessList[CustomPropertyName].PropertyGroup;
                if (PropertyGroup.BuildInPropertyProcessList.ContainsKey(buildInPropertyName))
                {
                    return PropertyGroup.BuildInPropertyProcessList[buildInPropertyName].PropertyInfo;
                }
            }
            return null;
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
                CustomPropertyProcessList[propertyName].SetIsProcess();
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
