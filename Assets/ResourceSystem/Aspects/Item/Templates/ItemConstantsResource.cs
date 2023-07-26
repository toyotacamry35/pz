using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Item.Templates
{
    public class ItemConstantsResource : BaseResource
    {
        public ResourceRef<ActionOnDeathDef> Destroy { get; set; }
        public ResourceRef<ActionOnDeathDef> MoveToCorpse { get; set; }
        public ResourceRef<ActionOnDeathDef> LeaveAtCharacter { get; set; }
    }
}
