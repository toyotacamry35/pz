using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class SyncTouchContainer<T> : ITouchContainer<T> where T : IDeltaObject
    {
        private T _value;
        public SyncTouchContainer(T value)
        {
            _value = value;
        }

        public bool WasSucessful { get; private set; }

        public Task Connect()
        {
            WasSucessful = true;
            var onSucess = _onSuccess;
            _onSuccess = null;
            try
            {
                onSucess?.Invoke(_value);
            }
            catch (Exception e)
            {
                ReactiveLogs.Logger.IfError()?.Message($"{GetType().Name}.Connect() was failed", e.Message, e.StackTrace).Write();
            }
            finally
            {
                _value = default;
            }
            return Task.CompletedTask;
        }

        private Action<T> _onSuccess = null;
        public void SetCallbacks(Action<T> onSuccess, Action onFail)
        {
            _onSuccess += onSuccess;
        }
    }
}
