// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IWorldPersonalMachineEngineImplementRemoteMethods
    {
        System.Threading.Tasks.Task<ResourceSystem.Utils.OuterRef> GetOrAddMachineImpl(SharedCode.Aspects.Item.Templates.WorldPersonalMachineDef def, ResourceSystem.Utils.OuterRef key);
        System.Threading.Tasks.Task RemoveMachineImpl(ResourceSystem.Utils.OuterRef key);
    }
}