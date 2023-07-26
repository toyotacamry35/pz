using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Input
{
    public class InputBindingDef : BaseResource
    {
        public ResourceRef<InputSlotDef> Slot { get; set; }
        public ResourceRef<InputSourceDef> Source { get; set; }
        public ResourceRef<InputSourceDef>[] Sources { get; set; }
    }
}
