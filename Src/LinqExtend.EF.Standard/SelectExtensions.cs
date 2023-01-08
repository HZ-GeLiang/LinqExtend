using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

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
