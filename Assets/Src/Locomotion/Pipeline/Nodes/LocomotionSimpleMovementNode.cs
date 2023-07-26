using ColonyShared.SharedCode.Aspects.Locomotion;

namespace Src.Locomotion
{
    public class LocomotionSimpleMovementNode : ILocomotionPipelinePassNode
    {
        bool ILocomotionPipelinePassNode.IsReady => true;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            var velocity = vars.Velocity;
            var position = vars.Position;

            if (!vars.Flags.Any(LocomotionFlags.Teleport))
            {
                var deltaPosition = (velocity + vars.ExtraVelocity) * dt + vars.ExtraPosition;
                position += deltaPosition;
            }

            return new LocomotionVariables(vars) { Position = position, Velocity = velocity, ExtraVelocity = LocomotionVector.Zero, ExtraPosition = LocomotionVector.Zero };
        }
    }
}