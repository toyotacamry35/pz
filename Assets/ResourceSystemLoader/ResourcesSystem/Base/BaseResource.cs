using ResourcesSystem.Loader;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedCode.Utils.BsonSerialization;
using System;
using System.IO;

namespace Assets.Src.ResourcesSystem.Base
{
    [UsedImplicitly]
    public abstract class SaveableBaseResource : BaseResource, ISaveableResource
    {
        public Guid Id { get; set; }
    }

    public class StubResource : BaseResource
    {
        public StubResource(string root)
        {
            ((IResource) this).Address = new ResourceIDFull(root);
        }
    }

    public class BinaryResource<T> : BaseResource, IBinarySerializable where T : IBinarySerializable
    {
        public BinaryResource()
        {
            Value = Activator.CreateInstance<T>();
        }

        public BinaryResource(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public void WriteToStream(Stream stream)
        {
            Value.WriteToStream(stream);
        }

        public void ReadFromStream(Stream stream)
        {
            Value.ReadFromStream(stream);
        }
    }

    [UsedImplicitly]
    public abstract class BaseResource : IResource, IComparable
    {
        [NotInSchema]
        ResourceIDFull IResource.Address { get; set; }
        [NotInSchema]
        IResourcesRepository IResource.OwningRepository { get; set; }

        [NotInSchema]
        [JsonProperty(
            "$id",
            Order = 1,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Required = Required.Default
        )]
        string IResource.LocalId { get; set; }

        [NotInSchema]
        [JsonIgnore]
        bool IResource.IsRef { get; set; }

        private string _debugAddress;

        public string ____GetDebugAddress()
        {
            return _debugAddress ?? (_debugAddress = (this as IResource).Address.ToString());
        }

        private string _debugShortName;

        public string ____GetDebugShortName()
        {
            if (_debugShortName == null)
            {
                var addr = (this as IResource).Address.ToString();
                var idx = addr.LastIndexOf('/');
                _debugShortName = idx != -1 ? addr.Substring(idx + 1) : addr;
            }

            return _debugShortName;
        }

        private string _debugRootName;

        public string ____GetDebugRootName()
        {
            if (_debugRootName == null)
            {
                var addr = (this as IResource).Address.Root;
                if (addr != null)
                {
                    var idx = addr.LastIndexOf('/');
                    _debugRootName = idx != -1 ? addr.Substring(idx + 1) : addr;
                }
                else
                    _debugRootName = "<null>";
            }

            return _debugRootName;
        }

        public override string ToString()
        {
            if (GetType().BaseType == typeof(BaseResource))
                return ____GetDebugAddress();
            return GetType().GetFriendlyName() + "#" + ____GetDebugAddress();
        }

        public int CompareTo(object obj)
        {
            var otherAddress = ((IResource) obj).Address;
            var thisAddress = ((IResource) this).Address;
            return thisAddress.CompareTo(otherAddress);
        }
    }
}