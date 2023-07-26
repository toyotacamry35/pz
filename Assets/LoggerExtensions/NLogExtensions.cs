using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog.Fluent;

namespace NLog.Fluent
{
    public static class NlogExtensions
    {
        [NotNull]
        public static MessageContext UnityObj([NotNull] this MessageContext builder, [CanBeNull] UnityEngine.Object obj)
        {
            if (obj != null)
                builder.Property("UnityObject", obj);
            return builder;
        }
    }
}
