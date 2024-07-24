namespace LinqExtend.ExtensionMethod
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}