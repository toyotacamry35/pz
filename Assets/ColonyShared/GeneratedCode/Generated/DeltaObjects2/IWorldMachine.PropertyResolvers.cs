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
    public partial class WorldMachine
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Inventory;
                    return true;
                case 11:
                    property = (T)(object)FuelContainer;
                    return true;
                case 12:
                    property = (T)(object)OutputContainer;
                    return true;
                case 13:
                    property = (T)(object)PriorityQueue;
                    return true;
                case 14:
                    property = (T)(object)Def;
                    return true;
                case 15:
                    property = (T)(object)MapOwner;
                    return true;
                case 16:
                    property = (T)(object)StaticIdFromExport;
                    return true;
                case 17:
                    property = (T)(object)Name;
                    return true;
                case 18:
                    property = (T)(object)Prefab;
                    return true;
                case 19:
                    property = (T)(object)SomeUnknownResourceThatMayBeUseful;
                    return true;
                case 20:
                    property = (T)(object)OnSceneObjectNetId;
                    return true;
                case 21:
                    property = (T)(object)AutoAddToWorldSpace;
                    return true;
                case 22:
                    property = (T)(object)WorldSpaced;
                    return true;
                case 23:
                    property = (T)(object)MovementSync;
                    return true;
                case 24:
                    property = (T)(object)OpenMechanics;
                    return true;
                case 25:
                    property = (T)(object)IsActive;
                    return true;
                case 26:
                    property = (T)(object)CraftEngine;
                    return true;
                case 27:
                    property = (T)(object)OwnerInformation;
                    return true;
                case 28:
                    property = (T)(object)CraftProgressInfo;
                    return true;
                case 29:
                    property = (T)(object)ContainerApi;
                    return true;
                case 30:
                    property = (T)(object)Stats;
                    return true;
                case 31:
                    property = (T)(object)Health;
                    return true;
                case 32:
                    property = (T)(object)Wizard;
                    return true;
                case 33:
                    property = (T)(object)SlaveWizardHolder;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Inventory))
                return 10;
            if (childName == nameof(FuelContainer))
                return 11;
            if (childName == nameof(OutputContainer))
                return 12;
            if (childName == nameof(PriorityQueue))
                return 13;
            if (childName == nameof(Def))
                return 14;
            if (childName == nameof(MapOwner))
                return 15;
            if (childName == nameof(StaticIdFromExport))
                return 16;
            if (childName == nameof(Name))
                return 17;
            if (childName == nameof(Prefab))
                return 18;
            if (childName == nameof(SomeUnknownResourceThatMayBeUseful))
                return 19;
            if (childName == nameof(OnSceneObjectNetId))
                return 20;
            if (childName == nameof(AutoAddToWorldSpace))
                return 21;
            if (childName == nameof(WorldSpaced))
                return 22;
            if (childName == nameof(MovementSync))
                return 23;
            if (childName == nameof(OpenMechanics))
                return 24;
            if (childName == nameof(IsActive))
                return 25;
            if (childName == nameof(CraftEngine))
                return 26;
            if (childName == nameof(OwnerInformation))
                return 27;
            if (childName == nameof(CraftProgressInfo))
                return 28;
            if (childName == nameof(ContainerApi))
                return 29;
            if (childName == nameof(Stats))
                return 30;
            if (childName == nameof(Health))
                return 31;
            if (childName == nameof(Wizard))
                return 32;
            if (childName == nameof(SlaveWizardHolder))
                return 33;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class CraftingPriorityQueueItem
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)CraftRecipe;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(CraftRecipe))
                return 10;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}