using GeneratedDefsForSpells;

namespace ColonyShared.SharedCode.InputActions
{
    
    public class InputActionBlockerDescriptor : IInputActionBlockerDescriptor
    {
        string IInputActionHandlerDescriptor.HandlerToString() => "Blocker";
    }
    
    
    public class InputActionHandlerSpellBreakerDescriptor: IInputActionHandlerSpellBreakerDescriptor
    {
        public When When { get; set; } = When.Active;
        public FinishReasonType FinishReason { get; set; } = FinishReasonType.Success;

        string IInputActionHandlerDescriptor.HandlerToString() => "SpellBreaker";
    }
    
    
    public class InputActionHandlerInputWindowDescriptor: IInputActionHandlerInputWindowDescriptor
    {
        public InputActionHandlerInputWindowDescriptor(long activationTime) => ActivationTime = activationTime;
        public long ActivationTime { get; }
        string IInputActionHandlerDescriptor.HandlerToString() => $"InputActionHandlerInputWindowDesc(Time:{ActivationTime})";
    }


    public class InputActionHandlerNullDescriptor : IInputActionHandlerNullDescriptor, IInputActionTriggerHandlerDescriptor, IInputActionValueHandlerDescriptor
    {
        public string HandlerToString() => "Null";
    }
}
