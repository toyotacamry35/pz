using System;
using System.Linq;
using JetBrains.Annotations;

namespace ColonyShared.SharedCode.InputActions
{
    public partial class InputActionsProcessor
    {
        private class InternalHandlersFactory : IInputActionHandlersFactory
        {
            private readonly IInputActionHandlersFactory _factory;
            private readonly IInputActionStatesRecipient _redirector;
            private readonly Action<InputActionState, int> _loopback;

            public InternalHandlersFactory(
                [NotNull] IInputActionHandlersFactory factory, 
                [NotNull] IInputActionStatesRecipient redirector, 
                [NotNull] Action<InputActionState,int> loopback)
            {
                _factory = factory ?? throw new ArgumentNullException(nameof(factory));
                _redirector = redirector ?? throw new ArgumentNullException(nameof(redirector));
                _loopback = loopback ?? throw new ArgumentNullException(nameof(loopback));
            }

            public T Create<T>(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId) where T : IInputActionHandler
            {
                switch (desc)
                {
                    case null:
                        throw new ArgumentNullException(nameof(desc));
                    case IInputActionHandlerNullDescriptor _:
                        return (T) InputActionHandler.Null;
                    case IInputActionHandlerInputWindowDescriptor d:
                        return (T) (IInputActionTriggerHandler) new InputActionHandlerInputWindow(d.ActivationTime, bindingId, _loopback);
                    case IInputActionHandlerCombinedDescriptor d:
                        return (T) (IInputActionTriggerHandler) new InputActionHandlerCombined(d.Handlers.Select(x => Create<IInputActionTriggerHandler>(action, x, bindingId)));
                    case IInputActionHandlerTriggerRedirectDescriptor d:
                        return (T) (IInputActionTriggerHandler) new InputActionHandlerTriggerRedirect(d.Action, _redirector, bindingId);
                    case IInputActionHandlerValueRedirectDescriptor d:
                        return (T) (IInputActionValueHandler) new InputActionHandlerValueRedirect(d.Action, _redirector, bindingId);
                    default:
                        return _factory.Create<T>(action, desc, bindingId);
                }
            }
        }
    }
}
