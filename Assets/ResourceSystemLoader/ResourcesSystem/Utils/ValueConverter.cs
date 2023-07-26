using System;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace ColonyShared.SharedCode
{
    public class ValueConverter<ReturnType>
    {
        protected static IValueConverter<ReturnType> _converter = new ValueConverterNotImplemented<ReturnType>();

        public static ReturnType Convert(Value value) => _converter.FromValue(value);
        public static Value Convert(ReturnType value) => _converter.ToValue(value);

        
    }

    public interface IValueConverter<ReturnType>
    {
        ReturnType FromValue(Value value);
        Value ToValue(ReturnType value);
    }

    public static class ValueConverterInitialized
    {
        public static object Lock = new object();
        public static bool Initialized; 
        public static void InitValueConverters()
        {
            if (!ValueConverterInitialized.Initialized)
            {
                ValueConverterInitialized.Initialized = true;
                ValueConverterFloat.Init();
                ValueConverterVector2.Init();
                ValueConverterVector3.Init();
                ValueConverterQuaternion.Init();
                ValueConverterInt.Init();
                ValueConverterLong.Init();
                ValueConverterULong.Init();
                ValueConverterBool.Init();
                ValueConverterString.Init();
                ValueConverterColor.Init();
                ValueConverterDateTime.Init();
                ValueConverterOuterRef.Init();
                ValueConverterResource.Init();
                ValueConverterValue.Init();
            }

        }
    }

    internal class ValueConverterValue : ValueConverter<Value>, IValueConverter<Value>
    {
        public static void Init() => _converter = new ValueConverterValue();
        public Value FromValue(Value value) => value;
        public Value ToValue(Value value) => value;
    }

    internal class ValueConverterBool : ValueConverter<bool>, IValueConverter<bool>
    {
        public static void Init() => _converter = new ValueConverterBool();
        public bool FromValue(Value value) => value.Bool;
        public Value ToValue(bool value) => new Value(value);
    }

    internal class ValueConverterFloat : ValueConverter<float>, IValueConverter<float>
    {
        public static void Init() => _converter = new ValueConverterFloat();
        public float FromValue(Value value) => value.Float;
        public Value ToValue(float value) => new Value(value);
    }

    internal class ValueConverterInt : ValueConverter<int>, IValueConverter<int>
    {
        public static void Init() => _converter = new ValueConverterInt();
        public int FromValue(Value value) => value.Int;
        public Value ToValue(int value) => new Value(value);
    }

    internal class ValueConverterLong : ValueConverter<long>, IValueConverter<long>
    {
        public static void Init() => _converter = new ValueConverterLong();
        public long FromValue(Value value) => value.Long;
        public Value ToValue(long value) => new Value(value);
    }

    internal class ValueConverterULong : ValueConverter<ulong>, IValueConverter<ulong>
    {
        public static void Init() => _converter = new ValueConverterULong();
        public ulong FromValue(Value value) => value.ULong;
        public Value ToValue(ulong value) => new Value(value);
    }

    internal class ValueConverterString : ValueConverter<string>, IValueConverter<string>
    {
        public static void Init() => _converter = new ValueConverterString();
        public string FromValue(Value value) => value.String;
        public Value ToValue(string value) => new Value(value);
    }

    internal class ValueConverterVector2 : ValueConverter<Vector2>, IValueConverter<Vector2>
    {
        public static void Init() => _converter = new ValueConverterVector2();
        public Vector2 FromValue(Value value) => value.Vector2;
        public Value ToValue(Vector2 value) => new Value(value);
    }

    internal class ValueConverterVector3 : ValueConverter<Vector3>, IValueConverter<Vector3>
    {
        public static void Init() => _converter = new ValueConverterVector3();
        public Vector3 FromValue(Value value) => value.Vector3;
        public Value ToValue(Vector3 value) => new Value(value);
    }

    internal class ValueConverterQuaternion : ValueConverter<Quaternion>, IValueConverter<Quaternion>
    {
        public static void Init() => _converter = new ValueConverterQuaternion();
        public Quaternion FromValue(Value value) => value.Quaternion;
        public Value ToValue(Quaternion value) => new Value(value);
    }

    internal class ValueConverterColor : ValueConverter<Color>, IValueConverter<Color>
    {
        public static void Init() => _converter = new ValueConverterColor();
        public Color FromValue(Value value) => value.Color;
        public Value ToValue(Color value) => new Value(value);
    }

    internal class ValueConverterDateTime : ValueConverter<DateTime>, IValueConverter<DateTime>
    {
        public static void Init() => _converter = new ValueConverterDateTime();
        public DateTime FromValue(Value value) => value.DateTime;
        public Value ToValue(DateTime value) => new Value(value);
    }

    internal class ValueConverterOuterRef : ValueConverter<OuterRef>, IValueConverter<OuterRef>
    {
        public static void Init() => _converter = new ValueConverterOuterRef();
        public OuterRef FromValue(Value value) => value.OuterRef;
        public Value ToValue(OuterRef value) => new Value(value);
    }

    internal class ValueConverterResource : ValueConverter<BaseResource>, IValueConverter<BaseResource>
    {
        public static void Init() => _converter = new ValueConverterResource();
        public BaseResource FromValue(Value value) => value.Resource;
        public Value ToValue(BaseResource value) => new Value(value);
    }

    internal class ValueConverterNotImplemented<ReturnType> : ValueConverter<ReturnType>, IValueConverter<ReturnType>
    {
        public ReturnType FromValue(Value value) => throw new NotImplementedException($"Value converter for {typeof(ReturnType).Name} is not implemented");
        public Value ToValue(ReturnType value) => throw new NotImplementedException($"Value converter for {typeof(ReturnType).Name} is not implemented");
    }
}
