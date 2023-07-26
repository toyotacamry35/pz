using System;
using UnityEngine;

namespace Src.Tools
{
    // Unity не умеет сериализовывать System.Guid 
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField] private ulong _a;
        [SerializeField] private ulong _b;

        public static readonly SerializableGuid Empty = new SerializableGuid(System.Guid.Empty);
        
        public static SerializableGuid NewGuid()
        {
            return new SerializableGuid(System.Guid.NewGuid());
        }

        public SerializableGuid(Guid guid)
        {
            var bb = guid.ToByteArray();
            _a = (ulong) bb[0] | (ulong) bb[1] << 8 | (ulong) bb[2] << 16 | (ulong) bb[3] << 24 | (ulong) bb[4] << 32 | (ulong) bb[5] << 40 | (ulong) bb[6] << 48 | (ulong) bb[7] << 56;
            _b = (ulong) bb[8] | (ulong) bb[9] << 8 | (ulong) bb[10] << 16 | (ulong) bb[11] << 24 | (ulong) bb[12] << 32 | (ulong) bb[13] << 40 | (ulong) bb[14] << 48 | (ulong) bb[15] << 56;
        }

        public SerializableGuid(ulong a, ulong b)
        {
            _a = a;
            _b = b;
        }

        public Guid Guid =>
            new Guid((int) _a, (short) (_a >> 32), (short) (_a >> 48), (byte) (_b), (byte) (_b >> 8),
                (byte) (_b >> 16), (byte) (_b >> 24), (byte) (_b >> 32), (byte) (_b >> 40), (byte) (_b >> 48), (byte) (_b >> 56));

        public bool IsValid => this != Empty;

        public static bool operator ==(SerializableGuid v1, SerializableGuid v2) => v1._a == v2._a && v1._b == v2._b;

        public static bool operator !=(SerializableGuid v1, SerializableGuid v2) => v1._a != v2._a || v1._b != v2._b;

        public static implicit operator Guid(SerializableGuid g) => g.Guid;

        public static implicit operator SerializableGuid(Guid g) => new SerializableGuid(g);

        public override bool Equals(object o)
        {
            if (!(o is SerializableGuid))
                return false;
            SerializableGuid guid = (SerializableGuid) o;
            return _a == guid._a && _b == guid._b;
        }

        public bool Equals(SerializableGuid other)
        {
            return _a == other._a && _b == other._b;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_a.GetHashCode() * 397) ^ _b.GetHashCode();
            }
        }

        public override string ToString() => $"{_a:x16}{_b:x16}";
    }
}