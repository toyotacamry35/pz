using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Src.Effects.FX
{
    public class GameObjectPool : MonoBehaviour
    {
        public static Logger logger = LogManager.GetLogger("GameObjectPool");

        private static GameObjectPool _instance;

        public static GameObjectPool Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var go = new GameObject("GameObjectPool") {hideFlags = HideFlags.HideAndDontSave};
                _instance = go.AddComponent<GameObjectPool>();
                _instance.StartInstrumentedCoroutine(_instance.SpawnCoroutine());
                return _instance;
            }
        }

        private List<IGameObjectPoolElement> tickElements = new List<IGameObjectPoolElement>(50);

        private GameObjectSpawnQueue _spawnQueue = new GameObjectSpawnQueue();
        private Dictionary<GameObject, GameObjectElementContainer> _storage = new Dictionary<GameObject, GameObjectElementContainer>();

        private float SpawnWindowInMilliseconds => Mathf.Min(0.15f / _timer.AverageFPS, 0.005f);
        
        private const float CleanTimeInSeconds = 15f;
        private float _currentTimePassedAfterLastClean;

        private bool _isWindowClosed;
        private GameObjectPoolTimer _timer = new GameObjectPoolTimer();

        public void Return<T>(GameObject prefab, IGameObjectPoolElement poolElement) where T : IGameObjectPoolElement, new()
        {
            logger.IfDebug()?.Message($"Trying to return {poolElement.GameObject} for {prefab.name}").Write();

            if (poolElement == null || AssertGameObjectContainer<T>(prefab))
                return;

            _storage[prefab].Add(poolElement);

            UnsubscribeFromTick(poolElement);

            logger.IfDebug()?.Message($"Successfully returned {poolElement.GameObject} for {prefab.name}").Write();
        }

        public void DelayedSpawn<T>(GameObjectPoolSettings settings, GameObject prefab, Vector3 position, Quaternion rotation, IGameObjectPoolParams poolParams = null)
            where T : IGameObjectPoolElement, new()
        {
            logger.IfDebug()?.Message($"Plan for later spawning {prefab.name}").Write();

            if (!_storage.ContainsKey(prefab))
                _storage.Add(prefab, GameObjectElementContainer.ProduceContainer<T>(prefab));

            var objectForSpawn = new GameObjectForSpawn
                {settings = settings, startedTime = Time.realtimeSinceStartup, prefab = prefab, position = position, rotation = rotation, poolParams = poolParams};

            _spawnQueue.Plan(objectForSpawn);
        }

        private void Spawn(GameObjectForSpawn spawn)
        {
            logger.IfDebug()?.Message($"Trying to spawn {spawn.prefab.name}").Write();

            if (spawn.prefab == null || GameObjectPoolTimer.ElapsedTimeFromStartTime(spawn.startedTime) > spawn.settings.maxTimeInSeconds)
                return;

            var poolElement = _storage[spawn.prefab].GetFromPool(spawn.position, spawn.rotation) ?? _storage[spawn.prefab].Produce(false, spawn.position, spawn.rotation);

            if (poolElement == null || poolElement.GameObject == null)
                throw new InvalidOperationException("Produce null object from GameObjectPool");

            poolElement.Show(spawn.position, spawn.rotation, spawn.poolParams);

            SubscribeToTick(poolElement);
        }

        public GameObject Get<T>(GameObject prefab, Vector3 position, Quaternion rotation, IGameObjectPoolParams poolParams = null) where T : IGameObjectPoolElement, new()
        {
            logger.IfDebug()?.Message($"Trying to get {prefab.name}").Write();

            if (AssertGameObjectContainer<T>(prefab))
                return null;

            var poolElement = _storage[prefab].GetOrCreate<T>(position, rotation);
            if (poolElement == null || poolElement.GameObject == null)
                throw new InvalidOperationException("Returned null object from GameObjectPool");

            poolElement.Show(position, rotation, poolParams);

            SubscribeToTick(poolElement);

            return poolElement.GameObject;
        }

        private void Update()
        {
            for (int i = 0; i < tickElements.Count; i++)
            {
                tickElements[i].Tick();
            }
        }

        IEnumerator SpawnCoroutine()
        {
            while (true)
            {
                yield return null;

                OpenTimeWindow();
                SpawnMandatoryObjects();
                if (IsTimeExpired())
                    continue;
                
                SpawnNonMandatoryObjects();
                if (IsTimeExpired())
                    continue;
                
                CleanSpawnQueue();
                if (IsTimeExpired())
                    continue;
                
                CleanPool();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SpawnMandatoryObjects()
        {
            while (_spawnQueue.HasImmediateObjectsForSpawn())
            {
                var next = _spawnQueue.Next();
                Spawn(next);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SpawnNonMandatoryObjects()
        {
            while (!IsTimeExpired() && _spawnQueue.HasObjectToSpawn())
            {
                var next = _spawnQueue.Next();
                Spawn(next);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CleanSpawnQueue()
        {
            _spawnQueue.Tick();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CleanPool()
        {
            if(_currentTimePassedAfterLastClean < CleanTimeInSeconds)
                return;
            
            foreach (var pair in _storage)
            {
                if (IsTimeExpired())
                    break;

                if (pair.Value.Count > pair.Value.NeedCount)
                    RemoveUnused(pair);
                else
                    ProduceRequested(pair);
            }
            _currentTimePassedAfterLastClean = 0;
        }

        private void OpenTimeWindow()
        {
            _isWindowClosed = false;
            _timer.Reset();
            _currentTimePassedAfterLastClean += Time.deltaTime;
        }

        private void ProduceRequested(KeyValuePair<GameObject, GameObjectElementContainer> pair)
        {
            while (pair.Value.Count + 2 <= pair.Value.NeedCount)
            {
                if (IsTimeExpired())
                    break;
                pair.Value.Produce(true, Vector3.zero, Quaternion.identity);
            }
        }

        private void RemoveUnused(KeyValuePair<GameObject, GameObjectElementContainer> pair)
        {
            var currentCount = pair.Value.Count;
            for (var i = pair.Value.NeedCount; i < currentCount; i++)
            {
                if (IsTimeExpired())
                    break;
                pair.Value.Remove();
            }
        }

        private bool IsTimeExpired()
        {
            if (_isWindowClosed)
                return _isWindowClosed;

            _isWindowClosed = _timer.ElapsedMilliseconds() > SpawnWindowInMilliseconds;
            return _isWindowClosed;
        }

        public void SubscribeToTick(IGameObjectPoolElement poolElement)
        {
            tickElements.Add(poolElement);
        }

        public void UnsubscribeFromTick(IGameObjectPoolElement poolElement)
        {
            tickElements.Remove(poolElement);
        }

        private bool AssertGameObjectContainer<T>(GameObject prefab) where T : IGameObjectPoolElement, new()
        {
            if (prefab == null)
                return true;

            if (!_storage.ContainsKey(prefab))
                _storage.Add(prefab, GameObjectElementContainer.ProduceContainer<T>(prefab));
            return false;
        }
    }
}