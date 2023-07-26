using UnityEngine;
using System.Collections.Generic;
using Assets.Instancenator;
using Assets.TerrainBaker;

namespace Assets.Src.Cartographer
{
    public class SceneLoaderBehaviour : MonoBehaviour
    {
        public string SceneName;
        public Vector3Int SceneIndex;
        public Vector3 SceneStart;
        public Vector3 SceneSize;
        public  Color SceneColor = Color.cyan;

        public List<GameObject> GameObjectsTerrain;
        public List<GameObject> GameObjectsStatic;
        public List<GameObject> GameObjectsFX;

        private int terrainActivateCount = 1;
        private int staticActivateCount = 1;
        private int fxActivateCount = 1;

        private bool somethingToActivate = true;
        private bool readyReported = true;

        private int terrainActivated = 0;
        private int staticActivated = 0;
        private int fxActivated = 0;

        private int readyUpdateCount = 0;
        private int readyFixedUpdateCount = 0;

        private void InitializeFromStreamer()
        {
            if (SceneStreamerSystem.Streamer != null)
            {
                if ((SceneStreamerSystem.Streamer.Mode == SceneStreamerMode.OptimiseLoadtime) || (SceneStreamerSystem.Streamer.SceneStreamer == null))
                {
                    SetActivateCounts(int.MaxValue, int.MaxValue, int.MaxValue);
                }
                else
                {
                    SetActivateCounts(SceneStreamerSystem.Streamer.SceneStreamer.TerrainActivateCount,
                                      SceneStreamerSystem.Streamer.SceneStreamer.StaticActivateCount,
                                      SceneStreamerSystem.Streamer.SceneStreamer.FxActivateCount);
                }
                TerrainBakerMaterialSupport terrain = null;
                if ( GameObjectsTerrain.Count > 0)
                {
                    terrain = GameObjectsTerrain[0].GetComponent<TerrainBakerMaterialSupport>();
                }
                SceneStreamerSystem.Streamer.ShowBackground(false, gameObject.scene.name, terrain);
            }
        }

        private void DeinitializeFromStreamer()
        {
            if (SceneStreamerSystem.Streamer != null)
            {
                SceneStreamerSystem.Streamer.ShowBackground(true, gameObject.scene.name, null);
            }
        }

        private bool ActivateElements()
        {
            if (somethingToActivate)
            {
                if ((GameObjectsTerrain != null) && (terrainActivateCount > 0))
                {
                    if (terrainActivated < GameObjectsTerrain.Count)
                    {
                        for (int index = 0; index < terrainActivateCount; ++index)
                        {
                            if (GameObjectsTerrain[terrainActivated] != null)
                            {
                                GameObjectsTerrain[terrainActivated].SetActive(true);
                            }
                            ++terrainActivated;
                            if (terrainActivated >= GameObjectsTerrain.Count)
                            {
                                break;
                            }
                        }
                        return true;
                    }
                }

                if ((GameObjectsStatic != null) && (staticActivateCount > 0))
                {
                    if (staticActivated < GameObjectsStatic.Count)
                    {
                        for (int index = 0; index < staticActivateCount; ++index)
                        {
                            if (GameObjectsStatic[staticActivated] != null)
                            {
                                GameObjectsStatic[staticActivated].SetActive(true);
                            }
                            ++staticActivated;
                            if (staticActivated >= GameObjectsStatic.Count)
                            {
                                break;
                            }
                        }
                        return true;
                    }
                }

                if ((GameObjectsFX != null) && (fxActivateCount > 0))
                {
                    if (fxActivated < GameObjectsFX.Count)
                    {
                        for (int index = 0; index < fxActivateCount; ++index)
                        {
                            if (GameObjectsFX[fxActivated] != null)
                            {
                                GameObjectsFX[fxActivated].SetActive(true);
                            }
                            ++fxActivated;
                            if (fxActivated >= GameObjectsFX.Count)
                            {
                                break;
                            }
                        }
                        return true;
                    }
                }
                somethingToActivate = false;
            }
            return false;
        }

        private void TryReportReady(bool fixedUpdate)
        {
            if (!somethingToActivate && !readyReported)
            {
                if (fixedUpdate)
                {
                    readyFixedUpdateCount += 1;
                }
                else
                {
                    readyUpdateCount += 1;
                }
                if ((readyFixedUpdateCount > 1) && (readyUpdateCount > 1))
                {
                    SceneStreamerSystem.Streamer.ReportReady(true, gameObject.scene.name);
                    readyReported = true;
                }
            }
        }

        public void SetActivateCounts(int _terrainActivateCount, int _staticActivateCount, int _fxActivateCount)
        {
            terrainActivateCount = _terrainActivateCount;
            staticActivateCount = _staticActivateCount;
            fxActivateCount = _fxActivateCount;
            SceneStreamerSystem.DebugReport(true)?.Invoke(false, $"SceneLoaderBehaviour.SetActivateCounts(), sceneName: {gameObject.scene.name}, terrainActivateCount: {terrainActivateCount}, staticActivateCount: {staticActivateCount}, fxActivateCount: {fxActivateCount}");
        }

        public void CheckGameObjects(bool showTerrain, bool showFoliage, bool showObjects, bool showEffects)
        {
            if (GameObjectsTerrain != null)
            {
                foreach (var gameObjectTerrain in GameObjectsTerrain)
                {
                    var meshRenderer = gameObjectTerrain.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = showTerrain;
                    }
                    var instanceCompositionRenderer = gameObjectTerrain.GetComponent<InstanceCompositionRenderer>();
                    if (instanceCompositionRenderer != null)
                    {
                        instanceCompositionRenderer.enabled = showFoliage;
                    }
                }
            }
            if (GameObjectsStatic != null)
            {
                foreach (var gameObjectStatic in GameObjectsStatic)
                {
                    var renderers = gameObjectStatic.GetComponentsInChildren<Renderer>();
                    if (renderers != null)
                    {
                        foreach (var renderer in renderers)
                        {
                            if (renderer != null)
                            {
                                renderer.enabled = showObjects;
                            }
                        }
                    }
                }
            }
            if (GameObjectsFX != null)
            {
                foreach (var gameObjectsFX in GameObjectsFX)
                {
                    var renderers = gameObjectsFX.GetComponentsInChildren<Renderer>();
                    if (renderers != null)
                    {
                        foreach (var renderer in renderers)
                        {
                            if (renderer != null)
                            {
                                renderer.enabled = showEffects;
                            }
                        }
                    }
                }
            }
        }

        public void Awake()
        {
            if (!enabled)
            {
                enabled = true;
            }
            InitializeFromStreamer();
            somethingToActivate = (terrainActivateCount > 0) || (staticActivateCount > 0) || (fxActivateCount > 0);
            terrainActivated = 0;
            staticActivated = 0;
            fxActivated = 0;
            readyUpdateCount = 0;
            readyFixedUpdateCount = 0;
            readyReported = false;
            ActivateElements();
        }

        public void OnDestroy()
        {
            DeinitializeFromStreamer();
        }

        public void Update()
        {
            ActivateElements();
            TryReportReady(false);
        }

        public void FixedUpdate()
        {
            TryReportReady(true);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = SceneColor;
            Gizmos.DrawWireCube(SceneStart + SceneSize * 0.5f, SceneSize);
        }
    }
}