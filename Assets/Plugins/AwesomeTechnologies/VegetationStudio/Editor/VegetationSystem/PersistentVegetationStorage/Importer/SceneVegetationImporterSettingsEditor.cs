using System.Collections;
using System.Collections.Generic;
using AwesomeTechnologies.Common;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine;
using UnityEditor;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    [CustomEditor(typeof(SceneVegetationImporterSettings))]
    public class SceneVegetationImporterSettingsEditor : VegetationStudioBaseEditor
    {

        private SceneVegetationImporterSettings _sceneVegetationImporterSettings;

        [MenuItem("Assets/Create/Awesome Technologies/Vegetation Importers/SceneVegetationImporter Settings")]
        public static void CreateYourScriptableObject()
        {
            ScriptableObjectUtility2.CreateAndReturnAsset<SceneVegetationImporterSettings>();
        }

        public override void OnInspectorGUI()
        {
            _sceneVegetationImporterSettings = (SceneVegetationImporterSettings) target;
            base.OnInspectorGUI();

            DrawGUI(_sceneVegetationImporterSettings,null);
        }

        public static void DrawGUI(SceneVegetationImporterSettings sceneVegetationImporterSettings, VegetationPackage vegetationPackage)
        {
            GUILayout.BeginVertical("box");         
                    
            //EditorGUI.BeginChangeCheck();
            //sceneVegetationImporterSettings.RootObject = EditorGUILayout.ObjectField("Root object", sceneVegetationImporterSettings.RootObject, typeof(GameObject), true) as GameObject;
            //if (EditorGUI.EndChangeCheck())
            //{
            //    EditorUtility.SetDirty(sceneVegetationImporterSettings);
            //}

            EditorGUILayout.HelpBox("Select the root object for the import. With no object selected the entire scene will be searched.", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            if (GUILayout.Button("Add import rule"))
            {
                SceneVegetationImporterRule newSceneVegetationImporterRule =
                    new SceneVegetationImporterRule {SearchName = ""};
                sceneVegetationImporterSettings.SceneVegetationImporterRuleList.Add(newSceneVegetationImporterRule);
                EditorUtility.SetDirty(sceneVegetationImporterSettings);
            }
            GUILayout.EndVertical();


            int deleteIndex = -1;
            for (int i = 0; i <= sceneVegetationImporterSettings.SceneVegetationImporterRuleList.Count - 1; i++)
            {
                EditorGUI.BeginChangeCheck();
                DrawSceneVegetationImporterRule(sceneVegetationImporterSettings.SceneVegetationImporterRuleList[i],i, vegetationPackage,ref deleteIndex);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(sceneVegetationImporterSettings);
                }
            }
            if (deleteIndex > -1) sceneVegetationImporterSettings.SceneVegetationImporterRuleList.RemoveAt(deleteIndex);
        }


        public static void DrawSceneVegetationImporterRule(SceneVegetationImporterRule sceneVegetationImporterRule, int index, VegetationPackage vegetationPackage, ref int deleteIndex)
        {
           
            var labelStyle = new GUIStyle("Label") { fontStyle = FontStyle.Italic };

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Import rule " + (index +1), labelStyle);
            sceneVegetationImporterRule.SearchName =
                EditorGUILayout.TextField("Searh for: ", sceneVegetationImporterRule.SearchName);
            if (sceneVegetationImporterRule.SearchName.Trim() == "")
            {
                EditorGUILayout.HelpBox("An empty name field will return 0 results.", MessageType.Info);
            }

            sceneVegetationImporterRule.IgnoreCase = EditorGUILayout.Toggle("Ignore search string case", sceneVegetationImporterRule.IgnoreCase);

            sceneVegetationImporterRule.SceneVegetaionImporterNameSearchType = (SceneVegetaionImporterNameSearchType)EditorGUILayout.EnumPopup("Search type",
                sceneVegetationImporterRule.SceneVegetaionImporterNameSearchType);

            sceneVegetationImporterRule.UseTag =  EditorGUILayout.Toggle("Use tag", sceneVegetationImporterRule.UseTag);
            if (sceneVegetationImporterRule.UseTag)
            {
                sceneVegetationImporterRule.Tag = EditorFunctions.DrawTagDropdown(sceneVegetationImporterRule.Tag);
            }

            sceneVegetationImporterRule.DisableGameObject = EditorGUILayout.Toggle("Disable GameObject after import", sceneVegetationImporterRule.DisableGameObject);

            sceneVegetationImporterRule.UseLayer = EditorGUILayout.Toggle("Use layer", sceneVegetationImporterRule.UseLayer);
            if (sceneVegetationImporterRule.UseLayer)
            {
                sceneVegetationImporterRule.LayerMask = EditorGUILayout.LayerField("Select layer", sceneVegetationImporterRule.LayerMask);
            }

            EditorGUILayout.LabelField("Target vegetation item", labelStyle);

            sceneVegetationImporterRule.SceneVegetaionImporterTargetType = (SceneVegetaionImporterTargetType)EditorGUILayout.EnumPopup("Target type",
                sceneVegetationImporterRule.SceneVegetaionImporterTargetType);

            if (sceneVegetationImporterRule.SceneVegetaionImporterTargetType ==
                SceneVegetaionImporterTargetType.ExistingVegetationItem)
            {
                if (vegetationPackage)
                {
                    List<string> vegetationItemIdList =
                        VegetationPackageEditorTools.CreateVegetationInfoIdList(vegetationPackage);
                    VegetationPackageEditorTools.DrawVegetationItemSelector(vegetationPackage, vegetationItemIdList, 60,
                        ref sceneVegetationImporterRule.TargetVegetationItemID);
                }
                else
                {
                    EditorGUILayout.LabelField("Target vegetationSystemID: " + sceneVegetationImporterRule.TargetVegetationItemID, labelStyle);
                }
              
            }
            else
            {
                sceneVegetationImporterRule.TargetPrefab = EditorGUILayout.ObjectField("Target prefab", sceneVegetationImporterRule.TargetPrefab, typeof(GameObject), true) as GameObject;
                if (sceneVegetationImporterRule.TargetPrefab == null)
                {
                    EditorGUILayout.HelpBox("Target prefab must be set in order to import.", MessageType.Warning);

                }
                sceneVegetationImporterRule.VegetationType = (VegetationType)EditorGUILayout.EnumPopup("Vegetation type",
                    sceneVegetationImporterRule.VegetationType);
            }

            if (GUILayout.Button("Delete rule",GUILayout.Width(120)))
            {
                deleteIndex = index;
            }

            if (vegetationPackage)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Preview results", labelStyle);
                EditorGUILayout.LabelField("Selected GameObjects: " + sceneVegetationImporterRule.SelectedGameObjectList.Count, labelStyle);
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }
}
