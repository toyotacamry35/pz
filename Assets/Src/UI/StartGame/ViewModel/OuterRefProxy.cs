using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;
using SharedCode.Repositories;

namespace Uins
{
    public class OuterRefProxy<TEntity> : BindingVmodel where TEntity : class, IEntity
    {
        private TouchableDeferredEgoProxy<TEntity> _entityTouchable = new TouchableDeferredEgoProxy<TEntity>();
        private UnityThreadStream<bool> _outStream;

        public OuterRefProxy(IStream<OuterRef<IEntity>> realmOuterRefStream, IStream<IEntitiesRepository> repositoryStream)
        {
            _outStream = new UnityThreadStream<bool>(s => s + nameof(OuterRefProxy<TEntity>));
            EntityExists = _outStream.Last(D, false);

            var toucher = new Toucher.Proxy<TEntity>(
                null,
                null,
                () => { _outStream.OnNext(false); });

            D.Add(_entityTouchable.Subscribe(toucher));

            var type = typeof(TEntity);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);

            repositoryStream
                .Zip(D, realmOuterRefStream)
                .Action(
                    D,
                    (repository, outerRef) =>
                    {
                        _entityTouchable.Disconnect();

                        if (outerRef == OuterRef<IEntity>.Invalid || repository == null)
                        {
                            _outStream.OnNext(false);
                            return;
                        }

                        _entityTouchable.Connect(
                            repository,
                            outerRef.TypeId,
                            outerRef.Guid,
                            replicationLevel,
                            TimeSpan.FromMilliseconds(3000),
                            () => { _outStream.OnNext(true); },
                            () => { _outStream.OnNext(false); });
                    }
                );
        }

        public IReactiveProperty<bool> EntityExists { get; }

        public IStream<TValue> ToStreamWithDefault<TValue>(
            ICollection<IDisposable> disposables,
            Expression<Func<TEntity, TValue>> getValueExpression,
            TValue defaultValue = default)
        {
            var valueStream = _entityTouchable.ToStream(disposables, getValueExpression);
            return EntityExists
                .ZipSecondOrDefault(disposables, valueStream)
                .Func(disposables, (exists, value) => exists ? value : defaultValue);
        }

        public override void Dispose()
        {
            _outStream?.OnCompleted();
            _outStream?.Dispose();
            _outStream = null;

            _entityTouchable?.Dispose();
            _entityTouchable = null;
            
            base.Dispose();
        }
    }
}