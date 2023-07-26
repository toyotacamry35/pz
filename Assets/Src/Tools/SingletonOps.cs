using System;
using Uins;
using UnityEngine;

public static class SingletonOps
{
    public static T TrySetInstance<T>(T newInstance, T instanceHolder) where T : class
    {
        if (!newInstance.AssertIfNull(nameof(newInstance)))
        {
            if (instanceHolder == null)
            {
            }
            else
            {
                UI.Logger.Error($"<{typeof(T)}> Is excess instance! New: '{InstanceToString<T, MonoBehaviour>(newInstance, MonoBehaviourToString)}'\n" +
                                $" already exists: '{InstanceToString<T, MonoBehaviour>(instanceHolder, MonoBehaviourToString)}'");
            }

            instanceHolder = newInstance;
        }

        return instanceHolder;
    }

    public static string InstanceToString<T, U>(T instance, Func<U, string> toStringFunc) where T : class where U : class
    {
        return instance is U instanceAsU ? toStringFunc(instanceAsU) : instance?.ToString();
    }

    public static string MonoBehaviourToString(MonoBehaviour mb)
    {
        if (mb == null)
            return null;

        return mb.transform.FullName();
    }
}