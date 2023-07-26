using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Cartographer
{
    public class SceneStreamerDef : BaseResource
    {
        public int TerrainActivateCount { get; set; }
        public int StaticActivateCount { get; set; }
        public int FxActivateCount { get; set; }

        public float CheckRange { get; set; }

        public float ImportanceRange { get; set; }
        public float LoadSceneRange { get; set; }
        public float UnloadSceneRange { get; set; }

        public int LoadSceneFramesInterval { get; set; }
        public int UnloadSceneFramesInterval { get; set; }
    }

    public class SceneStreamDef : BaseResource
    {
    }
}