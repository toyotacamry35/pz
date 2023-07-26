using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.EntitySystem.Delta
{
    //[AutoProtoIncludeSubTypes]
    [ProtoContract(IgnoreListHandling = true)]
    public interface IDeltaList<T> : IList<T>, IReadOnlyList<T>, IDeltaObject//, IBaseDeltaObjectWrapper
    {
        event DeltaListChangedDelegate<T> OnItemAdded;

        event DeltaListChangedDelegate<T> OnItemRemoved;

        event DeltaListChangedDelegate OnChanged;

        IDeltaList<T1> ToDeltaList<T1>() where T1 : IBaseDeltaObjectWrapper;

        new int Count { get; }
        
        new T this[int index] { get; set; }
    }

    public interface IDeltaListWrapper<T> : IDeltaList<T>, IBaseDeltaObjectWrapper
    {
    }


    public delegate Task DeltaListChangedDelegate<T>(DeltaListChangedEventArgs<T> args);

    public class DeltaListChangedEventArgs<T>
    {
        public DeltaListChangedEventArgs(T value, int index, IDeltaObject sender)
        {
            Value = value;
            Index = index;
            Sender = sender;
        }

        public T Value { get; }

        public int Index { get; }

        public IDeltaObject Sender { get; }

        public bool IsHandled { get; set; }
    }

    public delegate Task DeltaListChangedDelegate(DeltaListChangedEventArgs args);

    public class DeltaListChangedEventArgs
    {
        public DeltaListChangedEventArgs(IDeltaObject sender)
        {
            Sender = sender;
        }

        public IDeltaObject Sender { get; }

        public bool IsHandled { get; set; }
    }
}
