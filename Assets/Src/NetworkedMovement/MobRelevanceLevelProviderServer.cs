using System;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.NetworkedMovement;
using Assets.Src.RubiconAI;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using Src.Locomotion;
using Src.NetworkedMovement;
using UnityEngine;

namespace Assets.Src.Src.NetworkedMovement
{
    public class MobRelevanceLevelProviderServer : ILocomotionRelevancyLevelProvider, IMobRelevanceProvider
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Transform _self;
        private readonly IRelevanceLevelProviderSettings _settings;
        private Vector3 _queryPosition;
        public float ClosestObserverDistance { get; private set; }

        private IBoolVisibilityProvider _boolVisibilityProvider;

        //#Dbg:
        public float Dbg_ClosestObserverDistance_Forced_DANGER
        {
            get
            {
                if (GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && GlobalConstsDef.DebugFlagsGetter.IsDebugMobsHard_DANGER(GlobalConstsHolder.GlobalConstsDef))
                    /**/CalculateRelevanceLevel(true);
                return ClosestObserverDistance;
            }
        }

        public MobRelevanceLevelProviderServer(Transform self, IRelevanceLevelProviderSettings settings, IBoolVisibilityProvider boolVisibilityProvider)
        {
            _self = self ?? throw new ArgumentNullException(nameof(self));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _boolVisibilityProvider = boolVisibilityProvider ?? throw new ArgumentNullException(nameof(boolVisibilityProvider));
        }
        
        public float RelevancyLevelForNetwork => CalculateRelevanceLevel();

        private readonly float _recalcTimeStepBase = 1f;
        private float _recalcNextTime;
        private float _cachedNetworkRelevanceLvl;

        private bool _warnedOnceAboutForcedArg;
        private float CalculateRelevanceLevel(bool forced_DANGER = false)
        {
            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.CalculateRelevanceLevel");

            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.Begin");
            if (forced_DANGER)
                if (!_warnedOnceAboutForcedArg)
                {
                    _warnedOnceAboutForcedArg = true;
                    Logger.IfError()?.Message($"#WARN: {nameof(CalculateRelevanceLevel)} is called with {nameof(forced_DANGER)} arg == true!  [ Expected only while debugging ]").Write();
                }
            LocomotionProfiler3.EndSample();

            if (!_self)
            {
                LocomotionProfiler3.EndSample(); // 1 of 3
                return -1;
            }

            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.Now");
            var now = Time.realtimeSinceStartup;
            LocomotionProfiler3.EndSample();

            if (!forced_DANGER && now < _recalcNextTime)
            {
                LocomotionProfiler3.EndSample(); // 2 of 3
                return _cachedNetworkRelevanceLvl;
            }

            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.Updater");
            
            // Ask main question: 
            var closestDist = (!_boolVisibilityProvider.IsClose)
                                    ? -1f
                                    : AIWorld.RelevancyGrid
                                     .GetClosestPopulatedCellDistance(_self.position.ToShared(),(Vector3.one * GlobalConstsHolder.GlobalConstsDef.VisibilityDistance).ToShared());

            LocomotionProfiler3.EndSample();

            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.Ifs");
            if (closestDist < 0)
            {
                if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.closestDist < 0");

                //A. Farthest:
                _cachedNetworkRelevanceLvl = -1;
                _recalcNextTime = now + _recalcTimeStepBase * 3;
                ClosestObserverDistance = float.MaxValue;

                LocomotionProfiler3.EndSample();
            }
            else if (closestDist > _settings.DistanceForMinRelevanceLevel)
            {
                if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.closestDist > _settings.DistanceForMinRelevanceLevel");

                //B. Mid dist:
                _cachedNetworkRelevanceLvl = -1;
                _recalcNextTime = now + _recalcTimeStepBase * 2;
                ClosestObserverDistance = closestDist;

                LocomotionProfiler3.EndSample();
            }
            else
            {
                if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## MobRelevanceLevelProvider.else");

                //C. Closest:
                _cachedNetworkRelevanceLvl = Mathf.InverseLerp(_settings.DistanceForMinRelevanceLevel, _settings.DistanceForMaxRelevanceLevel, ClosestObserverDistance);
                _recalcNextTime = now + _recalcTimeStepBase;
                ClosestObserverDistance = closestDist;

                LocomotionProfiler3.EndSample();
            }
            LocomotionProfiler3.EndSample(); // Ifs

            LocomotionProfiler3.EndSample(); // 3 of 3
            return _cachedNetworkRelevanceLvl;
        }
    }
}