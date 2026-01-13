using LinqExtend.ExtensionMethods;
using System.Data;
using System.Text;

namespace LinqExtend;

/// <summary>
/// 聚合为一个字符串
/// </summary>
public static class AggregateExtensions
{

    public static string AggregateToString<TKey, TValue>(this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, string> func_content, string separator)
    {
        if (source == null)
        {
            return string.Empty;
        }

        List<string> list = new List<string>(source.Count);
        foreach (var item in source)
        {
            list.Add(func_content.Invoke(item));
        }

        return list.AggregateToString(a => a, separator);
    }


    #region 常见的列表对象调用 AggregateToString, 表现形式一般为 list.AggregateToString(a => a,",")

    public static string AggregateToString<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys, string separator)
    {
        var keyType = typeof(TKey);
        if (keyType.IsClass == false || keyType == typeof(string))
        {
            return keys.ToList().AggregateToString(null, a => a, separator);
        }
        throw new Exception($"不支持的TKey类型:{keyType}");
    }

    public static string AggregateToString<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys, Func<TKey, bool> predicate, string separator)
    {
        var keyType = typeof(TKey);
        if (keyType.IsClass == false || keyType == typeof(string))
        {
            return keys.ToList().AggregateToString(predicate, a => a, separator);
        }
        throw new Exception($"不支持的TKey类型:{keyType}");
    }

    public static string AggregateToString(this List<string> source, string separator)
    {
        return source.Where(a => string.IsNullOrEmpty(a) == false).AggregateToString(a => a, separator);
    }

    public static string AggregateToString(this List<string> source, Func<string, bool> predicate, string separator)
    {
        return source.Where(a => string.IsNullOrEmpty(a) == false && predicate.Invoke(a)).AggregateToString(a => a, separator);
    }

    public static string AggregateToString(this IOrderedEnumerable<string> source, string separator)
    {
        return source.Where(a => string.IsNullOrEmpty(a) == false).AggregateToString(a => a, separator);
    }

    public static string AggregateToString(this IOrderedEnumerable<string> source, Func<string, bool> predicate, string separator)
    {
        return source.Where(a => string.IsNullOrEmpty(a) == false && predicate.Invoke(a)).AggregateToString(a => a, separator);
    }

    public static string AggregateToString(this List<int> source, string separator)
    {
        return source.AggregateToString(null, a => a, separator);
    }

    public static string AggregateToString(this List<int> source, Func<int, bool> predicate, string separator)
    {
        return source.AggregateToString(predicate, a => a, separator);
    }

    public static string AggregateToString(this List<int?> source, string separator)
    {
        return source.AggregateToString(a => a.HasValue, a => a, separator);
    }

    public static string AggregateToString(this List<int?> source, Func<int?, bool> predicate, string separator)
    {
        return source.AggregateToString(a => a.HasValue && predicate.Invoke(a), a => a, separator);
    }

    #endregion

    #region AggregateToString , 聚合为一个字符串

    public static string AggregateToString(this DataRowCollection source, string columnName, string separator)
    {
        return AggregateToString(source, null, columnName, separator);
    }

    /// <summary>
    /// 聚合为一个字符串
    /// </summary>
    /// <param name="source"></param>
    /// <param name="func_predicate"></param>
    /// <param name="columnName"></param>
    /// <param name="separator"></param>
    /// <returns>没有值时返回""</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string AggregateToString(this DataRowCollection source, Func<DataRow, bool> func_predicate, string columnName, string separator)
    {
        if (source is null)
        {
            return string.Empty;
            //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
        }
        if (columnName.HasValue() == false)
        {
            throw new ArgumentNullException($"{nameof(columnName)}参数不能为空", nameof(columnName));
        }
        if (separator is null)
        {
            separator = "";
            //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
        }
        if (source.Count <= 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        if (separator.HasValue())
        {
            if (func_predicate == null)
            {
                foreach (DataRow item in source)
                {
                    sb.Append(item[columnName]).Append(separator);
                }
            }
            else
            {
                foreach (DataRow item in source)
                {
                    if (func_predicate.Invoke(item))
                    {
                        sb.Append(item[columnName]).Append(separator);
                    }
                }
            }
        }
        else
        {
            if (func_predicate == null)
            {
                foreach (DataRow item in source)
                {
                    sb.Append(item[columnName]);
                }
            }
            else
            {
                foreach (DataRow item in source)
                {
                    if (func_predicate.Invoke(item))
                    {
                        sb.Append(item[columnName]);
                    }
                }
            }
        }

        if (separator.HasValue())
        {
            sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
        }
        var txt = sb.ToString();
        return txt;
    }

    public static string AggregateToString(this DataRowCollection source, Func<DataRow, dynamic> func_predicate, string separator)
    {
        return AggregateToString(source, null, func_predicate, separator);
    }

    public static string AggregateToString(this DataRowCollection source, Func<DataRow, bool> func_predicate, Func<DataRow, dynamic> func_content, string separator)
    {
        if (source is null)
        {
            return string.Empty;
            //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
        }
        if (func_content is null)
        {
            throw new ArgumentNullException($"{nameof(func_content)}参数不能为空", nameof(func_content));
        }
        if (separator is null)
        {
            separator = "";
            //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
        }

        if (source.Count <= 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        if (separator.HasValue())
        {
            if (func_predicate == null)
            {
                foreach (DataRow row in source)
                {
                    sb.Append(func_content.Invoke(row)).Append(separator);
                }
            }
            else
            {
                foreach (DataRow row in source)
                {
                    if (func_predicate.Invoke(row) == true)
                    {
                        sb.Append(func_content.Invoke(row)).Append(separator);
                    }
                }
            }
        }
        else
        {
            if (func_predicate == null)
            {
                foreach (DataRow row in source)
                {
                    sb.Append(func_content.Invoke(row));
                }
            }
            else
            {
                foreach (DataRow row in source)
                {
                    if (func_predicate.Invoke(row) == true)
                    {
                        sb.Append(func_content.Invoke(row));
                    }
                }
            }
        }

        if (separator.HasValue())
        {
            sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
        }
        var txt = sb.ToString();
        return txt;
    }

    /// <inheritdoc cref="AggregateToString{TSource}(IEnumerable{TSource}, Func{TSource, bool}, Func{TSource, TSource}, string, int, string, bool)"/>
    public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> func_content, string separator)
    {
        return AggregateToString(source, null, func_content, separator, 0, string.Empty, false);
    }

    /// <inheritdoc cref="AggregateToString{TSource}(IEnumerable{TSource}, Func{TSource, bool}, Func{TSource, TSource}, string, int, string, bool)"/>
    public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> func_content, string separator, bool skipEmptyItem)
    {
        return AggregateToString(source, null, func_content, separator, 0, string.Empty, skipEmptyItem);
    }

    /// <inheritdoc cref="AggregateToString{TSource}(IEnumerable{TSource}, Func{TSource, bool}, Func{TSource, TSource}, string, int, string, bool)"/>
    public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> func_content, string separator, int groupCount, string groupSeparator)
    {
        return AggregateToString(source, null, func_content, separator, groupCount, groupSeparator, false);
    }

    /// <summary>
    /// 聚合为一个字符串
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="func_predicate"></param>
    /// <param name="func_content"></param>
    /// <param name="separator"></param>
    /// <param name="groupCount">每个分组的数量</param>
    /// <param name="groupSeparator">分组的结尾符号</param>
    /// <param name="skipEmptyItem">跳过空值的项,空值: null, 空字符串</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static string AggregateToString<TSource>(this IEnumerable<TSource> source,
        Func<TSource, bool> func_predicate, Func<TSource, TSource> func_content, string separator,
        int groupCount, string groupSeparator,
        bool skipEmptyItem)
    {
        if (source is null)
        {
            return string.Empty;
            //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
        }
        if (func_content is null)
        {
            throw new ArgumentNullException($"{nameof(func_content)}参数不能为空", nameof(func_content));
        }
        if (separator is null)
        {
            separator = "";
            //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
        }

        if (source.Any() == false)
        {
            return string.Empty;
        }

        var isStringCollection = typeof(TSource) == typeof(string);

#if DEBUG
        if (source.GetType() == typeof(string)) // string is IEnumerable<TSource> == true
        {
            // 一般来说这个场景不存在,防止代码无意中写错, 这个方法不支持 string 调用
            throw new Exception("字符串类型的变量不允许调用该方法");
        }
#endif

        var sb = new StringBuilder();
        var separatorhasValue = !string.IsNullOrEmpty(separator);

        if (groupCount > 0 && groupSeparator.HasValue())
        {
            var count = 0;
            var appendGroupSeparator = false;

            if (separatorhasValue)
            {
                if (func_predicate == null)
                {
                    foreach (var item in source)
                    {
                        var content = func_content.Invoke(item);
                        if (skipEmptyItem)
                        {
                            if (content == null || (isStringCollection && object.Equals("", content)))
                            {
                                continue;
                            }
                        }

                        if (appendGroupSeparator)
                        {
                            sb.Append(groupSeparator);
                            appendGroupSeparator = false;
                        }
                        count++;
                        sb.Append(content).Append(separator);
                        if (count > 0 && count % groupCount == 0)
                        {
                            appendGroupSeparator = true;
                        }
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (func_predicate.Invoke(item))
                        {
                            var content = func_content.Invoke(item);
                            if (skipEmptyItem)
                            {
                                if (content == null || (isStringCollection && object.Equals("", content)))
                                {
                                    continue;
                                }
                            }

                            if (appendGroupSeparator)
                            {
                                sb.Append(groupSeparator);
                                appendGroupSeparator = false;
                            }
                            count++;
                            sb.Append(content).Append(separator);
                            if (count > 0 && count % groupCount == 0)
                            {
                                appendGroupSeparator = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (func_predicate == null)
                {
                    foreach (var item in source)
                    {
                        var content = func_content.Invoke(item);
                        if (skipEmptyItem)
                        {
                            if (content == null || (isStringCollection && object.Equals("", content)))
                            {
                                continue;
                            }
                        }
                        if (appendGroupSeparator)
                        {
                            sb.Append(groupSeparator);
                            appendGroupSeparator = false;
                        }
                        count++;
                        sb.Append(content);
                        if (count > 0 && count % groupCount == 0)
                        {
                            appendGroupSeparator = true;
                        }
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (func_predicate.Invoke(item))
                        {
                            var content = func_content.Invoke(item);
                            if (skipEmptyItem)
                            {
                                if (content == null || (isStringCollection && object.Equals("", content)))
                                {
                                    continue;
                                }
                            }

                            if (appendGroupSeparator)
                            {
                                sb.Append(groupSeparator);
                                appendGroupSeparator = false;
                            }
                            count++;

                            sb.Append(content);
                            if (count > 0 && count % groupCount == 0)
                            {
                                appendGroupSeparator = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (separatorhasValue)
            {
                if (func_predicate == null)
                {
                    foreach (var item in source)
                    {
                        var content = func_content.Invoke(item);
                        if (skipEmptyItem)
                        {
                            if (content == null || (isStringCollection && object.Equals("", content)))
                            {
                                continue;
                            }
                        }
                        sb.Append(content).Append(separator);
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (func_predicate.Invoke(item))
                        {
                            var content = func_content.Invoke(item);
                            if (skipEmptyItem)
                            {
                                if (content == null || (isStringCollection && object.Equals("", content)))
                                {
                                    continue;
                                }
                            }
                            sb.Append(content).Append(separator);
                        }
                    }
                }
            }
            else
            {
                if (func_predicate == null)
                {
                    foreach (var item in source)
                    {
                        var content = func_content.Invoke(item);
                        if (skipEmptyItem)
                        {
                            if (content == null || (isStringCollection && object.Equals("", content)))
                            {
                                continue;
                            }
                        }
                        sb.Append(content);
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (func_predicate.Invoke(item))
                        {
                            var content = func_content.Invoke(item);
                            if (skipEmptyItem)
                            {
                                if (content == null || (isStringCollection && object.Equals("", content)))
                                {
                                    continue;
                                }
                            }
                            sb.Append(content);
                        }
                    }
                }
            }
        }

        if (sb.Length <= 0)
        {
            return string.Empty;
        }

        if (separatorhasValue)
        {
            sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
        }

        var txt = sb.ToString();
        return txt;
    }

    public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, dynamic> func_content, string separator)
    {
        return AggregateToString(source, null, func_content, separator);
    }

    /// <summary>
    /// 聚合为一个字符串
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="func_predicate"></param>
    /// <param name="func_content">dynamic的目的: 调用端不用刻意的使用 ToString()</param>
    /// <param name="separator"></param>
    /// <returns>没有值时返回""</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> func_predicate, Func<TSource, dynamic> func_content, string separator)
    {
        if (source is null)
        {
            return string.Empty;
            //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
        }
        if (func_content is null)
        {
            throw new ArgumentNullException($"{nameof(func_content)}参数不能为空", nameof(func_content));
        }
        if (separator is null)
        {
            separator = "";
            //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
        }

        if (source.Any() == false)
        {
            return string.Empty;
        }

#if DEBUG
        if (source.GetType() == typeof(string)) // string is IEnumerable<TSource> == true
        {
            // 一般来说这个场景不存在,防止代码无意中写错, 这个方法不支持 string 调用
            throw new Exception("字符串类型的变量不允许调用该方法");
        }
#endif

        var sb = new StringBuilder();
        var separatorhasValue = !string.IsNullOrEmpty(separator);

        if (separatorhasValue)
        {
            if (func_predicate == null)
            {
                foreach (var item in source)
                {
                    sb.Append(func_content.Invoke(item)).Append(separator);
                }
            }
            else
            {
                foreach (var item in source)
                {
                    if (func_predicate.Invoke(item))
                    {
                        sb.Append(func_content.Invoke(item)).Append(separator);
                    }
                }
            }
        }
        else
        {
            if (func_predicate == null)
            {
                foreach (var item in source)
                {
                    sb.Append(func_content.Invoke(item));
                }
            }
            else
            {
                foreach (var item in source)
                {
                    if (func_predicate.Invoke(item))
                    {
                        sb.Append(func_content.Invoke(item));
                    }
                }
            }
        }

        if (sb.Length <= 0)
        {
            return string.Empty;
        }

        if (separatorhasValue)
        {
            sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
        }
        var txt = sb.ToString();
        return txt;
    }

    #endregion
}