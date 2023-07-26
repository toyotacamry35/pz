using NLog;
using NLog.LayoutRenderers;
using System.Text;

namespace SharedCode.Logger
{
    [LayoutRenderer("containername")]
    public class ContainerNameLayoutRenderer : LayoutRenderer
    {
        public static string ContainerName { get; set; } = "";

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(ContainerName);
        }
    }
}
