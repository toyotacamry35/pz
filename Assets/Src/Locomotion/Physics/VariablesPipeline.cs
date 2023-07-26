using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public abstract class VariablesPipeline
    {
        protected LocomotionVariables _OriginalVariables;
		protected LocomotionTimestamp Timestamp;
        protected float DeltaTime;
        protected LocomotionFlags LocomotionFlags;
        protected LocomotionVector Position;
        protected LocomotionVector ExtraPosition;
        protected LocomotionVector Velocity;
        protected LocomotionVector ExtraVelocity;
        protected LocomotionVector Impulse;
        protected float Orientation;
        protected float AngularVelocity;
        protected float Gravity;
        protected float Friction;
        protected LocomotionVector VelocityClampMinData;
        protected LocomotionVector VelocityClampMaxData;
        protected SnapToGroundStruct SnapToGroundData;
        protected StepUpStruct StepUpData;
        protected readonly List<LocomotionPhysics.Accel> Accel = new List<LocomotionPhysics.Accel>();

        protected struct SnapToGroundStruct
        {
            public bool Snap;
            public float DistanceToGround;
            public float SlopeFactor;
            public float MaxDistance;
        }

        protected struct StepUpStruct
        {
            public float Shift;
            public float Limit;
        }
        
        protected void Clear()
        {
            DeltaTime = 0;
            Timestamp = LocomotionTimestamp.None;
            LocomotionFlags = 0;
            Position = LocomotionVector.Zero;
            ExtraPosition = LocomotionVector.Zero;
            Velocity = LocomotionVector.Zero;
            ExtraVelocity = LocomotionVector.Zero;
            Impulse = LocomotionVector.Zero;
            Gravity = 0;
            Friction = 0;
            Orientation = 0;
            AngularVelocity = 0;
            VelocityClampMinData = new LocomotionVector(float.MinValue, float.MinValue, float.MinValue);
            VelocityClampMaxData = new LocomotionVector(float.MaxValue, float.MaxValue, float.MaxValue);
            SnapToGroundData = new SnapToGroundStruct();
            StepUpData = new StepUpStruct();
            StepUpData.Limit = float.MaxValue;
            Accel.Clear();
        }

        //public ref readonly LocomotionVariables OriginalVariables => ref _OriginalVariables;
        public LocomotionVector OriginalPosition => _OriginalVariables.Position;

        public VariablesPipeline ApplyFlags(LocomotionFlags flags)
        {
            LocomotionFlags |= flags;
            return this;
        }
        
        public VariablesPipeline ApplyAccel(LocomotionPhysics.Accel accel)
        {
            Accel.Add(accel);
            return this;
        }

        public VariablesPipeline ApplyFriction(float friction)
        {
            Friction = friction;
            return this;
        }

        public VariablesPipeline ApplyGravity(float gravity)
        {
            Gravity = gravity;
            return this;
        }

        public VariablesPipeline ApplyGravityOnGround(float gravity, bool hasContact)
        {
            if (!hasContact)
                Gravity = gravity;
            return this;
        }

        public VariablesPipeline ApplyImpulse(LocomotionVector impulse)
        {
            Impulse += impulse;
            return this;
        }
        
        public VariablesPipeline ApplyVerticalImpulse(float impulse)
        {
            Impulse = new LocomotionVector(Impulse.Horizontal, Impulse.Vertical + impulse);
            return this;
        }

        public VariablesPipeline ApplyHorizontalImpulse(Vector2 impulse)
        {
            Impulse = new LocomotionVector(Impulse.Horizontal + impulse, Impulse.Vertical);
            return this;
        }

        public VariablesPipeline SetVelocity(LocomotionVector velocity)
        {
            Velocity = velocity;
            return this;
        }

        public VariablesPipeline SetVerticalVelocity(float velocity)
        {
            Velocity = new LocomotionVector(Velocity.Horizontal, velocity);
            return this;
        }

        public VariablesPipeline SetHorizontalVelocity(Vector2 velocity)
        {
            Velocity = new LocomotionVector(velocity, Velocity.Vertical);
            return this;
        }

        public VariablesPipeline ApplyStepUp(float shift, float limit)
        {
            StepUpData.Shift = Mathf.Max(StepUpData.Shift, shift);
            StepUpData.Limit = Mathf.Min(StepUpData.Limit, limit);
            return this;
        }

        public VariablesPipeline ApplyOrientation(Vector2 direction, float maxAngularVelocity)
        {
            if (!direction.ApproximatelyZero())
            {
                var targetOrientation = Mathf.Atan2(direction.y, direction.x);
                var angularVelocity = SharedHelpers.DeltaAngleRad(Orientation, targetOrientation) / DeltaTime;
                AngularVelocity += SharedHelpers.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);
            }
            return this;
        }

        public VariablesPipeline ApplyOrientation(Vector2 direction)
        {
            if (!direction.ApproximatelyZero())
                ApplyOrientation(Mathf.Atan2(direction.y, direction.x));
            return this;
        }

        public VariablesPipeline ApplyOrientation(float orientation)
        {
            Orientation = orientation;
            return this;
        }

        public VariablesPipeline ApplyOrientationDelta(float yawDelta)
        {
            AngularVelocity += yawDelta / DeltaTime;
            return this;
        }

        public VariablesPipeline ApplyAngularVelocity(float angularVelocity)
        {
            AngularVelocity += angularVelocity;
            return this;
        }

        public VariablesPipeline ApplyTeleport(LocomotionVector targetPoint)
        {
            ApplyFlags(LocomotionFlags.Teleport);
            ExtraPosition += targetPoint - Position;
            return this;
        }
        
        public VariablesPipeline SnapToGround(float distanceToGround, float slopeFactor, float maxDistance)
        {
            SnapToGroundData.Snap = true;
            SnapToGroundData.DistanceToGround = distanceToGround;
            SnapToGroundData.MaxDistance = maxDistance;
            SnapToGroundData.SlopeFactor = slopeFactor;
            return this;
        }

        public VariablesPipeline VelocityClampMin(float x = float.MinValue, float y = float.MinValue, float vertical = float.MinValue)
        {
            VelocityClampMinData = new LocomotionVector(
                Mathf.Max(VelocityClampMinData.Horizontal.x, x),
                Mathf.Max(VelocityClampMinData.Horizontal.y, y),
                Mathf.Max(VelocityClampMinData.Vertical, vertical)
            );
            return this;
        }

        public VariablesPipeline VelocityClampMax(float x = float.MaxValue, float y = float.MaxValue, float vertical = float.MaxValue)
        {
            VelocityClampMinData = new LocomotionVector(
                Mathf.Min(VelocityClampMaxData.Horizontal.x, x),
                Mathf.Min(VelocityClampMaxData.Horizontal.y, y),
                Mathf.Min(VelocityClampMaxData.Vertical, vertical)
            );
            return this;
        }

        public Vector2 Dbg_GetVeloByAccel(LocomotionVariables vars)
        {
            return LocomotionPhysics.ApplyAccel(vars.Velocity.Horizontal, Accel, DeltaTime);
        }
    }
}
