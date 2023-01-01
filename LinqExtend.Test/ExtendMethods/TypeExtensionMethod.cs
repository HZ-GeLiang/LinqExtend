using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqExtend.Test.ExtendMethods
{

    internal static class TypeExtensionMethod
    {
        public static List<T> MakeList<T>(this T data)
        {
            Type type = typeof(List<>).MakeGenericType(data.GetType());
            List<T> list = (List<T>)Activator.CreateInstance(type);
            list.Add(data);
            return list;
        }
    }

}
