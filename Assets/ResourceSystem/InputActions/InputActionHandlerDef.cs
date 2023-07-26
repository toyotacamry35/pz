using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using GeneratedDefsForSpells;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public abstract class InputActionHandlerDef : BaseResource, IInputActionHandlerDescriptor
    {
        public string HandlerToString()
        {
            var strImpl = ToStringImpl();
            
            return $"{GetType().Name}.{this.____GetDebugShortName()}" + (string.IsNullOrEmpty(strImpl) ? string.Empty : $"({strImpl})");
        }

        protected abstract string ToStringImpl();
    }
    
    
    
    public abstract class InputActionTriggerHandlerDef : InputActionHandlerDef, IInputActionTriggerHandlerDescriptor
    {
    }

    
    
    public abstract class InputActionValueHandlerDef : InputActionHandlerDef, IInputActionValueHandlerDescriptor
    {
    }
    
    
    
    public abstract class InputActionHandlerSpellBaseDef : InputActionTriggerHandlerDef
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<SpellDef> Spell { get; [UsedImplicitly] set; }

         public ResourceArray<SpellParameterDef> Parameters { get; [UsedImplicitly] set; }

         public bool Chain { get; [UsedImplicitly] set; } = true; // останавливать спелл из которого был активирован хэндлер
        
        protected override string ToStringImpl() => $"Spell:{Spell.Target?.____GetDebugAddress()}";
    }



    public class InputActionHandlerSpellDef : InputActionHandlerSpellBaseDef, IInputActionHandlerSpellDescriptor
    {
        SpellDef IInputActionHandlerSpellDescriptor.Spell => Spell.Target;

        IEnumerable<SpellParameterDef> IInputActionHandlerSpellDescriptor.Parameters => Parameters;
    }



    public class InputActionHandlerSpellOnceDef : InputActionHandlerSpellBaseDef, IInputActionHandlerSpellOnceDescriptor
    {
        SpellDef IInputActionHandlerSpellOnceDescriptor.Spell => Spell.Target;

        IEnumerable<SpellParameterDef> IInputActionHandlerSpellOnceDescriptor.Parameters => Parameters;
    }
    
    
    
    public class InputActionHandlerSpellContinuousDef : InputActionTriggerHandlerDef, IInputActionHandlerSpellContinuousDescriptor
    {        
        [JsonProperty(Required = Required.Always)] public ResourceRef<SpellDef> Spell { get; [UsedImplicitly] set; }
        
        public ResourceRef<SpellParameterDef>[] Parameters { get; [UsedImplicitly] set; }

        public SpellRestartReasonMask RestartIfReason { get; [UsedImplicitly] set; }
        
        public bool Chain { get; [UsedImplicitly] set; } = true;  // останавливать спелл из которого был активирован хэндлер

        public float FinishDelay { get; [UsedImplicitly] set; }
        
        
        public SpellFinishMethod FinishMethod { get; [UsedImplicitly] set; } = SpellFinishMethod.Stop;
        
        SpellDef IInputActionHandlerSpellContinuousDescriptor.Spell => Spell.Target;
        
        IEnumerable<SpellParameterDef> IInputActionHandlerSpellContinuousDescriptor.Parameters => Parameters?.Select(x => x.Target);

        protected override string ToStringImpl() => $"Spell:{Spell.Target?.____GetDebugAddress()} {(RestartIfReason != 0 ? $"RestartIf:{RestartIfReason}" : string.Empty)}";
    }

    
    
    public class InputActionHandlerSpellBreakerDef : InputActionTriggerHandlerDef, IInputActionHandlerSpellBreakerDescriptor
    {
         public When When { get; [UsedImplicitly] set; } = When.Activated;
         public FinishReasonType FinishReason { get; [UsedImplicitly] set; } = FinishReasonType.Success;
         
         protected override string ToStringImpl() => String.Empty;
    }

    
    
    public class InputActionHandlerLocomotionAxisDef : InputActionValueHandlerDef, IInputActionHandlerLocomotionAxisDescriptor
    {
         public string Input { get; [UsedImplicitly] set; }

        protected override string ToStringImpl() => $"Input:{Input}";
    }

    
    
    public class InputActionHandlerLocomotionTriggerDef : InputActionTriggerHandlerDef, IInputActionHandlerLocomotionTriggerDescriptor
    {
         public string Input { get; [UsedImplicitly] set; }

        protected override string ToStringImpl() => $"Input:{Input}";
    }

    
    
    public class InputActionHandlerLocomotionTriggerToAxisDef : InputActionTriggerHandlerDef, IInputActionHandlerLocomotionTriggerToAxisDescriptor
    {
         public string Input { get; [UsedImplicitly] set; }
         public float Value { get; [UsedImplicitly] set; }

        protected override string ToStringImpl() => $"Input:{Input} Value:{Value}";
    }
    
    
    
    public class InputActionHandlerCameraDef : InputActionValueHandlerDef, IInputActionHandlerCameraDescriptor
    {        
        [JsonProperty(Required = Required.Always)] public CameraAxis Axis { get; [UsedImplicitly] set; }
        
        protected override string ToStringImpl() => $"Camera:{Axis}";
    }

    
    
    public class InputActionHandlerInteractionDef : InputActionTriggerHandlerDef, IInputActionHandlerInteractionDescriptor
    {
        protected override string ToStringImpl() => string.Empty;
    }    

    
    
    public class InputActionHandlerCombinedDef : InputActionTriggerHandlerDef, IInputActionHandlerCombinedDescriptor
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<InputActionTriggerHandlerDef>[] Handlers { get; [UsedImplicitly] set; }

        IEnumerable<IInputActionTriggerHandlerDescriptor> IInputActionHandlerCombinedDescriptor.Handlers => Handlers.Select(x => x.Target); 
        protected override string ToStringImpl() => $"{string.Join(", ", Handlers.Select(x => x.Target.HandlerToString()))}";
    }


    public class InputActionHandlerTriggerRedirectDef : InputActionTriggerHandlerDef, IInputActionHandlerTriggerRedirectDescriptor
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<InputActionTriggerDef> Action { get; [UsedImplicitly] set; }
        InputActionTriggerDef IInputActionHandlerTriggerRedirectDescriptor.Action => Action;
        InputActionDef IInputActionHandlerRedirectDescriptor.Action => Action;
        protected override string ToStringImpl() => $"Action:{Action.Target?.ActionToString()}";
    }

    
    public class InputActionHandlerValueRedirectDef : InputActionValueHandlerDef, IInputActionHandlerValueRedirectDescriptor
    {
        [JsonProperty(Required = Required.Always)] public ResourceRef<InputActionValueDef> Action { get; [UsedImplicitly] set; }
        InputActionValueDef IInputActionHandlerValueRedirectDescriptor.Action => Action;
        InputActionDef IInputActionHandlerRedirectDescriptor.Action => Action;
        protected override string ToStringImpl() => $"Action:{Action.Target?.ActionToString()}";
    }

    [Flags] public enum SpellRestartReasonMask { None = 0, Success = 0x1, Fail = 0x2, FailOnStart = 0x4, Any = Success | Fail | FailOnStart }
    
    public enum When { Activated, Active, Deactivated }
}