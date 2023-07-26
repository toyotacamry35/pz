using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniquePlayerPrefsTest))]
public class UniquePlayerPrefsTestEditor : Editor
{
    private const string BasePart = "UniquePlayerPrefsTest_";
    private const string IntValueName = BasePart + "Int";
    private const string FloatValueName = BasePart + "Float";
    private const string StringValueName = BasePart + "String";
    private const string LongValueName = BasePart + "Long";
    private const string DoubleValueName = BasePart + "Double";

    private GUIStyle _redBoldLabelStyle;
    private GUIStyle _boldTextStyle;

    public override void OnInspectorGUI()
    {
        if (_redBoldLabelStyle == null)
        {
            _redBoldLabelStyle = EditorGuiAdds.GetStyle(Color.red, true, EditorStyles.label);
            _boldTextStyle = EditorGuiAdds.GetStyle(Color.black, true, EditorStyles.label);
        }

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Saved values:", _boldTextStyle);

        if (!UniquePlayerPrefs.HasKey(IntValueName))
            EditorGUILayout.LabelField(IntValueName, "-");
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.IntField(IntValueName, UniquePlayerPrefs.GetInt(IntValueName));
            if (GUILayout.Button("Delete"))
                UniquePlayerPrefs.DeleteKey(IntValueName);
            EditorGUILayout.EndHorizontal();
        }

        if (!UniquePlayerPrefs.HasKey(FloatValueName))
            EditorGUILayout.LabelField(FloatValueName, "-");
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.FloatField(FloatValueName, UniquePlayerPrefs.GetFloat(FloatValueName));
            if (GUILayout.Button("Delete"))
                UniquePlayerPrefs.DeleteKey(FloatValueName);
            EditorGUILayout.EndHorizontal();
        }

        if (!UniquePlayerPrefs.HasKey(StringValueName))
            EditorGUILayout.LabelField(StringValueName, "-");
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(StringValueName, UniquePlayerPrefs.GetString(StringValueName));
            if (GUILayout.Button("Delete"))
                UniquePlayerPrefs.DeleteKey(StringValueName);
            EditorGUILayout.EndHorizontal();
        }

        if (!UniquePlayerPrefs.HasKey(LongValueName))
            EditorGUILayout.LabelField(LongValueName, "-");
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LongField(LongValueName, UniquePlayerPrefs.GetLong(LongValueName));
            if (GUILayout.Button("Delete"))
                UniquePlayerPrefs.DeleteKey(LongValueName);
            EditorGUILayout.EndHorizontal();
        }

        if (!UniquePlayerPrefs.HasKey(DoubleValueName))
            EditorGUILayout.LabelField(DoubleValueName, "-");
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.DoubleField(DoubleValueName, UniquePlayerPrefs.GetDouble(DoubleValueName));
            if (GUILayout.Button("Delete"))
                UniquePlayerPrefs.DeleteKey(DoubleValueName);
            EditorGUILayout.EndHorizontal();
        }

        var intSprop = GetSerializedProperty("Int");
        if (intSprop == null)
            return;

        var floatSprop = GetSerializedProperty("Float");
        if (floatSprop == null)
            return;

        var stringSprop = GetSerializedProperty("String");
        if (stringSprop == null)
            return;

        var longSprop = GetSerializedProperty("Long");
        if (longSprop == null)
            return;

        var doubleSprop = GetSerializedProperty("Double");
        if (doubleSprop == null)
            return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load"))
        {
            intSprop.intValue = UniquePlayerPrefs.GetInt(IntValueName);
            floatSprop.floatValue = UniquePlayerPrefs.GetFloat(FloatValueName);
            stringSprop.stringValue = UniquePlayerPrefs.GetString(StringValueName);
            longSprop.longValue = UniquePlayerPrefs.GetLong(LongValueName);
            doubleSprop.doubleValue = UniquePlayerPrefs.GetDouble(DoubleValueName);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        if (GUILayout.Button("Save"))
        {
            UniquePlayerPrefs.SetInt(IntValueName, intSprop.intValue);
            UniquePlayerPrefs.SetFloat(FloatValueName, floatSprop.floatValue);
            UniquePlayerPrefs.SetString(StringValueName, stringSprop.stringValue);
            UniquePlayerPrefs.SetLong(LongValueName, longSprop.longValue);
            UniquePlayerPrefs.SetDouble(DoubleValueName, doubleSprop.doubleValue);
        }
        EditorGUILayout.EndHorizontal();

    }


    //=== Private =============================================================

    private SerializedProperty GetSerializedProperty(string spropName)
    {
        var sprop = serializedObject.FindProperty(spropName);
        if (sprop == null)
            EditorGUILayout.LabelField(string.Format("Not found serialized property '{0}'", spropName), _redBoldLabelStyle);
        return sprop;
    }

}
