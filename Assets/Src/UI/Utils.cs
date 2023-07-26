using System;
using System.Collections.Generic;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;

namespace Uins
{
    public static class Utils
    {
        public static void ConnectAccount(
            ICollection<IDisposable> d, 
            TouchableEgoProxy<IAccountEntityClientFull> accountProxy, 
            IStream<IEntitiesRepository> repositoryStream,
            IStream<Guid> accountIdStream)
        {
            repositoryStream.Zip(d, accountIdStream).Action(d, (repository, accountId) =>
            {
                accountProxy.Connect(
                    repository,
                    AccountEntity.StaticTypeId,
                    accountId,
                    ReplicationLevel.ClientFull
                );
            });
        }

        public static void ConnectEntity<T>(
            ICollection<IDisposable> d, 
            TouchableEgoProxy<T> entityProxy, 
            IStream<IEntitiesRepository> repositoryStream,
            IStream<Guid> entityIdStream,
            int staticTypeId) where T : class, IEntity
        {
            repositoryStream.Zip(d, entityIdStream).Action(d, (repository, id) =>
            {
                entityProxy.Connect(
                    repository,
                    staticTypeId,
                    id,
                    ReplicationLevel.ClientFull
                );
            });
        }

    }
}
