using Assets.ColonyShared.SharedCode.Aspects.Craft;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using EnumerableExtensions;

namespace SharedCode.Entities.Engine
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ICraftingQueueItem : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        int Index { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        CraftRecipeDef CraftRecipe { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        List<Int32> MandatorySlotPermutation { get; set; }  // Note: мы исходим из предположения, что однажды записанное здесь значение меняться не будет.. совсем, иначе Delta не сработает
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        List<Int32> OptionalSlotPermutation { get; set; }   // Note: мы исходим из предположения, что однажды записанное здесь значение меняться не будет.. совсем, иначе Delta не сработает
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        int SelectedVariantIndex { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        long TimeAlreadyCrafted { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        long CraftStartTime { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        bool IsActive { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        int Count { get; set; }
    }

    [Flags]
    public enum CraftOperationResult
    {
        None = 0x00,
        SuccessCraft = 0x01,
        SuccessAddedToQueue = 0x02,
        ErrorItemIsNotRepairable = 0x04,
        ErrorNotEnoughItemsForCraft = 0x08,
        ErrorCraftQueueIsFull = 0x10,
        ErrorNotEnoughFuel = 0x20,
        ErrorCraftQueueIsEmpty = 0x40,
        ErrorUnknown = 0x80,
        ErrorCraftResultConainerIsFull = 0x100,

        Success = SuccessCraft | SuccessAddedToQueue,
        Error = ErrorItemIsNotRepairable| ErrorNotEnoughItemsForCraft | ErrorCraftQueueIsFull | ErrorNotEnoughFuel | ErrorCraftQueueIsEmpty | ErrorUnknown | ErrorCraftResultConainerIsFull
    }

    public static class CraftOperationResultExtensions
    {
        public static bool Is(this CraftOperationResult check, CraftOperationResult check2)
        {
            return (check & check2) != 0;
        }
    }

    public struct CraftingItems
    {
        public BaseItemResource Item;
        public int Required;
        public int Count;
        public IEnumerable<CraftingSlots> Slots;

        public override string ToString()
        {
            return $"({Count}x{Item} req={Required} Slots: {Slots.ItemsToString()})";
        }
    }

    public struct CraftingSlots
    {
        public int Slot;
        public int Count;
        public PropertyAddress Address;

        public override string ToString()
        {
            return $"({nameof(CraftingSlots)}: [{Slot}] Count={Count} PA={Address} /cs)";
        }
    }   
}