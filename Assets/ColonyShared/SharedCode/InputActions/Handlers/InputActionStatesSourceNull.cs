using ColonyShared.SharedCode.InputActions;
using ReactivePropsNs.ThreadSafe;

namespace Src.InputActions
{
    public class InputActionStatesSourceNull : IInputActionStatesSource
    {
        private readonly StreamProxy<InputActionTriggerState> _dummyTrigger = new StreamProxy<InputActionTriggerState>();
        private readonly StreamProxy<InputActionValueState> _dummyValue = new StreamProxy<InputActionValueState>();

        public IStream<InputActionTriggerState> Stream(InputActionTriggerDef action) => _dummyTrigger;
        public IStream<InputActionValueState> Stream(InputActionValueDef action) => _dummyValue;

        public void Dispose()
        {
            _dummyTrigger?.Dispose();
            _dummyValue?.Dispose();
        }
    }
}
