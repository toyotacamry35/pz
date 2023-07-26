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
    public partial class LegionaryEntity
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)Wizard;
                    return true;
                case 11:
                    property = (T)(object)SlaveWizardHolder;
                    return true;
                case 12:
                    property = (T)(object)Health;
                    return true;
                case 13:
                    property = (T)(object)Mortal;
                    return true;
                case 14:
                    property = (T)(object)CorpseSpawner;
                    return true;
                case 15:
                    property = (T)(object)Brute;
                    return true;
                case 16:
                    property = (T)(object)Def;
                    return true;
                case 17:
                    property = (T)(object)MapOwner;
                    return true;
                case 18:
                    property = (T)(object)StaticIdFromExport;
                    return true;
                case 19:
                    property = (T)(object)Name;
                    return true;
                case 20:
                    property = (T)(object)Prefab;
                    return true;
                case 21:
                    property = (T)(object)SomeUnknownResourceThatMayBeUseful;
                    return true;
                case 22:
                    property = (T)(object)OnSceneObjectNetId;
                    return true;
                case 23:
                    property = (T)(object)AutoAddToWorldSpace;
                    return true;
                case 24:
                    property = (T)(object)WorldSpaced;
                    return true;
                case 25:
                    property = (T)(object)Stats;
                    return true;
                case 26:
                    property = (T)(object)MovementSync;
                    return true;
                case 27:
                    property = (T)(object)LogableEntity;
                    return true;
                case 28:
                    property = (T)(object)SpawnedObject;
                    return true;
                case 29:
                    property = (T)(object)Destroyable;
                    return true;
                case 30:
                    property = (T)(object)ReactionsOwner;
                    return true;
                case 31:
                    property = (T)(object)AnimationDoerOwner;
                    return true;
                case 32:
                    property = (T)(object)Doll;
                    return true;
                case 33:
                    property = (T)(object)ContainerApi;
                    return true;
                case 34:
                    property = (T)(object)Bank;
                    return true;
                case 35:
                    property = (T)(object)Buffs;
                    return true;
                case 36:
                    property = (T)(object)InputActionHandlers;
                    return true;
                case 37:
                    property = (T)(object)AttackEngine;
                    return true;
                case 38:
                    property = (T)(object)LocomotionOwner;
                    return true;
                case 39:
                    property = (T)(object)AiTargetRecipient;
                    return true;
                case 40:
                    property = (T)(object)Faction;
                    return true;
                case 41:
                    property = (T)(object)SquadId;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(Wizard))
                return 10;
            if (childName == nameof(SlaveWizardHolder))
                return 11;
            if (childName == nameof(Health))
                return 12;
            if (childName == nameof(Mortal))
                return 13;
            if (childName == nameof(CorpseSpawner))
                return 14;
            if (childName == nameof(Brute))
                return 15;
            if (childName == nameof(Def))
                return 16;
            if (childName == nameof(MapOwner))
                return 17;
            if (childName == nameof(StaticIdFromExport))
                return 18;
            if (childName == nameof(Name))
                return 19;
            if (childName == nameof(Prefab))
                return 20;
            if (childName == nameof(SomeUnknownResourceThatMayBeUseful))
                return 21;
            if (childName == nameof(OnSceneObjectNetId))
                return 22;
            if (childName == nameof(AutoAddToWorldSpace))
                return 23;
            if (childName == nameof(WorldSpaced))
                return 24;
            if (childName == nameof(Stats))
                return 25;
            if (childName == nameof(MovementSync))
                return 26;
            if (childName == nameof(LogableEntity))
                return 27;
            if (childName == nameof(SpawnedObject))
                return 28;
            if (childName == nameof(Destroyable))
                return 29;
            if (childName == nameof(ReactionsOwner))
                return 30;
            if (childName == nameof(AnimationDoerOwner))
                return 31;
            if (childName == nameof(Doll))
                return 32;
            if (childName == nameof(ContainerApi))
                return 33;
            if (childName == nameof(Bank))
                return 34;
            if (childName == nameof(Buffs))
                return 35;
            if (childName == nameof(InputActionHandlers))
                return 36;
            if (childName == nameof(AttackEngine))
                return 37;
            if (childName == nameof(LocomotionOwner))
                return 38;
            if (childName == nameof(AiTargetRecipient))
                return 39;
            if (childName == nameof(Faction))
                return 40;
            if (childName == nameof(SquadId))
                return 41;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}