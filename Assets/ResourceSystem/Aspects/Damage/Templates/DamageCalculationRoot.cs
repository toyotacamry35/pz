using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    [JsonObject(ItemRequired = Required.Always)]
    public class DamageCalculationRoot : BaseResource
    {
        public ResourceRef<StatResource> ArmorPenetrationStat { get; set; }
        public ResourceRef<StatResource> DamageStat { get; set; }
        public ResourceRef<StatResource> PassiveDamageStat { get; set; }
        public ResourceRef<StatResource> DamageMultiplierStat { get; set; } // damage * DamageMultiplierStat
        public ResourceRef<StatResource> IncomingDamageMultiplierStat { get; set; } // damage * (1 + IncomingDamageMultiplierStat)
        public ResourceRef<StatResource> OutgoingDamageMultiplierStat { get; set; }
        public ResourceRef<StatResource> MiningLootMultiplierStat { get; set; }
        public ResourceRef<StatResource> PowerStat { get; set; }
        public ResourceRef<StatResource> StabilityStat { get; set; }
        public ResourceRef<DamageChannelsDef> IncomingDamageChannels { get; set; } = new DamageChannelsDef();
        public ResourceRef<DamageChannelsDef> PassiveDamageChannels { get; set; } = new DamageChannelsDef();
        public ResourceRef<StatResource> BlockActive { get; set; }
        public SlotPassiveDamageCoefficient[] SlotPassiveDamage { get; set; }
        public float ArmorEfficiency { get; [UsedImplicitly] set; }
        
        public ResourceRef<ArgDef<float>> StaggerDurationArgument { get; set; }
        public ResourceRef<ArgDef<Vector2>> StaggerDirectionArgument { get; set; }
        public ResourceRef<ArgDef<float>> RecoilDurationArgument { get; set; }
        
        public ResourceArray<SlotDef> HitMaterialSlots { get; set; }
        [UsedImplicitly] public ResourceRef<PowerVsStabilityStruct> PowerVsStability;

        [UsedImplicitly] public ResourceRef<PowerVsStabilityStruct> PowerVsStabilityWhenBlocked;

        [UsedImplicitly] 
        public class PowerVsStabilityStruct : BaseResource
        {
            [UsedImplicitly] public ResourceRef<PiecewiseLinearFunctionDef> StaggerTime;
            [UsedImplicitly] public ResourceRef<PiecewiseLinearFunctionDef> RecoilTime;
            [UsedImplicitly] public ResourceRef<PiecewiseLinearFunctionDef> DamageMultiplier;
        }
        
        public static DamageCalculationRoot Instance => _instance.Target;
        
        private static readonly ResourceRef<DamageCalculationRoot> _instance = new ResourceRef<DamageCalculationRoot>("/UtilPrefabs/Stats/DamageRoot"); 
    }
}
