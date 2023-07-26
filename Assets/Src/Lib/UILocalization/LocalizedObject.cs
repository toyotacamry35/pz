using NLog;
using Uins;

namespace L10n
{
    public abstract class LocalizedObject : BindingViewModel
    {
        //=== Props ===========================================================

        protected static readonly Logger Logger = LogManager.GetLogger("UI");
        private bool _isSubscribedOnHolderInit;


        //=== Unity ===========================================================

        private void Awake()
        {
            OnAwake();

            if (L10nHolder.Instance == null)
            {
                L10nHolder.L10nHolderInited += OnL10nHolderInited;
                _isSubscribedOnHolderInit = true;
            }
            else
            {
                SubscribeOnLocalizationChanged();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (L10nHolder.Instance != null)
            {
                L10nHolder.Instance.LocalizationChanged -= OnLocalizationChanged;
                if (_isSubscribedOnHolderInit)
                {
                    _isSubscribedOnHolderInit = false;
                    L10nHolder.L10nHolderInited -= OnL10nHolderInited;
                }
            }

            WithinOnDestroy();
        }


        //=== Protected =======================================================

        protected virtual void OnAwake()
        {
        }

        protected virtual void WithinOnDestroy()
        {
        }

        protected abstract void GetLocalization(LocalizationSource localizationSource);

        protected void OnLocalizationChanged(string localizationName)
        {
            GetLocalization(L10nHolder.Instance);
        }

        protected void OnLocalizationChanged()
        {
            if (L10nHolder.Instance != null)
                OnLocalizationChanged(L10nHolder.Instance.CurrentLocalization.CultureData.Code);
        }


        //=== Private =========================================================

        private void OnL10nHolderInited()
        {
            if (L10nHolder.Instance.AssertIfNull(nameof(L10nHolder), gameObject))
                return;

            SubscribeOnLocalizationChanged();
        }

        private void SubscribeOnLocalizationChanged()
        {
            L10nHolder.Instance.LocalizationChanged += OnLocalizationChanged;
            OnLocalizationChanged();
        }
    }
}