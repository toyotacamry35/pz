using System;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Cartographer;
using SharedCode.Entities.GameObjectEntities;

namespace GeneratedCode.Custom.Config
{
    public struct Vec2
    {
        private const float Tolerance = 0.000001f;
        public float X { get; set; }
        public float Y { get; set; }

        public bool IsZero => Math.Abs(X) < Tolerance && Math.Abs(Y) < Tolerance;
    }
    public struct Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public struct AABB
    {
        public Vec3 Center { get; set; }
        public Vec3 Extents { get; set; }
    }
    public struct LocalScene
    {
        public AABB Aabb;
        public string SceneName { get; set; }
    }
    public class MapDef : SaveableBaseResource
    {
        public string DebugName { get; set; } = "";
        public bool NeedsUnity { get; set; } = true;
        public bool ClientMap { get; set; } = false;
        public bool DestroyOnLastUser { get; set; } = false;
        public bool IsForSingleUserOnly { get; set; } = false;
        public string[] SceneCollectionClientSceneNames => (SceneCollectionClient.Target?.SceneNames == null) ? Array.Empty<string>() : SceneCollectionClient.Target.SceneNames.Select(sceneName => $"{SceneCollectionClient.Target.SceneFolder}/{sceneName}.unity").Distinct().ToArray();
        public string[] AllScenesToBeLoadedOnServerViaJdb => GlobalScenesServer.Concat(JdbOnlyScenes).Concat(SceneCollectionClientSceneNames).Distinct().ToArray();
        public string[] AllScenesToBeLoadedOnClientViaJdb => GlobalScenesClient.Concat(JdbOnlyScenes).Concat(SceneCollectionClientSceneNames).Distinct().ToArray();
        public string[] AllScenesToBeLoadedViaJdb => GlobalScenesServer.Concat(AllScenesToBeLoadedOnClientViaJdb).Distinct().ToArray();
        public string[] ExcludeInDevMode { get; set; } = { };
        public string[] AllScenesExportedJdbsPaths => AllScenesToBeLoadedOnServerViaJdb.Select(x =>
        {
            var sceneNames = x.Split('/').Last().Split('.');
            var sceneName = sceneNames.Take(sceneNames.Count() - 1).Single();
            return $"/SpawnSystemData/{sceneName}/{sceneName}";
        })
        .ToArray();

        public string[] GlobalScenesClient { get; set; } = { };
        public string[] GlobalScenesServer { get; set; } = { };

        public string[] JdbOnlyScenes { get; set; } = { };

        public LocalScene[] LocalScenesClient { get; set; } = { };
        public Vec2 MapBottomLeftCorner { get; set; }
        public Vec2 MapTopRightCorner { get; set; }
        public Guid DefaultMapGuid { get; set; }
        public ResourceRef<SceneCollectionDef> SceneCollectionClient { get; set; }
        public ResourceRef<SceneStreamerDef> SceneStreamerClient { get; set; }
        public ResourceRef<SpawnPointTypeDef> SpawnPoint { get; set; }
    }
}
