// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface ICharacterMovementSyncImplementRemoteMethods
    {
        System.Threading.Tasks.Task UpdateMovementImpl(SharedCode.MovementSync.CharacterMovementState state, bool important, long counter);
        System.Threading.Tasks.Task TeleportDoneImpl(long teleportFrameTimestamp, long clientNowTimestamp);
        System.Threading.Tasks.Task NotifyThatClientIsGoneImpl();
    }
}