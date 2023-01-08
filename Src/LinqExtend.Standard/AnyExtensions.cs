using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtend
{
    public static class AnyExtensions
    {
        public static bool Any(this ICollection source) => source != null && source.Count > 0;
        public static bool Any(this StringBuilder source) => source != null && source.Length > 0;
        public static bool Any(this string source) => source != null && source.Length > 0;

    }
}
