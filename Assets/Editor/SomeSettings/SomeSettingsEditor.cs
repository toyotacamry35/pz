using UnityEditor;
using UnityEngine;

public abstract class SomeSettingsEditor : Editor
{
    private const int ButtonWdt = 200;

    private Editor _editor;
    private GUIStyle _redBoldLabelStyle;


    //=== Unity ===============================================================

    public override void OnInspectorGUI()
    {
        if (_redBoldLabelStyle == null)
            _redBoldLabelStyle = EditorGuiAdds.GetStyle(Color.red, true, EditorStyles.label);

        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(prop);
        EditorGUI.EndDisabledGroup();

        OnInspectorGUI_MainProps();
        OnInspectorGUI_OtherSettings();

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }


    //=== Protected ===========================================================

    protected virtual void OnInspectorGUI_MainProps()
    {
    }

    protected virtual void OnInspectorGUI_OtherSettings()
    {
    }

    protected void AssetsPopup<T>(string propertyName) where T : Object
    {
        var someSerializedProperty = GetSerializedProperty(propertyName);
        if (someSerializedProperty == null)
            return;

        var propertyTypeToString = typeof(T).NiceName();
        var targetFolderPathInfo = GetFolderPathInfo();

        if (!targetFolderPathInfo.IsExists)
        {
            EditorGUILayout.LabelField("Not found target folder", targetFolderPathInfo.RelativePath, _redBoldLabelStyle);
            return;
        }

        EditorGUILayout.ObjectField("Settings object", target, typeof(Object), false);
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Separator();
        if (GUILayout.Button("Create new " + propertyTypeToString, GUILayout.MaxWidth(ButtonWdt)))
        {
            AssetDatabase.CreateAsset(
                System.Activator.CreateInstance<T>(),
                AssetDatabase.GenerateUniqueAssetPath(targetFolderPathInfo.RelativePath + "/" + propertyTypeToString + ".asset"));
        }
        EditorGUILayout.Separator();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        string[] assetsGuids = AssetDatabase.FindAssets("t:" + propertyTypeToString, new[] { targetFolderPathInfo.RelativePath });

        if (assetsGuids == null || assetsGuids.Length == 0)
        {
            EditorGUILayout.LabelField("Not found instances of " + propertyTypeToString, _redBoldLabelStyle);
            EditorGUILayout.LabelField("Path: '" + targetFolderPathInfo.RelativePath + "'", _redBoldLabelStyle);
            return;
        }

        PathInfo[] assetPathInfos = new PathInfo[assetsGuids.Length];
        string[] popupNames = new string[assetsGuids.Length];

        var currentAsset = someSerializedProperty.objectReferenceValue;
        var currentAssetGuid = "";
        int popupIndex = -1;
        if (currentAsset != null)
            currentAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(currentAsset));

        for (int i = 0, len = assetsGuids.Length; i < len; i++)
        {
            assetPathInfos[i] = new PathInfo(AssetDatabase.GUIDToAssetPath(assetsGuids[i]), true, true, assetsGuids[i]);
            popupNames[i] = assetPathInfos[i].Filename;
            if (currentAssetGuid == assetsGuids[i])
                popupIndex = i;
        }

        if (popupIndex < 0)
            someSerializedProperty.objectReferenceValue = null;

        int newPopupIndex = EditorGUILayout.Popup("Current " + propertyTypeToString, popupIndex, popupNames);
        if (newPopupIndex != popupIndex)
            someSerializedProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath<T>(assetPathInfos[newPopupIndex].RelativePath);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"));

        CreateCachedEditor(someSerializedProperty.objectReferenceValue, null, ref _editor);
        if (_editor != null)
            _editor.OnInspectorGUI();
        EditorGUILayout.EndVertical();
    }


    //=== Private =============================================================

    private PathInfo GetFolderPathInfo()
    {
        string folderPath = AssetDatabase.GetAssetPath(target);
        int lastSlashPos = folderPath.LastIndexOf("/");
        folderPath = folderPath.Substring(0, lastSlashPos);
        var pathInfo = new PathInfo(folderPath, false, true);
        return pathInfo;
    }

    private SerializedProperty GetSerializedProperty(string spropName)
    {
        var sprop = serializedObject.FindProperty(spropName);
        if (sprop == null)
            EditorGUILayout.LabelField(string.Format("Not found serialized property '{0}'", spropName), _redBoldLabelStyle);
        return sprop;
    }
}
