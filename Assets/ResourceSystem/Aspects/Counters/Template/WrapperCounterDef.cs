using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public class WrapperCounterDef : QuestCounterDef
    {
        public ResourceRef<QuestCounterDef> SubCounter { get; set; }
    }
}