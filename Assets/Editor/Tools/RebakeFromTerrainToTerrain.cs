//Spirenkov Maxim
//(c) Enplex games
using UnityEditor;
using UnityEngine;


namespace Assets.Editor.Tools
{
    public class RebakeFromTerrainToTerrain : EditorWindow
    {

        [MenuItem("Level Design/RebakeTerrain2Terrain")]
        static void Init()
        {
            GetWindow(typeof(RebakeFromTerrainToTerrain), true, "Rebake data from Terrain To Terrain");
        }

        private Terrain from;
        private Terrain to;
        private string errorString;

        private void OnGUI()
        {
            GUILayout.Space(20);
            from = (Terrain)EditorGUILayout.ObjectField("From: ", from, typeof(Terrain), true);
            GUILayout.Space(20);
            to = (Terrain)EditorGUILayout.ObjectField("To: ", to, typeof(Terrain), true);
            GUILayout.Space(20);
            if (GUILayout.Button("Rebake now!", GUILayout.Height(32)))
            {
                if (IsValid(from, "From") && IsValid(to, "To") && from != to)
                {
                    errorString = null;
                    Rebake(from, to);
                }
                else if (from == to)
                {
                    errorString = "Terrain";
                }
            }
            if (!string.IsNullOrEmpty(errorString))
            {
                EditorGUILayout.HelpBox(errorString, MessageType.Error);
            }
        }

        public bool IsValid(Terrain t, string id)
        {
            if (t == null || t.terrainData == null)
            {
                errorString = id + " terrain no set";
                return false;
            }
            if (t.terrainData.size.x < 1 || t.terrainData.size.y < 1 || t.terrainData.size.z < 1)
            {
                errorString = id + " terrain has invalid size";
                return false;
            }
            if (t.terrainData.alphamapLayers <= 0 || t.terrainData.alphamapWidth < 4 || t.terrainData.alphamapHeight < 4)
            {
                errorString = id + " terrain no have alphamaps";
                return false;
            }
            if (t.terrainData.heightmapResolution < 4 || t.terrainData.heightmapResolution < 4)
            {
                errorString = id + " terrain have invalid TerrainData";
                return false;
            }
            return true;
        }

        public bool IsNotEqual(Terrain from, Terrain to)
        {
            if (from == to)
            {
                errorString = "Terrain must be differ";
                return false;
            }
            if (from.terrainData.alphamapLayers != to.terrainData.alphamapLayers)
            {
                errorString = "Terrain textures must be equal";
                return false;
            }
            return true;
        }

        public static void Rebake(Terrain from, Terrain to)
        {
            Vector3 posDelta = to.transform.position - from.transform.position;
            int alphamapLayers = from.terrainData.alphamapLayers;
            float[] alphamapLayersBuf = new float[alphamapLayers];
            float[,] heightsMap = to.terrainData.GetHeights(0, 0, to.terrainData.heightmapResolution, to.terrainData.heightmapResolution);
            float[,,] toAlphamaps = to.terrainData.GetAlphamaps(0, 0, to.terrainData.alphamapWidth, to.terrainData.alphamapHeight);
            float[,,] fromAlphamaps = from.terrainData.GetAlphamaps(0, 0, from.terrainData.alphamapWidth, from.terrainData.alphamapHeight);

            for (int z = 0; z < to.terrainData.heightmapResolution; z++)
            {
                float toRelZ = z / (to.terrainData.heightmapResolution - 1.0f);
                float fromRelZ = (to.terrainData.size.z * toRelZ + posDelta.z) / from.terrainData.size.z;
                if (fromRelZ < 0.0f || fromRelZ > 1.0f)
                {
                    continue;
                }
                for (int x = 0; x < to.terrainData.heightmapResolution; x++)
                {
                    float toRelX = x / (to.terrainData.heightmapResolution - 1.0f);
                    float fromRelX = (to.terrainData.size.x * toRelX + posDelta.x) / from.terrainData.size.x;
                    if (fromRelX < 0.0f || fromRelX > 1.0f)
                    {
                        continue;
                    }
                    //Height
                    float fromHeightWorld = from.terrainData.GetInterpolatedHeight(fromRelX, fromRelZ) + from.transform.position.y;
                    float toHeight = (fromHeightWorld - to.transform.position.y) / to.terrainData.size.y;
                    heightsMap[z, x] = Mathf.Clamp01(toHeight);
                    //Alphamap
                    float pixelOffet = 0.5f;
                    float alphamapPosX = Mathf.Min(fromRelX * (from.terrainData.alphamapWidth - 1) - pixelOffet, from.terrainData.alphamapWidth - 1);
                    int iposX0 = (int)alphamapPosX;
                    int iposX1 = Mathf.Min(iposX0 + 1, from.terrainData.alphamapWidth - 1);
                    float lerpX = alphamapPosX - iposX0;
                    float alphamapPosZ = Mathf.Min(fromRelZ * (from.terrainData.alphamapHeight - 1) - pixelOffet, from.terrainData.alphamapHeight - 1);
                    int iposZ0 = (int)alphamapPosZ;
                    int iposZ1 = Mathf.Min(iposZ0 + 1, from.terrainData.alphamapHeight - 1);
                    float lerpZ = alphamapPosZ - iposZ0;
                    float sum = 0.0f;
                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        float a00 = fromAlphamaps[iposZ0, iposX0, i];
                        float a01 = fromAlphamaps[iposZ0, iposX1, i];
                        float a10 = fromAlphamaps[iposZ1, iposX0, i];
                        float a11 = fromAlphamaps[iposZ1, iposX1, i];
                        float a0 = Mathf.Lerp(a00, a01, lerpX);
                        float a1 = Mathf.Lerp(a10, a11, lerpX);
                        float a = Mathf.Lerp(a0, a1, lerpZ);

                        alphamapLayersBuf[i] = a;
                        sum += a;
                    }
                    int toPosX = Mathf.Min((int)(toRelX * (to.terrainData.alphamapWidth - 1)), to.terrainData.alphamapWidth - 1);
                    int toPosZ = Mathf.Min((int)(toRelZ * (to.terrainData.alphamapHeight - 1)), to.terrainData.alphamapHeight - 1);
                    if (sum > 0.01f)
                    {
                        for (int i = 0; i < alphamapLayers; i++)
                        {
                            toAlphamaps[toPosZ, toPosX, i] = alphamapLayersBuf[i] / sum;
                        }
                    }
                    else
                    {
                        toAlphamaps[toPosZ, toPosX, 0] = 1.0f;
                        for (int i = 1; i < alphamapLayers; i++)
                        {
                            toAlphamaps[toPosZ, toPosX, i] = 0.0f;
                        }
                    }
                }
            }
            to.terrainData.SetHeights(0, 0, heightsMap);
            to.terrainData.SetAlphamaps(0, 0, toAlphamaps);
            heightsMap = null;
        }

    }
}