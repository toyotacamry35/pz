using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
#if PERSISTENT_VEGETATION
    public partial class TerrainDetailImporter : IVegetationImporter
    {
        private Terrain _sourceTerrain;
        private int _selectedGridIndex;
        private PersistentVegetationStorage _persistentVegetationStorage;

        public string ImporterName
        {
            get
            {
                return "Terrain detail importer";
            }
        }

        public PersistentVegetationStoragePackage PersistentVegetationStoragePackage { get; set; }
        public VegetationPackage VegetationPackage { get; set; }

        public PersistentVegetationStorage PersistentVegetationStorage
        {
            get
            {
                return _persistentVegetationStorage;
            }

            set
            {
                if (_sourceTerrain == null && _persistentVegetationStorage)
                {
                    _sourceTerrain = _persistentVegetationStorage.VegetationSystem.currentTerrain;
                }

                _persistentVegetationStorage = value;
            }
        }

        private void ImportDetails(List<Texture2D> textureList)
        {

            //float cellWidth = _sourceTerrain.terrainData.size.x / (_sourceTerrain.terrainData.detailWidth -1);
            //float cellHeight = _sourceTerrain.terrainData.size.z / (_sourceTerrain.terrainData.detailHeight -1);
            //Vector3 terrainPosition = _sourceTerrain.transform.position;
            _persistentVegetationStorage.VegetationSystem.LoadUnityTerrainDetails = true;

            for (int i = 0; i <= textureList.Count - 1; i++)
            {
                if (textureList[i] == null) continue;

                DetailPrototype detailPrototype = _sourceTerrain.terrainData.detailPrototypes[i];
                GameObject defaultTexturePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/GrassSystem/_Resources/DefaultGrassPatch.prefab");
                string vegetationItemId = VegetationPackage.AddVegetationItem(textureList[i], defaultTexturePrefab, VegetationType.Grass, false);
                VegetationItemInfo vegetationItemInfo = VegetationPackage.GetVegetationInfo(vegetationItemId);
                vegetationItemInfo.YScale = 2f;
                vegetationItemInfo.EnableRuntimeSpawn = true;
                vegetationItemInfo.MinScale = detailPrototype.minHeight;
                vegetationItemInfo.MaxScale = detailPrototype.maxHeight;
                vegetationItemInfo.IncludeDetailLayer = i;
                vegetationItemInfo.UseIncludeDetailMaskRules = true;
                vegetationItemInfo.UsePerlinMask = false;

                //int[,] detailsLayer = _sourceTerrain.terrainData.GetDetailLayer(0, 0,
                //    _sourceTerrain.terrainData.detailWidth, _sourceTerrain.terrainData.detailHeight, i);

                //int maxValue = 0;

                //for (int x = 0; x <= _sourceTerrain.terrainData.detailWidth - 1; x++)
                //{
                //    for (int z = 0; z <= _sourceTerrain.terrainData.detailHeight - 1; z++)
                //    {

                //        if (x % 50 == 0)
                //        {
                //            float progress = (float)x / _sourceTerrain.terrainData.detailWidth;
                //            EditorUtility.DisplayProgressBar("Import detail: " + textureList[i].name, "Importing details", progress);
                //        }
                //        float normalizedX = (float) x / (_sourceTerrain.terrainData.detailWidth -1);
                //        float normalizedZ = (float) z / (_sourceTerrain.terrainData.detailHeight -1);
                //        float height =_sourceTerrain.terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
                //        Vector3 worldPosition = new Vector3(x * cellWidth + terrainPosition.x, height,
                //            z * cellHeight + terrainPosition.z);

                //        Vector3 lookAt;
                //        Quaternion rotation = Quaternion.identity;
                //        Vector3 angleScale = Vector3.zero;

                //        Vector3 terrainNormal =
                //            _sourceTerrain.terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
                //        var slopeCos = Vector3.Dot(terrainNormal, Vector3.up);
                //        float slopeAngle = Mathf.Acos(slopeCos) * Mathf.Rad2Deg;

                //        lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                //        // reverse it if it is down.
                //        lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                //        // look at the hit's relative up, using the normal as the up vector
                //        rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                //        //targetUp = Rotation * Vector3.up;
                //        rotation *= Quaternion.AngleAxis(UnityEngine.Random.Range(0, 365f), new Vector3(0, 1, 0));

                //        float newScale = Mathf.Clamp01(slopeAngle / 45f);
                //        angleScale = new Vector3(newScale, 0, newScale);


                //        worldPosition += GetRandomOffset(cellWidth/2f);
                //        if (detailsLayer[x, z] > 1)
                //        {
                //            float widthScale =
                //                UnityEngine.Random.Range(detailPrototype.minWidth, detailPrototype.maxWidth) *2;

                //            PersistentVegetationStorage.AddVegetationItemInstance(vegetationItemId, worldPosition,
                //                    new Vector3(widthScale, UnityEngine.Random.Range(detailPrototype.minHeight, detailPrototype.maxHeight) *2, widthScale) + angleScale,
                //                rotation, 4); 
                //        }
                //    }
                //}

                //Debug.Log(maxValue);
                //EditorUtility.ClearProgressBar();

                //for (int j = 0; j <= _sourceTerrain.terrainData.treeInstances.Length - 1; j++)
                //{
                //    //TreeInstance treeInstance = _sourceTerrain.terrainData.treeInstances[j];
                //    //if (treeInstance.prototypeIndex == i)
                //    //{
                //    //    Vector3 position = new Vector3(treeInstance.position.x * _sourceTerrain.terrainData.size.x,
                //    //                           treeInstance.position.y * _sourceTerrain.terrainData.size.y,
                //    //                           treeInstance.position.z * _sourceTerrain.terrainData.size.z) + _sourceTerrain.transform.position;

                //    //    PersistentVegetationStorage.AddVegetationItemInstance(vegetationItemId, position,
                //    //        new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale),
                //    //        Quaternion.Euler(0, treeInstance.rotation * Mathf.Rad2Deg, 0), 2);
                //    //}
                //}
            }
        }

        //private Vector3 GetRandomOffset(float distance)
        //{
        //    return new Vector3(UnityEngine.Random.Range(-distance, distance), 0, UnityEngine.Random.Range(-distance, distance));
        //}
        /*
        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            //var labelStyle = new GUIStyle("Label") {fontStyle = FontStyle.Italic};

            _sourceTerrain =
                EditorGUILayout.ObjectField("Source terrain", _sourceTerrain, typeof(Terrain), true) as Terrain;

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
                    VegetationPackageEditorTools.DrawTextureSelectorGrid(textureList, 60, ref _selectedGridIndex);                 
                    EditorGUILayout.HelpBox("This will import all detail settings and set the Vegetation items for run-time spawning. Adjust rules on VegetationSystem component", MessageType.Info);

                    if (GUILayout.Button("Import detail Vegetation items"))
                    {
                        ImportDetails(textureList);
                        PersistentVegetationStorage.VegetationSystem.SetupVegetationSystem();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("The terrain contains no details.", MessageType.Info);
                }
            }

            GUILayout.EndVertical();
        }
        */
    }
#endif
}
