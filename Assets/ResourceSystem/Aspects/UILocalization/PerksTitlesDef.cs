using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class PerksTitlesDef : BaseResource
    {
        public LocalizedString UnlockSlotTitle { get; set; }
        public LocalizedString BestSlotsTypeMessage { get; set; }
        public LocalizedString SlotsOfTypeLimitReachedMessage { get; set; }
        public LocalizedString DestroyPerkDialogTitle { get; set; }
        public LocalizedString DestroyPerkDialogQuestion { get; set; }
        public LocalizedString SlotUnlockDialogQuestion { get; set; }
        public LocalizedString SlotUpgradeDialogQuestion { get; set; }
        public LocalizedString UpgradeSlotTitle { get; set; }
        public LocalizedString EmptySlotTitle { get; set; }
        public LocalizedString Perk { get; set; }
        public LocalizedString Slot { get; set; }
        public LocalizedString SavePerkTitle { get; set; }
        public LocalizedString SavePerkQuestion { get; set; }
    }
}