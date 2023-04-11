using System;

namespace LinqExtend.EF
{
    /// <summary>
    /// Select的扩展
    /// </summary>
    public static class SelectExtensions
    {

        /// <summary>
        /// 获得SelectMap映射的日志情况
        /// </summary>
        public static Action<string> OnSelectMapLogTo { get; set; }
        /*public static Action<string> OnSelectMapLogTo  
            get
            {
                return SelectMapMain.OnSelectMapLogTo;
            }
            set
            {
                SelectMapMain.OnSelectMapLogTo = value;
            }
        }*/
         
    }
}
