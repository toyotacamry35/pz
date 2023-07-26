using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace Uins
{
    public class CraftSourceVmodel
    {
        public OuterRef<IEntityObject> TargetOuterRef;
        public SpellWordCastData Cast;
        public WorldPersonalMachineDef WorldPersonalMachineDef;
        public IEntitiesRepository Repository;

        public override string ToString()
        {
            return $"outerRef={TargetOuterRef}, def={WorldPersonalMachineDef}, repo={Repository}, cast={Cast}";
        }
    }
}