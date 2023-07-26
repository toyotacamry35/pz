using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.Transactions;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class CorpseInteractiveEntity
    {
        public async Task<ContainerItemOperation> MoveAllItemsImpl(PropertyAddress source, PropertyAddress destination)
        {
            var itemTransaction = new ItemMoveAllManagementTransaction(source, destination, true, false, EntitiesRepository);
            var result = await itemTransaction.ExecuteTransaction();
            return result;
        }
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }
    }
}