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
    public partial class WorldCharacter
    {
        public override bool TryGetProperty<T>(int address, out T property)
        {
            switch (address)
            {
                case 10:
                    property = (T)(object)BuildingEngine;
                    return true;
                case 11:
                    property = (T)(object)KnowledgeEngine;
                    return true;
                case 12:
                    property = (T)(object)LastActivatedWasCommonBaken;
                    return true;
                case 13:
                    property = (T)(object)ActivatedCommonBakens;
                    return true;
                case 14:
                    property = (T)(object)PointMarkers;
                    return true;
                case 15:
                    property = (T)(object)PointsOfInterest;
                    return true;
                case 16:
                    property = (T)(object)TimeWhenUserDisconnected;
                    return true;
                case 17:
                    property = (T)(object)IsIdle;
                    return true;
                case 18:
                    property = (T)(object)IsAFK;
                    return true;
                case 19:
                    property = (T)(object)SessionId;
                    return true;
                case 20:
                    property = (T)(object)AccountId;
                    return true;
                case 21:
                    property = (T)(object)Def;
                    return true;
                case 22:
                    property = (T)(object)MapOwner;
                    return true;
                case 23:
                    property = (T)(object)StaticIdFromExport;
                    return true;
                case 24:
                    property = (T)(object)Name;
                    return true;
                case 25:
                    property = (T)(object)Prefab;
                    return true;
                case 26:
                    property = (T)(object)SomeUnknownResourceThatMayBeUseful;
                    return true;
                case 27:
                    property = (T)(object)OnSceneObjectNetId;
                    return true;
                case 28:
                    property = (T)(object)AutoAddToWorldSpace;
                    return true;
                case 29:
                    property = (T)(object)WorldSpaced;
                    return true;
                case 30:
                    property = (T)(object)Inventory;
                    return true;
                case 31:
                    property = (T)(object)Currency;
                    return true;
                case 32:
                    property = (T)(object)Dialog;
                    return true;
                case 33:
                    property = (T)(object)Doll;
                    return true;
                case 34:
                    property = (T)(object)TemporaryPerks;
                    return true;
                case 35:
                    property = (T)(object)PermanentPerks;
                    return true;
                case 36:
                    property = (T)(object)SavedPerks;
                    return true;
                case 37:
                    property = (T)(object)Statistics;
                    return true;
                case 38:
                    property = (T)(object)PerksDestroyCount;
                    return true;
                case 39:
                    property = (T)(object)PerkActionsPrices;
                    return true;
                case 40:
                    property = (T)(object)StatisticEngine;
                    return true;
                case 41:
                    property = (T)(object)CraftEngine;
                    return true;
                case 42:
                    property = (T)(object)Wizard;
                    return true;
                case 43:
                    property = (T)(object)SlaveWizardHolder;
                    return true;
                case 44:
                    property = (T)(object)SimpleStupidStats;
                    return true;
                case 45:
                    property = (T)(object)ContainerApi;
                    return true;
                case 46:
                    property = (T)(object)AllowedSpawnPoint;
                    return true;
                case 47:
                    property = (T)(object)Health;
                    return true;
                case 48:
                    property = (T)(object)Mortal;
                    return true;
                case 49:
                    property = (T)(object)Brute;
                    return true;
                case 50:
                    property = (T)(object)Stats;
                    return true;
                case 51:
                    property = (T)(object)MutationMechanics;
                    return true;
                case 52:
                    property = (T)(object)Faction;
                    return true;
                case 53:
                    property = (T)(object)Gender;
                    return true;
                case 54:
                    property = (T)(object)Traumas;
                    return true;
                case 55:
                    property = (T)(object)PingDiagnostics;
                    return true;
                case 56:
                    property = (T)(object)ItemsStatsAccumulator;
                    return true;
                case 57:
                    property = (T)(object)AuthorityOwner;
                    return true;
                case 58:
                    property = (T)(object)MovementSync;
                    return true;
                case 59:
                    property = (T)(object)LogableEntity;
                    return true;
                case 60:
                    property = (T)(object)LocomotionOwner;
                    return true;
                case 61:
                    property = (T)(object)Consumer;
                    return true;
                case 62:
                    property = (T)(object)Quest;
                    return true;
                case 63:
                    property = (T)(object)InputActionHandlers;
                    return true;
                case 64:
                    property = (T)(object)ReactionsOwner;
                    return true;
                case 65:
                    property = (T)(object)AttackEngine;
                    return true;
                case 66:
                    property = (T)(object)AnimationDoerOwner;
                    return true;
                case 67:
                    property = (T)(object)FounderPack;
                    return true;
                case 68:
                    property = (T)(object)SpatialDatahandlers;
                    return true;
                case 69:
                    property = (T)(object)Buffs;
                    return true;
                case 70:
                    property = (T)(object)LinksEngine;
                    return true;
                case 71:
                    property = (T)(object)FogOfWar;
                    return true;
                case 72:
                    property = (T)(object)WorldObjectInformationSetsEngine;
                    return true;
                case 73:
                    property = (T)(object)worldPersonalMachineEngine;
                    return true;
                case 74:
                    property = (T)(object)AccountStats;
                    return true;
                case 75:
                    property = (T)(object)SpellModifiers;
                    return true;
                case 76:
                    property = (T)(object)SpellModifiersCollector;
                    return true;
                case 77:
                    property = (T)(object)KillingRewardMechanics;
                    return true;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    property = default;
                    return false;
            }
        }

        public override int GetIdOfChildNonDeltaObj(string childName)
        {
            if (childName == nameof(BuildingEngine))
                return 10;
            if (childName == nameof(KnowledgeEngine))
                return 11;
            if (childName == nameof(LastActivatedWasCommonBaken))
                return 12;
            if (childName == nameof(ActivatedCommonBakens))
                return 13;
            if (childName == nameof(PointMarkers))
                return 14;
            if (childName == nameof(PointsOfInterest))
                return 15;
            if (childName == nameof(TimeWhenUserDisconnected))
                return 16;
            if (childName == nameof(IsIdle))
                return 17;
            if (childName == nameof(IsAFK))
                return 18;
            if (childName == nameof(SessionId))
                return 19;
            if (childName == nameof(AccountId))
                return 20;
            if (childName == nameof(Def))
                return 21;
            if (childName == nameof(MapOwner))
                return 22;
            if (childName == nameof(StaticIdFromExport))
                return 23;
            if (childName == nameof(Name))
                return 24;
            if (childName == nameof(Prefab))
                return 25;
            if (childName == nameof(SomeUnknownResourceThatMayBeUseful))
                return 26;
            if (childName == nameof(OnSceneObjectNetId))
                return 27;
            if (childName == nameof(AutoAddToWorldSpace))
                return 28;
            if (childName == nameof(WorldSpaced))
                return 29;
            if (childName == nameof(Inventory))
                return 30;
            if (childName == nameof(Currency))
                return 31;
            if (childName == nameof(Dialog))
                return 32;
            if (childName == nameof(Doll))
                return 33;
            if (childName == nameof(TemporaryPerks))
                return 34;
            if (childName == nameof(PermanentPerks))
                return 35;
            if (childName == nameof(SavedPerks))
                return 36;
            if (childName == nameof(Statistics))
                return 37;
            if (childName == nameof(PerksDestroyCount))
                return 38;
            if (childName == nameof(PerkActionsPrices))
                return 39;
            if (childName == nameof(StatisticEngine))
                return 40;
            if (childName == nameof(CraftEngine))
                return 41;
            if (childName == nameof(Wizard))
                return 42;
            if (childName == nameof(SlaveWizardHolder))
                return 43;
            if (childName == nameof(SimpleStupidStats))
                return 44;
            if (childName == nameof(ContainerApi))
                return 45;
            if (childName == nameof(AllowedSpawnPoint))
                return 46;
            if (childName == nameof(Health))
                return 47;
            if (childName == nameof(Mortal))
                return 48;
            if (childName == nameof(Brute))
                return 49;
            if (childName == nameof(Stats))
                return 50;
            if (childName == nameof(MutationMechanics))
                return 51;
            if (childName == nameof(Faction))
                return 52;
            if (childName == nameof(Gender))
                return 53;
            if (childName == nameof(Traumas))
                return 54;
            if (childName == nameof(PingDiagnostics))
                return 55;
            if (childName == nameof(ItemsStatsAccumulator))
                return 56;
            if (childName == nameof(AuthorityOwner))
                return 57;
            if (childName == nameof(MovementSync))
                return 58;
            if (childName == nameof(LogableEntity))
                return 59;
            if (childName == nameof(LocomotionOwner))
                return 60;
            if (childName == nameof(Consumer))
                return 61;
            if (childName == nameof(Quest))
                return 62;
            if (childName == nameof(InputActionHandlers))
                return 63;
            if (childName == nameof(ReactionsOwner))
                return 64;
            if (childName == nameof(AttackEngine))
                return 65;
            if (childName == nameof(AnimationDoerOwner))
                return 66;
            if (childName == nameof(FounderPack))
                return 67;
            if (childName == nameof(SpatialDatahandlers))
                return 68;
            if (childName == nameof(Buffs))
                return 69;
            if (childName == nameof(LinksEngine))
                return 70;
            if (childName == nameof(FogOfWar))
                return 71;
            if (childName == nameof(WorldObjectInformationSetsEngine))
                return 72;
            if (childName == nameof(worldPersonalMachineEngine))
                return 73;
            if (childName == nameof(AccountStats))
                return 74;
            if (childName == nameof(SpellModifiers))
                return 75;
            if (childName == nameof(SpellModifiersCollector))
                return 76;
            if (childName == nameof(KillingRewardMechanics))
                return 77;
            throw new System.InvalidOperationException($"Field {childName} is not a child of {this}");
        }
    }
}