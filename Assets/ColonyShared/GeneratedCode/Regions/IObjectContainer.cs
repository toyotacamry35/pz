using SharedCode.Utils;
using System;
using System.Collections.Generic;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public interface IObjectContainer<T>
    {
        bool GetByPoint(out List<T> list, out HashSet<T> bigObjects, Vector3 coords);
        ICollection<T> GetAllObjectsInContainer();

        void Compact();
    }

    public class BoundlessContainer<T> : IObjectContainer<T>
    {
        public List<T> _boundlessRegions = new List<T>();

        public ICollection<T> GetAllObjectsInContainer() => _boundlessRegions;
        public ICollection<T> GetByPoint(Vector3 coords) => _boundlessRegions;
        static HashSet<T> _bigObjects = new HashSet<T>();
        public bool GetByPoint(out List<T> list, out HashSet<T> bigObjects, Vector3 coords)
        {
            list = _boundlessRegions;
            bigObjects = _bigObjects;
            return true;
        }

        public void Compact()
        {
            _boundlessRegions.TrimExcess();
        }
    }

    public class AABBHashedContainer<T> : IObjectContainer<T>
    {
        private SpatialHashTable<T> _aabb = new SpatialHashTable<T>();

        public bool GetByPoint(out List<T> list, out HashSet<T> bigObjects, Vector3 coords) => _aabb.GetObjectsByPointNonAlloc(out list, out bigObjects, coords);
        public void AddByPoint(Vector3 coords, T obj) => _aabb.AddObjectByPoint(coords, obj);
        public void AddByRect(Vector3 startCoords, Vector3 dimensions, T obj) => _aabb.AddObjectByRect(startCoords, dimensions, obj);
        public bool AddByRect(BoundingBox boundingBox, T obj)
        {
            if (boundingBox != null)
            {
                _aabb.AddObjectByRect(boundingBox.StartCoords, boundingBox.Dimensions, obj);
                return true;
            }
            return false;
        }
        public ICollection<T> GetAllObjectsInContainer() => _aabb.GetAllObjects();

        public void Compact()
        {
            _aabb.Compact();
        }
    }

    public class ConcurrentAABBHashedContainer<T> : IObjectContainer<T>, IDisposable
    {
        private ConcurrentSpatialHashTable<T> _aabb = new ConcurrentSpatialHashTable<T>();
        private bool disposed = false;
        static HashSet<T> _bigObjects = new HashSet<T>();
        public bool GetByPoint(out List<T> list, out HashSet<T> bigObjects, Vector3 coords)
        {
            bigObjects = _bigObjects;
            return _aabb.GetObjectsByPoint(coords, out list);
        }
        public void AddByPoint(Vector3 coords, T obj) => _aabb.AddObjectByPoint(coords, obj);
        public void AddByRect(Vector3 startCoords, Vector3 dimensions, T obj) => _aabb.AddObjectByRect(startCoords, dimensions, obj);
        public bool Remove(T obj) => _aabb.RemoveObject(obj);
        public bool AddByRect(BoundingBox boundingBox, T obj)
        {
            if (boundingBox != null)
                return _aabb.AddObjectByRect(boundingBox.StartCoords, boundingBox.Dimensions, obj);
            else
                return false;
        }
        public ICollection<T> GetAllObjectsInContainer() => _aabb.GetAllObjects();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            _aabb.Dispose();
            disposed = true;
        }

        public void Compact()
        {

        }
    }
}
