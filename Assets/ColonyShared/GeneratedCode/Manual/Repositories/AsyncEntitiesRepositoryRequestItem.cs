//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GeneratedCode.Repositories;
//using SharedCode.ActorServices.DataSyncronisation;
//using SharedCode.Entities.Cloud;
//using SharedCode.EntitySystem;

//namespace GeneratedCode.Manual.Repositories
//{
//    public class AsyncEntitiesRepositoryRequestItem
//    {
//        public AsyncEntitiesRepositoryRequestItem Parent { get; set; }

//        public ConcurrentDictionary<AsyncEntitiesRepositoryRequestItem, object> Childs { get; set; }

//        public IEntitiesContainer ContainerWrapper { get; }

//        public List<DefferedReplicateSubscription> SubscribeEntities;

//        public Dictionary<Guid, Tuple<IRepositoryCommunicationEntity, UploadBatchContainer, UpdateBatchContainer>> UploadContainers;

//        public Dictionary<Guid, Tuple<IRepositoryCommunicationEntity, DestroyBatchContainer, DowngradeBatchContainer>> DestroyContainers;

//        public AsyncEntitiesRepositoryRequestItem(IEntitiesContainer containerWrapper)
//        {
//            ContainerWrapper = containerWrapper;
//        }

//        public override string ToString()
//        {
//            return $"<AsyncEntitiesRepositoryRequestItem entitiesContainer:{ContainerWrapper.ToString()}>";
//        }
//    }
//}
