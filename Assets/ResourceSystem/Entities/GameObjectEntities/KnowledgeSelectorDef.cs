using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;
using SharedCode.Wizardry;

namespace SharedCode.Entities.GameObjectEntities
{
    public class KnowledgeSelectorDef : BaseResource
    {
        public ResourceRef<SpellPredicateDef> Predicate { get; set; }
        public ResourceRef<KnowledgeDef> Knowledge { get; set; }
        public TechPointCountDef[] RewardPoints { get; set; }
    }

    //public static class KnowledgeSelectorExtensions
    //{
    //    public static KnowledgeSelectorDef GetActual(this IEnumerable<KnowledgeSelectorDef> selectors, IEntitiesRepository repository)
    //    {
    //        using (var wrapper = await repository.Get(parentEntity.TypeId, parentEntity.Id))
    //        {
    //            foreach (var selector in selectors)
    //            {

    //            }
    //        }
    //    }
    //}
}
