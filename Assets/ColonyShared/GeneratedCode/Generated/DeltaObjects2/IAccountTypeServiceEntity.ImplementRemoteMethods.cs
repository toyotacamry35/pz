// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IAccountTypeServiceEntityImplementRemoteMethods
    {
        System.Threading.Tasks.Task<long> GetAccountTypeImpl(System.Guid userId);
        System.Threading.Tasks.Task SetAccountTypeImpl(System.Guid userId, long accountType);
        System.Threading.Tasks.Task RemoveAccountTypeImpl(System.Guid userId);
    }
}