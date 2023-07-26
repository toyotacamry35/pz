using SharedCode.Entities.Engine;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class NavigationIndicator : BindingViewModel
    {
        private const float DefaultMaxDistanceToDisplay = 100000;
        public const float AngleEpsilon = 0.1f;

        private float _displayedFov;
        private bool _isInited;
        private float _squaredMaxDistanceForDisplay;
        private int _priorityConstant;
        private GameObject _pawnGameObject;


        //=== Props ===========================================================

        public INavigationIndicatorSettings Settings { get; private set; }

        public Transform Target { get; private set; }

        public IGuideProvider CameraGuideProvider { get; private set; }

        [Binding]
        public Sprite Icon => Settings?.Icon;

        [Binding]
        public Color IconColor => Settings?.IconColor ?? Color.clear;

        private bool _isDisplayed;

        [Binding]
        public bool IsDisplayed
        {
            get => _isDisplayed;
            set
            {
                if (value != _isDisplayed)
                {
                    _isDisplayed = value;
//                    UI.Logger.Info($"'{name}' IsDisplayed{_isDisplayed.AsSign()} SquaredDistance={SquaredDistance:f0}, " +
//                                    $"maxDist={_squaredMaxDistanceForDisplay:f0}, RS={ReferenceSystem?.transform.FullName()}"); //DEBUG
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isShowDist;

        [Binding]
        public bool IsShowDist
        {
            get => _isShowDist;
            set
            {
                if (value != _isShowDist)
                {
                    _isShowDist = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _dist;

        [Binding]
        public float Dist
        {
            get => _dist;
            set
            {
                if (!Mathf.Approximately(value, _dist))
                {
                    _dist = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float SquaredDistance { get; private set; }

        private float _displayedAngle;

        /// <summary>
        /// Угол (0-360) положения объекта индикатора относительно камеры
        /// </summary>
        public float SignedAngle { get; private set; }

        /// <summary>
        /// Угол (0-360) на котором _отображается_ индикатор на навигационной панели (обрезанный по _displayedFov)
        /// </summary>
        [Binding]
        public float DisplayedAngle
        {
            get => _displayedAngle;
            set
            {
                if (Mathf.Abs(value - _displayedAngle) > AngleEpsilon)
                {
                    _displayedAngle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int PriorityRatio => Mathf.RoundToInt(SquaredDistance) + _priorityConstant;


        //=== Public ==========================================================

        public static float GetDistanceWithoutZ(Transform t1, Transform t2)
        {
            if (t1.AssertIfNull(nameof(t1)) ||
                t2.AssertIfNull(nameof(t2)))
                return -1;

            return Vector2.Distance(new Vector2(t1.position.x, t1.position.z), new Vector2(t2.position.x, t2.position.z));
        }

        public void Init(
            Transform target,
            INavigationIndicatorSettings settings,
            IGuideProvider cameraGuideProvider,
            GameObject pawnGameObject,
            float displayedFov)
        {
            if (target.AssertIfNull(nameof(target)) ||
                settings.AssertIfNull(nameof(settings)))
            {
                _isInited = false;
                return;
            }

            _displayedFov = displayedFov;
            Target = target;
            CameraGuideProvider = cameraGuideProvider;
            _pawnGameObject = pawnGameObject;
            Settings = settings;
            IsShowDist = Settings.IsShowDist;
            NotifyPropertyChanged(nameof(Icon));
            NotifyPropertyChanged(nameof(IconColor));
            _squaredMaxDistanceForDisplay = Settings.DistanceToDisplay > 0
                ? Settings.DistanceToDisplay * Settings.DistanceToDisplay
                : DefaultMaxDistanceToDisplay * DefaultMaxDistanceToDisplay;
            _priorityConstant = Mathf.RoundToInt(DefaultMaxDistanceToDisplay) * 1000 * Mathf.RoundToInt(Settings.FovToDisplay);
            _isInited = GetIsInited();
        }

        public void UpdatePawn(GameObject pawnGameObject, IGuideProvider cameraGuideProvider)
        {
            _pawnGameObject = pawnGameObject;
            CameraGuideProvider = cameraGuideProvider;
            _isInited = GetIsInited();
        }

        public void OnUpdate(NavigationIndicator selectedNavigationIndicator)
        {
            if (!_isInited)
            {
                IsDisplayed = false;
                return;
            }

            if (Settings.IsSelectable && selectedNavigationIndicator != this)
            {
                IsDisplayed = false;
                return;
            }

            SquaredDistance = (_pawnGameObject.transform.position - Target.position).sqrMagnitude;
            if (SquaredDistance > _squaredMaxDistanceForDisplay)
            {
                IsDisplayed = false;
                return;
            }

            SignedAngle = NavigationPanel.GetDirectionRelativeRotationAngle(
                Target.position - _pawnGameObject.transform.position,
                CameraGuideProvider.Guide.ToUnity(),
                true);

            if (Mathf.Abs(SignedAngle) * 2 > Mathf.Max(_displayedFov, Settings.FovToDisplay))
            {
                IsDisplayed = false;
                return;
            }

            DisplayedAngle = Mathf.Clamp(SignedAngle, -_displayedFov / 2, _displayedFov / 2);

            IsDisplayed = true;
            Dist = GetDist();
        }


        //=== Private  ========================================================

        private bool GetIsInited()
        {
            return Target != null && CameraGuideProvider != null && _pawnGameObject != null && Settings != null;
        }

        private float GetDist()
        {
            if (_pawnGameObject == null)
                return -1;

            return GetDistanceWithoutZ(Target, _pawnGameObject.transform);
        }
    }
}