using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public class ItemsCounterDef : TargetedQuestCounterDef<BaseItemResource>, IItemResourceSource
    {
        public SourceType SourceType { get; set; } = SourceType.Player;
        public BaseItemResource Item => Target;
        public bool Less { get; set; } = false;
        public ResourceRef<BaseItemResource> Source { get; set; }
        public bool HaveSource => Source != null && Source.IsValid;
    }

    [System.Flags]
    public enum SourceType : byte
    {
        None = 0,
        PlayerInventory = 1 << 0,
        PlayerDoll = 1 << 1,
        ItemInventory = 1 << 2,
        Player = PlayerInventory | PlayerDoll,
        ItemEverywhere = PlayerInventory | PlayerDoll | ItemInventory,
        PerksTemporary = 16,
        PerksPermanent = 32,
        PerksSaved = 64,
        PerksEverywhere = PerksTemporary | PerksPermanent | PerksSaved
    }
    
    public static class SourceTypeHasFlag
    {
        public static bool CheckFlag(this SourceType value, SourceType mask) => (value & mask) != 0;
    }
}
