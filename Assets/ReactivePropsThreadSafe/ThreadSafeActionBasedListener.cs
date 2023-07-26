using System;

namespace ReactivePropsNs.ThreadSafe
{
    public class ActionBasedListener<T> : IListener<T>
    {
        private Action<T> _onNextAction;
        private Action _onCompleteAction;

        public bool IsDisposed { get; private set; }


        //=== Ctor ============================================================

        public ActionBasedListener(Action<T> onNextAction, Action onCompleteAction)
        {
            _onNextAction = onNextAction;
            _onCompleteAction = onCompleteAction;
        }

        ~ActionBasedListener()
        {
            if (IsDisposed)
                return;

            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
        }


        //=== Public ==========================================================

        public void OnNext(T value)
        {
            if (IsDisposed)
                return;

            _onNextAction?.Invoke(value);
        }

        public void OnCompleted()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            var onComplete = _onCompleteAction;
            _onNextAction = null;
            _onCompleteAction = null;
            onComplete?.Invoke();
            GC.SuppressFinalize(this);
        }
    }
}