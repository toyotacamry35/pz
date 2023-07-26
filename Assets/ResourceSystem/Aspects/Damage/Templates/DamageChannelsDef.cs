using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public struct DamageChannel
    {
        public ResourceRef<DamageTypeDef> DamageType;
        public ResourceRef<StatResource> ResistanceStat;
        public ResourceRef<StatResource> AbsorptionStat;
    }

    public class DamageChannelsDef : BaseResource
    {
        public DamageChannel[] DamageChannels = { };
        public ResourceRef<StatResource> GenericResistance;
        public ResourceRef<StatResource> GenericAbsorption;
    }

    public struct SlotPassiveDamageCoefficient
    {
        public ResourceRef<SlotDef> Slot;
        public float Coefficient;
    }
}
