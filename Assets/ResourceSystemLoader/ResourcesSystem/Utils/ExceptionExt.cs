using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ResourceSystemLoader.ResourcesSystem.Utils
{
    public static class ExceptionExt
    {
        public static string MessageWithInnerExceptions(this Exception e)
        {
            if (e.InnerException == null)
            {
                return e.Message;
            }
            else
            {
                var message = e.Message;
                if (message.Contains("Exception has been thrown by the target of an invocation"))
                    return e.InnerException.MessageWithInnerExceptions();
                else 
                    return e.InnerException.MessageWithInnerExceptions() + "\n <- " + message;
            }
        }
    }
}
