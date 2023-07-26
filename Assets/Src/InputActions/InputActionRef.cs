using System;
using Assets.Src.ResourceSystem;
using ColonyShared.SharedCode.InputActions;

namespace Src.InputActions
{
    [Serializable] public class InputActionTriggerRef : JdbRef<InputActionTriggerDef> {}

    [Serializable] public class InputActionValueRef : JdbRef<InputActionValueDef> {}
}