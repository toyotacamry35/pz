using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace ColonyShared.SharedCode
{
    public readonly struct Value
    {
        public enum Type : byte { None, Float, Vector2, Vector3, Quaternion, Int, Long, ULong, Bool, String, Color, DateTime, OuterRef, Resource, Guid }

        public static readonly (Type, System.Type)[] SupportedTypes = { (Type.None, typeof(void)), (Type.Float, typeof(float)), (Type.Vector2, typeof(Vector2)), (Type.Vector3, typeof(Vector3)), (Type.Quaternion, typeof(Quaternion)), (Type.Int, typeof(int)), (Type.Long, typeof(long)), (Type.ULong, typeof(ulong)), (Type.Bool, typeof(bool)), (Type.String, typeof(string)), (Type.Color, typeof(Color)), (Type.DateTime, typeof(DateTime)), (Type.OuterRef, typeof(OuterRef)), (Type.Resource, typeof(BaseResource)), (Type.Guid, typeof(Guid)) };
        //public static readonly (Type, string, System.Type)[] SupportedTypes = { (Type.None, "", typeof(void)), (Type.Float, "float", typeof(float)), (Type.Vector2, "Vector2", typeof(Vector2)), (Type.Vector3, "Vector3", typeof(Vector3)), (Type.Quaternion, "Quaternion", typeof(Quaternion)), (Type.Int, "int", typeof(int)), (Type.Long, "long", typeof(long)), (Type.ULong, "ulong", typeof(ulong)), (Type.Bool, "bool", typeof(bool)), (Type.String, "string", typeof(string)), (Type.Color, typeof(Color)), (Type.DateTime, typeof(DateTime)), (Type.OuterRef, typeof(OuterRef)), (Type.Resource, typeof(BaseResource)), (Type.Guid, typeof(Guid)) };

        public Type ValueType => _type;

        public System.Type SystemType => SupportedTypes[(int)_type].Item2; // o.O

        public bool IsNone => ValueType == Type.None;

        public Value(float value) : this()
        { _type = Type.Float; _union = new Union(value); }

        public Value(Vector2 value) : this()
        { _type = Type.Vector2; _union = new Union(value); }

        public Value(Vector3 value) : this()
        { _type = Type.Vector3; _union = new Union(value); }

        public Value(Quaternion value) : this()
        { _type = Type.Quaternion; _union = new Union(value); }

        public Value(int value) : this()
        { _type = Type.Int; _union = new Union(value); }

        public Value(long value) : this()
        { _type = Type.Long; _union = new Union(value); }

        public Value(bool value) : this()
        { _type = Type.Bool; _union = new Union(value ? 1 : 0); }

        public Value(string value) : this()
        { _type = Type.String; _ref = value; }

        public Value(Color value) : this()
        { _type = Type.Color; _union = new Union(value); }

        public Value(DateTime value) : this()
        { _type = Type.DateTime; _union = new Union(value.Ticks); }

        public Value(OuterRef value) : this()
        { _type = Type.OuterRef; _union = new Union(value); }

        public Value(ulong value) : this()
        { _type = Type.ULong; _union = new Union(value); }

        public Value(IResource value) : this()
        { _type = Type.Resource; _ref = value; }

        public Value(Guid value) : this()
        { _type = Type.Guid; _union = new Union(value); }

        public float Float
        { get { Check(Type.Float); return _union.Float; } }

        public Vector2 Vector2
        { get { Check(Type.Vector2); return _union.Vector2; } }

        public Vector3 Vector3
        { get { Check(Type.Vector3); return _union.Vector3; } }

        public Quaternion Quaternion
        { get { Check(Type.Quaternion); return _union.Quaternion; } }

        public int Int
        { get { Check(Type.Int); return _union.Int; } }

        public long Long
        { get { Check(Type.Long); return _union.Long; } }

        public bool Bool
        { get { Check(Type.Bool); return _union.Int != 0; } }

        public string String
        { get { Check(Type.String); return (string)_ref; } }

        public Color Color
        { get { Check(Type.Color); return _union.Color; } }

        public DateTime DateTime
        //#wrong: { get { Check(Type.DateTime); return new DateTime(1970, 1, 1).AddMilliseconds(_intValue); } }
        { get { Check(Type.DateTime); return new DateTime(_union.Long); } }

        public OuterRef OuterRef
        { get { Check(Type.OuterRef); return _union.OuterRef; } }

        public ulong ULong
        { get { Check(Type.ULong); return _union.ULong; } }

        public BaseResource Resource
        { get { Check(Type.Resource); return (BaseResource)_ref; } }

        public Guid Guid
        { get { Check(Type.Guid); return _union.OuterRef.Guid; } }

        public float AsFloat()
        {
            switch (ValueType)
            {
                case Type.Float: return _union.Float;
                case Type.Int: return _union.Int;
                case Type.Long: return _union.Long;
                case Type.ULong: return _union.ULong;
                default: throw new NotSupportedException($"Can't convert {ValueType} to Float");
            }
        }

        public override string ToString()
        {
            switch (_type)
            {
                case Type.None: break;
                case Type.Float: return _union.Float.ToString("F2");
                case Type.Vector3: return _union.Vector3.ToString();
                case Type.Vector2: return _union.Vector2.ToString();
                case Type.Quaternion: return _union.Quaternion.ToString();
                case Type.Int: return _union.Int.ToString();
                case Type.Long: return _union.Long.ToString();
                case Type.ULong: return _union.Long.ToString();
                case Type.Bool: return (_union.Int != 0).ToString();
                case Type.String: return (string)_ref;
                case Type.DateTime: return SharedHelpers.TimeStamp(DateTime);
                case Type.OuterRef: return _union.OuterRef.ToString();
                case Type.Resource: return _ref.ToString();
                case Type.Guid: return _union.OuterRef.Guid.ToString();
                default:
                    return $"Unhandled case for type: `{_type}`";
            }
            return string.Empty;
        }

        private void Check(Type type)
        {
            if (type != _type && _type != Type.None)
                throw new Exception($"Mismatch value type: expected {type} but got {_type}");
        }

        private readonly object _ref;
        private readonly Union _union;
        private readonly Type _type;

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct Union
        {
            [FieldOffset(0)]
            public readonly float Float;
            [FieldOffset(0)]
            public readonly long Long;
            [FieldOffset(0)]
            public readonly ulong ULong;
            [FieldOffset(0)]
            public readonly int Int;
            [FieldOffset(0)]
            public readonly Vector2 Vector2;
            [FieldOffset(0)]
            public readonly Vector3 Vector3;
            [FieldOffset(0)]
            public readonly Quaternion Quaternion;
            [FieldOffset(0)]
            public readonly Color Color;
            [FieldOffset(0)]
            public readonly OuterRef OuterRef;
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(float value) : this() { Float = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(long value) : this() { Long = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(ulong value) : this() { ULong = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(int value) : this() { Int = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(Vector2 value) : this() { Vector2 = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(Vector3 value) : this() { Vector3 = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(Quaternion value) : this() { Quaternion = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(Color value) : this() { Color = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(OuterRef value) : this() { OuterRef = value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Union(Guid value) : this() { OuterRef.Guid = value; }
        }
        
        public static IEnumerable<System.Type> MakeGenericTypes(System.Type genericType)
        {
            if (!genericType.IsGenericType) throw new ArgumentException($"{genericType} is not a generic");
            return SupportedTypes.Where(x => x.Item1 != Value.Type.None).Select(x => genericType.MakeGenericType(x.Item2));
        }
    }
}
