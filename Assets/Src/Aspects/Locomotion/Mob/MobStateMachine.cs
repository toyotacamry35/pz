using System;
using JetBrains.Annotations;
using SharedCode.Utils;
using Src.Locomotion.States;
using UnityEngine;
using static Src.Locomotion.Predicates<Src.Locomotion.MobStateMachineContext>;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.MobInputs;

using Vector2 = SharedCode.Utils.Vector2;


namespace Src.Locomotion
{
    public static class MobStateMachine
    {
        public static ILocomotionStateMachine Create(
            MobStateMachineContext context,
            MobLocomotionReactions reactions,
            [CanBeNull] ICurveLoggerProvider curveLogProv,
            Func<bool> shouldSaveLocoVars, Action<LocomotionVariables, Type> saveLocoVarsCallback)
        {
            var standingState             = new MobStateStanding();
            var turningState              = new MobStateTurning();
            var walkingState              = new MobStateWalking();
            var walkingPathState          = new MobStatePathWalking(curveLogProv);
            var jumpingFromRunState       = new MobStateJumpingFromRun();
            var jumpingToTargetState      = new MobStateJumpingToTarget();
            var jumpingFromSpotDelayState = new MobStateJumpingFromSpotDelay();
            var jumpingFromSpotState      = new MobStateJumpingFromSpot();
            var jumpingOnSpotState        = new MobStateJumpingOnSpot();
            var jumpingOffState           = new MobStateJumpingOff();
            var jumpingPathState          = new MobStatePathJumping();
            var jumpingToTargetPathState  = new MobStatePathJumpingThroughLink();
            var jumpingOffPathState       = new MobStatePathJumpingOff();
            var landingState              = new MobStateLanding();
            var landingPathState          = new MobStatePathLanding();
            var landingOnSpotState        = new MobStateLanding();
            var dodgeState                = new MobStateDodge();
            var teleportState             = new MobStateTeleport();
            var directState               = new CommonStateDirect();
            var invalidState              = new CommonStateNope();

            var isValid = IsTrue(c => c.Environment.Valid);
            var isHasGuide           = IsTrue(c => c.Input[Guide].Longer(0.001f));
            var isMoveAxesHigh       = IsTrue(c => c.Input[Move].Longer(c.Constants.InputMoveThreshold));
            var isMoveAxesLow        = IsTrue(c => c.Input[Move].Shorter(c.Constants.InputStandingThreshold));
            var isJumpingToTarget    = IsTrue(c => c.Input[JumpToTarget]);
            var isJumpActivated      = BecameTrue(c => c.Input[Jump], context.Stats.ActionTriggerInHindsight);
            var isDodgeActivated     = BecameTrue(c => c.Input[Dodge], context.Stats.ActionTriggerInHindsight);
       //     var isForwardActive      = Simple(c => c.Input[MoveLng] > c.Constants.InputMoveThreshold);
            var isFollowPathActive   = IsTrue(c => c.Input[FollowPath]);
            var isTeleport           = IsTrue(c => c.Input[Teleport]);
            var isAirborne           = IsTrue(c => c.Environment.Airborne);
            var isJumpingOffDistance = IsTrue(c => c.Environment.DistanceToGround > c.Stats.JumpOffDistance);
            var isLandingDistance = !isAirborne | (c => LocomotionHelpers.MovingTime(-c.Environment.DistanceToGround,c.Body_Deprecated.Velocity.Vertical, c.Environment.Gravity) < c.Stats.LandingDuration);
            var isSpeedForStanding   = IsTrue(c => c.Body_Deprecated.Velocity.Horizontal.Shorter(c.Stats.StandingSpeedThreshold)); 
            var isMovingForward      = IsTrue(c => Vector2.Dot(c.Body_Deprecated.Forward, c.Body_Deprecated.Velocity.Horizontal) > c.Stats.JumpFromRunSpeedThreshold);
            var isJumpFromSpotDelay  = IsTrue(c => c.StateElapsedTime < c.Stats.JumpFromSpotDelay);
            var isMoveAxesForJumpFormSpot   = WasTrue(c => c.Input[Move].Longer(c.Constants.InputMoveThreshold), context.Stats.JumpFromSpotDelay);
            var isWaitForJump = !isAirborne & IsTrue(c => c.StateElapsedTime < c.Stats.JumpMinDuration);
            var isJumpOffActionTimeWindow   = IsTrue(c => c.StateElapsedTime < c.Stats.ActionWhileJumpOffTimeWindow);
            var isDirectControl             = IsTrue(c => c.Input[Direct]);
            var isTowardsToGuide    = !isHasGuide | (c => Vector2.Dot(c.Body_Deprecated.Forward, c.Input[Guide].normalized) > Mathf.Cos(c.Stats.TowardsDirectionTolerance));
            var isTurnOnSpotRequired = isHasGuide & (c => Vector2.Dot(c.Body_Deprecated.Forward, c.Input[Guide].normalized) < Mathf.Cos(c.Stats.TurnOnSpotThreshold));
            var isTurnOnWalkRequired = isHasGuide & (c => Vector2.Dot(c.Body_Deprecated.Forward, c.Input[Guide].normalized) < Mathf.Cos(c.Stats.TurnOnRunThreshold));
            
         //   Action onEnterJumpState = (callbacksHolder != null) ? callbacksHolder.OnEnterJumpState : (Action)null;
      //      Action onExitJumpState  = (callbacksHolder != null) ? callbacksHolder.OnExitJumpState : (Action)null;

            return new StateMachineBuilder<MobStateMachineContext>(StatesRoughCountRoundedUp)
                .AnyState(_ => _
                    .If(!isValid).State(invalidState)
                    .If(isDirectControl).State(directState)
                    .If(isTeleport).State(teleportState)
                )
                .State(invalidState, _ => _
                    .If(isValid).State(standingState)
                )
                .State(standingState, _ => _
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isDodgeActivated).State(dodgeState)
                    .If(isTurnOnSpotRequired).State(turningState)
                    .If(isJumpingToTarget).State(jumpingToTargetState)
                    .If(isJumpActivated).State(jumpingFromSpotDelayState)
                    .If(isFollowPathActive).State(walkingPathState)
                    .If(isMoveAxesHigh).State(walkingState)
                )
                .State(turningState, _ => _
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isTowardsToGuide & isJumpingToTarget).State(jumpingToTargetState)
                    .If(isTowardsToGuide & isJumpActivated).State(jumpingFromSpotDelayState)
                    .If(isDodgeActivated).State(dodgeState)
                    .If(isTowardsToGuide).State(standingState)
                )
                .State(walkingState, _ => _
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isDodgeActivated).State(dodgeState)
                    .If(isTurnOnWalkRequired).State(turningState)
                    .If(isJumpingToTarget).State(jumpingToTargetState)
                    .If(isJumpActivated & isMovingForward).State(jumpingFromRunState)
                    .If(isJumpActivated).State(jumpingFromSpotDelayState)
                    .If(isFollowPathActive).State(walkingPathState)
                    .If(isMoveAxesLow & isSpeedForStanding).State(standingState)
                )
                .State(walkingPathState, _ => _
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffPathState)
                    .If(isTurnOnWalkRequired).State(turningState)
                    .If(isJumpingToTarget).State(jumpingToTargetPathState)
                    .If(isJumpActivated).State(jumpingPathState)
                    .If(isMoveAxesLow & isSpeedForStanding).State(standingState)
                    .If(!isFollowPathActive).State(walkingState)
                )
                .State(jumpingFromSpotDelayState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .If(isJumpFromSpotDelay).Stay()
                    .If(isMoveAxesForJumpFormSpot).State(jumpingFromSpotState)
                    .Otherwise(jumpingOnSpotState)
                )
                .State(jumpingOnSpotState, _ => _
                    .If(isWaitForJump).Stay()
                    .If(isLandingDistance).State(landingOnSpotState)
                )
                .State(jumpingFromSpotState, _ => _
                    .If(isWaitForJump).Stay()
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingFromRunState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .If(isWaitForJump).Stay()
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingOffState, _ => _        
                    .OnEnter(reactions.JumpReaction)
                    .If(isJumpActivated & isMovingForward & isJumpOffActionTimeWindow).State(jumpingFromRunState)
                    .If(isDodgeActivated & isMoveAxesHigh & isJumpOffActionTimeWindow).State(dodgeState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingToTargetState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .If(isWaitForJump).Stay()                                       
                    .If(isLandingDistance).State(landingOnSpotState)
                )
                .State(jumpingPathState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .If(isWaitForJump).Stay()
                    .If(isFollowPathActive & isLandingDistance).State(landingPathState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingToTargetPathState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .If(isWaitForJump).Stay()
                    .If(isFollowPathActive & isLandingDistance).State(landingPathState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingOffPathState, _ => _        
                    .OnEnter(reactions.JumpReaction)
                    .If(isFollowPathActive & isJumpActivated & isMovingForward & isJumpOffActionTimeWindow).State(jumpingPathState)
                    .If(isJumpActivated & isMovingForward & isJumpOffActionTimeWindow).State(jumpingFromRunState)
                    .If(isDodgeActivated & isMoveAxesHigh & isJumpOffActionTimeWindow).State(dodgeState)
                    .If(isFollowPathActive & isLandingDistance).State(landingPathState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(landingState, _ => _
                    .OnExit(reactions.LandReaction)
                    .If(isAirborne).Stay()
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingState)
                    .Otherwise(standingState)
                )
                .State(landingOnSpotState, _ => _
                    .OnExit(reactions.LandReaction)
                    .If(isAirborne).Stay()
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingPathState)
                    .Otherwise(standingState)
                )
                .State(landingPathState, _ => _
                    .OnExit(reactions.LandReaction)
                    .If(isAirborne).Stay()
                    .If(isFollowPathActive & (isMoveAxesHigh | !isSpeedForStanding)).State(walkingPathState)
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingState)
                    .Otherwise(standingState)
                )
                .State(dodgeState, _ => _
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(c => c.StateElapsedTime < c.Stats.DodgeDuration).Stay()
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingState)
                    .Otherwise(standingState)
                )
                .State(directState, _ => _
                    .If(!isDirectControl).State(standingState)
                )
                .State(teleportState, _ => _
                    .If(!isTeleport).State(standingState)
                )
                ///! Don't forget to adjust `StatesRoughCountRoundedUp` value, when adding/removing items from here!
                .Build(context, curveLogProv, null, shouldSaveLocoVars, saveLocoVarsCallback);
        }
        public static int StatesRoughCountRoundedUp = 23; //actually 21
    }
}