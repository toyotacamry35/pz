// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IBotCoordinatorImplementRemoteMethods
    {
        System.Threading.Tasks.Task RegisterImpl();
        System.Threading.Tasks.Task InitializeImpl(GeneratedCode.Custom.Config.MapDef mapDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> botsRefs, SharedCode.AI.LegionaryEntityDef botConfig);
        System.Threading.Tasks.Task ActivateBotsImpl(System.Guid account, System.Collections.Generic.List<System.Guid> botsIds);
        System.Threading.Tasks.Task DeactivateBotsImpl(System.Guid account);
    }
}