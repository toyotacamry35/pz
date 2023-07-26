using SharedCode.Utils;
using System;
using System.Collections.Generic;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public class SpatialHashTable<T>
    {
        private HashSet<T> _bigObjects = new HashSet<T>();
        private Dictionary<CellVector3, List<T>> _hashToObject = new Dictionary<CellVector3, List<T>>();
        //uncomment if you find it useful
        //private Dictionary<T, List<CellVector3>> _objectHashes = new Dictionary<T, List<CellVector3>>();
        private HashSet<T> _objectHashes = new HashSet<T>();
        private readonly Vector3Int _cellSizes;

        public SpatialHashTable() { _cellSizes = new Vector3Int(30, 50, 30); }

        public SpatialHashTable(Vector3Int cellSizes) { _cellSizes = cellSizes; }

        public ICollection<T> GetAllObjects() => _objectHashes;//.Keys;

        /*public ICollection<T> GetObjectsByPoint(Vector3 coords)
        {
            CellVector3 hash = new CellVector3(coords, _cellSizes);
            if (!_hashToObject.TryGetValue(hash, out List<T> objects))
                return _bigObjects;
            else
                return objects;
        }*/
        static List<T> _emptyList = new List<T>(); 
        public bool GetObjectsByPointNonAlloc(out List<T> list, out HashSet<T> bigObjects, Vector3 coords)
        {
            CellVector3 hash = new CellVector3(coords, _cellSizes);
            bool hasAny =  _hashToObject.TryGetValue(hash, out list) || _bigObjects.Count > 0;
            bigObjects = _bigObjects;
            if (list == null)
                list = _emptyList;
            return hasAny;
        }

        public void AddObjectByPoint(Vector3 coords, T obj)
        {
            CellVector3 hash = new CellVector3(coords, _cellSizes);
            if (!_hashToObject.TryGetValue(hash, out List<T> objects))
                _hashToObject.Add(hash, objects = new List<T>(2));
            objects.Add(obj);

            _objectHashes.Add(obj);
            //if (!_objectHashes.TryGetValue(obj, out List<CellVector3> hashes))
            //    _objectHashes.Add(obj, hashes = new List<CellVector3>());
            //hashes.Add(hash);
        }

        public void AddObjectByRect(Vector3 startCoords, Vector3 dimensions, T obj)
        {
            var nearestToOriginCorner = new CellVector3(startCoords, _cellSizes);
            var farthestFromOriginCorner = new CellVector3(startCoords + dimensions, _cellSizes);

            //if (!_objectHashes.TryGetValue(obj, out List<CellVector3> hashes))
            //    _objectHashes.Add(obj, hashes = new List<CellVector3>());
            _objectHashes.Add(obj);
            var volumeCells = farthestFromOriginCorner - nearestToOriginCorner;
            var volume = volumeCells.X * volumeCells.Y * volumeCells.Z;
            var bigObjectsVolume = 15 * 15 * 15; //really large objects
            if (volume > bigObjectsVolume)
                _bigObjects.Add(obj);
            else
                for (var x = nearestToOriginCorner.X; x <= farthestFromOriginCorner.X; x++)
                    for (var y = nearestToOriginCorner.Y; y <= farthestFromOriginCorner.Y; y++)
                        for (var z = nearestToOriginCorner.Z; z <= farthestFromOriginCorner.Z; z++)
                        {
                            CellVector3 hash = new CellVector3(x, y, z);
                            if (!_hashToObject.TryGetValue(hash, out List<T> objects))
                                _hashToObject.Add(hash, objects = new List<T>(2));
                            objects.Add(obj);
                            //hashes.Add(hash);
                        }
        }

        public void AddObjectByKey(CellVector3 hash, T obj)
        {
            if (!_hashToObject.TryGetValue(hash, out List<T> objects))
                _hashToObject.Add(hash, objects = new List<T>(2));
            objects.Add(obj);

            _objectHashes.Add(obj);
            //if (!_objectHashes.TryGetValue(obj, out List<CellVector3> hashes))
            //    _objectHashes.Add(obj, hashes = new List<CellVector3>());
            //hashes.Add(hash);
        }

        public bool GetObjectsByKey(out List<T> list, out HashSet<T> bigObjects, CellVector3 cellVector)
        {
            bool hasAny = _hashToObject.TryGetValue(cellVector, out list) || _bigObjects.Count > 0 ;
            bigObjects = _bigObjects;
            return hasAny;
                
        }

        public void Compact()
        {
            _objectHashes.TrimExcess();
            foreach (var hashToObj in _hashToObject)
                hashToObj.Value.TrimExcess();
        }
    }

    [Serializable]
    public struct CellVector3 : IEquatable<CellVector3>
    {
        public int X;
        public int Y;
        public int Z;

        public CellVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static CellVector3 operator -(CellVector3 v1, CellVector3 v2)
        {
            return new CellVector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public CellVector3(Vector3 pos, Vector3Int cellSizes)                                                     //#Note: Addresses of grid cells:
        {                                                                                   //                   ^Z
            Func<float, int, int> worldToCellCoordinate = (float coord, int cellSize) =>    //   :       :       |       : 1 , 1 :
                (coord < 0)                                                                 // __:_______:_______|_______:_______:
                    ? (int)(coord / cellSize) - 1                                           //   :       : -1, 0 | 0 , 0 :       :
                    : (int)(coord / cellSize);                                              // __:_______:_______._______:_______:____> X
                                                                                            //   :       : -1,-1 | 0 ,-1 :       :
            X = worldToCellCoordinate(pos.x, cellSizes.x);                                  // __:_______:_______|_______:_______:
            Z = worldToCellCoordinate(pos.z, cellSizes.z);                                  //   : -2,-2 :       |       :       :
            Y = worldToCellCoordinate(pos.y, cellSizes.y);
        }

        public Vector3Int GetStartCoords(Vector3Int cellSizes)
        {
            var x = X * cellSizes.x;
            var y = Y * cellSizes.y;
            var z = Z * cellSizes.z;
            return new Vector3Int(x, y, z);
        }

        public Vector3Int GetCenter(Vector3Int cellSizes)
        {
            return GetStartCoords(cellSizes) + cellSizes / 2;
        }

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

        public override int GetHashCode()
        {
            //int is 32 bits, I take 13 for x, 13 for z and 6 for y/height and quantize into that
            var positiveOffset = 4000;//8000 is somewhat around what you can fit into 13 bits, meaning that if you add it to the cell index it will always (except for far away corners) be positive number
            var positiveHeightOffset = 30;//same as before but there are only 6 bits
            var cellX = (uint)(X + positiveOffset);
            var cellY = (uint)(Y + positiveHeightOffset); //much bigger cells for height 
            var cellZ = (uint)(Z + positiveOffset);
            var first13bits = cellX & ((1 << 13) - 1);
            var second6bits = (cellY & ((1 << 6) - 1)) << 13;
            var third13bits = (cellZ & ((1 << 13) - 1)) << (13 + 6);
            var hash = first13bits | second6bits | third13bits;
            return (int)hash;
        }

        public override string ToString() => $"X:{X}, Y:{Y}, Z:{Z}";
    }
}
