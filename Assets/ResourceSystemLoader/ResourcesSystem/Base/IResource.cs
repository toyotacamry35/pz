using System;
using System.Collections.Generic;
using ProtoBuf;
using Assets.ColonyShared.SharedCode.Utils;
using ResourcesSystem.Loader;

namespace Assets.Src.ResourcesSystem.Base
{
    [ProtoContract]
    public struct ResourceIDFull : IEquatable<ResourceIDFull>, IComparable<ResourceIDFull>, IComparable
    {
        [ProtoMember(1)]
        public string Root { get; set; }
        [ProtoMember(2)]
        public int Line { get; set;}
        [ProtoMember(3)]
        public int Col { get; set;}
        [ProtoMember(4)]
        public int ProtoIndex { get; set;}
        public ulong RootID() => Root == null? 0 : Crc64.Compute(Root);
        public ResourceIDFull(string root, int line, int col, int protoIndex)
        {
            Root = root;
            Line = line;
            Col = col;
            ProtoIndex = protoIndex;
        }
        public ResourceIDFull(string root, int line, int col)
        {
            Root = root;
            Line = line;
            Col = col;
            ProtoIndex = 0;
        }

        public ResourceIDFull(string root)
        {
            Root = root;
            Line = 0;
            Col = 0;
            ProtoIndex = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceIDFull && Equals((ResourceIDFull)obj);
        }

        public bool Equals(ResourceIDFull other)
        {
            return Root == other.Root &&
                   Line == other.Line &&
                   Col == other.Col && 
                   ProtoIndex == other.ProtoIndex;
        }

        public override int GetHashCode()
        {
            var hashCode = 1301723813;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Root);
            hashCode = hashCode * -1521134295 + Line.GetHashCode();
            hashCode = hashCode * -1521134295 + Col.GetHashCode();
            hashCode = hashCode * -1521134295 + ProtoIndex.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            if (Root == null)
                return "<null>";
            if (Col == 0 && Line == 0 && Col == 0)
                return Root;
            return $"{Root}:{Line}:{Col}:{ProtoIndex}";
        }

        public static bool operator ==(ResourceIDFull ref1, ResourceIDFull ref2)
        {
            return ref1.Equals(ref2);
        }

        public static bool operator !=(ResourceIDFull ref1, ResourceIDFull ref2)
        {
            return !(ref1 == ref2);
        }

        public static ResourceIDFull Parse(string str)
        {
            var parts = str.Split(':');
            switch(parts.Length)
            {
                case 1:
                    return new ResourceIDFull(parts[0]);
                case 3:
                    return new ResourceIDFull(parts[0], int.Parse(parts[1]), int.Parse(parts[2]));
                case 4:
                    return new ResourceIDFull(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
                default:
                    throw new InvalidOperationException("Cannot parse string str");
            }

        }

        public int CompareTo(ResourceIDFull otherAddress)
        {
            var cmp = otherAddress.Root.CompareTo(Root);
            if (cmp != 0)
                return cmp;

            cmp = otherAddress.Line - Line;
            if (cmp != 0)
                return cmp;

            cmp = otherAddress.Col - Col;
            if (cmp != 0)
                return cmp;

            cmp = otherAddress.ProtoIndex - ProtoIndex;
            if (cmp != 0)
                return cmp;

            return 0;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((ResourceIDFull)obj);
        }
    }

    public interface IResource
    {
        bool IsRef { get; set; }
        string LocalId { get; set; }
        ResourceIDFull Address { get; set; }
        IResourcesRepository OwningRepository { get; set; }
    }
}