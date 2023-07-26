using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using L10n;

namespace ColonyShared.SharedCode.Input
{
    public class InputSlotDef : BaseResource
    {
        public LocalizedString DescriptionLs { get; set; } // описание действия для пользователя
        public ResourceArray<InputActionValueDef> Value { get; set; }
        public ResourceArray<InputActionTriggerDef> Press { get; set; }
        public ResourceArray<InputActionTriggerDef> Release { get; set; }
        public ResourceArray<InputActionTriggerDef> Hold { get; set; }
        public ResourceArray<InputActionTriggerDef> Click { get; set; }
        public float HoldTime { get; set; } = 0.5f;
        public float Threshold { get; set; } = 0.01f;

        public bool HasAction(InputActionDef action) => 
            Value?.Contains(action) == true || 
            Press?.Contains(action) == true || 
            Release?.Contains(action) == true || 
            Hold?.Contains(action)  == true || 
            Click?.Contains(action) == true;
    }
}