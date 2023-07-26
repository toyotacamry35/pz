//#define ENABLE_REACTIVE_STACKTRACE
using System;
using System.Diagnostics;

namespace ReactivePropsNs
{
    public static partial class Tools
    {
        /// <summary> Операция медленная и нудная, их надо устранить в конечном счёте. </summary>
        public static string StackTraceLastString()
        {
            var trace = CreateStackTrace(2);
            if (trace == null) return string.Empty;
            string full = trace.ToString();
            int spaces = 0; for (; spaces < full.Length && full[spaces] == ' '; spaces++) { }
            int index = full.IndexOf("\n");
            return full.Substring(spaces, Math.Max(0, Math.Min(full.IndexOf("\n") - spaces - 1, full.Length - spaces)));
        }
        /// <summary> Операция медленная и нудная, их надо устранить в конечном счёте. </summary>
        public static string StackTraceLastString(StackTrace trace)
        {
            if (trace == null) return string.Empty;
            string full = trace.ToString();
            int spaces = 0; for (; spaces < full.Length && full[spaces] == ' '; spaces++) { }
            return full.Substring(spaces, Math.Max(0, Math.Min(full.IndexOf("\n") - spaces - 1, full.Length - spaces)));
        }
        public static string StackFrame()
        {
            var trace = CreateStackTrace(1, true);
            if (trace == null) return string.Empty;
            var frame = trace.GetFrame(0);
            return $"{frame.GetFileName()}:{frame.GetFileLineNumber()} :{frame.GetFileColumnNumber()} // {frame.GetMethod()}";
        }
        public static string StackFilePosition()
        {
            var trace = CreateStackTrace(1, true);
            if (trace == null) return string.Empty;
            var frame = trace.GetFrame(0);
            return $"{frame.GetFileName()}:{frame.GetFileLineNumber()} [col:{frame.GetFileColumnNumber()}]";
        }
        public static string StackFilePosition(StackTrace trace)
        {
            if (trace == null) return string.Empty;
            var frame = trace.GetFrame(0);
            return $"{frame.GetFileName()}:{frame.GetFileLineNumber()} [col:{frame.GetFileColumnNumber()}]";
        }
                
#if ENABLE_REACTIVE_STACKTRACE
        public static StackTrace CreateStackTrace(int skipFrames, bool needFileInfo)
        {
            return new StackTrace(skipFrames, needFileInfo);
        }
        public static StackTrace CreateStackTrace(int skipFrames)
        {
            return new StackTrace(skipFrames);
        }
#else        
        public static StackTrace CreateStackTrace(int skipFrames, bool needFileInfo)
        {
            return null;
        }
        public static StackTrace CreateStackTrace(int skipFrames)
        {
            return null;
        }
#endif
    }
}
