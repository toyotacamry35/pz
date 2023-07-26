using System;
using Src.Locomotion.Delegates;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using Vector2 = SharedCode.Utils.Vector2;

namespace Src.Locomotion
{
    public class CharacterStatsProvider : ICharacterStatsProvider
    {
        private readonly ILocomotionConstants _constants;
        private readonly CharacterLocomotionSettingsDef _settings;
        private readonly CharacterLocomotionStatsDef _stats;
        private readonly CalcersCache.CalcerProxy _minSpeed; 
        private readonly CalcersCache.CalcerProxy _walkingSpeedFwd; 
        private readonly CalcersCache.CalcerProxy _walkingSpeedSide; 
        private readonly CalcersCache.CalcerProxy _walkingSpeedBwd; 
        private readonly CalcersCache.CalcerProxy _runningSpeedFwd; 
        private readonly CalcersCache.CalcerProxy _runningSpeedSide; 
        private readonly CalcersCache.CalcerProxy _runningSpeedBwd; 
        private readonly CalcersCache.CalcerProxy _blockingSpeedFwd; 
        private readonly CalcersCache.CalcerProxy _blockingSpeedSide; 
        private readonly CalcersCache.CalcerProxy _blockingSpeedBwd; 
        private readonly CalcersCache.CalcerProxy _sprintSpeedFwd; 
        private readonly CalcersCache.CalcerProxy _jumpVerticalImpulse; 
        private readonly CalcersCache.CalcerProxy _jumpHorizontalImpulseFwd; 
        private readonly CalcersCache.CalcerProxy _jumpHorizontalImpulseSide; 
        private readonly CalcersCache.CalcerProxy _jumpHorizontalImpulseBwd; 
        private readonly CalcersCache.CalcerProxy _slippingControlSpeed; 
        private readonly CalcersCache.CalcerProxy _airControlSpeed; 
        private readonly CalcersCache.CalcerProxy _standingYawSpeed; 
        private readonly CalcersCache.CalcerProxy _walkingYawSpeed; 
        private readonly CalcersCache.CalcerProxy _sprintYawSpeed; 
        private readonly CalcersCache.CalcerProxy _slipYawSpeed; 
        private readonly CalcersCache.CalcerProxy _jumpYawSpeed;

        public CharacterStatsProvider(
            CharacterLocomotionDef def, 
            ILocomotionConstants constants,
            CalcersCache calcersDoer)
        {
            if(calcersDoer == null) throw new ArgumentException(nameof(calcersDoer));            
            if(constants   == null) throw new ArgumentException(nameof(constants));            
            if(def         == null) throw new ArgumentException(nameof(def));            
           
            _constants = constants;
            _settings = def.Settings;
            _stats = def.Stats.Target;
            _minSpeed          = calcersDoer.Add(_stats.MinSpeed);
            _walkingSpeedFwd   = calcersDoer.Add(_stats.WalkingSpeedFwd);
            _walkingSpeedSide  = calcersDoer.Add(_stats.WalkingSpeedSide);
            _walkingSpeedBwd   = calcersDoer.Add(_stats.WalkingSpeedBwd);
            _runningSpeedFwd   = calcersDoer.Add(_stats.RunningSpeedFwd);
            _runningSpeedSide  = calcersDoer.Add(_stats.RunningSpeedSide);
            _runningSpeedBwd   = calcersDoer.Add(_stats.RunningSpeedBwd);
            _blockingSpeedFwd  = calcersDoer.Add(_stats.BlockingSpeedFwd);
            _blockingSpeedSide = calcersDoer.Add(_stats.BlockingSpeedSide);
            _blockingSpeedBwd  = calcersDoer.Add(_stats.BlockingSpeedBwd);
            _sprintSpeedFwd    = calcersDoer.Add(_stats.SprintSpeedFwd);
            _jumpVerticalImpulse       = calcersDoer.Add(_stats.JumpVerticalImpulse);
            _jumpHorizontalImpulseFwd  = calcersDoer.Add(_stats.JumpHorizontalImpulseFwd);
            _jumpHorizontalImpulseSide = calcersDoer.Add(_stats.JumpHorizontalImpulseSide);
            _jumpHorizontalImpulseBwd  = calcersDoer.Add(_stats.JumpHorizontalImpulseBwd);
            _slippingControlSpeed      = calcersDoer.Add(_stats.SlippingControlSpeed);
            _airControlSpeed   = calcersDoer.Add(_stats.AirControlSpeed);
            _standingYawSpeed  = calcersDoer.Add(_stats.StandingYawSpeed);
            _walkingYawSpeed   = calcersDoer.Add(_stats.WalkingYawSpeed);
            _sprintYawSpeed    = calcersDoer.Add(_stats.SprintYawSpeed);
            _slipYawSpeed      = calcersDoer.Add(_stats.SlipYawSpeed);
            _jumpYawSpeed      = calcersDoer.Add(_stats.JumpYawSpeed);

            WalkingSpeed = WalkingSpeedImpl;
            BlockingSpeed = BlockingSpeedImpl;
            SprintSpeed = SprintSpeedImpl;
            JumpVerticalImpulse = JumpVerticalImpulseImpl;
            JumpHorizontalImpulse = JumpHorizontalImpulseImpl;
            AirControlSpeed = AirControlSpeedImpl;
            AirControlAccel = AirControlAccelImpl;
            SlipSpeed = SlipSpeedImpl;
            SlipAccel = SlipAccelImpl;
            SlippingSpeed = SlippingSpeedImpl;
            AccelBySlope = AccelBySlopeImpl;
            AirborneAttackControlSpeed = AirborneAttackControlSpeedImpl;
            AirborneAttackControlAccel = AirborneAttackControlAccelImpl;
        }

        public SpeedByDirFn /*ICommonStatsProvider.*/WalkingSpeed { get; }

        private float WalkingSpeedImpl(Vector2 input)
        {
            var angle = Atan2(input.y, input.x);
            var value = Clamp01(input.magnitude);
            if (value < _constants.InputMoveThreshold)
                return 0;
            var minSpeed = _minSpeed;
            var walkingSpeed = LocomotionMath.CLerpRad(_walkingSpeedFwd, _walkingSpeedSide, _walkingSpeedBwd, angle);
            if (value < _constants.InputRunThreshold)
                return Lerp(minSpeed, walkingSpeed,
                    InverseLerp(_constants.InputMoveThreshold, _constants.InputRunThreshold, value));
            var runningSpeed = LocomotionMath.CLerpRad(_runningSpeedFwd, _runningSpeedSide, _runningSpeedBwd, angle);
            return Lerp(walkingSpeed, runningSpeed, InverseLerp(_constants.InputRunThreshold, 1, value));
        }

        public SpeedByDirFn /*ICharacterStatsProvider.*/BlockingSpeed { get; private set; }
        
        private readonly SpeedByDirFn _blockingSpeedFn;
        
        float BlockingSpeedImpl(Vector2 input)
        {
            var angle = Atan2(input.y, input.x);
            var value = Clamp01(input.magnitude);
            if (value < _constants.InputStandingThreshold)
                return 0;
            return LocomotionMath.CLerpRad(_blockingSpeedFwd, _blockingSpeedSide, _blockingSpeedBwd, angle);
        }

        public SpeedByDirFn /*ICharacterStatsProvider.*/SprintSpeed { get; private set; }

        private float SprintSpeedImpl(Vector2 input)
        {
            var angle = Atan2(input.y, input.x);
            var value = Clamp01(input.magnitude);
            if (value < _constants.InputStandingThreshold)
                return 0;
            return LocomotionMath.CLerpRad(_sprintSpeedFwd, _runningSpeedSide, _runningSpeedBwd, angle);
        }

        float ICommonStatsProvider.StandingSpeedThreshold => _settings.StandingSpeedThreshold;

        public ImpulseByDirFn /*ICommonStatsProvider.*/JumpVerticalImpulse { get; private set; }

        private float JumpVerticalImpulseImpl(Vector2 input)
        {
            return _jumpVerticalImpulse;
        }        
        
        public ImpulseByDirFn /*ICommonStatsProvider.*/JumpHorizontalImpulse { get; private set; }

        private float JumpHorizontalImpulseImpl(Vector2 input)
        {
            var angle = Atan2(input.y, input.x);
            return LocomotionMath.CLerpRad(_jumpHorizontalImpulseFwd, _jumpHorizontalImpulseSide,
                _jumpHorizontalImpulseBwd, angle);
        }

        float ICommonStatsProvider.JumpFromSpotDelay => _settings.JumpFromSpotDelay;

        float ICommonStatsProvider.JumpFromRunSpeedThreshold => _settings.JumpFromRunSpeedThreshold;

        float ICommonStatsProvider.JumpOffDistance => _settings.JumpingOffDistance;

        float ICommonStatsProvider.JumpMinDuration => _settings.MinAirborneTime * 2;

        float ICommonStatsProvider.LandingDuration => _settings.LandingDuration;

        public SpeedByDirAndTimeFn /*ICommonStatsProvider.*/AirControlSpeed { get; private set; }

        private float AirControlSpeedImpl(Vector2 input, float airborneTime) => _airControlSpeed;

        public AccelByTimeFn /*ICommonStatsProvider.*/AirControlAccel { get; private set; }

        private float AirControlAccelImpl(float airborneTime) => _settings.AirControlAccel.Target.Evaluate(airborneTime);
        
        float ICharacterStatsProvider.HardLandingStunTime => _settings.HardLandingStunTime;

        float ICommonStatsProvider.StandingYawSpeed => _standingYawSpeed * Deg2Rad;

        float ICommonStatsProvider.WalkingYawSpeed => _walkingYawSpeed * Deg2Rad;
        
        float ICharacterStatsProvider.SprintYawSpeed => _sprintYawSpeed * Deg2Rad;
        
        float ICharacterStatsProvider.SlipYawSpeed => _slipYawSpeed * Deg2Rad;

        float ICommonStatsProvider.JumpYawSpeed => _jumpYawSpeed * Deg2Rad;

        float ICommonStatsProvider.WalkingAccel => _settings.WalkingAccel;

        float ICharacterStatsProvider.SprintAccel => _settings.SprintAccel;

        float ICommonStatsProvider.Decel => _settings.Decel;

        float ICharacterStatsProvider.SingleStepTime => _settings.SingleStepTime;

        float ICharacterStatsProvider.StickHitAngle => _settings.StickHitAngle * Deg2Rad;

        public SlipSpeedBySlopFn /*ICharacterStatsProvider.*/SlipSpeed { get; private set; }

        private float SlipSpeedImpl(float slope) => 
            slope >= _settings.SlipSlope * Deg2Rad ? _settings.SlipSpeed.Target.Evaluate(InverseLerp(_settings.SlipSlope * Deg2Rad, HalfPi, slope)) : 0;

        public SlipAccelBySlopeFn /*ICharacterStatsProvider.*/SlipAccel { get; private set; }

        private float SlipAccelImpl(float slope) => 
            slope >= _settings.SlipSlope * Deg2Rad ? _settings.SlipAccel.Target.Evaluate(InverseLerp(_settings.SlipSlope * Deg2Rad, HalfPi, slope)) : 0;

        float ICharacterStatsProvider.SlippingSlopeFactorThreshold => Sin(_settings.SlipSlope * Deg2Rad);

        float ICharacterStatsProvider.SlippingStartSpeedThreshold => _settings.SlippingStartSpeedThreshold;

        float ICharacterStatsProvider.SlippingStopSpeedThreshold => _settings.SlippingStopSpeedThreshold;

        float ICharacterStatsProvider.SlippingTimeThreshold => _settings.SlippingTimeThreshold;

        public SpeedByDirFn /*ICharacterStatsProvider.*/SlippingSpeed { get; private set; }

        private float SlippingSpeedImpl(Vector2 dir) => _slippingControlSpeed * Clamp01(SharedHelpers.Abs(dir.y));

        float ICharacterStatsProvider.SlippingAccel => _settings.SlippingAccel;

        float ICharacterStatsProvider.SlippingDecel => _settings.SlippingDecel;
        
        float ICharacterStatsProvider.CheatSpeed => _settings.CheatSpeed;

        float ICharacterStatsProvider.MaxWalkingSlope => _settings.MaxWalkingSlope * Deg2Rad;

        float ICommonStatsProvider.ActionWhileJumpOffTimeWindow => _settings.ActionWhileJumpOffTimeWindow;

        float ICommonStatsProvider.ActionTriggerInHindsight => _settings.ActionTriggerInHindsight;

        public AccelBySlopeFn /*ICommonStatsProvider.*/AccelBySlope { get; private set; }

        float ICharacterStatsProvider.AirborneAttackHitDistance => _settings.AirborneAttackHitDistance;
        
        public SpeedByDirAndTimeFn /*ICharacterStatsProvider.*/AirborneAttackControlSpeed { get; }

        private float AirborneAttackControlSpeedImpl(Vector2 dir, float time) => _settings.AirborneAttackControlSpeed;
        
        public AccelByTimeFn /*ICharacterStatsProvider.*/AirborneAttackControlAccel { get; }
        
        private float AirborneAttackControlAccelImpl(float time) => _settings.AirborneAttackControlAccel.Target.Evaluate(time);
        
        float AccelBySlopeImpl(float slope) => 
            slope <= _settings.MaxWalkingSlope * Deg2Rad ? _settings.AccelBySlope.Target.Evaluate(InverseLerp(0, _settings.MaxWalkingSlope * Deg2Rad, Math.Max(slope,0))) : 0;
    }
}