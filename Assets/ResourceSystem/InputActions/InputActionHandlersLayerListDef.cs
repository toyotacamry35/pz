using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlersLayerListDef : BaseResource
    {
        [UsedImplicitly] public ResourceRef<InputActionHandlersLayerDef> Layer;
        [UsedImplicitly] public Dictionary<ResourceRef<InputActionDef>,ResourceRef<InputActionHandlerDef>> Handlers;
    }
}