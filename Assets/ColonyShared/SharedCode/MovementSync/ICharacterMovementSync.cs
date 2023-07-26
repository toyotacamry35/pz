using System;
using System.Threading.Tasks;
using SharedCode.Aspects.Utils;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.MovementSync
{
    public interface IHasCharacterMovementSync : IHasLogableEntity
    {
        [LockFreeMutableProperty]
        [ReplicationLevel(ReplicationLevel.Always)]
        ICharacterMovementSync MovementSync { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ICharacterMovementSync : IDeltaObject, IPositionedObject, IPositionableObject
    {
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Always)]
        CharacterMovementStateFrame MovementState { get; }

        /// <summary>
        /// Вызывается при получении нового фрейма перемещения. 
        /// </summary>
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.Always)] 
        ICharacterMovementStateEvent OnMovementStateChanged { get; }

        /// <summary>
        /// Вызывается при получении корректирующего фрейма перемещения. 
        /// </summary>
        [RuntimeData(SkipField = true), ReplicationLevel(ReplicationLevel.ClientFull)] 
        ICharacterMovementStateEvent OnMovementStateReclaimed { get; }
        
        #region Internal
        /// <summary>
        /// Unreliable канал для пересылки данных перемещения персонажа на slave'ы. Только для внутреннего пользования в MovementSyncImpl 
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Always), RemoteMethod(MessageSendOptions.Unreliable)] 
        event Func<CharacterMovementStateFrame, Task> __SyncMovementStateUnreliable; 
        
        /// <summary>
        /// Это reliable канал для пересылки (важных) данных перемещения персонажа на slave'ы. Только для внутреннего пользования в MovementSyncImpl
        /// Сделано пропертёй, а не event'ом (как unreliable), чтобы при создании реплики этой entity в новом репозитарии, в ней уже сразу был
        /// валидный state, без необходимости его как-то отдельно пересылать
        /// </summary>
        [ReplicationLevel(ReplicationLevel.Always), RemoteMethod(MessageSendOptions.ReliableSequenced)] 
        CharacterMovementStateFrame __SyncMovementStateReliable { get; set; }

        /// <summary>
        /// Это канал для корректировки перемещения персонажа с сервера на authority client. Только для внутреннего пользования в MovementSyncImpl
        /// </summary>
        [ReplicationLevel(ReplicationLevel.ClientFull), RemoteMethod(MessageSendOptions.ReliableSequenced)]
        event Func<CharacterMovementStateFrame, Task> __SyncMovementStateReclaim;
        #endregion

        [ReplicationLevel(ReplicationLevel.Always), RemoteMethod(MessageSendOptions.ReliableSequenced)]
        Task UpdateMovement(CharacterMovementState state, bool important, long counter);

        [ReplicationLevel(ReplicationLevel.ClientFull), RemoteMethod(MessageSendOptions.ReliableSequenced)]
        Task TeleportDone(long teleportFrameTimestamp, long clientNowTimestamp);
        
        [ReplicationLevel(ReplicationLevel.Server)]
        Task NotifyThatClientIsGone();
    }

    public interface ICharacterMovementStateEvent
    {
        event Action<CharacterMovementState,long> Action;
    }
}