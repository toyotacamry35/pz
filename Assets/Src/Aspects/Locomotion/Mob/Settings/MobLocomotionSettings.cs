using ColonyShared.SharedCode.Aspects.Locomotion;
using Src.Locomotion.Unity;

using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    /// <summary>
    /// Just an wrapper-getter over statical data from def
    /// </summary>
    public class MobLocomotionSettings :
        NavMeshGroundSensor.ISettings,
        LocomotionEnvironment.ISettings,
        MobCommitToAnimator.ISettings,
        LocomotionDamperNode.ISettings
    {
        private readonly MobLocomotionDef _def;
        private readonly MobLocomotionNetworkClientSettings _client;
        private readonly MobLocomotionNetworkServerSettings _server;

        public MobLocomotionSettings(MobLocomotionDef def)
        {
            _def = def;
            _client = new MobLocomotionNetworkClientSettings(def.Network.Target.Client);
            _server = new MobLocomotionNetworkServerSettings(def.Network.Target.Server);
        }

        public MobLocomotionNetworkServerSettings Server => _server;
        public MobLocomotionNetworkClientSettings Client => _client;

        float NavMeshGroundSensor.ISettings.RaycastGroundTolerance => _def.GroundSensor.Target.RaycastGroundTolerance;
        float LocomotionEnvironment.ISettings.MinAirborneTime => _def.Settings.Target.MinAirborneTime;
        float LocomotionEnvironment.ISettings.Gravity => _def.Constants.Target.Gravity;
        float MobCommitToAnimator.ISettings.MovementDirectionSmoothness => _def.Settings.Target.AnimatorDirectionSmoothness;
        float MobCommitToAnimator.ISettings.MovementSpeedSmoothness => _def.Settings.Target.AnimatorSpeedSmoothness;
        float MobCommitToAnimator.ISettings.AngularVelocitySmoothness => _def.Settings.Target.AnimatorAngularVelocitySmoothness;

        float MobCommitToAnimator.ISettings.AngularVelocityToTwist(float av) =>
            _def.Settings.Target.AnimatorAngularVelocityForMaxTwist != 0
                ? Clamp(av * Rad2Deg / _def.Settings.Target.AnimatorAngularVelocityForMaxTwist, -1, 1)
                : Sign(av);

        float MobCommitToAnimator.ISettings.MotionThreshold => _def.Settings.Target.AnimatorMotionThreshold;

    #region LocomotionDamperNode.ISettings

        public float DamperMinDeltaPosition => _client.DamperMinDeltaPosition; //#todo(when needed): it's should be mob-specific, so move it to mob stats.
        public float DamperMinDeltaRotationDeg => _client.DamperMinDeltaRotationDeg; //#todo(when needed): it's should be mob-specific, so move it to mob stats.
        public float DamperSmoothTime       => _client.DamperSmoothTime;
        public float ObjectMaxSpeed         => _def.Stats.Target.RunningSpeed;
        public float ObjectMinSpeed         => _def.Stats.Target.MinSpeed;
        public float DamperMaxSpeedFactor   => _client.DamperMaxSpeedFactor_TmpHere; //#todo(when needed): it's should be mob-specific, so move it to mob stats.:  "_def.Stats.Target.DamperMaxSpeedFactor;"
        public float ObjectMaxYawSpeedDeg   => Max(_def.Stats.Target.StandingYawSpeed,
                                                   _def.Stats.Target.WalkingYawSpeed, 
                                                   _def.Stats.Target.RunningYawSpeed,
                                                   _def.Stats.Target.JumpYawSpeed);
        public float ObjectMinYawSpeedDeg   => Min(_def.Stats.Target.StandingYawSpeed,
                                                   _def.Stats.Target.WalkingYawSpeed,
                                                   _def.Stats.Target.RunningYawSpeed,
                                                   _def.Stats.Target.JumpYawSpeed);

    #endregion LocomotionDamperNode.ISettings

    }
}