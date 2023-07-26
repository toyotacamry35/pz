using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionStatesSource
    {
        IStream<InputActionTriggerState> Stream(InputActionTriggerDef action);
        IStream<InputActionValueState> Stream(InputActionValueDef action);
    }
}
