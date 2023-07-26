using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Camera;
using Assets.Src.NetworkedMovement;
using Assets.Src.RubiconAI;
using JetBrains.Annotations;
using NLog;
using Src.Locomotion;
using UnityEngine;

namespace Src.NetworkedMovement
{
    public class MobRelevanceLevelProviderClient : IMobRelevanceProvider
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const float RecalcTimeStepBase = 1f;
        
        private readonly Transform _self;
        private readonly IRelevanceLevelProviderSettings _settings;
        private readonly IsVisibleByCameraDetector _visibilityDetector;
        private float _recalcNextTime;
        private float _closestObserverDistance;
        private bool _warnedOnceAboutForcedArg;

        public float ClosestObserverDistance => GetClosestObserverDistance();

        public float Dbg_ClosestObserverDistance_Forced_DANGER => GetClosestObserverDistance(GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && GlobalConstsDef.DebugFlagsGetter.IsDebugMobsHard_DANGER(GlobalConstsHolder.GlobalConstsDef));

        public MobRelevanceLevelProviderClient(Transform self, IRelevanceLevelProviderSettings settings, [CanBeNull] IsVisibleByCameraDetector visibilityDetector)
        {
            _self = self ?? throw new ArgumentNullException(nameof(self));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _visibilityDetector = visibilityDetector;
        }

        private float GetClosestObserverDistance(bool forced_DANGER = false)
        {
            if (!_self)
                return -1;

            if (forced_DANGER)
                if (!_warnedOnceAboutForcedArg)
                {
                    _warnedOnceAboutForcedArg = true;
                    Logger.Error(
                        $"#WARN: {nameof(CalculateObserverDistance)} is called with {nameof(forced_DANGER)} arg == true!  [ Expected only while debugging ]");
                }

            // когда игрок смотрит на моба, тот должен апдейтится с максимальной частотой
            if (_visibilityDetector != null && _visibilityDetector.IsVisible)
                return 0;
            
            var now = Time.realtimeSinceStartup;
            if (!forced_DANGER && now < _recalcNextTime)
                return _closestObserverDistance;
            var res = CalculateObserverDistance();
            _recalcNextTime = now + res.Delay;
            _closestObserverDistance = res.Distance;
            return _closestObserverDistance;
        }

        private (float Distance, float Delay) CalculateObserverDistance()
        {
            var closestDist = GameCamera.Camera ? Vector3.Distance(GameCamera.Camera.transform.position, _self.position) : -1;
            float delay;
            if (closestDist < 0)
                delay = RecalcTimeStepBase * 3;
            else if (closestDist > _settings.DistanceForMinRelevanceLevel)
                delay = RecalcTimeStepBase * 2;
            else
                delay = RecalcTimeStepBase;
            return (closestDist, delay);
        }
    }
}