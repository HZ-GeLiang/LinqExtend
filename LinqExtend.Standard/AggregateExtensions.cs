using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LinqExtend
{
    public static class AggregateExtensions
    {
        #region private Extension Methods
        private static bool EndWith(this StringBuilder value, string str)
        {
            if (value is null) throw new System.ArgumentNullException(nameof(value));
            if (str is null) throw new System.ArgumentNullException(nameof(str));
            for (int i = str.Length; i > 0; i--)
            {
                if (str[i - 1] != value[value.Length - (str.Length - i + 1)])
                {
                    return false;
                }
            }
            return true;
        }

        private static StringBuilder RemoveLastChar(this StringBuilder value, string str)
        {
            if (value is null) throw new System.ArgumentNullException(nameof(value));
            if (str is null || str.Length <= 0) return value; //不能进行移除操作
            if (str.Length > value.Length) return value; //移除失败,无法移除
            if (!value.EndWith(str)) return value;   //EndWith不匹配,无法移除

            return value.Replace(str, string.Empty, value.Length - str.Length, str.Length);
        }

        #endregion

        public static string AggregateToString(this DataRowCollection source, string columnName, string separator)
        {
            if (source is null) throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException($"{nameof(columnName)}参数不能为空", nameof(columnName));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
            var sb = new StringBuilder();
            foreach (DataRow item in source)
            {
                sb.Append(item[columnName]).Append(separator);
            }
            sb.RemoveLastChar(separator);
            var txt = sb.ToString();
            return txt;
        }

        public static string AggregateToString(this DataRowCollection source, Func<DataRow, dynamic> content, string separator)
        {
            if (source is null) throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            if (content is null) throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));

            var sb = new StringBuilder();
            foreach (DataRow row in source)
            {
                sb.Append(content.Invoke(row)).Append(separator);
            }
            sb.RemoveLastChar(separator);
            var txt = sb.ToString();
            return txt;
        }

        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, string separator)
        {
            if (source is null) throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));

            var sb = new StringBuilder();
            foreach (var item in source)
            {
                sb.Append(item).Append(separator);
            }

            sb.RemoveLastChar(separator);

            var txt = sb.ToString();
            return txt;
        }

        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> content, string separator)
        {
            if (source is null) throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            if (content is null) throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));

            var sb = new StringBuilder();
            foreach (var item in source)
            {
                sb.Append(content.Invoke(item)).Append(separator);
            }

            sb.RemoveLastChar(separator);

            var txt = sb.ToString();
            return txt;
        }

        
        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, dynamic> content, string separator)
        {
            if (source is null) throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            if (content is null) throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));

            var sb = new StringBuilder();
            foreach (var item in source)
            {
                sb.Append(content.Invoke(item)).Append(separator);
            }
            sb.RemoveLastChar(separator);
            var txt = sb.ToString();
            return txt;
        }

    }
}
