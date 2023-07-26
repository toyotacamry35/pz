using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Utility
{
    [Serializable]
    public class MatrixListPool
    {
        public List<List<Matrix4x4>> PoolList = new List<List<Matrix4x4>>();
        public int MaxCapasity;
        public int CreateCount;
        private List<Matrix4x4> _returnList;

        public MatrixListPool(int poolCount, int capasity)
        {
            CreateCount = 0;
            MaxCapasity = capasity;
            for (int i = 0; i <= poolCount - 1; i++)
                CreateList();
        }

        private void CreateList()
        {
            CreateCount++;
            List<Matrix4x4> newList = new List<Matrix4x4>(MaxCapasity);
            PoolList.Add(newList);
        }

        public List<Matrix4x4> GetList()
        {
            if (PoolList.Count == 0)
                CreateList();

            _returnList = PoolList[PoolList.Count - 1];
            PoolList.RemoveAt(PoolList.Count - 1);
            return _returnList;
        }

        public void ReturnList(List<Matrix4x4> list)
        {
            if (list.Capacity > MaxCapasity)
                MaxCapasity = list.Capacity;

            list.Clear();
            PoolList.Add(list);
        }

        public void Clear()
        {
            for (int i = 0; i <= PoolList.Count - 1; i++)
            {
                PoolList[i].Clear();
            }

            PoolList.Clear();
        }
    }

    [Serializable]
    public class ListPool<T>
    {
        public List<List<T>> PoolList = new List<List<T>>();
        public int MaxCapasity;
        public int CreateCount;
        private List<T> _returnList;

        public ListPool(int poolCount, int capasity)
        {
            CreateCount = 0;
            MaxCapasity = capasity;
            for (int i = 0; i <= poolCount - 1; i++)
                CreateList();
        }

        private void CreateList()
        {
            CreateCount++;
            List<T> newList = new List<T>(MaxCapasity);
            PoolList.Add(newList);
        }

        public List<T> GetList()
        {
            if (PoolList.Count == 0)
                CreateList();

            _returnList = PoolList[PoolList.Count - 1];
            PoolList.RemoveAt(PoolList.Count - 1);
            return _returnList;
        }

        public void ReturnList(List<T> list)
        {
            if (list.Capacity > MaxCapasity)
                MaxCapasity = list.Capacity;

            list.Clear();
            PoolList.Add(list);
        }

        public void Clear()
        {
            for (int i = 0; i <= PoolList.Count - 1; i++)
            {
                PoolList[i].Clear();
            }
            PoolList.Clear();
        }
    }

    [Serializable]
    public class ObjectPool<T> where T : new()
    {
        private readonly List<T> _available = new List<T>();
        private readonly List<T> _inUse = new List<T>();

        public T Get()
        {
            lock (_available)
            {
                if (_available.Count != 0)
                {
                    T obj = _available[0];
                    _inUse.Add(obj);
                    _available.RemoveAt(0);
                    return obj;
                }
                else
                {
                    T obj = new T();
                    _inUse.Add(obj);
                    return obj;
                }
            }
        }

        public void Release(T obj)
        {
            CleanUp(obj);

            lock (_available)
            {
                _available.Add(obj);
                _inUse.Remove(obj);
            }
        }

        private void CleanUp(T obj)
        {
        }
    }
}
