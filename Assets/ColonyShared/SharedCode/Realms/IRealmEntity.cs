using System;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratorAnnotations;
using NLog;
using SharedCode.Aspects.Sessions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.MapSystem;
using SharedCode.Serializers;

namespace SharedCode.Entities
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IRealmsCollectionEntity : IEntity
    {
        IDeltaDictionary<Guid, RealmRulesDef> Realms { get; set; }
        Task<bool> AddRealm(Guid mapId, RealmRulesDef realmDef);
        Task<bool> RemoveRealm(Guid mapId);

    }

    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IRealmEntity : IEntity
    {
        [LockFreeReadonlyProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        RealmRulesDef Def { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        long StartTime { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        bool Active { get; set; }

        [RuntimeData(SkipField = true)]
        bool Dead { get; }
        [RuntimeData(SkipField = true)]
        bool AllowsToJoin { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        IDeltaDictionary<Guid, bool> AttachedAccounts { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> TryAttach(Guid account);

        [ReplicationLevel(ReplicationLevel.Server)]
        //IAccountEntity
        Task<bool> Enter(OuterRef<IEntity> account);

        [ReplicationLevel(ReplicationLevel.Server)]
        //IAccountEntity
        Task<bool> Leave(OuterRef<IEntity> account);

        IDeltaDictionary<Guid, MapMeta> Maps { get; set; }
        Task<bool> AddMap(Guid mapId, MapMeta mapMeta);
        Task<bool> RemoveMap(Guid mapId);

        Task<bool> SetActive(bool active);
        Task<bool> SetMapDead(Guid mapId);
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmsCollectionEntity
    {

        public async Task<bool> AddRealmImpl(Guid mapId, RealmRulesDef realmDef)
        {
            Realms.Add(mapId, realmDef);
            return true;
        }
        public async Task<bool> RemoveRealmImpl(Guid mapId)
        {
            Realms.Remove(mapId);
            return true;
        }
    }
    public partial class RealmEntity : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public bool Dead => IsRealmDead(Def, StartTime);

        public static bool IsRealmDead(RealmRulesDef realmRulesDef, long startTime)
        {
            return GetAliveLeftMs(realmRulesDef, startTime) <= 0;
        }

        public static long GetAliveLeftMs(RealmRulesDef realmRulesDef, long startTime)
        {
            if (realmRulesDef.ExistenceHours != -1)
                return startTime + SyncTime.FromHours(realmRulesDef.ExistenceHours) - SyncTime.Now;
            return long.MaxValue;
        }

        public bool AllowsToJoin => IsRealmAllowsToJoin(Def, StartTime);

        public static bool IsRealmAllowsToJoin(RealmRulesDef realmRulesDef, long startTime)
        {
            return realmRulesDef.CanJoinHours == -1 || SyncTime.FromHours(realmRulesDef.CanJoinHours) > SyncTime.Now - startTime;
        }
        public Task OnInit()
        {
            Active = true;
            StartTime = SyncTime.Now;
            return Task.CompletedTask;
        }
        
        public async Task<bool> SetActiveImpl(bool active)
        {
            using (await this.GetThisExclusive())
            {
                if (active)
                {
                    StartTime = SyncTime.Now;
                }
                else
                {
                    var left = GetAliveLeftMs(Def, StartTime);
                    StartTime -= left;
                }
            }

            return true;
        }

        public async Task<bool> EnterImpl(OuterRef<IEntity> account)
        {
            using (var accW = await EntitiesRepository.Get<IAccountEntityServer>(account.Guid))
            {
                var acc = accW.Get<IAccountEntityServer>(account.Guid);
                var uid = await acc.GetCurrentUserId();
                var mapDef = acc.Characters.First().MapId == default ?
                     (Def.FirstTimeMap != null && !(await acc.CalcAccLevel() > 1) ?
                     Def.FirstTimeMap :
                     Def.DefaultMap) : null;
                using (var ent = await EntitiesRepository.GetFirstService<ILoginServiceEntityServer>())
                {
                    var lg = ent.GetFirstService<ILoginServiceEntityServer>();
                    await lg.RequestUpdateAccountData(account.Guid);
                }
                using (var ent = await EntitiesRepository.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>())
                {
                    var coord = ent.GetFirstService<IWorldCoordinatorNodeServiceEntityServer>();
                    await coord.RequestLoginToMap(new SharedCode.Entities.Service.MapLoginMeta()
                    {
                        CurrentRealmId = Id,
                        TargetRealmGuid = Id,
                        UserId = uid,
                        TargetMap = mapDef,
                        TargetMapId = acc.Characters.First().MapId,
                        CurrentMapId = acc.Characters.First().MapId,
                        RealmRules = Def
                    });
                }
            }
            return true;
        }

        public Task<bool> TryAttachImpl(Guid account)
        {
            if (!AttachedAccounts.ContainsKey(account))
                AttachedAccounts.Add(account, true);
            return Task.FromResult(true);
        }

        public Task<bool> LeaveImpl(OuterRef<IEntity> account)
        {
            AttachedAccounts.Remove(account.Guid);
            return Task.FromResult(true);
        }

        public Task<bool> AddMapImpl(Guid mapId, MapMeta mapMeta)
        {
            if (!Maps.ContainsKey(mapId))
                Maps.Add(mapId, mapMeta);
            return Task.FromResult(true);
        }

        public Task<bool> RemoveMapImpl(Guid mapId)
        {
            Maps.Remove(mapId);
            return Task.FromResult(true);
        }
        public Task<bool> SetMapDeadImpl(Guid mapId)
        {
            if (Maps.TryGetValue(mapId, out var mapMeta))
                Maps[mapId] = new MapMeta() { MapDef = mapMeta.MapDef, IsDead = true };
            return Task.FromResult(true);

        }

        public async Task OnDatabaseLoad()
        {
            //EntitiesRepository.CloudRequirementsMet += EntitiesRepository_CloudRequirementsMet;
            await AsyncUtils.RunAsyncTask(async () => { await EntitiesRepository_CloudRequirementsMet(); });
        }

        private async Task EntitiesRepository_CloudRequirementsMet()
        {
            for(int i = 0; i< 10; i++)
            {

                try
                {

                    using (var w = await EntitiesRepository.GetFirstService<IInGameTimeServiceEntityServer>())
                    {
                        var itime = w.GetFirstService<IInGameTimeServiceEntityServer>();

                        using (var ww = await this.GetThisRead())
                            await itime.SetTimeFromRealm(new DateTime(SyncTime.ToTicks(StartTime)));
                    }
                    return;
                }
                catch(Exception e)
                {
                    Logger.Error(e);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}