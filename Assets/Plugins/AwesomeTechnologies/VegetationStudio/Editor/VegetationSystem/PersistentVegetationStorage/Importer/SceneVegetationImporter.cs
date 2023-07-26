using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{

#if PERSISTENT_VEGETATION
    public class SceneVegetationImporter : IVegetationImporter
    {
        private SceneVegetationImporterSettings _sceneVegetationImporterSettings;
        public string ImporterName
        {
            get
            {
                return "Scene vegetation importer";
            }
        }
        public PersistentVegetationStorage PersistentVegetationStorage { get; set; }
        public PersistentVegetationStoragePackage PersistentVegetationStoragePackage { get; set; }
        public VegetationPackage VegetationPackage { get; set; }

        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            _sceneVegetationImporterSettings = EditorGUILayout.ObjectField("Settings", _sceneVegetationImporterSettings, typeof(SceneVegetationImporterSettings), true) as SceneVegetationImporterSettings;
            if (EditorGUI.EndChangeCheck())
            {
                if (_sceneVegetationImporterSettings!= null && _sceneVegetationImporterSettings.AutomaticPreview) CreateObjectList();
            }
            EditorGUILayout.HelpBox("To create a new settings object right click in a project folder. Choose Create/AwesomeTechnologies/Vegetation Importers/SceneVegetationImporterSettings. Then drag and drop it here.", MessageType.Info);

            GUILayout.EndVertical();

            if (_sceneVegetationImporterSettings != null)
            {
                EditorGUI.BeginChangeCheck();
                SceneVegetationImporterSettingsEditor.DrawGUI(_sceneVegetationImporterSettings, VegetationPackage);
                if (EditorGUI.EndChangeCheck())
                {
                    if (_sceneVegetationImporterSettings.AutomaticPreview) CreateObjectList();
                }

                GUILayout.BeginVertical("box");

                _sceneVegetationImporterSettings.AutomaticPreview =
                    EditorGUILayout.Toggle("Automatic preview on change.", _sceneVegetationImporterSettings.AutomaticPreview);
                EditorGUILayout.HelpBox("For scenes with huge amount of GameObjects you can turn of the automatic preview as you edit rules.", MessageType.Info);

                if (!_sceneVegetationImporterSettings.AutomaticPreview)
                {
                    if (GUILayout.Button("Generate preview"))
                    {
                        CreateObjectList();
                    }
                }

                if (GUILayout.Button("Import"))
                {
                    CreateObjectList();
                    Import();
                }
                GUILayout.EndVertical();
            }
        }

        private void CreateObjectList()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
                             
            List<GameObject> possibleGameObjectList = allObjects.Where(go => go.activeInHierarchy).ToList();          

            for (int i = 0; i <= _sceneVegetationImporterSettings.SceneVegetationImporterRuleList.Count - 1; i++)
            {
                _sceneVegetationImporterSettings.SceneVegetationImporterRuleList[i].SelectedGameObjectList.Clear();
                for (int j = 0; j <= possibleGameObjectList.Count - 1; j++)
                {
                    if (ApplyRule(_sceneVegetationImporterSettings.SceneVegetationImporterRuleList[i],
                        possibleGameObjectList[j]))
                    {
                        _sceneVegetationImporterSettings.SceneVegetationImporterRuleList[i].SelectedGameObjectList.Add(possibleGameObjectList[j]);
                    }
                }               
            }
        }

        bool ApplyRule(SceneVegetationImporterRule sceneVegetationImporterRule, GameObject go)
        {
            if (sceneVegetationImporterRule.SearchName.Trim() == "") return false;

            string goName = go.name;
            string searchName = sceneVegetationImporterRule.SearchName;

            if (sceneVegetationImporterRule.IgnoreCase)
            {
                goName = goName.ToLower();
                searchName = searchName.ToLower();
            }

            switch (sceneVegetationImporterRule.SceneVegetaionImporterNameSearchType)
            {
                case SceneVegetaionImporterNameSearchType.Excact:
                    if (goName != searchName) return false;
                    break;
                case SceneVegetaionImporterNameSearchType.Contains:
                    if (!goName.Contains(searchName)) return false;
                    break;
                case SceneVegetaionImporterNameSearchType.StartsWith:
                    if (goName.Length < searchName.Length) return false;
                    if (goName.Substring(0, searchName.Length) != searchName) return false;
                    break;
            }

            if (sceneVegetationImporterRule.UseLayer)
            {
                if (go.layer != sceneVegetationImporterRule.LayerMask) return false;
            }

            if (sceneVegetationImporterRule.UseTag)
            {
                if (!go.CompareTag(sceneVegetationImporterRule.Tag)) return false;
            }

            return true;
        }        

        void Import()
        {
            for (int i = 0; i <= _sceneVegetationImporterSettings.SceneVegetationImporterRuleList.Count - 1; i++)
            {
                SceneVegetationImporterRule rule = _sceneVegetationImporterSettings.SceneVegetationImporterRuleList[i];
                string targetVegetationItemID = rule.TargetVegetationItemID;

                if (rule.SceneVegetaionImporterTargetType == SceneVegetaionImporterTargetType.Prefab)
                {
                    if (rule.TargetPrefab == null) continue;
                    targetVegetationItemID =
                        VegetationPackage.AddVegetationItem(rule.TargetPrefab, rule.VegetationType, false);
                }

                if (targetVegetationItemID == "") continue;

                for (int j = 0; j <= rule.SelectedGameObjectList.Count - 1; j++)
                {

                    Transform itemTransform = rule.SelectedGameObjectList[j].transform;
                    PersistentVegetationStorage.AddVegetationItemInstance(targetVegetationItemID, itemTransform.position,
                        itemTransform.lossyScale, itemTransform.rotation, true, 3,true);

                    if (rule.DisableGameObject)
                    {
                        rule.SelectedGameObjectList[j].SetActive(false);
                    }

                }
            }            
        }
    }
#endif
}
