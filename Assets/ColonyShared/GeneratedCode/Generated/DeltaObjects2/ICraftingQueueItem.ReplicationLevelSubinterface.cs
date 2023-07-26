// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 2085513184, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1821635195, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -851611732, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -1778119230, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Index
        {
            get;
        }

        Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef CraftRecipe
        {
            get;
        }

        System.Collections.Generic.List<int> MandatorySlotPermutation
        {
            get;
        }

        System.Collections.Generic.List<int> OptionalSlotPermutation
        {
            get;
        }

        int SelectedVariantIndex
        {
            get;
        }

        long TimeAlreadyCrafted
        {
            get;
        }

        long CraftStartTime
        {
            get;
        }

        bool IsActive
        {
            get;
        }

        int Count
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1358409902, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1301872377, typeof(SharedCode.Entities.Engine.ICraftingQueueItem))]
    public interface ICraftingQueueItemServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        int Index
        {
            get;
        }

        Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef CraftRecipe
        {
            get;
        }

        System.Collections.Generic.List<int> MandatorySlotPermutation
        {
            get;
        }

        System.Collections.Generic.List<int> OptionalSlotPermutation
        {
            get;
        }

        int SelectedVariantIndex
        {
            get;
        }

        long TimeAlreadyCrafted
        {
            get;
        }

        long CraftStartTime
        {
            get;
        }

        bool IsActive
        {
            get;
        }

        int Count
        {
            get;
        }
    }
}