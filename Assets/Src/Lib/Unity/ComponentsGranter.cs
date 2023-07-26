using System;
using UnityEngine;

public static class ComponentsGranter
{
    public static T GrantComponent<T>(Transform parentTransform, string gameObjectName = "filler") where T : Component
    {
        GameObject go = typeof(T) == typeof(Transform)
            ? new GameObject(gameObjectName)
            : new GameObject(gameObjectName, new Type[] {typeof(T)});
        go.transform.parent = parentTransform;
        return go.GetComponent<T>();
    }

    public static GameObject GrantGameObject(Transform parentTransform, string gameObjectName = "filler")
    {
        GameObject go = new GameObject(gameObjectName);
        go.transform.parent = parentTransform;
        return go;
    }

    public static void GrantNonNullableItems<T>(this T[] array, Transform parentTransform, string gameObjectName = "filler")
        where T : Component
    {
        for (int i = 0, len = array.Length; i < len; i++)
        {
            if (array[i] == null)
                array[i] = GrantComponent<T>(parentTransform, gameObjectName);
        }
    }
}