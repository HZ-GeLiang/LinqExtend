using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExtend.EF.ExtendMethods
{

    internal static class TypeExtensions
    {


        /// <summary>
        /// 获得泛型类型的第一个T的 Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type? GetFirstGenericType(this Type type)
        {
            if (!type.IsGenericType || type.GenericTypeArguments.Length <= 0)
            {
                return null;
            }
            return type.GenericTypeArguments[0];
        }


        public static bool IsGenericType(this Type type)
        {
#if NETCOREAPP
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

    }
}