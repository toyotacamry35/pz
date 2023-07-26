using System;
using SharedCode.EntitySystem;
using GeneratorAnnotations;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Entities.Service;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem.Delta;
using System.Collections.Generic;

namespace GeneratedCode.DeltaObjects
{
    public interface IHasFounderPack : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] IFounderPack FounderPack { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface IFounderPack : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [BsonIgnore]
        IDeltaDictionary<string, bool> Packs { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        ValueTask RefreshStatus(List<string> packs);
    }

    public partial class FounderPack : IFounderPackImplementRemoteMethods
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask RefreshStatusImpl(List<string> packs)
        {
            Packs = new DeltaDictionary<string, bool>();
            foreach (var p in packs)
                Packs.Add(p, true);
        }
    }
}
