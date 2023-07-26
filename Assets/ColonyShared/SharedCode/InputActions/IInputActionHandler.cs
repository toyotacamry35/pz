using System;

namespace ColonyShared.SharedCode.InputActions
{

    /// <summary>
    /// Хэндлеры не обязаны быть thread safe так как InputActionProcessor осуществляет вызовы метода Execute строго по очереди. 
    /// Dispose, так же, вызывается только после того, когда будет обработано последнее событие.  
    /// </summary>
    public interface IInputActionHandler : IDisposable
    {
        bool PassThrough { get; }
    }

    
    public interface IInputActionTriggerHandler : IInputActionHandler
    {
        void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive);
    }
    
    
    public interface IInputActionValueHandler : IInputActionHandler
    {
        void ProcessEvent(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive);
    }

    public interface IInputActionHandlerSpellBreaker
    {}
}
