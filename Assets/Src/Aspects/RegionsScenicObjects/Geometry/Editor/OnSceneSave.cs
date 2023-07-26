using Assets.Src.ResourceSystem;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using ColonyHelpers;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Regions;
using SharedCode.Entities.GameObjectEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ShapeEditor;

namespace Assets.Src.Aspects.RegionsScenicObjects.Geometry.Editor
{

    [CustomEditor(typeof(TemplatesPlacer))]
    class TemplatesPlacerEditor : UnityEditor.Editor
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        bool _alreadyUpdatedShape = false;
        private static Collider[] _hits = new Collider[500];

        private const float threshold = 0.4f;
        string displayedTemplateNotification;
        string diplayedObjectNotification;

        private GUIStyle _redStyle;
        private GUIStyle RedStyle
        {
            get
            {
                if (_redStyle == null)
                {
                    _redStyle = new GUIStyle(GUI.skin.button);
                    _redStyle.normal.textColor = DebugHelper.ColorDarkRed;
                }
                return _redStyle;
            }
        }

        public override void OnInspectorGUI()
        {
            TemplatesPlacer placer = (TemplatesPlacer)target;
            PolygonRegion reg = placer.GetComponent<PolygonRegion>();
            if (reg == null)
            {
                GUI.color = Color.red;
                GUILayout.Label("No polygon region on this object!");
                return;
            }

            if (GUILayout.Button("Generate"))
            {
                PlaceProceduralTemplates(placer, reg);
            }

            base.DrawDefaultInspector();

            if (GUILayout.Button("Generate ALL", RedStyle))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to generate ALL ?", "Yes", "Cancel"))
                    foreach (var obj in FindObjectsOfType<TemplatesPlacer>())
                    {
                        var objreg = obj.GetComponent<PolygonRegion>();
                        PlaceProceduralTemplates(obj, objreg);
                    }
            }
            if (displayedTemplateNotification != null)
            {
                EditorGUILayout.HelpBox(displayedTemplateNotification, MessageType.Info);
                EditorGUILayout.HelpBox(diplayedObjectNotification, MessageType.Info);
            }
        }

        private void PlaceProceduralTemplates(TemplatesPlacer placer, PolygonRegion reg)
        {
            EditorSceneManager.MarkSceneDirty(placer.gameObject.scene);
            foreach (var sp in reg.GetComponentsInChildren<SpawnTemplate>())
                DestroyImmediate(sp.gameObject);

            System.Random random = new System.Random();
            var shape = reg.Shape;

            List<float> trianglesArea = new List<float>();
            float doubleSumOfTrianglesAreas = 0;
            foreach (var triangle in shape.Triangles)
            {
                doubleSumOfTrianglesAreas += GetTriangleDoubleArea(triangle);
                trianglesArea.Add(doubleSumOfTrianglesAreas);
            }
            var sumOfTrianglesAreas = doubleSumOfTrianglesAreas / 2;

            List<string> placedTemplatesInfo = new List<string>();
            Dictionary<String, int> placedObjectsInfo = new Dictionary<string, int>();
            foreach (var setting in placer.Settings)
            {
                float sumOfObjectAreas = 0;
                sumOfObjectAreas += (float)Math.Pow(setting.Template.Radius, 2) * setting.Count;
                sumOfObjectAreas *= (float)Math.PI;
                if (sumOfTrianglesAreas * 0.9 < sumOfObjectAreas)
                {
                    EditorUtility.DisplayDialog("Warning", $"It is not possible to place requested number of {setting.Template.name} objects to given area.", "OK");
                    continue;
                }
                int placedSpawnTemplatesCount = 0;
                for (int k = 0; k < 50 && placedSpawnTemplatesCount != setting.Count; k++)
                {
                    for (int i = 0; i < setting.Count - placedSpawnTemplatesCount; i++)
                    {

                        //Dictionary<string, int> placedInTemplateTotal = new Dictionary<string, int>();

                        // choose triangle regarding it's area
                        var randValueToSelectTriangle = (float)random.NextDouble() * doubleSumOfTrianglesAreas;
                        int j = 0;
                        for (j = 0; j < trianglesArea.Count; j++)
                        {
                            if (randValueToSelectTriangle <= trianglesArea[j])
                                break;
                        }
                        var randomTriangle = shape.Triangles[j];

                        SharedCode.Utils.Vector2 randomPoint = GetRandomPointInTriangle(randomTriangle, random);
                        int templatePlacingResult = PlaceTemplate(randomPoint, placer, setting, reg, placedObjectsInfo);
                        if (templatePlacingResult > 0)
                        {
                            placedSpawnTemplatesCount++;
                        }

                    }
                }
                if (placedSpawnTemplatesCount != setting.Count)
                    EditorUtility.DisplayDialog("Warning", "Only " + placedSpawnTemplatesCount + $" templates of type {setting.Template.name} were placed", "OK");
                placedTemplatesInfo.Add($"{setting.Template.name} : " + placedSpawnTemplatesCount.ToString());
            }
            displayedTemplateNotification = String.Join("\n", placedTemplatesInfo.ToArray());
            diplayedObjectNotification = String.Join("\n", ConvertDictionaryToList(placedObjectsInfo).ToArray());
        }

        private List<string> ConvertDictionaryToList(Dictionary<string, int> placedObjectsInfo)
        {
            List<string> result = new List<string>();
            foreach (var pair in placedObjectsInfo)
            {
                result.Add(pair.Key + "\t" + pair.Value.ToString());
            }
            return result;
        }

        private int PlaceTemplate(SharedCode.Utils.Vector2 randomPoint, TemplatesPlacer placer, TemplatesPlacer.TemplateSettings setting, PolygonRegion reg, Dictionary<string, int> placedObjects)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(new Vector3(randomPoint.x, reg.MaxHeight, randomPoint.y), Vector3.down), out hit, reg.MaxHeight - reg.MinHeight, PhysicsLayers.TerrainMask))
            {
                int hitsCount = Physics.OverlapSphereNonAlloc(hit.point, setting.Template.Radius, _hits, PhysicsLayers.DefaultMask);
                bool hasOverlappingSpawnTemplates = false;
                for (int h = 0; h < hitsCount && !hasOverlappingSpawnTemplates; h++)
                {
                    var t = _hits[h].gameObject.GetComponent<SpawnTemplate>();
                    if (t != null && t.ExclusionGroup == setting.Template.ExclusionGroup)
                        hasOverlappingSpawnTemplates = true;
                }
                if (hasOverlappingSpawnTemplates)
                    return 0;

                //DebugExtension.DebugWireSphere(hit.point, 1f, 5f, true);
                return PlaceObjectsOfTemplate(placer, setting, reg, hit, placedObjects);
            }
            return 0;
        }

        private int PlaceObjectsOfTemplate(TemplatesPlacer placer, TemplatesPlacer.TemplateSettings setting, PolygonRegion reg, RaycastHit hit, Dictionary<string, int> placedObjects)
        {
            int placedInTemplate = 0;
            GameObject go = new GameObject($"SpawnTemplate {setting.Template.name}");
            go.transform.position = hit.point;
            go.SetActive(false);
            var st = go.AddComponent<SpawnTemplate>();
            st.ExclusionGroup = setting.Template.ExclusionGroup;
            st.ExclusionRadius = setting.Template.ExclusionRadius;

            for (int p = 0; p < setting.Template.Settings.Length; p++)
            {
                int placedObjectsOfEachTypeInTemplate = 0;
                var templateSetting = setting.Template.Settings[p];
                for (int pCount = 0; pCount < templateSetting.Count; pCount++)
                {
                    var srPoint = hit.point.ToXZ() + UnityEngine.Random.insideUnitCircle * setting.Template.Radius;
                    RaycastHit srHit;
                    if (Physics.Raycast(new Ray(new Vector3(srPoint.x, reg.MaxHeight, srPoint.y), Vector3.down), out srHit, reg.MaxHeight - reg.MinHeight, PhysicsLayers.TerrainMask))
                    {
                        var hitGameObject = srHit.transform.gameObject;
                        if (templateSetting.TextureFilter == null)
                        {
                            PlaceObjectOfTemplate(go, srHit, templateSetting);
                            placedObjectsOfEachTypeInTemplate++;
                        }
                        else
                        {
                            var hitTerrain = hitGameObject.GetComponent<Terrain>();
                            if (hitTerrain != null) // if gameobject is terrain
                            {
                                // var texture = TerrainPoint.GetTexture(hitTerrain, srHit.point); 
                                var textureNumber = GetTextureNumber(hitTerrain.terrainData, templateSetting.TextureFilter);
                                if (textureNumber >= 0)
                                {
                                    var textureMix = GetTextureMix(srHit.point, hitTerrain.terrainData, hitTerrain.transform.position);
                                    if (CheckTextureDominance(textureMix, textureNumber) || textureMix[textureNumber] >= threshold)
                                    {
                                        PlaceObjectOfTemplate(go, srHit, templateSetting);
                                        placedObjectsOfEachTypeInTemplate++;
                                    }
                                }
                            }
                        }

                    }
                }
                AddToDictionary(placedObjects, templateSetting.SpawnPointType.name, placedObjectsOfEachTypeInTemplate);
                placedInTemplate += placedObjectsOfEachTypeInTemplate;
            }

            if (placedInTemplate > 0)
            {
                go.transform.SetParent(placer.transform, true);
                go.SetActive(true);
            }
            else
            {
                GameObject.DestroyImmediate(go);
            }
            return placedInTemplate;
        }

        private void PlaceObjectOfTemplate(GameObject gObject, RaycastHit hit, ProceduralTemplate.SpawnPointSettings templateSetting)
        {
            GameObject spawnPointGo = new GameObject($"SpawnPointOf {templateSetting.SpawnPointType.Get<SpawnPointTypeDef>().____GetDebugShortName()}");
            var spawnPoint = spawnPointGo.AddComponent<SpawnPoint>();
            spawnPoint.PointType = templateSetting.SpawnPointType;
            spawnPointGo.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.value * 360, 0);
            spawnPointGo.transform.position = hit.point;
            spawnPointGo.transform.SetParent(gObject.transform, true);
            //DebugExtension.DebugWireSphere(srHit.point, Color.green, 1f, 5f, false);
        }

        private float GetTriangleDoubleArea(Triangle triangle)
        {
            float x0 = triangle.PointA.x;
            float x1 = triangle.PointB.x;
            float x2 = triangle.PointC.x;

            float y0 = triangle.PointA.y;
            float y1 = triangle.PointB.y;
            float y2 = triangle.PointC.y;

            // cross product
            float doubleArea = (x1 - x0) * (y2 - y0) - (x2 - x0) * (y1 - y0);

            if (doubleArea < 0)
                doubleArea = -doubleArea;

            return doubleArea;
        }

        private SharedCode.Utils.Vector2 GetRandomPointInTriangle(Triangle randomTriangle, System.Random random)
        {
            // uniform distribution of points within triangle
            var sqrt_r1 = (float)Math.Sqrt(random.NextDouble());
            var r2 = (float)random.NextDouble();
            var pointA = randomTriangle.PointA;
            var pointB = randomTriangle.PointB;
            var pointC = randomTriangle.PointC;
            var randomPoint = pointA * (float)(1 - sqrt_r1) + pointB * sqrt_r1 * (1 - r2) + pointC * sqrt_r1 * r2;
            return randomPoint;
        }

        private void AddToDictionary(Dictionary<string, int> dictionary, string str, int count)
        {
            if (dictionary.ContainsKey(str))
                dictionary[str] += count;
            else
                dictionary.Add(str, count);
        }

        private static float[] GetTextureMix(Vector3 worldPosition, TerrainData terrainData, Vector3 terrainPos)
        {


            var mapX = (int)(((worldPosition.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            var mapZ = (int)(((worldPosition.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            float[] cellTextureMix = new float[splatmapData.GetUpperBound(2) + 1];
            for (int i = 0; i < cellTextureMix.Length; i++)
            {
                cellTextureMix[i] = splatmapData[0, 0, i];
            }
            return cellTextureMix;
        }

        private bool CheckTextureDominance(float[] textureMix, int textureNumber)
        {
            float max = 0;
            for (int i = 0; i < textureMix.Length; i++)
            {
                if (textureMix[i] > max)
                {
                    max = textureMix[i];
                }
            }
            if (textureMix[textureNumber] == max)
                return true;
            return false;
        }

        private int GetTextureNumber(TerrainData terrainData, Texture2D texture)
        {
            int result = -1;
            for (int i = 0; i < terrainData.splatPrototypes.Length; i++)
            {
                if (texture == terrainData.splatPrototypes[i].texture)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }
    }
}
