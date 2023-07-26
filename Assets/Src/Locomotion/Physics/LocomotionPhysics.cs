using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Utils;

namespace Src.Locomotion
{
    public static class LocomotionPhysics
    {
        public struct Accel
        {
            public readonly Vector2 Direction;
            public readonly float Acceleration;
            public readonly float TargetSpeed;

            public Accel(Vector2 direction, float acceleration, float targetSpeed)
            {
                Direction = direction;
                Acceleration = acceleration;
                TargetSpeed = targetSpeed;
            }
        }

        public static Vector2 ApplyAccel(Vector2 velocity, Vector2 dir, float accel, float targetSpeed, float deltaTime)
        {
            //Assert.IsTrue(dir.IsNormalized());
            var newVelocity = velocity + dir * accel * deltaTime;
            var newSpeed = newVelocity.magnitude;
            var curSpeed = velocity.magnitude;
            var maxSpeed = SharedHelpers.Max(targetSpeed, curSpeed);
            if (newSpeed > maxSpeed)
                newVelocity = newVelocity * maxSpeed / newSpeed;
            return newVelocity;
        }

        public static Vector2 ApplyAccel(Vector2 velocity, List<Accel> accel, float deltaTime)
        {
            Vector2 deltaVelocity = Vector2.zero;
            for (int i = 0; i < accel.Count; i++)
            {
                var newVelocity = ApplyAccel(velocity, accel[i].Direction, accel[i].Acceleration, accel[i].TargetSpeed, deltaTime);
                deltaVelocity += newVelocity - velocity;
            }

            return velocity + deltaVelocity;
        }

        public static Vector2 ApplyAccel(Vector2 velocity, Accel accel, float deltaTime)
        {
            return ApplyAccel(velocity, accel.Direction, accel.Acceleration, accel.TargetSpeed, deltaTime);
        }

        public static Vector2 ApplyFriction(Vector2 velocity, Vector2 dir, float targetSpeed, float friction, float deltaTime)
        {
            var curSpeed = velocity.magnitude;
            if (!curSpeed.ApproximatelyZero())
            {
                float maxSpeed = 0;
                maxSpeed = SharedHelpers.Max(Vector2.Dot(dir, velocity) / curSpeed * targetSpeed, maxSpeed);
                if (curSpeed > maxSpeed)
                {
                    var newSpeed = SharedHelpers.Max(curSpeed - friction * deltaTime, maxSpeed);
                    velocity = velocity * newSpeed / curSpeed;
                }

                return velocity;
            }

            return Vector2.zero;
        }

        internal static Vector2 ApplyFriction(Vector2 velocity, List<Accel> accel, float friction, float deltaTime)
        {
            var curSpeed = velocity.magnitude;
            if (!curSpeed.ApproximatelyZero())
            {
                float maxSpeed = 0;
                for (int i = 0; i < accel.Count; i++)
                {
                    var dir = accel[i].Direction;
                    maxSpeed = SharedHelpers.Max(Vector2.Dot(dir, velocity) / curSpeed * accel[i].TargetSpeed, maxSpeed);
                }

                if (curSpeed > maxSpeed) ///#PZ-13568: ??(to Andrey): why `friction` is applied only "if (curSpeed > maxSpeed)" ?
                {
                    var newSpeed = SharedHelpers.Max(curSpeed - friction * deltaTime, maxSpeed);
                    velocity = velocity * newSpeed / curSpeed;
                }

                return velocity;
            }

            return Vector2.zero;
        }
    }
}