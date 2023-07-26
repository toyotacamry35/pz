using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;

namespace ColonyShared.SharedCode.Input
{
    public class InputBindingsDef : BaseResource
    {
        public ResourceRef<InputBindingDef>[] Bindings { get; set; }
        public ResourceRef<InputActionsListDef> BlockList { get; set; }
    }
}
