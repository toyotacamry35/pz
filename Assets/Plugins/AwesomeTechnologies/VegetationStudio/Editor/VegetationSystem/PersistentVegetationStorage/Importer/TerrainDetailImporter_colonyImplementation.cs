using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
#if PERSISTENT_VEGETATION
    public partial class TerrainDetailImporter : IVegetationImporter
    {
        public void OnGUI()
        {
            //GUILayout.BeginVertical("box");
            //var labelStyle = new GUIStyle("Label") {fontStyle = FontStyle.Italic};

            _sourceTerrain = PersistentVegetationStorage.VegetationSystem.UnityTerrainData.terrain;

            if (_sourceTerrain != null)
            {

                List<Texture2D> textureList = new List<Texture2D>();
                for (int i = 0; i <= _sourceTerrain.terrainData.detailPrototypes.Length - 1; i++)
                {
                    DetailPrototype detailPrototype = _sourceTerrain.terrainData.detailPrototypes[i];
                    textureList.Add(detailPrototype.prototypeTexture);
                }

                if (textureList.Count > 0)
                {
                    //VegetationPackageEditorTools.DrawTextureSelectorGrid(textureList, 60, ref _selectedGridIndex);
                    //EditorGUILayout.HelpBox("This will import all detail settings and set the Vegetation items for run-time spawning. Adjust rules on VegetationSystem component", MessageType.Info);

                    if (GUILayout.Button("Import detail"))
                    {
                        ImportDetails(textureList);
                        PersistentVegetationStorage.VegetationSystem.SetupVegetationSystem();
                    }
                }

            }


    // GUILayout.EndVertical();
        }
    }
#endif
}
