using Assets.Src.Tools;
using ColonyShared.SharedCode.Utils;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Aspects.Building;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using SharedCode.Entities.GameObjectEntities;
using GeneratedCode.Repositories;
using SharedCode.AI;
using SharedCode.Entities;
using System.Threading;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;

namespace SharedCode.MovementSync
{
    // It's hack to have position sync-ly
    public struct VisibilityDataSample
    {
        public OuterRef<IEntity> Ref;
        public Vector3 Pos;
        public IEntityObjectDef Def;
    }

    public struct GenericVisibilityType : IPointSpatialHashType
    {
        public IEntityObjectDef Def { get; set; }
        public int StaticCellSize => 20;

        public int StaticHeightSize => 20;

        public int StaticReplicationRadius => 100;

        public bool ShouldCheckForRadius => true;
    }
    public interface IUnityProfilerImpl
    {
        void BeginSample(string label);
        void EndSample();
    }

    public static class ProfilerProxy
    {
        public static IUnityProfilerImpl Profiler;
    }
    public class VisibilityGrid
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ConcurrentDictionary<(Guid, IEntitiesRepository), VisibilityGrid> _grids = new ConcurrentDictionary<(Guid, IEntitiesRepository), VisibilityGrid>();
        Guid _shard;
        IEntitiesRepository _repo;

        public VisibilityGrid(Guid shard, IEntitiesRepository repo)
        {
            _shard = shard;
            _repo = repo;
            _samplingSpatialHash = new PointSpatialHash<GenericVisibilityType>(_repo, Logger);
        }

        public static VisibilityGrid Get(Guid shard, IEntitiesRepository repo)
        {
            if (shard == Guid.Empty)
            {
                Logger.IfError()?.Message("Can't get grid for empty worldspace id").Write();
                return null;
            }

            ///#PZ-9704:
            if (Logger.IsDebugEnabled)
                if (!_grids.ContainsKey((shard, repo)))
                    Logger.IfDebug()?.Message/*Warn*/(" - - - - - #Dbg (not Warn): new guid: " + shard).Write();

            return _grids.GetOrCreate((shard, repo), () => new VisibilityGrid(shard, repo));
        }

        public static void ClearAll() => _grids.Clear();

        //ConcurrentDictionary<Type, IPointSpatialHash> _spatialHashesByType = new ConcurrentDictionary<Type, IPointSpatialHash>();
        public PointSpatialHash<GenericVisibilityType> _samplingSpatialHash;

        public void SetGridData(Vector3 position, OuterRef<IEntity> id, Type dataType, IEntityObjectDef def)
        {
            if (position == default)
                Logger.IfError()?.Message("ZERO POS SET TO GRID DATA").Write();
            //var spatialHash = (IPointSpatialHash)_spatialHashesByType.GetOrAdd(dataType, (type) => (IPointSpatialHash)Activator.CreateInstance(typeof(PointSpatialHash<>).MakeGenericType(dataType)));
            var samplingHash = _samplingSpatialHash;
            //spatialHash.UpdateOrSet(position, id, def);
            samplingHash.UpdateOrSet(position, id, def);
        }
        public void ForgetGridData(OuterRef<IEntity> id, Type dataType)
        {
            //var spatialHash = (IPointSpatialHash)_spatialHashesByType.GetOrAdd(dataType, (type) => (IPointSpatialHash)Activator.CreateInstance(typeof(PointSpatialHash<>).MakeGenericType(dataType)));
            var samplingHash = _samplingSpatialHash;
            //spatialHash.Remove(id);
            samplingHash.Remove(id);
        }
        public void SetGridData<T>(Vector3 position, OuterRef<IEntity> id, T data) where T : IPointSpatialHashType
        {
            //var spatialHash = (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            var samplingHash = _samplingSpatialHash;
            //spatialHash.UpdateOrSet(position, id, data);
            samplingHash.UpdateOrSet(position, id, data.Def);
        }

        public void ForgetGridData<T>(OuterRef<IEntity> id) where T : IPointSpatialHashType
        {
            //var spatialHash = (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            var samplingHash = _samplingSpatialHash;
            //spatialHash.Remove(id);
            samplingHash.Remove(id);
        }
        public VisibilityDataSample GetGridData(OuterRef<IEntity> oRef)
        {
            IPointSpatialHash samplerHash = _samplingSpatialHash;
            //_spatialHashesByType.TryGetValue(type, out samplerHash);
            SpatialQueryResult samplerMeta;
            samplerHash.QueryId(oRef, out samplerMeta);
            if (samplerMeta.Ref.IsValid)
                return new VisibilityDataSample() { Def = samplerMeta.Def, Pos = samplerMeta.Pos, Ref = samplerMeta.Ref };
            return default(VisibilityDataSample);

        }

        public bool SampleDataForAnyAround<T>(OuterRef<IEntity> oRef, float customRadius, bool checkAlive)
        {
            SpatialQueryResult samplerMeta;
            // Get our pos (w/o async query to cluster)
            _samplingSpatialHash.QueryId(oRef, out samplerMeta, out var noNeedSamplerData);
            if (!samplerMeta.Ref.IsValid)
                return false;

            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            //var list = spatialHash.GetFreeQueryList();
            bool hasAny = spatialHash.Any(typeof(T), samplerMeta.Pos, customRadius, checkAlive);
            //spatialHash.ReturnFreeList(list);
            return hasAny;
        }
        public int SampleDataForAllAround(OuterRef<IEntity> oRef, Dictionary<OuterRef<IEntity>, VisibilityDataSample> alreadyKnownEntities, float customRadius, bool checkAlive, int maxQueryCount = int.MaxValue)
        {
            var samplerHash = _samplingSpatialHash;
            SpatialQueryResult samplerMeta;
            samplerHash.QueryId(oRef, out samplerMeta, out var noNeedSamplerData);
            if (!samplerMeta.Ref.IsValid)
                return 0;

            var spatialHash = _samplingSpatialHash;
            var list = spatialHash.GetFreeQueryList();
            var c = spatialHash.Query(samplerMeta.Pos, alreadyKnownEntities, customRadius, checkAlive, maxQueryCount);
            spatialHash.ReturnFreeList(list);
            return c;
        }
        public void SampleDataForAllAround(Vector3 position, Dictionary<OuterRef<IEntity>, VisibilityDataSample> alreadyKnownEntities, float customRadius, bool checkAlive)
        {
            var spatialHash = _samplingSpatialHash;
            var list = spatialHash.GetFreeQueryList();
            spatialHash.Query(position, alreadyKnownEntities, customRadius, checkAlive);
            spatialHash.ReturnFreeList(list);
        }
        public void SampleDataFor<T, K>(OuterRef<IEntity> oRef, ConcurrentDictionary<OuterRef<IEntity>, Vector3> alreadyKnownEntities, out Vector3 selfPosition) where T : IPointSpatialHashType where K : IPointSpatialHashType
        {
            var samplerHash = _samplingSpatialHash;// (PointSpatialHash<K>)_spatialHashesByType.GetOrAdd(typeof(K), (type) => new PointSpatialHash<K>());
            samplerHash.QueryId(oRef, out var samplerMeta);
            selfPosition = samplerMeta.Pos;
            if (!samplerMeta.Ref.IsValid)
            {
                //Logger.IfWarn()?.Message($"VISIBILITYUPDATE ref is invalid {oRef}").Write();
                return;
            }

            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            var lists = spatialHash.GetFreeLists();
            spatialHash.Query(samplerMeta.Pos, lists.Item1, lists.Item2, false);
            for (int i = 0; i < lists.Item2.Count; i++)
            {
                alreadyKnownEntities[lists.Item1[i].Ref] = lists.Item1[i].Pos;
            }

            spatialHash.ReturnFreeLists(lists);
        }
        public void SampleDataFor<T, K>(OuterRef<IEntity> oRef, Dictionary<OuterRef<IEntity>, Vector3> alreadyKnownEntities) where T : IPointSpatialHashType where K : IPointSpatialHashType
        {
            var samplerHash = _samplingSpatialHash;// (PointSpatialHash<K>)_spatialHashesByType.GetOrAdd(typeof(K), (type) => new PointSpatialHash<K>());
            SpatialQueryResult samplerMeta;
            samplerHash.QueryId(oRef, out samplerMeta);
            if (!samplerMeta.Ref.IsValid)
                return;
            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            var lists = spatialHash.GetFreeLists();
            spatialHash.Query(samplerMeta.Pos, lists.Item1, lists.Item2, false);
            for (int i = 0; i < lists.Item2.Count; i++)
            {
                alreadyKnownEntities[lists.Item1[i].Ref] = lists.Item1[i].Pos;
            }
            spatialHash.ReturnFreeLists(lists);
        }

        public bool HasAny(Type t, Vector3 pos, float radius, bool checkAlive)
        {
            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            return spatialHash.Any(t, pos, radius, checkAlive);
        }

        // --- Dbg -----------------------------------------------------------------------------------------------------------

        ///#PZ-7910 #Dbg:
        public void Dbg_GetGridData<T>(Vector3 pos, float radius, IList<string> outlist) where T : IPointSpatialHashType
        {
            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            spatialHash.DbgGetData(pos, radius, outlist);
        }
        public IList<KeyValuePair<OuterRef<IEntity>, PointSpatialHash<T>.Meta>> Dbg_GetObjsWithinRadius<T>(Vector3 pos, float radius) where T : IPointSpatialHashType
        {
            var spatialHash = _samplingSpatialHash;// (PointSpatialHash<T>)_spatialHashesByType.GetOrAdd(typeof(T), (type) => new PointSpatialHash<T>());
            return null;// spatialHash.Dbg_GetObjsWithinRadius(in pos, radius);
        }
        public IPointSpatialHash Dbg_GetPointHashByType(Type t)
        {
            return null;// _spatialHashesByType[t];
        }
    }

    public struct SimpleObjectData : IPointSpatialHashType
    {
        public int StaticCellSize => 100;

        public int StaticHeightSize => 300;

        public int StaticReplicationRadius => 100;

        public bool ShouldCheckForRadius => false;//никому особенно не важно, что ресурсы ты видишь в коробке, а не в радиусе, а нам небольшая оптимизация

        public IEntityObjectDef Def { get; set; }
    }

    public struct GridSampler<T>
    {
        public OuterRef<IEntity> Ref { get; set; }
        public Vector3 Size { get; set; }
    }
    public struct SpatialQueryResult
    {
        public OuterRef<IEntity> Ref;
        public Vector3 Pos;
        public IEntityObjectDef Def;
    }
    public interface IPointSpatialHashType
    {
        IEntityObjectDef Def { get; set; }
        bool ShouldCheckForRadius { get; }
        int StaticCellSize { get; }
        int StaticHeightSize { get; }
        int StaticReplicationRadius { get; }
    }
    public interface IPointSpatialHash
    {
        void QueryId(OuterRef<IEntity> ent, out SpatialQueryResult queryMeta);
        int Query(Vector3 center, Dictionary<OuterRef<IEntity>, VisibilityDataSample> alreadyKnownEntities, float customRadius, bool checkAlive, int maxCount = int.MaxValue);
        List<SpatialQueryResult> GetFreeQueryList();
        void ReturnFreeList(List<SpatialQueryResult> list);
        void UpdateOrSet(Vector3 position, OuterRef<IEntity> id, IEntityObjectDef def);
        void Remove(OuterRef<IEntity> id);
    }



    public class PointSpatialHash<T> : IPointSpatialHash where T : IPointSpatialHashType
    {
        static int _cellSize = 30;
        static int _heightCellSize = 50;
        static int _replicationDistance = 30;
        static bool _checkRadius = true;
        private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(30);

        private static readonly Pool<List<ReaderWriterLockSlim>> _lockListPool =
            new Pool<List<ReaderWriterLockSlim>>(500, Environment.ProcessorCount,
                () => new List<ReaderWriterLockSlim>(50), l => l.Clear());

        static PointSpatialHash()
        {
            var defT = default(T);
            _cellSize = defT.StaticCellSize;
            _heightCellSize = defT.StaticHeightSize;
            _replicationDistance = defT.StaticReplicationRadius;
            _checkRadius = defT.ShouldCheckForRadius;
        }
        IEntitiesRepository _repo;
        public PointSpatialHash(IEntitiesRepository repo, ILogger logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public class SpatialHashCell
        {
            public ConcurrentDictionary<OuterRef<IEntity>, Meta> CellsDictionary = new ConcurrentDictionary<OuterRef<IEntity>, Meta>();
            public ReaderWriterLockSlim CellLock = new ReaderWriterLockSlim();
            // public volatile bool Deleted = false;

            // @param `t` == null - means any type
            public bool HasAnyOfType([CanBeNull]Type t, bool checkAlive, IEntitiesRepository repo)
            {
                if (t == null)
                    return CellsDictionary.Count > 0;

                foreach (var meta in CellsDictionary)
                    if (meta.Value.Def != null && meta.Value.Def.GetType() == t &&
                        (!checkAlive || (repo.TryGetLockfree<IHasMortalClientBroadcast>(meta.Key, ReplicationLevel.ClientBroadcast)?.Mortal.IsAlive ?? true)))
                        return true;
                return false;
            }
        }

        //#Important: there is equivalent struct: `SpatialHash<T>.CellVector3`. So don't forget to copy your changes there!
        ///#PZ-9704: #Dbg:
        public
        struct CellVector3 : IEquatable<CellVector3>
        {
            private sealed class LockTakePriorityComparer : IComparer<CellVector3>
            {
                // we order cells in the same order as sampling iterates them
                // for (int x = cellsMin.X; x <= cellsMax.X; ++x)
                // for (int z = cellsMin.Z; z <= cellsMax.Z; ++z)
                // for (int y = cellsMin.Y; y <= cellsMax.Y; ++y)
                public int Compare(CellVector3 x, CellVector3 y)
                {
                    var xComparison = x.X.CompareTo(y.X);
                    if (xComparison != 0) return xComparison;
                    var zComparison = x.Z.CompareTo(y.Z);
                    if (zComparison != 0) return zComparison;
                    return x.Y.CompareTo(y.Y);
                }
            }

            public static IComparer<CellVector3> LockTakeComparer { get; } = new LockTakePriorityComparer();

            public int X;
            public int Y;
            public int Z;

            public CellVector3(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public CellVector3(Vector3 pos) : this()                                            //#Note: Addresses of grid cells:
            {                                                                                   //                   ^Z
                Func<float, int, int> worldToCellCoordinate = (float coord, int cellSize) =>    //   :       :       |       : 1 , 1 :
                    (coord < 0)                                                                 // __:_______:_______|_______:_______:
                        ? (int)(coord / cellSize) - 1                                           //   :       : -1, 0 | 0 , 0 :       :
                        : (int)(coord / cellSize);                                              // __:_______:_______._______:_______:____> X
                                                                                                //   :       : -1,-1 | 0 ,-1 :       :
                X = worldToCellCoordinate(pos.x, _cellSize);                                    // __:_______:_______|_______:_______:
                Z = worldToCellCoordinate(pos.z, _cellSize);                                    //   : -2,-2 :       |       :       :
                Y = worldToCellCoordinate(pos.y, _heightCellSize);
            }

            #region Operators
            public static CellVector3 operator *(CellVector3 v1, CellVector3 v2)
            {
                return new CellVector3(v1.X * v2.X,
                                        v1.Y * v2.Y,
                                        v1.Z * v2.Z);
            }

            // float operators:
            public static CellVector3 operator +(CellVector3 v, int i) => new CellVector3(v.X + i,
                                                                                           v.Y + i,
                                                                                           v.Z + i);
            public static CellVector3 operator -(CellVector3 v, int i) => v + (-i);

            public static explicit operator Vector3(CellVector3 cv) => new Vector3(cv.X, cv.Y, cv.Z);
            #endregion Operators

            public override bool Equals(object obj)
            {
                if (!(obj is CellVector3))
                {
                    return false;
                }

                var vector = (CellVector3)obj;
                return X == vector.X &&
                       Y == vector.Y &&
                       Z == vector.Z;
            }

            public bool Equals(CellVector3 other)
            {
                return X == other.X &&
                       Y == other.Y &&
                       Z == other.Z;
            }
            public bool EqualsByRef(ref CellVector3 other)
            {
                return X == other.X &&
                       Y == other.Y &&
                       Z == other.Z;
            }

            private const int offset13Minus1 = (1 << 13) - 1;
            private const int offset6Minus1 = (1 << 6) - 1;
            private const int positiveOffset = 4000;//8000 is somewhat around what you can fit into 13 bits, meaning that if you add it to the cell index it will always (except for far away corners) be positive number
            private const int positiveHeightOffset = 30;//same as before but there are only 6 bits
            private const int _13plus6 = 13 + 6;
            public override int GetHashCode()
            {
                //int is 32 bits, I take 13 for x, 13 for z and 6 for y/height and quantize into that
                var cellX = (uint)(X + positiveOffset);
                var cellY = (uint)(Y + positiveHeightOffset); //much bigger cells for height 
                var cellZ = (uint)(Z + positiveOffset);
                var first13bits = cellX & offset13Minus1;
                var second6bits = (cellY & offset6Minus1) << 13;
                var third13bits = (cellZ & offset13Minus1) << _13plus6;
                var hash = first13bits | second6bits | third13bits;
                return (int)hash;
            }

            public override string ToString() => $"X:{X}, Y:{Y}, Z:{Z}";
        }

        private class CellVector3EqualityComparer : IEqualityComparer<CellVector3>
        {
            public static CellVector3EqualityComparer Default = new CellVector3EqualityComparer();

            public bool Equals(CellVector3 x, CellVector3 y)
            {
                return x.EqualsByRef(ref y);
            }

            public int GetHashCode(CellVector3 obj)
            {
                return obj.GetHashCode();
            }
        }

        public ConcurrentDictionary<CellVector3, SpatialHashCell> _cells = new ConcurrentDictionary<CellVector3, SpatialHashCell>(CellVector3EqualityComparer.Default);
        SpatialHashCell GetCell(Vector3 pos) => GetCell(new CellVector3(pos));
        SpatialHashCell GetCell(CellVector3 cellAddres)
        {
            SpatialHashCell cell;
            _cells.TryGetValue(cellAddres, out cell);
            return cell;
        }

        private (Vector3, Vector3) GetCellMinMaxCorners(in CellVector3 cellAddress)
        {
            var cellSize = new CellVector3(_cellSize, _heightCellSize, _cellSize);
            return ((Vector3)(cellSize * cellAddress), (Vector3)(cellSize * (cellAddress + 1)));
        }

        private Vector3 GetCellFarthestCornerFrom(in CellVector3 cellAddress, in Vector3 fromPos)
        {
            var cellMinMaxCorners = GetCellMinMaxCorners(in cellAddress);
            return new Vector3((Abs(fromPos.x - cellMinMaxCorners.Item1.x) >
                                 Abs(fromPos.x - cellMinMaxCorners.Item2.x))
                                      ? cellMinMaxCorners.Item1.x
                                      : cellMinMaxCorners.Item2.x,
                                (Abs(fromPos.y - cellMinMaxCorners.Item1.y) >
                                 Abs(fromPos.y - cellMinMaxCorners.Item2.y))
                                      ? cellMinMaxCorners.Item1.y
                                      : cellMinMaxCorners.Item2.y,
                                (Abs(fromPos.z - cellMinMaxCorners.Item1.z) >
                                 Abs(fromPos.z - cellMinMaxCorners.Item2.z))
                                      ? cellMinMaxCorners.Item1.z
                                      : cellMinMaxCorners.Item2.z);
        }
        private Vector3 GetCellClosestCornerFrom(in CellVector3 cellAddress, in Vector3 fromPos)
        {
            var cellMinMaxCorners = GetCellMinMaxCorners(in cellAddress);
            return new Vector3((Abs(fromPos.x - cellMinMaxCorners.Item1.x) <
                                 Abs(fromPos.x - cellMinMaxCorners.Item2.x))
                                      ? cellMinMaxCorners.Item1.x
                                      : cellMinMaxCorners.Item2.x,
                                (Abs(fromPos.y - cellMinMaxCorners.Item1.y) <
                                 Abs(fromPos.y - cellMinMaxCorners.Item2.y))
                                      ? cellMinMaxCorners.Item1.y
                                      : cellMinMaxCorners.Item2.y,
                                (Abs(fromPos.z - cellMinMaxCorners.Item1.z) <
                                 Abs(fromPos.z - cellMinMaxCorners.Item2.z))
                                      ? cellMinMaxCorners.Item1.z
                                      : cellMinMaxCorners.Item2.z);
        }

        SpatialHashCell GetOrCreateCell(in Vector3 pos)
        {
            //int is 32 bits, I take 13 for x, 13 for y and 6 for height and quantize into that
            var cell = _cells.GetOrAdd(new CellVector3(pos), (h) => new SpatialHashCell());
            return cell;
        }
        public Tuple<List<SpatialQueryResult>, List<T>> GetFreeLists()
        {
            List<SpatialQueryResult> queryList = null;
            if (!_freeQueryLists.TryPop(out queryList))
                queryList = new List<SpatialQueryResult>();
            List<T> dataList;
            if (!_freeDataLists.TryPop(out dataList))
                dataList = new List<T>();
            queryList.Clear();
            dataList.Clear();
            return new Tuple<List<SpatialQueryResult>, List<T>>(queryList, dataList);
        }

        public void ReturnFreeLists(Tuple<List<SpatialQueryResult>, List<T>> lists)
        {
            _freeQueryLists.Push(lists.Item1);
            _freeDataLists.Push(lists.Item2);
        }
        ConcurrentStack<List<SpatialQueryResult>> _freeQueryLists = new ConcurrentStack<List<SpatialQueryResult>>();
        ConcurrentStack<List<T>> _freeDataLists = new ConcurrentStack<List<T>>();
        ///#PZ-7910:(8325): #Dbg:         
        public
        struct Meta
        {
            public T Data;
            public Vector3 Pos;
            public IEntityObjectDef Def;

            public override string ToString() => $"Pos:{Pos}, Data:{Data}.";
        }
        public ConcurrentDictionary<OuterRef<IEntity>, SpatialHashCell> _fromIdToCellMap = new ConcurrentDictionary<OuterRef<IEntity>, SpatialHashCell>();
        // public void UpdateOrSet(Vector3 position, OuterRef<IEntity> id, T data)
        // {
        //     Remove(id);
        //     var newCell = GetOrCreateCell(in position);
        //     newCell.CellsDictionary[id] = new Meta() { Data = data, Pos = position, Def = data.Def };
        //     _fromIdToCellMap[id] = newCell;
        // }

        public void UpdateOrSet(Vector3 position, OuterRef<IEntity> id, IEntityObjectDef def)
        {
            var data = default(T);

            _fromIdToCellMap.TryGetValue(id, out var oldCell);
            var newCell = GetOrCreateCell(in position);

            // we stay at the same cell
            if (oldCell == newCell || oldCell == null)
            {
                newCell.CellsDictionary[id] = new Meta() { Data = data, Pos = position, Def = def };
                if (oldCell == null)
                {
                    _fromIdToCellMap[id] = newCell;
                }
            }
            else
            {
                // old cell was deleted
                var oldCellWasDeleted = !oldCell.CellsDictionary.TryGetValue(id, out var oldPosition);
                if (oldCellWasDeleted)
                {
                    _logger.IfError()?.Message("WTF Couldn't find old position").Write();
                }
                else
                {
                    var oldCellVector = new CellVector3(oldPosition.Pos);
                    var newCellVector = new CellVector3(position);
                    var compareResult = CellVector3.LockTakeComparer.Compare(oldCellVector, newCellVector);

                    if (compareResult == 0)
                    {
                        _logger.IfError()?.Message("Somehow two cells have same pos {oldCell} {newCell}", oldPosition.Pos, position).Write();
                    }
                    else
                    {
                        SpatialHashCell firstLockCell;
                        SpatialHashCell secondLockCell;

                        if (compareResult == 1)
                        {
                            firstLockCell = newCell;
                            secondLockCell = oldCell;
                        }
                        // if (compareResult == -1)
                        else
                        {
                            firstLockCell = oldCell;
                            secondLockCell = newCell;
                        }

                        // we are moving from one cell to another 
                        // and we don't want the possibility of disappearing entity
                        // and this is why we take lock in the same priority as sampling does
                        CheckLock(firstLockCell.CellLock.TryEnterWriteLock(LockTimeout), "we are moving to another cell firstCell",
                            firstLockCell);
                        try
                        {
                            CheckLock(secondLockCell.CellLock.TryEnterWriteLock(LockTimeout), "we are moving to another secondCell",
                                secondLockCell);
                            try
                            {
                                oldCell.CellsDictionary.TryRemove(id, out _);
                                newCell.CellsDictionary[id] = new Meta() { Data = data, Pos = position, Def = def };
                                _fromIdToCellMap[id] = newCell;
                            }
                            finally
                            {
                                secondLockCell.CellLock.ExitWriteLock();
                            }
                        }
                        finally
                        {
                            firstLockCell.CellLock.ExitWriteLock();
                        }
                    }
                }
            }
        }

        private void CheckLock(bool lockTaken, string type, SpatialHashCell cell)
        {
            if (!lockTaken)
            {
                var positions = string.Join(", ", cell.CellsDictionary.Select(c => c.Value.Pos));
                throw new Exception($"Couldn't take lock {type} in {LockTimeout} positions {positions}");
            }
        }

        object _globalLock = new object();
        public void Remove(OuterRef<IEntity> id)
        {
            if (_fromIdToCellMap.TryRemove(id, out var cell))
            {
                cell.CellsDictionary.TryRemove(id, out _);
            }
        }

        public void QueryId(OuterRef<IEntity> ent, out SpatialQueryResult queryMeta)
        {
            QueryId(ent, out queryMeta, out _);
        }

        public void QueryId(OuterRef<IEntity> ent, out SpatialQueryResult queryMeta, out T queryData)
        {
            SpatialHashCell cell;
            Meta meta;
            queryMeta = default;
            queryData = default;

            bool querySet = false;
            int iterationsCount = 0;
            while (!querySet)
            {
                if (!_fromIdToCellMap.TryGetValue(ent, out cell))
                {
                    querySet = true;
                }
                else
                {
                    if (cell.CellsDictionary.TryGetValue(ent, out meta))
                    {
                        queryMeta = new SpatialQueryResult() { Pos = meta.Pos, Ref = ent, Def = meta.Def };
                        queryData = meta.Data;
                        querySet = true;
                    }
                    // entity currently in process of moving to another cell
                    else
                    {
                        iterationsCount++;
                        if (iterationsCount > 5)
                        {
                            _logger.IfWarn()?.Message("QueryId a lot of iterations").Write();
                        }

                        Thread.Yield();
                    }
                }
            }
        }
        public int Query(Vector3 center, List<SpatialQueryResult> queryMetaInto, List<T> queryDataInto, bool checkAlive, int maxQueryCount = int.MaxValue)
        {
            var objs = GetObjsWithinRadius(in center, _replicationDistance, _checkRadius, checkAlive);
            int count = 0;
            foreach (var kvp in objs)
            {
                if (count++ > maxQueryCount)
                    break;
                queryMetaInto.Add(new SpatialQueryResult() { Ref = kvp.Key, Pos = kvp.Value.Pos });
                queryDataInto.Add(kvp.Value.Data);
            }
            int countOfObjs = objs.Count;
            ReturnList(objs);
            return countOfObjs;
        }

        // @param `t` == null - means any type
        internal bool Any([CanBeNull]Type t, Vector3 pos, float radius, bool checkAlive)
        {
            var cellsMin = new CellVector3(pos - radius);
            var cellsMax = new CellVector3(pos + radius);

            var sqrR = Sqr(radius);

            var cellLocks = _lockListPool.Take();
            try
            {
                for (int x = cellsMin.X; x <= cellsMax.X; ++x)
                    for (int z = cellsMin.Z; z <= cellsMax.Z; ++z)
                        for (int y = cellsMin.Y; y <= cellsMax.Y; ++y)
                        {
                            var cellAddress = new CellVector3(x, y, z);
                            var cell = GetCell(cellAddress);
                            if (cell == null)
                                continue;

                            CheckLock(cell.CellLock.TryEnterReadLock(LockTimeout), "Any read lock", cell);
                            cellLocks.Add(cell.CellLock);
                            // #Variant #1 (fast, but false-positive)
                            //if (cell.Count > 0)
                            //    return true;
                            // // #Variant #2 (true, but slow)
                            // foreach (var meta in cell.Values)
                            // {
                            //     if ((meta.Pos - pos).sqrMagnitude <= sqrR)
                            //         return true;
                            // }
                            // #Variant #3 (true, & optimized)
                            //  #opt1(done): if the farthest corner of cell is within `radius` from `pos`, && cell.Count > 0 => return true;
                            //  #opt2(todo, if needed): if cell is totally out of sphere with `radius` & center in `pos`, don't check it (f.e. it's useful for "corner" cells)
                            //          Тут сложнее - возможна ситуация, все вершины и рёбра вне сферы, но есть погружение грани. Можно грубо например, сравнивать 
                            //      расстояние от `pos` до центра ячейки-прям.параллелепипеда с: `radius` + 1/2 главной диагонали ячейки.

                            //opt#1: if the farthest corner of cell is within `radius` from `pos`, all its content is within radius:
                            var farthestCellCorner = GetCellFarthestCornerFrom(cellAddress, pos);
                            if ((farthestCellCorner - pos).sqrMagnitude < sqrR
                                && cell.HasAnyOfType(t, checkAlive, _repo))
                                return true;

                            var minMaxCorners = GetCellMinMaxCorners(cellAddress);
                            //opt#2.a) if every `pos1 coordinate is out of cell coordinates - so closest cell point to sphere is corner
                            if (OutOfRangeFast(pos.x, minMaxCorners.Item1.x, minMaxCorners.Item2.x)
                                && OutOfRangeFast(pos.y, minMaxCorners.Item1.y, minMaxCorners.Item2.y)
                                && OutOfRangeFast(pos.z, minMaxCorners.Item1.z, minMaxCorners.Item2.z))
                            {
                                //if closest corner is out of sphere, then whole cell is out of it
                                var closestCellCorner = GetCellClosestCornerFrom(cellAddress, pos);
                                if ((closestCellCorner - pos).sqrMagnitude >= sqrR)
                                    continue;
                            }
                            //place other opt#2 "b"-case here. (2 cases left: b) closest cell point is on an edge 
                            //.. & c) is on a face(грань). This last case is actually impossible here ('cos of cell choosing logic))

                            foreach (var meta in cell.CellsDictionary)
                            {
                                if (t == null || (meta.Value.Def != null && meta.Value.Def.GetType() == t))
                                    if ((meta.Value.Pos - pos).sqrMagnitude <= sqrR &&
                                        (!checkAlive || (_repo.TryGetLockfree<IHasMortalClientBroadcast>(meta.Key, ReplicationLevel.ClientBroadcast)
                                                             ?.Mortal.IsAlive ?? true)))
                                        return true;
                            }
                        }
            }
            finally
            {
                foreach (var cellLock in cellLocks)
                {
                    cellLock.ExitReadLock();
                }

                _lockListPool.Return(cellLocks);
            }

            return false;
        }

        ConcurrentStack<List<KeyValuePair<OuterRef<IEntity>, Meta>>> _freeLists = new ConcurrentStack<List<KeyValuePair<OuterRef<IEntity>, Meta>>>();
        public void ReturnList(List<KeyValuePair<OuterRef<IEntity>, Meta>> list)
        {
            _freeLists.Push(list);
        }
        ///#PZ-7910:(8325): #Dbg:        
        public /*I*/List<KeyValuePair<OuterRef<IEntity>, Meta>> Dbg_GetObjsWithinRadius(in Vector3 pos, float radius, bool checkRadius, bool checkAlive)
            => GetObjsWithinRadius(in pos, radius, checkRadius, checkAlive);

        public List<KeyValuePair<OuterRef<IEntity>, Meta>> GetObjsWithinRadius(in Vector3 pos, float radius, bool checkRadius,
            bool checkAlive)
        {

            var cellsMin = new CellVector3(pos - radius);
            var cellsMax = new CellVector3(pos + radius);
            _freeLists.TryPop(out var result);
            if (result == null)
                result = new List<KeyValuePair<OuterRef<IEntity>, Meta>>();
            result.Clear();
            var sqrR = Sqr(radius);

            var cellLocks = _lockListPool.Take();
            try
            {
                for (int x = cellsMin.X; x <= cellsMax.X; ++x)
                    for (int z = cellsMin.Z; z <= cellsMax.Z; ++z)
                        for (int y = cellsMin.Y; y <= cellsMax.Y; ++y)
                        {
                            var cellAddress = new CellVector3(x, y, z);
                            var cell = GetCell(cellAddress);
                            if (cell == null)
                                continue;

                            CheckLock(cell.CellLock.TryEnterReadLock(LockTimeout), "GetObjsWithinRadius", cell);
                            cellLocks.Add(cell.CellLock);

                            // #Variant #1 (fast, but false-positive)
                            //if (cell.Count > 0)
                            //    return true;
                            // // #Variant #2 (true, but slow)
                            // if (!checkRadius)
                            //     result.AddRange(cell);
                            // else
                            // {
                            //     foreach (var kvp in cell)
                            //     {
                            //         if ((kvp.Value.Pos - pos).sqrMagnitude <= sqrR)
                            //             result.Add(kvp);
                            //     }
                            // }
                            // #Variant #3 (true, & optimized)
                            //  #opt1(done): if the farthest corner of cell is within `radius` from `pos`, && cell.Count > 0 => return true;
                            //  #opt2(todo, if needed): if cell is totally out of sphere with `radius` & center in `pos`, don't check it (f.e. it's useful for "corner" cells)
                            //          Тут сложнее - возможна ситуация, все вершины и рёбра вне сферы, но есть погружение грани. Можно грубо например, сравнивать 
                            //      расстояние от `pos` до центра ячейки-прям.параллелепипеда с: `radius` + 1/2 главной диагонали ячейки.
                            if (!checkRadius)
                            {
                                if (checkAlive)
                                {
                                    foreach (var cellE in cell.CellsDictionary)
                                    {
                                        if (_repo.TryGetLockfree<IHasMortalClientBroadcast>(cellE.Key, ReplicationLevel.ClientBroadcast)?.Mortal
                                                .IsAlive ?? true)
                                            result.Add(cellE);
                                    }
                                }
                                else
                                {
                                    foreach (var cellE in cell.CellsDictionary)
                                        result.Add(cellE);
                                }
                            }
                            else
                            {
                                //opt#1: if the farthest corner of cell is within `radius` from `pos`, all its content is within radius:
                                var farthestCellCorner = GetCellFarthestCornerFrom(in cellAddress, in pos);
                                if ((farthestCellCorner - pos).sqrMagnitude < sqrR)
                                {
                                    if (checkAlive)
                                    {
                                        foreach (var cellE in cell.CellsDictionary)
                                        {
                                            if (_repo.TryGetLockfree<IHasMortalClientBroadcast>(cellE.Key, ReplicationLevel.ClientBroadcast)?.Mortal
                                                    .IsAlive ?? true)
                                                result.Add(cellE);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var cellE in cell.CellsDictionary)
                                            result.Add(cellE);
                                    }
                                }
                                else
                                {
                                    var minMaxCorners = GetCellMinMaxCorners(in cellAddress);
                                    //opt#2.a) if every `pos1 coordinate is out of cell coordinates - so closest cell point to sphere is corner
                                    if (OutOfRangeFast(pos.x, minMaxCorners.Item1.x, minMaxCorners.Item2.x)
                                        && OutOfRangeFast(pos.y, minMaxCorners.Item1.y, minMaxCorners.Item2.y)
                                        && OutOfRangeFast(pos.z, minMaxCorners.Item1.z, minMaxCorners.Item2.z))
                                    {
                                        //if closest corner is out of sphere, then whole cell is out of it
                                        var closestCellCorner = GetCellClosestCornerFrom(in cellAddress, in pos);
                                        if ((closestCellCorner - pos).sqrMagnitude >= sqrR)
                                            continue;
                                    }

                                    //place other opt#2 "b"-case here. (2 cases left: b) closest cell point is on an edge 
                                    //.. & c) is on a face(грань). This last case is actually impossible here ('cos of cell choosing logic))
                                    if (checkAlive)
                                    {
                                        foreach (var cellE in cell.CellsDictionary)
                                        {
                                            if ((cellE.Value.Pos - pos).sqrMagnitude <= sqrR)
                                                if (_repo.TryGetLockfree<IHasMortalClientBroadcast>(cellE.Key, ReplicationLevel.ClientBroadcast)
                                                        ?.Mortal.IsAlive ?? true)
                                                    result.Add(cellE);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var cellE in cell.CellsDictionary)
                                            if ((cellE.Value.Pos - pos).sqrMagnitude <= sqrR)
                                                result.Add(cellE);
                                    }
                                }
                            }
                        }
            }
            finally
            {
                foreach (var cellLock in cellLocks)
                {
                    cellLock.ExitReadLock();
                }

                _lockListPool.Return(cellLocks);
            }

            return result;
        }


        public void DbgGetData(Vector3 pos, float radius, IList<string> outlist)
        {
            // var objs = GetObjsWithinRadius(pos, radius, true);
            // return objs.Select((x) => $"pos:{pos}). Data: {x.Value}.").ToList();

            var cellsMin = new CellVector3(pos - radius);
            var cellsMax = new CellVector3(pos + radius);

            outlist.Clear();

            var sqrR = Sqr(radius);
            for (int x = cellsMin.X; x <= cellsMax.X; ++x)
                for (int z = cellsMin.Z; z <= cellsMax.Z; ++z)
                    for (int y = cellsMin.Y; y <= cellsMax.Y; ++y)
                    {
                        var cell = GetCell(new CellVector3(x, y, z));
                        if (cell == null)
                            continue;

                        // Variant #1 (fast, but false-positive)
                        //if (cell.Count > 0)
                        //    return true;
                        // Variant #2 (true, but slow)
                        foreach (var meta in cell.CellsDictionary.Values)
                        {
                            if ((meta.Pos - pos).sqrMagnitude <= sqrR)
                                outlist.Add($"N=={cell.CellsDictionary.Count} - Cell(x:{x}, y:{y}, z:{z}) (by pos:{pos}). Datas: {string.Join(";.\n", cell.CellsDictionary.Values)} ");
                        }
                        // Variant #3 (true, & optimized) (description see in similar case in curr.file)
                    }
        }


        DateTime dbg_nextTimeRefreshCachedAllObjects;
        TimeSpan dbg_deltaTimeRefreshCachedAllObjects = TimeSpan.FromSeconds(5f);
        List<Tuple<CellVector3, Vector3, Guid>> _dbg_cachedGetAllObjects = new List<Tuple<CellVector3, Vector3, Guid>>();
        private ILogger _logger;

        public List<Tuple<CellVector3, Vector3, Guid>> Dbg_GetAllObjects()
        {
            if (DateTime.UtcNow > dbg_nextTimeRefreshCachedAllObjects)
            {
                _dbg_cachedGetAllObjects.Clear();
                dbg_nextTimeRefreshCachedAllObjects = DateTime.UtcNow + dbg_deltaTimeRefreshCachedAllObjects;
                foreach (var kvp in _cells)
                {
                    if (kvp.Value.CellsDictionary.Count == 0)
                        continue;

                    foreach (var kvp2 in kvp.Value.CellsDictionary)
                        _dbg_cachedGetAllObjects.Add(new Tuple<CellVector3, Vector3, Guid>(kvp.Key, kvp2.Value.Pos, kvp2.Key.Guid));
                }
            }
            return _dbg_cachedGetAllObjects;
        }

        //public void Query(Vector3 center, List<SpatialQueryResult> queryMetaInto, float customRadius)
        public int Query(Vector3 center, Dictionary<OuterRef<IEntity>, VisibilityDataSample> alreadyKnownEntities, float customRadius, bool checkAlive, int maxObjectsCount = int.MaxValue)
        {
            int count = 0;
            var objs = GetObjsWithinRadius(center, customRadius, true, checkAlive);
            foreach (var kvp in objs)
            {
                if (count++ > maxObjectsCount)
                    break;
                alreadyKnownEntities[kvp.Key] = new VisibilityDataSample() { Def = kvp.Value.Def, Pos = kvp.Value.Pos, Ref = kvp.Key };
            }
            var allCount = objs.Count;
            ReturnList(objs);
            return allCount;
        }
        public List<SpatialQueryResult> GetFreeQueryList()
        {
            List<SpatialQueryResult> queryList = null;
            if (!_freeQueryLists.TryPop(out queryList))
                queryList = new List<SpatialQueryResult>();
            queryList.Clear();
            return queryList;
        }

        public void ReturnFreeList(List<SpatialQueryResult> list)
        {
            _freeQueryLists.Push(list);
        }
    }

    public struct SpatialQuery
    {
        public Vector3 Center { get; set; }
        public Vector3 Size { get; set; }
    }

}
