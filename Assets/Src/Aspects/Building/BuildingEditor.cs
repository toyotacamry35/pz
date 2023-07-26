using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Shared;
using AwesomeTechnologies;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// костры, верстаки
namespace Assets.Src.Aspects.Building
{
    public class BuildingEditor : MonoBehaviour
    {
        public float RotationAngle { get; set; }

        [SerializeField, Range(0, 90)]
        private float _maximumAngle = 0;

        [SerializeField, Range(0, 1)]
        private float _gizmoDrawRadius = 0.3f;

        [SerializeField, Range(0.01f, 10)]
        private float _pointTraceDistance = 1;

        [SerializeField]
        private List<Vector3> _onGroundReferencePoints = new List<Vector3>();

        [SerializeField]
        private List<Vector3> _underGroundReferencePoints = new List<Vector3>();

        [SerializeField]
        private List<Vector3> _aboveGroundReferencePoints = new List<Vector3>();

        [SerializeField]
        private List<Bounds> _colliderCheckers = new List<Bounds>();
        
        [SerializeField]
        private List<GameObject> _disabledWhenPlacingActiveObjects = new List<GameObject>();

        private Vector3 _scaleCache = Vector3.one;
        private Quaternion _rotationCache = Quaternion.identity;
        private Dictionary<Vector3, Vector3> _toWorldCoordinatesCacheWithScale = new Dictionary<Vector3, Vector3>();
        private Dictionary<Vector3, Vector3> _toWorldCoordinatesCacheWithRotation = new Dictionary<Vector3, Vector3>();

        private Vector3? _buildPoint = null;

        public Vector3 BuildPoint
        {
            get
            {
                if (_buildPoint == null)
                {
                    _buildPoint = Vector3.zero;
                    foreach (var groundPoint in _onGroundReferencePoints)
                    {
                        _buildPoint += groundPoint;
                    }

                    _buildPoint = ToWorldCoordinates(_buildPoint.Value / _onGroundReferencePoints.Count, true);
                }

                return _buildPoint.Value;
            }
        }

        private bool? _isVisible = null;

        public void SetVisibility(bool enable)
        {
            for (int i = 0; i < _disabledWhenPlacingActiveObjects.Count; i++)
            {
                if (_disabledWhenPlacingActiveObjects[i].activeSelf)
                    _disabledWhenPlacingActiveObjects[i].SetActive(false);
            }
            
            if (!_isVisible.HasValue || _isVisible != enable)
            {
                _isVisible = enable;
                var renderers = GetComponentsInChildren<Renderer>(true);
                foreach (var r in renderers)
                {
                    r.enabled = enable;
                }
            }
        }

        private bool? _isAvaliable = null;

        public bool IsAvaliable => _isAvaliable.HasValue && _isAvaliable.Value;

        public void SetAvaliable(bool enable)
        {
            if (!_isAvaliable.HasValue || _isAvaliable != enable)
            {
                _isAvaliable = enable;
                var renderers = GetComponentsInChildren<Renderer>(true);
                Color color = enable ? Color.green : Color.red;
                foreach (var r in renderers)
                {
                    if (r.materials.Length > 0)
                        r.materials[0].color = color;
                }
            }
        }

        private bool? _isCollidersEnable = null;

        public void SetCollidersEnable(bool enable)
        {
            if (!_isCollidersEnable.HasValue || _isCollidersEnable != enable)
            {
                _isCollidersEnable = enable;
                foreach (var collider in GetComponentsInChildren<Collider>(true))
                {
                    collider.enabled = enable;
                }

                foreach (var navMesh in GetComponentsInChildren<NavMeshObstacle>(true))
                {
                    navMesh.enabled = enable;
                }

                foreach (var component in GetComponentsInChildren<VegetationMaskArea>(true))
                {
                    component.enabled = enable;
                }
            }
        }

        public bool CanBuildHere(Vector3 buildPoint, Quaternion rotation, Guid sceneId, IResource resource, bool isOnSrvDebug = false)
        {
            Vector3 position = GetPosition(buildPoint);
            var canBuildHere = true;
            Vector3 crossPoint;

            foreach (var point in _underGroundReferencePoints)
            {
                if (!GetGroundCrossPoint(ToWorldCoordinates(point) + position, out crossPoint, true))
                {
                    canBuildHere = false;
                    break;
                }
            }

            if (canBuildHere)
            {
                foreach (var point in _aboveGroundReferencePoints)
                {
                    if (!GetGroundCrossPoint(ToWorldCoordinates(point) + position, out crossPoint, false))
                    {
                        canBuildHere = false;
                        break;
                    }
                }
            }

            if (canBuildHere)
            {
                foreach (var colliderChecker in _colliderCheckers)
                {
                    var checkBounds = colliderChecker;

                    // TODOA: Bag is here with checking collider with object rotation
                    var checkBoxPos = ToWorldCoordinates(checkBounds.center, true) + position;
                    var checkBoxHalfExtents = ToWorldCoordinates(checkBounds.extents, true) / 2;
                    if (Physics.CheckBox(checkBoxPos, checkBoxHalfExtents, rotation, PhysicsLayers.BuildMask,
                        QueryTriggerInteraction.Ignore))
                    {
                        canBuildHere = false;
                        break;
                    }
                }
            }

            if (canBuildHere)
            {
                canBuildHere = BuildingEngineHelper.CanBuildHere(buildPoint.ToShared(), sceneId, resource, true);
            }

            return canBuildHere;
        }

        public Vector3 GetPosition(Vector3 position)
        {
            return position - BuildPoint;
        }

        public Quaternion GetRotation(Vector3 position)
        {
            Quaternion rotation = Quaternion.AngleAxis(RotationAngle, Vector3.up);

            if (_onGroundReferencePoints.Count == 2)
            {
                Vector3 p1, p2;
                if (GetGroundCrossPoint(ToWorldCoordinates(_onGroundReferencePoints[0]) + position, out p1) &&
                    GetGroundCrossPoint(ToWorldCoordinates(_onGroundReferencePoints[1]) + position, out p2))
                {
                    var v = p2 - p1;
                    var angle = Mathf.Clamp(Mathf.Asin((p2.y - p1.y) / v.magnitude) * Mathf.Rad2Deg, -_maximumAngle, _maximumAngle);
                    var n = Vector3.Cross(v, Vector3.up).normalized;
                    rotation = Quaternion.AngleAxis(angle, n) * rotation;
                }
            }
            else if (_onGroundReferencePoints.Count == 3)
            {
                Vector3 p1, p2, p3;
                if (GetGroundCrossPoint(ToWorldCoordinates(_onGroundReferencePoints[0]) + position, out p1) &&
                    GetGroundCrossPoint(ToWorldCoordinates(_onGroundReferencePoints[1]) + position, out p2) &&
                    GetGroundCrossPoint(ToWorldCoordinates(_onGroundReferencePoints[2]) + position, out p3))
                {
                    var v1 = p2 - p1;
                    var v2 = p3 - p1;
                    var n = Vector3.Cross(v1, v2).normalized;
                    var r = Vector3.Cross(n, Vector3.up);
                    var angle = -Mathf.Clamp(Mathf.Asin(r.magnitude) * Mathf.Rad2Deg, -_maximumAngle, _maximumAngle);
                    rotation = Quaternion.AngleAxis(angle, r.normalized) * rotation;
                }
            }

            return rotation;
        }

        private bool GetGroundCrossPoint(Vector3 point, out Vector3 crossPoint, bool? traceUp = null)
        {
            RaycastHit _hit;
            var ray = GetRay(point, traceUp);
            var result = Physics.Raycast(ray.Item1, ray.Item2, out _hit, ray.Item2.magnitude, PhysicsLayers.BuildMask, QueryTriggerInteraction.Ignore);
            
            if (result)
            {
                crossPoint = _hit.point;
                return true;
            }

            crossPoint = default(Vector3);
            return false;
        }

        private Tuple<Vector3, Vector3> GetRay(Vector3 point, bool? traceUp = null)
        {
            Tuple<Vector3, Vector3> ray =
                !traceUp.HasValue ?
                    new Tuple<Vector3, Vector3>(point + transform.rotation * (_pointTraceDistance * Vector3.up), transform.rotation * (2 * _pointTraceDistance * Vector3.down)) :
                traceUp.Value ?
                    new Tuple<Vector3, Vector3>(point + transform.rotation * (_pointTraceDistance * Vector3.up), transform.rotation * (_pointTraceDistance * Vector3.down)) :
                    new Tuple<Vector3, Vector3>(point, transform.rotation * (_pointTraceDistance * Vector3.down));

            return ray;
        }

        private Vector3 ToWorldCoordinates(Vector3 point, bool scaleOnly = false)
        {
            var scale = transform.localScale;
            var rotation = transform.rotation;

            if (_onGroundReferencePoints.Count <= 1 && !scaleOnly)
            {
                if (scale != _scaleCache || rotation != _rotationCache)
                {
                    _toWorldCoordinatesCacheWithRotation.Clear();
                    _scaleCache = scale;
                    _rotationCache = rotation;
                }

                if (!_toWorldCoordinatesCacheWithRotation.ContainsKey(point))
                {
                    _toWorldCoordinatesCacheWithRotation.Add(point,
                        rotation * new Vector3(point.x * scale.x, point.y * scale.y, point.z * scale.z));
                }

                return _toWorldCoordinatesCacheWithRotation[point];
            }
            else
            {
                if (scale != _scaleCache)
                {
                    _toWorldCoordinatesCacheWithScale.Clear();
                    _scaleCache = scale;
                }

                if (!_toWorldCoordinatesCacheWithScale.ContainsKey(point))
                {
                    _toWorldCoordinatesCacheWithScale.Add(point, new Vector3(point.x * scale.x, point.y * scale.y, point.z * scale.z));
                }

                return scaleOnly ? _toWorldCoordinatesCacheWithScale[point] : rotation * _toWorldCoordinatesCacheWithScale[point];
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var point in _onGroundReferencePoints)
            {
                var worldPoint = ToWorldCoordinates(point) + transform.position;
                var ray = GetRay(worldPoint);
                Gizmos.DrawLine(ray.Item1, ray.Item1 + ray.Item2);
                Gizmos.DrawSphere(worldPoint, _gizmoDrawRadius);
            }

            Gizmos.color = Color.red;
            foreach (var point in _underGroundReferencePoints)
            {
                var worldPoint = ToWorldCoordinates(point) + transform.position;
                var ray = GetRay(worldPoint, true);
                Gizmos.DrawLine(ray.Item1, ray.Item1 + ray.Item2);
                Gizmos.DrawSphere(ToWorldCoordinates(point) + transform.position, _gizmoDrawRadius);
            }

            Gizmos.color = Color.blue;
            foreach (var point in _aboveGroundReferencePoints)
            {
                var worldPoint = ToWorldCoordinates(point) + transform.position;
                var ray = GetRay(worldPoint, false);
                Gizmos.DrawLine(ray.Item1, ray.Item1 + ray.Item2);
                Gizmos.DrawSphere(ToWorldCoordinates(point) + transform.position, _gizmoDrawRadius);
            }

            Gizmos.color = Color.yellow;
            foreach (var colliderChecker in _colliderCheckers)
            {
                Gizmos.DrawCube(ToWorldCoordinates(colliderChecker.center, true) + transform.position,
                    ToWorldCoordinates(colliderChecker.extents, true));
            }
        }
    }
}