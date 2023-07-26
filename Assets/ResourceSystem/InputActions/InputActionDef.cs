using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.InputActions
{
    public abstract class InputActionDef : BaseResource
    {
        public string Description;
        public string ActionToString() => ____GetDebugRootName();
        
        public static readonly InputActionDef Empty = new InputActionNull();

        public class InputActionNull : InputActionDef {}
    }

    
    public class InputActionValueDef : InputActionDef
    {
    }
    
    public class InputActionTriggerDef : InputActionDef
    {
    }
    
    public class InputActionMetaTriggerDef : InputActionTriggerDef
    {
        public ResourceArray<InputActionTriggerDef> Actions;
        public ResourceArray<InputActionTriggerDef> NotActions;
    }
}