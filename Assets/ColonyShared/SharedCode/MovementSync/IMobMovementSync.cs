using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Player;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.Aspects.Utils;
using SharedCode.AI;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Network;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Wizardry;
using static Assets.Src.RubiconAI.AIWorld;
using SharedCode.Repositories;

namespace SharedCode.MovementSync
{
    public interface IHasMobMovementSync : IHasLogableEntity
    {
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        IMobMovementSync MovementSync { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IMobMovementSync : IDeltaObject, IPositionedObject, IPositionableObject, IDebugPositionLogger
    {       
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Always)]
        IMobMovementStateEvent OnMovementStateChanged { get; }

        #region Internal
        /// <summary>
        /// Unreliable канал для пересылки данных перемещения моба на slave'ы. Только для внутреннего пользования в MovementSyncImpl 
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Always), RemoteMethod(MessageSendOptions.Unreliable)]
        event Func<MobMovementStatePacked, Task> __SyncMovementUnreliable; 
        
        /// <summary>
        /// Это reliable канал для пересылки (важных) данных перемещения персонажа на slave'ы. Только для внутреннего пользования в MovementSyncImpl
        /// Сделано пропертёй, а не event'ом (как unreliable), чтобы при создании реплики этой entity в новом репозитарии, в ней уже сразу был
        /// валидный state, без необходимости его как-то отдельно пересылать
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Always), RemoteMethod(MessageSendOptions.ReliableSequenced)]
        MobMovementStatePacked __SyncMovementReliable { get; set; }
        #endregion

        SpellEffectDef def { get; set; }

        /// <summary>
        /// It has the only use: spell MoveEffect sets it (by calling `SetMovementData`). Pawn subscribed to this property changed. 
        /// .. & calls MoveActionsDoer.OnMoveEffect-Started/-Finished in subscribed func.
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Always)]
        MovementData MovementData { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Always)]
        Task SetMovementData(MovementData data);

        [ReplicationLevel(ReplicationLevel.Always)]
        Guid PathFindingOwnerRepositoryId { get; }
        
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Always)]
        Guid PathFindingOwnerRepositoryIdRuntime { get; }
        
        [ReplicationLevel(ReplicationLevel.Always)]
        Task<bool> SetPathFindingOwnerRepositoryId(Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Always)]
        Task UpdateMovement(MobMovementStatePacked state, long counter, bool important);
        
        [ReplicationLevel(ReplicationLevel.Always)]
        Task StopMovement(SpellId spellId, MoveEffectDef moveEffectDef, bool success);
    }

    public interface IMobMovementStateEvent
    {
        event Action<MobMovementState, long> Action;
    }

    public interface IDebugPositionLogger
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        event Func<bool /*enabledStatus*/, bool /*dump*/, Task> SetDebugMobPositionLoggingEvent;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task InvokeSetDebugMobPositionLoggingEvent(bool enabledStatus, bool dump);
    }

    public class MockLocomotionWorld
    {
        static ConcurrentDictionary<Guid, MockLocomotionWorld> _worlds = new ConcurrentDictionary<Guid, MockLocomotionWorld>();
        public static MockLocomotionWorld GetWorld(Guid guid)
        {
            _worlds.TryGetValue(guid, out var aiworld);
            return aiworld;
        }
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        IEntitiesRepository _repo;
        CancellationTokenSource _source;
        public MockLocomotionWorld(IEntitiesRepository repo)
        {
            _source = new CancellationTokenSource();
            _repo = repo;
            repo.EntityCreated += Repo_EntityCreated;
            repo.NewEntityUploaded += Repo_EntityLoaded;
            repo.EntityLoaded += Repo_EntityLoaded;
            repo.EntityUnloaded += Repo_EntityUnloaded;
            repo.EntityDestroy += Repo_EntityDestroy;
        }
        public void Register(Guid wsGuid)
        {
            _worlds.TryAdd(wsGuid, this);
            Run();
        }
        public void Unregister()
        {
            _worlds.TryRemove(_repo.Id, out var removed);
            removed.Stop();
        }

        private void Stop()
        {
            _source.Cancel();
        }


        int _legionaryEntityTypeId = ReplicaTypeRegistry.GetIdByType(typeof(ILegionaryEntity));
        ConcurrentDictionary<OuterRef<IEntity>, MockLocomotionHost> _mobs = new ConcurrentDictionary<OuterRef<IEntity>, MockLocomotionHost>();

        private Task Repo_EntityCreated(int typeId, Guid entityId) => Repo_EntityCreatedLoadedDo(typeId, entityId);
        private Task Repo_EntityLoaded(int typeId, Guid entityId) => Repo_EntityCreatedLoadedDo(typeId, entityId);
        private Task Repo_EntityCreatedLoadedDo(int typeId, Guid entityId)
        {
            if (typeId == _legionaryEntityTypeId)
                InitLegionary(new OuterRef<IEntity>(entityId, typeId));
            return Task.CompletedTask;
        }

        private Task Repo_EntityDestroy(int typeId, Guid entityId, IEntity entity) => Repo_EntityDestroyUnloadedDo(typeId, entityId);
        private Task Repo_EntityUnloaded(int typeId, Guid entityId, bool arg3, IEntity entity) => Repo_EntityDestroyUnloadedDo(typeId, entityId);
        private Task Repo_EntityDestroyUnloadedDo(int typeId, Guid entityId)
        {
            if (typeId == _legionaryEntityTypeId)
                RemoveLegionary(new OuterRef<IEntity>(entityId, typeId));
            return Task.CompletedTask;
        }

        ConcurrentQueue<OuterRef<IEntity>> _entitiesToRemove = new ConcurrentQueue<OuterRef<IEntity>>();
        ConcurrentQueue<OuterRef<IEntity>> _entitiesToAdd = new ConcurrentQueue<OuterRef<IEntity>>();
        ConcurrentQueue<OuterRef<IEntity>> _dummiesToAdd = new ConcurrentQueue<OuterRef<IEntity>>();
        void InitLegionary(OuterRef<IEntity> oRef)
        {
            _entitiesToAdd.Enqueue(oRef);
        }
        void InitDummy(OuterRef<IEntity> oRef)
        {
            _dummiesToAdd.Enqueue(oRef);
        }


        void RemoveLegionary(OuterRef<IEntity> oRef)
        {
            var type = oRef.Type;
            if (type == typeof(ILegionaryEntity) || typeof(IIsDummyLegionary).IsAssignableFrom(type))
            {
                _entitiesToRemove.Enqueue(oRef);
            }
        }
        bool _started = false;
        public void Run()
        {
            var token = _source.Token;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        while (_entitiesToAdd.TryDequeue(out var oRef))
                        {

                            using (var ew = await _repo.Get<ILegionaryEntityAlways>(oRef.Guid))
                            {
                                var entity = ew.Get<ILegionaryEntityAlways>(oRef.TypeId, oRef.Guid, ReplicationLevel.Always);
                                var host = new MockLocomotionHost(this, _repo, oRef, entity.WorldSpaced.OwnWorldSpace.Guid);
                                _mobs.TryAdd(oRef, host);

                            }
                        }

                        while (_entitiesToRemove.TryDequeue(out var eToR))
                        {
                            if (_mobs.TryRemove(eToR, out var removedMob))
                                await removedMob.Stop();
                        }

                        foreach (var mob in _mobs)
                        {
                            await mob.Value.UpdateOnce();
                        }

                        await Task.Delay(100);
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message($"{e.ToString()}").Write();
                    _source.Cancel();
                    _source.Dispose();
                    _source = new CancellationTokenSource();
                    Run();
                    throw;
                }
            }, _repo);
        }




    }

    // Is historical heir of `SpatialLegionary`
    public class MockLocomotionHost
    {
        private new static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MockLocomotionWorld _world;
        MockLocomotingMob _locoMob;
        public MockLocomotionHost(MockLocomotionWorld world, IEntitiesRepository repo, OuterRef<IEntity> ent, Guid worldSpaceGuid)
        {
            this._world = world;
            Repo = repo;
            Ent = ent;
            WorldSpaceGuid = worldSpaceGuid;
            _locoMob = new MockLocomotingMob(Ent, Repo, WorldSpaceGuid);
        }

        public IEntitiesRepository Repo { get; }
        public OuterRef<IEntity> Ent { get; }
        public Guid WorldSpaceGuid { get; }
        public bool ReallyClose;

        public event Action OnFrameStart;
        public event Action OnFrameEnd;
        public ActionType ActionType;
        bool _started = false;
        long _lastSampleTime;
        long _lastUpdateTime;
        bool _isClose = false;
        public async Task UpdateOnce()
        {
            try
            {
                if (!_started)
                {
                    await _locoMob.Start();
                    _started = true;
                }
                else
                {

                    var deltaTime = SyncTime.NowUnsynced - _lastUpdateTime;
                    var deltaTimeSample = SyncTime.NowUnsynced - _lastSampleTime;

                    if (deltaTimeSample > 1)
                    {
                        _isClose = VisibilityGrid.Get(WorldSpaceGuid, Repo).SampleDataForAnyAround<WorldCharacterDef>(Ent, 30, true);
                        _lastSampleTime = SyncTime.NowUnsynced;
                    }
                    if (_isClose && deltaTime > 30)
                    {

                        _lastUpdateTime = SyncTime.NowUnsynced;
                        await _locoMob.Update();
                    }
                    else if (deltaTime > 300)
                    {
                        _lastUpdateTime = SyncTime.NowUnsynced;
                        await _locoMob.Update();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Exception during mob update {e.ToString()}").Write();
                throw;
            }


        }

        internal Task Stop()
        {
            return _locoMob.Stop();
        }
    }
    public class MockLocomotingMob
    {
        private readonly OuterRef<IEntity> _mob;
        private readonly IEntitiesRepository _repo;
        private readonly Guid _worldSpace;

        public MockLocomotingMob(OuterRef<IEntity> mob, IEntitiesRepository repo, Guid worldSpace)
        {
            this._mob = mob;
            this._repo = repo;
            this._worldSpace = worldSpace;
        }
        public async Task Start()
        {
            using (var entW = await _repo.Get(_mob))
            {
                var mobMovement = entW.Get<IHasMobMovementSyncAlways>(_mob, ReplicationLevel.Always).MovementSync;
                await MoveDataChanged(mobMovement.MovementData);
                mobMovement.SubscribePropertyChanged(nameof(mobMovement.MovementData), OnMoveDataChanged);
            }
        }

        private async Task OnMoveDataChanged(EntityEventArgs args)
        {
            var newMoveData = (MovementData)args.NewValue;
            await MoveDataChanged(newMoveData);
        }

        public async Task Stop()
        {
            using (var entW = await _repo.Get(_mob))
            {
                var mobMovement = entW.Get<IHasMobMovementSyncAlways>(_mob, ReplicationLevel.Always).MovementSync;
                await MoveDataChanged(mobMovement.MovementData);
                mobMovement.UnsubscribePropertyChanged(nameof(mobMovement.MovementData), OnMoveDataChanged);
            }
        }
        private async Task MoveDataChanged(MovementData newMoveData)
        {
            _speed = 0;
            if (newMoveData.Start)
            {

                var spellPred = new SpellPredCastData(
                       castData: newMoveData.CastData,
                       currentTime: 0,
                       wizard: default,
                       caster: _mob,
                       slaveMark: null,
                       canceled: false,
                       modifiers: null, 
                       repo: _repo);


                var result = await OnMoveEffectStarted(
                   newMoveData.SpellId,
                   newMoveData.MoveEffectDef,
                   newMoveData.CastData.Def,
                   spellPred);

                if (!result)
                {
                    using (var ent = await _repo.Get(_mob))
                    {
                        await ent.Get<IHasMobMovementSyncAlways>(_mob, ReplicationLevel.Always).MovementSync
                                    .StopMovement(newMoveData.SpellId, newMoveData.MoveEffectDef, false);
                    }

                }
            }
            else
            {
                await OnMoveEffectFinished(
                    newMoveData.SpellId,
                    newMoveData.MoveEffectDef, false);
            }
        }

        private async Task OnMoveEffectFinished(SpellId spellId, MoveEffectDef moveEffectDef, bool success)
        {
            using (var ent = await _repo.Get(_mob))
            {
                await ent.Get<IHasMobMovementSyncAlways>(_mob, ReplicationLevel.Always).MovementSync
                            .StopMovement(spellId, moveEffectDef, success);
            }

        }

        public async ValueTask Update()
        {
            if (_lastUpdateTime == 0)
                _lastUpdateTime = SyncTime.NowUnsynced;
            if (_targetPos == default || _speed == default)
                return;
            if (_target != default)
            {
                var grid = VisibilityGrid.Get(_worldSpace, _repo);
                var gridData = grid.GetGridData(_target);
                if (gridData.Pos == default)
                    return;
                _targetPos = gridData.Pos;
            }
            using (var wrapper = await _repo.Get(_mob))
            {
                var movementSync = wrapper.Get<IHasMobMovementSyncAlways>(_mob, ReplicationLevel.Always)?.MovementSync;
                if (movementSync == null)
                    return;
                var t = movementSync.Transform;
                var deltaToTarget = _targetPos - t.Position;
                if (_acceptedRange > deltaToTarget.magnitude)
                {
                    await movementSync.StopMovement(_id, _def, true);
                    return;
                }
                var velocity = deltaToTarget.Normalized * _speed;
                var packed = MobMovementStatePacked.Pack(new MobMovementState(t.Position + velocity * SyncTime.ToSeconds(SyncTime.NowUnsynced - _lastUpdateTime), velocity, 0, 0, ColonyShared.SharedCode.Aspects.Locomotion.LocomotionFlags.Moving), SyncTime.Now);
                _lastUpdateTime = SyncTime.NowUnsynced;
                await movementSync.UpdateMovement(packed, packed.Counter, _counter++ % 10 == 0);
            }
        }
        long _counter = 0;
        long _lastUpdateTime;
        float _speed = 0;
        float _speedBase = 3;
        Vector3 _targetPos;
        float _acceptedRange;
        SpellId _id;
        MoveEffectDef _def;
        OuterRef<IEntity> _target;
        private async Task<bool> OnMoveEffectStarted(SpellId spellId, MoveEffectDef moveEffectDef, SpellDef spell, SpellPredCastData spellCast)
        {
            switch (moveEffectDef.MoveType)
            {
                case MoveEffectDef.EffectType.FollowPathToPosition:
                    {
                        var targetPos = moveEffectDef.Vec3.Target.GetVector(spellCast);
                        if (_targetPos == default)
                            return false;
                        _target = default;
                        _targetPos = targetPos;
                        _acceptedRange = moveEffectDef.AcceptedRange;
                        _speed = _speedBase;
                    }
                    break;
                case MoveEffectDef.EffectType.FollowPathToTarget:
                    {
                        var grid = VisibilityGrid.Get(_worldSpace, _repo);
                        _target = await moveEffectDef.Target.Target.GetOuterRef(spellCast, _repo);
                        var gridData = grid.GetGridData(_target);
                        if (gridData.Pos == default)
                            return false;
                        _targetPos = gridData.Pos;
                        _acceptedRange = moveEffectDef.AcceptedRange;
                        _speed = _speedBase;
                    }
                    break;
            }
            _id = spellId;
            _def = moveEffectDef;
            return true;
        }
    }
}