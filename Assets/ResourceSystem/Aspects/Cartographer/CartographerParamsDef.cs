using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Utils;

namespace SharedCode.Aspects.Cartographer
{
    public class CartographerParamsDef : BaseResource
    {
        public class SceneLoadArea : BaseResource
        {
            public string Name { get; set; }
            public SceneCollectionDef Collection { get; set; }
        }

        public class OperationArea : BaseResource
        {
            public bool Active { get; set; }
            public Vector3Int Start { get; set; }
            public Vector3Int Count { get; set; }
        }

        public class BackgroundClientCreation : BaseResource
        {
            public string TerrainPrefabsFolder { get; set; }
            public string TerrainPrefabName { get; set; }
            public string TerrainMeshName { get; set; }
            public string TerrainDiffuseTextureName { get; set; }
            public string TerrainMaterialName { get; set; }
            public int TerrainTextureSize { get; set; }
            public int TerrainVertexCount { get; set; }
            public string TerrainCollidersGameObjectName { get; set; }
            public string TerrainVisualsGameObjectName { get; set; }

            public string StaticObjectsMaterialsFolder { get; set; }
            public string StaticObjectsMaterialPostfix { get; set; }
            public string StaticObjectVisualsGameObjectName { get; set; }
            public Vector3 MinimalBounds { get; set; }

            public OperationArea CleanupTerrainColliders { get; set; }
            public OperationArea CleanupTerrainVisuals { get; set; }
            public OperationArea CleanupStaticObjectVisuals { get; set; }

            public OperationArea GenerateTerrainColliders { get; set; }
            public OperationArea GenerateTerrainVisuals { get; set; }
            public OperationArea GenerateStaticObjectVisuals { get; set; }
        }

        public class BackgroundServerCreation : BaseResource
        {
            public string TerrainCollidersGameObjectName { get; set; }
            public string StaticObjectCollidersGameObjectName { get; set; }

            public OperationArea CleanupTerrainColliders { get; set; }
            public OperationArea CleanupStaticObjectColliders { get; set; }

            public OperationArea GenerateTerrainColliders { get; set; }
            public OperationArea GenerateStaticObjectColliders { get; set; }
        }

        // Crater Background creation -------------------------------------------------------------
        public string BackgroundClientSceneName { get; set; }
        public BackgroundClientCreation BackgroundClientCreationParams { get; set; }
        // Crater Server creation -----------------------------------------------------------------
        public string BackgroundServerSceneName { get; set; }
        public BackgroundServerCreation BackgroundServerCreationParams { get; set; }
        // Custom fields --------------------------------------------------------------------------
        public ResourceRef<MapDef> MapDef { get; set; }
        public bool ShowGridlines { get; set; }
        public bool ShowLoadedScenes { get; set; }
        public bool ConstantRepaint { get; set; }
        public SceneCollectionDef SourceCollection { get; set; }
        public Vector3 MinimalOcculderBounds { get; set; }
        public string VegetationAsset { get; set; }
        public string TemporarySceneName { get; set; }
        public string ResultCollectionFolder { get; set; }
        public string SceneCollectionSubFolder { get; set; }
        public string TerrainCollectionSubFolder { get; set; }
        public string RootTerrainGameObjectName { get; set; }
        public string RootSceneGameObjectName { get; set; }
        public string ResultSceneCollectionDefName { get; set; }
        public string ResultSceneCollectionMinimap { get; set; }
        public string AssetsFolder { get; set; }
        public bool CreateTemporaryScene { get; set; }
        public bool SplitTerrain { get; set; }
        public bool CreateVegetation { get; set; }
        public bool CreateSceneCollection { get; set; }
        public bool CreateScenes { get; set; }
        public List<SceneLoadArea> SceneLoadAreas { get; set; }
    }
}
