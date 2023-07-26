using System;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionLayer
    {
        void AddBinding(object handlerOwner, InputActionDef action, IInputActionHandlerDescriptor handler, InputActionHandlerContext ctx);

        void RemoveBinding(object bindingOwner, InputActionDef action, IInputActionHandlerDescriptor handler);

        void RemoveBinding(object bindingOwner, InputActionDef action);

        void RemoveBindings(object bindingOwner);        
    }
}