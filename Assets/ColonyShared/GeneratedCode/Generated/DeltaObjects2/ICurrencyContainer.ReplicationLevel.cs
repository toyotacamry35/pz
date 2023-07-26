// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class CurrencyContainerAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerAlways
    {
        public CurrencyContainerAlways(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> TransactionReservedSlots => __deltaObject__.TransactionReservedSlots;
        public override int TypeId => 987417111;
    }

    public class CurrencyContainerClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerClientBroadcast
    {
        public CurrencyContainerClientBroadcast(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientBroadcast> __Items__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientBroadcast> Items
        {
            get
            {
                if (__Items__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Items__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Items)
                    __Items__Wrapper__ = __deltaObject__.Items == null ? null : new DeltaDictionaryWrapper<int, SharedCode.DeltaObjects.ISlotItem, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientBroadcast>(__deltaObject__.Items);
                return __Items__Wrapper__;
            }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> TransactionReservedSlots => __deltaObject__.TransactionReservedSlots;
        public int Size => __deltaObject__.Size;
        public System.Threading.Tasks.Task<float> GetMaxWeigth()
        {
            return __deltaObject__.GetMaxWeigth();
        }

        public System.Threading.Tasks.Task<float> GetTotalWeight()
        {
            return __deltaObject__.GetTotalWeight();
        }

        public System.Threading.Tasks.Task<int> GetMaxStackForSlot(int destinationSlot)
        {
            return __deltaObject__.GetMaxStackForSlot(destinationSlot);
        }

        public System.Threading.Tasks.Task<bool> IgnoreMaxStack()
        {
            return __deltaObject__.IgnoreMaxStack();
        }

        public System.Threading.Tasks.Task<bool> CanAutoselectEmptySlotsForAddStacks()
        {
            return __deltaObject__.CanAutoselectEmptySlotsForAddStacks();
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Items;
                    break;
                case 11:
                    currProperty = Size;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 294235708;
    }

    public class CurrencyContainerClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerClientFullApi
    {
        public CurrencyContainerClientFullApi(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        public override int TypeId => 426107258;
    }

    public class CurrencyContainerClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerClientFull
    {
        public CurrencyContainerClientFull(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientFull> __Items__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientFull> Items
        {
            get
            {
                if (__Items__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Items__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Items)
                    __Items__Wrapper__ = __deltaObject__.Items == null ? null : new DeltaDictionaryWrapper<int, SharedCode.DeltaObjects.ISlotItem, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemClientFull>(__deltaObject__.Items);
                return __Items__Wrapper__;
            }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> TransactionReservedSlots => __deltaObject__.TransactionReservedSlots;
        public int Size => __deltaObject__.Size;
        public System.Threading.Tasks.Task<float> GetMaxWeigth()
        {
            return __deltaObject__.GetMaxWeigth();
        }

        public System.Threading.Tasks.Task<float> GetTotalWeight()
        {
            return __deltaObject__.GetTotalWeight();
        }

        public System.Threading.Tasks.Task<int> GetMaxStackForSlot(int destinationSlot)
        {
            return __deltaObject__.GetMaxStackForSlot(destinationSlot);
        }

        public System.Threading.Tasks.Task<bool> IgnoreMaxStack()
        {
            return __deltaObject__.IgnoreMaxStack();
        }

        public System.Threading.Tasks.Task<bool> CanAutoselectEmptySlotsForAddStacks()
        {
            return __deltaObject__.CanAutoselectEmptySlotsForAddStacks();
        }

        public event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, int, bool, System.Threading.Tasks.Task> ItemAddedToContainer
        {
            add
            {
                __deltaObject__.ItemAddedToContainer += value;
            }

            remove
            {
                __deltaObject__.ItemAddedToContainer -= value;
            }
        }

        public event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, int, bool, System.Threading.Tasks.Task> ItemRemovedToContainer
        {
            add
            {
                __deltaObject__.ItemRemovedToContainer += value;
            }

            remove
            {
                __deltaObject__.ItemRemovedToContainer -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Items;
                    break;
                case 11:
                    currProperty = Size;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1143649639;
    }

    public class CurrencyContainerServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerServerApi
    {
        public CurrencyContainerServerApi(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        public override int TypeId => -761361413;
    }

    public class CurrencyContainerServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICurrencyContainerServer
    {
        public CurrencyContainerServer(SharedCode.DeltaObjects.ICurrencyContainer deltaObject): base(deltaObject)
        {
        }

        SharedCode.DeltaObjects.ICurrencyContainer __deltaObject__
        {
            get
            {
                return (SharedCode.DeltaObjects.ICurrencyContainer)__deltaObjectBase__;
            }
        }

        IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemServer> __Items__Wrapper__;
        public IDeltaDictionaryWrapper<int, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemServer> Items
        {
            get
            {
                if (__Items__Wrapper__ == null || ((IBaseDeltaObjectWrapper)__Items__Wrapper__).GetBaseDeltaObject() != __deltaObject__.Items)
                    __Items__Wrapper__ = __deltaObject__.Items == null ? null : new DeltaDictionaryWrapper<int, SharedCode.DeltaObjects.ISlotItem, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISlotItemServer>(__deltaObject__.Items);
                return __Items__Wrapper__;
            }
        }

        public System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> TransactionReservedSlots => __deltaObject__.TransactionReservedSlots;
        public int Size => __deltaObject__.Size;
        public System.Threading.Tasks.Task<float> GetMaxWeigth()
        {
            return __deltaObject__.GetMaxWeigth();
        }

        public System.Threading.Tasks.Task<float> GetTotalWeight()
        {
            return __deltaObject__.GetTotalWeight();
        }

        public System.Threading.Tasks.Task<int> GetMaxStackForSlot(int destinationSlot)
        {
            return __deltaObject__.GetMaxStackForSlot(destinationSlot);
        }

        public System.Threading.Tasks.Task<bool> IgnoreMaxStack()
        {
            return __deltaObject__.IgnoreMaxStack();
        }

        public System.Threading.Tasks.Task<bool> CanAutoselectEmptySlotsForAddStacks()
        {
            return __deltaObject__.CanAutoselectEmptySlotsForAddStacks();
        }

        public System.Threading.Tasks.Task<bool> CanAdd(GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemServer item, int index, int count, bool manual)
        {
            return __deltaObject__.CanAdd(item.To<SharedCode.Entities.IItem>(), index, count, manual);
        }

        public System.Threading.Tasks.Task<bool> CanRemove(GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemServer item, int index, int count, bool manual)
        {
            return __deltaObject__.CanRemove(item.To<SharedCode.Entities.IItem>(), index, count, manual);
        }

        public System.Threading.Tasks.Task OnItemAdded(GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemServer item, int index, int count, bool manual)
        {
            return __deltaObject__.OnItemAdded(item.To<SharedCode.Entities.IItem>(), index, count, manual);
        }

        public System.Threading.Tasks.Task<SharedCode.EntitySystem.PropertyAddress> OnBeforeItemRemoved(GeneratedCode.DeltaObjects.ReplicationInterfaces.IItemServer item, int index, int count, bool manual)
        {
            return __deltaObject__.OnBeforeItemRemoved(item.To<SharedCode.Entities.IItem>(), index, count, manual);
        }

        public event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, int, bool, System.Threading.Tasks.Task> ItemAddedToContainer
        {
            add
            {
                __deltaObject__.ItemAddedToContainer += value;
            }

            remove
            {
                __deltaObject__.ItemAddedToContainer -= value;
            }
        }

        public event System.Func<SharedCode.Aspects.Item.Templates.BaseItemResource, int, int, bool, System.Threading.Tasks.Task> ItemRemovedToContainer
        {
            add
            {
                __deltaObject__.ItemRemovedToContainer += value;
            }

            remove
            {
                __deltaObject__.ItemRemovedToContainer -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Items;
                    break;
                case 11:
                    currProperty = Size;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 660657984;
    }
}