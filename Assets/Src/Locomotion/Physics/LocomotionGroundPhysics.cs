using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.Utils;
using Src.Locomotion.Delegates;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public static class LocomotionGroundPhysics
    {        
        public static LocomotionPhysics.Accel CalcWalkAccel(
            Vector2 input, 
            Vector2 guide, 
            Vector2 forward,
            float accel,
            AccelBySlopeFn accelFn,
            SpeedByDirFn speedFn, 
            SlopeByDirFn slopeFactorFn,
            float elapsedTime,
            ref Vector2 lastAccelDir,
            float minSpeed = 0,
            float speedFactor = 1)
        {
            if(elapsedTime <= 0)
                lastAccelDir = Vector2.zero;
            Vector2 inputWorld;
            float speed;
            if (!input.ApproximatelyZero())
            {
                inputWorld = LocomotionHelpers.TransformMovementInputAxes(input, guide);
                var inputLocal = LocomotionHelpers.InverseTransformVector(inputWorld, forward);
                speed = speedFn(inputLocal) * speedFactor;
            }
            else
            {
                inputWorld = lastAccelDir;
                speed = minSpeed;
            }
            var accelDir = inputWorld.normalized;
            var slopeFactor = slopeFactorFn(accelDir);
            var invSlopeFactor = Mathf.Sqrt(1 - slopeFactor.Sqr());
            lastAccelDir = accelDir;
            var accelFactor = accelFn(Mathf.Asin(-slopeFactor));
            return new LocomotionPhysics.Accel(accelDir, accel * accelFactor, speed * invSlopeFactor);
        }
        
        public static LocomotionPhysics.Accel CalcWalkAccel(
            Vector2 input, 
            Vector2 guide, 
            Vector2 forward,
            float accel,
            SpeedByDirFn speedFn, 
            SlopeByDirFn slopeFactorFn,
            float speedFactor = 1,
            [CanBeNull] CurveLogger curveLogger = null)
        {
            Vector2 inputWorld = LocomotionHelpers.TransformMovementInputAxes(input, guide);
            var accelDir = inputWorld.normalized;   //0805?toA: normalized already if sqrmgntd>1 inside ^^^
            var inputLocal = LocomotionHelpers.InverseTransformVector(inputWorld, forward); //0805?toA: !=? input? ## input tranformed by guide, this one transformed back by forward
            var speed = speedFn(inputLocal) * speedFactor;
            var invSlopeFactor = Mathf.Sqrt(1 - slopeFactorFn(accelDir).Sqr());

            //Dbg:
            // curveLogger?.IfActive?.AddData("0.1) GroundPhys.inputW", SyncTime.Now, inputWorld.x, inputWorld.y);
            // curveLogger?.IfActive?.AddData("0.1) GroundPhys.forward", SyncTime.Now, forward.x, forward.y);
            // curveLogger?.IfActive?.AddData("0.1) GroundPhys.speed", SyncTime.Now, speed);

            return new LocomotionPhysics.Accel(accelDir, accel, speed * invSlopeFactor);
        }
        
        public static LocomotionPhysics.Accel CalcSlipAccel(
            Vector2 slopeDirection, 
            float slopeFactor,
            SlipAccelBySlopeFn slipAccelFn,
            SlipSpeedBySlopFn slipSpeedFn)
        {
            var slopeAngle = Mathf.Asin(slopeFactor);
            var slipAccel = slipAccelFn(slopeAngle);
            var slipSpeed = slipSpeedFn(slopeAngle);
            var invSlopeFactor = Mathf.Sqrt(1 - slopeFactor.Sqr());
            return new LocomotionPhysics.Accel(slopeDirection, slipAccel, slipSpeed * invSlopeFactor);
        }

        public static LocomotionPhysics.Accel CalcSlippingAccel(
            Vector2 inputAxes, 
            Vector2 guide,
            Vector2 forward,
            float accel,
            SpeedByDirFn speedFn, 
            SlopeByDirFn slopeFactorFn)
        {
            var inputDir = LocomotionHelpers.TransformMovementInputAxes(inputAxes, guide);
            var inputDirLocal = LocomotionHelpers.InverseTransformVector(inputDir, forward);
            var accelDir = new Vector2(0, inputDir.y).normalized;
            var speed = speedFn(inputDirLocal) * Mathf.Sqrt(1 - slopeFactorFn(accelDir).Sqr());
            return new LocomotionPhysics.Accel(accelDir, accel, speed);
        }
        
        public static float CalculateStepUp(
            Vector2 input,
            Vector2 guide,
            LocomotionVector velocity,
            SlopeByDirFn slopeFn,
            ObstacleByDirFn obstacleFn,
            float lookAheadTime,
            float maxStepUp,
            float deltaTime
        )
        {
            var inputWorld = LocomotionHelpers.TransformMovementInputAxes(input, guide).normalized;
            return CalculateStepUp(inputWorld, velocity, slopeFn, obstacleFn, lookAheadTime, maxStepUp, deltaTime);
        }

        public static float CalculateStepUp(
            Vector2 direction,
            LocomotionVector velocity,
            SlopeByDirFn slopeFn,
            ObstacleByDirFn obstacleFn,
            float lookAheadTime,
            float maxStepUp,
            float deltaTime
        )
        {
            var minSpeed = 1;
            var probeDir = LocomotionHelpers.SurfaceTangent(slopeFn(direction), direction);
            var speed = LocomotionVector.Dot(velocity, probeDir);
            if (speed > -0.0001f)
            {
                speed = Mathf.Max(speed, minSpeed);
                var lookAheadDistance = lookAheadTime * speed;
                var nfo = obstacleFn(probeDir, lookAheadDistance, maxStepUp);
                if (nfo.Detected && nfo.IsStair)
                {
                    var distToStairPoint = LocomotionVector.Dot(nfo.StairPoint - nfo.Pivot, probeDir);
                    var distance = Mathf.Lerp(nfo.Distance, distToStairPoint, distToStairPoint > 0 ? nfo.Distance / distToStairPoint : 1);
                    var vertical = Mathf.Max(nfo.StairPoint.Vertical - nfo.Pivot.Vertical, 0);
                    var time = Mathf.Max(distance / speed, deltaTime);
                    return vertical / time * deltaTime;
                }
            }
            return 0;
        }
    }
}