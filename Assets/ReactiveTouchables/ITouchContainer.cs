using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public interface ITouchContainer<T> where T : IDeltaObject {
        bool WasSucessful { get; }
        void SetCallbacks(Action<T> onSuccess, Action onFail);
        Task Connect();
    }
}
