using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Editor.SceneProcessors;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Lib.Extensions;
using Assets.Src.Regions.RegionMarkers;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem;
using Assets.Src.RubiconAI;
using Assets.Src.RubiconAI.Groups;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using SharedCode.Aspects.Regions;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.SpatialSystem.Editor;
using Core.Environment.Logging.Extension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Color = UnityEngine.Color;

namespace Assets.Editor
{
	internal class SceneExport : EditorWindow
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		[MenuItem("Level Design/Export Scenes To Jdb")]
		private static void ExportScenesToJdb()
		{
			var sceneCount = SceneManager.sceneCount;
			var guids = new HashSet<Guid>();
			for (var i = 0; i < sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.isLoaded)
				{
					ValidateScene(scene, guids);
					SceneExporter.ExportScene(scene, guids);
				}
			}
		}

		[MenuItem("Level Design/Export Interactive Markers")]
		private static void ExportAABBTriggeredObjects()
		{
			var sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.isLoaded)
				{
					try
					{
						EditorUtility.DisplayProgressBar("Saving non-entity markers", "Assigming stuff", (float) sceneCount / i);
						ExportNonEntityMarkersToHash.ExportMarkers(ref scene);
					}
					catch (Exception e)
					{
						Logger.IfError()?.Exception(e).Write();
						throw;
					}
					finally
					{
						EditorUtility.ClearProgressBar();
					}
				}
			}
		}

		[MenuItem("Tools/RemoveMissingScripts")]
		public static void RemoveMissingScripts()
		{
			EditorUtility.DisplayProgressBar("Removing scripts", "AssetDatabase.FinsAssets", 0.0f);
			try
			{
				var guids = AssetDatabase.FindAssets("t:Prefab");
				EditorUtility.DisplayProgressBar("Removing scripts", "ProcessingAssets", 0.3f);
				int index = 0;
				List<Transform> cmps = new List<Transform>();
				foreach (var guid in guids)
				{
					index++;
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var toCheck = AssetDatabase.LoadAllAssetsAtPath(path);
					bool stillDo = true;
					foreach (var obj in toCheck)
					{
						var go = obj as GameObject;
						if (go == null)
						{
							continue;
						}

						cmps.Clear();
						EditorUtility.DisplayProgressBar("Removing scripts", go.name, 0.3f + 0.7f * index / guids.Length);
						if (ProcessGo(go, path, cmps, index, guids.Length))
						{
							AssetDatabase.Refresh();
							AssetDatabase.SaveAssets();
						}
					}
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}

		private static bool ProcessGo(GameObject go, string path, List<Transform> components, int index, int length)
		{
			bool removed = false;
			if (components != null)
			{
				go.GetComponentsInChildren<Transform>(components);
				foreach (var t in components)
				{
					if (ProcessGo(t.gameObject, null, null, index, length))
						removed = true;
				}
			}

			if (GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go) > 0)
			{
				if (components != null)
				{
					var ngo = PrefabUtility.SaveAsPrefabAsset(go, path, out var succes);
					if (go.GetComponents<Component>().Any(x => x == null))
						Logger.IfError()?.Message($"Can't remove missings from {go.name}").Write();
					if (!succes)
						Logger.IfError()?.Message($"Can't save {go.name}").Write();
					EditorUtility.SetDirty(ngo);
					EditorUtility.DisplayProgressBar("Removing scripts, do", go.name, 0.3f + 0.7f * index / length);
				}

				removed = true;
			}

			return removed;
		}

		[MenuItem("Level Design/Validate scene")]
		private static void ValidateScenes()
		{
			var sceneCount = SceneManager.sceneCount;
			HashSet<Guid> _ids = new HashSet<Guid>();
			for (int i = 0; i < sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.isLoaded)
					ValidateScene(scene, _ids);
			}
		}

		private static void ValidateScene(Scene scene, HashSet<Guid> _ids)
		{
			bool hadSomething = false;
			var spawnDaemons = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SpawnDaemonGroup>());
			foreach (var spawnDaemon in spawnDaemons.Where(s => s.isActiveAndEnabled))
			{
				if (!_ids.Add(spawnDaemon.SpawnDaemonGuid))
				{
					var serializedObject = new SerializedObject(spawnDaemon);
					Logger.IfWarn()?.Message($"Encountered duplicate id {spawnDaemon.name}").Write();
					serializedObject.FindProperty(nameof(spawnDaemon._guid)).stringValue = Guid.NewGuid().ToString();
					serializedObject.ApplyModifiedProperties();
				}

				hadSomething = true;
			}

			var mapObjects = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<MapObject>());
			foreach (var mapObject in mapObjects.Where(s => s.isActiveAndEnabled))
			{
				if (!_ids.Add(mapObject.ObjectGuid))
				{
					var serializedObject = new SerializedObject(mapObject);
					Logger.IfWarn()?.Message($"Encountered duplicate id {mapObject.name}").Write();
					mapObject._guid = Guid.NewGuid().ToString();
					serializedObject.FindProperty(nameof(mapObject._guid)).stringValue = Guid.NewGuid().ToString();
					serializedObject.ApplyModifiedProperties();
				}

				{
					if (mapObject is SpawnDaemonGroup)
						continue;
					var serializedObject = new SerializedObject(mapObject);
					if (mapObject.ObjectToSpawn == null)
					{
						var oldEgo = mapObject.GetComponent<EntityGameObject>();
						if (oldEgo != null)
						{
							Logger.IfWarn()?.Message($"Encountered null objectToSpawn and fixed {mapObject.name}").Write();
							mapObject.ObjectToSpawn = oldEgo.StaticDef;
							serializedObject.FindProperty(nameof(mapObject.ObjectToSpawn)).objectReferenceValue = oldEgo.StaticDef;
						}
						else
							Logger.IfError()?.Message($"Encountered null objectToSpawn and fixed {mapObject.name}").Write();
					}

					serializedObject.ApplyModifiedProperties();
				}
				hadSomething = true;
			}

			if (hadSomething)
				EditorSceneManager.MarkSceneDirty(scene);
		}
	}

	internal class SceneExporter
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public static void ExportScene(Scene scene, HashSet<Guid> guids)
		{
			try
			{
				EditorUtility.DisplayProgressBar("Saving regions", "Assigning stuff", 0.1f);
				SpawnTemplate[] templates = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SpawnTemplate>())
					.Distinct().ToArray();

				EditorUtility.DisplayProgressBar("Saving regions", "Assigning stuff", 0.2f);
				SpawnRegion[] regions = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SpawnRegion>()).Distinct()
					.ToArray();
				PlayerSpawnPoint[] spawnPoints = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<PlayerSpawnPoint>())
					.Distinct().ToArray();

				EditorUtility.DisplayProgressBar("Saving regions and spawn points", "Assigning stuff", 0.5f);
				ExportSceneDef(scene, templates, regions, spawnPoints);
				EditorUtility.DisplayProgressBar("Saving regions and spawn points", "Assigning stuff", 1f);
				ExportRegions(scene, guids);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}

		private static void ExportRegions(Scene scene, HashSet<Guid> guids)
		{
			var rootGameObjects = scene.GetRootGameObjects();

			var spatialObjects = rootGameObjects.SelectMany(x => x.GetComponentsInChildren<SpatialTrigger>()).ToArray();

			var rootRegions = rootGameObjects.Select(x => x.GetComponent<RegionMarker>()).Where(x => x != null).ToArray();
			var rootRegionsCount = rootRegions.Count();

			ARegionDef rootDef;
			if (rootRegionsCount == 0)
			{
				Debug.LogWarning($"No root regions found on scene {scene.name}");
				if (spatialObjects.Length == 0)
					return;

				rootDef = ExportSpatialTriggers(spatialObjects);
				if (rootDef == default)
					return;
			}
			else
			{
				rootDef = new RootRegionDef() {Data = new List<ResourceRef<ARegionDataDef>>().ToArray()};
				var childRegionDefs = new List<ResourceRef<ARegionDef>>(rootRegionsCount);
				foreach (var rootRegion in rootRegions)
				{
					var aRegionDef = rootRegion.BuildDefs();
					childRegionDefs.Add(aRegionDef);
					rootDef.ChildRegions = childRegionDefs.ToArray();
				}

				if (!CheckSavableResources(rootDef, guids))
					return;

				var spatialRoot = ExportSpatialTriggers(spatialObjects);
				if (spatialRoot != default)
				{
					var childRegs = rootDef.ChildRegions.ToList();
					childRegs.Add(spatialRoot);
					rootDef.ChildRegions = childRegs.ToArray();
				}
			}

			var roots = new Stack<GeoRegionRoot>();
			RegionBuildHelper.BuildRegion(rootDef, 0, new Dictionary<ARegionDef, IRegion>(), roots, null);
			GameResources.SimpleSave(GameResourcesHolder.Instance.Deserializer.Loader.GetRoot() + "/ColonyShared/GeneratedCode/Regions/",
				scene.name, rootDef, out string resultPath);
		}
		
		private static bool CheckSavableResources(ARegionDef rootDef, ISet<Guid> guids)
		{
			return CheckSavableResource(rootDef, guids) &&
			       (
				       rootDef.ChildRegions == null ||
				       rootDef.ChildRegions.All(childRegionDef => CheckSavableResources(childRegionDef, guids))
			       ) &&
			       (
				       rootDef.Data == null ||
				       rootDef.Data.All(childRegionDataDef => CheckSavableResource(childRegionDataDef, guids))
			       );
		}

		private static bool CheckSavableResource(BaseResource rootDef, ISet<Guid> guids)
		{
			if (rootDef is ISaveableResource savable && !guids.Add(savable.Id))
			{
				Logger.IfError()?.Message($"Error Encountered Duplicate ID In {rootDef.GetType().Name} id {savable.Id}").Write();
				return false;
			}

			return true;
		}

		private static ARegionDef ExportSpatialTriggers(SpatialTrigger[] spatialTriggers)
		{
			var regionsList = new List<ResourceRef<ARegionDef>>();
			foreach (var spTrigger in spatialTriggers)
			{
				var collider = spTrigger.GetComponent<Collider>();
				if (collider == null)
					throw new NotImplementedException(
						$"Can not export {nameof(SpatialTrigger)} object {spTrigger.gameObject.name} (no '{nameof(Collider)}' component found)");
				var regDef = GeoRegionExporter.GetGeoRegionDefFromGameObject(collider, spTrigger.gameObject);
				var aabbDef = GeoRegionExporter.GetBoundingBoxBasedOnCollider(collider, spTrigger.transform);
				regDef.AABB = aabbDef;
				regDef.ChildRegions = new List<ResourceRef<ARegionDef>>(0).ToArray();

				//if (regDef != default)
				//    collider.enabled = false;

				var data = new List<ResourceRef<ARegionDataDef>>();
				var spRegDef = new SpellCastRegionDef();
				if (spTrigger.OnEnterSpell || spTrigger.OnEnterSpell || spTrigger.WhileInsideSpell)
				{
					if (spTrigger.OnEnterSpell)
						spRegDef.OnEnterSpellDef = new ResourceRef<SpellDef>(spTrigger.OnEnterSpell.Get<SpellDef>());
					if (spTrigger.OnExitSpell)
						spRegDef.OnExitSpellDef = new ResourceRef<SpellDef>(spTrigger.OnExitSpell.Get<SpellDef>());
					if (spTrigger.WhileInsideSpell)
						spRegDef.WhileInsideSpellDef = new ResourceRef<SpellDef>(spTrigger.WhileInsideSpell.Get<SpellDef>());

					data.Add(spRegDef);
					regDef.Data = data.ToArray();
					regionsList.Add(regDef);
				}
				else
				{
					Logger.Error(
						$"Can not export {nameof(SpatialTrigger)} object {spTrigger.gameObject.name} (all spells is null, skipping)");
					continue;
				}
			}

			return regionsList.Count != 0
				? new GeoRegionRootDef {ChildRegions = regionsList.ToArray(), Data = new List<ResourceRef<ARegionDataDef>>().ToArray()}
				: default(ARegionDef);
		}

		private static void ExportSceneDef(Scene scene, SpawnTemplate[] templates, SpawnRegion[] regions, PlayerSpawnPoint[] spawnPoints)
		{
			SceneChunkDef sceneChunk = new SceneChunkDef();

			foreach (var spawnPoint in spawnPoints)
			{
				var position = spawnPoint.gameObject.transform.position.ToShared();
				var rotation = spawnPoint.gameObject.transform.rotation.ToSharedQuaternion();
				var spawnPointType = spawnPoint.PointTypeMetaData?.Get<SpawnPointTypeDef>();
				var name = spawnPointType?.____GetDebugShortName() ?? spawnPoint.gameObject.name;

				sceneChunk.PlayerSpawnPoints.Add(new SpawnPointData(position, rotation, name, spawnPoint.SpawnRadius, spawnPointType));
			}

			var def = new SpawnTemplatesMapDef
			{
				Templates = new List<SpawnTemplateDef>(), SceneName = scene.name
			};
			foreach (var template in templates)
				def.Templates.Add(new SpawnTemplateDef()
				{
					Points = template
						.GetComponentsInChildren<SpawnPoint>()
						.Where(sp => sp.PointType != null)
						.GroupBy(sp => sp.PointType)
						.Select(spG => new SpawnPoints()
						{
							PointType = spG.First().PointType.Get<SpawnPointTypeDef>(),
							Points = spG.Select(sp => new SpawnTemplatePoint()
							{
								SpatialPoint = sp.transform.position.ToSharedVec3(),
								Rotation = sp.transform.rotation.ToSharedQ()
							}).ToArray()
						}).ToList()
				});
			sceneChunk.SpawnTemplates.Add(def);

			var mapObjects = scene
				.GetRootGameObjects()
				.SelectMany(x => x.GetComponentsInChildren<MapObject>());
			foreach (var mapObject in mapObjects.Where(s => s.isActiveAndEnabled))
			{
				IEntityObjectDef objectDef;
				SpawnDaemonSceneDef spawnDaemonDef = null;
				if (mapObject is SpawnDaemonGroup spawnDaemon)
				{
					spawnDaemonDef = new SpawnDaemonSceneDef()
					{
						Goals = spawnDaemon.SpawnDaemonGoals.Get<SpawnDaemonGoalsDef>(),
						SpawnDaemonId = spawnDaemon.SpawnDaemonGuid,
						Name = spawnDaemon._spawnDaemonName,
						Filter = spawnDaemon.Filter,
						Filters = spawnDaemon.Filters
					};
					objectDef = GameResourcesHolder.Instance.LoadResource<SpawnDaemonDef>("/Constants/DefaultSpawnDaemon");
					var camp = spawnDaemon.GetComponent<Camp>();
					if (camp != null)
					{
						int generatedPointsAmount = 10;
						List<SharedCode.Utils.Vector3> pointsList = new List<SharedCode.Utils.Vector3>();
						foreach (var spawnZone in spawnDaemon.GetComponentsInChildren<SpawnZone>()
							.Concat(spawnDaemon.GetComponents<SpawnZone>()))
						{
							int pointsCount = spawnZone.Radius < 5 ? 5 : generatedPointsAmount;
							for (int i = 0; i < pointsCount; i++)
							{
								var hit = spawnZone.FindPlace();
								if (hit.hit)
								{
									pointsList.Add(spawnDaemon.transform.InverseTransformPoint(hit.position).ToShared());
								}
							}
						}

						foreach (var campStatic in camp.GetComponents<CampStatics>())
						{
							var cat = GameResourcesHolder.Instance.LoadResource<KnowledgeCategoryDef>(new ResourceIDFull(campStatic.Name));
							IEnumerable<GameObject> links = new List<GameObject>();
							if (campStatic != null)
								links = campStatic.Links;
							if (campStatic.FoundLinks != null)
								links = links.Concat(campStatic.FoundLinks ?? new List<GameObject>(1));
							var rr = new ResourceRef<KnowledgeCategoryDef>(cat);
							spawnDaemonDef.POIs.TryGetValue(rr, out var poisVal);
							var poisList = links.Where(x => x != null)
								.Where(x => x.GetComponent<MapObject>() && x.GetComponent<MapObject>().TypeId != 0).Select(x =>
									new SavedOuterRef()
									{
										Guid = x.GetComponent<MapObject>().ObjectGuid,
										ObjectType = new ResourceRef<IEntityObjectDef>(x.GetComponent<MapObject>().ObjectToSpawn
											.Get<IEntityObjectDef>())
									}).ToList();
							if (poisVal == null)
								spawnDaemonDef.POIs.Add(rr, poisList);
							else
								poisVal.AddRange(poisList);
						}

						spawnDaemonDef.MobsSpawnPoints = pointsList.ToArray();
					}
				}
				else
				{
					objectDef = mapObject.ObjectToSpawn.Get<IEntityObjectDef>();
				}

				var jdbLocatorPath = mapObject.JdbLocator?.RootPath;
				var scenicEntityDef = new ScenicEntityDef()
				{
					RefId = mapObject.ObjectGuid,
					Position = mapObject.transform.position.ToShared(),
					Rotation = mapObject.transform.rotation.ToSharedQ(),
					TimeToRespawn = mapObject.TimeToRespawn,
					Object = new ResourceRef<IEntityObjectDef>(objectDef),
					SpawnDaemonSceneDef = spawnDaemonDef,
					JdbLocator = jdbLocatorPath != null ? new StubResource(jdbLocatorPath) : null
				};
				var linksForLinksEngine = mapObject.GetComponents<Links>();
				foreach (var link in linksForLinksEngine)
				{
					var linkType = new LinkTypeDef();
					((IResource) linkType).Address = new ResourceIDFull(link.LinkType.RootPath);
					scenicEntityDef.LinksToStatics.Add(linkType, new List<Guid>(link.MapObjects.Select(x => x.ObjectGuid)));
				}

				sceneChunk.Entities.Add(scenicEntityDef);
			}

			if (sceneChunk.Entities.Count == 0 && sceneChunk.PlayerSpawnPoints.Count == 0 && sceneChunk.SpawnTemplates.Count == 0)
				GameResourcesLikeFileSaver.Clear(Application.dataPath + "/SpawnSystemData/" + scene.name + "/", scene.name, sceneChunk);
			else
			{
				var oldSceneChunk = EditorGameResourcesForMonoBehaviours.NewGR()
					.TryLoadResource<SceneChunkDef>("/SpawnSystemData/" + scene.name + "/" + scene.name);
				if (oldSceneChunk != null)
					sceneChunk.Id = oldSceneChunk.Id != Guid.Empty ? oldSceneChunk.Id : Guid.NewGuid();
				else
					sceneChunk.Id = Guid.NewGuid();

				GameResourcesLikeFileSaver.SaveFile(Application.dataPath + "/SpawnSystemData/" + scene.name + "/", scene.name, sceneChunk);
			}
		}

		[MenuItem("Tools/Test/Export Test FogOfWar Texture")]
		private static void ExportTestFogOfWar()
		{
			const string path = "Assets/Scenes/_TestScenes/Test_FogOfWar/Test_obj_FogOfWar.256x128.png";

			const int count = 256;
			var texture = new Texture2D(count, count);
			for (var y = 0; y < 128; y++)
			for (var x = 0; x < count; x++)
				texture.SetPixel(x, y, new Color((float) x / count, (float) y / count, 0, 1f));
			texture.Apply();
			// const string path = "Assets/Content/RegionsData/FogOfWar/Savannah_FogOfWar.png";
			File.Delete(path);
			File.WriteAllBytes(path, texture.EncodeToPNG());
			AssetDatabase.ImportAsset(path);

			SetIndexTextureParams(path);
		}

		private static void SetIndexTextureParams(string path)
		{
			var importer = (TextureImporter) AssetImporter.GetAtPath(path);
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.alphaSource = TextureImporterAlphaSource.None;
			importer.filterMode = FilterMode.Point;
			importer.mipmapEnabled = false;
			importer.isReadable = true;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			var platformSettings = importer.GetDefaultPlatformTextureSettings();
			platformSettings.format = TextureImporterFormat.RGB24;
			importer.SetPlatformTextureSettings(platformSettings);
			EditorUtility.SetDirty(importer);
			importer.SaveAndReimport();
		}
		
        [MenuItem("Tools/Test/Export Test FogOfWar Index Texture")]
        private static void ExportTestIndex()
        {
            EditorUtility.DisplayProgressBar("Exporting Test Index Texture", "...", 0);

            const string path = "Assets/Scenes/_TestScenes/Test_FogOfWar/Test_obj_FogOfWar.png";
            const string outPath = "Assets/Scenes/_TestScenes/Test_FogOfWar/Test_obj_FogOfWar.index.R16.bytes";
            var bytes = File.ReadAllBytes(path);

            var maxValue = short.MaxValue;
            var source = new Texture2D(1, 1, TextureFormat.R16, false);
            source.LoadImage(bytes);
            var pixels = source.GetPixels32();
            var indexes = SVOExporter.CreateIndexes<Color32, short>(pixels);
            var indexPixels = pixels.Select(color32 => new Color((float) indexes[color32] / maxValue, 0, 0)).ToArray();
            var texture = new Texture2D(source.width, source.height, TextureFormat.R16, false, true);
            texture.SetPixels(indexPixels);
            texture.Apply();

            //Test Read Written
            var indexPixels2 = texture.GetPixels();
            for (var i = 0; i < indexPixels2.Length; i++)
                if ((short) Mathf.RoundToInt(indexPixels2[i].r * maxValue) != indexes[pixels[i]])
                    Debug.LogError($"Error Set Pixel {i} {indexPixels2[i].r * maxValue} {indexes[pixels[i]]}");

            var encoded = texture.GetRawTextureData();

            //Test Encode
            texture = new Texture2D(1, 1, TextureFormat.R16, false, true);
            texture.LoadRawTextureData(encoded);
            var indexPixels3 = texture.GetPixels();
            for (var i = 0; i < indexPixels3.Length; i++)
                if ((short) Mathf.RoundToInt(indexPixels3[i].r * maxValue) != indexes[pixels[i]])
                    Debug.LogError($"Error Encoded Pixel {i} {indexPixels2[i].r * maxValue} {indexes[pixels[i]]}");

            File.Delete(outPath);
            File.WriteAllBytes(outPath, encoded);
            AssetDatabase.ImportAsset(outPath);

            EditorUtility.ClearProgressBar();
        }
	}
}