using UnityEditor;
using UnityEngine;
using Assets.Instancenator;
using Assets.Instancenator.Editor;
using System.Collections.Generic;


namespace Assets.WorldFiller.Editor
{
    [CustomEditor(typeof(TerrainFiller)), CanEditMultipleObjects]
    public class TerrainFillerInspector : UnityEditor.Editor
    {

        InstanceCompositionBuilder builder;


        private void OnEnable()
        {
            CheckParams();
        }

        private void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Generate"))
            {
                CheckParams();
                TerrainFiller terrainFiller = target as TerrainFiller;
                Terrain terrain = terrainFiller.GetComponent<Terrain>();
                if (terrain != null && terrain.terrainData != null && terrainFiller != null && terrainFiller.atlas != null && terrainFiller.terrainFilling != null)
                {
                    Generate();
                    EditorUtility.SetDirty(terrainFiller);
                    Instancenator.Editor.Utils.OnChange(terrainFiller.terrainFilling);
                }
            }
            GUILayout.EndVertical();
        }

        public void CheckParams()
        {
            TerrainFiller terrainFiller = target as TerrainFiller;
            if (terrainFiller.atlas == null)
            {
                TerrainBaker.TerrainBakerMaterialSupport support = terrainFiller.GetComponent<TerrainBaker.TerrainBakerMaterialSupport>();
                if (support != null)
                {
                    terrainFiller.atlas = support.atlas;
                }
            }
            if (terrainFiller.terrainFilling == null)
            {
                Terrain terrain = terrainFiller.GetComponent<Terrain>();
                if (terrain != null)
                {
                    const string assetExt = ".asset";
                    string prefixPath = AssetDatabase.GetAssetOrScenePath(terrain.terrainData);
                    if (prefixPath.EndsWith(assetExt))
                    {
                        prefixPath = prefixPath.Remove(prefixPath.Length - assetExt.Length, assetExt.Length);
                    }
                    string prefixName = prefixPath;
                    if (prefixName.LastIndexOf('/') >= 0)
                    {
                        prefixName = prefixName.Remove(0, prefixName.LastIndexOf('/') + 1);
                    }

                    const string postfix = "_filling";
                    string assetPath = prefixPath + postfix + assetExt;
                    terrainFiller.terrainFilling = Instancenator.Editor.Utils.LoadOrCreate(assetPath);
                }
            }

        }

        private struct GenerateInfo
        {
            public System.Random random;
            public float[,,] alphamaps;
            public int layerIndex;
            public int alphamapsWidth;
            public int alphamapsHeight;
            public TerrainBaker.TerrainAtlas.Layer layer;
            public Vector3 terrainSize;
            public TerrainData terrainData;
        }


        public void Generate()
        {
            TerrainFiller terrainFiller = target as TerrainFiller;
            if (terrainFiller.terrainFilling == null || terrainFiller.atlas == null)
            {
                Debug.LogError("Error TerrainFiller content");
                return;
            }

            Terrain terrain = terrainFiller.GetComponent<Terrain>();
            if (terrain == null || terrain.terrainData == null)
            {
                Debug.LogError("Error Terrain content");
                return;
            }

            terrainFiller.terrainFilling.blocks = null;
            terrainFiller.terrainFilling.instances = null;

            Vector3 terrainSize = terrain.terrainData.size;
            Vector3 position = terrainFiller.transform.position;
            position.x = Mathf.Abs(terrainSize.x) > 0.001f ? position.x / terrainSize.x : position.x;
            position.z = Mathf.Abs(terrainSize.z) > 0.001f ? position.z / terrainSize.z : position.z;
            int seedFromTerrainPosition = (((int)position.x) & 0xfff) | ((((int)position.x) & 0xfff) << 12);

            builder = new InstanceCompositionBuilder();

            float[,,] alphamaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
            SplatPrototype[] layers = terrain.terrainData.splatPrototypes;
            if (alphamaps == null || layers == null || alphamaps.GetLength(2) != layers.Length)
            {
                Debug.LogError("Error TerrainData content");
                return;
            }


            foreach (TerrainBaker.TerrainAtlas.Layer layer in terrainFiller.atlas.layers)
            {
                LayerFillingData layerFilling = layer.layerFilling as LayerFillingData;
                if (layerFilling == null)
                {
                    continue;
                }

                int layerIndex = -1;
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].texture != null && layers[i].texture.name == layer.albedoName)
                    {
                        layerIndex = i;
                        break;
                    }
                }
                if (layerIndex < 0)
                {
                    continue;
                }

                int seed = layerFilling.seed;
                if (!layerFilling.disableModifySeedFromPosition)
                {
                    seed *= seedFromTerrainPosition;
                }

                GenerateInfo genInfo = new GenerateInfo();
                genInfo.random = new System.Random(layerFilling.seed * seedFromTerrainPosition);
                genInfo.alphamaps = alphamaps;
                genInfo.alphamapsWidth = terrain.terrainData.alphamapWidth;
                genInfo.alphamapsHeight = terrain.terrainData.alphamapHeight;
                genInfo.layerIndex = layerIndex;
                genInfo.layer = layer;
                genInfo.terrainSize = terrainSize;
                genInfo.terrainData = terrain.terrainData;

                foreach (LayerFillingData.Node node in layerFilling.nodes)
                {
                    ProcessNode(node, genInfo);
                }
            }

            builder.Process(terrainFiller.terrainFilling);

            Utils.OnChange(terrainFiller.terrainFilling);
        }

        private void ProcessNode(LayerFillingData.Node node, GenerateInfo genInfo)
        {
            if (node.lods.Count == 0 || node.lods[0].mesh == null || node.lods[0].material == null)
            {
                return;
            }

            InstanceCompositionBuilder.Block block = new InstanceCompositionBuilder.Block();
            PlaceNode(node, block, genInfo);
            if (block.instances.Count > 0)
            {
                builder.blocks.Add(block);
            }
        }

        private void PlaceNode(LayerFillingData.Node node, InstanceCompositionBuilder.Block block, GenerateInfo genInfo)
        {
            block.lods = new InstanceCompositionBuilder.LOD[Mathf.Min(node.lods.Count, 4)];
            int[] instancesCounts = new int[block.lods.Length];
            for (int i = 0; i < block.lods.Length; i++)
            {
                block.lods[i].mesh = node.lods[i].mesh;
                block.lods[i].material = node.lods[i].material;
                block.lods[i].distanceInPercents = node.lods[i].distanceInPercents;
                block.lods[i].isCastShadows = node.lods[i].isCastShadows;
                instancesCounts[i] = (int)(Mathf.Clamp01(node.lods[i].countInPercents * 0.01f) * node.instanceCount);
            }
            instancesCounts[0] = node.instanceCount;
            block.isEnableTransition = node.isEnableTransition;
            block.transitionInPersents = node.transitionInPersents;
            block.maxDistance = node.maxViewDistance;
            block.instances.Capacity = node.instanceCount;
            for (int i = 0; i < node.instanceCount; i++)
            {
                InstanceCompositionBuilder.InstanceData instance = new InstanceCompositionBuilder.InstanceData();
                float relX = (float)genInfo.random.NextDouble();
                float relZ = (float)genInfo.random.NextDouble();
                float y = genInfo.terrainData.GetInterpolatedHeight(relX, relZ);
                instance.position = new Vector3(relX * genInfo.terrainSize.x, y, relZ * genInfo.terrainSize.z);
                instance.rotate = Quaternion.Euler(0.0f, (float)(genInfo.random.NextDouble() * 359.0), 0.0f);
                instance.value.w = GetRange(node.scaleStart, node.scaleEnd, genInfo.random);
                instance.value.x = GetRange(0.4f, 0.8f, genInfo.random);
                instance.value.y = GetRange(0.4f, 0.8f, genInfo.random);
                instance.value.z = GetRange(0.4f, 0.8f, genInfo.random);
                for (int j = block.lods.Length - 1; j > 0; j--)
                {
                    if (i < instancesCounts[j])
                    {
                        instance.lodIndex2bit = (byte)j;
                        break;
                    }
                }
                instance.options2bit = 0;

                //for random invariant
                if (!node.disableGenerate)
                {
                    float threshold = Mathf.Clamp01(node.weightThreshold);
                    float layerWeight = GetAlphmap(relX, relZ, genInfo);
                    if (layerWeight >= threshold)
                    {
                        if (layerWeight < 1.0f)
                        {
                            instance.value.w = Mathf.Lerp(node.scaleStart, instance.value.w, (layerWeight - threshold) / (1.0f - threshold));
                        }
                        block.instances.Add(instance);
                    }

                }
            }

        }

        private static float GetRange(float min, float max, System.Random random)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        private static float GetAlphmap(float relX, float relZ, GenerateInfo genInfo)
        {
            float x = (genInfo.alphamapsWidth - 1) * relX;
            float z = (genInfo.alphamapsHeight - 1) * relZ;
            int x0 = Mathf.Clamp((int)x, 0, genInfo.alphamapsWidth - 1);
            int z0 = Mathf.Clamp((int)z, 0, genInfo.alphamapsHeight - 1);
            int x1 = Mathf.Clamp(x0 + 1, 0, genInfo.alphamapsWidth - 1);
            int z1 = Mathf.Clamp(z0 + 1, 0, genInfo.alphamapsWidth - 1);

            float v00 = genInfo.alphamaps[z0, x0, genInfo.layerIndex];
            float v01 = genInfo.alphamaps[z0, x1, genInfo.layerIndex];
            float v10 = genInfo.alphamaps[z1, x0, genInfo.layerIndex];
            float v11 = genInfo.alphamaps[z1, x1, genInfo.layerIndex];

            float kx = Mathf.Clamp01(x - x0);
            float v0 = Mathf.Lerp(v00, v01, kx);
            float v1 = Mathf.Lerp(v10, v11, kx);

            float kz = Mathf.Clamp01(z - z0);
            float v = Mathf.Lerp(v0, v1, kz);

            return v;
        }

    }

}