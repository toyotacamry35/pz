using System;
using System.Collections.Generic;
using Uins;
using UnityEngine;
using Assets.Src.Aspects.Impl;
using Assets.Src.Aspects.Doings;
using JetBrains.Annotations;
using System.Linq;
using Assets.Src.Tools;
using Assets.Src.Camera;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion.Unity;
using Logger = NLog.Logger;

namespace Assets.Src.Target
{
    public class SetTargetCone : MonoBehaviour
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private const  int MaxCollidersCount = 32;
        
        /// <summary>
        /// переключается на новый таргет если разница расстояний до центральной линии конуса превышает это число
        /// </summary>
        [UsedImplicitly]
        [SerializeField]
        float _deltaDistanceForNewTarget;

        /// <summary>
        /// раз в столько секунд может смениться таркет
        /// </summary>
        [UsedImplicitly]
        [SerializeField]
        float _deltaTime;

        /// <summary>
        /// local euler rotation Y находится в этом пределе
        /// </summary>
        [UsedImplicitly]
        [SerializeField]
        float _deltaHorizontalRotation;

        /// <summary>
        /// дистанция для изученных объектов меньше чем весь конус
        /// </summary>
        [UsedImplicitly]
        [SerializeField]
        float _distanceKnownObj = 10;

        Dictionary<ConeInfo.Type, ConeInfo> _coneDictionary = new Dictionary<ConeInfo.Type, ConeInfo>();
        ConeInfo _cone;
        Transform _camera;
        TargetHolder _targetHolder;
        ICharacterBrain _characterBrain;
        private readonly Collider[] _colliders = new Collider[MaxCollidersCount];
        private readonly (Collider,Interactive)[] _objects = new (Collider,Interactive)[MaxCollidersCount];
        private Collider _prevTarget;
        private int _objectsCount;
        float _time = 0;

        public void Init(TargetHolder targetHolder, GameObject pawn, ICharacterBrain characterBrain)
        {
            if (targetHolder == null)
            {
                Debug.LogError(string.Format($"Error Init SetTargetCone.  targetHolder = {targetHolder}, pawn = {pawn}"));
                return;
            }

            _targetHolder = targetHolder;
            _characterBrain = characterBrain;

            ConeInfo[] coneInfo = gameObject.GetComponents<ConeInfo>();
            foreach (var info in coneInfo)
            {
                if (_coneDictionary.ContainsKey(info.type))
                {
                    Debug.LogError(string.Format("There are more one ConeInfo with type = {0}, obj = {1}", info.type, gameObject.name));
                }
                else
                {
                    _coneDictionary.Add(info.type, info);
                }
            }

            ChangeConeType(ConeInfo.Type.general);

            if (GameCamera.Camera)
                InitCamera(GameCamera.Camera);
            else
            {
                GameCamera.OnCameraCreated -= InitCamera;
                GameCamera.OnCameraCreated += InitCamera;
            }
        }

        public void InitCamera(UnityEngine.Camera camera)
        {
            GameCamera.OnCameraCreated -= InitCamera;
            if (!this)
                return;

            _camera = camera.gameObject.transform;
        }

        void OnDestroy()
        {
//            if (_current != null)
//            {
//                _current.Cone.TriggerStayEvent -= TriggerStay;
//                _current.Cone.TriggerEnterEvent -= TriggerEnter;
//                _current.Cone.TriggerExitEvent -= TriggerExit;
//                _current.Cone.gameObject.SetActive(false);
//            }

            _objectsCount = 0;
            if (_targetHolder != null)
            {
                _targetHolder.CurrentTarget.Value = null;
            }

            _cone = null;
        }

        void ChangeConeType(ConeInfo.Type type)
        {
            if (_cone != null)
            {
                if (_cone.type == type)
                {
                    return;
                }
//                else
//                {
//                    _current.Cone.TriggerStayEvent -= TriggerStay;
//                    _current.Cone.TriggerEnterEvent -= TriggerEnter;
//                    _current.Cone.TriggerExitEvent -= TriggerExit;
//                    _current.Cone.gameObject.SetActive(false);
//                }
            }

            if (_coneDictionary.ContainsKey(type))
            {
                _objectsCount = 0;
                _cone = _coneDictionary[type];
//                _current.Cone.TriggerStayEvent += TriggerStay;
//                _current.Cone.TriggerEnterEvent += TriggerEnter;
//                _current.Cone.TriggerExitEvent += TriggerExit;
//                _current.Cone.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("SetTargetCone: Not find cone type " + type.ToString());
            }
        }

        bool CorrectLayer(Collider other)
        {
            int layer = other.gameObject.layer;
            return ((1 << layer) & Shared.PhysicsLayers.TargetableMask) != 0b0;
        }

        private void Update()
        {
            if (_characterBrain == null) return;
            _time += Time.deltaTime;

            if (!(_time > _deltaTime)) return;
            _time = 0;

            Vector3 cameraAngle = _camera
                ? new Vector3(_camera.eulerAngles.x - _cone.angleCamera, GetHorizontalAngleRotation(), 0)
                : Vector3.zero;

            if (_cone && _camera)
            {
                _cone.Cone.gameObject.transform.localEulerAngles = cameraAngle;
            }

            if (_targetHolder)
            {
                Collider newTarget = null;
                if (_cone)
                {
                    var collidersCount = _cone.Overlap(_colliders, _deltaTime);
                    _objectsCount = 0;
                    for (int i = 0; i < collidersCount; ++i)
                    {
                        var collider = _colliders[i];
                        if (!collider)
                            continue;
                        
                        if (PhysicsUtils.IsSameObject(collider.transform, transform))
                            continue;
                        
                        var interactive = collider.gameObject.GetComponent<Interactive>();
                        if (!interactive)
                            continue;
                        
                        _objects[_objectsCount] = (collider, interactive);
                        ++_objectsCount;
                    }
                    newTarget = GetNear(_objects, _objectsCount, _prevTarget);
                }

                _prevTarget = newTarget;
                _targetHolder.CurrentTarget.Value = newTarget ? newTarget.gameObject : null;
            }
        }

        /// <summary>
        /// выбирает ближайший из colliders
        /// </summary>
        Collider GetNear((Collider Collider, Interactive Interactive)[] objects, int objectsCount, Collider current)
        {
            Vector3 currentPos = gameObject.transform.position + 2 * Vector3.up;
            Vector3 line = (_cone.SmallCentrePosition - _cone.BigCentrePosition).normalized;
            Collider best = null;
            int bestPriority = Int32.MinValue;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < objectsCount; i++)
            {
                var @object = objects[i];
                if (@object.Collider.gameObject.activeInHierarchy &&
                    CorrectLayer(@object.Collider) &&
                    CorrectDistanceObject(@object.Collider) &&
                    CorrectNoObstacle(@object.Collider, currentPos))
                {
                    var p = @object.Interactive.Priority;
                    var d = DistanceToCone(@object.Collider, line) - (PhysicsUtils.IsSameObject(@object.Collider, current) ? _deltaDistanceForNewTarget : 0);
                    if (p > bestPriority || (p == bestPriority && d < bestDistance))
                    {
                        bestPriority = p;
                        bestDistance = d;
                        best = @object.Collider;
                    }
                }
            }
            if (Logger.IsTraceEnabled)  Logger.IfTrace()?.Message("{@}",  new { Target = best?.transform.FullName(), Distance = bestDistance, Priority = bestPriority }).Write();
            return best;
        }

        /// <summary>
        /// если объект изучен, то ограничиваем расстояние
        /// </summary>
        bool CorrectDistanceObject(Collider collider)
        {
            if (!collider)
            {
                return false;
            }

            float distance = PlayerInteractionViewModel.DistanceFromPointToCollider(gameObject.transform.position, collider);
            if (distance > _distanceKnownObj)
            {
                return false;
            }

            return true;
        }

        private bool CorrectNoObstacle(Collider toCollider, Vector3 currentPos)
        {
            Vector3 colliderPos = PlayerInteractionViewModel.GetColliderClosestPoint(currentPos, toCollider);
            Vector3 direction = colliderPos - currentPos;
#if false // UNITY_EDITOR
            var hit = Physics.RaycastAll(currentPos, direction, PhysicsChecker.CheckDistance(direction.magnitude, "SetTargetCone"), Shared.PhysicsLayers.ObstacleMask); // debug
#endif
            return !Physics.Raycast(currentPos, direction, PhysicsChecker.CheckDistance(direction.magnitude, "SetTargetCone2"), Shared.PhysicsLayers.ObstacleMask);
        }

        /// <summary>
        /// расстояние от коллайдера до центральной линии конуса
        /// </summary>
        float DistanceToCone(Collider obj, Vector3 line)
        {
            Vector3 bigObj = obj.bounds.center - _cone.BigCentrePosition;
            float angle = Vector3.Angle(bigObj, line);
            Vector3
                controlPoint =
                    _cone.BigCentrePosition +
                    line * (bigObj.magnitude * Mathf.Cos(angle)); //точка на оси, соединяющей центры сечений конусов, расстояние от которой минимально до центра объекта

            float distanceCentr = Vector3.Distance(obj.bounds.center, controlPoint); //расстояние от центра объекта до контрольной точки
            float distanceObj =
                PlayerInteractionViewModel.DistanceFromPointToCollider(controlPoint,
                    obj); //расстояние от контрольной точки до границы коллайдера

            return distanceCentr < distanceObj || obj.bounds.Contains(controlPoint) ? 0 : distanceObj;
        }

        float GetHorizontalAngleRotation()
        {
            float angle = (_camera.eulerAngles.y - gameObject.transform.eulerAngles.y) % 360;
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            return Mathf.Clamp(angle, -_deltaHorizontalRotation / 2, _deltaHorizontalRotation / 2);
        }
    }
}