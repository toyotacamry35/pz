using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace SharedCode.Utils
{
    public static class StackTraceUtils
    {
        public static StackTrace GetStackTrace()
        {
            return ServerCoreRuntimeParameters.CollectStackTraces ? new StackTrace(false) : null;
        }
    }
}
