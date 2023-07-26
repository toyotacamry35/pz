namespace ColonyShared.SharedCode.InputActions
{
    
    public interface IInputActionNullHandler : IInputActionTriggerHandler, IInputActionValueHandler {}
    
    public static class InputActionHandler
    {
        public static readonly IInputActionNullHandler Null = new InputActionHandlerNull();

        private class InputActionHandlerNull : IInputActionNullHandler
        {
            public bool PassThrough => false;

            public void ProcessEvent(InputActionValueState @event, InputActionHandlerContext ctx, bool inactive)
            {
            }

            public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
            {
            }

            public void Dispose()
            {
            }

            public override string ToString()
            {
                return "Null";
            }
        }
    }
}