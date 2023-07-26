using System;
using Assets.TerrainBaker;
using SharedCode.Aspects.Cartographer;
using UnityEngine;

namespace Assets.Src.Cartographer
{
    public enum SceneStreamerMode
    {
        Default,
        OptimiseLoadtime,
        OptimiseRuntime
    }

    public enum SceneStreamerStatus
    {
        Unknown, // streamer не инициализирован
        NotReady, // текущая сцена (там где находится центр стриминга, т.е. игрок) не загружена
        ImportantReady, // текущая сцена загружена, но перефирийные ещё нет
        AllReady // загружена текущая сцена и все перефирийные сцены 
    }

    public interface ISceneStreamerInterface
    {
        bool Initialized { get; }
        SceneStreamerMode Mode { get; set; }
        SceneCollectionDef SceneCollection { get; }
        SceneStreamerDef SceneStreamer { get; }
        Bounds WorldBounds { get; }

        event Action<Guid, SceneStreamerStatus, SceneStreamerStatus> StatusChanged;

        bool Initialize(SceneCollectionDef sceneCollection, SceneStreamerDef sceneStreamer);
        void Deinitialize();

        SceneStreamerStatus GetStatus(Guid id);
        void SetPosition(Guid id, Vector3 position, bool force);
        bool Remove(Guid id);

        void ShowBackground(bool show, string sceneName, TerrainBakerMaterialSupport terrain);
        void ReportReady(bool ready, string sceneName);
    }
}
