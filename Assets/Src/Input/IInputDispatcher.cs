using System.Collections.Generic;
using ColonyShared.SharedCode.Input;
using ColonyShared.SharedCode.InputActions;

namespace Src.Input
{
    public interface IInputDispatcher
    {
        void UpdateStates(Dictionary<InputActionDef, InputActionState> states);

        void ResetStates();

        IEnumerable<InputSourceDef> GetBindingForAction(InputActionDef action);

        InputSlotDef GetSlotForAction(InputActionDef action);
        
        bool IsActionBlocked(InputActionDef action);
    }
}