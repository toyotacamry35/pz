using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypeRef))]
public class TypeRefDrawer : PropertyDrawer
{
    //=== Props ===============================================================

//    private float SingleLineStep
//    {
//        get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; }
//    }


    //=== Unity ===============================================================

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var typeAsMonoScriptSprop = property.FindPropertyRelative("TypeAsMonoScript");
        var typeAsStringSprop = property.FindPropertyRelative("TypeAsString");

        var typeAsMonoScript = (MonoScript) typeAsMonoScriptSprop.objectReferenceValue;
        var typeAsString = typeAsStringSprop.stringValue;
        var isEmpty = string.IsNullOrEmpty(typeAsString);

        //Inconsistency detecting
        if (!isEmpty && typeAsMonoScript == null)
        {
            var checkMonoScript = GetMonoScriptByTypeAsString(typeAsString);
            SetPropsByAsset(checkMonoScript, typeAsMonoScriptSprop, typeAsStringSprop);
            return;
        }

        var currentPosition = new Rect(position.position, new Vector2(position.width, EditorGUIUtility.singleLineHeight));

        var newMonoScript = (MonoScript) EditorGUI.ObjectField(
            currentPosition, GUIContent.none, typeAsMonoScriptSprop.objectReferenceValue, typeof(MonoScript), false);
        if (newMonoScript != typeAsMonoScript)
        {
            SetPropsByAsset(newMonoScript, typeAsMonoScriptSprop, typeAsStringSprop);
        }
//        currentPosition.yMin += SingleLineStep;
//        currentPosition.height = EditorGUIUtility.singleLineHeight;
//        EditorGUI.TextField(currentPosition, typeAsString);

        EditorGUI.EndProperty();
    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label) + SingleLineStep;
//    }


    //=== Private =============================================================

    private MonoScript GetMonoScriptByTypeAsString(string typeAsString)
    {
        var monoScripts = GameObject.FindObjectsOfType<MonoScript>();
        for (int i = 0, len = monoScripts.Length; i < len; i++)
        {
            var monoScript = monoScripts[i];
            if (typeAsString == monoScript.GetClass().ToString())
                return monoScript;
        }
        return null;
    }

    private void SetPropsByAsset(MonoScript monoScript, SerializedProperty monoScriptSprop, SerializedProperty stringSprop)
    {
        monoScriptSprop.objectReferenceValue = monoScript;
        if (monoScript == null)
        {
            stringSprop.stringValue = string.Empty;
        }
        else
        {
            var classType = monoScript.GetClass();
            if (classType == null)
            {
                monoScriptSprop.objectReferenceValue = null;
                stringSprop.stringValue = string.Empty;
            }
            else
            {
                stringSprop.stringValue = classType.ToString();
            }
        }
    }
}