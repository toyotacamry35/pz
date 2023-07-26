using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Src.Editor.Tools
{
    public static class LodSettingsReferenceInspector
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Preset _lastUsedPreset;
        
        
        [MenuItem("CONTEXT/LODGroup/Apply LODs Percentage")]
        static void ApplyLODsPercentage(MenuCommand command)
        {
            LODGroup lodGroup = command.context as LODGroup ?? throw new Exception("No LODGroup slected");
            int ctrlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<Preset>(_lastUsedPreset, false, "", ctrlID);
            EditorApplication.CallbackFunction part2 = null; 
            part2 = () => ApplyLODsPercentagePart2(lodGroup, part2);
            UnityEditor.EditorApplication.update += part2;
        }
        
               
        [MenuItem("CONTEXT/LODGroup/Setup Renderers")]
        static void FindRenderers(MenuCommand command)
        {
            LODGroup lodGroup = command.context as LODGroup ?? throw new Exception("No LODGroup slected");
            var renderersByLod = FindLodRenderers(lodGroup.gameObject);
            Undo.RegisterCompleteObjectUndo(lodGroup, "Setup Renderers");
            SetupLodRenderers(lodGroup, renderersByLod);
            UnityEditor.EditorUtility.SetDirty(lodGroup);
        }

        [MenuItem("CONTEXT/LODGroup/Setup Renderers With Percentage")]
        static void FindRenderersWithPercentage(MenuCommand command)
        {
            FindRenderers(command);
            ApplyLODsPercentage(command);
        }

        static void ApplyLODsPercentagePart2(LODGroup lodGroup, EditorApplication.CallbackFunction self)
        {
            if (EditorGUIUtility.GetObjectPickerObject() is Preset preset)
            {
                if (preset.GetPresetType() == new PresetType(lodGroup))
                {
                    UnityEditor.EditorApplication.update -= self;
                    _lastUsedPreset = preset;
                    ApplyLODsPercentage(lodGroup, preset);
                }
                else
                    Debug.Log($"{preset} is not a {nameof(lodGroup)}");
            }
        }
        
        public static void ApplyLODsPercentage(LODGroup lodGroup, Preset preset)
        {
            var lodGroupSerialized = new SerializedObject(lodGroup);
            ApplyLODsPercentage(lodGroupSerialized, preset);
        }
        

        public static void ApplyLODsPercentage(SerializedObject lodGroupSerialized, Preset preset)
        {
            var lodsProperty = lodGroupSerialized.FindProperty("m_LODs");
            var lodsArraySize = lodsProperty.FindPropertyRelative("Array.size").intValue;
            var modifications = preset.PropertyModifications;
            var arrayElementRegex = new Regex(@"m_LODs\.Array\.data\[(\d+)\].screenRelativeHeight");
            foreach (var modification in modifications)
            {
                var elementMatch = arrayElementRegex.Match(modification.propertyPath);
                if (elementMatch.Success)
                {
                    var elementIdxStr = elementMatch.Groups[1].Value;
                    var elementIdx = int.Parse(elementIdxStr, CultureInfo.InvariantCulture);
                    var elementValue = float.Parse(modification.value, CultureInfo.InvariantCulture);
                    if (elementIdx < lodsArraySize)
                        lodsProperty.GetArrayElementAtIndex(elementIdx).FindPropertyRelative("screenRelativeHeight").floatValue = elementValue;
                }
            }
            lodGroupSerialized.ApplyModifiedProperties();
        }
        
        public static void SetupLodRenderers(LODGroup lodGroup, List<List<Renderer>> renderer)
        {
            var oldLods = lodGroup.GetLODs();
            var newLods = new LOD[Mathf.Max(renderer.Count, oldLods.Length)];
            float prevScreenRelativeTransitionHeight = 1;
            for (var lodIdx = 0; lodIdx < newLods.Length; lodIdx++)
            {
                var lod = lodIdx < oldLods.Length ? oldLods[lodIdx] : new LOD();
                if (lod.screenRelativeTransitionHeight >= prevScreenRelativeTransitionHeight)
                    lod.screenRelativeTransitionHeight = prevScreenRelativeTransitionHeight / 2f;
                prevScreenRelativeTransitionHeight = lod.screenRelativeTransitionHeight;
                lod.renderers = (lodIdx < renderer.Count) ? renderer[lodIdx].ToArray() : new Renderer[0];
                newLods[lodIdx] = lod;
                Debug.Log($"LOD:{lodIdx} Renderers:[{string.Join(", ",newLods[lodIdx].renderers.Select(x => x.name))}] Height:{newLods[lodIdx].screenRelativeTransitionHeight}");
            }
            lodGroup.SetLODs(newLods);
        }

        public static List<List<Renderer>> FindLodRenderers(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            var renderersByLod = new List<List<Renderer>>();
            var regex = new Regex(@".*lod(\d+)", RegexOptions.IgnoreCase);
            foreach (var renderer in renderers)
            {
                var name = renderer.name;
                Debug.Log($"Renderer:{name}");

                var match = regex.Match(name);
                var lodIdx = 0;
                if (match.Success)
                {
                    var lodStr = match.Groups[1].Value; 
                    lodIdx = int.Parse(lodStr);
                }
                
                if (lodIdx < 0 && lodIdx > 10)
                {
                    Debug.LogError($"Bad LOD index {lodIdx} in {name}");
                }
                else
                {
                    while (lodIdx >= renderersByLod.Count)
                        renderersByLod.Add(new List<Renderer>());
                    renderersByLod[lodIdx].Add(renderer);
                    Debug.Log($"Renderer:{name} LOD:{lodIdx}");
                }
            }
            return renderersByLod;
        }
    }
}