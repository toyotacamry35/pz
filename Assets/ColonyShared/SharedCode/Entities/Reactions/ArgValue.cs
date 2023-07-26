using System;
using System.Linq;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using ResourceSystem.Reactions;
using SharedCode.Serializers.Protobuf;

namespace ColonyShared.SharedCode.Entities.Reactions
{
    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public abstract class ArgValue : IValue
    {
        public abstract Type ValueType { get; }
        
        public abstract Value Value { get; }
        
        public static ArgValue<T> Create<T>(T value)
        {
            return new ArgValue<T>(value);
        }

        public static ArgValue<T> Create<T>(Value value)
        {
            return new ArgValue<T>(ValueConverter<T>.Convert(value));
        }
        
        public static ArgValue Create(Value value)
        {
            return Converters[(int)value.ValueType].Invoke(value);
        }

        private static Func<Value, ArgValue> ConverterCreator<T>() => (v) => new ArgValue<T>(ValueConverter<T>.Convert(v));

        private static Func<Value, ArgValue>[] MakeConvertors()
        {
            var creatorMethod = typeof(ArgValue).GetMethod(nameof(ConverterCreator), BindingFlags.Static | BindingFlags.NonPublic);
            var arr = new Func<Value, ArgValue>[(int)Enum.GetValues(typeof(Value.Type)).Cast<Value.Type>().Max() + 1];
            foreach (var tuple in Value.SupportedTypes.Where(x => x.Item1 != Value.Type.None))
                arr[(int) tuple.Item1] = (Func<Value, ArgValue>) creatorMethod.MakeGenericMethod(tuple.Item2).Invoke(null, new object[]{});
            return arr;
        }

        private static readonly Func<Value, ArgValue>[] Converters = MakeConvertors();
    }

    [ProtoContract]
    public class ArgValue<T> : ArgValue, IValue<T>
    {
        [ProtoMember(1)] public T _value;

        public ArgValue(T value)
        {
            if (Value.SupportedTypes.All(x => x.Item2 != typeof(T)))
                throw new Exception($"Unsupported type {typeof(T)}");
            _value = value;
        }

        public override Type ValueType => typeof(T);

        T IValue<T>.Value => _value;

        public override Value Value => ValueConverter<T>.Convert(_value);

        public override string ToString() => $"{_value}";
    }    
}