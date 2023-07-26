using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using SharedCode.Repositories;

namespace SharedCode.EntitySystem.Delta
{
    public class DeltaListWrapper<T, T1> : IDeltaListWrapper<T1>, IReadOnlyList<T1> where T:IDeltaObject where T1: IBaseDeltaObjectWrapper
    {
        public string TypeName => GetType().ToString();

        private readonly ReplicationLevel _replicationLevel;

        private IDeltaList<T> _deltaList;

        public int TypeId { get { throw new NotImplementedException(); } }

        public ulong LocalId => ((IDeltaObjectExt)_deltaList).LocalId;

        public DeltaListWrapper(IDeltaList<T> deltaList)
        {
            _deltaList = deltaList;
            _replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T1));
        }

        T1 convertValue(T value)
        {
            if (value == null)
                return default(T1);
            return (T1)value.GetReplicationLevel(_replicationLevel);
        }

        public IEnumerator<T1> GetEnumerator()
        {
            return _deltaList.Select(x => convertValue(x)).AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T1 item)
        {
            //TODO возможно здесь (и в аналогичных методах на изменение) стоит бросать эксепшн, так как враппер говорит о том что мы не на мастер копии и не имеем права менять коллекцию
            var baseItem = item != null ? (T)((IBaseDeltaObjectWrapper)item).GetBaseDeltaObject() : default(T);
            _deltaList.Add(baseItem);
        }

        public void Clear()
        {
            _deltaList.Clear();
        }

        public bool Contains(T1 item)
        {
            var baseItem = item != null ? (T) ((IBaseDeltaObjectWrapper) item).GetBaseDeltaObject() : default(T);
            return _deltaList.Contains(baseItem);
        }

        public void CopyTo(T1[] array, int arrayIndex)
        {
            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        public bool Remove(T1 item)
        {
            var baseItem = item != null ? (T)((IBaseDeltaObjectWrapper)item).GetBaseDeltaObject() : default(T);
            return _deltaList.Remove(baseItem);
        }

        public int Count {
            get { return _deltaList.Count; }
        }

        public bool IsReadOnly {
            get { return _deltaList.IsReadOnly; }
        }

        public int IndexOf(T1 item)
        {
            var baseItem = item != null ? (T)((IBaseDeltaObjectWrapper)item).GetBaseDeltaObject() : default(T);
            return _deltaList.IndexOf(baseItem);
        }

        public void Insert(int index, T1 item)
        {
            var baseItem = item != null ? (T)((IBaseDeltaObjectWrapper)item).GetBaseDeltaObject() : default(T);
            _deltaList.Insert(index, baseItem);
        }

        public void RemoveAt(int index)
        {
            _deltaList.RemoveAt(index);
        }

        public T1 this[int index]
        {
            get { return convertValue(_deltaList[index]); }
            set { _deltaList[index] = value != null ? (T)((IBaseDeltaObjectWrapper)value).GetBaseDeltaObject() : default(T); }
        }

        public event DeltaListChangedDelegate<T1> OnItemAdded
        {
            add
            {
                DeltaListChangedDelegate<T> func = async (eventArgs) =>
                {
                    var wrapper = convertValue(eventArgs.Value);
                    await AsyncUtils.RunAsyncWithCheckTimeout(() => value(new DeltaListChangedEventArgs<T1>(wrapper, eventArgs.Index, eventArgs.Sender)),
                        ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {value.Target?.GetType().FullName ?? "unknown"} method {value.Method.Name}");
                };

                ((IDeltaListExt<T>)_deltaList).SubscribeCustomAddAction(value.Target, value.Method, func);
            }
            remove
            {
                ((IDeltaListExt<T>)_deltaList).UnsubscribeCustomAddAction(value.Target, value.Method);
            }
        }

        public event DeltaListChangedDelegate<T1> OnItemRemoved
        {
            add
            {
                DeltaListChangedDelegate<T> func = async (eventArgs) =>
                {
                    var wrapper = convertValue(eventArgs.Value);
                    await AsyncUtils.RunAsyncWithCheckTimeout(() => value(new DeltaListChangedEventArgs<T1>(wrapper, eventArgs.Index, eventArgs.Sender)),
                        ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {value.Target?.GetType().FullName ?? "unknown"} method {value.Method.Name}");
                };

                ((IDeltaListExt<T>)_deltaList).SubscribeCustomRemoveAction(value.Target, value.Method, func);
            }
            remove
            {
                ((IDeltaListExt<T>)_deltaList).UnsubscribeCustomRemoveAction(value.Target, value.Method);
            }
        }

        public event DeltaListChangedDelegate OnChanged
        {
            add { _deltaList.OnChanged += value; }
            remove { _deltaList.OnChanged -= value; }
        }

        public IDeltaList<T11> ToDeltaList<T11>() where T11 : IBaseDeltaObjectWrapper
        {
            var wrapper = (IDeltaList<T11>)Activator.CreateInstance(typeof(DeltaListWrapper<,>)
                    .MakeGenericType(typeof(T), typeof(T11)),
                _deltaList);
            return wrapper;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            throw new NotImplementedException();
        }

        public int ParentTypeId => _deltaList.ParentTypeId;
        public Guid ParentEntityId => _deltaList.ParentEntityId;
        public IEntitiesRepository EntitiesRepository => _deltaList.EntitiesRepository;
        public Guid OwnerRepositoryId => _deltaList.OwnerRepositoryId;

        public bool NeedFireEvents()
        {
            throw new NotImplementedException();
        }

        public void ClearDelta()
        {
            throw new NotImplementedException();
        }

        public void GetAllLinkedEntities(long replicationMask, List<(long level, IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            throw new NotImplementedException();
        }
        
        public void LinkEntityRefsRecursive(IEntitiesRepository repository)
        {
            throw new NotImplementedException();
        }

        public void MarkAllChanged()
        {
            throw new NotImplementedException();
        }

        public void SubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribePropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeAll()
        {
            throw new NotImplementedException();
        }

        public bool IsChanged()
        {
            throw new NotImplementedException();
        }

        public void ProcessEvents(List<Func<Task>> container)
        {
            throw new NotImplementedException();
        }

        public bool TryGetProperty<T2>(int address, out T2 property)
        {
            property = default(T2);
            int index = address;
            if (_deltaList.Count <= index)
            {
                Log.Logger.IfError()?.Message("DeltaObject TryGetProperty {0} Count {1} < index {2}", _deltaList.GetType().Name, _deltaList.Count, index).Write();
                return false;
            }

            object currProperty = convertValue(_deltaList[index]);
            property = (T2)currProperty;
            return true;
        }

        public T2 To<T2>() where T2 : IDeltaObject
        {
            throw new NotImplementedException();
        }

        public IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel)
        {
            throw new NotImplementedException();
        }

        IDeltaObject IBaseDeltaObjectWrapper.GetBaseDeltaObject()
        {
            return _deltaList;
        }

        public void Fill(int depthCount, bool withReadonly)
        {
            throw new NotImplementedException();
        }
    }
}
