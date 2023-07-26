namespace Uins
{
    public abstract class NavigationIndicatorNotifierBase : BindingViewModel
    {
        public abstract INavigationIndicatorSettings NavigationIndicatorSettings { get; }

        public abstract IMapIndicatorSettings MapIndicatorSettings { get; }

        private bool _isNotificationSended;


        //=== Props ===========================================================

        private bool _isDisplayable;

        public bool IsDisplayable
        {
            get => _isDisplayable;
            protected set
            {
                if (_isDisplayable != value)
                {
                    _isDisplayable = value;
                    OnDisplayableChanged();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (NavigationProvider.Instance == null)
                return;

            IsDisplayable = false;
        }


        //=== Private =========================================================

        protected virtual void OnDisplayableChanged()
        {
            if (_isDisplayable)
            {
                if (!_isNotificationSended)
                    SendNotification();
            }
            else
            {
                if (_isNotificationSended)
                    RevokeNotification();
            }
        }

        private void SendNotification()
        {
            NavigationProvider.Instance.AddNavigationIndicatorNotifier(this);
            _isNotificationSended = true;
        }

        private void RevokeNotification()
        {
            NavigationProvider.Instance?.RemoveNavigationIndicatorNotifier(this);
            _isNotificationSended = false;
        }
    }
}
