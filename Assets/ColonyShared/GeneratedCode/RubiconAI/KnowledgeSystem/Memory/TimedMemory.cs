﻿using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Utils;
using System;
using Assets.Src.RubiconAI.BehaviourTree;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SharedCode.EntitySystem;
using GeneratedCode.Repositories;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace Assets.Src.RubiconAI.KnowledgeSystem.Memory
{
    public class TimedMemory
    {
        Predicate<(MemoryPieceHandle, MemoryData)> _prunePredicate;
        IEntitiesRepository _repo;
        object _lock = new object();
        public TimedMemory(IEntitiesRepository repo)
        {
            _repo = repo;
            _prunePredicate = RemoveMemoryPiece;
        }
        FastDictionary<MemoryPieceHandle, MemoryData> _memoryPieces = new FastDictionary<MemoryPieceHandle, MemoryData>();
        FastDictionary<CachedMemoryHandle, float> _cachedValues = new FastDictionary<CachedMemoryHandle, float>();
        public void GetMemoryPieces(List<(MemoryPieceHandle, MemoryData)> toList)
        {
            lock (_lock)
            {
                foreach (var mPiece in MemoryPieces)
                {
                    toList.Add((mPiece.Key, mPiece.Value));
                }
            }
        }
        public void FilterMemoryPieces(MemorizedStatDef statDef, List<Legionary> toList)
        {
            lock (_lock)
            {
                foreach (var mPiece in MemoryPieces)
                {
                    if (mPiece.Key.StatDef == statDef)
                        toList.Add(mPiece.Key.About);
                }
            }
        }
        public FastDictionary<MemoryPieceHandle, MemoryData> MemoryPieces { get { Prune(); return _memoryPieces; } }
        public void SetStatMod(Legionary legionary, long duration, MemorizedStatDef category, float flat, StatModKey keyDef)
        {
            lock (_lock)
            {
                AIProfiler.BeginSample("SetStatMod");
                Prune();
                var pieceHandle = new MemoryPieceHandle() { About = legionary, Assigner = keyDef.Assigner, ModDef = keyDef.Def, StatDef = category };
                var memPos = _memoryPieces.InitOrGetPosition(pieceHandle);
                MemoryData data = _memoryPieces.GetAtPosition(memPos);
                if (data != null)
                {
                    data.EndsAt = SyncTime.NowUnsynced + duration;
                    var prevFlat = data.Flat;
                    data.Flat = flat;
                    var pos = _cachedValues.InitOrGetPosition(new CachedMemoryHandle() { About = pieceHandle.About, StatDef = pieceHandle.StatDef });
                    _cachedValues.StoreAtPosition(pos, _cachedValues.GetAtPosition(pos) - prevFlat + flat);
                }
                else
                {
                    _memoryPieces.StoreAtPosition(memPos, new MemoryData() { EndsAt = SyncTime.NowUnsynced + duration, Flat = flat });
                    var pos = _cachedValues.InitOrGetPosition(new CachedMemoryHandle() { About = pieceHandle.About, StatDef = pieceHandle.StatDef });
                    _cachedValues.StoreAtPosition(pos, _cachedValues.GetAtPosition(pos) + flat);
                }
                AIProfiler.EndSample();

            }
        }

        public void RemoveStatMod(Legionary legionary, MemorizedStatDef category, StatModKey keyDef)
        {
            lock (_lock)
            {
                AIProfiler.BeginSample("RemoveStatMod");

                var pieceHandle = new MemoryPieceHandle() { About = legionary, Assigner = keyDef.Assigner, ModDef = keyDef.Def, StatDef = category };
                var handle = MemoryPieces[pieceHandle];
                if (handle == null)
                {
                    AIProfiler.EndSample();
                    return;
                }
                MemoryPieces.Remove(pieceHandle);
                var flat = handle.Flat;
                var cachedMemHandle = new CachedMemoryHandle() { About = pieceHandle.About, StatDef = pieceHandle.StatDef };
                var pos = _cachedValues.InitOrGetPosition(cachedMemHandle);
                var newRes = _cachedValues.GetAtPosition(pos) - flat;
                if (Math.Abs(newRes) < 0.05)
                    _cachedValues.Remove(cachedMemHandle);
                else
                    _cachedValues.StoreAtPosition(pos, newRes);
                Prune();
                AIProfiler.EndSample();
            }
        }
        public float GetStat(Legionary legionary, MemorizedStatDef category)
        {
            lock (_lock)
            {
                AIProfiler.BeginSample("GetStat");
                Prune();
                CollectCache();
                float value = 0f;
                _cachedValues.TryGetValue(new CachedMemoryHandle() { About = legionary, StatDef = category }, out value);
                AIProfiler.EndSample();
                return value;
            }

        }

        long _nextTimeToCache = 0;
        private void CollectCache()
        {
            lock (_lock)
            {
                if (SyncTime.NowUnsynced < _nextTimeToCache)
                    return;
                AIProfiler.BeginSample("CollectCache");
                _nextTimeToCache = SyncTime.NowUnsynced + SyncTime.FromSeconds(1f);
                _cachedValues.Clear();
                foreach (var piece in MemoryPieces)
                {
                    var handle = piece.Key;
                    var pos = _cachedValues.InitOrGetPosition(new CachedMemoryHandle() { About = handle.About, StatDef = handle.StatDef });
                    _cachedValues.StoreAtPosition(pos, _cachedValues.GetAtPosition(pos) + piece.Value.Flat);
                }
            }
            AIProfiler.EndSample();
        }

        public MemoryData GetStatMod(Legionary legionary, MemorizedStatDef category, StatModKey key)
        {
            lock (_lock)
            {
                AIProfiler.BeginSample("GetStatMod");
                Prune();
                var pieceHandle = new MemoryPieceHandle() { About = legionary, Assigner = key.Assigner, ModDef = key.Def, StatDef = category };
                MemoryData memory;
                if (MemoryPieces.TryGetValue(pieceHandle, out memory))
                {
                    AIProfiler.EndSample();
                    return memory;
                }
                else
                {
                    AIProfiler.EndSample();
                    return null;
                }
            }
        }
        long _nextTimeToPrune = 0;
        void Prune()
        {
            lock (_lock)
            {
                if (SyncTime.NowUnsynced < _nextTimeToPrune)
                    return;
                _nextTimeToPrune = SyncTime.NowUnsynced + SyncTime.FromSeconds(1f);
                AIProfiler.BeginSample("Prune");
                _memoryPieces.RemoveWhere(_prunePredicate);
                AIProfiler.EndSample();
            }
        }
        bool RemoveMemoryPiece((MemoryPieceHandle, MemoryData) data)
        {
            return SyncTime.InThePast(data.Item2.EndsAt, SyncTime.NowUnsynced) || !data.Item1.About.IsValid ||
                !(_repo.TryGetLockfree<IHasMortalClientBroadcast>(data.Item1.About.Ref, ReplicationLevel.ClientBroadcast)?.Mortal.IsAlive ?? true);
        }
    }


    public class Memory
    {
        public Legionary About { get; }
        public Dictionary<MemorizedStatDef, MemorizedStat> Stats = new Dictionary<MemorizedStatDef, MemorizedStat>();

        public Memory(Legionary about)
        {
            About = about;
        }
        public Memory(Memory value)
        {
            Stats = new Dictionary<MemorizedStatDef, MemorizedStat>();
            foreach (var stat in value.Stats)
            {
                Stats.Add(stat.Key, new MemorizedStat(stat.Value));
            }
        }

        public Memory(IGrouping<Legionary, (MemoryPieceHandle, MemoryData)> memoryGrouping)
        {
            Stats = new Dictionary<MemorizedStatDef, MemorizedStat>();
            foreach (var stat in memoryGrouping)
            {
                Stats[stat.Item1.StatDef] = new MemorizedStat() { Value = memoryGrouping.Sum(x => (x.Item1.StatDef == stat.Item1.StatDef) ? stat.Item2.Flat : 0) };
            }
        }
    }
    public class MemorizedStat
    {
        public float Value;
        public Dictionary<StatModKey, MemorizedStatMod> Mods = new Dictionary<StatModKey, MemorizedStatMod>();
        public MemorizedStat()
        {

        }
        public MemorizedStat(MemorizedStat value)
        {
            Value = value.Value;
            foreach (var mod in value.Mods)
            {
                Mods.Add(mod.Key, mod.Value);
            }
        }
    }
    public class MemoryData
    {
        public long EndsAt;
        public float Flat;
    }
    public struct MemoryPieceHandle : IEquatable<MemoryPieceHandle>
    {
        public Legionary About;
        public Legionary Assigner;
        public MemorizedStatDef StatDef;
        public StatModifierDef ModDef;

        public bool Equals(MemoryPieceHandle other)
        {
            return other.About == About && other.Assigner == Assigner && StatDef == other.StatDef && ModDef == other.ModDef;
        }

        public override int GetHashCode()
        {
            var hashCode = 1902834753;
            hashCode = hashCode * -1521134295 + About.Index;
            hashCode = hashCode * -1521134295 + Assigner.Index;
            hashCode = hashCode * -1521134295 + EqualityComparer<MemorizedStatDef>.Default.GetHashCode(StatDef);
            hashCode = hashCode * -1521134295 + EqualityComparer<StatModifierDef>.Default.GetHashCode(ModDef);
            return hashCode;
        }
    }
    public struct CachedMemoryHandle : IEquatable<CachedMemoryHandle>
    {
        public Legionary About;
        public MemorizedStatDef StatDef;

        public bool Equals(CachedMemoryHandle other)
        {
            return other.About == About && StatDef == other.StatDef;
        }

        public override int GetHashCode()
        {
            var hashCode = 1902834753;
            hashCode = hashCode * -1521134295 + About.Index;
            hashCode = hashCode * -1521134295 + EqualityComparer<MemorizedStatDef>.Default.GetHashCode(StatDef);
            return hashCode;
        }
    }

    public struct StatModKey : IEquatable<StatModKey>
    {
        public Legionary Assigner;
        public StatModifierDef Def;


        public bool Equals(StatModKey other)
        {
            return Assigner == other.Assigner && Def == other.Def;
        }

        public override int GetHashCode()
        {
            var hashCode = 99442175;
            hashCode = hashCode * -1521134295 + Assigner.Index;
            hashCode = hashCode * -1521134295 + Def.GetHashCode();
            return hashCode;
        }
    }
    public struct MemorizedStatMod : IEquatable<MemorizedStatMod>
    {
        public float Percent;
        public float Flat;
        public long EndsAt;

        public bool Equals(MemorizedStatMod other)
        {
            return other.Percent == Percent && other.Flat == Flat && other.EndsAt == EndsAt;
        }

        public override int GetHashCode()
        {
            var hashCode = -385105166;
            hashCode = hashCode * -1521134295 + Percent.GetHashCode();
            hashCode = hashCode * -1521134295 + Flat.GetHashCode();
            hashCode = hashCode * -1521134295 + EndsAt.GetHashCode();
            return hashCode;
        }
    }

    #region COPYPASTED CUSTOM DICTIONARY WITH REMOVE ALL
    [Serializable()]
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FastDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ISerializable, IDeserializationCallback
    {

        private struct Entry
        {
            public int hashCode;    // Lower 31 bits of hash code, -1 if unused
            public int next;        // Index of next entry, -1 if last
            public TKey key;           // Key of entry
            public TValue value;         // Value of entry
        }

        private int[] buckets;
        private Entry[] entries;
        private int count;
        private int version;
        private int freeList;
        private int freeCount;
        private IEqualityComparer<TKey> comparer;
        private KeyCollection keys;
        private ValueCollection values;
        private Object _syncRoot;

        private SerializationInfo m_siInfo; //A temporary variable which we need during deserialization.        

        // constants for serialization
        private const String VersionName = "Version";
        private const String HashSizeName = "HashSize";  // Must save buckets.Length
        private const String KeyValuePairsName = "KeyValuePairs";
        private const String ComparerName = "Comparer";

        public FastDictionary() : this(100, null) { }

        public FastDictionary(int capacity) : this(capacity, null) { }

        public FastDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) { }

        public FastDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity");
            if (capacity > 0) Initialize(capacity);
            if (comparer == null) comparer = EqualityComparer<TKey>.Default;
            this.comparer = comparer;
        }

        public FastDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }

        public FastDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) :
            this(dictionary != null ? dictionary.Count : 0, comparer)
        {

            if (dictionary == null)
            {
                throw new ArgumentNullException("Dictionary");
            }

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        protected FastDictionary(SerializationInfo info, StreamingContext context)
        {
            //We can't do anything with the keys and values until the entire graph has been deserialized
            //and we have a resonable estimate that GetHashCode is not going to fail.  For the time being,
            //we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            m_siInfo = info;
        }

        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                return comparer;
            }
        }

        public int Count
        {
            get { return count - freeCount; }
        }

        public KeyCollection Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(this);
                return keys;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(this);
                return keys;
            }
        }

        public ValueCollection Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this);
                return values;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                if (values == null) values = new ValueCollection(this);
                return values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return entries[i].value;
                return default(TValue);
            }
            set
            {
                Insert(key, value, false);
            }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int i = FindEntry(keyValuePair.Key);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(entries[i].value, keyValuePair.Value))
            {
                return true;
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            int i = FindEntry(keyValuePair.Key);
            if (i >= 0 && EqualityComparer<TValue>.Default.Equals(entries[i].value, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                Array.Clear(entries, 0, count);
                freeList = -1;
                count = 0;
                freeCount = 0;
                version++;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && entries[i].value == null) return true;
                }
            }
            else
            {
                EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0 && c.Equals(entries[i].value, value)) return true;
                }
            }
            return false;
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");

            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
            }

            int count = this.count;
            Entry[] entries = this.entries;
            for (int i = 0; i < count; i++)
            {
                if (entries[i].hashCode >= 0)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                }
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue(VersionName, version);
            info.AddValue(ComparerName, comparer, typeof(IEqualityComparer<TKey>));
            info.AddValue(HashSizeName, buckets == null ? 0 : buckets.Length); //This is the length of the bucket array.
            if (buckets != null)
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue(KeyValuePairsName, array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        private int FindEntry(TKey key)
        {
            if (buckets != null)
            {
                int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                {

                    if (entries[i].hashCode == hashCode)
                        if (comparer.Equals(entries[i].key, key))
                            return i;

                }
            }
            return -1;
        }

        private void Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            buckets = new int[size];
            for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
            entries = new Entry[size];
            freeList = -1;
        }

        public int InitOrGetPosition(TKey key)
        {
            return Insert(key, default(TValue), true);
        }
        public void StoreAtPosition(int pos, TValue value)
        {
            entries[pos].value = value;
            version++;
        }

        public TValue GetAtPosition(int pos)
        {
            return entries[pos].value;
        }
        public void RemoveWhere(Predicate<(TKey, TValue)> predicate)
        {
            for (int bucket = 0; bucket < buckets.Length; bucket++)
            {
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode > 0 && predicate((entries[i].key, entries[i].value)))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        entries[i].key = default(TKey);
                        entries[i].value = default(TValue);
                        freeList = i;
                        freeCount++;
                        version++;
                    }
                }
            }
        }
        private int Insert(TKey key, TValue value, bool add)
        {

            if (buckets == null) Initialize(1000);
            int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
            for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                {
                    if (add)
                    {
                        return i;
                    }
                    entries[i].value = value;
                    version++;
                    return i;
                }
            }
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length) Resize();
                index = count;
                count++;
            }
            int bucket = hashCode % buckets.Length;
            entries[index].hashCode = hashCode;
            entries[index].next = buckets[bucket];
            entries[index].key = key;
            entries[index].value = value;
            buckets[bucket] = index;
            version++;

            return index;
        }

        public virtual void OnDeserialization(Object sender)
        {
            if (m_siInfo == null)
            {
                // It might be necessary to call OnDeserialization from a container if the container object also implements
                // OnDeserialization. However, remoting will call OnDeserialization again.
                // We can return immediately if this function is called twice. 
                // Note we set m_siInfo to null at the end of this method.
                return;
            }

            int realVersion = m_siInfo.GetInt32(VersionName);
            int hashsize = m_siInfo.GetInt32(HashSizeName);
            comparer = (IEqualityComparer<TKey>)m_siInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKey>));

            if (hashsize != 0)
            {
                buckets = new int[hashsize];
                for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
                entries = new Entry[hashsize];
                freeList = -1;

                KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])
                    m_siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

                if (array == null)
                {
                    throw new SerializationException("Serialization_MissingKeyValuePairs");
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Key == null)
                    {
                        throw new SerializationException("Serialization_NullKey");
                    }
                    Insert(array[i].Key, array[i].Value, true);
                }
            }
            else
            {
                buckets = null;
            }

            version = realVersion;
            m_siInfo = null;
        }

        private void Resize()
        {
            int newSize = HashHelpers.GetPrime(count + 2);
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;
            Entry[] newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);
            for (int i = 0; i < count; i++)
            {
                int bucket = newEntries[i].hashCode % newSize;
                newEntries[i].next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
            buckets = newBuckets;
            entries = newEntries;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (buckets != null)
            {
                int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                int bucket = hashCode % buckets.Length;
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }
                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        entries[i].key = default(TKey);
                        entries[i].value = default(TValue);
                        freeList = i;
                        freeCount++;
                        version++;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Arg_RankMultiDimNotSupported");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException("Arg_NonZeroLowerBound");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
            }

            KeyValuePair<TKey, TValue>[] pairs = array as KeyValuePair<TKey, TValue>[];
            if (pairs != null)
            {
                CopyTo(pairs, index);
            }
            else if (array is DictionaryEntry[])
            {
                DictionaryEntry[] dictEntryArray = array as DictionaryEntry[];
                Entry[] entries = this.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0)
                    {
                        dictEntryArray[index++] = new DictionaryEntry(entries[i].key, entries[i].value);
                    }
                }
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException("Argument_InvalidArrayType");
                }

                try
                {
                    int count = this.count;
                    Entry[] entries = this.entries;
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i].hashCode >= 0)
                        {
                            objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].key, entries[i].value);
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Argument_InvalidArrayType");
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.KeyValuePair);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)Keys; }
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    int i = FindEntry((TKey)key);
                    if (i >= 0)
                    {
                        return entries[i].value;
                    }
                }
                return null;
            }
            set
            {
                VerifyKey(key);
                VerifyValueType(value);
                this[(TKey)key] = (TValue)value;
            }
        }

        private static void VerifyKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (!(key is TKey))
            {
                throw new ArgumentException("Invalid type", "key");
            }
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return (key is TKey);
        }

        private static void VerifyValueType(object value)
        {
            if ((value is TValue) || (value == null && !typeof(TValue).IsValueType))
            {
                return;
            }
            throw new ArgumentException("Invalid type", "value");
        }

        void IDictionary.Add(object key, object value)
        {
            VerifyKey(key);
            VerifyValueType(value);
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, Enumerator.DictEntry);
        }

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        [Serializable()]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>,
            IDictionaryEnumerator
        {
            private FastDictionary<TKey, TValue> dictionary;
            private int version;
            private int index;
            private KeyValuePair<TKey, TValue> current;
            private int getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int DictEntry = 1;
            internal const int KeyValuePair = 2;

            internal Enumerator(FastDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = new KeyValuePair<TKey, TValue>();
            }

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entries[index].hashCode >= 0)
                    {
                        current = new KeyValuePair<TKey, TValue>(dictionary.entries[index].key, dictionary.entries[index].value);
                        index++;
                        return true;
                    }
                    index++;
                }

                index = dictionary.count + 1;
                current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return current; }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    }

                    if (getEnumeratorRetType == DictEntry)
                    {
                        return new System.Collections.DictionaryEntry(current.Key, current.Value);
                    }
                    else
                    {
                        return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                    }
                }
            }

            void IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                }

                index = 0;
                current = new KeyValuePair<TKey, TValue>();
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    }

                    return new DictionaryEntry(current.Key, current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    }

                    return current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    }

                    return current.Value;
                }
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [Serializable()]
        public sealed class KeyCollection : ICollection<TKey>, ICollection
        {
            private FastDictionary<TKey, TValue> dictionary;

            public KeyCollection(FastDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }
                this.dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
                }

                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0) array[index++] = entries[i].key;
                }
            }

            public int Count
            {
                get { return dictionary.Count; }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException("NotSupported_KeyCollectionSet");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("NotSupported_KeyCollectionSet");
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException("NotSupported_KeyCollectionSet");
                return false;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Arg_RankMultiDimNotSupported");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Arg_NonZeroLowerBound");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
                }

                TKey[] keys = array as TKey[];
                if (keys != null)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArgumentException("Argument_InvalidArrayType");
                    }

                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].key;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Argument_InvalidArrayType");
                    }
                }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            [Serializable()]
            public struct Enumerator : IEnumerator<TKey>, System.Collections.IEnumerator
            {
                private FastDictionary<TKey, TValue> dictionary;
                private int index;
                private int version;
                private TKey currentKey;

                internal Enumerator(FastDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentKey = default(TKey);
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentKey = dictionary.entries[index].key;
                            index++;
                            return true;
                        }
                        index++;
                    }

                    index = dictionary.count + 1;
                    currentKey = default(TKey);
                    return false;
                }

                public TKey Current
                {
                    get
                    {
                        return currentKey;
                    }
                }

                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                        }

                        return currentKey;
                    }
                }

                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    }

                    index = 0;
                    currentKey = default(TKey);
                }
            }
        }

        [DebuggerDisplay("Count = {Count}")]
        [Serializable()]
        public sealed class ValueCollection : ICollection<TValue>, ICollection
        {
            private FastDictionary<TKey, TValue> dictionary;

            public ValueCollection(FastDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }
                this.dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
                }

                if (array.Length - index < dictionary.Count)
                {
                    throw new ArgumentException("Arg_ArrayPlusOffTooSmall");
                }

                int count = dictionary.count;
                Entry[] entries = dictionary.entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].hashCode >= 0) array[index++] = entries[i].value;
                }
            }

            public int Count
            {
                get { return dictionary.Count; }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("NotSupported_ValueCollectionSet");
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("NotSupported_ValueCollectionSet");
                return false;
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("NotSupported_ValueCollectionSet");
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return dictionary.ContainsValue(item);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (array.Rank != 1)
                {
                    throw new ArgumentException("Arg_RankMultiDimNotSupported");
                }

                if (array.GetLowerBound(0) != 0)
                {
                    throw new ArgumentException("Arg_NonZeroLowerBound");
                }

                if (index < 0 || index > array.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_NeedNonNegNum");
                }

                if (array.Length - index < dictionary.Count)
                    throw new ArgumentException("Arg_ArrayPlusOffTooSmall");

                TValue[] values = array as TValue[];
                if (values != null)
                {
                    CopyTo(values, index);
                }
                else
                {
                    object[] objects = array as object[];
                    if (objects == null)
                    {
                        throw new ArgumentException("Argument_InvalidArrayType");
                    }

                    int count = dictionary.count;
                    Entry[] entries = dictionary.entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries[i].hashCode >= 0) objects[index++] = entries[i].value;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        throw new ArgumentException("Argument_InvalidArrayType");
                    }
                }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            Object ICollection.SyncRoot
            {
                get { return ((ICollection)dictionary).SyncRoot; }
            }

            [Serializable()]
            public struct Enumerator : IEnumerator<TValue>, System.Collections.IEnumerator
            {
                private FastDictionary<TKey, TValue> dictionary;
                private int index;
                private int version;
                private TValue currentValue;

                internal Enumerator(FastDictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary.version;
                    index = 0;
                    currentValue = default(TValue);
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    }

                    while ((uint)index < (uint)dictionary.count)
                    {
                        if (dictionary.entries[index].hashCode >= 0)
                        {
                            currentValue = dictionary.entries[index].value;
                            index++;
                            return true;
                        }
                        index++;
                    }
                    index = dictionary.count + 1;
                    currentValue = default(TValue);
                    return false;
                }

                public TValue Current
                {
                    get
                    {
                        return currentValue;
                    }
                }

                Object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary.count + 1))
                        {
                            throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                        }

                        return currentValue;
                    }
                }

                void System.Collections.IEnumerator.Reset()
                {
                    if (version != dictionary.version)
                    {
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                    }
                    index = 0;
                    currentValue = default(TValue);
                }
            }
        }
    }

    internal static class HashHelpers
    {
        // Table of prime numbers to use as hash table sizes. 
        // The entry used for capacity is the smallest prime number in this aaray
        // that is larger than twice the previous capacity. 

        internal static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        internal static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }
                return true;
            }
            return (candidate == 2);
        }

        internal static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException("min");

            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min) return prime;
            }

            //outside of our predefined table. 
            //compute the hard way. 
            for (int i = (min | 1); i < Int32.MaxValue; i += 2)
            {
                if (IsPrime(i))
                    return i;
            }
            return min;
        }
    }

    #endregion




}
