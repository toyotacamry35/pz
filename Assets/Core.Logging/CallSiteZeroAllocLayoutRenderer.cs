using System.Text;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Targets;

namespace Core.Environment.Logging
{
    [LayoutRenderer("callsiteza")]
    [ThreadAgnostic]
    [ThreadSafe]
    [UsedImplicitly]
    public class CallSiteZeroAllocLayoutRenderer : LayoutRenderer
    {
        public const string Name = "callsiteza";
        
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.CallerMemberName != null)
            {
                string methodName = logEvent.CallerMemberName;
                if (string.IsNullOrEmpty(methodName))
                    methodName = "<no method>";

                builder.Append(methodName);

                string fileName = logEvent.CallerFilePath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    int lineNumber = logEvent.CallerLineNumber;
                    AppendFileName(builder, fileName, lineNumber);
                }
            }
        }

        private void AppendFileName(StringBuilder builder, string fileName, int lineNumber)
        {
            builder.Append("(");
            builder.Append(fileName);
            builder.Append(":");
            builder.Append(lineNumber);
            builder.Append(")");
        }
    }
}