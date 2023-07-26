using Assets.Src.Camera;
using Cinemachine;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class GuiBadge : BindingViewModel
    {
        private const float ScreenWidthExtensionRatio = 0.2f;
        private const int IndicatorYOffset = 0;
        private readonly Vector2 OutOfScreenPosition = new Vector2(0, -400);

        private RectTransform _rectTransform;

        protected DisposableComposite ConnectionD = new DisposableComposite();


        //=== Props ===============================================================

        public IBadgePoint BadgePoint { get; private set; }

        [Binding]
        public bool HasPoint { get; protected set; }

        [Binding]
        public bool IsSelected { get; protected set; }

        private static readonly PropertyBinder<GuiBadge, bool> IsSelectedBinder = PropertyBinder<GuiBadge>.Create(_ => _.IsSelected);

        /// <summary>
        /// Должен ли объект показываться на экране согласно логике игры
        /// </summary>
        [Binding]
        public bool IsVisibleLogically { get; protected set; }

        private static readonly PropertyBinder<GuiBadge, bool> IsVisibleLogicallyBinder = PropertyBinder<GuiBadge>.Create(_ => _.IsVisibleLogically);

        /// <summary>
        /// Виден ли объект на экране
        /// </summary>
        public ReactiveProperty<bool> IsOnScreenRp { get; protected set; } = new ReactiveProperty<bool>() {Value = false};

        public bool IsConnected { get; protected set; } = false;

        /// <summary>
        /// Должен ли объект показываться на экране согласно логике игры
        /// </summary>
        [Binding]
        public bool IsVisibleFinally { get; protected set; }

        private static readonly PropertyBinder<GuiBadge, bool> IsVisibleFinallyBinder = PropertyBinder<GuiBadge>.Create(_ => _.IsVisibleFinally);


        //=== Unity ===========================================================

        protected virtual void Awake() //1
        {
            _rectTransform = transform as RectTransform;
        }


        //=== Public ==========================================================

        public void SetOutPosition()
        {
            _rectTransform.anchoredPosition = OutOfScreenPosition;
        }

        public virtual void Connect([NotNull] IBadgePoint badgePoint) //2
        {
            if (IsConnected)
            {
                if (BadgePoint == badgePoint)
                    return;

                Disconnect();
            }

            IsConnected = true;
            BadgePoint = badgePoint;
            if (HasPoint != badgePoint.HasPoint)
            {
                HasPoint = badgePoint.HasPoint;
                NotifyPropertyChanged(nameof(HasPoint));
            }

            badgePoint.IsVisibleLogicallyRp.Bind(ConnectionD, this, IsVisibleLogicallyBinder);
            badgePoint.IsSelectedRp.Bind(ConnectionD, this, IsSelectedBinder);

            var isVisibleFinallyStream = badgePoint.IsVisibleLogicallyRp
                .Zip(ConnectionD, IsOnScreenRp)
                .Func(ConnectionD, (isVisibleLogically, isOnScreen) => isVisibleLogically && (isOnScreen || !HasPoint));
            isVisibleFinallyStream.Bind(ConnectionD, this, IsVisibleFinallyBinder);

            if (HasPoint)
            {
                isVisibleFinallyStream.Subscribe(
                    ConnectionD,
                    isVisibleFinally =>
                    {
                        if (!isVisibleFinally)
                            SetOutPosition();
                    },
                    SetOutPosition);
                CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdatedEvent);
            }
        }

        public virtual void Disconnect()
        {
            if (!IsConnected)
                return;

            IsConnected = false;
            if (HasPoint)
                CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdatedEvent);
            ConnectionD.Clear();
        }


        //=== Private =========================================================

        private void OnCameraUpdatedEvent(CinemachineBrain cinemachineBrain)
        {
            if (!IsVisibleLogically)
                return;

            IsOnScreenRp.Value = GetIsObjectOnScreen(out Vector2 anchoredPosition);
            if (!IsOnScreenRp.Value)
                return;

            _rectTransform.anchoredPosition = anchoredPosition;
        }

        private bool GetIsObjectOnScreen(out Vector2 anchoredPosition)
        {
            var camera = GameCamera.Camera;
            if (camera == null)
            {
                anchoredPosition = OutOfScreenPosition;
                return false;
            }

            var parentRectTransform = transform.parent as RectTransform;

            float width = parentRectTransform.rect.width;
            float height = parentRectTransform.rect.height;
            float widthExtension = width * ScreenWidthExtensionRatio;
            Vector3 viewPortCoords = camera.WorldToViewportPoint(BadgePoint.Position);
            if (viewPortCoords.z < 0)
            {
                //Объект за камерой
                anchoredPosition = OutOfScreenPosition;
                return false;
            }

            anchoredPosition = new Vector2(viewPortCoords.x * width, viewPortCoords.y * height + IndicatorYOffset);
            //в пределах ли ширины экрана с запасом widthExtension:
            return anchoredPosition.x >= -widthExtension && anchoredPosition.x <= width + widthExtension;
        }
    }
}