using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    [Localized]
    public class MutatingFactionDef : FactionDef
    {
        public float MaxMutation { get; set; }
        public float MutationOnDeath { get; set; }
        public ResourceRef<MutationStageDef>[] Stages { get; set; }
    }    
}
