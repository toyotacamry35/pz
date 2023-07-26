using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Character.Events
{
    public class VisualEffectAttributesDef
    {
        public List<ResourceRef<VisualEffectHandlerCasterTargetDef>> Target { get; set; } = new List<ResourceRef<VisualEffectHandlerCasterTargetDef>>();
        public List<ResourceRef<VisualEffectHandlerCasterTargetDef>> Caster { get; set; } = new List<ResourceRef<VisualEffectHandlerCasterTargetDef>>();
        public VisualEffectRadiusTargetDef AllAround { get; set; }
    }

    public class VisualEffectRadiusTargetDef
    {
        float Range;
        public VisualEffectHandlerCasterTargetDef Action;
    }
}
