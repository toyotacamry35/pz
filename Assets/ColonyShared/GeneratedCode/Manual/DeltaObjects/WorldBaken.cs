using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ColonyShared.SharedCode.Entities;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldBaken : IHookOnInit, IHookOnDatabaseLoad, IHookOnStart, IHookOnReplicationLevelChanged
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private WorldBakenDef _def => (WorldBakenDef)Def;

        public async Task OnInit()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            if (OwnerInformation.Owner.IsValid)
                using (var cnt = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                    if (cnt.TryGet<IHasFaction>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Master, out var hasFaction))
                    {
//                        OwnerInformation.AccessPredicate = hasFaction?.Faction?.RelationshipRules.Target?.ChestAccessPredicate;
                        IncomingDamageMultiplier = hasFaction?.Faction?.RelationshipRules.Target?.BakenIncomingDamageMultiplier;
                    }
        }

        public Task OnDatabaseLoad()
        {
            Health.ZeroHealthEvent += OnZeroHealthEvent;
            return Task.CompletedTask;
        }

        public Task OnStart()
        {
            return WorldBakenRepoSubscriber.SubscribeBakenRepository(EntitiesRepository, this);
        }

        private async Task OnZeroHealthEvent(Guid arg1, int arg2)
        {
            Guid bakenCharRepo = default;
            Guid ownerGuid = default;
            using (var wrapper = await EntitiesRepository.Get<IWorldBaken>(Id))
            {
                var thisObj = wrapper?.Get<IWorldBaken>(Id);
                if (thisObj == null)
                    Logger.IfError()?.Message("Something went wrong: this obj is not found {0}", Id).Write();
                else
                {
                    bakenCharRepo = await WorldBakenRepoSubscriber.GetBakenCharRepoByBaken(EntitiesRepository, thisObj);
                    ownerGuid = OwnerInformation.Owner.Guid;
                    await thisObj.OwnerInformation.SetOwner(new OuterRef<IEntity>());
                }
            }

            if (bakenCharRepo != default && ownerGuid != default)
            {
                using (var wrapper = await EntitiesRepository.Get<IBakenCoordinatorServiceEntityServer>(bakenCharRepo))
                {
                    var coordEntity = wrapper?.Get<IBakenCoordinatorServiceEntityServer>(bakenCharRepo);
                    if (coordEntity == null)
                    {
                        Logger.IfError()?.Message("Cant find baken char repo replica {0}", bakenCharRepo).Write();
                    }
                    else
                    {
                        await coordEntity.BakenIsDestroyed(ownerGuid, new OuterRef<IEntity>(this));
                    }
                }
            }

            _ = AsyncUtils.RunAsyncTask(async () =>
              {
                  await Task.Delay(TimeSpan.FromSeconds(10));
                  await EntitiesRepository.Destroy(TypeId, Id);
              });
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Always))
            {
                AsyncUtils.RunAsyncTask(() => SetBakenCreatedOrLoaded(), EntitiesRepository);
            }

            if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Always))
            {
                AsyncUtils.RunAsyncTask(() => SetBakenUnloadedOrDestroyed(), EntitiesRepository);
            }
        }

        private async Task SetBakenCreatedOrLoaded()
        {
            Guid characterId;
            Guid masterNode;
            using (var wrapper = await EntitiesRepository.Get<IWorldBakenAlways>(this.Id))
            {
                var bakenEntity = wrapper?.Get<IWorldBakenAlways>(this.Id);
                if (bakenEntity == null)
                {
                    Logger.IfError()?.Message("Cant get worldbaken {0} from repository", this.Id).Write();
                    return;
                }

                characterId = bakenEntity.OwnerInformation.Owner.Guid;
                masterNode = bakenEntity.OwnerRepositoryId;
            }

            if (characterId == default)
                return;

            using (var wrapper = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
            {
                var bakenCoordinator = wrapper?.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                if (bakenCoordinator == null)
                    return;
                await bakenCoordinator.RegisterBaken(characterId, masterNode, new OuterRef<IEntity>(this));
            }
        }

        private async Task SetBakenUnloadedOrDestroyed()
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IBakenCoordinatorServiceEntityServer>())
            {
                var bakenCoordinator = wrapper?.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                if (bakenCoordinator == null)
                    return;
                await bakenCoordinator.UnregisterBaken(new OuterRef<IEntity>(this));
            }
        }

        public Task<float> GetVerticalSpawnPointDistanceImpl()
        {
            return Task.FromResult(_def.VerticalDistanceFromSpawnPointToCenter);
        }

        public Task SetCooldownImpl()
        {
            var cooldownTime = _def.Cooldown;
            ReadyTimeUTC0InMilliseconds = GetCurrentTimeUTC0InMilliseconds() + (long)(cooldownTime * 1000.0f);

            return Task.CompletedTask;
        }

        private static long GetCurrentTimeUTC0InMilliseconds()
        {
            return SharedCode.Utils.UnixTimeHelper.ToUnix(DateTime.UtcNow);
        }

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
        
        public ItemSpecificStats SpecificStats => ((WorldBakenDef)Def).DefaultStats;
        
        public ValueTask<CalcerDef<float>> GetIncomingDamageMultiplierImpl() => new ValueTask<CalcerDef<float>>(IncomingDamageMultiplier.GetCalcer());
    }

    public static class WorldBakenRepoSubscriber
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("WorldBakenRepoSubscriber");

        public static async Task<Guid> GetBakenRepositoryId(IEntitiesRepository repo, Guid charId)
        {
            //TODO временно возвращает ид репозитория с логин сервисом by Boris
            using (var wrapper = await repo.GetFirstService<IBakenCoordinatorServiceEntityServer>())
            {
                var loginServiceEntity = wrapper?.GetFirstService<IBakenCoordinatorServiceEntityServer>();
                if (loginServiceEntity == null)
                {
                    Logger.IfError()?.Message("ILoginInternalServiceEntityServer not found").Write();
                    return default;
                }

                var ownRepo = repo.Id;
                var targetRepo = ((IEntityExt)loginServiceEntity.GetBaseDeltaObject()).OwnerNodeId;

                if (ownRepo == targetRepo)
                    return default;

                return targetRepo;
            }
        }

        public static async Task<Guid> GetBakenCharRepoByBaken(IEntitiesRepository repo, IEntity entity)
        {
            var hasOwner = entity as IHasOwner;
            if (hasOwner == null)
                return default;
            var repoId = await GetBakenRepositoryId(repo, hasOwner.OwnerInformation.Owner.Guid);
            return repoId;
        }

        public static async Task SubscribeBakenRepository(IEntitiesRepository repo, IEntity entity)
        {
            var targetRepoId = await GetBakenCharRepoByBaken(repo, entity);
            if(targetRepoId != default)
                await repo.SubscribeReplication(entity.TypeId, entity.Id, targetRepoId, ReplicationLevel.Always);
        }
    }
}