using System;
using Assets.Src.Utils;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;
using SharedCode.Repositories;

namespace Uins
{
    public class EntityVM<T> : BindingVmodel where T : class, IEntity
    {
        private readonly TouchableEgoProxy<T> _touchable = TouchableUtils.CreateEgoProxy<T>();
        public ITouchable<T> Touchable => _touchable;

        public EntityVM(
            IStream<IEntitiesRepository> repositoryStream,
            IStream<Guid> idStream)
        {
            var type = typeof(T);
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);
            var staticTypeId = ReplicaTypeRegistry.GetIdByType(masterType);

            repositoryStream
                .ZipAnyOrDefault(D, idStream)
                .Action(
                    D,
                    (repository, guid) =>
                    {
                        _touchable.Disconnect();
                        
                        if (repository != null && !guid.Equals(Guid.Empty))
                            _touchable.Connect(
                                repository,
                                staticTypeId,
                                guid,
                                replicationLevel
                            );
                    }
                );
        }
    }
}