using System;
using System.Collections.Generic;
using AwesomeTechnologies.Billboards;
using AwesomeTechnologies.Colliders;
using AwesomeTechnologies.TouchReact;
using AwesomeTechnologies.Utility.Quadtree;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine;

namespace AwesomeTechnologies.VegetationStudio
{
    [HelpURL("http://www.awesometech.no/index.php/vegetation-studio-manager")]
    [ExecuteInEditMode]
    public partial class VegetationStudioManager
    {
        public static VegetationStudioManager Instance;
        [NonSerialized]
        public List<VegetationSystem> VegetationSystemList = new List<VegetationSystem>();
        [NonSerialized]
        public List<TerrainSystem> TerrainSystemList = new List<TerrainSystem>();
        public delegate void MultiAddVegetationSystemDelegate(VegetationSystem vegetationSystem);
        public MultiAddVegetationSystemDelegate OnAddVegetationSystemDelegate;
        public delegate void MultiRemoveVegetationSystemDelegate(VegetationSystem vegetationSystem);
        public MultiRemoveVegetationSystemDelegate OnRemoveVegetationSystemDelegate;

        [NonSerialized]
        private VegetationItemInfo _clippboardvegetationItemInfo;

        [NonSerialized]
        private AnimationCurve _clippboardAnimationCurve;

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            if (Instance == null) Instance = this;                       
            
            VegetationSystemList.Clear();
            TerrainSystemList.Clear();
            //itemIcons = new List<ItemIcons>();

            //LoadFromContextPreset(context);
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            throw new InvalidOperationException("This should not be active in all sorts of play mode.");
        }

        [ContextMenu("Refresh All")]
        public void RefreshAll()
        {
            for (int i=0; i< VegetationSystemList[0].currentVegetationPackage.VegetationInfoList.Count; i++)
            {
                VegetationSystemList[0].GetModelData(i);
            }
        }

        public static void AddVegetationStudioManagerToScene()
        {
            VegetationStudioManager vegetationStudioManager = FindObjectOfType<VegetationStudioManager>();
            if (vegetationStudioManager == null)
            {
                GameObject go = new GameObject {name = "VegetationStudio"};
                go.AddComponent<VegetationStudioManager>();
                GameObject touchReactSystem = new GameObject {name = "TouchReactSystem"};
                touchReactSystem.transform.SetParent(go.transform);
#if TOUCH_REACT
                touchReactSystem.AddComponent<TouchReactSystem>();
#endif
            }
        }

        /// <summary>
        /// This will add a new vegetation system object to the provided terrain. It can also create a run-time persistentvegetationStorage package
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="vegetationPackage"></param>
        /// <param name="createPersistentVegetationStoragePackage"></param>
        /// <returns></returns>
        public static VegetationSystem AddVegetationSystemToTerrain(Terrain terrain, VegetationPackage vegetationPackage, bool createPersistentVegetationStoragePackage = false)
        {
            GameObject vegetationSystemObject = new GameObject { name = "VegetationSystem" };
            vegetationSystemObject.transform.SetParent(terrain.transform);
            VegetationSystem vegetationSystem = vegetationSystemObject.AddComponent<VegetationSystem>();
            if (vegetationPackage != null)
            {
                vegetationSystem.AddVegetationPackage(vegetationPackage, true);
            }

            vegetationSystemObject.AddComponent<TerrainSystem>();
            BillboardSystem billboardSystem = vegetationSystemObject.AddComponent<BillboardSystem>();
            billboardSystem.VegetationSystem = vegetationSystem;
            vegetationSystemObject.AddComponent<ColliderSystem>();
#if PERSISTENT_VEGETATION
            PersistentVegetationStorage persistentVegetationStorage = vegetationSystemObject.AddComponent<PersistentVegetationStorage>();
            persistentVegetationStorage.VegetationSystem = vegetationSystem;

            if (createPersistentVegetationStoragePackage)
            {
                PersistentVegetationStoragePackage persistentVegetationStoragePackage = ScriptableObject.CreateInstance<PersistentVegetationStoragePackage>();
                persistentVegetationStorage.PersistentVegetationStoragePackage = persistentVegetationStoragePackage;
                persistentVegetationStorage.AutoInitPersistentVegetationStoragePackage = true;
            }
#endif
            vegetationSystem.AutoselectTerrain = false;
            vegetationSystem.currentTerrain = terrain;

            

            vegetationSystem.SetSleepMode(false);

            return vegetationSystem;
        }

        /// <summary>
        /// Internal function used by VegetationSystem components to register with the manager
        /// </summary>
        /// <param name="vegetationSystem"></param>
        public void Instance_RegisterVegetationSystem(VegetationSystem vegetationSystem)
        {
            if (!VegetationSystemList.Contains(vegetationSystem))
            {
                VegetationSystemList.Add(vegetationSystem);
                OnAddVegetationSystem(vegetationSystem);
                if (OnAddVegetationSystemDelegate != null) OnAddVegetationSystemDelegate(vegetationSystem);
            }
        }

        /// <summary>
        /// Internal function to test for sleeping vegetationSystem components
        /// </summary>
        /// <returns></returns>
        public bool HasSleepingVegetationSystems()
        {
            for (int i = 0; i <= VegetationSystemList.Count - 1; i++)
            {
                if (VegetationSystemList[i].GetSleepMode()) return true;
            }

            return false;
        }

        /// <summary>
        /// this will wake up all sleeping vegetationSystem components
        /// </summary>
        public void WakeUpVegetationSystems()
        {
            for (int i = 0; i <= VegetationSystemList.Count - 1; i++)
            {
                VegetationSystemList[i].SetSleepMode(false);
            }
        }

        /// <summary>
        ///  Internal function used by VegetationSystem components to register with the manager
        /// </summary>
        /// <param name="vegetationSystem"></param>
        public static void RegisterVegetationSystem(VegetationSystem vegetationSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                Instance.Instance_RegisterVegetationSystem(vegetationSystem);
            }
        }


        /// <summary>
        /// Static function to find the singelton instance
        /// </summary>
        protected static void FindInstance()
        {
            Instance = (VegetationStudioManager)FindObjectOfType(typeof(VegetationStudioManager));
        }


        /// <summary>
        /// This will enable touch react on all Grass materials in all vegetation system components.
        /// </summary>
        /// <param name="value"></param>
        public static void EnableTouchReact(bool value)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].EnableTouchReact(value);
                }
            }
        }

        /// <summary>
        /// Internal function on the instance to unregister vegetation system components. 
        /// </summary>
        /// <param name="vegetationSystem"></param>
        public void Instance_UnregisterVegetationSystem(VegetationSystem vegetationSystem)
        {
            VegetationSystemList.Remove(vegetationSystem);
            OnRemoveVegetationSystem(vegetationSystem);
            if (OnRemoveVegetationSystemDelegate != null) OnRemoveVegetationSystemDelegate(vegetationSystem);
        }


        /// <summary>
        ///  Internal function used by VegetationSystem components to unregister with the manager
        /// </summary>
        /// <param name="vegetationSystem"></param>
        public static void UnregisterVegetationSystem(VegetationSystem vegetationSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                Instance.Instance_UnregisterVegetationSystem(vegetationSystem);
            }
        }

        /// <summary>
        /// Internal function on the instance to register terrain system components. 
        /// </summary>
        /// <param name="terrainSystem"></param>
        protected void Instance_RegisterTerrainSystem(TerrainSystem terrainSystem)
        {
            TerrainSystemList.Add(terrainSystem);
        }

        /// <summary>
        /// Internal function on the instance to unregister terrain system components. 
        /// </summary>
        /// <param name="terrainSystem"></param>
        protected void Instance_UnregisterTerrainSystem(TerrainSystem terrainSystem)
        {
            TerrainSystemList.Remove(terrainSystem);
        }

        /// <summary>
        ///  Internal function used by TerrainSystem components to register with the manager
        /// </summary>
        /// <param name="terrainSystem"></param>
        public static void RegisterTerrainSystem(TerrainSystem terrainSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                Instance.Instance_RegisterTerrainSystem(terrainSystem);
            }
        }

        /// <summary>
        ///  Internal function used by TerrainSystem components to unregister with the manager
        /// </summary>
        /// <param name="terrainSystem"></param>
        public static void UnregisterTerrainSystem(TerrainSystem terrainSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                Instance.Instance_UnregisterTerrainSystem(terrainSystem);
            }
        }

        public static void GenerateTerrainSplatMap(Bounds bounds, bool clearAllLayers)
        {
            GenerateTerrainSplatMap(bounds, true, clearAllLayers);
        }

        public static void GenerateTerrainSplatMap(bool clearAllLayers)
        {
            GenerateTerrainSplatMap(new Bounds(), false, clearAllLayers);
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        private static void GenerateTerrainSplatMap(Bounds bounds, bool useBounds, bool clearAllLayers = false)
        {
            if (!Instance) FindInstance();

            if (!Instance) return;
            for (int i = 0; i <= Instance.TerrainSystemList.Count - 1; i++)
            {
                Instance.TerrainSystemList[i].GenerateTerrainSplatMap(bounds, useBounds, clearAllLayers);
            }
        }


        public static void RefreshTerrainSplatMap(Bounds bounds)
        {
            RefreshTerrainSplatMap(bounds, true);
        }

        public static void RefreshTerrainSplatMap()
        {
            RefreshTerrainSplatMap(new Bounds(), false);
        }

        static void RefreshTerrainSplatMap(Bounds bounds, bool useBounds)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].RefreshSplatMap(bounds, useBounds);
                }
            }
        }

        public static void RefreshTerrainSplatMap(Terrain terrain)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if (Instance.VegetationSystemList[i].currentTerrain == terrain)
                    {
                        Instance.VegetationSystemList[i].RefreshSplatMap(new Bounds(), false);
                    }
                }
            }
        }

        public static void RefreshTerrainHeightMap(Bounds bounds)
        {
            RefreshTerrainHeightMap(bounds, true);
        }

        /// <summary>
        /// This will refresh the heightmap of the VegetationSystem. Use when heighmap has changed on the TerrainData object. 
        /// </summary>
        public static void RefreshTerrainHeightMap()
        {
            RefreshTerrainHeightMap(new Bounds(), false);
        }

        /// <summary>
        /// This will refresh the heightmap of the VegetationSystem. Use when heighmap has changed on the TerrainData object. Bounds sets area changed. 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="useBounds"></param>
        public static void RefreshTerrainHeightMap(Bounds bounds, bool useBounds)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i]
                        .RefreshHeightMap(
                            useBounds ? bounds : Instance.VegetationSystemList[i].UnityTerrainData.TerrainBounds,
                            false,
                            true);
                }
            }
        }

        /// <summary>
        /// This will refresh all VegetationSystem components using the terrain
        /// </summary>
        /// <param name="terrain"></param>
        public static void RefreshTerrainHeightMap(Terrain terrain)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if (Instance.VegetationSystemList[i].currentTerrain == terrain)
                    {
                        Instance.VegetationSystemList[i]
                            .RefreshHeightMap(
                                Instance.VegetationSystemList[i].UnityTerrainData.TerrainBounds, false, true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a TerrainSystemComponent using a Terrain
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public TerrainSystem GetTerrainSystemFromTerrain(Terrain terrain)
        {
            for (int i = 0; i <= TerrainSystemList.Count - 1; i++)
            {
                if (TerrainSystemList[i].VegetationSystem.UnityTerrainData.terrain == terrain)
                {
                    return TerrainSystemList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a VegetationSystem Component using a Terrain
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public VegetationSystem GetVegetationSystemFromTerrain(Terrain terrain)
        {
            for (int i = 0; i <= VegetationSystemList.Count - 1; i++)
            {
                if (VegetationSystemList[i].UnityTerrainData.terrain == terrain)
                {
                    return VegetationSystemList[i];
                }
            }
            return null;
        }


        /// <summary>
        /// Sets the plant density on all VegetationSystem components in the scene. 
        /// </summary>
        /// <param name="density"></param>
        public static void SetGrassDensity(float density)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetGrassDensity(density);
                }
            }
        }
        /// <summary>
        /// Sets the plant density on all VegetationSystem components in the scene. 
        /// </summary>
        /// <param name="density"></param>
        public static void SetPlantDensity(float density)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetPlantDensity(density);
                }
            }
        }
        /// <summary>
        /// Sets the tree density on all VegetationSystem components in the scene. 
        /// </summary>
        /// <param name="density"></param>
        public static void SetTreeDensity(float density)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetTreeDensity(density);
                }
            }
        }

        /// <summary>
        /// Sets the object density on all VegetationSystem components in the scene. 
        /// </summary>
        /// <param name="density"></param>
        public static void SetObjectDensity(float density)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetObjectDensity(density);
                }
            }
        }

        /// <summary>
        /// Sets the large object density on all VegetationSystem components in the scene. 
        /// </summary>
        /// <param name="density"></param>
        public static void SetLargeObjectDensity(float density)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetLargeObjectDensity(density);
                }
            }
        }

        /// <summary>
        /// Sets the sun directional light on all vegetation systems
        /// </summary>
        /// <param name="sunDirectionalLight"></param>
        public static void SetSunDirectionalLight(Light sunDirectionalLight)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SunDirectionalLight = sunDirectionalLight;
                }
            }
        }


        /// <summary>
        /// This will refresh all materials for the VegetationItems on all vegetation systems. 
        /// </summary>
        public static void RefreshVegetationModelInfoMaterials()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].RefreshVegetationModelInfoMaterials();
                }
            }
        }

        protected void Internal_AddVegetationItemToClipboard(VegetationItemInfo vegetationItemInfo)
        {

            _clippboardvegetationItemInfo = new VegetationItemInfo(vegetationItemInfo);
        }

        private VegetationItemInfo Internal_GetVegetationItemFromClipboard()
        {
            return _clippboardvegetationItemInfo;
        }

        /// <summary>
        /// Adds a new VegetationItemInfo to the Clippboard. Used for copy paste in the VegetationSystem Inspector
        /// </summary>
        /// <param name="vegetationItemInfo"></param>
        public static void AddVegetationItemToClipboard(VegetationItemInfo vegetationItemInfo)
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                Instance.Internal_AddVegetationItemToClipboard(vegetationItemInfo);
            }
        }

        public static void AddAnimationCurveToClipboard(AnimationCurve animationCurve)
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                Instance.Internal_AddAnimationCurveToClipboard(animationCurve);
            }
        }

        private void Internal_AddAnimationCurveToClipboard(AnimationCurve animationCurve)
        {
            _clippboardAnimationCurve = animationCurve;
        }

        public static AnimationCurve GetAnimationCurveFromClippboard()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                return Instance.Internal_GetAnimationCurveFromClippboard();
            }

            return null;
        }

        public AnimationCurve Internal_GetAnimationCurveFromClippboard()
        {
            return _clippboardAnimationCurve;
        }


        /// <summary>
        /// Gets the current VegetationItemInfo from the Clippboard. Used for copy paste in the VegetationSystem Inspector.
        /// </summary>
        /// <returns></returns>
        public static VegetationItemInfo GetVegetationItemFromClipboard()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                return Instance.Internal_GetVegetationItemFromClipboard();
            }

            return null;
        }


        /// <summary>
        ///  Sets the vegetation distance of all the VegetationSystem Components in the scene.
        /// </summary>
        /// <param name="distance"></param>
        public static void SetVegetationDistance(float distance)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetVegetationDistance(distance);
                }
            }
        }

        /// <summary>
        ///  Sets the tree distance of all the VegetationSystem Components in the scene.
        /// </summary>
        /// <param name="distance"></param>
        public static void SetTreeDistance(float distance)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetTreeDistance(distance);
                }
            }
        }

        /// <summary>
        /// Sets the billboard distance of all the VegetationSystem Components in the scene. 
        /// </summary>
        /// <param name="distance"></param>
        public static void SetBillboardDistance(float distance)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    Instance.VegetationSystemList[i].SetBillboardDistance(distance);
                }
            }
        }

        /// <summary>
        /// Returns an array of all billboard distances of all VegetationSystem Components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetBillboardDistances()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                float[] distances = new float[Instance.VegetationSystemList.Count];
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    distances[i] = Instance.VegetationSystemList[i].GetBillboardDistance();
                }
                return distances;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of all Tree distances of all VegetationSystem Components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetTreeDistances()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                float[] distances = new float[Instance.VegetationSystemList.Count];
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    distances[i] = Instance.VegetationSystemList[i].GetTreeDistance();
                }
                return distances;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of all vegetation distances of all VegetationSystem Components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetVegetationDistances()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                float[] distances = new float[Instance.VegetationSystemList.Count];
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    distances[i] = Instance.VegetationSystemList[i].GetVegetationDistance();
                }
                return distances;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of the current set vegetationPackageIndex if all VegetationSystem Components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetVegetationPackageIndexes()
        {
            if (!Instance) FindInstance();
            if (Instance)
            {
                float[] indexes = new float[Instance.VegetationSystemList.Count];
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    indexes[i] = Instance.VegetationSystemList[i].VegetationPackageIndex;
                }
                return indexes;
            }
            return new float[0];
        }
       
        /// <summary>
        /// Sets a new camera on all VegetationSystem Components in the scene. 
        /// </summary>
        /// <param name="camera"></param>
        public static void SetCamera(Camera camera)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if (Instance.VegetationSystemList[i].InitDone)
                    {
                        Instance.VegetationSystemList[i].SetCamera(camera);
                    }
                    else
                    {
                        Instance.VegetationSystemList[i].SelectedCamera = camera;
                    }

                }
            }
            
        }

        /// <summary>
        /// Seta s new terrain on all VegetationSystem Components in the scene. This should only be used when all VegetationSystems should have the same terrain. 
        /// </summary>
        /// <param name="terrain"></param>
        public static void SetTerrain(Terrain terrain)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if (Instance.VegetationSystemList[i].InitDone)
                    {
                        Instance.VegetationSystemList[i].SetTerrain(terrain);
                    }
                    else
                    {
                        Instance.VegetationSystemList[i].currentTerrain = terrain;
                    }

                }
            }
        }


        /// <summary>
        /// Returns an array of floats with all current grass densities of all VegetationSystem components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetGrassDensity()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                float[] density = new float[Instance.VegetationSystemList.Count];

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    density[i] = Instance.VegetationSystemList[i].GetGrassDensity();
                }
                return density;
            }
            return new float[0];
        }


        /// <summary>
        /// Returns an array of floats with all current plant densities of all VegetationSystem components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetPlantDensity()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                float[] density = new float[Instance.VegetationSystemList.Count];

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    density[i] = Instance.VegetationSystemList[i].GetPlantDensity();
                }

                return density;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of floats with all current tree densities of all VegetationSystem components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetTreeDensity()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                float[] density = new float[Instance.VegetationSystemList.Count];

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    density[i] = Instance.VegetationSystemList[i].GetTreeDensity();
                }

                return density;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of floats with all current Object densities of all VegetationSystem components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetObjectDensity()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                float[] density = new float[Instance.VegetationSystemList.Count];

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    density[i] = Instance.VegetationSystemList[i].GetObjectDensity();
                }

                return density;
            }
            return new float[0];
        }

        /// <summary>
        /// Returns an array of floats with all current LargeObject densities of all VegetationSystem components in the scene. 
        /// </summary>
        /// <returns></returns>
        public static float[] GetLargeObjectDensity()
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                float[] density = new float[Instance.VegetationSystemList.Count];

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    density[i] = Instance.VegetationSystemList[i].GetLargeObjectDensity();
                }

                return density;
            }
            return new float[0];
        }

        public static void VegetationPackageSync_RefreshVegetationPackage(VegetationSystem sourceVegetationSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                VegetationPackage vegetationPackage = sourceVegetationSystem.currentVegetationPackage;

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if ((Instance.VegetationSystemList[i] != null) && (Instance.VegetationSystemList[i] != sourceVegetationSystem))
                    {
                        if (Instance.VegetationSystemList[i].currentVegetationPackage == vegetationPackage)
                        {
                            Instance.VegetationSystemList[i].RefreshVegetationPackage(true);
                        }
                    }
                }
            }
        }

        public static void VegetationPackageSync_RefreshVegetationPackage(VegetationPackage updatedVegetationPackage)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if ((Instance.VegetationSystemList[i] != null) &&(Instance.VegetationSystemList[i].currentVegetationPackage == updatedVegetationPackage))
                    {
                        Instance.VegetationSystemList[i].RefreshVegetationPackage(true);
                    }
                }
            }
        }

        /// <summary>
        /// This will return all instances of a VegetationItem in cells that overlap the provided bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="vegetationItemID"></param>
        /// <param name="includePersistentStorage"></param>
        /// <returns></returns>
        public static List<Matrix4x4> GetVegetationItemInstances(Bounds bounds, string vegetationItemID, bool includePersistentStorage)
        {
            List<Matrix4x4> instanceList = new List<Matrix4x4>();

            if (!Instance) FindInstance();
            if (Instance)
            {
                Rect areaRect = RectExtension.CreateRectFromBounds(bounds);
                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {                    
                    List<VegetationCell> overlapVegetationCellList = Instance.VegetationSystemList[i].VegetationCellQuadTree.Query(areaRect);
                    for (int j = 0; j <= overlapVegetationCellList.Count - 1; j++)
                    {
                        instanceList.AddRange(overlapVegetationCellList[j].DirectSpawnVegetation(vegetationItemID, includePersistentStorage));
                    }
                }
            }
            return instanceList;
        }

        public static void VegetationPackageSync_ClearVegetationSystemCellCache(VegetationSystem sourceVegetationSystem)
        {
            if (!Instance) FindInstance();

            if (Instance)
            {
                VegetationPackage vegetationPackage = sourceVegetationSystem.currentVegetationPackage;

                for (int i = 0; i <= Instance.VegetationSystemList.Count - 1; i++)
                {
                    if ((Instance.VegetationSystemList[i] != null) && (Instance.VegetationSystemList[i] != sourceVegetationSystem))
                    {
                        if (Instance.VegetationSystemList[i].currentVegetationPackage == vegetationPackage)
                        {
                            Instance.VegetationSystemList[i].ClearVegetationCellCache();
							Instance.VegetationSystemList[i].RefreshVegetationModelInfoMaterials();
                            /*
#if UNITY_EDITOR
                            BillboardSystem billboardSystem = Instance.VegetationSystemList[i].GetComponent<BillboardSystem>();
                            if (billboardSystem)
                            {
                                billboardSystem.CreateBillboards();
                            }
#endif
*/
                        }
                    }
                }
            }
        }
    }
}

