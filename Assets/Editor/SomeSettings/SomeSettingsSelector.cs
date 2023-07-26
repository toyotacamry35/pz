using UnityEditor;
using UnityEngine;

public class SomeSettingsSelector : MonoBehaviour
{
    public static void Select(string assetFilter)
    {
        var assets = AssetDatabase.FindAssets(assetFilter);
        if (assets == null || assets.Length == 0)
        {
            Debug.LogError($"Not found asset by filter '{assetFilter}'!");
            return;
        }

        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]));
    }
}
