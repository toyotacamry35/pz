using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.External.MapMagicInterface
{
    [CustomEditor(typeof(MapMagicInfiniteTerrain))]
    public class MapMagicInfiniteTerrainEditor : VegetationStudioBaseEditor
    {
        private MapMagicInfiniteTerrain _mapMagicInfiniteTerrain;
        public override void OnInspectorGUI()
        {
            HelpTopic = "map-magic-infinite-terrain";
            _mapMagicInfiniteTerrain = (MapMagicInfiniteTerrain)target;
            base.OnInspectorGUI();
#if MAPMAGIC
            EditorGUILayout.HelpBox("Map Magic installed", MessageType.Info);
#else
            EditorGUILayout.HelpBox("Map Magic not detected", MessageType.Error);
#endif
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);

            _mapMagicInfiniteTerrain.vegetationSystem = EditorGUILayout.ObjectField("Source vegetation system.", _mapMagicInfiniteTerrain.vegetationSystem, typeof(VegetationSystem), true) as VegetationSystem;
            if (_mapMagicInfiniteTerrain.vegetationSystem == null)
            {
                EditorGUILayout.HelpBox("You need to select the source vegetation system.", MessageType.Error);
            }
            EditorGUILayout.HelpBox("Select the source Vegetation system. An instance of this will be spawned for each new terrain created by map magic.", MessageType.Info);
            GUILayout.EndVertical();
        }
    }
}