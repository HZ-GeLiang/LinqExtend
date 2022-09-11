using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LinqExtend
{
    public static class SelectExtensions
    {
        public static IEnumerable<TResult> Select<TResult>(this DataColumnCollection dataColumns, Func<DataColumn, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataColumn item in dataColumns)
            {
                list.Add(selector(item));
            }

            return list;
        }

        public static IEnumerable<TResult> Select<TResult>(this DataRowCollection rows, Func<DataRow, TResult> selector)
        {
            if (selector == null)
            {
                return Enumerable.Empty<TResult>();
            }

            var list = new List<TResult>() { };

            foreach (DataRow item in rows)
            {
                list.Add(selector(item));
            }

            return list;
        }

    }
}
