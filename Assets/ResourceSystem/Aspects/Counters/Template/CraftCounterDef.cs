using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Aspects.Impl.Factions.Template;
using System;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public class CraftCounterDef : TargetedQuestCounterDef<CraftRecipeDef>
    {
        public CraftSourceType CraftSource { get; set; } = CraftSourceType.Any;
    }
    [Flags]
    public enum CraftSourceType 
    {
        Player = 1,
        Bench = 2,
        Any = Player | Bench
    }
    public static class CraftSourceTypeHasFlag
    {
        public static bool CheckFlag(this CraftSourceType value, CraftSourceType mask) => (value & mask) != 0;
    }
}
