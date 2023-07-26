using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Src.Effects.FX
{
    public static class GameObjectElementFactory
    {
        public static IGameObjectPoolElement ProducePoolElement<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : IGameObjectPoolElement, new()
        {
            var go = Object.Instantiate(prefab, position, rotation);
#if DEBUG
            go.name = $"{prefab.name}_{Guid.NewGuid()}";
#endif
            var poolElement = new T();
            poolElement.Init(go, prefab);
            GameObjectPool.logger.IfDebug()?.Message($"Fill pool with {poolElement.GameObject} for {prefab.name}").Write();
            return poolElement;
        }
    }

    public class GameObjectElementContainer
    {
        private GameObject _prefab;
        private Func<GameObject, Vector3, Quaternion, IGameObjectPoolElement> _produceFunction;

        private List<IGameObjectPoolElement> _pooledElement = new List<IGameObjectPoolElement>();
        public int Count => _pooledElement.Count;

        public int GetCount { get; set; } = 0; //количество элементов забаранных из очереди
        public int NeedCount { get; set; } = 0; //предполагаемое количество элементов, которые могут пригодится

        public static GameObjectElementContainer ProduceContainer<T>(GameObject prefab) where T : IGameObjectPoolElement, new()
        {
            return new GameObjectElementContainer(prefab, GameObjectElementFactory.ProducePoolElement<T>);
        }

        public GameObjectElementContainer(GameObject prefab, Func<GameObject, Vector3, Quaternion, IGameObjectPoolElement> produceFunction)
        {
            _prefab = prefab;
            _produceFunction = produceFunction;
        }

        public void Add(IGameObjectPoolElement element)
        {
            _pooledElement.Add(element);
        }

        public IGameObjectPoolElement GetOrCreate<T>(Vector3 position, Quaternion rotation) where T : IGameObjectPoolElement, new()
        {
            GameObjectPool.logger.IfDebug()?.Message($"Trying to get pool {_prefab.name}").Write();
            GetCount++;

            var pooled = GetFromPool(position, rotation);
            return pooled ?? CreateAndReturn<T>(position, rotation);
        }

        public IGameObjectPoolElement GetFromPool(Vector3 position, Quaternion rotation)
        {
            if (_pooledElement.Count == 0)
                return null;

            var element = _pooledElement[0];
            _pooledElement.RemoveAt(0);

            if (element == null || element.GameObject == null)
                return null;

            GameObjectPool.logger.IfDebug()?.Message($"Get element {element.GameObject} for {_prefab.name} from pool").Write();

            return element;
        }

        public IGameObjectPoolElement Produce(bool addToQueue, Vector3 position, Quaternion rotation)
        {
            GameObjectPool.logger.IfDebug()?.Message($"Trying to produce new {_prefab.name}").Write();
            if (_prefab == null)
                return null;

            _prefab.SetActive(false);

            var poolElement = _produceFunction.Invoke(_prefab, position, rotation);

            GameObjectPool.logger.IfDebug()?.Message($"Produced new {poolElement.GameObject} for {_prefab.name} and is added to queue.").Write();

            if (addToQueue)
                Add(poolElement);

            return poolElement;
        }

        public void Remove()
        {
            if (Count < 1) return;

            var lastElement = _pooledElement[Count - 1];
            _pooledElement.RemoveAt(Count - 1);
            GameObjectPool.logger.IfDebug()?.Message($"Remove {lastElement.GameObject} for {_prefab.name}").Write();
            Object.Destroy(lastElement.GameObject);
        }

        private IGameObjectPoolElement CreateAndReturn<T>(Vector3 position, Quaternion rotation) where T : IGameObjectPoolElement, new()
        {
            GameObjectPool.logger.IfDebug()?.Message($"Trying to create new {_prefab.name}").Write();
            return _prefab == null ? null : GameObjectElementFactory.ProducePoolElement<T>(_prefab, position, rotation);
        }
    }
}