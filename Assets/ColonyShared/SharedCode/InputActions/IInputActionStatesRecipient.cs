using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionStatesRecipient
    {
        IListener<InputActionTriggerState> Listener(InputActionTriggerDef action);

        IListener<InputActionValueState> Listener(InputActionValueDef action);
    }
}