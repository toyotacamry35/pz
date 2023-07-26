using Assets.Src.ResourceSystem;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneRef))]
public class SceneRefDrawer : PropertyDrawer
{
    //=== Props ===============================================================

    private float SingleLineStep => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


    //=== Public ==============================================================

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var sceneAssetMetadata = property.FindPropertyRelative(nameof(SceneRef.SceneReference));
        var isOptionalProp = property.FindPropertyRelative(nameof(SceneRef.IsOptional));

        var currentPosition = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));

        var newSceneAssetMetadata = (JdbMetadata)
            EditorGUI.ObjectField(currentPosition, "Scene Ref", sceneAssetMetadata.objectReferenceValue, typeof(JdbMetadata), false);
        bool isEmpty = newSceneAssetMetadata == null;

        var sceneReference = newSceneAssetMetadata?.Get<SceneReferenceDef>();
        sceneAssetMetadata.objectReferenceValue = newSceneAssetMetadata;
        var scenePath = $"{sceneReference?.ScenePath ?? string.Empty}";

        var isSceneInBuildSettings = !isEmpty && IsSceneInBuildSettings(scenePath);
        EditorGUI.BeginDisabledGroup(isEmpty);

        currentPosition.yMin += SingleLineStep; //переходим на след. строку
        currentPosition.height = EditorGUIUtility.singleLineHeight; //высота строки
        currentPosition.xMin = position.xMin;
        currentPosition.width = position.width / 2;

        var customLabel = new GUIContent(label)
        {
            text = "Is optional",
        };
        isOptionalProp.boolValue = EditorGUI.Toggle(currentPosition, customLabel, isOptionalProp.boolValue);

        currentPosition.xMin += position.width / 2;
        currentPosition.width += position.width / 2;

        customLabel = new GUIContent(label)
        {
            text = "In build",
            tooltip = isSceneInBuildSettings ? "Scene is present in editor build settings" : "Scene isn't present in editor build settings"
        }; //Workaround: "label.tooltip = " is's wrong way, cause that tooltip shows on some other elements :)

        var newIsSceneInBuildSettings = EditorGUI.Toggle(currentPosition, customLabel, isSceneInBuildSettings);
        if (newIsSceneInBuildSettings != isSceneInBuildSettings)
        {
            SetBuildSettingsScenes(newIsSceneInBuildSettings, scenePath);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }


    //=== Private =============================================================

    private void SetPropsByAsset(SceneAsset sceneAsset, SerializedProperty sceneProp, SerializedProperty pathProp)
    {
        sceneProp.objectReferenceValue = sceneAsset;
        pathProp.stringValue = sceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(sceneAsset);
    }

    private void SetBuildSettingsScenes(bool toAddNorRemove, string scenePath)
    {
        if (string.IsNullOrEmpty(scenePath))
            return;

        var orgScenes = EditorBuildSettings.scenes;
        var isPresented = TryToGetSceneBuildSettingsIndex(scenePath, out int sceneIndex);
        if (toAddNorRemove)
        {
            if (isPresented)
            {
                orgScenes[sceneIndex].enabled = true;
                EditorBuildSettings.scenes = orgScenes; //orgScenes isn't reference of EditorBuildSettings.scenes - it's copy
                return;
            }

            var newScenes = new EditorBuildSettingsScene[orgScenes.Length + 1];
            Array.Copy(orgScenes, newScenes, orgScenes.Length);
            var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);
            newScenes[newScenes.Length - 1] = sceneToAdd;
            EditorBuildSettings.scenes = newScenes;
        }
        else
        {
            if (!isPresented)
                return;

            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>();
            for (int i = 0, len = orgScenes.Length; i < len; i++)
            {
                if (i != sceneIndex)
                    newScenes.Add(orgScenes[i]);
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }
    }

    private bool TryToGetSceneBuildSettingsIndex(string scenePath, out int sceneIndex)
    {
        sceneIndex = -1;
        var editorBuildSettingsScenes = EditorBuildSettings.scenes;
        if (editorBuildSettingsScenes == null || editorBuildSettingsScenes.Length == 0)
            return false;

        for (int i = 0, len = editorBuildSettingsScenes.Length; i < len; i++)
        {
            var editorBuildSettingsScene = editorBuildSettingsScenes[i];
            if (scenePath == editorBuildSettingsScene.path)
            {
                sceneIndex = i;
                return true;
            }
        }

        return false;
    }

    private static bool IsSceneInBuildSettings(string scenePath)
    {
        if (string.IsNullOrEmpty(scenePath))
            return false;

        var editorBuildSettingsScenes = EditorBuildSettings.scenes;
        if (editorBuildSettingsScenes == null || editorBuildSettingsScenes.Length == 0)
            return false;

        for (int i = 0, len = editorBuildSettingsScenes.Length; i < len; i++)
        {
            var editorBuildSettingsScene = editorBuildSettingsScenes[i];
            if (scenePath == editorBuildSettingsScene.path)
                return editorBuildSettingsScene.enabled;
        }

        return false;
    }
}