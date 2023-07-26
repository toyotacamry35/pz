using System.Collections.Generic;
using System;
using System.Text;
using SharedCode.Utils;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static partial class Extensions
{
    /// <summary>
    /// Returns component T from transform that's located by componentRelativePath, 
    /// or returns null if not exists component/transform by path.
    /// If path componentRelativePath is empty, trying get component on parentTransform
    /// </summary>
    public static T GetComponentByPath<T>(this Transform parentTransform, string componentRelativePath)
        where T : Component
    {
        if (parentTransform == null)
            return null;

        Transform tr = (string.IsNullOrEmpty(componentRelativePath))
            ? parentTransform
            : parentTransform.Find(componentRelativePath);
        if (tr == null)
            return null;

        return tr.GetComponent<T>();
    }

    /// <summary>
    /// Returns components T from transform that's located by componentRelativePath (not from childrens of that transform), 
    /// or returns null if not exists components/transform by path.
    /// If path componentRelativePath is empty, trying get components from parentTransform
    /// </summary>
    public static List<T> GetComponentsByPath<T>(this Transform parentTransform, string componentRelativePath)
        where T : Component
    {
        if (parentTransform == null)
            return null;

        if (string.IsNullOrEmpty(componentRelativePath))
        {
            return new List<T>(parentTransform.GetComponents<T>());
        }

        T[] componentsInChildren = parentTransform.GetComponentsInChildren<T>();
        List<T> lstT = new List<T>();
        if (componentsInChildren != null)
        {
            foreach (T comp in componentsInChildren)
                if (comp.transform.FullName().EndsWith("/" + componentRelativePath))
                    lstT.Add(comp);
        }

        return lstT;
    }

    /// <summary>
    /// Returns full transform name (path from top transform)
    /// </summary>
    public static string FullName(this Transform tr, Transform root = null)
    {
        if (!tr)
            return string.Empty;
        var sb = StringBuildersPool.Get;
        GatherFullName(tr, root, sb);
        return sb.ToStringAndReturn();
    }

    private static void GatherFullName(this Transform tr, Transform root, StringBuilder sb)
    {
        if (!tr) 
            return;
        if (tr != root)
        {
            GatherFullName(tr.parent, root, sb);
            if (sb.Length != 0)
                sb.Append('/');
            sb.Append(tr.name);
        }
    }
    
    public static string ReversedPathTo(this Transform transform, Transform root)
    {
        if (!transform)
            return null;

        var sb = StringBuildersPool.Get;
        while (transform.parent && transform.parent != root)
        {
            sb.Append(transform.name).Append("/");
            transform = transform.parent;
        }

        return sb.ToStringAndReturn();
    }

    public static Transform FindChildRecursive(this Transform transform, string name)
    {
        if (transform.name == name)
        {
            return transform;
        }

        foreach (Transform child in transform)
        {
            var result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }

        return null;
    }

    public static void ForEachChild(this Transform transform, Action<Transform> action)
    {
        action?.Invoke(transform);

        foreach (Transform child in transform)
            ForEachChild(child, action);
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
            Object.Destroy(child.gameObject);
    }

    public static RectTransform GetRectTransform(this Transform transform)
    {
        if (transform == null)
            return null;

        return transform.GetComponent<RectTransform>();
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
            comp = go.AddComponent<T>();
        return comp;
    }
}