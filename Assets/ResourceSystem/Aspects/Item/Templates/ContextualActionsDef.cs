using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;

namespace SharedCode.Aspects.Item.Templates
{
    public class ContextualActionsDef : BaseResource
    {
        public Dictionary<ResourceRef<InputActionDef>, Dictionary<int, ResourceRef<ContextualActionDef>>> SpellsByAction =
            new Dictionary<ResourceRef<InputActionDef>, Dictionary<int, ResourceRef<ContextualActionDef>>>();
    }
}