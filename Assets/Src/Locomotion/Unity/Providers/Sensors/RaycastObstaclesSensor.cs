using UnityEngine;
using UnityEngine.Assertions;
using SVector2 = SharedCode.Utils.Vector2;
using SVector3 = SharedCode.Utils.Vector3;

namespace Src.Locomotion.Unity
{
    public class RaycastObstaclesSensor : ILocomotionObstacleSensor
    {
        private readonly float _verticalSlopeCos;
        private readonly float _colliderTolerance;
        private readonly float _minStairHeightFactor;
        private readonly ILocomotionCollider _collider;
        private readonly ILocomotionGroundSensor _groundSensor;

        public RaycastObstaclesSensor(ISettings settings, ILocomotionCollider collider, ILocomotionGroundSensor groundSensor)
        {
            Assert.IsNotNull(settings);
            Assert.IsNotNull(collider);
            Assert.IsNotNull(groundSensor);
            _collider = collider;
            _groundSensor = groundSensor;
            _verticalSlopeCos = Mathf.Cos(settings.VerticalSlopeAngle);
            _colliderTolerance = settings.ColliderTolerance;
            _minStairHeightFactor = settings.MinStairHeightFactor;
        }

        public ObstacleInfo DetectObstacle(LocomotionVector direction, float distance, float maxStepUp)
        {
            return DetectObstacle(
                position: _collider.OriginPoint,
                direction: direction,
                lookAheadDistance: distance,
                distanceToGround: _groundSensor.DistanceToGround,
                verticalSlopeCos: _verticalSlopeCos,
                colliderRadius: _collider.Radius,
                colliderTolerance: _colliderTolerance,
                layer: _collider.GroundLayerMask,
                stairMinHeight: _collider.Radius * _minStairHeightFactor,
                stairMaxHeight: maxStepUp
                );
        }

        private static readonly RaycastHit[] _hits = new RaycastHit[1];
        public static ObstacleInfo DetectObstacle(
            LocomotionVector position,
            LocomotionVector direction,
            float lookAheadDistance,
            float colliderRadius,
            float colliderTolerance,
            float stairMinHeight,
            float stairMaxHeight,
            float distanceToGround,
            float verticalSlopeCos,
            int layer
            )
        {
            var pos = (Vector3)LocomotionHelpers.LocomotionToWorldVector(position);
            var dir = (Vector3)LocomotionHelpers.LocomotionToWorldVector(direction);
            ObstacleInfo rv = new ObstacleInfo();
            
            rv.Pivot = position;
            
            // проверка наличия помехи и получение расстояния до него
            var probeRadius = colliderRadius - colliderTolerance;
            var fwdProbeStart = pos + new Vector3(0, colliderRadius, 0) + dir * colliderTolerance;
            var fwdProbeRay = new Ray(fwdProbeStart, dir);
            
            //RaycastHit fwdProbeHit;
            //if (!Physics.SphereCast(fwdProbeRay, probeRadius, out fwdProbeHit, lookAheadDistance, layer))
            if (0 == Physics.SphereCastNonAlloc(fwdProbeRay, probeRadius, _hits, lookAheadDistance, layer))
               return rv;

            var fwdProbeHit = _hits[0];
            var fwdProbeHitTangent = Vector3.Cross(fwdProbeHit.normal, new Vector3(-dir.z, 0, dir.x)).normalized;
            
            rv.Detected = true;
            rv.Distance = fwdProbeHit.distance;
            rv.HitPoint = LocomotionHelpers.WorldToLocomotionVector(fwdProbeHit.point);
            rv.HitNormal = LocomotionHelpers.WorldToLocomotionVector(fwdProbeHit.normal);
            rv.HitTangent = LocomotionHelpers.WorldToLocomotionVector(fwdProbeHitTangent);

            if (fwdProbeHitTangent.y <= 0)
                return rv;

            // проверка является ли помеха "ступенью", т.е. есть ли у него "плато" на высоте между minHeight и maxHeight от земли 
            var leftDirection = Vector3.Cross(Vector3.up, dir).normalized;
            var fwdProbeHitDistanceToRay = Vector3.Dot(Vector3.Cross(dir, fwdProbeHit.point - fwdProbeStart), leftDirection);
            var dwnProbeOffset = (fwdProbeHitDistanceToRay - colliderRadius - distanceToGround + stairMaxHeight + probeRadius);   // FIXME: не учитывается наклон probeDir от горизонтали
            var dwnProbeStart = fwdProbeHit.point + fwdProbeHitTangent * dwnProbeOffset;  
            var dwnProbeRay = new Ray(dwnProbeStart, Vector3.down);

//            if(debug!=null) debug.TrailValue = fwdProbeHit.point.y;
    
            //RaycastHit dwnProbeHit;
            //if (!Physics.SphereCast(dwnProbeRay, probeRadius, out dwnProbeHit, stairMaxHeight - stairMinHeight, layer))
            if (0 == Physics.SphereCastNonAlloc(dwnProbeRay, probeRadius, _hits, stairMaxHeight - stairMinHeight, layer))
                    return rv;

            var dwnProbeHit = _hits[0];
            var dwnProbeHitSlopeCos = dwnProbeHit.normal.y;
            if (dwnProbeHitSlopeCos < verticalSlopeCos) // угол наклона обнаруженной поверхности больше порогового => это не "ступенька"
                return rv;

            rv.IsStair = true;
            rv.StairPoint = LocomotionHelpers.WorldToLocomotionVector(dwnProbeHit.point);
            //rv.StairPoint = LocomotionHelpers.WorldToLocomotionVector(dwnProbeRay.GetPoint(dwnProbeHit.distance) - new Vector3(0,probeRadius,0));
            rv.StairHeight = dwnProbeHit.point.y - pos.y + distanceToGround;

            return rv;
        }
        
        public interface ISettings
        {
            float VerticalSlopeAngle { get; }
            float ColliderTolerance { get; }
            float MinStairHeightFactor { get; }
        }
    }
}