using System;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public abstract class InputActionHandlerRedirect<TState> 
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IListener<TState> _listener;
        private readonly int _bindingId;
        private readonly InputActionDef _action;

        protected InputActionHandlerRedirect(InputActionDef action, IListener<TState> listener, int bindingId)
        {
            _listener = listener ?? throw new ArgumentNullException(nameof(listener));
            _bindingId = bindingId;
            _action = action;
        }

        public bool PassThrough => false;
        
        public void ProcessEvent(TState @event, InputActionHandlerContext ctx, bool inactive)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Redirect event | Binding:#{_bindingId} RedirectTo:{_action.ActionToString()} Event:{@event}").Write();
            _listener.OnNext(@event);
        }

        public void Dispose()
        {
            _listener.OnNext(default);
        }

        public override string ToString() => $"{GetType().Name}(Action:{_action.ActionToString()})";
    }
    
    public class InputActionHandlerTriggerRedirect : InputActionHandlerRedirect<InputActionTriggerState>, IInputActionTriggerHandler
    {
        public InputActionHandlerTriggerRedirect(InputActionTriggerDef action, IInputActionStatesRecipient actionsStatesListeners, int bindingId) 
            : base(action, actionsStatesListeners.Listener(action), bindingId) {}
    }
    
    
    public class InputActionHandlerValueRedirect : InputActionHandlerRedirect<InputActionValueState>, IInputActionValueHandler
    {
        public InputActionHandlerValueRedirect(InputActionValueDef action, IInputActionStatesRecipient actionsStatesListeners, int bindingId)
            : base(action, actionsStatesListeners.Listener(action), bindingId) {}
    }
}
