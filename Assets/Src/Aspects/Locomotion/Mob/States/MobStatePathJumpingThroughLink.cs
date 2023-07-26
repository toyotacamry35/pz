using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using static Src.Locomotion.LocomotionAirbornePhysics;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion.States
{
    /// <summary>
    /// Jump to target point through NavMesh link
    /// </summary>
    internal class MobStatePathJumpingThroughLink : StateBase<CommonStateMachineContext>
    {
        public override void OnEnter(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            var mobStats = (IMobStatsProvider) ctx.Stats;
            var targetPoint = new LocomotionVector(ctx.Input[MobInputs.TargetPointXY], ctx.Input[MobInputs.TargetPointV]);
            pipeline.ApplyImpulse(
                CalcJumpToTargetInitialVelocity(
                    startPos: ctx.Body_Deprecated.Position,
                    targetPoint: targetPoint,
                    currVelocity: ctx.Body_Deprecated.Velocity,
                    gravity: ctx.Environment.Gravity,
                    obstacleHeightRelativeToStartPos: 0f, //todo(1/3): pass here data from obstacle
                    isObstacleAtHorizontalTopPoint: true, //todo(2/3): pass here data from obstacle
                    distanceFromStartPosToObstacleHorizontal: -1f, //todo(3/3): pass here data from obstacle
                    jumpDistanceMin: mobStats.JumpToTargetMinDistance,
                    jumpDistanceMax: mobStats.JumpToTargetMaxDistance,
                    jumpHeightMax: mobStats.JumpToTargetMaxHeight
                )
            );
            DebugAgent.Set(DrawSize, Vector3.one * 0.1f);
            DebugAgent.Set(DrawColor, Color.magenta);
            DebugAgent.Set(DrawSphere, ctx.Body_Deprecated.Position);
            DebugAgent.Set(DrawColor, Color.cyan);
            DebugAgent.Set(DrawSphere, targetPoint);
        }

        public override void Execute(CommonStateMachineContext ctx, VariablesPipeline pipeline)
        {
            pipeline.ApplyFlags(LocomotionFlags.Jumping | LocomotionFlags.Moving | LocomotionFlags.Airborne/*| LocomotionFlags.FollowPath*/)
                .ApplyOrientation(
                    direction: ctx.Input[Guide],
                    maxAngularVelocity: ctx.Stats.JumpYawSpeed)
                .ApplyGravity(
                    ctx.Environment.Gravity);
        }
    }
}