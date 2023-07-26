//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GeneratorAnnotations;
//using SharedCode.Entities.Core;
//using SharedCode.EntitySystem;
//using SharedCode.EntitySystem.ChainCalls;
//using SharedCode.EntitySystem.Delta;
//using MongoDB.Bson.Serialization.Attributes;

//namespace SharedCode.Entities.Core
//{
//    //заглушка для генерации базовых пропертей в BaseEntity.
//    //Нужно расскоментировать, перегенерить, скопипастить проперти в BaseEntity.Impl, перемаркировать протоаф начиная с тега 2 и закомментировать этот интерфейс обратно
//    //Потом нужно доработать генератор чтобы он генерил сразу в BaseEntity.Impl

//    [GenerateDeltaObjectCode]
//    public interface IBaseEntityDummy : IEntity
//    {
//        [ReplicationLevel(ReplicationLevel.Always)]
//        [BsonIgnore]
//        Guid OwnerNodeId222 { get; set; }

//        [ReplicationLevel(ReplicationLevel.Master)]
//        IDeltaDictionary<Guid, IEntityMethodsCallsChain> ChainCalls222 { get; set; }

//        [ReplicationLevel(ReplicationLevel.Master)]
//        [BsonIgnore]
//        ConcurrentDictionary<Guid, ReplicateRefsContainer> ReplicateRepositoryIds222 { get; set; }
//    }
//}
