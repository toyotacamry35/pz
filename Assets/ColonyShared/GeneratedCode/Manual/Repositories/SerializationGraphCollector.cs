using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GeneratedCode.Manual.Repositories
{
    public struct EntityCollections
    {
        public Dictionary<Guid, UploadInfo> UploadContainers { get; set; }
        public Dictionary<Guid, DestroyInfo> DestroyContainers { get; set; }
        
        public readonly struct UploadInfo
        {
            public UploadInfo(IRepositoryCommunicationEntity communicationEntity, UploadBatchContainer uploadContainer, UpdateBatchContainer updateContainer)
            {
                CommunicationEntity = communicationEntity;
                UploadContainer = uploadContainer;
                UpdateContainer = updateContainer;
            }

            public IRepositoryCommunicationEntity CommunicationEntity { get; }
            public UploadBatchContainer UploadContainer { get; }
            public UpdateBatchContainer UpdateContainer { get; }
        }
        
        public readonly struct DestroyInfo
        {
            public DestroyInfo(IRepositoryCommunicationEntity communicationEntity, DestroyBatchContainer destroyContainer, DowngradeBatchContainer downgradeContainer)
            {
                CommunicationEntity = communicationEntity;
                DestroyContainer = destroyContainer;
                DowngradeContainer = downgradeContainer;
            }

            public IRepositoryCommunicationEntity CommunicationEntity { get; }
            public DestroyBatchContainer DestroyContainer { get; }
            public DowngradeBatchContainer DowngradeContainer { get; }
        }
    }
}
