using System;
using UnityEngine;

public class AliveWatcher : MonoBehaviour
{
    public event Action<object> WatcherDestroy;
    private object _idObject = new object();

    public static object SubscribeOnDestroy(GameObject targetGameObject, Action<object> onDestroy)
    {
        if (targetGameObject.AssertIfNull(nameof(targetGameObject)) ||
            onDestroy.AssertIfNull(nameof(onDestroy)))
            return null;

        var aliveWatcher = targetGameObject.GetComponent<AliveWatcher>();
        if (aliveWatcher == null)
            aliveWatcher = targetGameObject.AddComponent<AliveWatcher>();

        aliveWatcher.WatcherDestroy += onDestroy;

        return aliveWatcher._idObject;
    }

    private void OnDestroy()
    {
        WatcherDestroy?.Invoke(_idObject);
        WatcherDestroy = null;
    }
}