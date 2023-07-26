using System;

namespace Assets.Src.Effects.FX
{
    public class GameObjectSpawnQueue
    {
        
        private PriorityQueue<GameObjectForSpawn> _waitingForSpawn = new PriorityQueue<GameObjectForSpawn>();

        public void Plan(GameObjectForSpawn spawn)
        {
            _waitingForSpawn.Enqueue(spawn.settings.priority, spawn);
        }

        public bool HasImmediateObjectsForSpawn()
        {
            return _waitingForSpawn.IsMaximumPriority();
        }

        public bool HasObjectToSpawn()
        {
            return _waitingForSpawn.Count != 0;
        }

        public GameObjectForSpawn Next()
        {
            return _waitingForSpawn.Dequeue();
        }

        public void Tick()
        {
            _waitingForSpawn.ClearOrUpdatePriority(
                go =>
                {
                    if (GameObjectPoolTimer.ElapsedTimeFromStartTime(go.startedTime) < go.settings.maxTimeInSeconds)
                        return PriorityQueueOperationVariation.Nothing;
                    return go.settings.isItMandatory ? PriorityQueueOperationVariation.GiveMaximumPriority : PriorityQueueOperationVariation.Delete;
                });
            if(_waitingForSpawn.Count > 0)
                _waitingForSpawn.ShowCurrentQueue();
        }
    }
}