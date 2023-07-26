using Assets.Src.ResourceSystem;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneRef
{
    public JdbMetadata SceneReference;

    public string ScenePath => $"{SceneReference?.Get<SceneReferenceDef>()?.ScenePath ?? string.Empty}";

    public bool IsOptional;

    public bool IsEmpty => string.IsNullOrEmpty(SceneReference?.Get<SceneReferenceDef>()?.ScenePath);

    public override string ToString() => $"[{typeof(SceneRef).Name}: {nameof(SceneReferenceDef.ScenePath)}='{ScenePath}'{(IsOptional ? " optional" : "")}]";
}
