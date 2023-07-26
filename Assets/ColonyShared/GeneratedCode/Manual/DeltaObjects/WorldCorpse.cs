using System.Threading.Tasks;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.Transactions;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Logging;
using System.Linq;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.Entities;
using ResourceSystem.Utils;
using Assets.ColonyShared.SharedCode.Aspects.Damage;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldCorpse : IHookOnInit
    {
        public Task OnInit()
        {
            Doll.Size = 100;
            Inventory.Size = 100;

            return Task.CompletedTask;
        }

        public async Task<ContainerItemOperation> MoveAllItemsImpl(PropertyAddress source,
            PropertyAddress destination)
        {
            var itemTransaction =
                new ItemMoveAllManagementTransaction(source, destination, true, false, EntitiesRepository);
            var result = await itemTransaction.ExecuteTransaction();
            return result;
        }

        public Task<bool> DestroyImpl() => Destroyable.Destroy();

        public Task<bool> NameSetImpl(string name)
        {
            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            Prefab = prefab;
            return Task.FromResult(true);
        }

        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

        public async ValueTask<bool> MustBeInstantiatedHereImpl() => true;

        
        public async ValueTask<UnityRef<GameObject>> GetPrefabImpl()
        {
            bool hasAuthority = false;
            if (VisibleOnlyForOwner && OwnerInformation.Owner.IsValid)
                using (var cnt = await EntitiesRepository.Get(OwnerInformation.Owner))
                    if (cnt.TryGet<IHasAuthorityOwnerClientFull>(OwnerInformation.Owner, ReplicationLevel.ClientFull, out var owner))
                        hasAuthority = owner.AuthorityOwner.HasClientAuthority;
            return !VisibleOnlyForOwner || hasAuthority ? Def.Prefab : ((WorldCorpseDef)Def).AnotherPrefab;
        }
    }
}