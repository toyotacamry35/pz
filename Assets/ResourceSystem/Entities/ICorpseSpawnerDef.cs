using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface ICorpseSpawnerDef
    {
        ResourceRef<IEntityObjectDef> CorpseEntityDef { get; set; }
    }
}
