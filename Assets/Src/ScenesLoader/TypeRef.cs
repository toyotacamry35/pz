using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TypeRef
{
#if UNITY_EDITOR
    public MonoScript TypeAsMonoScript;
#endif

    public string TypeAsString;

    public bool IsEmpty => string.IsNullOrEmpty(TypeAsString);

    public Type AsType => IsEmpty ? null : Type.GetType(TypeAsString);
}