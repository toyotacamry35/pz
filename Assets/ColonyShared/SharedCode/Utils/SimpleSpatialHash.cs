using Assets.ColonyShared.SharedCode.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SharedCode.Utils
{

    public class SimpleSpatialHash<T> : ISpatialHash where T : class, ISpatialHashed
    {
        private readonly int _cellSize;
        Dictionary<Vector3Int, HashSet<T>> _hash = new Dictionary<Vector3Int, HashSet<T>>();
        Dictionary<T, Vector3Int[]> _hashesPerObject = new Dictionary<T, Vector3Int[]>();
        Stack<HashSet<T>> _hashes = new Stack<HashSet<T>>();
        Stack<List<Vector3Int>> _lists = new Stack<List<Vector3Int>>();

        public SimpleSpatialHash(int cellSize)
        {
            _cellSize = cellSize;
        }

        public void Add(T obj)
        {
            List<Vector3Int> hashList = _lists.Any() ? _lists.Pop() : new List<Vector3Int>();
            hashList.Clear();
            obj.GetHash(this, hashList);
            DebugDrawer.DrawHash(hashList, _cellSize, 10f, Color.green);
            foreach (var cell in hashList)
            {
                HashSet<T> cellSet;
                if (!_hash.TryGetValue(cell, out cellSet))
                    _hash.Add(cell, cellSet = new HashSet<T>());
                cellSet.Add(obj);
            }
            _lists.Push(hashList);
        }

        public void Remove(T obj)
        {
            List<Vector3Int> hashList = _lists.Any() ? _lists.Pop() : new List<Vector3Int>();
            hashList.Clear();
            obj.GetHash(this, hashList);
            DebugDrawer.DrawHash(hashList, _cellSize, 10f, Color.red);
            foreach (var cell in hashList)
            {
                HashSet<T> cellSet;
                if (!_hash.TryGetValue(cell, out cellSet))
                    _hash.Add(cell, cellSet = new HashSet<T>());
                cellSet.Add(obj);
            }
            _lists.Push(hashList);
        }

        public IEnumerable<T> Query(Vector3 pos, Vector3 size)
        {
            return Query(new RectHash(pos, size));
        }

        public IEnumerable<T> Query(ISpatialHashed obj)
        {
            HashSet<T> _returnedObjects = _hashes.Any() ? _hashes.Pop() : new HashSet<T>();
            List<Vector3Int> hashList = _lists.Any() ? _lists.Pop() : new List<Vector3Int>();
            hashList.Clear();
            obj.GetHash(this, hashList);
            DebugDrawer.DrawHash(hashList, _cellSize, 10f, Color.yellow);
            foreach (var cell in hashList)
            {
                HashSet<T> objs;
                if (_hash.TryGetValue(cell, out objs))
                    foreach (var cellObj in objs)
                        if (_returnedObjects.Add(cellObj))
                            yield return cellObj;
            }
            _lists.Push(hashList);
            _returnedObjects.Clear();
            _hashes.Push(_returnedObjects);
        }


        int DimHash(float dimPos)
        {
            return (int)dimPos / _cellSize;
        }

        public Vector3Int PosHash(Vector3 pos)
        {
            return new Vector3Int(DimHash(pos.x), DimHash(pos.y), DimHash(pos.z));
        }
    }
}
