using JetBrains.Annotations;

namespace Core.Environment.Logging
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;
    using NLog;
    using NLog.Config;
    using NLog.Internal;
    using NLog.LayoutRenderers;

    /// <summary>
    /// Stack trace renderer.
    /// </summary>
    [LayoutRenderer("stacktracedetailed")]
    [ThreadAgnostic]
    [UsedImplicitly]
    public class StackTraceLayoutRenderer2 : LayoutRenderer, IUsesStackTrace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackTraceLayoutRenderer2" /> class.
        /// </summary>
        public StackTraceLayoutRenderer2()
        {
            Separator = " => ";
            TopFrames = 3;
        }

        /// <summary>
        /// Gets or sets the number of top stack frames to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(3)]
        public int TopFrames { get; set; }

        /// <summary>
        /// Gets or sets the number of frames to skip.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(0)]
        public int SkipFrames { get; set; }

        /// <summary>
        /// Gets or sets the stack frame separator string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(" => ")]
        public string Separator { get; set; }

        /// <summary>
        /// Gets the level of stack trace information required by the implementing class.
        /// </summary>
        /// <value></value>
        StackTraceUsage IUsesStackTrace.StackTraceUsage => StackTraceUsage.Max;

        /// <summary>
        /// Renders the call site and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.StackTrace == null)
                return;

            int startingFrame = logEvent.UserStackFrameNumber + TopFrames - 1;
            if (startingFrame >= logEvent.StackTrace.FrameCount)
            {
                startingFrame = logEvent.StackTrace.FrameCount - 1;
            }

            int endingFrame = logEvent.UserStackFrameNumber + SkipFrames;
            AppendFlat(builder, logEvent, startingFrame, endingFrame);
        }

        private void AppendFlat(StringBuilder builder, LogEventInfo logEvent, int startingFrame, int endingFrame)
        {
            for (int i = endingFrame; i <= startingFrame; ++i)
            {
                StackFrame frame = logEvent.StackTrace.GetFrame(i);

                if(i != endingFrame)
                    builder.Append(Separator);

                var method = frame.GetMethod();
                builder.Append(method.DeclaringType.Name);
                builder.Append(".");
                builder.Append(method.Name);
                builder.Append("(");
                builder.Append(frame.GetFileName());
                builder.Append(":");
                builder.Append(frame.GetFileLineNumber());
                builder.Append(")");
            }
        }

    }
}