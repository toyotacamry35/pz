using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Src.Locomotion.Unity;

namespace Src.Locomotion
{
    public class CharacterLocomotionSettings :
        RaycastGroundSensor.ISettings,
        RaycastObstaclesSensor.ISettings,
        LocomotionEnvironment.ISettings,
        CharacterCommitToAnimator.ISettings,
        LocomotionCharacterBody.ISettings,
        LocomotionSpeedLimiter.ISettings,
        LocomotionSlideDownFromActorsNode.ISettings,
        LocomotionColliderResizer.ISettings,
        LocomotionCharacterDepenetrationNode.ISettings,
        LocomotionCharacterCollisionNode.ISettings
    {
        private readonly CharacterLocomotionDef _def;

        public CharacterLocomotionSettings(CharacterLocomotionDef def)
        {
            _def = def;
        }

        public CharacterLocomotionNetworkServerSettings Server => new CharacterLocomotionNetworkServerSettings(_def.Network.Target);
        
        public CharacterLocomotionNetworkClientSettings Client => new CharacterLocomotionNetworkClientSettings(_def.Network.Target);

        float LocomotionCharacterBody.ISettings.MaxWalkingSlopeAngle => _def.Settings.Target.MaxWalkingSlope; // градусы!
        float RaycastGroundSensor.ISettings.RaycastDistance => _def.GroundSensor.Target.RaycastDistance;
        float RaycastGroundSensor.ISettings.RaycastDistanceLong => _def.GroundSensor.Target.RaycastDistanceLong;
        float RaycastGroundSensor.ISettings.RaycastOffset => _def.GroundSensor.Target.RaycastOffset;
        float RaycastGroundSensor.ISettings.RaycastGroundTolerance => _def.GroundSensor.Target.RaycastGroundTolerance;
        float RaycastGroundSensor.ISettings.SphereCastSlopeAngle => _def.Settings.Target.MaxWalkingSlope * SharedHelpers.Deg2Rad;
        float RaycastGroundSensor.ISettings.VerticalSlopeAngle => _def.Constants.Target.VerticalSlopeAngle * SharedHelpers.Deg2Rad;
        float RaycastGroundSensor.ISettings.NormalSmoothingDistance => _def.GroundSensor.Target.NormalSmoothingDistance;
        float RaycastObstaclesSensor.ISettings.ColliderTolerance => _def.ObstaclesSensor.Target.ColliderTolerance;
        float RaycastObstaclesSensor.ISettings.MinStairHeightFactor => _def.ObstaclesSensor.Target.MinStairHeightFactor;
        float RaycastObstaclesSensor.ISettings.VerticalSlopeAngle => _def.Constants.Target.VerticalSlopeAngle * SharedHelpers.Deg2Rad;
        float LocomotionEnvironment.ISettings.MinAirborneTime => _def.Settings.Target.MinAirborneTime;
        float LocomotionEnvironment.ISettings.Gravity => _def.Constants.Target.Gravity;
        float CharacterCommitToAnimator.ISettings.MovementDirectionSmoothness => _def.Settings.Target.AnimatorDirectionSmoothness;
        float CharacterCommitToAnimator.ISettings.MovementSpeedSmoothness => _def.Settings.Target.AnimatorSpeedSmoothness;
        float CharacterCommitToAnimator.ISettings.MotionThreshold => _def.Settings.Target.AnimatorMotionThreshold;
        float LocomotionSpeedLimiter.ISettings.HorizontalSpeedLimit => _def.Settings.Target.HorizontalSpeedLimit;
        float LocomotionSpeedLimiter.ISettings.VerticalSpeedLimit => _def.Settings.Target.VerticalSpeedLimit;
        float LocomotionSlideDownFromActorsNode.ISettings.Accel => _def.Settings.Target.SlideDownFromActorsAccel;
        float LocomotionSlideDownFromActorsNode.ISettings.Speed => _def.Settings.Target.SlideDownFromActorsSpeed;
        float LocomotionColliderResizer.ISettings.BlendTime => _def.ColliderResizer.Target.BlendTime;
        IEnumerable<(LocomotionColliderResizer.Condition, LocomotionColliderResizer.Preset)> LocomotionColliderResizer.ISettings.Presets =>
            _def.ColliderResizer.Target.Presets.Select(x =>
                ( new LocomotionColliderResizer.Condition { WithFlags = x.WithFlags, WithoutFlags = x.WithoutFlags },
                  new LocomotionColliderResizer.Preset { Center = x.Center.ToUnity(), Height = x.Height, Enabled = x.Enabled, BlendTime = x.BlendTime } )
            );
        int LocomotionCharacterCollisionNode.ISettings.MaxIterations => _def.Constants.Target.ActorWithActorCollisionMaxIterations;
        float LocomotionCharacterCollisionNode.ISettings.DepenetrationOffset => _def.Constants.Target.ActorWithActorCollisionDepenetrationOffset;
        float LocomotionCharacterDepenetrationNode.ISettings.AdditionalOffset => _def.Constants.Target.ActorWithActorCollisionDepenetrationOffset;
    }
}
