using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public class ConcurrentSpatialHashTable<T> : IDisposable
    {
        private readonly Dictionary<int, Cell2DGrid>[] _columns;
        private readonly Dictionary<T, ObjectInfo> _objectHashes = new Dictionary<T, ObjectInfo>();

        private readonly ReaderWriterLockSlim[] _columnLocks;
        private readonly ReaderWriterLockSlim _objectHashLock = new ReaderWriterLockSlim();

        private readonly int _concurrencyLevelPower;
        private readonly int _concurrencyLevel;

        private readonly int _cellColumnWidth;
        private readonly Vector2Int _gridCellSizes;

        public ConcurrentSpatialHashTable(int concurrencyPower = 3) : this(new Vector3Int(30, 50, 30), concurrencyPower) { }

        public ConcurrentSpatialHashTable(Vector3Int cellSizes, int concurrencyPower = 3)
        {
            _cellColumnWidth = cellSizes.z;
            _gridCellSizes = new Vector2Int(cellSizes.x, cellSizes.y);
            if (concurrencyPower < 1)
                throw new ArgumentOutOfRangeException("concurrencyPower", "ConcurrentSpatialHashTable concurrencyPower must be positive");
            _concurrencyLevelPower = concurrencyPower;
            _concurrencyLevel = 2 ^ _concurrencyLevelPower;
            _columnLocks = new ReaderWriterLockSlim[_concurrencyLevel];
            _columns = new Dictionary<int, Cell2DGrid>[_concurrencyLevel];
            for (int i = 0; i < _concurrencyLevel; i++)
            {
                _columnLocks[i] = new ReaderWriterLockSlim();
                _columns[i] = new Dictionary<int, Cell2DGrid>();
            }
        }

        public void Dispose()
        {
            foreach (var columnLock in _columnLocks)
            {
                if (columnLock != null)
                    columnLock.Dispose();
            }
        }

        public static int GetColumnIndex(int columnWidth, float z) =>
                    z < 0
                        ? (int)(z / columnWidth) - 1
                        : (int)(z / columnWidth);

        public int GetRelativeColumnIndex(int columnIndex) => columnIndex % (2 ^ _concurrencyLevelPower);

        public ICollection<T> GetAllObjects()
        {
            List<T> keys;
            int locksAcquired = 0;
            AcquireAllReadLocks(ref locksAcquired);

            try
            {
                keys = new List<T>(_objectHashes.Keys);
            }
            finally
            {
                ReleaseReadLocks(0, locksAcquired);
            }
            return keys;
        }

        public bool GetObjectsByPoint(Vector3 coords, out List<T> objects)
        {
            bool result = false;

            int columnIndex = GetColumnIndex(_cellColumnWidth, coords.z);
            int relativeColumnIndex = GetRelativeColumnIndex(columnIndex);
            var columnLock = GetCorrespondingLock(relativeColumnIndex);
            var gridIndex = new CellVector2(coords, _gridCellSizes);

            columnLock.EnterReadLock();
            try
            {
                if (_columns[relativeColumnIndex].TryGetValue(columnIndex, out Cell2DGrid grid) && grid.ColumnToObject.TryGetValue(gridIndex, out List<T> objectsInHash))
                {
                    objects = new List<T>(objectsInHash);
                    result = true;
                }
                else
                    objects = default;
            }
            finally
            {
                columnLock.ExitReadLock();
            }

            return result;
        }

        public bool AddObjectByPoint(Vector3 coords, T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            bool result = false;
            bool taken = false;

            _objectHashLock.EnterReadLock();
            try
            {
                if (_objectHashes.ContainsKey(obj))
                    return false;
            }
            finally
            {
                _objectHashLock.ExitReadLock();
            }

            var columnIndex = GetColumnIndex(_cellColumnWidth, coords.z);
            var relativeColumnIndex = GetRelativeColumnIndex(columnIndex);
            var columnLock = GetCorrespondingLock(relativeColumnIndex);
            var gridIndex = new CellVector2(coords, _gridCellSizes);

            columnLock.EnterWriteLock();

            try
            {
                var lockedColumns = _columns[relativeColumnIndex];
                if (!lockedColumns.TryGetValue(columnIndex, out Cell2DGrid grid))
                    lockedColumns.Add(columnIndex, grid = new Cell2DGrid());

                if (grid.ColumnToObject.TryGetValue(gridIndex, out List<T> objects))
                    objects.Add(obj);
                else
                    grid.ColumnToObject.Add(gridIndex, new List<T>() { obj });


                _objectHashLock.EnterWriteLock();
                taken = true;

                var corner = new CellVector3(gridIndex.X, gridIndex.Y, relativeColumnIndex);
                _objectHashes.Add(obj, new ObjectInfo() { ObjInHashNearestCorner = corner, ObjInHashFarthestCorner = corner });
                result = true;
            }
            finally
            {
                if (taken)
                    _objectHashLock.ExitWriteLock();
                columnLock.ExitWriteLock();
            }

            return result;
        }

        public bool AddObjectByRect(Vector3 startCoords, Vector3 dimensions, T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            bool result = false;

            var nearestToOriginCorner = new CellVector3(startCoords, new Vector3Int(_gridCellSizes.x, _gridCellSizes.y, _cellColumnWidth));
            var farthestFromOriginCorner = new CellVector3(startCoords + dimensions, new Vector3Int(_gridCellSizes.x, _gridCellSizes.y, _cellColumnWidth));

            for (var z = nearestToOriginCorner.Z; z <= farthestFromOriginCorner.Z; z++)
            {
                var relativeColumnIndex = GetRelativeColumnIndex(z);
                var columnLock = GetCorrespondingLock(relativeColumnIndex);
                columnLock.EnterWriteLock();
                try
                {
                    var lockedColumns = _columns[relativeColumnIndex];
                    if (!lockedColumns.TryGetValue(z, out Cell2DGrid grid))
                        lockedColumns.Add(z, grid = new Cell2DGrid());

                    for (var x = nearestToOriginCorner.X; x <= farthestFromOriginCorner.X; x++)
                        for (var y = nearestToOriginCorner.Y; y <= farthestFromOriginCorner.Y; y++)
                        {
                            var gridIndex = new CellVector2(x, y);
                            if (grid.ColumnToObject.TryGetValue(gridIndex, out List<T> objects))
                                objects.Add(obj);
                            else
                                grid.ColumnToObject.Add(gridIndex, new List<T>() { obj });
                        }
                }
                finally
                {
                    columnLock.ExitWriteLock();
                }
            }

            _objectHashLock.EnterWriteLock();
            try
            {
                _objectHashes.Add(obj, new ObjectInfo() { ObjInHashNearestCorner = nearestToOriginCorner, ObjInHashFarthestCorner = farthestFromOriginCorner });
                result = true;
            }
            finally
            {
                _objectHashLock.ExitWriteLock();
            }

            return result;
        }

        public bool RemoveObject(T obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            bool result = false;
            bool taken = false;

            _objectHashLock.EnterUpgradeableReadLock();

            try
            {
                if (_objectHashes.TryGetValue(obj, out ObjectInfo objectInfo))
                {
                    _objectHashLock.EnterWriteLock();
                    taken = true;

                    if (_objectHashes.TryGetValue(obj, out ObjectInfo objectInfoWriteLocked))
                    {
                        _objectHashes.Remove(obj);

                        var nearestToOriginCorner = objectInfoWriteLocked.ObjInHashNearestCorner;
                        var farthestFromOriginCorner = objectInfoWriteLocked.ObjInHashFarthestCorner;

                        for (var z = nearestToOriginCorner.Z; z <= farthestFromOriginCorner.Z; z++)
                        {
                            var relativeColumnIndex = GetRelativeColumnIndex(z);
                            var columnLock = GetCorrespondingLock(relativeColumnIndex);
                            columnLock.EnterWriteLock();
                            try
                            {
                                var lockedColumns = _columns[relativeColumnIndex];
                                if (lockedColumns.TryGetValue(z, out Cell2DGrid grid))
                                {
                                    for (var x = nearestToOriginCorner.X; x <= farthestFromOriginCorner.X; x++)
                                        for (var y = nearestToOriginCorner.Y; y <= farthestFromOriginCorner.Y; y++)
                                        {
                                            var gridIndex = new CellVector2(x, y);
                                            var objects = grid.ColumnToObject[gridIndex];
                                            objects.Remove(obj);
                                            if (objects.Count == 0)
                                                grid.ColumnToObject.Remove(gridIndex);
                                        }
                                }
                            }
                            finally
                            {
                                columnLock.ExitWriteLock();
                            }
                        }
                        result = true;
                    }
                }
            }
            finally
            {
                if (taken)
                    _objectHashLock.ExitWriteLock();
                _objectHashLock.ExitUpgradeableReadLock();
            }

            return result;
        }

        public void Clear()
        {
            _objectHashLock.EnterWriteLock();
            try
            {
                _objectHashes.Clear();
            }
            finally
            {
                _objectHashLock.ExitWriteLock();
            }


            int writeLocksAcquired = 0;
            try
            {
                AcquireAllWriteLocks(ref writeLocksAcquired);
                foreach (var column in _columns)
                    column.Clear();
            }
            finally
            {
                ReleaseWriteLocks(0, writeLocksAcquired);
            }
        }

        #region LockHeplers

        private ReaderWriterLockSlim GetCorrespondingLock(int relativeColumnIndex) => _columnLocks[relativeColumnIndex];

        private void AcquireAllReadLocks(ref int locksAcquired)
        {
            AcquireReadLocks(0, _concurrencyLevel, ref locksAcquired);
        }

        private void AcquireReadLocks(int from, int to, ref int locksAcquired)
        {
            for (int i = from; i < to; i++)
            {
                bool taken = false;
                try
                {
                    _columnLocks[i].EnterReadLock();
                }
                finally
                {
                    if (taken)
                        locksAcquired++;
                }
            }
        }

        private void ReleaseReadLocks(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                _columnLocks[i].ExitReadLock();
            }
        }

        private void AcquireAllWriteLocks(ref int locksAcquired)
        {
            AcquireWriteLocks(0, _concurrencyLevel, ref locksAcquired);
        }

        private void AcquireWriteLocks(int from, int to, ref int locksAcquired)
        {
            for (int i = from; i < to; i++)
            {
                bool taken = false;
                try
                {
                    _columnLocks[i].EnterWriteLock();
                }
                finally
                {
                    if (taken)
                        locksAcquired++;
                }
            }
        }

        private void ReleaseWriteLocks(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                _columnLocks[i].ExitWriteLock();
            }
        }
        #endregion LockHelpers


        #region PrivateObjects

        internal struct CellVector2 : IEquatable<CellVector2>
        {
            public int X;
            public int Y;

            public CellVector2(int x, int y)
            {
                X = x;
                Y = y;
            }

            public CellVector2(Vector3 pos, Vector2Int cellSizes)                               //#Note: Addresses of grid cells:
            {                                                                                   //                   ^Z
                int worldToCellCoordinate(float coord, int cellSize) =>                         //   :       :       |       : 1 , 1 :
                    coord < 0                                                                   // __:_______:_______|_______:_______:
                        ? (int)(coord / cellSize) - 1                                           //   :       : -1, 0 | 0 , 0 :       :
                        : (int)(coord / cellSize);                                              // __:_______:_______._______:_______:____> X
                                                                                                //   :       : -1,-1 | 0 ,-1 :       :
                                                                                                // __:_______:_______|_______:_______:
                X = worldToCellCoordinate(pos.x, cellSizes.x);                                  //   : -2,-2 :       |       :       :
                Y = worldToCellCoordinate(pos.y, cellSizes.y);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CellVector2))
                    return false;

                var vector = (CellVector2)obj;
                return X == vector.X &&
                       Y == vector.Y;
            }

            public bool Equals(CellVector2 other)
            {
                return X == other.X &&
                       Y == other.Y;
            }

            public override int GetHashCode()
            {
                // 20 bits for z and 12 for y/height
                var positiveOffset = 500000;
                var positiveHeightOffset = 2000;
                var cellX = (uint)(X + positiveOffset);
                var cellY = (uint)(Y + positiveHeightOffset);
                var first20bits = cellX & (1 << 20) - 1;
                var second12bits = (cellY & (1 << 12) - 1) << 20;
                var hash = first20bits | second12bits;
                return (int)hash;
            }

            public override string ToString() => $"Y:{Y}, Z:{X}";
        }

        private class Cell2DGrid
        {
            public Dictionary<CellVector2, List<T>> ColumnToObject = new Dictionary<CellVector2, List<T>>();
        }

        private class ObjectInfo
        {
            public CellVector3 ObjInHashNearestCorner;
            public CellVector3 ObjInHashFarthestCorner;
        }

        #endregion PrivateObjects
    }
}