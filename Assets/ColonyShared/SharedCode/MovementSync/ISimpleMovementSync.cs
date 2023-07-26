using SharedCode.Entities;
using SharedCode.EntitySystem;
using System;

namespace SharedCode.MovementSync
{
    public interface IHasSimpleMovementSync
    {
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        ISimpleMovementSync MovementSync { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ISimpleMovementSync : IDeltaObject, IPositionedObject, IPositionableObject
    {
        [RuntimeData]
        Type GridSyncType { get; set; }
        [RuntimeData]
        SimpleMovementStateEvent OnMovementStateChanged { get; set; }

        // Used to off visibility system. Should be set while init-tion & shouldn't be set farther. (used for POI). 
        [RuntimeData]
        bool VisibilityOff { get; set; }

        #region internal
        [ReplicationLevel(ReplicationLevel.Always)]
        [LockFreeMutableProperty]
        Transform __SyncTransform { get; set; }
        #endregion
    }
   

    public class SimpleMovementStateEvent
    {
        public event Action<Transform> Action;
        public void Invoke(Transform state) => Action?.Invoke(state);
    }
}
