namespace Uins.Inventory
{
    public abstract class BaseSideContextViewTarget : BindingViewModel, IContextViewTargetWithParams
    {
        //=== Props ===========================================================

        protected virtual ContextViewParams.LayoutType Layout => ContextViewParams.LayoutType.ExtraSpace;

        public abstract InventoryTabType? TabType { get; }

        private ContextViewParams _contextViewParams;


        //=== Public ==========================================================

        private void Awake()
        {
            _contextViewParams = new ContextViewParams() {Layout = Layout};
        }

        public ContextViewParams GetContextViewParamsForOpening()
        {
            return _contextViewParams;
        }
    }
}