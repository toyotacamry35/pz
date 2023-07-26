using System.Collections.Generic;
using GeneratedDefsForSpells;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionHandlerDescriptor
    {
        string HandlerToString();
    }
    
    public interface IInputActionTriggerHandlerDescriptor : IInputActionHandlerDescriptor {}
    
    public interface IInputActionValueHandlerDescriptor : IInputActionHandlerDescriptor {}

    public interface IInputActionHandlerNullDescriptor : IInputActionTriggerHandlerDescriptor, IInputActionValueHandlerDescriptor {}

    public interface IInputActionHandlerSpellBreakerDescriptor : IInputActionTriggerHandlerDescriptor
    {
        When When { get; }
        FinishReasonType FinishReason { get; }
    }

    public interface IInputActionHandlerSpellDescriptor : IInputActionTriggerHandlerDescriptor
    {
        SpellDef Spell { get; }
        IEnumerable<SpellParameterDef> Parameters { get; }
        bool Chain { get; }
    }

    public interface IInputActionHandlerSpellOnceDescriptor : IInputActionTriggerHandlerDescriptor
    {
        SpellDef Spell { get; }
        IEnumerable<SpellParameterDef> Parameters { get; }
        bool Chain { get; }
    }
    
    public interface IInputActionHandlerSpellContinuousDescriptor : IInputActionTriggerHandlerDescriptor
    {
        SpellDef Spell { get; }
        IEnumerable<SpellParameterDef> Parameters { get; }
        SpellRestartReasonMask RestartIfReason { get; }
        float FinishDelay { get; }
        SpellFinishMethod FinishMethod { get; }
        bool Chain { get; }
    }
    
    public interface IInputActionHandlerInteractionDescriptor : IInputActionTriggerHandlerDescriptor {}

    public interface IInputActionHandlerInputWindowDescriptor : IInputActionTriggerHandlerDescriptor
    {
        long ActivationTime { get; }
    }

    public interface IInputActionHandlerCombinedDescriptor : IInputActionTriggerHandlerDescriptor
    {
        IEnumerable<IInputActionTriggerHandlerDescriptor> Handlers { get; }
    }

    public interface IInputActionHandlerLocomotionTriggerDescriptor : IInputActionTriggerHandlerDescriptor
    {
        string Input { get; }
    }
    
    public interface IInputActionHandlerLocomotionAxisDescriptor : IInputActionValueHandlerDescriptor
    {
        string Input { get; }
    }

    public interface IInputActionHandlerLocomotionTriggerToAxisDescriptor : IInputActionTriggerHandlerDescriptor
    {
        string Input { get; }
        float Value { get; }
    }
   
    public interface IInputActionHandlerCameraDescriptor : IInputActionValueHandlerDescriptor
    {
        CameraAxis Axis { get; }
    }
    
    public enum CameraAxis { X, Y }

    public interface IInputActionHandlerRedirectDescriptor
    {
        InputActionDef Action { get; }  
    }
    
    public interface IInputActionHandlerTriggerRedirectDescriptor : IInputActionHandlerRedirectDescriptor, IInputActionTriggerHandlerDescriptor
    {
        new InputActionTriggerDef Action { get; }  
    }
    
    public interface IInputActionHandlerValueRedirectDescriptor : IInputActionHandlerRedirectDescriptor, IInputActionValueHandlerDescriptor
    {
        new InputActionValueDef Action { get; }  
    }
    
    public interface IInputActionBlockerDescriptor : IInputActionHandlerNullDescriptor {}
}