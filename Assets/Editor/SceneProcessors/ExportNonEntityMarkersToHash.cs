using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.Aspects.SpatialObjects;
using Assets.Src.Aspects.VisualMarkers;
using Assets.Src.Cartographer;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Item.Templates;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SVector3 = SharedCode.Utils.Vector3;
using SVector3Int = SharedCode.Utils.Vector3Int;

namespace Assets.Editor.SceneProcessors
{
    public class ExportNonEntityMarkersToHash : IProcessSceneWithReport
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const float maxCellsInLength = 2;

        public int callbackOrder => 0;

        public static WorldConstantsResource Constants => EditorGameResourcesForMonoBehaviours
            .GetNew()
            .LoadResource<WorldConstantsResource>(SharedCode.Aspects.Item.Templates.Constants.WorldConstantsPath);

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // report is null in editor playmode
            //if (report == null)
            //{
            //    Logger.IfDebug()?.Message("Skipping export of non-entity markers for scene {0}, playmode detected", scene.name).Write();
            //    return;
            //}

            ExportMarkers(ref scene);
        }

        public static bool ExportMarkers(ref Scene scene)
        {
            var spatialTriggeredObjects = scene.CollectComponents<IAABBTriggered>();
            if (!spatialTriggeredObjects.Any())
                return true;

            var interactiveObjectsIndex = scene.CollectComponents<IndexOfNoEntityInteractiveObjects>(includeInactive: false).SingleOrDefault();
            if (interactiveObjectsIndex == default)
            {
                var sceneLoader = scene.CollectGameObjectsWithComponent<SceneLoaderBehaviour>().FirstOrDefault();
                if (sceneLoader == default)
                {
                    Logger.Debug("Skipping export of non-entity markers for scene {0}, no {1} and {2} found", scene.name,
                        nameof(IndexOfNoEntityInteractiveObjects), nameof(SceneLoaderBehaviour));
                    return false;
                }

                interactiveObjectsIndex = sceneLoader.AddComponent<IndexOfNoEntityInteractiveObjects>();
            }

            var diameter = (int)(Constants.SelectionRadius * 2);
            if (diameter == 0)
            {
                Logger.IfError()?.Message($"Raduis of selection is set to zero. Check '{SharedCode.Aspects.Item.Templates.Constants.WorldConstantsPath}'").Write();
                return false;
            }

            var cellSizes = new SVector3Int(diameter, diameter, diameter);
            List<NoEntityIndexItem> objectList = new List<NoEntityIndexItem>();

            foreach (var spatialTriggeredObject in spatialTriggeredObjects)
            {
                var component = spatialTriggeredObject as MonoBehaviour;
                var colliders = component.GetComponents<Collider>();
                if (!colliders.Any())
                {
                    var cell = new CellVector3((SVector3)component.transform.position, cellSizes);
                    objectList.Add(new NoEntityIndexItem { GameObject = component, CellMin = cell, CellMax = cell });
                }
                else
                {
                    Bounds bounds = new Bounds();
                    bounds.center = colliders[0].bounds.center;
                    bounds.size = colliders[0].bounds.size;
                    for (int i = 1; i < colliders.Length; i++)
                    {
                        bounds.Encapsulate(colliders[i].bounds.min);
                        bounds.Encapsulate(colliders[i].bounds.max);
                    }

                    var sizeInCells = bounds.size / diameter;
                    if ((sizeInCells.x > maxCellsInLength || sizeInCells.y > maxCellsInLength || sizeInCells.z > maxCellsInLength) &&
                        (spatialTriggeredObject is VisualMarkerNoEntity visualMarkerNoEntity) && visualMarkerNoEntity.HasPoint)
                    {
                        Logger.IfWarn()?.Message($"Object {component} is too large for marker(dimensions in cells: '{sizeInCells.x}) * {sizeInCells.y} * {sizeInCells.z}')").Write();
                        continue;
                    }

                    var nearestToOriginCorner = new CellVector3((SVector3)bounds.min, cellSizes);
                    var farthestFromOriginCorner = new CellVector3((SVector3)bounds.max, cellSizes);
                    objectList.Add(new NoEntityIndexItem { GameObject = component, CellMin = nearestToOriginCorner, CellMax = farthestFromOriginCorner });
                }
            }

            interactiveObjectsIndex.Set(objectList, diameter);
            return true;
        }
    }
}