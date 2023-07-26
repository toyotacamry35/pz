using System.Collections.Generic;
using ReactivePropsNs.ThreadSafe;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionBindingsSource
    {
        IStream<IEnumerable<InputActionBindingsStream>> Bindings();
        IStream<bool> BindingsWait();
    }

    public readonly struct InputActionBindingsStream
    {
        public readonly InputActionDef Action;
        public readonly IStream<IEnumerable<InputActionBinding>> Stream;

        public InputActionBindingsStream(InputActionDef action, IStream<IEnumerable<InputActionBinding>> stream)
        {
            Action = action;
            Stream = stream;
        }
    }
}