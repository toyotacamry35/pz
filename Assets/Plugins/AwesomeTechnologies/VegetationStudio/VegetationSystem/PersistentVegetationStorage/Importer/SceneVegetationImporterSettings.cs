using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    [System.Serializable]
    public enum SceneVegetaionImporterNameSearchType
    {
        Contains = 0,
        StartsWith = 1,
        Excact = 2
    }

    [System.Serializable]
    public enum SceneVegetaionImporterTargetType
    {
        ExistingVegetationItem = 0,
        Prefab = 1
    }

    [System.Serializable]
    public class SceneVegetationImporterRule
    {
        public string SearchName;
        public bool IgnoreCase = true;
        public bool DisableGameObject = true;
        public SceneVegetaionImporterNameSearchType SceneVegetaionImporterNameSearchType = SceneVegetaionImporterNameSearchType.StartsWith;
        public bool UseTag;
        public bool UseLayer;
        public LayerMask LayerMask;
        public string Tag;
        public string TargetVegetationItemID;
        public GameObject TargetPrefab;
        public VegetationType VegetationType = VegetationType.Tree;

        public List<GameObject> SelectedGameObjectList = new List<GameObject>();

        public SceneVegetaionImporterTargetType SceneVegetaionImporterTargetType =
            SceneVegetaionImporterTargetType.ExistingVegetationItem;
    }

 

    public class SceneVegetationImporterSettings : ScriptableObject
    {
        public bool AutomaticPreview = true;
        public List<SceneVegetationImporterRule> SceneVegetationImporterRuleList = new List<SceneVegetationImporterRule>();
    }
}
