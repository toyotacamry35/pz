// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IBankerEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<ResourceSystem.Utils.OuterRef> GetBankCellImpl(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, ResourceSystem.Utils.OuterRef bankCell);
        System.Threading.Tasks.Task DestroyBankCellsImpl(Assets.ResourceSystem.Aspects.Banks.BankDef bankDef, SharedCode.EntitySystem.PropertyAddress corpseInventoryAddress);
    }
}