using System;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using Assets.Src.Camera;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using SVector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.Aspects.VisualMarkers
{
    public abstract class AVisualMarker : EntityGameObjectComponent
    {
        public const float UpdateDelay = 1f;

        [FormerlySerializedAs("_sphereRadius"), Range(0, 1000)]
        public float SphereRadius;

        [FormerlySerializedAs("_hasPoint")]
        public bool HasPoint = true;

        public bool IsMoving;

        public bool IsDebug;

        [SerializeField, UsedImplicitly]
        private JdbMetadata _predicateIgnoreGroup;

        public bool ShowMarker { get; set; } = true;
        public ReactiveProperty<bool> IsNearRp = new ReactiveProperty<bool>();

        private ReactiveProperty<bool> _isCulledRp = new ReactiveProperty<bool>() {Value = true};
        private ReactiveProperty<UnityEngine.Camera> _cameraRp = new ReactiveProperty<UnityEngine.Camera>() {Value = null};

        private bool _isInitedOnce;
        private CullingGroup _markerCullingGroup;

        private DisposableComposite _calculationInnerD;
        private ReactiveProperty<float> _spherePlusSelectorRadiusSqrRp = new ReactiveProperty<float>();


        //=== Props ===========================================================

        public ReactiveProperty<VisualMarkerSelector> VisualMarkerSelectorRp { get; } = new ReactiveProperty<VisualMarkerSelector>() {Value = null};

        public PredicateIgnoreGroupDef PredicateIgnoreGroupDef { get => _predicateIgnoreGroup?.Get<PredicateIgnoreGroupDef>(); }

        public ReactiveProperty<bool> IsOurPlayerRp { get; } = new ReactiveProperty<bool>() {Value = true};

        public ReactiveProperty<bool> IsSelectedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<bool> IsInRangeRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<bool> IsImportantBadgeShownRp { get; } = new ReactiveProperty<bool>() {Value = false};

        // used for automatic scene export
        public void SetPredicateIgnoreGroupMetadata(JdbMetadata metadata)
        {
            _predicateIgnoreGroup = metadata;
        }


        //=== Protected =======================================================

        protected void OnceInit()
        {
            if (_isInitedOnce)
                return;

            IsOurPlayerRp.Value = GetIsOurPlayer();

            if (!IsOurPlayerRp.Value) //Активация подписок стримов IsInRangeRp, IsSelectedRp
            {
                SetupReactiveProperties();

                if (GameCamera.Camera != null)
                    _cameraRp.Value = GameCamera.Camera;
                else
                    GameCamera.OnCameraCreated += OnCameraSpawned;
            }

            _isInitedOnce = true;
        }

        protected abstract bool GetIsOurPlayer();


        //=== Private =========================================================

        private void SetupReactiveProperties()
        {
            if (SurvivalGuiNode.Instance.AssertIfNull(nameof(SurvivalGuiNode), gameObject))
                return;

            SurvivalGuiNode.Instance.PawnChangesStream.Action(D, OnPawnChanged);

            var needToCalculatingStream = VisualMarkerSelectorRp
                .Zip(D, _cameraRp)
                .Zip(D, IsNearRp)
                .Func(D, NeedToCalculatingFunc);

            if (HasPoint)
                VisualMarkerSelectorRp.Action(D, OnVisualMarkerSelectorAction);

            needToCalculatingStream.Action(D, SwitchCalculation);

            // if (IsDebug) //DEBUG
            // {
            //     needToCalculatingStream.Log(D, $"{name}.{nameof(needToCalculatingStream)}");
            //     //IsOurPlayerRp.Log(D, $"{name}.{nameof(IsOurPlayerRp)}");
            //     IsSelectedRp.Log(D, $"{name}.{nameof(IsSelectedRp)}");
            //     //VisualMarkerSelectorRp.Log(D, $"{name}.{nameof(VisualMarkerSelectorRp)}");
            //     IsInRangeRp.Log(D, $"{name}.{nameof(IsInRangeRp)}");
            //     IsImportantBadgeShownRp.Log(D, $"{name}.{nameof(IsImportantBadgeShownRp)}");
            //     _isCulledRp.Log(D, $"{name}.{nameof(_isCulledRp)}");
            //     IsNearRp.Log(D, $"{name}.{nameof(IsNearRp)}");
            // }
        }

        private bool NeedToCalculatingFunc(VisualMarkerSelector selector, UnityEngine.Camera cam, bool isNear)
        {
            return selector != null && cam != null && isNear;
        }

        private void OnVisualMarkerSelectorAction(VisualMarkerSelector vmSelector)
        {
            var dist = SphereRadius + vmSelector?.Radius ?? 0;
            _spherePlusSelectorRadiusSqrRp.Value = dist * dist;
        }

        private void OnCameraSpawned(UnityEngine.Camera cam)
        {
            _cameraRp.Value = cam;
            if (cam != null)
                GameCamera.OnCameraCreated -= OnCameraSpawned;
        }

        private void SwitchCalculation(bool isOn)
        {
            if (_calculationInnerD!= null)
                D.DisposeInnerD(_calculationInnerD);
            _calculationInnerD = D.CreateInnerD();

            if (isOn)
            {
                if (IsMoving || Mathf.Approximately(SphereRadius, 0))
                {
                    _isCulledRp.Value = false;
                }
                else
                {
                    Assert.IsNull(_markerCullingGroup);
                    _markerCullingGroup = new CullingGroup
                    {
                        targetCamera = _cameraRp.Value, 
                        onStateChanged = CullingStateChanged
                    };
                    _markerCullingGroup.SetBoundingSpheres(new[] {new BoundingSphere(transform.position, SphereRadius)});
                    _markerCullingGroup.SetBoundingSphereCount(1);
                    _calculationInnerD.Add(_markerCullingGroup);
                }

                VisualMarkerSelectorRp.Value.TargetHolder.CurrentTarget.Func(_calculationInnerD, IsSelectedFunc).Bind(_calculationInnerD, IsSelectedRp);

                var isInRangeStream = TimeTicker.Instance.GetLocalTimer(UpdateDelay)
                    .Zip(_calculationInnerD, _isCulledRp)
                    .Func(_calculationInnerD, IsInRangeAndNoCulledFunc);

                isInRangeStream.Bind(_calculationInnerD, IsInRangeRp);
            }
            else
            {
                if (!IsMoving)
                {
                    _isCulledRp.Value = true;
                    _markerCullingGroup = null;
                }
            }
        }

        private bool IsSelectedFunc(GameObject selection)
        {
            return selection == gameObject;
        }

        private bool IsInRangeAndNoCulledFunc(DateTime dt, bool isCulled)
        {
            return !isCulled && IsInRange();
        }

        private bool IsInRange()
        {
            if (!HasPoint)
                return true;

            var directionSqrMagnitude = (VisualMarkerSelectorRp.Value.Position - (SVector3) transform.position).sqrMagnitude;
            return directionSqrMagnitude < _spherePlusSelectorRadiusSqrRp.Value;
        }

        private void CullingStateChanged(CullingGroupEvent sphere)
        {
            if (sphere.hasBecomeInvisible)
                _isCulledRp.Value = true;

            else if (sphere.hasBecomeVisible)
                _isCulledRp.Value = false;
        }

        private void OnPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            VisualMarkerSelectorRp.Value = newEgo != null ? newEgo.GetComponent<VisualMarkerSelector>() : null;
        }
    }
}