using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[UsedImplicitly]
public class BacktraceReference : EditorWindow
{
    private Component _theObject;
    private bool _specificComponent;


    [MenuItem("GameObject/What Objects Reference this?")]
    [UsedImplicitly]
    public static void Init()
    {
        GetWindow(typeof(BacktraceReference));
    }

    [UsedImplicitly]
    public void OnGUI()
    {

        _specificComponent = GUILayout.Toggle(_specificComponent, "Match a single specific component");
        if (_specificComponent)
        {
            _theObject = EditorGUILayout.ObjectField("Component referenced : ", _theObject, typeof(Component), true) as Component;
            if (_theObject == null)
                return;

            if (GUILayout.Button("Find Objects Referencing it"))
                FindObjectsReferencing(_theObject);
        }
        else if (GUILayout.Button("Find Objects Referencing Selected GameObjects"))
        {
            var obj = Selection.activeObject;
            var script = obj as MonoScript;
            if(script != null)
            {
                FindMonoBehaviour(script.GetClass());
            }

            var go = obj as GameObject;
            if (go == null)
                return;

            FindObjectsReferencing(go);

            foreach (Component c in go.GetComponents(typeof(Component)).Where(v => v != null))
            {
                FindObjectsReferencing(c);
            }
        }
    }

    private static void FindMonoBehaviour(Type type)
    {
        var allAssetGUIDs = AssetDatabase.FindAssets("");
        var q = from assetGuid in allAssetGUIDs
                let assetPath = AssetDatabase.GUIDToAssetPath(assetGuid)
                where AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(GameObject)
                let subassets = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                from subasset in subassets
                let obj = subasset as GameObject
                where obj != null
                let components = obj.GetComponents(type)
                where components.Any()
                select new { obj, assetPath };
        foreach (var obj in q)
        {
            Debug.Log("Ref: Object " + obj.obj.name + " at path " + obj.assetPath + " references source component " + type, obj.obj);
        }
    }

    private static void FindObjectsReferencing<T>(T mb) where T : Object
    {
        var allAssetGUIDs = AssetDatabase.FindAssets("");
        var q = from assetGuid in allAssetGUIDs
                let assetPath = AssetDatabase.GUIDToAssetPath(assetGuid)
                where AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(GameObject)
                let subassets = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                from subasset in subassets
                let obj = subasset as GameObject
                where obj != null
                where mb != obj
                where !(mb is Component) || (mb as Component).gameObject != obj
                let components = obj.GetComponents(typeof(Component))
                from component in components
                where component != null
                let fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                from field in fields
                where FieldReferencesComponent(component, field, mb)
                select new { component, assetPath };
        foreach (var obj in q)
        {
            Debug.Log("Ref: Component " + obj.component.GetType() + " from Object " + obj.component.name + "at path " + obj.assetPath + " references source component " + mb.GetType(), obj.component);
        }
    }

    private static bool FieldReferencesComponent<T>(Component obj, FieldInfo fieldInfo, T mb) where T : Object
    {
        if (fieldInfo.FieldType.IsArray)
        {
            var arr = fieldInfo.GetValue(obj) as Array;
            if (arr == null)
                return false;
            foreach (object elem in arr)
            {
                if (elem != null && mb != null && elem.GetType() == mb.GetType())
                {
                    var o = elem as T;
                    if (o == mb)
                        return true;
                }
            }
        }
        else
        {
            if (fieldInfo.FieldType == mb.GetType())
            {
                var o = fieldInfo.GetValue(obj) as T;
                if (o == mb)
                    return true;
            }
        }
        return false;
    }
}