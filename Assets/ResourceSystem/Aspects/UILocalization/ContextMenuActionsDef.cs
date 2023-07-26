using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class ContextMenuActionsDef : BaseResource
    {
        public LocalizedString Drop { get; set; }
        public LocalizedString DropAll { get; set; }
        public LocalizedString Destroy { get; set; }
        public LocalizedString Repair { get; set; }
        public LocalizedString Break { get; set; }
        public LocalizedString Split { get; set; }
        public LocalizedString Equip { get; set; }
        public LocalizedString Unequip { get; set; }
        public LocalizedString Consume { get; set; }
        public LocalizedString Build { get; set; }
        public LocalizedString Take { get; set; }
    }
}