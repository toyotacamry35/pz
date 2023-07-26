using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class DeltaDictionaryWrapper<TKey, TValue, TValue1>: IDeltaDictionaryWrapper<TKey, TValue1> where TValue : IDeltaObject where TValue1 : IBaseDeltaObjectWrapper
    {
        public string TypeName => GetType().ToString();

        private readonly ReplicationLevel _replicationLevel;

        private IDeltaDictionary<TKey, TValue> _deltaDictionary;

        public int TypeId { get { throw new NotImplementedException(); } }

        public ulong LocalId => ((IDeltaObjectExt)_deltaDictionary).LocalId;

        public DeltaDictionaryWrapper(IDeltaDictionary<TKey, TValue> deltaDictionary)
        {
            _deltaDictionary = deltaDictionary;
            _replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(TValue1));
        }

        TValue1 convertValue(TValue value)
        {
            if (value == null)
                return default(TValue1);
            return (TValue1)value.GetReplicationLevel(_replicationLevel);
        }

        public IEnumerator<KeyValuePair<TKey, TValue1>> GetEnumerator()
        {
            return _deltaDictionary.Select(x => new KeyValuePair<TKey, TValue1>(x.Key, convertValue(x.Value))).AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue1> item)
        {
            //TODO возможно здесь (и в аналогичных методах на изменение) стоит бросать эксепшн, так как враппер говорит о том что мы не на мастер копии и не имеем права менять коллекцию
            var baseItem = item.Value != null ? (TValue)((IBaseDeltaObjectWrapper)item.Value).GetBaseDeltaObject() : default(TValue);
            _deltaDictionary.Add(new KeyValuePair<TKey, TValue>(item.Key, baseItem));
        }

        public void Clear()
        {
            _deltaDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue1> item)
        {
            var baseItem = item.Value != null ? (TValue)((IBaseDeltaObjectWrapper)item.Value).GetBaseDeltaObject() : default(TValue);
            return _deltaDictionary.Contains(new KeyValuePair<TKey, TValue>(item.Key, baseItem));
        }

        public void CopyTo(KeyValuePair<TKey, TValue1>[] array, int arrayIndex)
        {
            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        public bool Remove(KeyValuePair<TKey, TValue1> item)
        {
            var baseItem = item.Value != null ? (TValue)((IBaseDeltaObjectWrapper)item.Value).GetBaseDeltaObject() : default(TValue);
            return _deltaDictionary.Remove(new KeyValuePair<TKey, TValue>(item.Key, baseItem));
        }

        public int Count
        {
            get { return _deltaDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _deltaDictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _deltaDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue1 value)
        {
            var baseItem = value != null ? (TValue)((IBaseDeltaObjectWrapper)value).GetBaseDeltaObject() : default(TValue);
            _deltaDictionary.Add(key, baseItem);
        }

        public bool Remove(TKey key)
        {
            return _deltaDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue1 value)
        {
            value = default(TValue1);
            TValue tmpValue;
            var result = _deltaDictionary.TryGetValue(key, out tmpValue);
            if (result)
                value = convertValue(tmpValue);
            return result;
        }

        public TValue1 this[TKey key]
        {
            get { return convertValue(_deltaDictionary[key]); }
            set { _deltaDictionary[key] = value != null ? (TValue)((IBaseDeltaObjectWrapper)value).GetBaseDeltaObject() : default(TValue); }
        }

        public ICollection<TKey> Keys {
            get { return _deltaDictionary.Keys; }
        }

        public ICollection<TValue1> Values {
            get { return _deltaDictionary.Values.Select(x => convertValue(x)).ToList(); }//TODO по перформансу конечно пздц, да и поведение немного меняется, но ничего лучшего пока не придумал
        }

        public event DeltaDictionaryChangedDelegate<TKey, TValue1> OnItemAddedOrUpdated
        {
            add
            {
                DeltaDictionaryChangedDelegate<TKey, TValue> func = async (eventArgs) =>
                {
                    var wrapperOld = convertValue(eventArgs.OldValue);
                    var wrapper = convertValue(eventArgs.Value);
                    await AsyncUtils.RunAsyncWithCheckTimeout(() => value(new DeltaDictionaryChangedEventArgs<TKey, TValue1>(eventArgs.Key, wrapperOld, wrapper, eventArgs.Sender)), 
                        ServerCoreRuntimeParameters.EntityEventTimeoutSeconds, 
                            () => $"obj {value.Target?.GetType().FullName ?? "unknown"} method {value.Method.Name}");
                };

                ((IDeltaDictionaryExt<TKey, TValue>)_deltaDictionary).SubscribeCustomAddOrUpdateAction(value.Target, value.Method, func);
            }
            remove
            {
                ((IDeltaDictionaryExt<TKey, TValue>)_deltaDictionary).UnsubscribeCustomAddOrUpdateAction(value.Target, value.Method);
            }
        }

        public event DeltaDictionaryChangedDelegate<TKey, TValue1> OnItemRemoved
        {
            add
            {
                DeltaDictionaryChangedDelegate<TKey, TValue> func = async (eventArgs) =>
                {
                    var wrapperOld = convertValue(eventArgs.OldValue);
                    var wrapper = convertValue(eventArgs.Value);
                    await AsyncUtils.RunAsyncWithCheckTimeout(() => value(new DeltaDictionaryChangedEventArgs<TKey, TValue1>(eventArgs.Key, wrapperOld, wrapper, eventArgs.Sender)),
                        ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {value.Target?.GetType().FullName ?? "unknown"} method {value.Method.Name}");
                };

                ((IDeltaDictionaryExt<TKey, TValue>)_deltaDictionary).SubscribeCustomRemoveAction(value.Target, value.Method, func);
            }
            remove
            {
                ((IDeltaDictionaryExt<TKey, TValue>)_deltaDictionary).UnsubscribeCustomRemoveAction(value.Target, value.Method);
            }
        }

        public event DeltaDictionaryChangedDelegate OnChanged
        {
            add { _deltaDictionary.OnChanged += value; }
            remove { _deltaDictionary.OnChanged -= value; }
        }

        public IDeltaDictionary<TKey, T1> ToDeltaDictionary<T1>() where T1 : IBaseDeltaObjectWrapper
        {
            var wrapper = (IDeltaDictionary<TKey, T1>)Activator.CreateInstance(typeof(DeltaDictionaryWrapper<,,>)
                    .MakeGenericType(typeof(TKey), typeof(TValue), typeof(T1)),
                     _deltaDictionary);
            return wrapper;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            throw new NotImplementedException();
        }

        public int ParentTypeId => _deltaDictionary.ParentTypeId;
        public Guid ParentEntityId => _deltaDictionary.ParentEntityId;
        public IEntitiesRepository EntitiesRepository => _deltaDictionary.EntitiesRepository;
        public Guid OwnerRepositoryId => _deltaDictionary.OwnerRepositoryId;

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

        public string GetAddrByKey(TKey key)
        {
            var list = _deltaDictionary.OrderBy(x => x.Key).ToList();
            var pair = list.FirstOrDefault(x => x.Key.Equals(key));
            if (pair.Equals(default(KeyValuePair<TKey, TValue>)))
                return string.Empty;

            return list.IndexOf(pair).ToString();
        }

        public bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            var index = address;
            if (_deltaDictionary.Count <= index)
            {
                Log.Logger.IfError()?.Message("DeltaObject TryGetProperty {0} Count {1} < index {2}", _deltaDictionary.GetType().Name, _deltaDictionary.Count, index).Write();
                return false;
            }

            var list = _deltaDictionary.OrderBy(x => x.Key).ToList();
            object currProperty = convertValue(list[index].Value);
            if (currProperty == null)
                return false;

            property = (T)currProperty;
            return true;
        }

        public T To<T>() where T : IDeltaObject
        {
            throw new NotImplementedException();
        }

        public IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel)
        {
            throw new NotImplementedException();
        }

        IDeltaObject IBaseDeltaObjectWrapper.GetBaseDeltaObject()
        {
            return _deltaDictionary;
        }

        public void Fill(int depthCount, bool withReadonly)
        {
            throw new NotImplementedException();
        }
    }
}
