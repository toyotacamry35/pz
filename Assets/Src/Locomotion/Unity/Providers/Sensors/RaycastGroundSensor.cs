using System.Collections.Generic;
using NLog;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;
using static UnityEngine.Mathf;
using SVector2 = SharedCode.Utils.Vector2;

namespace Src.Locomotion.Unity
{
    public class RaycastGroundSensor : ILocomotionGroundSensor, ILocomotionUpdateable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static RaycastHit[] HitsBuffer = new RaycastHit[32];
        private static readonly (Vector3,float)[] NormalsBuffer = new (Vector3,float)[32];

        private readonly ISettings _settings;
        private readonly ILocomotionCollider _collider;
        private bool _longRangeCheck;
        private float _longRangeCheckThrottle;

        public RaycastGroundSensor(ISettings settings, ILocomotionCollider collider)
        {
            _settings = settings;
            _collider = collider;
        }

        public bool OnGround { get; private set; }

        public LocomotionVector GroundNormal { get; private set; }

        public float DistanceToGround { get; private set; }

        public bool HasGroundContact { get; private set; }

        void ILocomotionUpdateable.Update(float dt)
        {
            float dist;
            if (_longRangeCheck)
            {
                if ((_longRangeCheckThrottle += dt) < 0.1f)
                    return;
                _longRangeCheckThrottle = 0;
                dist = _settings.RaycastDistanceLong;
            }
            else
                dist = _settings.RaycastDistance;

            if (UpdateDistanceToGround(dist))
            {
                if (_longRangeCheck && DistanceToGround <= _settings.RaycastDistance)
                    _longRangeCheck = false;
            }
            else if (!_longRangeCheck)
            {
                _longRangeCheck = true;
                _longRangeCheckThrottle = 0;
                UpdateDistanceToGround(_settings.RaycastDistanceLong);
            }
        }

        private bool UpdateDistanceToGround(float rayLength)
        {
            DistanceToGround = float.MaxValue;
            GroundNormal = LocomotionVector.Up;
            OnGround = false;
            HasGroundContact = false;
            bool rv = false;
            
            if (SphereCast(rayLength, out float distance, out var normal))
            {
                DistanceToGround = distance;
                OnGround = DistanceToGround <= _settings.RaycastGroundTolerance;
                rv = true;
                foreach (var contact in _collider.Contacts)
                    if (contact.ObjectType == ContactPointObjectType.Ground)
                    {
                        HasGroundContact = true;
                        break;
                    }
            }
            
            if (!normal.ApproximatelyZero())
                GroundNormal = normal;

            DebugAgent?.Gather(_collider);
            return rv;
        }

        private bool SphereCast(float rayLength, out float distance, out LocomotionVector normal)
        {
            var rayOrigin = new LocomotionVector(_collider.OriginPoint.Horizontal, _collider.OriginPoint.Vertical + _settings.RaycastOffset);
            var ray = new Ray(LocomotionHelpers.LocomotionToWorldVector(rayOrigin).ToUnity(), Vector3.down);
            var rayFullLength = rayLength + _settings.RaycastOffset;

            DebugAgent.Set(GroundRaycastStart, rayOrigin);

            if (rayFullLength > 150)
                LocomotionLogger.Default.IfError()?.Message($"GroundSensor rayLength is too big {rayFullLength}").Write();

            var verticalAngleCos = Cos(_settings.VerticalSlopeAngle);
            var layerMask = _collider.GroundLayerMask;
            var radius = _collider.Radius;
            var bottom = _collider.OriginPoint.Vertical;
            var hitsCount = Physics.SphereCastNonAlloc(ray.origin + Vector3.up * radius, radius, Vector3.down, HitsBuffer, rayLength, layerMask, QueryTriggerInteraction.Ignore);
            float minDistance = float.MaxValue;
            int normalsCount = 0;
            for (int i = 0; i < hitsCount; ++i)
            {
                var hitInfo = HitsBuffer[i];
                if (_collider.IsSame(hitInfo.collider))
                    continue;
                    
                var dist = bottom - hitInfo.point.y;
                var isVertical = hitInfo.normal.y <= verticalAngleCos;
                if (!isVertical)
                {
                    if (dist < minDistance)
                        minDistance = dist;
                    NormalsBuffer[normalsCount++] = (hitInfo.normal, dist);
                }
            }
            float delta = _settings.NormalSmoothingDistance;
            var normalSum = Vector3.zero;
            var weightSum = 0f;
            for (int i = 0; i < normalsCount; ++i)
            {
                var w = Clamp01((minDistance + delta - NormalsBuffer[i].Item2) / delta);
                normalSum += NormalsBuffer[i].Item1 * w;
                weightSum += w;
            }
            normal = weightSum > 0 ? LocomotionHelpers.WorldToLocomotionVector(normalSum / weightSum).Normalized : LocomotionVector.Zero;
            distance = minDistance;
            return distance < float.MaxValue;
        }

        public interface ISettings
        {
            float RaycastDistance { get; }
            float RaycastDistanceLong { get; }
            float RaycastOffset { get; }
            float RaycastGroundTolerance { get; }
            float SphereCastSlopeAngle { get; } // радианы
            float VerticalSlopeAngle { get; } // радианы
            float NormalSmoothingDistance { get; }
        }
    }
}
