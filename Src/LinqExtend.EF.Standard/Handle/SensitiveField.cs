using System.Collections.Generic;

namespace LinqExtend.EF.Handle
{
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
}