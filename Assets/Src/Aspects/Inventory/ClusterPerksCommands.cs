using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Assets.Tools;
using Uins;
using Uins.Slots;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Assets.Src.Aspects
{
    public static class ClusterPerksCommands
    {
        public static void MovePerk(Guid playerGuid, PerkBaseSlotViewModel fromPerkBaseSvm, PerkBaseSlotViewModel toPerkBaseSvm)
        {
            if (fromPerkBaseSvm.AssertIfNull(nameof(fromPerkBaseSvm)) ||
                toPerkBaseSvm.AssertIfNull(nameof(toPerkBaseSvm)))
                return;

            UI.CallerLog(
                $"{nameof(playerGuid)}={playerGuid}, {nameof(fromPerkBaseSvm)}={fromPerkBaseSvm}, {nameof(toPerkBaseSvm)}={toPerkBaseSvm}");
            ClusterCommands.MoveItem_OnClient(playerGuid, fromPerkBaseSvm, toPerkBaseSvm, 1);
        }

        public static void SavePerk(Guid playerGuid, PerkBaseSlotViewModel fromPerkBaseSvm, PerkBaseSlotViewModel toPerkBaseSvm)
        {
            if (fromPerkBaseSvm.AssertIfNull(nameof(fromPerkBaseSvm)) ||
                toPerkBaseSvm.AssertIfNull(nameof(toPerkBaseSvm)))
                return;

            UI.CallerLog(
                $"{nameof(playerGuid)}={playerGuid}, {nameof(fromPerkBaseSvm)}={fromPerkBaseSvm}, {nameof(toPerkBaseSvm)}={toPerkBaseSvm}");
            ClusterCommands.SavePerk_OnClient(playerGuid, fromPerkBaseSvm, toPerkBaseSvm, 1);
        }

        public static void UnlockPerkSlot(Guid playerGuid, PerkBaseSlotViewModel perkBaseSvm, PerkSlotTypes perkSlotTypes)
        {
            if (perkBaseSvm.AssertIfNull(nameof(perkBaseSvm)) ||
                perkSlotTypes.AssertIfNull(nameof(perkSlotTypes)))
                return;

            UI.CallerLog($"{nameof(playerGuid)}={playerGuid}, {nameof(perkBaseSvm)}={perkBaseSvm}");
            AsyncUtils.RunAsyncTask(() => AddPerkSlotAsync(playerGuid, perkBaseSvm, false, perkSlotTypes));
        }

        public static void UpgradePerkSlot(Guid playerGuid, PerkBaseSlotViewModel perkBaseSvm, PerkSlotTypes perkSlotTypes)
        {
            if (perkBaseSvm.AssertIfNull(nameof(perkBaseSvm)))
                return;

            UI.CallerLog($"{nameof(playerGuid)}={playerGuid}, {nameof(perkBaseSvm)}={perkBaseSvm}");
            AsyncUtils.RunAsyncTask(() => AddPerkSlotAsync(playerGuid, perkBaseSvm, true, perkSlotTypes));
        }

        public static void DisassemblyPerk(Guid playerGuid, PerkBaseSlotViewModel perkBaseSvm)
        {
            if (perkBaseSvm.AssertIfNull(nameof(perkBaseSvm)))
                return;

            UI.CallerLog($"{nameof(playerGuid)}={playerGuid}, {nameof(perkBaseSvm)}={perkBaseSvm}");
            AsyncUtils.RunAsyncTask(() => ClusterCommands.DisassemblyPerkTask_OnClient(perkBaseSvm));
        }


        //=== Private =========================================================

        private static async Task AddPerkSlotAsync(Guid playerGuid, PerkBaseSlotViewModel fromSvm, bool isUpgrade,
            PerkSlotTypes perkSlotTypes)
        {
            if (perkSlotTypes.AssertIfNull(nameof(perkSlotTypes)))
                return;

            var newPerkSlotType = isUpgrade
                ? perkSlotTypes.GetNextItemType(fromSvm.PerkSlotType)
                : perkSlotTypes.FirstType;

            if (newPerkSlotType.AssertIfNull(nameof(newPerkSlotType)))
                return;

            using (var worldCharacterWrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(playerGuid))
            {
                if (worldCharacterWrapper.AssertIfNull(nameof(worldCharacterWrapper)))
                    return;

                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                await worldCharacter.AddPerkSlot(fromSvm.PerkSlotsCollectionApi.CollectionPropertyAddress, fromSvm.SlotId, newPerkSlotType);
            }
        }
    }
}