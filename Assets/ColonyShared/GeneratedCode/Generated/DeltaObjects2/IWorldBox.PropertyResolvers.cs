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
    public partial class WorldBox
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Def;
                    return true;
                case 11:
                    property = (T)(object)MapOwner;
                    return true;
                case 12:
                    property = (T)(object)StaticIdFromExport;
                    return true;
                case 13:
                    property = (T)(object)Name;
                    return true;
                case 14:
                    property = (T)(object)Prefab;
                    return true;
                case 15:
                    property = (T)(object)SomeUnknownResourceThatMayBeUseful;
                    return true;
                case 16:
                    property = (T)(object)OnSceneObjectNetId;
                    return true;
                case 17:
                    property = (T)(object)AutoAddToWorldSpace;
                    return true;
                case 18:
                    property = (T)(object)WorldSpaced;
                    return true;
                case 19:
                    property = (T)(object)MovementSync;
                    return true;
                case 20:
                    property = (T)(object)Inventory;
                    return true;
                case 21:
                    property = (T)(object)OpenMechanics;
                    return true;
                case 22:
                    property = (T)(object)OwnerInformation;
                    return true;
                case 23:
                    property = (T)(object)ContainerApi;
                    return true;
                case 24:
                    property = (T)(object)LimitedLifetime;
                    return true;
                case 25:
                    property = (T)(object)Destroyable;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Def))
                return 10;
            if (childName == nameof(MapOwner))
                return 11;
            if (childName == nameof(StaticIdFromExport))
                return 12;
            if (childName == nameof(Name))
                return 13;
            if (childName == nameof(Prefab))
                return 14;
            if (childName == nameof(SomeUnknownResourceThatMayBeUseful))
                return 15;
            if (childName == nameof(OnSceneObjectNetId))
                return 16;
            if (childName == nameof(AutoAddToWorldSpace))
                return 17;
            if (childName == nameof(WorldSpaced))
                return 18;
            if (childName == nameof(MovementSync))
                return 19;
            if (childName == nameof(Inventory))
                return 20;
            if (childName == nameof(OpenMechanics))
                return 21;
            if (childName == nameof(OwnerInformation))
                return 22;
            if (childName == nameof(ContainerApi))
                return 23;
            if (childName == nameof(LimitedLifetime))
                return 24;
            if (childName == nameof(Destroyable))
                return 25;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}