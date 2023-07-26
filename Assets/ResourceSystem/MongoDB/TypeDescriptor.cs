using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using ResourcesSystem.Loader;

namespace SharedCode.Utils.BsonSerialization
{
    [KnownToGameResources]
    public class TypeDescriptor
    {
        public TypeDescriptor(string type, SerializerDescriptor customSerializer, (string Name, TypeDescriptor Type)[] members)
        {
            Type = type;
            CustomSerializer = customSerializer;
            Members = members;
        }

        public static TypeDescriptor ClassTypeDescriptor(Type type, (string Name, TypeDescriptor Type)[] members)
        {
            return new TypeDescriptor(type.GetFriendlyName(true), null, members);
        }

        public static TypeDescriptor SimpleTypeDescriptor(Type type)
        {
            return new TypeDescriptor(type.GetFriendlyName(true), null, null);
        }

        public static TypeDescriptor CustomSerializerTypeDescriptor(Type type, Type serializerType, int serializerVersion)
        {
            return new TypeDescriptor(type.GetFriendlyName(true), new SerializerDescriptor(serializerType.GetFriendlyName(true), serializerVersion), null);
        }

        public DiffDescriptor DiffFromVersion(TypeDescriptor descriptor)
        {
            DiffDescriptor result = new DiffDescriptor();

            if (descriptor.Type != Type)
                result.Type = String.Format("{0} = > {1}", descriptor.Type, Type);

            if (descriptor.CustomSerializer != CustomSerializer)
            {
                string from = descriptor.CustomSerializer != null ? String.Format("{0}:{1}", descriptor.CustomSerializer.Type, descriptor.CustomSerializer.Version) : "null";
                string to = CustomSerializer != null ? String.Format("{0}:{1}", CustomSerializer.Type, CustomSerializer.Version) : "null";
                result.Seritalizer = String.Format("{0} = > {1}", from, to);
            }

            var members = new Dictionary<string, ValueTuple<TypeDescriptor, TypeDescriptor>>();
            if (descriptor.Members != null)
            {
                foreach (var pair in descriptor.Members)
                {
                    ValueTuple<TypeDescriptor, TypeDescriptor> value;
                    members.TryGetValue(pair.Name, out value);
                    value.Item1 = pair.Type;
                    members[pair.Name] = value;
                }
            }
            if (Members != null)
            {
                foreach (var pair in Members)
                {
                    ValueTuple<TypeDescriptor, TypeDescriptor> value;
                    members.TryGetValue(pair.Name, out value);
                    value.Item2 = pair.Type;
                    members[pair.Name] = value;
                }
            }
            if (members.Count > 0)
            {
                var added = new Dictionary<string, string>();
                var removed = new Dictionary<string, string>();
                var changed = new Dictionary<string, DiffDescriptor>();
                foreach (var pair in members)
                {
                    if (pair.Value.Item1 == null)
                    {
                        added[pair.Key] = pair.Value.Item2.Type;
                    }
                    else if (pair.Value.Item2 == null)
                    {
                        removed[pair.Key] = pair.Value.Item1.Type;
                    }
                    else if (!Enumerable.SequenceEqual(pair.Value.Item1.Hash(), pair.Value.Item2.Hash()))
                    {
                        changed[pair.Key] = pair.Value.Item2.DiffFromVersion(pair.Value.Item1);
                    }
                }

                if (added.Count > 0)
                    result.Added = added;

                if (removed.Count > 0)
                    result.Removed = removed;

                if (changed.Count > 0)
                    result.Changed = changed;
            }

            return result;
        }

        public byte[] Hash()
        {
            List<byte> resultBytes = new List<byte>();

            resultBytes.AddRange(Encoding.UTF8.GetBytes(Type));

            if (CustomSerializer != null)
            {
                resultBytes.Add((byte)'&');
                resultBytes.AddRange(Encoding.UTF8.GetBytes(CustomSerializer.Type));
                resultBytes.Add((byte)'?');
                resultBytes.AddRange(Encoding.UTF8.GetBytes(CustomSerializer.Version.ToString()));
            }
            else
            {
                if (Members != null)
                {
                    foreach (var pair in Members)
                    {
                        resultBytes.Add((byte)'@');
                        resultBytes.AddRange(Encoding.UTF8.GetBytes(pair.Name));
                        resultBytes.Add((byte)':');
                        resultBytes.AddRange(pair.Type.Hash());
                    }
                }
            }

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(resultBytes.ToArray());
            }
        }

        [KnownToGameResources]
        public class SerializerDescriptor
        {
            public SerializerDescriptor(string type, int version)
            {
                Type = type;
                Version = version;
            }
            public string Type { get; private set; }
            public int Version { get; private set; }

            public override bool Equals(object obj)
            {
                var descriptor = obj as SerializerDescriptor;
                return descriptor != null &&
                       Type == descriptor.Type &&
                       Version == descriptor.Version;
            }
        }

        public string Type { get; private set; }
        public SerializerDescriptor CustomSerializer { get; private set; }
        public (string Name, TypeDescriptor Type)[] Members { get; private set; }
    }
}