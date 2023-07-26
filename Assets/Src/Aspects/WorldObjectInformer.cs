using Assets.Src.Aspects.RegionsScenicObjects;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using UnityEngine;
using Transform = SharedCode.Entities.Transform;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities;
using SharedCode.Serializers;

namespace Assets.Src.Aspects
{
    // Using only by static (not by player character and not by mobs)
    public class WorldObjectInformer : EntityGameObjectComponent
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("WorldObjectInformer");


        Transform _curState;

        void OnNewPosState(Transform state)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("New position of {0} is {1}", this.GetOuterRef<IEntity>(), state.Position).Write();
            lock (this)
                _curState = state;
            UnityQueueHelper.RunInUnityThread(() =>
            {
                lock (this)
                {
                    transform.position = _curState.Position.ToXYZ();
                    transform.rotation = (Quaternion)_curState.Rotation;
                }
            });
        }


        protected override void GotServer()
        {
            AsyncUtils.RunAsyncTask(async () => //#danger: Possibly I should defer it
            {
                using (var wrapper = await ServerRepo.Get(TypeId, EntityId))
                {
                    var movementSync = wrapper.Get<IHasSimpleMovementSyncClientBroadcast>(TypeId, EntityId, ReplicationLevel.ClientBroadcast)?.MovementSync;
                    var posObj = PositionedObjectHelper.GetPositioned(wrapper, TypeId, EntityId);
                    if (movementSync != null)
                    {
                        movementSync.OnMovementStateChanged.Action += OnNewPosState;
                        var defaultState = posObj.Transform;
                        OnNewPosState(defaultState);
                    }
                }
            }, ServerRepo);
        }


    }
}
