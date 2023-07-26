namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionHandlersFactory
    {
        T Create<T>(InputActionDef action, IInputActionHandlerDescriptor handler, int bindingId) where T : IInputActionHandler;
    }
}