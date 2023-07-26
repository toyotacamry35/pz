using System;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.Utils;
using Src.Locomotion.Delegates;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public static class LocomotionAirbornePhysics
    {
        [NotNull] private static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger("LocomotionPhysics");

        public static LocomotionPhysics.Accel CalcAirControlAccel(
            Vector2 input, 
            Vector2 guide, 
            Vector2 forward,
            AccelByTimeFn accelFn,
            SpeedByDirAndTimeFn speedFn,
            float airborneTime)
        {
            var inputWorld = LocomotionHelpers.TransformMovementInputAxes(input, guide);
            var accelDir = inputWorld.normalized;
            var inputLocal = LocomotionHelpers.InverseTransformVector(inputWorld, forward);
            var speed = speedFn(inputLocal, airborneTime);
            var accel = accelFn(airborneTime);
            return new LocomotionPhysics.Accel(accelDir, accel, speed);
        }

        public static LocomotionVector CalcJumpImpulse(
            Vector2 input,
            Vector2 guide,
            Vector2 forward,
            Vector2 velocity,
            ImpulseByDirFn verticalImpulseFn,
            ImpulseByDirFn horizontalImpulseFn)
        {
            var inputWord = LocomotionHelpers.TransformMovementInputAxes(input, guide);
            var inputLocal = LocomotionHelpers.InverseTransformVector(inputWord, forward);
            var speedAlongInput = Vector2.Dot(inputWord, velocity);
            return new LocomotionVector(
                inputWord.normalized * Math.Max(horizontalImpulseFn(inputLocal) - speedAlongInput, 0),
                verticalImpulseFn(inputLocal)
            );
        }

        //@params `jumpDistanceMin`, `jumpDistanceMax`, `jumpHeightMax` - leave their values by dflt to ignore these restrictions
        //..  * `jumpDistanceMin/-Max` - min/max jump distance under conditions: dh==0 & a==45deg (i.e. conditions, providing max possible jump distance with fixed |V0| value)
        //..  * `jumpHeightMax` - is taken into account _only_ if obstacle is taken into account (is used to avoid very high jump in attempt to overcome the obstacle)
        //..                        In other cases max-H-restriction is actually indirectly(��������) defined by 2 other restrictions: `jumpDistanceMin/-Max`, 
        //..                        ///.. 'cos in this case (without really obstructing obstacles (i.e. crossing trajectory)) we use angle == 45deg.
        //@param `obstacleHeightRelativeToStartPos` - leave it <= 0 (as by dflt) to ignore obstacle check
        //@param `isObstacleAtHorizontalMidPoint` - is "at-mid-point" if dh==0
        public static LocomotionVector CalcJumpToTargetInitialVelocity(
            LocomotionVector startPos,
            LocomotionVector targetPoint,
            LocomotionVector currVelocity,
            float gravity,
            float jumpDistanceMin = 0,
            float jumpDistanceMax = float.MaxValue,
            float jumpHeightMax = float.MaxValue,
            float obstacleHeightRelativeToStartPos = 0,
            bool isObstacleAtHorizontalTopPoint = true,
            float distanceFromStartPosToObstacleHorizontal = -1
        )
        {
            // Check inputs:
            if (jumpDistanceMin > jumpDistanceMax)
            {
                Logger.IfWarn()?.Message("jumpDistanceMin({0}) > jumpDistanceMax({1})", jumpDistanceMin, jumpDistanceMax).Write();
                Swap(ref jumpDistanceMin, ref jumpDistanceMax);
            }

            var g = Math.Abs(gravity); // it's == "-9.81" at LocoStats.jdb
            var toTarget = targetPoint - startPos;
            var dh = toTarget.Vertical;
            var dx = toTarget.Horizontal.magnitude;

            // angle (rad):
            var a = 45 * Pi / 180;
            var angle90 = HalfPi;

            // Every time V0 recalced, it should be clamped by these borders:
            float V0_restrictionMin = 0f;
            float V0_restrictionMax = float.MaxValue;

            // 1. 1st calc of Initial velocity (it's supposed method result, if don't violate any of restrictions):
            var V0 = CalcV0(dx, dh, a, g, V0_restrictionMin, V0_restrictionMax);

            // ТЕСТ КОДИРОВКИ
            // 2. Take account Min & Max distance restrictions:
            //.. `jumpDistanceMin/-Max` �������� ��� ������ ���������� ���./����. ������ V0 
            //.. (��� ������ ��� �����������-�� ��. � �������� ���������).
            //.. ���-� ���� �������� - `V0_restrictionMin/-Max`, �� ���-e ���� �������� V0 ��� ����.����.���������.
            //#note: ���������� ������� (��������, ���� �����������) ����� �������� `a`, ��� ����� �������� � ����, ��� ������-�� �����.����-� ������ ����� < `jumpDistanceMin`
            var currJumpMaxHorizDistance = CalcCurrJumpMaxHorizDistance(V0, a, g);
            if (currJumpMaxHorizDistance < jumpDistanceMin)
            {
                var prevV0 = V0;
                V0 = V0_restrictionMin = Sqrt(jumpDistanceMin * g / Sin(2 * a));
                Logger.IfDebug()?.Message("Hit distanceMIN({0}). (was:{1}). => V0 restriction MIN == {2} (prevV0:{3})", jumpDistanceMin, currJumpMaxHorizDistance, V0_restrictionMin, prevV0).Write();
            }
            else if (currJumpMaxHorizDistance > jumpDistanceMax)
            {
                var prevV0 = V0;
                V0 = V0_restrictionMax = Sqrt(jumpDistanceMax * g / Sin(2 * a));
                Logger.IfDebug()?.Message(string.Format("Hit distanceMAX({0}). (was:{1}). => V0 restriction MAX == {2} (prevV0:{3})", jumpDistanceMax, currJumpMaxHorizDistance, V0_restrictionMax, prevV0)).Write();
            }

            Logger.IfDebug()?.Message("1) CalcJumpToTargetImpulse: trgt:{0}, myPos:{1}, dh:{2}, dx:{3}, alfa:{4}, V0:{5} " +
                                         "\n dx_obst:{6}, dh_obst:{7}, g:{8}", targetPoint, startPos, dh, dx, a, V0, distanceFromStartPosToObstacleHorizontal, obstacleHeightRelativeToStartPos, g)
                .Write();

            // 3. Take account obstacle:
            float dh_obst = obstacleHeightRelativeToStartPos;
            float dx_obst;
            if (dh_obst <= 0)
                return PrepareResult(V0, a, toTarget, currVelocity);

            if (isObstacleAtHorizontalTopPoint)
            {
                // ����� ��� ��� ������ ����� ���������. �� ����� �������������� 
                //.. - � ���� ������ ������ ���-�� ������� maxH: "V^2 * sin^2(a) / 2g". ���-� ���������� � obstacleH.
                dx_obst = CalcCurrJumpMaxHorizDistance(V0, a, g) / 2;
            }
            else
            {
                dx_obst = distanceFromStartPosToObstacleHorizontal;
                if (dx_obst <= 0)
                {
                    Logger.IfWarn()?.Message($"`{nameof(dh_obst)}` has valid value ({dh_obst}), " +
                                $"but {nameof(isObstacleAtHorizontalTopPoint)} == {isObstacleAtHorizontalTopPoint} " +
                                $"&& {nameof(dx_obst)} <= 0 : ({dx_obst}).")
                        .Write();
                    return PrepareResult(V0, a, toTarget, currVelocity);
                }
            }

            if (dx_obst >= dx)
            {
                Logger.IfWarn()?.Message($"`{nameof(dx_obst)}`({dx_obst}) >= dist.to target point ({dx})").Write();
                return PrepareResult(V0, a, toTarget, currVelocity);
            }

            var minObstacleGapVert = 0.1f;

            // vert.coordinate of curr. parabola graph at obstacle local x:
            var dh_overObstCurr = dx_obst * Tan(a) - (g / 2) * Sqr(dx_obst / (V0 * Cos(a)));
            if (dh_overObstCurr < dh_obst + minObstacleGapVert)
            {
                // Check's failed - so we should recalc. `localV0` & `a` to satisfy this restriction:
                a = Atan( (Sqr(dx) * (dh_obst + minObstacleGapVert) - Sqr(dx_obst) * dh)
                        / (Sqr(dx) * dx_obst - Sqr(dx_obst) * dx) );

                a = a.Clamped(0, angle90);
                V0 = CalcV0(dx, dh, a, g, V0_restrictionMin, V0_restrictionMax);

                Logger.IfDebug()?.Message("2) CalcJumpToTargetImpulse: 1) alfa:{0}, locV0:{1}", a, V0).Write();
            }

            // 4. Take account height restriction: 
            var maxH = Sqr(V0) * Sqr(Sin(a)) / (2*g);
            Logger.IfDebug()?.Message("maxH: {0}", maxH).Write();
            if (maxH > jumpHeightMax)
            {
                var prevV0 = V0;
                V0 = Sqrt(2 * g * jumpHeightMax) / Sin(a);
                V0 = V0.Clamped(V0_restrictionMin, V0_restrictionMax);
                Logger.IfDebug()?.Message("maxH({0}) > jumpHeightMax({1}). So V0 recalced: {2}-->{3}", maxH, jumpDistanceMax, prevV0, V0).Write();
            }

            // 5. Return result:
            return PrepareResult(V0, a, toTarget, currVelocity);
        }
       
        
        //sqrt((-9.81 * sqr(dx)) / (2 * (dh - dx * tan(a_deg)) * sqr(cos(a_deg))))
        private static float CalcV0(float dx, float dh, float a, float g, float V0_restrictionMin, float V0_restrictionMax)
        {
            var res = Sqrt(-(g * dx*dx) / (2 * (dh - dx * Tan(a)) * Sqr(Cos(a))));
            return res.Clamped(V0_restrictionMin, V0_restrictionMax);
        }

        private static float CalcCurrJumpMaxHorizDistance(float V0, float a, float g)
        {
            return Sqr(V0) * Sin(2 * a) / g;
        }
        
        private static LocomotionVector PrepareResult (float V0, float a, LocomotionVector toTarget, LocomotionVector currVelocity)
        {
            // 1. Make vec, rotated by angle `a` (to simplify, let this vec.Horizontal be == toTarget.Horizontal)
            var vecV0 = new LocomotionVector(toTarget.Horizontal, (float) Math.Tan(a) * toTarget.Horizontal.magnitude);
            // 2. normalize:
            vecV0 = vecV0.Normalized;
            // 3. scale by V0 scalar (calculated earlier):
            vecV0 *= V0;

            Logger.IfDebug()?.Message("Result vec: {0}", vecV0).Write();
            return vecV0 - currVelocity;
        }
    }
}
