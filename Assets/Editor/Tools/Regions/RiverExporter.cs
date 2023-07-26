using Assets.Editor.Tools.SplineFlatteners;
using Assets.Plugins.River_Auto_Material.Spline_System.Scripts;
using Assets.ResourceSystem.Aspects.Misc;
using Assets.ResourceSystemLoader.ResourcesSystem.Utils;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Aspects.VisualMarkers;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using Assets.Src.Shared;
using SharedCode.Aspects.Item.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Uins;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor.Tools.Regions
{
    public class RiverExporter : EditorWindow
    {
        // comments started with ### and ended with ###/ indicates blocks of code copied directly from RamSpline (with minor changes) to avoid any inconsistencies
        // revise this code if RAM River is updated

        private const string settingsPath = "/UtilPrefabs/Environment/RiverExportInteractionSettings";
        private const string materialSettingsPath = "Assets/UtilPrefabs/Environment/RiverMaterialToInteractionType.asset";


        private static float tolerance = 5;
        private static float height = 3;
        private static float widthMultiplier = 0.75f;

        private static RiverExportSettingsDef _riverSettings;
        private static Dictionary<Material, RiverInteractionType> _materialToInteraction;

        private static void GetSettings()
        {
            _riverSettings = EditorGameResourcesForMonoBehaviours.GetNew().LoadResource<RiverExportSettingsDef>(settingsPath);
            var matToInteractAsset = AssetDatabase.LoadAssetAtPath<RiverExportMaterialSettings>(materialSettingsPath);
            if (matToInteractAsset == null)
                Debug.LogError($"No settings found at path '{materialSettingsPath}'");
            _materialToInteraction = matToInteractAsset.MaterialToInteractionType.ToDictionary(k => k.Material, v => v.Interaction);
        }

        private static GameObject CreateObjectForBox(BoxForCollider boxForCollider, GameObject parentGameObject)
        {
            var gameObject = new GameObject("RiverCollider");
            gameObject.layer = PhysicsLayers.Interactive;
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            var transform = gameObject.transform;
            transform.parent = parentGameObject.transform;
            transform.position = boxForCollider.position;
            transform.rotation = boxForCollider.rotation;
            transform.localScale = boxForCollider.scale;
            return gameObject;
        }

        private static RiverInteractionType GetInteractionTypeBasedOnMaterial(Component water)
        {
            var material = water.GetComponent<MeshRenderer>()?.sharedMaterial;
            if (material == default)
            {
                Debug.Log($"No material set for object '{water.gameObject}'", water.gameObject);
                return RiverInteractionType.None;
            }
            if (_materialToInteraction.TryGetValue(material, out var interactionType))
            {
                return interactionType;
            }
            else
            {
                Debug.LogWarning($"No interaction type found in settings for object '{water.gameObject}', material : '{material}'", water.gameObject);
                return RiverInteractionType.None;
            }
        }

        private static void AddInteractiveComponents(RiverInteractionType interactionType, GameObject gameObject)
        {
            RiverSettingsDef riverSettings = default;
            switch (interactionType)
            {
                case RiverInteractionType.Clear:
                    riverSettings = _riverSettings.ClearRiverSettings.Target;
                    break;
                case RiverInteractionType.Toxic:
                    riverSettings = _riverSettings.ToxicRiverSettings.Target;
                    break;
                case RiverInteractionType.None:
                default:
                    return;
            }

            var interactive = gameObject.AddComponent<Interactive>();
            interactive.InteractionType = InteractionType.CollectLow;
            interactive.KnowType = InteractionType.CollectLow;
            interactive.LocalObjectDefRef = new NonEntityObjectDefRef() { Metadata = GetMetadata(riverSettings.InteractionObject.Target) };
            var spatialTrigger = gameObject.AddComponent<SpatialTrigger>();
            spatialTrigger.WhileInsideSpell = GetMetadata(riverSettings.SpatialSpellForFlask.Target);
            var badgePoint = gameObject.AddComponent<InteractiveBadgePoint>();
            badgePoint.SetPrefab(_riverSettings.GuiBadgePrefab?.Target?.GetComponent<GuiBadge>());
            var marker = gameObject.AddComponent<VisualMarkerNoEntity>();
            marker.HasPoint = false;
            marker.IsMoving = false;
            marker.SetPredicateIgnoreGroupMetadata(GetMetadata(riverSettings.PredicateIgnoreGroup.Target));
        }

        private static JdbMetadata GetMetadata(BaseResource resource)
        {
            return AssetDatabase.LoadAssetAtPath<JdbMetadata>("Assets" + resource.____GetDebugAddress() + ".jdb");
        }
        public static void ExportRivers(float tolerance)
        {
            GetSettings();

            var scenes = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(sceneIndex => SceneManager.GetSceneAt(sceneIndex))
                .Where(scene => scene.isLoaded);
            foreach (var scene in scenes)
            {
                var ramSplines = scene.GetRootGameObjects()
                        .SelectMany(rootGameObject => rootGameObject.GetComponentsInChildren<RamSpline>());

                if (ramSplines.Any())
                {
                    var splines = new List<SplineWithInteraction>();
                    foreach (var ramSpline in ramSplines)
                    {
                        var splineOffset = ramSpline.transform.position;
                        // ###
                        List<Vector4> pointsChecked = new List<Vector4>();
                        for (int i = 0; i < ramSpline.controlPoints.Count; i++)
                        {
                            if (i > 0)
                            {
                                if (Vector3.Distance(ramSpline.controlPoints[i], ramSpline.controlPoints[i - 1]) > 0)
                                    pointsChecked.Add(ramSpline.controlPoints[i]);
                            }
                            else
                                pointsChecked.Add(ramSpline.controlPoints[i]);
                        }

                        var splineSections = new List<CatmullRomSection>();
                        for (int i = 0; i < pointsChecked.Count; i++)
                        {
                            if (i > pointsChecked.Count - 2)
                                break;
                            var pos = i;
                            Vector4 p0 = ramSpline.controlPoints[pos];
                            Vector4 p1 = ramSpline.controlPoints[pos];
                            Vector4 p2 = ramSpline.controlPoints[ramSpline.ClampListPos(pos + 1)];
                            Vector4 p3 = ramSpline.controlPoints[ramSpline.ClampListPos(pos + 1)];

                            if (pos > 0)
                                p0 = ramSpline.controlPoints[ramSpline.ClampListPos(pos - 1)];

                            if (pos < ramSpline.controlPoints.Count - 2)
                                p3 = ramSpline.controlPoints[ramSpline.ClampListPos(pos + 2)];
                            // ###/
                            var point1 = (Vector3)p0 + splineOffset;
                            var point2 = (Vector3)p1 + splineOffset;
                            var point3 = (Vector3)p2 + splineOffset;
                            var point4 = (Vector3)p3 + splineOffset;
                            var splineSection = new CatmullRomSection(point1, point2, point3, point4, p1.w * widthMultiplier, p2.w * widthMultiplier);
                            splineSections.Add(splineSection);
                        }
                        if (splineSections.Count != 0)
                        {
                            splines.Add(new SplineWithInteraction() { SplineDividedBySections = splineSections, InteractionType = GetInteractionTypeBasedOnMaterial(ramSpline) });
                        }
                    }
                    var root = new GameObject("AutogeneratedRiverColliders");
                    SceneManager.MoveGameObjectToScene(root, scene);
                    EditorSceneManager.MarkSceneDirty(scene);
                    ExportCatmullRomSplinesToColliders(splines, tolerance, root);
                }

                var planes = scene.GetRootGameObjects()
                        .SelectMany(rootGameObject => rootGameObject.GetComponentsInChildren<RamPlane>());
                if (planes.Any())
                {
                    var root = new GameObject("AutogeneratedPlaneColliders");
                    SceneManager.MoveGameObjectToScene(root, scene);
                    EditorSceneManager.MarkSceneDirty(scene);
                    foreach (var plane in planes)
                    {
                        var interactObject = new GameObject("PlaneCollider");
                        interactObject.layer = PhysicsLayers.Interactive;
                        interactObject.transform.parent = root.transform;
                        interactObject.transform.position = plane.transform.position - plane.transform.rotation * (Vector3.up * (plane.depthInMeters / 2));
                        interactObject.transform.rotation = plane.transform.rotation;
                        interactObject.transform.localScale = new Vector3(plane.transform.lossyScale.x * 10f, plane.depthInMeters, plane.transform.lossyScale.z * 10f); // 10 for correct plane scale
                        var collider = interactObject.AddComponent<BoxCollider>();
                        collider.isTrigger = true;
                        AddInteractiveComponents(GetInteractionTypeBasedOnMaterial(plane), interactObject);

                    }
                }
            }
        }
        private static void ExportCatmullRomSplinesToColliders(List<SplineWithInteraction> splines, float tolerance, GameObject parentGameObject)
        {
            foreach (var spline in splines)
            {
                var flattenedSpline = CatmullRomSplineFlattener.FlattenSpline(spline.SplineDividedBySections, tolerance).SelectMany(x => x).ToArray();
                if (flattenedSpline.Length > 1)
                    for (int i = 0; i < flattenedSpline.Length - 1; i++)
                        AdjustLineLengths(ref flattenedSpline[i], ref flattenedSpline[i + 1]);
                foreach (var line in flattenedSpline)
                    AddInteractiveComponents(spline.InteractionType, CreateObjectForBox(CreateBoxColliderFromLine(line), parentGameObject));
            }
        }
        private static void AdjustLineLengths(ref LineSectionWithWidth line, ref LineSectionWithWidth nextLine)
        // adjust each of line lengths so outer box corners (defined by parallel translation by line width of our line section) will roughly reach an outer angle bisector between lines
        {
            if (line.end != nextLine.start)
                throw new ArgumentException("Line should start at the end of the previous line", nameof(nextLine));

            // do nothing because lines are coincident
            if (line.start == nextLine.end)
                return;

            var line1Direction = line.start - line.end;
            var line2Direction = nextLine.end - nextLine.start;

            var angleBetweenLines = Mathf.Acos(Vector3.Dot(line1Direction, line2Direction) / (line1Direction.magnitude * line2Direction.magnitude));
            var angleBetweenNormalAndBisector = Mathf.PI * 0.5f - (angleBetweenLines / 2);
            var prolongation = Mathf.Tan(angleBetweenNormalAndBisector) * line.width * 0.5f;
            line.end = line.end - line1Direction.normalized * prolongation;
            nextLine.start = nextLine.start - line2Direction.normalized * prolongation;
        }

        private static BoxForCollider CreateBoxColliderFromLine(LineSectionWithWidth line)
        {
            var position = (line.end + line.start) * 0.5f;
            position.y -= height * 0.5f;
            var direction = line.end - line.start;
            // Z-axis length
            var len = direction.magnitude;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            var scale = new Vector3(line.width, height, len);
            return new BoxForCollider() { position = position, rotation = rotation, scale = scale };
        }

        [MenuItem("Level Design/Generate river colliders")]
        public static void MenuItem()
        {
            var window = GetWindow<RiverExporter>(true, "Generate river colliders");
            var minsize = window.minSize;
            minsize.y = 88;
            window.minSize = minsize;
            var pos = window.position;
            pos.height = window.minSize.y;
            window.position = pos;
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    tolerance = EditorGUILayout.FloatField("Tolerance:", tolerance);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    height = EditorGUILayout.FloatField("Height:", height);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    widthMultiplier = EditorGUILayout.FloatField("Width multiplier:", widthMultiplier);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Export"))
                        ExportRivers(tolerance);
                }
            }
        }

        private class SplineWithInteraction
        {
            public List<CatmullRomSection> SplineDividedBySections;
            public RiverInteractionType InteractionType;
        }

        private struct BoxForCollider
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
        }
    }
}
