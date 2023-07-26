using System;
using Newtonsoft.Json;

namespace JSONTools.JsonSerialization
{
    public static class JsonSerializerExtension
    {
        public static JsonSerializerSettings GetLoopProtectedSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Error = (sender, ev) =>
                {
                    switch (ev.ErrorContext.Error?.InnerException)
                    {
                        case NotSupportedException _:
                        case ArgumentNullException _:
                            ev.ErrorContext.Handled = true;
                            break;
                    }
                }
            };
        }
    }
}