using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Character.Events
{
    public sealed class TriggerFXRuleDef : BaseResource
    {
        public string AnimatedProp { get; set; }
        public ResourceRef<PredicateDef> Predicate { get; set; } = PredicateDef.True;
        public ResourceRef<FXEventType> EventType { get; set; }
        public VisualEffectAttributesDef OnEvent { get; set; }
    }
}