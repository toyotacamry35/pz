using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using UnityEngine;

namespace Src.Locomotion.Unity
{
    public class RaycastOnGroundAligner : MonoBehaviour, ILocomotionPipelineCommitNode
    {
        [Tooltip("Опорные точки")][SerializeField] private Vector3[] _feetPoints;
        [Tooltip("Расстояние каста")][SerializeField] private float _castDistance = 1;
        [Tooltip("Радиус опорных точек")][SerializeField] private float _castRadius = 0.2f;
        [Tooltip("Делает изменение наклона более плавным")][SerializeField] private float _smoothness = 0.1f;
        [Tooltip("Максимальный угол наклона вперд/назад")][SerializeField] private float _pitchLimit = 30;
        [Tooltip("Максимальный угол наклона вбок")][SerializeField] private float _rollLimit = 30;

        private Transform _reference;
        private LayerMask _layers;
        private Vector3[] _groundPoints;
        private Vector3 _velocity;

        public RaycastOnGroundAligner Init(Transform reference, LayerMask layers)
        {
            _reference = reference;
            _layers = layers;
            _groundPoints = new Vector3[_feetPoints.Length];
            return this;
        }

        private readonly RaycastHit[] _hits = new RaycastHit[1];
        private bool PlaneCast([NotNull] Transform reference, out Plane plane)
        {
            plane = new Plane(reference.up, reference.position);
            var vertical = plane.normal;

            for (var i = 0; i < _feetPoints.Length; i++)
            {
                var feetPoint = _feetPoints[i];
                var worldFeetPoint = reference.TransformPoint(feetPoint);
                var ray = new Ray(worldFeetPoint + vertical * (_castDistance + _castRadius), -vertical);
                RaycastHit hit;
                //if (_castRadius > 0.01 && Physics.SphereCast(ray, _castRadius, out hit, _castDistance * 2, _layers) ||
                //      _castRadius <= 0.01 && Physics.Raycast(ray, out hit, _castDistance * 2, _layers) )
                if ( _castRadius > 0.01 && 0 < Physics.SphereCastNonAlloc(ray, _castRadius, _hits, _castDistance * 2, _layers) 
                  || _castRadius <= 0.01 && 0 < Physics.RaycastNonAlloc(ray, _hits, _castDistance * 2, _layers) )
                    _groundPoints[i] = ray.GetPoint(_hits[0].distance + _castRadius);
                else
                    _groundPoints[i] = ray.GetPoint(_castDistance * 2 + _castRadius);
            }
            
            if (FitHeightPlane(_groundPoints, out plane))
            {
//                vertical = plane.normal;
//                var angleLimitCos = Mathf.Cos(_angleLimit * Mathf.Deg2Rad);
//                if (Vector3.Dot(vertical, reference.up) < angleLimitCos)
//                {
//                    vertical = reference.InverseTransformDirection(vertical);
//                    var hor = new Vector2(vertical.x, vertical.z).normalized * Mathf.Sin(_angleLimit * Mathf.Deg2Rad);
//                    vertical = reference.TransformDirection(new Vector3(hor.x, angleLimitCos, hor.y));
//                }
                return true;
            }

            return false;
        }

        static bool FitHeightPlane(Vector3[] points, out Plane plane)
        {
            Vector3 mean = Vector3.zero;
            for (int i = 0; i < points.Length; ++i)
                mean += points[i];
            mean /= points.Length;

            // Compute the linear system matrix and vector elements.
            float xxSum = 0, xzSum = 0, xySum = 0, zzSum = 0, zySum = 0;
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 diff = points[i] - mean;
                xxSum += diff.x * diff.x;
                xzSum += diff.x * diff.z;
                xySum += diff.x * diff.y;
                zzSum += diff.z * diff.z;
                zySum += diff.z * diff.y;
            }

            // Solve the linear system.ssss
            float det = xxSum*zzSum - xzSum*xzSum;
            if (!Mathf.Approximately(det,0))
            {
                // Plane: y = mean.y + a∗(x − mean.x) + b∗(z − mean.z).
                var a = (zzSum * xySum - xzSum * zySum) / det;
                var b = (xxSum * zySum - xzSum * xySum) / det;
                plane = new Plane(new Vector3(-a, 1, -b).normalized, mean);
                return true;
            }
            
            plane = new Plane(mean, Vector3.up);
            return false;
        }

        bool ILocomotionPipelineCommitNode.IsReady => true;

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            if(!_reference)
                return;

            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: m)RaycastOnGroundAligner");

            Plane plane;
            if (!inVars.Flags.Any(LocomotionFlags.Airborne | LocomotionFlags.Jumping | LocomotionFlags.Falling) && PlaneCast(_reference, out plane))
            {
                var vertical = Vector3.SmoothDamp(transform.up, plane.normal, ref _velocity, _smoothness, float.MaxValue, dt);
                var forward = Vector3.ProjectOnPlane(_reference.forward, vertical).normalized;
                var rotation = Quaternion.LookRotation(forward, vertical);
                var angleLimitCos = Mathf.Cos(Mathf.Min(_pitchLimit, _rollLimit) * Mathf.Deg2Rad);
                if (Vector3.Dot(vertical, _reference.up) < angleLimitCos)
                {
                    var euler = rotation.eulerAngles;
                    euler.x = Mathf.Clamp(Mathf.DeltaAngle(0, euler.x), -_pitchLimit, _pitchLimit);
                    euler.z = Mathf.Clamp(Mathf.DeltaAngle(0, euler.z), -_rollLimit, _rollLimit);
                    rotation = Quaternion.Euler(euler);
                }
                transform.rotation =  rotation;
            }

            LocomotionProfiler.EndSample();
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var feetPoint in _feetPoints)
            {
                var pt = transform.TransformPoint(feetPoint);
                Gizmos.color = Color.green;
                Gizmos.DrawCube(pt, new Vector3(_castRadius, 0, _castRadius));
                var up = transform.up * _castDistance;
                Gizmos.DrawLine(pt - up, pt + up);
            }
        }
    }
}