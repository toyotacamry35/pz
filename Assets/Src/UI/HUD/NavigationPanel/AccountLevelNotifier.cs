using ReactivePropsNs;

namespace Uins
{
    public class AccountLevelNotifier : NavigationIndicatorNotifier
    {
        private static bool _isSubscribed;
        private static int _currentAccountLevel;
        private static DisposableComposite _d = new DisposableComposite();


        //=== Props ===========================================================

        public static ListStream<AccountLevelNotifier> Notifiers { get; } = new ListStream<AccountLevelNotifier>();

        public bool Active
        {
            set => IsDisplayable = value;
        }


        //=== Unity ===========================================================

        protected override void Start()
        {
            base.Start();
            if (NavIndicatorDefRef.Target == null)
                return;

            if (!_isSubscribed && SurvivalGuiNode.Instance?.AccountLevelStream != null)
            {
                SurvivalGuiNode.Instance.AccountLevelStream.Action(_d, OnAccLevelChanged);
                _isSubscribed = true;
            }

            Notifiers.Add(this);
            IsDisplayable = GetIsDisplayable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Notifiers.Contains(this))
                Notifiers.Remove(this);
        }

        protected override bool GetIsDisplayable()
        {
            return (NavIndicatorDef?.AccLevel ?? int.MaxValue) <= _currentAccountLevel;
        }


        //=== Private =========================================================

        private static void OnAccLevelChanged(int currentAccountLevel)
        {
            _currentAccountLevel = currentAccountLevel;
            for (int i = 0; i < Notifiers.Count; i++)
            {
                var notifier = Notifiers[i];
                if (notifier != null)
                    notifier.IsDisplayable = notifier.GetIsDisplayable();
            }
        }
    }
}