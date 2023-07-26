namespace Uins
{
    public abstract class TypedContextView<T> : BindingViewModel where T : BindingViewModel
    {
        public delegate void TypedTargetChangedDelegate(T target);

        public event TypedTargetChangedDelegate TargetChanged;


        //=== Props ===========================================================

        private T _currentTarget;

        public T CurrentTarget
        {
            get => _currentTarget;
            private set
            {
                if (_currentTarget != value)
                {
                    var old = _currentTarget;
                    _currentTarget = value;
                    OnCurrentTargetChanged(old, _currentTarget);
                }
            }
        }


        //=== Public ==========================================================

        public void TakeContext(T contextViewTarget)
        {
            if (contextViewTarget == CurrentTarget)
                return;

            CurrentTarget = contextViewTarget;
            TargetChanged?.Invoke(CurrentTarget);
        }


        //=== Private =========================================================

        protected abstract void OnCurrentTargetChanged(T prevContextViewTarget, T newContextViewTarget);
    }
}