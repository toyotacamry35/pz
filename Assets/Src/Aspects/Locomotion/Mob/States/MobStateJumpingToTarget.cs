using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion.States
{
    /// <summary>
    /// Jump to target point
    /// </summary>
    internal class MobStateJumpingToTarget : StateBase<CommonStateMachineContext>
    {
        private Vector2 _jumpDirection;
        
        public override void OnEnter(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            _jumpDirection = ctx.Input[Guide];
            var mobStats = (IMobStatsProvider) ctx.Stats;
            var targetPoint = new LocomotionVector(ctx.Input[MobInputs.TargetPointXY], ctx.Input[MobInputs.TargetPointV]);
            var currentVars = ctx.CurrentVars;
            pipeline.ApplyImpulse( CalcJumpToTargetInitialVelocity( startPos: currentVars.Position,
                                                            targetPoint: targetPoint,
                                                            currVelocity: currentVars.Velocity, ///затык : new LocomotionVector(0f, 0f, 0f),
                                                            gravity: ctx.Environment.Gravity,
                                                            obstacleHeightRelativeToStartPos: 0f,           //todo(1/3): pass here data from obstacle
                                                            isObstacleAtHorizontalTopPoint: true,           //todo(2/3): pass here data from obstacle
                                                            distanceFromStartPosToObstacleHorizontal: -1f,  //todo(3/3): pass here data from obstacle
                                                            jumpDistanceMin: mobStats.JumpToTargetMinDistance,
                                                            jumpDistanceMax: mobStats.JumpToTargetMaxDistance,
                                                            jumpHeightMax:   mobStats.JumpToTargetMaxHeight ) );
            DebugAgent.Set(DrawSize, Vector3.one * 0.1f);
            DebugAgent.Set(DrawColor, Color.magenta);
            DebugAgent.Set(DrawSphere, ctx.Body_Deprecated.Position);
            DebugAgent.Set(DrawColor, Color.cyan);
            DebugAgent.Set(DrawSphere, targetPoint);

            //DebugTest(mobStats);
        }

        private void DebugTest(IMobStatsProvider mobStats)
        { 
            if (DbgLog.Enabled) DbgLog.Log("Тестовый вызов №1: 5, 2, 2, 2.5. Ожидаемый ответ: a: 0.7854(45*), V0: 9.04.   a: 1.086(62.2*), V0: 8.68");
            CalcJumpToTargetInitialVelocity( startPos: new LocomotionVector(0, 0, 0), 
                                     targetPoint: new LocomotionVector(5, 0, 2),
                                     currVelocity: new LocomotionVector(), 
                                     gravity: -9.81f,
                                     obstacleHeightRelativeToStartPos: 2.5f,
                                     isObstacleAtHorizontalTopPoint: true,
                                     distanceFromStartPosToObstacleHorizontal: 2f,
                                     jumpDistanceMin: mobStats.JumpToTargetMinDistance,
                                     jumpDistanceMax: mobStats.JumpToTargetMaxDistance,
                                     jumpHeightMax: mobStats.JumpToTargetMaxHeight );
            if (DbgLog.Enabled) DbgLog.Log("Тестовый вызов №2: 5, -2, 3, .6. Ожидаемый ответ: a: 0.7854(45*), V0: 5.919.   a: 0.833(47.7*), V0: 6.011");
            CalcJumpToTargetInitialVelocity(startPos: new LocomotionVector(0, 0, 2),
                                     targetPoint: new LocomotionVector(0, -5, 0),
                                     currVelocity: new LocomotionVector(),
                                     gravity: -9.81f,
                                     obstacleHeightRelativeToStartPos: 0.6f,
                                     isObstacleAtHorizontalTopPoint: true,
                                     distanceFromStartPosToObstacleHorizontal: 3f,
                                     jumpDistanceMin: mobStats.JumpToTargetMinDistance,
                                     jumpDistanceMax: mobStats.JumpToTargetMaxDistance,
                                     jumpHeightMax: mobStats.JumpToTargetMaxHeight );
            if (DbgLog.Enabled) DbgLog.Log("Тестовый вызов №3: 5, -2, 4, -.2. Ожидаемый ответ: a: 0.7854(45*), V0: 5.919.   a: 0.933(53.47*), V0: 6.289");
            CalcJumpToTargetInitialVelocity(startPos: new LocomotionVector(0, 0, 0),
                                     targetPoint: new LocomotionVector(0, 5, -2),
                                     currVelocity: new LocomotionVector(),
                                     gravity: -9.81f,
                                     obstacleHeightRelativeToStartPos: -.2f,
                                     isObstacleAtHorizontalTopPoint: true,
                                     distanceFromStartPosToObstacleHorizontal: 4f,
                                     jumpDistanceMin: mobStats.JumpToTargetMinDistance,
                                     jumpDistanceMax: mobStats.JumpToTargetMaxDistance,
                                     jumpHeightMax: mobStats.JumpToTargetMaxHeight );
        }


        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping | LocomotionFlags.Moving | LocomotionFlags.Airborne)
                .ApplyOrientation(
                    direction: _jumpDirection,
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                .ApplyGravity(
                    ctx.Environment.Gravity);
        }
    }
}