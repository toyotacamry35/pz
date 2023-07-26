using System.Collections.Generic;
using System.Linq;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlerCombined : IInputActionTriggerHandler
    {
        private readonly IInputActionTriggerHandler[] _handlers;

        public InputActionHandlerCombined(params IInputActionTriggerHandler[] handlers)
        {
            _handlers = handlers ?? new IInputActionTriggerHandler[0];
        }
        
        public InputActionHandlerCombined(IEnumerable<IInputActionTriggerHandler> handlers)
        {
            _handlers = (handlers ?? Enumerable.Empty<IInputActionTriggerHandler>()).ToArray();
        }

        public bool PassThrough
        {
            get
            {
                foreach (var h in _handlers)
                    if (h.PassThrough)
                        return true;
                return false;
            }
        }

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            foreach (var h in _handlers)
                h.ProcessEvent(@event, ctx, inactive);
        }

        public void Dispose()
        {
            foreach (var h in _handlers)
                h.Dispose();
        }

        public override string ToString()
        {
            return StringBuildersPool.Get.Append("[").Append(string.Join(", ", (object[])_handlers)).Append("]").ToStringAndReturn();
        }
    }
}