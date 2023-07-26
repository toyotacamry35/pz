using Assets.Src.Lib.Cheats;
using Assets.Src.Locomotion.Debug;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using Src.Locomotion.States;
using SharedCode.Utils;
using Src.Aspects.Locomotion;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.Predicates<Src.Locomotion.CharacterStateMachineContext>;
using static Src.Locomotion.CommonInputs;
using static Src.Locomotion.CharacterInputs;

namespace Src.Locomotion
{
    public static class CharacterStateMachine
    {
        public static ILocomotionStateMachine Create(
            CharacterStateMachineContext context,
            CharacterLocomotionBindingsDef bindings,
            CharacterLocomotionReactions reactions,
            CalcersCache calcersDoer,
            [CanBeNull] IDumpingLoggerProvider dumpLoggerProvider)
        {
            var standingState = new CharacterStateStanding();
            var walkingState = new CharacterStateWalking();
            var jumpingFromRunState = new CharacterStateJumpingFromRun();
            var jumpingFromSpotDelayState = new CharacterStateJumpingFromSpotDelay();
            var jumpingFromSpotState = new CharacterStateJumpingFromSpot();
            var jumpingOnSpotState = new CharacterStateJumpingOnSpot();
            var jumpingOffState = new CharacterStateJumpingOff();
            var landingState = new CharacterStateLanding();
            var landingOnSpotState = new CharacterStateLanding();
            var hardLandingState = new CharacterStateLanding().SetName("Hard Landing");
            var postHardLandingState = new CharacterStatePostLandingStun().SetName("Post Hard Landing");
            var fallingState = new CharacterStateFalling();
            var sprintState = new CharacterStateSprint();
            var slippingState = new CharacterStateSlipping();
            var directState = new CharacterStateDirect();
            var airborneAttackBgnState = new CharacterStateAirborneAttack();
            var airborneAttackHitState = new CharacterStateAirborneAttackHit();
            var cheatState = new CharacterStateCheatMovement();
            var postCheatState = new CharacterStatePostCheatMovement();
            var invalidState = new CommonStateNope().SetName("Invalid");

            var isValid              = IsTrue(c => c.Environment.Valid);
            var isMoveAxesHigh       = IsTrue(c => c.Input[Move].Longer(c.Constants.InputMoveThreshold));
            var isMoveAxesLow        = IsTrue(c => c.Input[Move].Shorter(c.Constants.InputStandingThreshold));
            var isForwardActivated   = BecameTrue(c => c.Input[MoveLng] > c.Constants.InputMoveThreshold);
            var isForwardActive      = IsTrue(c => c.Input[MoveLng] > c.Constants.InputMoveThreshold);
            var isJumpActivated      = BecameTrue(c => c.Input[Jump], context.Stats.ActionTriggerInHindsight);
            var isJumpEnoughStamina    = IsTrue(calcersDoer.Add(bindings.JumpStaminaPredicate));
            var maxWalkingAngleSin = Sin(context.Stats.MaxWalkingSlope);  
            var isJumpAllowed          = IsTrue(calcersDoer.Add(bindings.JumpPredicate)) & isJumpEnoughStamina & IsTrue(c => c.Environment.SlopeFactor() <= maxWalkingAngleSin );
            var isSprintActivated   = (BecameTrue(c => c.Input[Sprint]) & isForwardActive) | (IsTrue(c => c.Input[Sprint]) & isForwardActivated);
            var isSprintActive       = IsTrue(c => c.Input[Sprint]) & isForwardActive;
            var isSprintEnoughStamina  = IsTrue(calcersDoer.Add(bindings.SprintStaminaPredicate));
            var isSprintAllowed        = IsTrue(calcersDoer.Add(bindings.SprintPredicate)) & isSprintEnoughStamina;
//            var isBlockActive        = IsTrue(c => c.Input[Block]);
//            var isAimActive          = IsTrue(c => c.Input[Aim]);
            var isAirborne           = IsTrue(c => c.Environment.Airborne);
            var isJumpingOffDistance = IsTrue(c => c.Environment.DistanceToGround > c.Stats.JumpOffDistance);
            var isLandingDistance = !isAirborne | IsTrue(c => LocomotionHelpers.MovingTime(-(c.Environment.DistanceToGround - c.Environment.ColliderOffset),c.Body_Deprecated.Velocity.Vertical, c.Environment.Gravity) < c.Stats.LandingDuration);
            var fallHeightBinder = CalcerArgBinder(c => c.History.FallingDistance);
            var isFalling = IsTrue(calcersDoer.Add(bindings.FallingPredicate, "FallHeight", () => fallHeightBinder.Value, period: 1f/60), fallHeightBinder);
            var isSlipping = StayedTrue(SlippingCondition, c => c.Stats.SlippingTimeThreshold);
            var isSlippingStop              = IsTrue(c => c.Body_Deprecated.Velocity.SqrMagnitude < c.Stats.SlippingStopSpeedThreshold);
            var isLongerThanSingleStep      = IsTrue(c => c.StateElapsedTime > c.Stats.SingleStepTime);
            var isSpeedForStanding          = IsTrue(c => c.Body_Deprecated.Velocity.Horizontal.Shorter(c.Stats.StandingSpeedThreshold)); 
            var isMovingForward             = IsTrue(c => Vector2.Dot(c.Body_Deprecated.Forward, c.Body_Deprecated.Velocity.Horizontal) > c.Stats.JumpFromRunSpeedThreshold);
            var isJumpFromSpotDelay         = IsTrue(c => c.StateElapsedTime < c.Stats.JumpFromSpotDelay);
            var isMoveAxesForJumpFormSpot   = WasTrue(c => c.Input[Move].Longer(c.Constants.InputMoveThreshold), context.Stats.JumpFromSpotDelay);
            var isWaitForJump = !isAirborne & IsTrue(c => c.StateElapsedTime < c.Stats.JumpMinDuration);
            var isJumpOffActionTimeWindow   = IsTrue(c => c.StateElapsedTime < c.Stats.ActionWhileJumpOffTimeWindow);
            var isDirectControl             = IsTrue(c => c.Input[Direct]).Debug(DebugTag.IsDirectControl);
//            var isDirectControlOff = IsTrue(c => !c.Input[Direct]);
            var isCheatActivated            = BecameTrue(c => c.Input[CheatMode]);
            var isAirborneAttack             = IsTrue(c => c.Input[AirborneAttack]);
            var isAirborneAttackOff = IsTrue(c => !c.Input[AirborneAttack]);
            var isAirborneAttackHitDistance = !isAirborne | IsTrue(c => (c.Environment.DistanceToGround - c.Environment.ColliderOffset) < c.Stats.AirborneAttackHitDistance);
            var stickHitAngleCos = Cos(context.Stats.StickHitAngle);
            var isAirborneAttackSticking = BecameTrue(c => c.Input[Sticking] && CheckAirborneAttackSticking(c, stickHitAngleCos));
            
            return new StateMachineBuilder<CharacterStateMachineContext>(StatesRoughCountRoundedUp)
                .AnyState(_ => _
                    .If(isCheatActivated & (c => c.CurrentState != cheatState) & (c => ClientCheatsState.Fly)).State(cheatState)
                    .If(!isValid).State(invalidState)
                    .If(isJumpActivated & !isJumpEnoughStamina).Do(reactions.NotEnoughStaminaReaction)
                    .If(isSprintActivated & !isSprintEnoughStamina).Do(reactions.NotEnoughStaminaReaction)
                )
                .State(invalidState, _ => _
                    .OnEnter(reactions.StopAll)
                    .If(isValid).State(standingState)
                )
                .State(standingState, _ => _
                    .If(isAirborneAttack).State(airborneAttackHitState)
                    .If(isDirectControl & !isAirborneAttack).State(directState)
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isSlipping).State(slippingState)
                    .If(isJumpActivated & isJumpAllowed).State(jumpingFromSpotDelayState)
                    .If(isSprintActive & isSprintAllowed).State(sprintState)
                    .If(isMoveAxesHigh).State(walkingState)
                )
                .State(walkingState, _ => _
                    .If(isAirborneAttack).State(airborneAttackHitState)
                    .If(isDirectControl & !isAirborneAttack).State(directState)
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isSlipping).State(slippingState)
                    .If(isJumpActivated & isJumpAllowed & isMovingForward).State(jumpingFromRunState)
                    .If(isJumpActivated & isJumpAllowed).State(jumpingFromSpotDelayState)
                    .If(isSprintActivated & isSprintAllowed).State(sprintState)
                    .If(isMoveAxesLow & isSpeedForStanding & isLongerThanSingleStep).State(standingState)
                )
                .State(sprintState, _ => _
                    .OnEnter(reactions.SprintStartReaction)    
                    .OnExit(reactions.SprintEndReaction)    
                    .If(isAirborneAttack).State(airborneAttackHitState)
                    .If(isDirectControl & !isAirborneAttack).State(directState)
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isSlipping).State(slippingState)
                    .If(isJumpActivated & isJumpAllowed & isMovingForward).State(jumpingFromRunState)
                    .If(isJumpActivated & isJumpAllowed).State(jumpingFromSpotDelayState)
                    .If(!isSprintEnoughStamina).Do(reactions.NotEnoughStaminaReaction)
                    .If(!isSprintAllowed).State(walkingState)
                    .If(!isSprintActive & isLongerThanSingleStep).State(walkingState)
                    .If(isMoveAxesLow & isLongerThanSingleStep).State(walkingState)
                )
                .State(jumpingFromSpotDelayState, _ => _
                    .If(isAirborneAttack).State(airborneAttackHitState)
                    .If(isDirectControl & !isAirborneAttack).State(directState)
                    .If(isJumpFromSpotDelay).Stay()
                    .If(isMoveAxesForJumpFormSpot).State(jumpingFromSpotState)
                    .Otherwise(jumpingOnSpotState)
                )
                .State(jumpingOnSpotState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .OnEnter(reactions.AirborneReaction)
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isWaitForJump).Stay()
                    .If(isFalling).State(fallingState)
                    .If(isLandingDistance).State(landingOnSpotState)
                )
                .State(jumpingFromSpotState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .OnEnter(reactions.AirborneReaction)
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isWaitForJump).Stay()
                    .If(isFalling).State(fallingState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingFromRunState, _ => _
                    .OnEnter(reactions.JumpReaction)
                    .OnEnter(reactions.AirborneReaction)
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isWaitForJump).Stay()
                    .If(isFalling).State(fallingState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(jumpingOffState, _ => _        
                    .OnEnter(reactions.AirborneReaction)
                    .If(isJumpActivated & isJumpAllowed & isMovingForward & isJumpOffActionTimeWindow).State(jumpingFromRunState)
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isFalling).State(fallingState)
                    .If(isLandingDistance).State(landingState)
                )
                .State(fallingState, _ => _
                    .If(isLandingDistance).State(hardLandingState)
                )
                .State(landingState, _ => _
                    .OnExit(c => reactions.LandReaction(c.History.LastFallHeight))
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isAirborne).Stay()
                    .If(isFalling).State(hardLandingState)
                    .If(isSprintActive & isSprintAllowed).State(sprintState)
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingState)
                    .Otherwise(standingState)
                )
                .State(landingOnSpotState, _ => _
                    .OnExit(c => reactions.LandReaction(c.History.LastFallHeight))
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isAirborne).Stay()
                    .If(isFalling).State(hardLandingState)
                    .If(isSprintActive & isSprintAllowed).State(sprintState)
                    .If(isMoveAxesHigh | !isSpeedForStanding).State(walkingState)
                    .Otherwise(standingState)
                )
                .State(hardLandingState, _ => _
                    .OnExit(c => reactions.LandReaction(c.History.LastFallHeight))
                    .If(isAirborne).Stay()
                    .Otherwise(postHardLandingState)
                )
                .State(postHardLandingState, _ => _
                    .If(c => c.StateElapsedTime > c.Stats.HardLandingStunTime).State(standingState)
                )
                .State(slippingState, _ => _
                    .OnEnter(reactions.SlippingStartReaction)    
                    .OnExit(reactions.SlippingEndReaction)    
                    .If(isAirborneAttack).State(airborneAttackHitState)
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(isJumpActivated & isJumpAllowed & isMovingForward).State(jumpingFromRunState)
                    .If(isJumpActivated & isJumpAllowed).State(jumpingFromSpotDelayState)
                    .If(isSlippingStop).State(standingState)
                )
                .State(directState, _ => _
                    .OnEnter(c => reactions.StopAll())
                    .If(isFalling).State(fallingState)
                    .If(isAirborneAttack & isAirborneAttackHitDistance).State(airborneAttackHitState)
                    .If(isAirborneAttack & isAirborneAttackSticking).State(airborneAttackHitState)
                    .If(isAirborneAttack).State(airborneAttackBgnState)
                    .If(isAirborne & isJumpingOffDistance).State(jumpingOffState)
                    .If(!isDirectControl).State(standingState)
                    .If(isJumpActivated & isJumpAllowed).State(jumpingFromSpotDelayState)
                )
                .State(airborneAttackBgnState, _ => _
                    .OnEnter(c => reactions.StopAll())
                    .If(isAirborneAttackHitDistance).State(airborneAttackHitState)
                    .If(isAirborneAttackSticking).State(airborneAttackHitState)
                    .If(isAirborneAttackOff).State(standingState)
                )
                .State(airborneAttackHitState, _ => _
                    .OnEnter(c => reactions.AirborneAttackHit())
                    .OnExit(c => reactions.AirborneAttackHitEnd(c.History.LastFallHeight))
                    .If(isAirborneAttackOff).State(standingState)
                )
                .State(cheatState, _ => _
                    .OnEnter(c => reactions.StopAll())
                    .If(isDirectControl).State(directState)
                    .If(isCheatActivated).State(postCheatState)
                )
                .State(postCheatState, _ => _
                    .If(c => c.StateElapsedTime > 0.1).State(standingState)
                )
                ///! Don't forget to adjust `StatesRoughCountRoundedUp` value, when adding/removing items from here!
                .Build(context, null, dumpLoggerProvider);
        }
        public static int StatesRoughCountRoundedUp = 23; //actually 21

        private static bool SlippingCondition(CharacterStateMachineContext c)
        {
            float speed = c.Body_Deprecated.Velocity.Magnitude;
            return c.Environment.SlopeFactor() > c.Stats.SlippingSlopeFactorThreshold &&
                   Vector2.Dot(c.Body_Deprecated.Velocity.Horizontal.normalized * speed, c.Environment.SlopeDirection) > c.Stats.SlippingStartSpeedThreshold;
        }


//        private static bool CheckSticking(CharacterStateMachineContext c, float hitAngleCos)
//        {
//            var movingDir = c.Body_Deprecated.Velocity.Horizontal.normalized;
//            foreach (var contact in c.Environment.Contacts)
//            {
//                if (contact.ObjectType == ContactPointObjectType.Actor)
//                {
//                    if (contact.Location == ContactPointLocation.Side && Vector2.Dot(contact.Point.Horizontal - c.Body_Deprecated.Position.Horizontal.normalized, movingDir) > hitAngleCos)
//                        return true;
//                }            
//            }
//            return false;
//        }
        
        private static bool CheckAirborneAttackSticking(CharacterStateMachineContext c, float hitAngleCos)
        {
            var movingDir = c.Body_Deprecated.Velocity.Horizontal.normalized;
            for (int i=0, cnt = c.Environment.Contacts.Count; i < cnt; ++i)
            {
                var contact = c.Environment.Contacts[i];
                if (contact.ObjectType == ContactPointObjectType.Actor)
                {
                    if (contact.Location == ContactPointLocation.Side && Vector2.Dot(contact.Point.Horizontal - c.Body_Deprecated.Position.Horizontal.normalized, movingDir) > hitAngleCos)
                        return true;
                    if (contact.Location == ContactPointLocation.Bottom)
                        return true;
                }            
            }
            return false;
        }
    }
}