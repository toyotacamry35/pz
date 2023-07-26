using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IComputableStateMachineOwnerDef
    {
        ResourceRef<ComputableStateMachineDef> ComputableStateMachine { get; set; }
    }
}
