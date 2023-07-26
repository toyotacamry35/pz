using UnityEngine;
using UnityEditor;

 
public class ReplaceGameObjects : ScriptableWizard
{
    public bool copyValues = true;
    public GameObject NewType;
    public GameObject[] OldObjects;
 
    [MenuItem("Level Design/Replace Object")]
 
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace Object", typeof(ReplaceGameObjects), "Replace");
    }
 
    void OnWizardCreate()
    {
 
        foreach (GameObject go in OldObjects)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(NewType);
            newObject.transform.position = go.transform.position;
            newObject.transform.rotation = go.transform.rotation;
			newObject.transform.localScale = go.transform.localScale;
            newObject.transform.parent = go.transform.parent;
 
            DestroyImmediate(go);
 
        }
 
    }
}