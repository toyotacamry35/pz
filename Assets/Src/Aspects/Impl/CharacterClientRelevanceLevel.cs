using System;
using System.Collections;
using Assets.Src.Shared;
using Assets.Src.Tools;
using JetBrains.Annotations;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Src.Aspects.Impl
{
    // Should be created only for authority client. Using by LocomotionN/wSender.
    public class CharacterClientRelevanceLevel : ILocomotionRelevancyLevelProvider, IDisposable
    {
        private static readonly Collider[] Buffer = new Collider[128];
        
        // --- ILocomotionNetworkRelevancyLevelProvider: ----------------------
        public float RelevancyLevelForNetwork { get; private set; }

        // --- Privates: ------------------------------------------------------
        private readonly IRelevanceLevelProviderSettings _settings;
        private readonly int _updateStepSec = 3;
        private readonly MonoBehaviour _player;
        private Coroutine _updateCoroutine;

        // --- API: ------------------------------------------------------

        public CharacterClientRelevanceLevel(MonoBehaviour player, [NotNull] IRelevanceLevelProviderSettings settings)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Assert.IsTrue(_settings.DistanceForMaxRelevanceLevel <= _settings.DistanceForMinRelevanceLevel);
            _updateCoroutine = _player.StartInstrumentedCoroutine(UpdateRelevancyCoroutine());
        }

        public void Dispose()
        {
            if (_updateCoroutine != null)
                _player.StopCoroutine(_updateCoroutine);
            _updateCoroutine = null;
        }
        
        // --- Privates: ------------------------------------------------------

        private IEnumerator UpdateRelevancyCoroutine()
        {
            yield return new WaitForSeconds(UnityEngine.Random.value);
            while (true)
            {
                var collidersCount = Physics.OverlapSphereNonAlloc(_player.transform.position, _settings.DistanceForMinRelevanceLevel, Buffer, PhysicsLayers.ActiveMask);
                RelevancyLevelForNetwork = CalculateRelevanceLevel(Buffer, collidersCount);
                yield return CoroutineAwaiters.GetTick(_updateStepSec);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private float CalculateRelevanceLevel(Collider[] colliders, int collidersCount)
        {
            if (LocomotionProfiler2.EnableProfile) LocomotionProfiler2.BeginSample("## CharacterClientRelevanceLevel.CalculateRelevanceLevel");

            if (_player == null || collidersCount == 0)
            {
                LocomotionProfiler2.EndSample(); // 1 of 2
                return -1;
            }

            float minDistanceSqr = float.MaxValue;
            var playerPosition = _player.transform.position;
            for  (int i = 0; i < collidersCount; ++i)
            {
                var trans = colliders[i].transform;
                if (PhysicsUtils.IsSameObject(trans, _player.transform))
                    continue;
            
                var distanceSqr = Vector3.SqrMagnitude(trans.position - playerPosition);
                if (distanceSqr < minDistanceSqr)
                    minDistanceSqr = distanceSqr;
            }
            var result = Mathf.InverseLerp(_settings.DistanceForMinRelevanceLevel, _settings.DistanceForMaxRelevanceLevel, Mathf.Sqrt(minDistanceSqr));
            LocomotionProfiler2.EndSample(); // 2 of 2
            return result;
        }
    }
}