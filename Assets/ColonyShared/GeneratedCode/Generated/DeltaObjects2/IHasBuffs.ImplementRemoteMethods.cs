// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IBuffsImplementRemoteMethods
    {
        System.Threading.Tasks.Task<SharedCode.Wizardry.SpellId> TryAddBuffImpl(Scripting.ScriptingContext cast, SharedCode.Wizardry.BuffDef buffDef);
        System.Threading.Tasks.Task<bool> RemoveBuffImpl(SharedCode.Wizardry.SpellId buffId);
        System.Threading.Tasks.Task<bool> RemoveBuffImpl(SharedCode.Wizardry.BuffDef buffDef);
    }
}