using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class StatResourcesDef : BaseResource
    {
        public ResourceRef<StatResource> HealthCurrentStat      { get; set; }
        public ResourceRef<StatResource> HealthMaxStat          { get; set; }
        public ResourceRef<StatResource> HealthMinStat          { get; set; }
        public ResourceRef<StatResource> HealthMaxAbsoluteStat  { get; set; }
        public ResourceRef<StatResource> HealthCurrentRegenStat { get; set; }
    }

}
