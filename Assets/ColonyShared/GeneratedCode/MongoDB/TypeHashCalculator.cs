using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Reflection;
using MongoDB.Bson.Serialization;
using System.Linq;

namespace SharedCode.Utils.BsonSerialization
{
    public static class TypeHashCalculator
    {
        private static readonly ConcurrentDictionary<Type, string> CacheValue = new ConcurrentDictionary<Type, string>();

        public static void ClearCache()
        {
            CacheValue.Clear();
        }

        public static string GetHashByType(Type type)
        {
            string hash;
            if (CacheValue.TryGetValue(type, out hash))
                return hash;

            var descriptor = GetTypeDescriptor(type);
            var hashBytes = descriptor.Hash();
            hash = ByteArrayToString(hashBytes);
            CacheValue[type] = hash;
            return hash;
        }

        public static TypeDescriptor GetTypeDescriptor(Type type)
        {
            return GetTypeDescription(type, BsonSerializer.LookupSerializer(type).GetType());
        }

        private static TypeDescriptor GetTypeDescription(Type type, Type serializerType)
        {
            if (!(serializerType.IsGenericType && serializerType.GetGenericTypeDefinition() == typeof(BsonClassMapSerializer<>)))
            {
                int? version = GetSerializerVersion(serializerType);
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    if (version != null)
                    {
                        return TypeDescriptor.CustomSerializerTypeDescriptor(type, serializerType, version.Value);
                    }
                    else
                    {
                        if (serializerType.Assembly == typeof(BsonSerializer).Assembly)
                            return TypeDescriptor.SimpleTypeDescriptor(type);
                        else
                            throw new Exception($"No serializer \"Version\" field for serializer type {serializerType.Name}");
                    }
                }
            }

            var bsonClassMap = BsonClassMap.LookupClassMap(type);

            var members = bsonClassMap.AllMemberMaps
                .Select(member => (Name: member.ElementName, Type: GetTypeDescription(member.MemberType, member.GetSerializer().GetType())))
                .OrderBy(v => v.Name, StringComparer.Ordinal)
                .ToArray();

            return TypeDescriptor.ClassTypeDescriptor(type, members);
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        private static int? GetSerializerVersion(Type type)
        {
            FieldInfo field = type.GetField("Version", BindingFlags.Public | BindingFlags.Static);
            if (field != null && field.FieldType == typeof(int))
            {
                return (int)field.GetValue(null);
            }
            return null;
        }
    }
}