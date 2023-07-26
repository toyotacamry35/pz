using System;
using Src.Locomotion.Delegates;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using Vector2 = SharedCode.Utils.Vector2;

namespace Src.Locomotion
{
    /// <summary>
    /// Just an helper-calcers-&-getters-wrapper over statical data from defs: (constants, settings & stats)
    /// </summary>
    public class MobStatsProvider : IMobStatsProvider
    {
        private readonly ILocomotionConstants _constants;
        private readonly MobLocomotionSettingsDef _settings;
        private readonly MobLocomotionStatsDef _stats;

        public MobStatsProvider(
            MobLocomotionDef def, 
            ILocomotionConstants constants)
        {
            if(constants == null) throw new ArgumentException(nameof(constants));            
            if(def == null) throw new ArgumentException(nameof(def));            
           
            _constants = constants;
            _settings = def.Settings;
            _stats = def.Stats.Target;
            
            WalkingSpeed = WalkingSpeedImpl;
            RunningSpeed = RunningSpeedImpl;
            JumpVerticalImpulse = JumpVerticalImpulseImpl;
            JumpHorizontalImpulse = JumpHorizontalImpulseImpl;
            AirControlSpeed = AirControlSpeedImpl;
            AirControlAccel = AirControlAccelImpl;
            DodgeVelocity = DodgeVelocityImpl;
            AccelBySlope = AccelBySlopeImpl;
        }

        public SpeedByDirFn /*ICommonStatsProvider.*/WalkingSpeed { get; private set; }

        private float WalkingSpeedImpl(Vector2 input)
        {
            var angle = Mathf.Atan2(input.y, input.x);
            var value = input.magnitude;
            if (value < _constants.InputMoveThreshold)
                return 0;
            var minSpeed = _stats.MinSpeed;
            var walkingSpeed = LocomotionMath.CLerpRad(_stats.WalkingSpeedFwd, _stats.WalkingSpeedSide, _stats.WalkingSpeedBwd, angle);
            return Mathf.Lerp(minSpeed, walkingSpeed, Mathf.InverseLerp(_constants.InputMoveThreshold, 1, value));
        }

        public SpeedByDirFn /*IMobStatsProvider.*/RunningSpeed { get; private set; }

        private float RunningSpeedImpl(Vector2 input)
        {
            var angle = Mathf.Atan2(input.y, input.x);
            var value = input.magnitude;
            if (value < _constants.InputMoveThreshold)
                return 0;
            var minSpeed = _stats.MinSpeed;
            var runningSpeed = LocomotionMath.CLerpRad(_stats.RunningSpeed, _stats.WalkingSpeedSide, _stats.WalkingSpeedBwd, angle);
            return Mathf.Lerp(minSpeed, runningSpeed, Mathf.InverseLerp(_constants.InputMoveThreshold, 1, value));
        }

        float ICommonStatsProvider.StandingSpeedThreshold => _settings.StandingSpeedThreshold;

        public ImpulseByDirFn /*ICommonStatsProvider.*/JumpVerticalImpulse { get; private set; }

        private float JumpVerticalImpulseImpl(Vector2 input)
        {
            return _stats.JumpVerticalImpulse;
        }
        
        public ImpulseByDirFn /*ICommonStatsProvider.*/JumpHorizontalImpulse { get; private set; }

        private float JumpHorizontalImpulseImpl(Vector2 input)
        {
            var angle = Mathf.Atan2(input.y, input.x);
            return LocomotionMath.CLerpRad(_stats.JumpHorizontalImpulseFwd, _stats.JumpHorizontalImpulseSide, _stats.JumpHorizontalImpulseBwd, angle);
        }

        float ICommonStatsProvider.JumpFromSpotDelay => _settings.JumpFromSpotDelay;

        float ICommonStatsProvider.JumpFromRunSpeedThreshold => _settings.JumpFromRunSpeedThreshold;

        float ICommonStatsProvider.JumpOffDistance => _settings.JumpingOffDistance;

        float ICommonStatsProvider.JumpMinDuration => _settings.JumpMinDuration;

        float ICommonStatsProvider.LandingDuration => _settings.LandingDuration;

        public SpeedByDirAndTimeFn /*ICommonStatsProvider.*/AirControlSpeed { get; private set; }

        private float AirControlSpeedImpl(Vector2 input, float airborneTime) => _stats.AirControlSpeed;

        public AccelByTimeFn /*ICommonStatsProvider.*/AirControlAccel { get; private set; }

        private float AirControlAccelImpl(float airborneTime) => _settings.AirControlAccel.Target.Evaluate(airborneTime);
        
        float ICommonStatsProvider.StandingYawSpeed => _stats.StandingYawSpeed * Mathf.Deg2Rad;

        float ICommonStatsProvider.WalkingYawSpeed => _stats.WalkingYawSpeed * Mathf.Deg2Rad;
        
        float IMobStatsProvider.RunningYawSpeed => _stats.RunningYawSpeed * Mathf.Deg2Rad;
        
        float ICommonStatsProvider.JumpYawSpeed => _stats.JumpYawSpeed * Mathf.Deg2Rad;

        float IMobStatsProvider.JumpToTargetMinDistance => _stats.JumpToTargetMinDistance;
        float IMobStatsProvider.JumpToTargetMaxDistance => _stats.JumpToTargetMaxDistance;
        float IMobStatsProvider.JumpToTargetMaxHeight   => _stats.JumpToTargetMaxHeight;

        float ICommonStatsProvider.WalkingAccel => _stats.WalkingAccel;

        float IMobStatsProvider.RunningAccel => _stats.RunningAccel;

        float ICommonStatsProvider.Decel => _stats.Decel;

        public SpeedByTimeFn /*ICommonStatsProvider.*/DodgeVelocity { get; private set; }

        private float DodgeVelocityImpl(float time) => _settings.DodgeMotion.Target.Evaluate(time) * _stats.DodgeSpeed;

        public AccelBySlopeFn /*ICommonStatsProvider.*/AccelBySlope { get; private set; }

        float AccelBySlopeImpl(float slopeAngle) => 1;

        float IMobStatsProvider.DodgeDuration => _settings.DodgeMotion.Target.LastTime;

        float ICommonStatsProvider.ActionWhileJumpOffTimeWindow => _settings.ActionWhileJumpOffTimeWindow;

        float ICommonStatsProvider.ActionTriggerInHindsight => _settings.ActionTriggerInHindsight;

        float IMobStatsProvider.TowardsDirectionTolerance => _settings.TowardsDirectionTolerance * Mathf.Deg2Rad;

        float IMobStatsProvider.TurnOnSpotThreshold => _settings.TurnOnSpotThreshold * Mathf.Deg2Rad;

        float IMobStatsProvider.TurnOnRunThreshold => _settings.TurnOnRunThreshold * Mathf.Deg2Rad;
    }
}