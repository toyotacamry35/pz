// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using SharedCode.Logging;
using System.Collections.Generic;
using System.Linq;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class SimpleWorldEntity
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Name;
                    return true;
                case 11:
                    property = (T)(object)Prefab;
                    return true;
                case 12:
                    property = (T)(object)SomeUnknownResourceThatMayBeUseful;
                    return true;
                case 13:
                    property = (T)(object)OnSceneObjectNetId;
                    return true;
                case 14:
                    property = (T)(object)AutoAddToWorldSpace;
                    return true;
                case 15:
                    property = (T)(object)WorldSpaced;
                    return true;
                case 16:
                    property = (T)(object)MovementSync;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Name))
                return 10;
            if (childName == nameof(Prefab))
                return 11;
            if (childName == nameof(SomeUnknownResourceThatMayBeUseful))
                return 12;
            if (childName == nameof(OnSceneObjectNetId))
                return 13;
            if (childName == nameof(AutoAddToWorldSpace))
                return 14;
            if (childName == nameof(WorldSpaced))
                return 15;
            if (childName == nameof(MovementSync))
                return 16;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}