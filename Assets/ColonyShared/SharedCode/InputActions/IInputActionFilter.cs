using System;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionFilter<TEvent>  : IDisposable, IListener<TEvent>, IStream<TEvent>
    {
        void Output(Action<TEvent, IInputActionFilter<TEvent>> listener);
        
        void OnEvent(TEvent @event);
    }
}