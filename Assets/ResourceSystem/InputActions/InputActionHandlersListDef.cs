using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlersListDef : BaseResource
    {
        [UsedImplicitly] public Dictionary<ResourceRef<InputActionDef>, ResourceRef<InputActionHandlerDef>> Handlers;
    }
}