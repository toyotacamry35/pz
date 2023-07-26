using System.Text;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace Core.Environment.Logging
{
    [LayoutRenderer(LayoutRendererId)]
    [ThreadAgnostic]
    [UsedImplicitly]
    public class EntityIdLayoutRenderer : LayoutRenderer
    {
        public const string LayoutRendererId = "entityid";
        public const string EntityIdKey = "EntityId";
        
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            object entityId;
            if(logEvent.Properties.TryGetValue(EntityIdKey, out entityId) && entityId != null)
                builder.Append(entityId);
        }
    }
}