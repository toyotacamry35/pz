using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SharedCode.Utils.BsonSerialization
{
    public class EntityVersionsSnapshotsCollection : BaseResource
    {
        public List<ResourceRef<EntityVersionsSnapshot>> Snapshots { get; set; } = new List<ResourceRef<EntityVersionsSnapshot>>();
    }

    [KnownToGameResources]
    public class HashItem
    {
        public HashItem(string hash, TypeDescriptor typeDescriptor)
        {
            Hash = hash;
            TypeDescriptor = typeDescriptor;
        }
        public string Hash { get; private set; }
        public TypeDescriptor TypeDescriptor { get; private set; }
    }

    [KnownToGameResources]
    class StrHashItemDictionary : Dictionary<string, HashItem> { }

    public class EntityVersionsSnapshot : BaseResource
    {
        [JsonProperty]
        private StrHashItemDictionary EntityVersions { get; set; } = new StrHashItemDictionary();

        public EntityVersionsSnapshot()
        {
        }

        public void Set(Type type, string hash, TypeDescriptor typeDescriptor)
        {
            EntityVersions[type.GetFriendlyName(false)] = new HashItem(hash, typeDescriptor);
        }

        public HashItem Get(Type type)
        {
            HashItem result;
            if (EntityVersions.TryGetValue(type.GetFriendlyName(false), out result))
                return result;
            else
                return null;
        }
    }
}