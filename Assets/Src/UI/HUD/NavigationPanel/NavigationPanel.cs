using System.Collections.Generic;
using System.Linq;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Entities.Engine;
using Src.Aspects.Impl;
using UnityEngine;
using UnityWeld.Binding;
using WeldAdds;

namespace Uins
{
    [Binding]
    public class NavigationPanel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private UpdateInterval _updateInterval;

        [SerializeField, UsedImplicitly]
        private int _displayedFovInDegr = 90;

        [SerializeField, UsedImplicitly]
        private float _segmentWidthInDegr = 15;

        [SerializeField, UsedImplicitly]
        private float _segmentWidth = 61;

        [SerializeField, UsedImplicitly]
        private float _majorTickWidth = 2;

        [SerializeField, UsedImplicitly]
        private RectTransformSetter _rectTransformSetter;

        [SerializeField, UsedImplicitly]
        private NavigationIndicator _navigationIndicatorPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _indicatorsTransform;

        private float _scaleWith;

        private IGuideProvider _cameraGuideProvider;
        private MapOrientation _mapOrientation;
        private bool _hasOurPawn;
        private GameObject _pawnGameObject;
        private bool _isInited;

        private INavigationNotificationProvider _navigationNotificationProvider;
        private List<NavigationIndicator> Indicators { get; } = new List<NavigationIndicator>();
        private NavigationIndicator _selectedIndicator;


        //=== Props ===========================================================

        private int _cameraRotationAngle;

        [Binding]
        public int CameraRotationAngle
        {
            get => _cameraRotationAngle;
            set
            {
                if (value != _cameraRotationAngle)
                {
                    _cameraRotationAngle = value;
                    _rectTransformSetter.Amount = _cameraRotationAngle % _displayedFovInDegr;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(CameraRotationAngleLeftShift));
                    NotifyPropertyChanged(nameof(CameraRotationAngleRightShift));
                }
            }
        }

        [Binding]
        public int CameraRotationAngleLeftShift => TranslateNegativeAngleToPositive360(CameraRotationAngle - _displayedFovInDegr / 2);

        [Binding]
        public int CameraRotationAngleRightShift => TranslateNegativeAngleToPositive360(CameraRotationAngle + _displayedFovInDegr / 2);


        //=== Unity ===========================================================

        private void Awake()
        {
            _navigationIndicatorPrefab.AssertIfNull(nameof(_navigationIndicatorPrefab));
            _indicatorsTransform.AssertIfNull(nameof(_indicatorsTransform));
        }

        private void Start()
        {
            if (_rectTransformSetter.AssertIfNull(nameof(_rectTransformSetter)))
                return;

            //шкала под маской, центрирована по центру, видна половина ширины (и по одной четверти скрыто по краям)
            _scaleWith = _displayedFovInDegr * _segmentWidth / _segmentWidthInDegr * 2; //фактическая ширина рабочей шкалы
            var scaleShift = _scaleWith / 4; //ход
            var tickShift = -_majorTickWidth / 2;
            _rectTransformSetter.TargetMinValue = scaleShift + tickShift;
            _rectTransformSetter.TargetMaxValue = -scaleShift + tickShift;
            _rectTransformSetter.AmountMinValue = 0;
            _rectTransformSetter.AmountMaxValue = _displayedFovInDegr;
            _rectTransformSetter.OnEnable(); //reinit
        }

        void Update()
        {
            if (!_isInited || !_updateInterval.IsItTime())
                return;

            CameraRotationAngle = (int)GetDirectionRelativeRotationAngle(_pawnGameObject.transform.forward, _mapOrientation.transform.forward);
            foreach (var navigationIndicator in Indicators)
                navigationIndicator.OnUpdate(_selectedIndicator);
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource, INavigationNotificationProvider navigationNotificationProvider)
        {
            _navigationNotificationProvider = navigationNotificationProvider;
            if (_navigationNotificationProvider.AssertIfNull(nameof(_navigationNotificationProvider)))
                return;

            _navigationNotificationProvider.AddNavigationIndicatorTarget += OnAddNavigationIndicatorTarget;
            _navigationNotificationProvider.RemoveNavigationIndicatorTarget += OnRemoveNavigationIndicatorTarget;
            _navigationNotificationProvider.SelectedTargetChanged += OnSelectedTargetChanged;

            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public static float GetDirectionRelativeRotationAngle(Vector3 targetWorldDirection, Vector3 pawnWorldDirection, bool isSigned = false)
        {
            targetWorldDirection = new Vector3(targetWorldDirection.x, 0, targetWorldDirection.z).normalized;
            pawnWorldDirection = new Vector3(pawnWorldDirection.x, 0, pawnWorldDirection.z).normalized;
            var signedAngle180 = Vector3.SignedAngle(pawnWorldDirection, targetWorldDirection, Vector3.up);
            return isSigned ? signedAngle180 : TranslateNegativeAngleToPositive360(signedAngle180);
        }

        public static int TranslateNegativeAngleToPositive360(int signedAngle) => (signedAngle >= 0 ? signedAngle : 360 + signedAngle) % 360;

        public static float TranslateNegativeAngleToPositive360(float signedAngle) => (signedAngle >= 0 ? signedAngle : 360 + signedAngle) % 360;


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _pawnGameObject = null;
                _cameraGuideProvider = null;
                _hasOurPawn = false;
            }

            if (newEgo != null)
            {
                _pawnGameObject = newEgo.gameObject;
                _hasOurPawn = _pawnGameObject != null;
                _cameraGuideProvider = newEgo.GetComponent<ICharacterPawn>()?.CameraGuideProvider;
                _cameraGuideProvider.AssertIfNull(nameof(_cameraGuideProvider));
                if (_mapOrientation == null)
                {
                    _mapOrientation = FindObjectOfType<MapOrientation>();
                    _mapOrientation.AssertIfNull(nameof(_mapOrientation));
                }
            }

            _isInited = GetIsInited();
            foreach (var navigationIndicator in Indicators)
                navigationIndicator.UpdatePawn(_pawnGameObject, _cameraGuideProvider);
        }

        private void OnAddNavigationIndicatorTarget(Transform target, INavigationIndicatorSettings navigationIndicatorSettings)
        {
            if (navigationIndicatorSettings.AssertIfNull(nameof(navigationIndicatorSettings)) ||
                target.AssertIfNull(nameof(target)))
                return;

            if (GetIndicator(target) != null)
                return;

            var newIndicator = Instantiate(_navigationIndicatorPrefab, _indicatorsTransform);
            newIndicator.name = $"For_{target.name}";
            newIndicator.Init(target, navigationIndicatorSettings, _cameraGuideProvider, _pawnGameObject, _displayedFovInDegr);
            Indicators.Add(newIndicator);
        }

        private void OnRemoveNavigationIndicatorTarget(Transform target)
        {
            var indicatorToRemove = GetIndicator(target);
            if (indicatorToRemove == null)
                return;

            Indicators.Remove(indicatorToRemove);
            Destroy(indicatorToRemove.gameObject);
        }

        private void OnSelectedTargetChanged(Transform target)
        {
            _selectedIndicator = GetIndicator(target);
        }

        private NavigationIndicator GetIndicator(Transform target)
        {
            return Indicators.FirstOrDefault(indicator => indicator.Target == target);
        }

        private bool GetIsInited()
        {
            return _cameraGuideProvider != null && _mapOrientation != null && _hasOurPawn;
        }
    }
}