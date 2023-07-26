using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public class MutationCounterDef : QuestCounterDef, IMutationSource
    {
        public ResourceRef<MutatingFactionDef> Faction { get; set; }
        public ResourceRef<MutationStageDef> Stage { get; set; }

        MutationStageDef IMutationSource.Stage => Stage.Target;
    }
}