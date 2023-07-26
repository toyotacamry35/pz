using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IBruteDef : IHasStatisticsTypeDef
    {
        float ForwardDamageMultiplier  { get; set; }
        float SideDamageMultiplier     { get; set; }
        float BackwardDamageMultiplier { get; set; }
        float DestructionPowerRequired { get; set; }
    }
}
