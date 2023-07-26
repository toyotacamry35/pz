using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.EntitySystem.Delta
{
    //[AutoProtoIncludeSubTypes]
    [ProtoContract(IgnoreListHandling = true)]
    public interface IDeltaDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDeltaObject//, IBaseDeltaObjectWrapper
    {
        /// <summary>
        /// Calls when new item adds or replaces the old one
        /// </summary>
        event DeltaDictionaryChangedDelegate<TKey, TValue> OnItemAddedOrUpdated;

        /// <summary>
        /// Calls when item removed or replaced with new one
        /// </summary>
        event DeltaDictionaryChangedDelegate<TKey, TValue> OnItemRemoved;

        /// <summary>
        /// Calls when any other event called - dont use
        /// </summary>
        event DeltaDictionaryChangedDelegate OnChanged;

        IDeltaDictionary<TKey, T1> ToDeltaDictionary<T1>() where T1 : IBaseDeltaObjectWrapper;
    }

    public interface IDeltaDictionaryWrapper<TKey, TValue1> : IDeltaDictionary<TKey, TValue1>, IBaseDeltaObjectWrapper
    {
    }

    public delegate Task DeltaDictionaryChangedDelegate<TKey, TValue>(DeltaDictionaryChangedEventArgs<TKey, TValue> args);

    public class DeltaDictionaryChangedEventArgs<TKey, TValue>
    {
        public DeltaDictionaryChangedEventArgs(TKey key, TValue oldValue, TValue value, IDeltaObject sender)
        {
            Key = key;
            OldValue = oldValue;
            Value = value;
            Sender = sender;
        }

        public TKey Key { get; }

        public TValue OldValue { get; }

        public TValue Value { get; }

        public IDeltaObject Sender { get; }

        public bool IsHandled { get; set; }
    }

    public delegate Task DeltaDictionaryChangedDelegate(DeltaDictionaryChangedEventArgs args);

    public class DeltaDictionaryChangedEventArgs
    {
        public DeltaDictionaryChangedEventArgs(IDeltaObject sender)
        {
            Sender = sender;
        }

        public IDeltaObject Sender { get; }

        public bool IsHandled { get; set; }
    }
}