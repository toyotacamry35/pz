// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IWorldMachineImplementRemoteMethods
    {
        System.Threading.Tasks.Task<SharedCode.Entities.RecipeOperationResult> SetRecipeActivityImpl(Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef recipeDef, bool activate);
        System.Threading.Tasks.Task<SharedCode.Entities.RecipeOperationResult> SetRecipePriorityImpl(Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef recipeDef, int priority);
        System.Threading.Tasks.Task UpdateCraftProgressInfoImpl();
        System.Threading.Tasks.Task<bool> NameSetImpl(string value);
        System.Threading.Tasks.Task<bool> PrefabSetImpl(string value);
        System.Threading.Tasks.Task<SharedCode.Entities.RecipeOperationResult> SetActiveImpl(bool activate);
        System.Threading.Tasks.Task<bool> CraftProgressInfoSetImpl(SharedCode.DeltaObjects.ICraftProgressInfo value);
        System.Threading.Tasks.Task<ResourceSystem.Utils.OuterRef> GetOpenOuterRefImpl(ResourceSystem.Utils.OuterRef oref);
    }
}