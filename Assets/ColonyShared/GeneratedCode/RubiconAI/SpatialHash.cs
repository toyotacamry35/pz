using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using Vector3 = SharedCode.Utils.Vector3;
using SharedCode.Utils;

namespace Assets.Src.NetworkedMovement
{
    public class SpatialHash<T>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const bool IsDebugEnabled = false;

        // Note: these dflt values could be overwritten at ctor:
        private readonly int _cellSize = 25;
        private readonly int _heightCellSize = 50;

        public interface IQueryAgent
        {
            //@return - should we early-break query loop
            bool Do(T obj);
        }

        //#Important: there is equivalent struct: `PointSpatialHash<T>.CellVector3`. So don't forget to copy your changes there!
        ///#PZ-9704: #Dbg:
        public
        struct CellVector3 : IEquatable<CellVector3>
        {
            public /*readonly*/ int X;
            public /*readonly*/ int Y;
            public /*readonly*/ int Z;

            public CellVector3(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public static CellVector3 operator +(CellVector3 a, CellVector3 b)
            {
                return new CellVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }
            public static CellVector3 operator -(CellVector3 a, CellVector3 b)
            {
                return new CellVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CellVector3))
                    return false;

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
                var first13Bits = cellX & offset13Minus1;
                var second6Bits = (cellY & offset6Minus1) << 13;
                var third13Bits = (cellZ & offset13Minus1) << _13plus6;
                var hash = first13Bits | second6Bits | third13Bits;
                return (int)hash;
            }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z})";
            }
        }
        private readonly Dictionary<CellVector3, HashSet<T>> _dict;
        private readonly Dictionary<T, CellVector3> _objects;

        private class CellVector3EqualityComparer: IEqualityComparer<CellVector3>
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

        public SpatialHash()
        {
            _dict = new Dictionary<CellVector3, HashSet<T>>(CellVector3EqualityComparer.Default);
            _objects = new Dictionary<T, CellVector3>();
        }

        public SpatialHash(int cellSize, int heightCellSize)
        {
            _cellSize = cellSize;
            _heightCellSize = heightCellSize;
            _dict = new Dictionary<CellVector3, HashSet<T>>(CellVector3EqualityComparer.Default);
            _objects = new Dictionary<T, CellVector3>();
        }
        
        public void Insert(Vector3 pos, T obj)
        {
            var cell = GetCellVector(pos);
            HashSet<T> hs;
            if (!_dict.TryGetValue(cell, out hs))
                _dict[cell] = hs = new HashSet<T>();
            hs.Add(obj);
            _objects[obj] = cell;
        }

        public void UpdatePosition(Vector3 pos, T obj)
        {
            //if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## SpatialHash.UpdatePosition");

            CellVector3 cell;
            if (_objects.TryGetValue(obj, out cell))
                _dict[cell].Remove(obj);
            Insert(pos, obj);

            //LocomotionProfiler3.EndSample();
        }
        ///#PZ-9704: #Dbg:
        CellVector3 _dbg_returnedRefCell;
        public ref CellVector3 UpdatePositionReturnsCellRef(Vector3 pos, T obj)
        {
            //if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## SpatialHash.UpdatePosition");

            if (_objects.TryGetValue(obj, out _dbg_returnedRefCell))
                _dict[_dbg_returnedRefCell].Remove(obj);
            Insert(pos, obj);

            //LocomotionProfiler3.EndSample();
            return ref _dbg_returnedRefCell;
        }

        ///#PZ-9704: #Dbg:
        public Dictionary<T, CellVector3> Dbg_DANGER_GetAllObjects()
        {
            if (!IsDebugEnabled)
                return null;

            return _objects;
        }


        public void Remove(T obj)
        {
            if (_objects.ContainsKey(obj))
            {
                _dict[_objects[obj]].Remove(obj);
                _objects.Remove(obj);
            }
        }

        // Has any objects in cells near pos. Factually always checks 1 enclosing + 7 closest cells: 2x2x2.
        public bool ContainsKey(Vector3 vector)
        {
            var cellHalfSize = _cellSize * 0.5f;
            var cellHalfHeight = _heightCellSize * 0.5f;
            {
                var cellKey1 = GetCellVector(vector + new Vector3(cellHalfSize, cellHalfHeight, cellHalfSize));
                if (Has(cellKey1))
                {
                    return true;
                }
                var cellKey2 = GetCellVector(vector + new Vector3(-cellHalfSize, cellHalfHeight, cellHalfSize));
                if (Has(cellKey2))
                {
                     return true;
                }
                var cellKey3 = GetCellVector(vector + new Vector3(cellHalfSize, cellHalfHeight, -cellHalfSize));
                if (Has(cellKey3))
                {
                     return true;
                }
                var cellKey4 = GetCellVector(vector + new Vector3(-cellHalfSize, cellHalfHeight, -cellHalfSize));
                if (Has(cellKey4))
                {
                     return true;
                }
            }
            {
                var cellKey1 = GetCellVector(vector + new Vector3(cellHalfSize, -cellHalfHeight, cellHalfSize));
                if (Has(cellKey1))
                {
                     return true;
                }
                var cellKey2 = GetCellVector(vector + new Vector3(-cellHalfSize, -cellHalfHeight, cellHalfSize));
                if (Has(cellKey2))
                {
                     return true;
                }
                var cellKey3 = GetCellVector(vector + new Vector3(cellHalfSize, -cellHalfHeight, -cellHalfSize));
                if (Has(cellKey3))
                {
                     return true;
                }
                var cellKey4 = GetCellVector(vector + new Vector3(-cellHalfSize, -cellHalfHeight, -cellHalfSize));
                if (Has(cellKey4))
                {
                    return true;
                }
            }

            return false;
        }

        //@Returns -1 if not found
        //.. Result is rounded to Cell center - to Cell center distance
        //.. Result is reduced by size of 1 cell at every dimension (to compensate situation, when 2 objs are very close, but in different adjacent cells & i.e.)
        public float GetClosestPopulatedCellDistance(Vector3 pos, Vector3 volumeHalfSize)
        {
            var sqr = GetClosestPopulatedCellDistanceSqr(pos, volumeHalfSize);

            //#Dbg:
            // if (Time.realtimeSinceStartup > dbg_nextTimeLog2)
            // {
            //     dbg_nextTimeLog2 = Time.realtimeSinceStartup + dbg_deltaTimeLog;
            //     DbgLog.Log($"Dist: {((sqr < 0) ? "far" : Sqrt(sqr).ToString())}");
            // }

            return (sqr < 0)
                ? sqr
                : Sqrt(sqr);
        }

        //@Returns -1 if not found
        //.. Result is rounded to Cell center - to Cell center distance
        //.. Result is reduced by size of 1 cell at every dimension (to compensate situation, when 2 objs are very close, but in different adjacent cells & i.e.)
        public float GetClosestPopulatedCellDistanceSqr(Vector3 pos, Vector3 volumeHalfSize)
        {
            var closestCellAddress = GetClosestPopulatedCellRelativeIndexAddress(pos, volumeHalfSize);

            //#Dbg:
            // if (Time.realtimeSinceStartup > dbg_nextTimeLog)
            // {
            //     dbg_nextTimeLog = Time.realtimeSinceStartup + dbg_deltaTimeLog;
            //     DbgLog.Log($"closestCellAddress: {closestCellAddress?.ToString() ?? "null"}");
            // }

            if (!closestCellAddress.HasValue)
                return -1f;

           // Cell center - to Cell center distance
           //.. , reduced by size of 1 cell to compensate situation, when 2 objs are very close, but in different adjacent cells & i.e.
           var distSqr = ((closestCellAddress.Value.X == 0) ? 0 : Sqr((Abs(closestCellAddress.Value.X)-1) * _cellSize))
                       + ((closestCellAddress.Value.Z == 0) ? 0 : Sqr((Abs(closestCellAddress.Value.Z)-1) * _cellSize)) 
                       + ((closestCellAddress.Value.Y == 0) ? 0 : Sqr((Abs(closestCellAddress.Value.Y)-1) * _heightCellSize));


            return distSqr;
        }

        private CellVector3? GetClosestPopulatedCellRelativeIndexAddress(Vector3 pos, Vector3 volumeHalfSize)
        {
            // 1. Get center cell for query: 
            var centerCell = GetCellVector(pos);

            // 2. Calc. query H & V border (square)radiuses:
            var querySizeHoriz = Max(volumeHalfSize.x, volumeHalfSize.z);
            // R measured in cells
            var queryHorizR = (querySizeHoriz > 0)
                ? CeilToInt(querySizeHoriz / _cellSize)
                : 0; // R == 0 means query only your cell
            var queryVertR = (volumeHalfSize.y > 0)
                ? CeilToInt(volumeHalfSize.y / _heightCellSize)
                : 0; // R == 0 means query only your cell

            if (queryVertR > queryHorizR)
                Logger.IfWarn()?.Message($"#Warn: queryVertR > queryHorizR ({queryVertR}>{queryHorizR}) is unhandled now.").Write();

            // 3. Do main work:
            // square R:
            int rV, rH;
            int lastRHForAdjacentLayers;
            for (rH = 0;  rH <= queryHorizR;  ++rH)
            {
                // 3.1) Query myCell layer 1st:  
                var result = GetClosestPopulatedCellIndexAddressAtContur(centerCell, rH);
                if (result.HasValue)
                    return result.Value - centerCell;
                                                                                                                            
                if (rH < 1)                                                                                                 
                    continue;
                //#Solution_variant_#2
                // // Опрашивать соседний слой только если инкрементировав псоледний раз rH, мы величиной rH * _cellSize перешагнули 
                // //.. очередную границу N*_heightCellSize. (где N - некий множитель).
                // //.. Это д/ того, чтобы обходимый объём был ближе к сфере, чем к 2-ной 4-гранной пирамиде (совмещенны основаниями).
                // //.. (при обходие след.слоя, используем rH = hvRatio - 1, т.е. опираемся на это же соотношение)
                // int prevHvRatio = (rH-1) * _cellSize / _heightCellSize;
                // int hvRatio = rH * _cellSize / _heightCellSize;
                // if (hvRatio == prevHvRatio)
                //     continue;
                // lastRHForAdjacentLayers = hvRatio - 1;
                // Debug.Assert(lastRHForAdjacentLayers >= 0); ///#PZ-7910(8716): #Dbg
                                                                                                                            //        ^ Y
                                                                                                                            //        |
                                                                                                                            // |_|_|_|_|_|_|_|
                                                                                                                            // |_|_|_|X|_|_| | - layer dH ==2
                // 3.2) Then query upper/lower layers (reducing query rH by 1 every next layer):                            // |_|_|X|X|X|_|_| - layer dH ==1
                var currLayerDeltaH = 1;                                                                                    // |_|X|X|X|X|X| | - myCell Layer
                var currLayerCenterCell = centerCell;                                                                       // |_|_|X|X|X|_|_| - layer dH ==-1
                //#TODO: it's tmp simple solution - just query upper/lower layers with rH reduced by "1" every next layer.  // |_|_|_|X|_|_|_|--> X,Z
                //#Solution_variant_#2 - Faster: обходимый объём ближе к сфере, чем к 2-ной 4-гранной пирамиде (совмещенны основаниями).
                // for (var currLayerRH = lastRHForAdjacentLayers;  (currLayerRH >= 0) && (currLayerDeltaH <= queryVertR);  )
                // {
                //     // 3.2.1) Check upper layer (+dH):
                //     currLayerCenterCell.Y += currLayerDeltaH *2 -1; // "x2 -1" to compensate prev. "-=" 
                //     result = GetClosestPopulatedCellIndexAddressAtContur(currLayerCenterCell, currLayerRH);
                //     if (result.HasValue)
                //         return result.Value - centerCell;
                // 
                //     // 3.2.2) Check lower layer (-dH):
                //     currLayerCenterCell.Y -= currLayerDeltaH *2; // x2 to compensate prev. "+=" 
                //     result = GetClosestPopulatedCellIndexAddressAtContur(currLayerCenterCell, currLayerRH);
                //     if (result.HasValue)
                //         return result.Value - centerCell;
                // 
                //     if (lastRHForAdjacentLayers < 1)
                //         break;
                //     int prevHvRatio2 = (lastRHForAdjacentLayers - 1) * _cellSize / _heightCellSize;
                //     int hvRatio2 = lastRHForAdjacentLayers * _cellSize / _heightCellSize;
                //     if (hvRatio2 == prevHvRatio2)
                //         break;
                //     currLayerRH = hvRatio2 - 1;
                //     Debug.Assert(currLayerRH >= 0); ///#PZ-7910(8716): #Dbg
                // 
                //     ++currLayerDeltaH;
                // }
                //#Solution_variant_#1 - Simple: обходимый объём - 2-ная 4-гранная пирамида (совмещенны основаниями)
                for (var currLayerRH = rH - 1;  (currLayerRH >= 0) && (currLayerDeltaH <= queryVertR);  --currLayerRH)
                {
                    // 3.2.1) Check upper layer (+dH):
                    currLayerCenterCell.Y += currLayerDeltaH *2 -1; // "x2 -1" to compensate prev. "-=" 
                    result = GetClosestPopulatedCellIndexAddressAtContur(currLayerCenterCell, currLayerRH);
                    if (result.HasValue)
                        return result.Value - centerCell;
                
                    // 3.2.2) Check lower layer (-dH):
                    currLayerCenterCell.Y -= currLayerDeltaH *2; // x2 to compensate prev. "+=" 
                    result = GetClosestPopulatedCellIndexAddressAtContur(currLayerCenterCell, currLayerRH);
                    if (result.HasValue)
                        return result.Value - centerCell;
                
                    ++currLayerDeltaH;
                }
            }

            //see ASCII art: inverse it in mind to see all unchecked, but should.
            if (queryVertR > queryHorizR)
                Logger.IfError()?.Message($"#Err: queryVertR > queryHorizR ({queryVertR}>{queryHorizR}) is unhandled now. Not all asked cells was checked!").Write(); 

            return null;
        }

        //@param `currConturN`: 0 means check only centerCell. "1" - check all 8 adjacent cells (in 2d). Contur is square.
        private CellVector3? GetClosestPopulatedCellIndexAddressAtContur(CellVector3 centerCell, int currConturN)
        {
            int x;
            ///int y = 0;
            int z = currConturN;

            var cell = centerCell + new CellVector3(0, /*y*/0, z);

            if (currConturN == 0)
                return (Has(cell)) ? cell : (CellVector3?)null;

            for (x = 0;  x <= currConturN;  ++x, ++cell.X) // #1/5) Go horizontally right from North to North-East corner
                if (Has(cell)) return cell; ///new CellVector3(x, y, z);
            --x; --cell.X;
            --z; --cell.Z;
            for ( ;  z >= -currConturN;  --z, --cell.Z)    // #2/5) Go vertically down from North-East corner to South-East corner
                if (Has(cell))  return cell; ///new CellVector3(x, y, z);
            ++z; ++cell.Z;
            --x; --cell.X;
            for ( ;  x >= -currConturN;  --x, --cell.X) // #3/5) Go horizontally left from South-East corner to South-West corner
                if (Has(cell))  return cell; ///new CellVector3(x, y, z);
            ++x; ++cell.X;
            ++z; ++cell.Z;
            for ( ;  z <= currConturN;  ++z, ++cell.Z)    // #4/5) Go vertically up from South-West corner to North-West corner
                if (Has(cell))  return cell; ///new CellVector3(x, y, z);
            --z; --cell.Z;
            ++x; ++cell.X;
            for ( ;  x < 0;  ++x, ++cell.X) // #5/5) Go horizontally right from North-West corner to North
                if (Has(cell))  return cell; ///new CellVector3(x, y, z);

            return null;
        }

        public bool Query(Vector3 pos, Vector3 volumeHalfSize, Func<T, bool> action)
        {

            var cellsMin = GetCellVector(pos - volumeHalfSize);
            var cellsMax = GetCellVector(pos + volumeHalfSize);
            for         (int x = cellsMin.X;  x <= cellsMax.X;  ++x)
                for     (int z = cellsMin.Z;  z <= cellsMax.Z;  ++z)
                    for (int y = cellsMin.Y;  y <= cellsMax.Y;  ++y)
                    { 
                        if (Do(new CellVector3(x, y, z), action))
                            return true;
                    }

            return false;
        }

        // @param `action` - returns true if query loop should be interrupted
        // @returns - true if query loop has been early-interrupted by `action` result
        public bool Query(Vector3 pos, Vector3 volumeHalfSize, IQueryAgent action)
        {

            var cellsMin = GetCellVector(pos - volumeHalfSize);
            var cellsMax = GetCellVector(pos + volumeHalfSize);
            for         (int x = cellsMin.X;  x <= cellsMax.X;  ++x)
                for     (int z = cellsMin.Z;  z <= cellsMax.Z;  ++z)
                    for (int y = cellsMin.Y;  y <= cellsMax.Y;  ++y)
                    { 
                        if (Do(new CellVector3(x, y, z), action))
                            return true;
                    }

            return false;
        }

        // @param `action` - returns true if query loop should be interrupted
        // @returns - true if query loop should be early-interrupted
        bool Do(CellVector3 cellKey, Func<T, bool> action)
        {
            HashSet<T> cell;
            if (!_dict.TryGetValue(cellKey, out cell))
                return false;
            foreach (var obj in cell)
                if (action(obj))
                    return true;
            return false;
        }

        // @param `action` - returns true if query loop should be interrupted
        // @returns - true if query loop should be early-interrupted
        bool Do(CellVector3 cellKey, IQueryAgent action)
        {
            HashSet<T> cell;
            if (!_dict.TryGetValue(cellKey, out cell))
                return false;
            foreach (var obj in cell)
                if (action.Do(obj))
                    return true;
            return false;
        }
        
        bool Has(CellVector3 cellKey)
        {
            HashSet<T> cell;
            if (!_dict.TryGetValue(cellKey, out cell))
                return false;
            return cell.Count > 0;
        }

        public void Clear()
        {
            var keys = _dict.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++)
                _dict[keys[i]].Clear();
            _objects.Clear();
        }

        public void Reset()
        {
            _dict.Clear();
            _objects.Clear();
        }

        ///#PZ-4552: Похоже, это старое решение . Пробуем без неё.
        // private const int BIG_ENOUGH_INT = 16 * 1024;
        // private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT + 0.0000;
        // 
        // private static int FastFloor(float f)
        // {
        //     return (int)(f + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
        // }

        ///#PZ-9704: #Dbg:
        public
        /*private*/ CellVector3 GetCellVector(Vector3 pos)
        {                                                                                   //#Note: Addresses of grid cells:
            Func<float, int, int> worldToCellCoordinate = (float coord, int cellSize) =>    //                   ^Z
                (coord < 0)                                                                 //   :       :       |       : 1 , 1 :
                    ? (int)(coord / cellSize) - 1                                           // __:_______:_______|_______:_______:
                    : (int)(coord / cellSize);                                              //   :       : -1, 0 | 0 , 0 :       :
                                                                                            // __:_______:_______._______:_______:____> X
            var x = worldToCellCoordinate(pos.x, _cellSize);                                //   :       : -1,-1 | 0 ,-1 :       :
            var z = worldToCellCoordinate(pos.z, _cellSize);                                // __:_______:_______|_______:_______:
            var y = worldToCellCoordinate(pos.y, _heightCellSize);                          //   : -2,-2 :       |       :       :
            return new CellVector3(x,y,z);
        }

        /// --- Debug ----------------------------------------------------

        ///#PZ-4552: #Dbg:
        public SharedCode.Utils.Vector3Int Dbg_GetCellByObj(T obj)
        {
            CellVector3 cell;
            return (_objects.TryGetValue(obj, out cell))
                ? new SharedCode.Utils.Vector3Int(cell.X, cell.Y, cell.Z)
                : SharedCode.Utils.Vector3Int.zero;
        }

        public SharedCode.Utils.Vector3Int Dbg_GetCellByPos(Vector3 pos)
        {
            var cellVec = GetCellVector(pos);
            return new SharedCode.Utils.Vector3Int(cellVec.X, cellVec.Y, cellVec.Z);
        }
    }
}
