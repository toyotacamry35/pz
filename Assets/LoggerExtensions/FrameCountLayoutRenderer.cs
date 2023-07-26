using System.Text;
using UnityUpdate;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using UnityEngine;

namespace LoggerExtensions
{
    [LayoutRenderer(LayoutRendererId)]
    [ThreadAgnostic]
    [UsedImplicitly]
    public class FrameCountLayoutRenderer : LayoutRenderer
    {
        public const string LayoutRendererId = "framecount";
        private int _frameCount;

        public FrameCountLayoutRenderer()
        {
            if (Application.isPlaying)
                UnityUpdateDelegate.OnUpdate += () => _frameCount = Time.frameCount;
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(_frameCount);
        }
    }
}