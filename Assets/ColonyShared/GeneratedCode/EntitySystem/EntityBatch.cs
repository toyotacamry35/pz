using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using SharedCode.Repositories;
using SharedCode.Utils.Extensions;

namespace SharedCode.EntitySystem
{
    public class EntityBatch: IEntityBatch, IEntityBatchExt
    {
        private static long LastId = 0;

        public long Id { get; }

        private BatchItem[] _items;
        public BatchItem[] Items => _items;
        public int Length { get; set; }

        public bool Empty => Length == 0;
        public EntityBatch()
        {
            Id = Interlocked.Increment(ref LastId);
            _items = new BatchItem[1];
        }

        public EntityBatch(int defaultCapacity)
        {
            Id = Interlocked.Increment(ref LastId);
            _items = new BatchItem[defaultCapacity];
        }

        private void checkCapacity()
        {
            if (_items.Length > Length)
                return;

            var newCapacity = _items.Length + Math.Min(_items.Length, 8);
            Array.Resize(ref _items, newCapacity);
        }

        IEntityBatch IEntityBatchExt.AddExclusive<T>(Guid entityId, string callerTag)
        {
            return ((IEntityBatchExt) this).AddExclusiveTag<T>(entityId, callerTag);
        }

        IEntityBatch IEntityBatchExt.AddExclusive(int typeId, Guid entityId, string callerTag)
        {
            return ((IEntityBatchExt)this).AddExclusiveTag(typeId, entityId, callerTag);
        }

        IEntityBatch IEntityBatchExt.AddExclusiveTag<T>(Guid entityId, string callerTag)
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            return ((IEntityBatchExt)this).AddExclusiveTag(typeId, entityId, callerTag);
        }

        IEntityBatch IEntityBatchExt.AddExclusiveTag(int typeId, Guid entityId, string callerTag)
        {
            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            for (int i = 0; i < Length; i++)
            {
                ref var item = ref _items[i];
                if (item.EntityMasterTypeId == masterTypeId && item.EntityId == entityId)
                {
                    item.RequestOperationType = ReadWriteEntityOperationType.Write;
                    return this;
                }
            }
            
            checkCapacity();
            _items[Length++] = new BatchItem(
                ReadWriteEntityOperationType.Write,
                entityId,
                masterTypeId,
                masterTypeId,
                callerTag);
            return this;
        }

        IEntityBatch IEntityBatchExt.AddTag<T>(Guid entityId, string callerTag)
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            return ((IEntityBatchExt)this).AddTag(typeId, entityId, callerTag);
        }

        IEntityBatch IEntityBatchExt.AddTag(int typeId, Guid entityId, string callerTag)
        {
            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            for (int i = 0; i < Length; i++)
            {
                ref var item = ref _items[i];
                if (item.EntityMasterTypeId == masterTypeId && item.EntityId == entityId)
                    return this;
            }

            checkCapacity();
            _items[Length++] = new BatchItem(
                ReadWriteEntityOperationType.Read,
                entityId,
                masterTypeId,
                typeId,
                callerTag);
            return this;
        }

        public IEntityBatch Add<T>(Guid entityId, string callerTag) where T : IEntity
        {
            return ((IEntityBatchExt)this).AddTag<T>(entityId, callerTag);
        }

        public IEntityBatch Add(int typeId, Guid entityId, string callerTag)
        {
            return ((IEntityBatchExt)this).AddTag(typeId, entityId, callerTag);
        }

        public bool HasItem(int typeId, Guid entityId)
        {
            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            for (int i = 0; i < Length; i++)
            {
                ref var item = ref _items[i];
                if (item.EntityMasterTypeId == masterTypeId && item.EntityId == entityId)
                    return true;
            }

            return false;
        }

        public bool HasItem<T>(Guid entityId) where T : IEntity
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            return HasItem(typeId, entityId);
        }

        public static IEntityBatch Create()
        {
            return new EntityBatch();
        }

        public static IEntityBatch Create(int defaultCapacity)
        {
            return new EntityBatch(defaultCapacity);
        }

        public IEntityBatch Add(OuterRef<IEntity> outerRef, ReplicationLevel replicationLevel, [CallerMemberName] string callerTag = "")
        {
            return Add(outerRef.RepTypeId(replicationLevel), outerRef.Guid, callerTag);
        }

        public void DumpToStringBuilder(StringBuilder sb)
        {
            sb.AppendFormat("<Batch {0} items", Id);
            sb.AppendLine();
            for (int i = 0; i < Length; i++)
            {
                ref var batchItem = ref _items[i];
                sb.AppendFormat("               <type:{0} id:{1} op:{2} up:{3} caller:{4}>",
                    ReplicaTypeRegistry.GetTypeById(batchItem.EntityMasterTypeId).Name, batchItem.EntityId, batchItem.RequestOperationType.ToString(), batchItem.UpFromReadToExclusive, batchItem.CallerTag);
                sb.AppendLine();
            }

            sb.AppendLine(">");
        }

        public override string ToString()
        {
            return GetType().Name + "(Id " + Id.ToString() + ")";
        }

        public bool HasWriteItem()
        {
            for (int i = 0; i < Length; i++)
            {
                ref var item = ref _items[i];
                if (item.RequestOperationType == ReadWriteEntityOperationType.Write)
                    return true;
            }

            return false;
        }

        public void RemoveAt(int i)
        {
            Array.Copy(_items, i + 1, _items, i, Length - i - 1);
            Length--;
        }
    }

    public static class ArrayExts
    {
        public interface MutatingPredicate<T>
        {
            bool NeedRemove(ref T item);
        }

        // See List.RemoveAll https://github.com/dotnet/coreclr/blob/a9f3fc16483eecfc47fb79c362811d870be02249/src/System.Private.CoreLib/shared/System/Collections/Generic/List.cs#L854
        public static int RemoveAll<T, Pred>(this T[] array, int start, int len, ref Pred match) where Pred : MutatingPredicate<T>
        {
            int freeIndex = start;   // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < start + len && !match.NeedRemove(ref array[freeIndex])) freeIndex++;
            if (freeIndex >= start + len) return freeIndex;

            int current = freeIndex + 1;
            while (current < start + len)
            {
                // Find the first item which needs to be kept.
                while (current < start + len && match.NeedRemove(ref array[current])) current++;

                if (current < start + len)
                {
                    // copy item to the free slot.
                    array[freeIndex++] = array[current++];
                }
            }

            return freeIndex;
        }
    }
}