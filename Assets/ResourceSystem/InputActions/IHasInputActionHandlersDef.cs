using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IHasInputActionHandlersDef
    {
        Dictionary<ResourceRef<InputActionHandlersLayerDef>,Dictionary<ResourceRef<InputActionDef>,ResourceRef<InputActionHandlerDef>>> InputActionHandlers { get; }
    }
}