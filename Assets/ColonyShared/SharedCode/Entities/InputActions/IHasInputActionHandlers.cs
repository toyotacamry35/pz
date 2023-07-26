using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using SharedCode.EntitySystem;

namespace SharedCode.Entities.Engine
{
    public interface IHasInputActionHandlers
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [LockFreeReadonlyProperty]
        IInputActionHandlers InputActionHandlers { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IInputActionHandlers : IDeltaObject
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull)]
        [LockFreeReadonlyProperty]
        IInputActionLayersStack LayersStack { get; }

        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull)] 
        IInputActionBindingsSource BindingsSource { get; }
    }    
   
}