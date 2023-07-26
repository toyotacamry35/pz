using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class TransitionMutationStageDef : MutationStageDef
    {
        public ResourceRef<MutatingFactionDef> ToFaction { get; set; }
    }
}