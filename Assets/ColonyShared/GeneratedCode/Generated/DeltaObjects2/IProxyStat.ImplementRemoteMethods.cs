// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IProxyStatImplementRemoteMethods
    {
        System.Threading.Tasks.Task ProxySubscribeImpl(SharedCode.EntitySystem.PropertyAddress containerAddress);
        System.Threading.Tasks.ValueTask InitializeImpl(Assets.Src.Aspects.Impl.Stats.StatDef statDef, bool resetState);
        System.Threading.Tasks.ValueTask<bool> RecalculateCachesImpl(bool calcersOnly);
        System.Threading.Tasks.ValueTask<float> GetValueImpl();
    }
}