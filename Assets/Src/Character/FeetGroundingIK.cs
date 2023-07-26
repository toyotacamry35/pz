using System;
using Plugins.DebugDrawingExtension;
using Src.Locomotion.Unity;
using UnityEngine;

namespace Assets.Src.Character
{
    public class FeetGroundingIK : MonoBehaviour
    {
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer("FeetGroundingIK");

        [Header("Raycast")]
        [SerializeField] private float _raycastUpOffset = 0.5f;
        [SerializeField] private float _raycastDownDistance = 0.5f;
        [SerializeField] private float _raycastDistanceThreshold = 0.1f;
        [SerializeField] private float _skinWidthCompensation = 0.0f;
        [SerializeField] private LayerMask _groundLayers;
        [SerializeField] private float _normalConstraint = 90;
        [Header("Feet")]
        [SerializeField] private float _feetSmoothTime = 0.1f;
        [SerializeField] private float _feetPositionFactor = 1;
        [SerializeField] private float _feetRotationFactor = 1;
        [SerializeField] private Range _feetConstraint = new Range { Min = -0.5f, Max = 0.5f };
        [Header("Pelvis")]
        [SerializeField] private float _pelvisOffset = 0;
        [SerializeField] private float _pelvisSmoothTime = 0.1f;
        [SerializeField] private float _pelvisFactor = 1;
        [SerializeField] private Range _pelvisConstraint = new Range { Min = -0.5f, Max = 0.5f };
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _leftFootAnimVariable = "LeftFootIK";
        [SerializeField] private string _rightFootAnimVariable = "RightFootIK";
        
        private Pose _rightFootIkPose;
        private Pose _leftFootIkPose;
        private bool _hasLastPelvisOffset;
        private float _lastPelvisOffset;
        private float _pelvisVelocity;
        private int _leftFootAnimVariableIdx;
        private int _rightFootAnimVariableIdx;
        private float _lastRightFootPositionY;
        private float _rightFootVelocityY;
        private float _lastLeftFootPositionY;
        private float _leftFootVelocityY;
        private float _normalMaxAngleCos;
        private float _normalMaxAngleSin;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[16];
#if UNITY_EDITOR            
        private float _prevNormalConstraint;
#endif
        
        private float OriginY => transform.position.y - _skinWidthCompensation;
        
        private void Awake()
        {
            _animator = _animator ? _animator : GetComponent<Animator>();
            if (!_animator) throw new NullReferenceException($"No animator component on {transform.FullName()}");
            _leftFootAnimVariableIdx = Animator.StringToHash(_leftFootAnimVariable);
            _rightFootAnimVariableIdx = Animator.StringToHash(_rightFootAnimVariable);
            OnNormalConstraintChanged();
        }


        private void Update()
        {
#if UNITY_EDITOR            
            if (!Mathf.Approximately(_normalConstraint, _prevNormalConstraint))
            {
                _prevNormalConstraint = _normalConstraint;
                OnNormalConstraintChanged();
            }
#endif            
            var rightFootTransform = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
            _rightFootIkPose = FootPositionSolver(new Pose(rightFootTransform.position, rightFootTransform.rotation), _rightFootIkPose.Valid);
            var leftFootTransform = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            _leftFootIkPose = FootPositionSolver(new Pose(leftFootTransform.position, leftFootTransform.rotation), _leftFootIkPose.Valid);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            float rightFootWeight = _animator.GetFloat(_rightFootAnimVariableIdx);
            float leftFootWeight = _animator.GetFloat(_leftFootAnimVariableIdx);
            float pelvisWeight = Mathf.Max(rightFootWeight, leftFootWeight); //_animator.GetFloat(_pevisAnimVariableIdx);
            
            MovePelvis(_leftFootIkPose, _rightFootIkPose, pelvisWeight);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight * _feetPositionFactor);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight * _feetRotationFactor);
            MoveFootToIkPoint(AvatarIKGoal.RightFoot, _rightFootIkPose, ref _lastRightFootPositionY, ref _rightFootVelocityY);

            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight * _feetPositionFactor);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight * _feetRotationFactor);
            MoveFootToIkPoint(AvatarIKGoal.LeftFoot, _leftFootIkPose, ref _lastLeftFootPositionY, ref _leftFootVelocityY);
        }

        private void MoveFootToIkPoint(AvatarIKGoal foot, Pose pose, ref float lastLocalPositionY, ref float lastVelocityY)
        {
            Vector3 targetPosition = _animator.GetIKPosition(foot);
            if (pose.Valid)
            {
                float targetLocalPositionY = (pose.Valid ? pose.Position.y : 0) - OriginY;
                targetLocalPositionY = Mathf.Clamp(targetLocalPositionY, _feetConstraint.Min, _feetConstraint.Max);
                float localPositionY = _feetSmoothTime > 0 ? Mathf.SmoothDamp(lastLocalPositionY, targetLocalPositionY, ref lastVelocityY, _feetSmoothTime) : targetLocalPositionY;
                lastLocalPositionY = localPositionY;
                targetPosition.y += localPositionY;
                _animator.SetIKPosition(foot, targetPosition);
                var targetIkRotation = _animator.GetIKRotation(foot);
                _animator.SetIKRotation(foot, pose.Rotation * targetIkRotation);
            }
        }

        private void MovePelvis(in Pose leftFootIkPose, in Pose rightFootIkPose, float weight)
        {
            var originalBodyPosition = _animator.bodyPosition;
            float lOffset = leftFootIkPose.Valid ? leftFootIkPose.Position.y - OriginY : 0;
            float rOffset = rightFootIkPose.Valid ? rightFootIkPose.Position.y - OriginY : 0;
            float targetPelvisOffset = Mathf.Min(lOffset, rOffset);
            targetPelvisOffset = Mathf.Lerp(0, targetPelvisOffset, _pelvisFactor * weight);
            targetPelvisOffset += _pelvisOffset;
            targetPelvisOffset = Mathf.Clamp(targetPelvisOffset, _pelvisConstraint.Min, _pelvisConstraint.Max);
            if (!_hasLastPelvisOffset)
            {
                _lastPelvisOffset = originalBodyPosition.y;
                _hasLastPelvisOffset = true;
            }
            var pelvisOffset = _pelvisSmoothTime > 0 ? Mathf.SmoothDamp(_lastPelvisOffset, targetPelvisOffset, ref _pelvisVelocity, _pelvisSmoothTime) : targetPelvisOffset;
            _lastPelvisOffset = pelvisOffset;
            _animator.bodyPosition = _animator.bodyPosition.AddY(pelvisOffset);

            if (Drawer.IsDebugEnabled)
            {
                if (leftFootIkPose.Valid)
                    Drawer.Debug
                        .Line(leftFootIkPose.Position, leftFootIkPose.Position.SetY(_animator.bodyPosition.y), Color.white)
                        .Line(_animator.bodyPosition, leftFootIkPose.Position.SetY(_animator.bodyPosition.y), Color.yellow);
                if (rightFootIkPose.Valid)
                    Drawer.Debug
                        .Line(rightFootIkPose.Position, rightFootIkPose.Position.SetY(_animator.bodyPosition.y), Color.white)
                        .Line(_animator.bodyPosition, rightFootIkPose.Position.SetY(_animator.bodyPosition.y), Color.yellow);
                Drawer.Debug
                    .Line(_animator.bodyPosition, originalBodyPosition, Color.red)
                    .Sphere(transform.position, 0.05f, Color.green);
            }
        }

        private Pose FootPositionSolver(in Pose footPose, bool lasValid)
        {
            //raycast handling section 
            var ray = new Ray(footPose.Position.SetY(OriginY + _raycastUpOffset), Vector3.down);
            float fullDistance = _raycastUpOffset + _raycastDownDistance + (lasValid ? _raycastDistanceThreshold : 0);
            Drawer.Trace?.Line(ray.origin, ray.GetPoint(fullDistance), new Color(1f, 0.7f, 0f, 0.5f));
            int count = Physics.RaycastNonAlloc(ray, _raycastHits, fullDistance, _groundLayers, QueryTriggerInteraction.Ignore);
            RaycastHit bestHit = default;
            bool hasHit = false;
            for (int i = 0; i < count; ++i)
            {
                var hit = _raycastHits[i];
                if (hit.transform.root == transform.root)
                    continue;
                if (!hasHit || hit.distance < bestHit.distance)
                {
                    bestHit = hit;
                    hasHit = true;
                }
                //var offsetY = Mathf.Clamp(feetOutHit.point.y - OriginY, _footConstraint.Min, _footConstraint.Max);
            }

            if (hasHit)
            {
                var normal = bestHit.normal;
                if (normal.y < _normalMaxAngleCos)
                {
                    normal.y = _normalMaxAngleCos;
                    var normalXZ = new Vector2(normal.x, normal.z).normalized * _normalMaxAngleSin;
                    normal.x = normalXZ.x;
                    normal.z = normalXZ.y;
                }
                var pose = new Pose(
                    bestHit.point.AddY(_skinWidthCompensation),
                    Quaternion.FromToRotation(Vector3.up, normal) /* * footPose.Rotation*/);
                Drawer.Debug?.Line(pose.Position, pose.Position + pose.Rotation * Vector3.up * 0.3f, Color.cyan);
                return pose;
            }

            return default;
        }

        private void OnNormalConstraintChanged()
        {
            _normalMaxAngleCos = Mathf.Cos(_normalConstraint * Mathf.Deg2Rad);
            _normalMaxAngleSin = Mathf.Sin(_normalConstraint * Mathf.Deg2Rad);
        }

        private readonly struct Pose
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly bool Valid;

            public Pose(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
                Valid = true;
            }
        }

        [Serializable]
        private struct Range
        {
            public float Min;
            public float Max;
        }
    }
}