using UnityEngine;



namespace Assets.TerrainBaker
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class TerrainBakerMaterialSupport : MonoBehaviour
    {
        public TerrainAtlas atlas = null;
        public Texture2D matTexture = null;
        public Texture2D weightsTexture = null;
        public Mesh terrainMesh = null;        
        public TerrainMaterialsArray materialsArray = null;
        public Texture2D normalsHeightsTexture = null;

        public float heightsScale;


        public Vector4 terrainSizes;
        public Vector4 terrainMatTexInfo;
        public Vector4 terrainWgtTexInfo;

        public bool isHasEmissionTexture;
        public bool isHasMacroTexture;
        public bool isHasTriplanar;

        public Material material;

        public const string renderShaderName = "Hidden/TerrainBaker";
        private const string enableEmissionTexture = "EMISSION_TEXTURE";
        private const string enableMacroTexture = "MACRO_TEXTURE";

        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }

        private void OnWillRenderObject()
        {
            UpdateMaterial();
        }

        public void UpdateMaterial()
        {
            if (atlas == null || material == null)
            {
                return;
            }
            material.color = Color.white;
            material.SetTexture("TerrainMaterials", matTexture);
            material.SetTexture("TerrainWeights", weightsTexture);
            material.SetTexture("_MainTex", normalsHeightsTexture);
            material.SetVector("TerrainSizes", terrainSizes);
            material.SetVector("TerrainMatTexInfo", terrainMatTexInfo);
            material.SetVector("TerrainWgtTexInfo", terrainWgtTexInfo);
            atlas.Set(material);

            if (isHasEmissionTexture)
            {
                material.EnableKeyword(enableEmissionTexture);
            }
            else
            {
                material.DisableKeyword(enableEmissionTexture);
            }
            if (isHasMacroTexture)
            {
                material.EnableKeyword(enableMacroTexture);
            }
            else
            {
                material.DisableKeyword(enableMacroTexture);
            }
        }

        public TerrainAtlas.Layer GetDominantLayer(Vector2 terrainCoords)
        {
            if (terrainCoords.x > terrainSizes.y || terrainCoords.y > terrainSizes.y)
                return default(TerrainAtlas.Layer);
            var singleTerrainLayrBlockSize = terrainSizes.y / materialsArray.alphaMapSize;
            var layerMaterialIndexX = Mathf.FloorToInt(terrainCoords.x / singleTerrainLayrBlockSize);
            var layerMaterialIndexY = Mathf.FloorToInt(terrainCoords.y / singleTerrainLayrBlockSize);
            var layerMaterialIndex = layerMaterialIndexX + layerMaterialIndexY * materialsArray.alphaMapSize;
            var layerAtlasIndex = materialsArray.materialsMap[layerMaterialIndex];
            if (layerAtlasIndex == TerrainMaterialsArray.noMaterial)
                return null;
            var layer = atlas.layers[layerAtlasIndex];
            return layer;
        }
    }
}
