using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LinqExtend.EF.Helper
{
    internal sealed class DebuggerHelper
    {
        /// <summary>
        /// 只在deubg下生效
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="memberName">调用此方法的方法名</param>
        /// <param name="fileName">调用此方法的文件名</param>
        /// <param name="lineNumber">调用此方法的文件的行号</param>
        [Conditional("DEBUG")]
        public static void Break(Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //Debugger.Break();
        }

        public static (StackFrame frame, System.Reflection.MethodBase method, string methodName, string fileName, int lineNumber)
            GetStackFrame()
        {
#if DEBUG
            StackFrame frame = new StackFrame(1, true);
            System.Reflection.MethodBase method = frame.GetMethod();
            string methodName = method.Name;
            string fileName = frame.GetFileName();
            int lineNumber = frame.GetFileLineNumber();
            return (frame, method, methodName, fileName, lineNumber);
#else
            return (null, null, null, null, null);
#endif
        }
    }
}
