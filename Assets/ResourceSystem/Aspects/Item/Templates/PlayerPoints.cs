using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;

namespace SharedCode.Aspects.Item.Templates
{
    /// <summary>
    /// Ресурсы игрока стартовые
    /// </summary>
    public class PlayerPoints : BaseResource
    {
        public TechPointCountDef[] TechPointCounts;
        public ResourceRef<CurrencyResource>[] UsedTechPoints;
        public ResourceRef<ScienceDef>[] UsedSciences;
        public ResourceRef<KnowledgeDef>[] StartKnowledges;
        public ResourceRef<TechnologyDef>[] StartTechnologies;
    }
}