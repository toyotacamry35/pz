using System;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Entities.Engine;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// UI controller на карте игрока, ответственный за показ индикатора
    /// </summary>
    [Binding]
    public class MapIndicator : BindingController<MapIndicatorVmodel>
    {
        [SerializeField, UsedImplicitly]
        private Image[] _images;

        [SerializeField, UsedImplicitly]
        [Range(0.05f, 2)]
        private float _arrowRatio = .5f;

        private Vector3 _lastPosition;
        private Vector3 _lastDirection;

        private RectTransform _rectTransform;


        //=== Props ===========================================================

        public Guid MarkerGuid => Vmodel.Value.MarkerGuid;

        public Transform Target => Vmodel.Value.Target;

        public Vector2 MapRelPosition => _rectTransform.anchorMin; //равно как и Max

        [Binding]
        public Sprite Icon { get; private set; }

        [Binding]
        public string Description { get; private set; }

        [Binding]
        public Color PointColor { get; private set; }

        public bool IsSelectable { get; private set; }

        [Binding]
        public bool IsSelected { get; private set; }

        [Binding]
        public bool IsPlayer { get; private set; }

        public bool IsInited { get; private set; }

        private IGuideProvider _cameraGuideProvider;

        public IGuideProvider CameraGuideProvider
        {
            get => _cameraGuideProvider;
            set
            {
                if (_cameraGuideProvider != value)
                {
                    var oldHasDirectionIndicator = HasDirectionIndicator;
                    _cameraGuideProvider = value;
                    if (oldHasDirectionIndicator != HasDirectionIndicator)
                        NotifyPropertyChanged(nameof(HasDirectionIndicator));
                }
            }
        }

        [Binding]
        public bool HasDirectionIndicator => CameraGuideProvider != null;

        private float _directionAngle;

        [Binding]
        public float DirectionAngle
        {
            get => _directionAngle;
            set
            {
                if (!Mathf.Approximately(_directionAngle, value))
                {
                    _directionAngle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _arrowWidthRatio;

        [Binding]
        public float ArrowWidthRatio
        {
            get => _arrowWidthRatio;
            set
            {
                if (!Mathf.Approximately(_arrowWidthRatio, value))
                {
                    _arrowWidthRatio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _arrowHeigthRatio;

        [Binding]
        public float ArrowHeigthRatio
        {
            get => _arrowHeigthRatio;
            set
            {
                if (!Mathf.Approximately(_arrowHeigthRatio, value))
                {
                    _arrowHeigthRatio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int QuestZoneDiameter { get; private set; }

        [Binding]
        public bool HasQuestZone { get; private set; }

        [Binding]
        public float QuestZoneIconSize { get; private set; }

        [Binding]
        public float QuestZoneLineRotationAngle { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _rectTransform = transform.GetRectTransform();
            var settingsStream = Vmodel.Func(D, vm => vm.MapIndicatorSettings);
            var isSelectableStream = settingsStream.Func(D, settings => settings.IsSelectable);
            Bind(isSelectableStream, () => IsSelectable);
            Bind(settingsStream.Func(D, settings => settings.IsPlayer), () => IsPlayer);
            var iconStream = settingsStream.Func(D, settings => settings.MapIcon);
            Bind(iconStream, () => Icon);
            Bind(settingsStream.Func(D, settings => settings.PointColor), () => PointColor);
            var questZoneDiameterStream = settingsStream.Func(D, settings => settings.QuestZoneDiameter);
            Bind(questZoneDiameterStream, () => QuestZoneDiameter);
            var hasQuestZoneStream = questZoneDiameterStream.Func(D, d => d > 0);
            Bind(hasQuestZoneStream, () => HasQuestZone);
            Bind(settingsStream.Func(D, settings => settings.Description), () => Description);

            var currentMapScaleStream = Vmodel.SubStream(D, vm => vm.MapIndicatorsPositions.CurrentMapScaleRp);
            var questZoneIconSizeStream = hasQuestZoneStream
                .Zip(D, currentMapScaleStream)
                .Zip(D, questZoneDiameterStream)
                .Where(D, (hasZone, scale, diameter) => hasZone)
                //.Log(D, "pre.questZoneIconSizeStream", (t, diameter) => $"scale={t.Item2}, d={diameter}") //2del
                .Func(D, (hasZone, scale, diameter) => scale * diameter);

            Bind(questZoneIconSizeStream, () => QuestZoneIconSize);

            var isSelectedStream = Vmodel
                .SubStream(
                    D,
                    vm => vm.MapIndicatorsPositions.SelectedIndicator
                        .Func(D, mapIndicator => mapIndicator == this),
                    false)
                .Zip(D, isSelectableStream)
                .Func(D, (isSelected, isSelectable) => isSelectable && isSelected);
            Bind(isSelectedStream, () => IsSelected);

            Bind(Vmodel.Func(D, vm => vm.MapIndicatorsPositions != null && vm.MapIndicatorSettings != null && vm.Target != null), () => IsInited);
        }

        protected override void OnDestroy()
        {
            if (IsSelected)
                Vmodel.Value.MapIndicatorsPositions.TakeSelectedIndicator(null);

            base.OnDestroy();
        }


        //=== Public ==========================================================

        public void OnClick(bool isLmb)
        {
            if (isLmb)
            {
                if (!IsSelectable)
                    return;

                Vmodel.Value.MapIndicatorsPositions.TakeSelectedIndicator(IsSelected ? null : this);
            }
            else
            {
                //rmb
                Vmodel.Value.MapIndicatorsPositions.OnIndicatorClickRmb(this);
            }
        }

        public void UpdatePosition(bool isForced = false)
        {
            if (!IsInited || Target == null)
                return;

            if (isForced || _lastPosition != Target.position)
            {
                _lastPosition = Target.position;

                var anchorRatio = Vmodel.Value.MapIndicatorsPositions.GetAnchorRatioByLevelPosition(Target.position);
                _rectTransform.anchorMin = anchorRatio;
                _rectTransform.anchorMax = anchorRatio;
                _rectTransform.anchoredPosition = Vector2.zero;
            }

            if (HasDirectionIndicator)
            {
                var guide = CameraGuideProvider.Guide.ToUnity();
                if (isForced || guide != _lastDirection)
                {
                    _lastDirection = guide;
                    DirectionAngle = Vector2.SignedAngle(Vector2.up, new Vector2(_lastDirection.x, _lastDirection.z));
                    //Mathf.Atan2(-_lastDirection.x, _lastDirection.z) * Mathf.Rad2Deg;
                    var absCos = Mathf.Abs(Mathf.Cos(DirectionAngle * Mathf.Deg2Rad));
                    ArrowWidthRatio = 1 + absCos * _arrowRatio;
                    ArrowHeigthRatio = 1 - absCos * _arrowRatio;
                    //UI.CallerLog($"DirectionAngle={DirectionAngle}, ArrowWidthRatio={ArrowWidthRatio}");
                }
            }

            if (HasQuestZone)
            {
                QuestZoneLineRotationAngle += Vmodel.Value.MapIndicatorsPositions.QuestZoneRotationFactor;
                NotifyPropertyChanged(nameof(QuestZoneLineRotationAngle));
            }
        }

        public void SetOffImageColliders()
        {
            if (_images == null || _images.Length == 0)
                return;

            for (int i = 0; i < _images.Length; i++)
            {
                var image = _images[i];
                if (image != null)
                    image.raycastTarget = false;
            }
        }


        //=== Private =========================================================

    }
}