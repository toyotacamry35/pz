using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;
using SharedCode.Entities.GameObjectEntities;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasKnowledgeDef
    {
        ResourceRef<KnowledgeSelectorDef>[] KnowledgeSelectors { get; set; }
    }
}
