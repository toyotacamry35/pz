using SharedCode.Utils;
using System;
using UnityEngine;
namespace Assets.Src.BuildingSystem
{
    public class DelayAndDestroyVisualTimer
    {
        ElementData data = null;
        GameObject gameObject = null;

        private bool inProgress = false;
        private long timestamp = 0;
        private float delayTimer = 0.01f;
        private float timer = 0.02f;

        public bool IsInProgress() { return inProgress; }

        public float GetInterval()
        {
            return (float)((DateTime.UtcNow - UnixTimeHelper.DateTimeFromUnix(timestamp)).TotalSeconds) / timer;
        }

        public void Set(float _delayTime, float _destroyTime, ElementData _data, GameObject _gameObject)
        {
            timestamp = DateTime.UtcNow.ToUnix();
            delayTimer = Mathf.Max(0.01f, _delayTime);
            timer = Mathf.Max(0.02f, _delayTime + _destroyTime);
            data = _data;
            gameObject = _gameObject;
            inProgress = (gameObject != null);
        }

        public void Update()
        {
            if (inProgress)
            {
                if ((delayTimer > 0.0f) && (DateTime.UtcNow >= UnixTimeHelper.DateTimeFromUnix(timestamp).AddSeconds(delayTimer)))
                {
                    if (data != null)
                    {
                        VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
                    }
                    delayTimer = 0.0f;
                }
                if (DateTime.UtcNow >= UnixTimeHelper.DateTimeFromUnix(timestamp).AddSeconds(timer))
                {
                    if (data != null)
                    {
                        data.DestroyElementGameObject();
                        data = null;
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(gameObject);
                    }
                    gameObject = null;
                    inProgress = false;
                }
            }
        }
    }
}