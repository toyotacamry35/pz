using System;

namespace ColonyShared.SharedCode.InputActions
{
    public readonly struct InputActionBinding
    {
        public readonly int Id;
        public readonly InputActionDef Action;
        public readonly IInputActionHandlerDescriptor Handler;
        public readonly InputActionHandlerContext Context;

        public InputActionBinding(int id, InputActionDef action, IInputActionHandlerDescriptor handler, InputActionHandlerContext context)
        {
            Id = id;
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Context = context;
        }
        
        public override string ToString() => $"Binding#{Id} {Action.ActionToString()} <-> {Handler.HandlerToString()}";
    }
}