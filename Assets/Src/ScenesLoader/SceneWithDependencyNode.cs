using System;

[Serializable]
public class SceneWithDependencyNode
{
    public SceneRef SceneRef;

    public bool IsWrong => SceneRef == null ||
                           SceneRef.IsEmpty;

    public string ScenePath => SceneRef == null ? "" : SceneRef.ScenePath;
}