using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;
using System.Threading.Tasks;
using NLog;
using SharedCode.Serializers;
using SharedCode.Entities.Service;
using SharedCode.Entities.GameObjectEntities;
using System;

namespace GeneratedCode.DeltaObjects
{
    public partial class SimpleMovementSync : IHookOnReplicationLevelChanged, IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("SimpleMovementSync");

        private VisibilityGrid _grid;
        private readonly TrivialPositionHistory _history = new TrivialPositionHistory();
        private object _lock = new object();

        public Transform Transform
        {
            get
            {
                lock (_lock)
                    return __SyncTransform;
            }
        }

        public Transform SetTransform
        {
            set
            {
                //if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_ent.Id, $"SimpleMovementSync Entity:{_ent.Id} Transform: {value}").Write();

                UpdateState(value);
                ((IEntityExt)parentEntity).CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

                lock (_lock)
                    __SyncTransform = value;
                //if (DbgLog.Enabled) DbgLog.Log($"X. SimpleMovementSync.SetTransf(__SyncTransf:=): pos:{value.Position}, rot:{value.Rotation.eulerAngles}");

                OnMovementStateChanged.Invoke(value);
                UpdateGrid(Transform);
            }
        }

        public IPositionHistory History => _history;

        protected override void constructor()
        {
            base.constructor();
            OnMovementStateChanged = new SimpleMovementStateEvent();
        }

        private Task OnSyncTransformMaster(EntityEventArgs args)
        {
            var transform = (Transform)args.NewValue;
            UpdateState(transform);
            UpdateGrid(transform);
            OnMovementStateChanged.Invoke(transform);
            return Task.CompletedTask;
        }


        private Task OnSyncTransformReplica(EntityEventArgs args)
        {
            var transform = (Transform)args.NewValue;
            UpdateState(transform);
            UpdateGrid(transform);
            OnMovementStateChanged.Invoke(transform);            
            return Task.CompletedTask;
        }

        private void UpdateState(Transform transform)
        {
            _history.Update(transform);
        }

        Type _syncGridType;
        private void UpdateGrid(Transform transform)
        {
            if (VisibilityOff)
                return;
            _grid = VisibilityGrid.Get(((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace.Guid, EntitiesRepository);
            if (GridSyncType != null)
            {
                _grid?.SetGridData(transform.Position, new OuterRef<IEntity>(parentEntity), _syncGridType = GridSyncType, ((IEntityObject)parentEntity).Def);
            }
            else
            {
                _grid?.SetGridData(transform.Position, new OuterRef<IEntity>(parentEntity), new SimpleObjectData() { Def = ((IEntityObject)parentEntity).Def });
            }
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedAsync(oldReplicationMask, newReplicationMask), EntitiesRepository);
        }

        private async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask)
        {
            if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Master) || (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master)))
            {
                using (await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {   
                    UnsubscribePropertyChanged(nameof(ISimpleMovementSync.__SyncTransform), OnSyncTransformMaster);
                    SubscribePropertyChanged(nameof(ISimpleMovementSync.__SyncTransform), OnSyncTransformReplica);
                    var transform = __SyncTransform;
                    UpdateState(transform);
                    UpdateGrid(transform);
                    OnMovementStateChanged.Invoke(transform);
                }
            }
            else if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                using (await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    UnsubscribePropertyChanged(nameof(ISimpleMovementSync.__SyncTransform), OnSyncTransformReplica);
                    SubscribePropertyChanged(nameof(ISimpleMovementSync.__SyncTransform), OnSyncTransformMaster);
                    if (!VisibilityOff)
                        _grid = VisibilityGrid.Get(((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace.Guid, EntitiesRepository);
                    var transform = Transform;
                    UpdateGrid(transform);
                    OnMovementStateChanged.Invoke(transform);
                }
            }
            if (newReplicationMask == 0)
            {
                _grid?.ForgetGridData(new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId), _syncGridType ?? typeof(SimpleObjectData));
            }
        }

        public Task OnInit()
        {
            SetTransform = Transform;
            return Task.CompletedTask;

        }

        public Task OnDatabaseLoad()
        {
            UpdateState(Transform);
            SetTransform = Transform;
            return Task.CompletedTask;
        }

        public Vector3 Position => Transform.Position;

        public Quaternion Rotation => Transform.Rotation;

        public Vector3 Scale => Transform.Scale;

        public Vector3 SetPosition
        {
            set { SetTransform = Transform.WithPosition(value); }
        }

        public Quaternion SetRotation
        {
            set { SetTransform = Transform.WithRotation(value); }
        }

        public Vector3 SetScale
        {
            set { SetTransform = Transform.WithScale(value); }
        }

    }
}
