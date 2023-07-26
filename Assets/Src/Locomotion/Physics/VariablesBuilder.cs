using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Utils;

namespace Src.Locomotion
{
    internal class VariablesBuilder : VariablesPipeline
    {
        public void Init(LocomotionVariables vars, float deltaTime)
        {
            Clear();
            _OriginalVariables = vars;
			Timestamp = vars.Timestamp;
            LocomotionFlags = vars.Flags;
            Position = vars.Position;
            ExtraPosition = vars.ExtraPosition;
            Velocity = vars.Velocity;
            ExtraVelocity = vars.ExtraVelocity;
            Orientation = vars.Orientation;
            DeltaTime = deltaTime;
        }

        public LocomotionVariables Build()
        {
            bool profileEnabled = LocomotionProfiler.EnableProfile;
            var velocityHorizontal = Velocity.Horizontal;
            var velocityVertical = Velocity.Vertical;

            if (profileEnabled) LocomotionProfiler.BeginSample("## VarsBuilder.Build 1) ApplyAccel");
            velocityHorizontal = LocomotionPhysics.ApplyAccel(velocityHorizontal, Accel, DeltaTime);
            if (profileEnabled) LocomotionProfiler.EndSample();

            if (profileEnabled)  LocomotionProfiler.BeginSample("## VarsBuilder.Build 2) ApplyFriction");
            velocityHorizontal = LocomotionPhysics.ApplyFriction(velocityHorizontal, Accel, Friction, DeltaTime);
            if (profileEnabled) LocomotionProfiler.EndSample();

            velocityHorizontal += Impulse.Horizontal;
            velocityVertical += Impulse.Vertical;
            velocityVertical += Gravity * DeltaTime;
            
            if (profileEnabled)  LocomotionProfiler.BeginSample("## VarsBuilder.Build 3) all other except result struct construction");
            var velocity = new LocomotionVector(
                SharedHelpers.Clamp(velocityHorizontal.x, VelocityClampMinData.Horizontal.x, VelocityClampMaxData.Horizontal.x),
                SharedHelpers.Clamp(velocityHorizontal.y, VelocityClampMinData.Horizontal.y, VelocityClampMaxData.Horizontal.y),
                SharedHelpers.Clamp(velocityVertical, VelocityClampMinData.Vertical, VelocityClampMaxData.Vertical));

            var extraVelocityVertical = ExtraVelocity.Vertical;
            var extraPositionVertical = ExtraPosition.Vertical;

            if (StepUpData.Shift > 0.001f)
            {
                extraPositionVertical += SharedHelpers.Min(StepUpData.Shift, StepUpData.Limit * DeltaTime);
                velocity = new LocomotionVector(velocity.Horizontal, SharedHelpers.Max(velocity.Vertical, 0));
            }

            if (SnapToGroundData.Snap && SnapToGroundData.DistanceToGround <= SnapToGroundData.MaxDistance && DeltaTime > 0)
            {
                var distanceDelta = SnapToGroundData.SlopeFactor * velocity.Horizontal.magnitude * DeltaTime;
                var distance = SnapToGroundData.DistanceToGround - distanceDelta;
                if (distance > 0)
                    extraVelocityVertical = SharedHelpers.Clamp(SharedHelpers.Min(extraVelocityVertical, extraVelocityVertical - distance / DeltaTime), float.MinValue, float.MaxValue);
            }

            var orientation = SharedHelpers.NormalizeAngleRad(Orientation + AngularVelocity * DeltaTime);
            if (profileEnabled) LocomotionProfiler.EndSample();

            return new LocomotionVariables(
                timestamp: Timestamp,
                flags: LocomotionFlags,
                position: Position,
                extraPosition: new LocomotionVector(ExtraPosition.Horizontal, extraPositionVertical),
                velocity: velocity,
                extraVelocity: new LocomotionVector(ExtraVelocity.Horizontal, extraVelocityVertical),
                orientation: orientation,
                angularVelocity: AngularVelocity);
        }
    }
}