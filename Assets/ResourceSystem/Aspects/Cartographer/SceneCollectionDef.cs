using Assets.Src.ResourcesSystem.Base;
using SharedCode.Utils;
using System.Collections.Generic;

namespace SharedCode.Aspects.Cartographer
{
    public class SceneCollectionDef : BaseResource
    {
        public bool CollectByX { get; set; }
        public bool CollectByY { get; set; }
        public bool CollectByZ { get; set; }

        public string ScenePrefix { get; set; }
        public string SceneFolder { get; set; }
        public List<string> SceneNames { get; set; }
        public string BackgroundSceneName { get; set; }
        public string BackgroundTerrainColliderName { get; set; }
        public string BackgroundTerrainName { get; set; }
        public string BackgroundStaticObjectsName { get; set; }
        public string ServerSceneName { get; set; }

        public Vector3 SceneSize { get; set; }

        public Vector3Int SceneStart { get; set; }
        public Vector3Int SceneCount { get; set; }
    }

    public class SceneSourceDef : BaseResource
    {
        public List<string> SceneNames { get; set; }
        public Vector2 SceneSize { get; set; }
    }

    public class SceneSpaceDef : BaseResource
    {
        public class SceneNode
        {
            public Vector2Int Index { get; set; }
            public Vector2 Center { get; set; }
            public Vector2 HalfSize { get; set; }
            public string Name { get; set; }
        }

        public Vector2Int Start { get; set; }
        public Vector2Int Count { get; set; }
        public Vector2 Center { get; set; }
        public Vector2 HalfSize { get; set; }
        public string Prefix { get; set; }
        public string Folder { get; set; }
        public List<SceneNode> Nodes { get; set; }
    }
}