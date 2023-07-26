using System;

namespace Src.Locomotion
{
    /// <summary>
    /// Mob Locomotion State machine callbacks holder. Is used as a proxy to provide a feedback channel from Locomotion back to MoveAction.
    /// </summary>
    public class MobLocomotionReactions
    {
        public event Action Jumped;
        public event Action Landed;

        public void JumpReaction() => Jumped?.Invoke();

        public void LandReaction() => Landed?.Invoke();
    }
}
