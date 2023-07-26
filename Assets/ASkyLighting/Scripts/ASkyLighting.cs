using UnityEngine;
using System;
using System.Collections.Generic;
using DeepSky.Haze;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.Camera;
using Core.Cheats;
using Assets.Src.Lib.Cheats;
using Core.Environment.Logging.Extension;
#if UNITY_EDITOR
using System.Reflection;
#endif

namespace TOD
{
    public enum CelestialType
    {
        GazGiant = 0,
        GasGiantSatellite = 1,
        PlanetSatellite = 2,
    }

    public enum LightSource
    {
        Sun = 0,
        Moon01 = 1,
        Moon02 = 2,
        Stars = 3
    }

    public enum DirectionLightEnum
    {
        DirectionLight = 0,
        SupportDirectionLight = 1,
        None = 2
    }

    public enum DayName
    {
        Night = 0,
        Morning = 1,
        Day = 2,
        Evening = 3,
        Twilight = 4,
        DarkNight = 5,
    }

    public enum EmissionCurve
    {
        TwilightCurve = 0,
        NightCurve = 1,
        DarkNightCurve = 2
    }

    public enum CelestialSkyboxIndex
    {
        Disabled = 0,
        Moon0 = 1,
        Moon1 = 2,
        Moon2 = 3,
        Moon3 = 4
    }

    public enum CelestialDirectionLight
    {
        Disabled = 0,
        Direction0 = 1,
        Direction1 = 2,
    }

    public enum CelestialMovement
    {
        AroundParent = 0,
        World = 1
    }

    [System.Serializable]
    public class DayPart
    {
        public Vector2 percent;
        public Vector2 defaultPercent;
        public Color color;
        public DayName state;
        public Cubemap reflections;
        public DayPart(Vector2 percent, Vector2 defaultPercent, Color color, DayName state, Cubemap reflection)
        {
            this.percent = percent;
            this.defaultPercent = defaultPercent;
            this.color = color;
            this.state = state;
            this.reflections = reflection;
        }
    }

    public struct LightSkyData
    {
        public int number;
        public float intensity;

        public LightSkyData(int number, float intensity)
        {
            this.number = number;
            this.intensity = intensity;

        }
    }

    [System.Serializable]
    public class LightSkyBehaviour
    {
        public string name;
        public Vector3 forward = Vector3.down;
        public Color color = Color.red;
        public float intensity = 0;
        public float shadowStrenght = 1;
        public float shadowBias = 1;
        public float dot = 1;
        public Flare flare;

        public void Dot()
        {
            dot = Vector3.Dot(forward, Vector3.down);
        }

        public bool IsLighting()
        {
            if (dot > 0 && intensity > 0)
                return true;
            else
                return false;
        }

        public float IsLightingInt()
        {
            if (dot > 0 && intensity > 0)
                return intensity;
            else
                return 0;
        }

    }

    [Serializable]
    public class Celestial
    {
        public string name;
        public ASkyPlanet prefab = null;
        public CelestialType celestialType;

        public float scale;
        public float eccentricity;
        public float periapsis;

        public float longitude;
        public float inclination;
        public float argument;
        public float meanAnomaly;

        public int periodDays;
        public int turnsPeriod;

        public float meanAnomalyRot;
        public int periodDaysRot;
        public int turnsPeriodRot;

        public Vector3 up = Vector3.up;
        public Vector3 mainDirection;
        public Vector3 mainRotation;
        public float mainDistance;
        public bool directionOrbit;
        public bool directionRotation;
        public bool isTimelineAcceleration;
        public bool isShadowCasting;

        public ColorGradient color;
        public FloatCurve curve;
        public float eclipseAtmoIntensity;
        public FloatCurve gamma;
        public FloatCurve atmosphereDensity;

        public bool isRings = false;
        public bool isAtmopshere = true;

        public bool isHalo;
        public float haloSize;
        [ColorUsage(false, true)]
        public Color haloColor;

        public bool isActive = false;
        public bool isMovement = false;
        public bool isOrbit = false;
        public bool isVisual = false;
        public bool isRendered = true;

        public CelestialMovement planetMovement;
        public CelestialSkyboxIndex skyBoxIndex = CelestialSkyboxIndex.Disabled;
        public CelestialDirectionLight moonDirLight = CelestialDirectionLight.Disabled;

        public bool isColorMovement = false;

        public ColorGradient moonLightColor;
        public FloatCurve moonLightIntensity;

        public FloatCurve moonLightShadowStrenght;
        public FloatCurve moonLightShadowBias;

        public float intensityPower = 1;
        public int parent;

        public Celestial()
        {
            name = "NewName";

            color = new ColorGradient();
            curve = new FloatCurve();
            gamma = new FloatCurve();

            moonLightColor = new ColorGradient();
            moonLightIntensity = new FloatCurve();

            moonLightShadowStrenght = new FloatCurve();
            moonLightShadowBias = new FloatCurve();

            scale = 1f;
            parent = 0;
        }
        public void CopyFrom(Celestial other)
        {
#if UNITY_EDITOR
            if (other == null)
            {
                Debug.LogError("Satellite:CopyFrom - null context passed!");
                return;
            }

            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }

            mainDistance = other.mainDistance;
            mainRotation = other.mainRotation;

            curve.GetCurve(other.curve);
            gamma.GetCurve(other.gamma);
            atmosphereDensity.GetCurve(other.atmosphereDensity);
            color.GetGradient(other.color);

            moonLightColor.GetGradient(other.moonLightColor);
            moonLightIntensity.GetCurve(other.moonLightIntensity);
            moonLightShadowBias.GetCurve(other.moonLightShadowBias);
            moonLightShadowStrenght.GetCurve(other.moonLightShadowStrenght);
#endif
        }
    }

    [ExecuteInEditMode]
    public class ASkyLighting : MonoBehaviour
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static bool timeOfDayCheat = false;
        private static float timeOfDayCheatValue = 0.0f;

        private static bool timeOfDayShiftCheat = false;
        private static float timeOfDayShiftCheatValue = 0.0f;

        private static bool timeOfDayFlowCheat = false;
        private static float timeOfDayFlowCheatStartValue = 0.0f;
        private static float timeOfDayFlowCheatStartTime = 0.0f;
        private static bool timeOfDayFlowCheatPause = false;
        private static float timeOfDayFlowCheatPauseFactor = 0.0f;
        private static float timeOfDayFlowCheatFactor = 0.0f;
        private static float timeOfDayFlowCheatMinFactor = 0.25f;
        private static float timeOfDayFlowCheatMaxFactor = 128.0f;

        private static bool timeOfDayHazeCheat = true;

        [Cheat]
        public static void TimeOfDayCheat(float time)
        {
            if ((time >= 0.0f) && (time <= 24.0f))
            {
                timeOfDayCheat = true;
                timeOfDayCheatValue = time;
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDay cheat: {timeOfDayCheat}, value: {timeOfDayCheatValue}").Write();
            }
            else
            {
                timeOfDayCheat = false;
                timeOfDayCheatValue = 0.0f;
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDay cheat: {timeOfDayCheat}").Write();
            }
        }

        [Cheat]
        public static void TimeOfDayShiftCheat(float time)
        {
            if (time != 0.0f)
            {
                timeOfDayShiftCheat = true;
                timeOfDayShiftCheatValue = time;
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDayShift cheat: {timeOfDayShiftCheat}, value: {timeOfDayShiftCheatValue}").Write();
            }
            else
            {
                timeOfDayShiftCheat = false;
                timeOfDayShiftCheatValue = 0.0f;
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDayShift cheat: {timeOfDayShiftCheat}").Write();
            }
        }

        [Cheat]
        public static void TimeOfDayFlowCheat(float factor)
        {
            if ((_instance != null) && (factor >= 0.0f))
            {
                timeOfDayFlowCheatStartValue = _instance._timeline;
                timeOfDayFlowCheatStartTime = Time.time;
                timeOfDayFlowCheatFactor = factor;
                timeOfDayFlowCheat = true; // we must turn it on after we aquire new parameters
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDay flow cheat: {timeOfDayFlowCheat}, factor: {timeOfDayFlowCheatFactor}, pause: {timeOfDayFlowCheatPause}, pause factor: {timeOfDayFlowCheatPauseFactor}, start value: {timeOfDayFlowCheatStartValue}, statrt time: {timeOfDayFlowCheatStartTime}").Write();
            }
            else
            {
                timeOfDayFlowCheat = false;
                timeOfDayFlowCheatFactor = factor;
                Logger.IfInfo()?.Message($"ASkyLighting, TimeOfDay flow cheat: {timeOfDayFlowCheat}").Write();
            }
        }

        [Cheat]
        public static void TimeOfDayHazeCheat(bool show)
        {
            timeOfDayHazeCheat = show;
            if ((_instance != null) && (_instance.PlayerCamera))
            {
                var haze = _instance.PlayerCamera.gameObject.GetComponent<DS_HazeView>();
                if (haze != null)
                {
                    haze.enabled = timeOfDayHazeCheat;
                }
            }
        }

        [Cheat]
        public static void TimeOfDayResetCheat()
        {
            timeOfDayCheat = false;
            timeOfDayCheatValue = 0.0f;

            timeOfDayShiftCheat = false;
            timeOfDayShiftCheatValue = 0.0f;

            timeOfDayFlowCheat = false;
            timeOfDayFlowCheatStartValue = 0.0f;
            timeOfDayFlowCheatStartTime = 0.0f;
            timeOfDayFlowCheatPause = false;
            timeOfDayFlowCheatPauseFactor = 0.0f;
            timeOfDayFlowCheatFactor = 0.0f;

            TimeOfDayHazeCheat(true);
        }

        #region Properties
        public static bool isActiveOnClient;

        public static Func<float> ExternalTimeCB = () => RegionTime.CurrentDayInHourWithMinutes;
        public static float eclipsePower;
        public static float nightEclipseTimeHaze = 0.5f;
        public static float sunDotLimit = 0.05f;
        private static float ExternalTime => ExternalTimeCB?.Invoke() ?? 0.0f;
        public static float horizonFade = 4f;
        public static float shadowSmooth = 0.2f;
        public static float highlightOffset = 0.2f;
        public static float CGTime;
        public static float CGTimeFull;

        public bool isReflectionProbe = false;
        public bool playTime;
        public bool useServerTime;
        public bool useHaze = true;

        public bool isDebug;
        public bool isHide;

        public bool isOpenPresets;
        public bool isFullInitialized;

        public float intensityLight;
        public float intensityHorizon;
        public float intensityDirection;
        private float lastUpdateTime;

        public Material skyboxSource;
        public Material skyboxRendering;
        public Material skyboxClouds;

        private const int k_RightAngle = 90;
        public int skyRenderingLayer = 30;
        public float fullTimeline = 7f;
        public float dayInSeconds = 900f;
        protected const float k_DayDuration = 24f;

        protected int[] moonsDirLightIndex;

#if UNITY_EDITOR
        public string debugLightBehavioursLog;
#endif

        [SerializeField]
        public LightSkyBehaviour[] lightSkyBehaviours;
        public LightSkyData[] sort = new LightSkyData[3];

        private List<ASkyPlanet> celestialInstances = new List<ASkyPlanet>();
        private List<ASkyPlanet> celestialInstancesWithShadows = new List<ASkyPlanet>();
        [SerializeField]
        public DayPart[] dayParts;
        [SerializeField]
        public DayPart nightParts;
        [SerializeField]
        public ASkyLightingContext context = new ASkyLightingContext();
        [SerializeField]
        protected Light m_DirectionalLight = null;
        [SerializeField]
        protected Light m_DirectionalLight02 = null;
        [SerializeField]
        protected Transform m_SunTransform = null;
        [SerializeField]
        public Transform[] m_MoonTransform = null;

        public ASkyLightingContextAsset defaultContext;
        public DeepSky.Haze.DS_HazeCore hazeCore;

        private Material skyMaterial = null;

        public Camera PlayerCamera;
        public Camera skyboxRenderCamera;
        private ASkyPlanet[] planetsSort;

        public GameObject renderCameraHolder;
        public ASkyLightingSkyboxRendering SkyBoxRender;
        public Transform CelestialsTransform;
        public ReflectionProbe probe;

        public RenderTexture cloudsRenderTarget;
        public Cubemap starsCubemap = null;
        #endregion

#if UNITY_EDITOR
        public bool isDayPartSettings = false;
        public bool isAtmosphereSettings = false;
        public bool isSunSettings = false;
        public bool isStarsSettings = false;
        public bool isFogSettings = false;
        public bool isOtherSettings = false;
        public bool isResourcesSettings = false;
        public bool isPresetSettings = false;
        public bool isSatellites = false;
#endif

        public static event Action<ASkyLighting> Created;

        [ContextMenu("Create")]
        public void Create()
        {
            _instance = this;
            isFullInitialized = false;

            if (context.weather != null)
                context.weather.weatherFullyChanged = false;

            if (hazeCore == null)
                hazeCore = FindObjectOfType<DeepSky.Haze.DS_HazeCore>();

            if (m_SunTransform == null)
            {
                m_SunTransform = transform.Find("Sun Transform");
                if (m_SunTransform == null)
                {
                    GameObject sunObject = new GameObject();
                    sunObject.transform.parent = transform;
                    sunObject.name = "Sun Transform";
                    sunObject.transform.localPosition = Vector3.zero;
                    sunObject.hideFlags = HideFlags.HideInHierarchy;
                    m_SunTransform = sunObject.transform;
                }
            }

            if (m_MoonTransform == null || m_MoonTransform.Length != context.sattellites.Count)
                m_MoonTransform = new Transform[context.sattellites.Count];

            for (int i = 0; i < context.sattellites.Count; i++)
            {
                string moonName = "Moon" + ((i == 0) ? "" : ("0" + (i + 1).ToString())) + " Transform";
                m_MoonTransform[i] = transform.Find(moonName);
                if (m_MoonTransform[i] == null)
                {
                    GameObject moonObject = new GameObject();
                    moonObject.transform.parent = transform;
                    moonObject.name = moonName;
                    moonObject.transform.localPosition = Vector3.zero;
                    moonObject.hideFlags = HideFlags.HideInHierarchy;
                    m_MoonTransform[i] = moonObject.transform;
                }
            }

            UpdateSkyboxMaterial();
            CreateCelestials();

            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; i++)
            {
                if (allCameras[i].cameraType == CameraType.SceneView)
                    allCameras[i].cullingMask = 1 << context.cloudRenderingLayer;

            }
            fullTimeline = _timeline;

            Created?.Invoke(this);
        }
        public bool GetCamera()
        {
            if (PlayerCamera)
                return true;

            var playerCam = MainCamera.Camera; 
            if (playerCam)
            {
                PlayerCamera = playerCam;
                return true;
            }
            else
                return false;
        }

        private void OnCameraSpawned(Camera newCamera)
        {
            MainCamera.OnCameraCreated -= OnCameraSpawned;
            InitCamera(newCamera);
        }

        private void OnCameraDestroyed()
        {
            MainCamera.OnCameraDestroyed -= OnCameraDestroyed;
            ResetCamera();
        }

        public static void InitCamera(Camera camera)
        {
            if (_instance)
                _instance.InitNewCamera(camera);
            else
                Created += self => self.InitNewCamera(camera);
        }

        public static void ResetCamera()
        {
            _instance?.ResetCurrentCamera();
        }

        public void InitNewCamera(Camera camera)
        {
            PlayerCamera = camera ?? throw new ArgumentNullException(nameof(camera));
            isFullInitialized = false;
        }
        public void ResetCurrentCamera()
        {
            PlayerCamera = null;
        }
        public void LoadFromContextPreset(ASkyLightingContextAsset ctx)
        {
            context = new ASkyLightingContext();
            context.CopyFrom(ctx.context);
            InitBakedModules();
        }
        public void LoadDefaultPreset()
        {
            context = new ASkyLightingContext();
            context.CopyFrom(defaultContext.context);
            InitBakedModules();
        }
        void OnEnable()
        {
            if (context.weather.currentActiveWeatherPreset)
                context.weather.lastActiveWeatherPreset = context.weather.currentActiveWeatherPreset;

            if (context == null)
                context = new ASkyLightingContext();

            if (GameCamera.Camera != null)
                InitCamera(GameCamera.Camera);
            else
                GameCamera.OnCameraCreated += OnCameraSpawned;
            GameCamera.OnCameraDestroyed += OnCameraDestroyed;
        }
        private void Update()
        {
            if (!isActiveOnClient && Application.isPlaying)
                return;

            if (_instance == null || celestialInstances == null || celestialInstances.Count == 0)
                Create();

            CelestialUpdateVisual();
            //DirectionLightUpdate();

            if (ClientCheatsState.TimeOfDay)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if ((Input.GetKeyDown(KeyCode.F9)) || Input.GetKeyDown(KeyCode.F10) || Input.GetKeyDown(KeyCode.F11))
                    {
                        TimeOfDayResetCheat();
                    }
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    if (Input.GetKeyDown(KeyCode.F9))
                    {
                        if (timeOfDayFlowCheat)
                        {
                            if (timeOfDayFlowCheatPause)
                            {
                                timeOfDayFlowCheatPause = false;
                                TimeOfDayFlowCheat(timeOfDayFlowCheatPauseFactor);
                            }
                            else if (timeOfDayFlowCheatFactor >= timeOfDayFlowCheatMinFactor)
                            {
                                TimeOfDayFlowCheat(Mathf.Max(timeOfDayFlowCheatMinFactor, timeOfDayFlowCheatFactor * 0.5f));
                            }
                            else
                            {
                                TimeOfDayFlowCheat(0.0f);
                            }
                        }
                        else
                        {
                            TimeOfDayFlowCheat(timeOfDayFlowCheatMinFactor);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.F10))
                    {
                        if (timeOfDayFlowCheat)
                        {
                            if (timeOfDayFlowCheatPause)
                            {
                                timeOfDayFlowCheatPause = false;
                                TimeOfDayFlowCheat(timeOfDayFlowCheatPauseFactor);
                            }
                            else
                            {
                                timeOfDayFlowCheatPause = true;
                                timeOfDayFlowCheatPauseFactor = timeOfDayFlowCheatFactor;
                                TimeOfDayFlowCheat(0.0f);
                            }
                        }
                        else
                        {
                            timeOfDayFlowCheatPause = true;
                            timeOfDayFlowCheatPauseFactor = timeOfDayFlowCheatMinFactor * 2.0f;
                            TimeOfDayFlowCheat(0.0f);
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.F11))
                    {
                        if (timeOfDayFlowCheat)
                        {
                            if (timeOfDayFlowCheatPause)
                            {
                                timeOfDayFlowCheatPause = false;
                                TimeOfDayFlowCheat(timeOfDayFlowCheatPauseFactor);
                            }
                            else if (timeOfDayFlowCheatFactor > 0.0f)
                            {
                                TimeOfDayFlowCheat(Mathf.Min(timeOfDayFlowCheatMaxFactor, timeOfDayFlowCheatFactor * 2.0f));
                            }
                            else
                            {
                                TimeOfDayFlowCheat(timeOfDayFlowCheatMinFactor);
                            }
                        }
                        else
                        {
                            TimeOfDayFlowCheat(timeOfDayFlowCheatMinFactor * 4.0f);
                        }
                    }
                }
                else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    if (Input.GetKeyDown(KeyCode.F10))
                    {
                        TimeOfDayHazeCheat(!timeOfDayHazeCheat);
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.F9))
                    {
                        var shiftCheatValue = timeOfDayShiftCheatValue + 1.0f;
                        shiftCheatValue %= 24.0f;
                        TimeOfDayShiftCheat(shiftCheatValue);
                    }
                    else if (Input.GetKeyDown(KeyCode.F10))
                    {
                        var shiftCheatValue = timeOfDayShiftCheatValue - 1.0f;
                        if (shiftCheatValue < 0.0f)
                        {
                            shiftCheatValue += 24.0f;
                        }
                        TimeOfDayShiftCheat(shiftCheatValue);
                    }
                }
            }
        }
        void LateUpdate()
        {
            if (!isActiveOnClient && Application.isPlaying)
                return;

            ForceUpdate();
        }
        public void ForceUpdate()
        {
            UpdateTime();
            SunMovement();

            if (isActiveOnClient)
            {
                CelestialUpdateMovement();
                UpdatePlanetShadows();
                SetSatellitesRenderQueue();
            }

            Atmosphere();
            SunVisual();
            UpdateAmbient();
            Stars();
            DirectionLightUpdate();

            hazeCore.ForceUpdate();

            if (isActiveOnClient)
            {
                WeatherUpdate();
                Eclipse();
            }

            UpdateEnvironment();
        }
        public void UpdateSkyboxMaterial()
        {
            skyMaterial = new Material(skyboxSource);
            RenderSettings.skybox = skyMaterial;
        }
        public Light GetDirectionalLight()
        {
            return m_DirectionalLight;
        }

        public Vector3 SunDirection { get { return -m_SunTransform.forward; } }
        public float Hour { get; private set; }
        public float Minute { get; private set; }
        [SerializeField]
        private float _timeline = 7f;
        public float timeline
        {
            get { return _timeline; }
            set { _timeline = value; }
        }
        private static ASkyLighting __instance;
        public static ASkyLighting _instance
        {
            get
            {
                return __instance;
            }

            set
            {
                __instance = value;
                OnTODManagerArrived?.Invoke();
            }
        }

        public static event Action OnTODManagerArrived;

        #region Celestials
        private ASkyPlanet CreatePlanetVisual(int id)
        {
            Transform pivot = CreateSatellite(id);
            ASkyPlanet planetVisual = (ASkyPlanet)Instantiate<ASkyPlanet>(context.sattellites[id].prefab, pivot);
            if (isHide)
                planetVisual.planet.gameObject.hideFlags = HideFlags.HideAndDontSave;
            planetVisual.planet.gameObject.layer = context.cloudRenderingLayer;

            planetVisual.name = context.sattellites[id].name + "_planet";
            CelestialMechanics.CelestialRotation rotation = planetVisual.gameObject.AddComponent<CelestialMechanics.CelestialRotation>();
            rotation.startEpoch = context.sattellites[id].meanAnomalyRot;
            rotation.direction = (context.sattellites[id].directionRotation) ? 1 : -1;
            planetVisual.celestialOrbit = pivot.GetComponent<CelestialMechanics.CelestialOrbit>();
            planetVisual.celestialRotation = rotation;
            planetVisual.planetRenderer.transform.localScale = new Vector3(context.sattellites[id].scale, context.sattellites[id].scale, context.sattellites[id].scale);

            planetVisual.CreateMaterials();
            return planetVisual;
        }
        private ASkyPlanet CreatePlanet(int id)
        {
            Transform pivot = CreateSatellite(id);

            GameObject planetRotation = new GameObject();
            planetRotation.transform.parent = pivot.transform;
            planetRotation.name = context.sattellites[id].name + "_planet";
            ASkyPlanet planet = planetRotation.AddComponent<ASkyPlanet>();

            GameObject planetScale = new GameObject();
            planetScale.transform.parent = planetRotation.transform;
            planetScale.transform.localPosition = Vector3.zero;
            planetScale.transform.localRotation = Quaternion.identity;
            planetScale.name = "planetScale";
            planetScale.transform.localScale = new Vector3(context.sattellites[id].scale, context.sattellites[id].scale, context.sattellites[id].scale);

            CelestialMechanics.CelestialRotation rotation = planetRotation.gameObject.AddComponent<CelestialMechanics.CelestialRotation>();
            rotation.startEpoch = context.sattellites[id].meanAnomalyRot;
            rotation.direction = (context.sattellites[id].directionRotation) ? 1 : -1;
            planet.celestialOrbit = pivot.GetComponent<CelestialMechanics.CelestialOrbit>();
            planet.celestialRotation = rotation;
            planet.planet = planetScale;

            return planet;
        }
        private Transform CreateSatellite(int id)
        {
            if (context.sattellites[id].prefab == null)
            {
                Debug.Log("Satellite without prefab! Pleae assign a prefab to all satellites.");
                return null;
            }

            if (context.sattellites[id].isRendered == false)
                return null;

            if (context.sattellites[id].moonDirLight != CelestialDirectionLight.Disabled)
                moonsDirLightIndex[(int)context.sattellites[id].moonDirLight - 1] = id;

            Transform firstParent = CelestialsTransform;
            if (context.sattellites[id].parent > 0)
            {
                string parentName = context.sattellites[context.sattellites[id].parent - 1].name;
                foreach (ASkyPlanet hierarchyPlanet in celestialInstances)
                    if (hierarchyPlanet.name.Equals(parentName + "_planet"))
                        firstParent = hierarchyPlanet.planet.transform;
            }

            if (context.sattellites[id].planetMovement == CelestialMovement.World)
            {
                GameObject worldPivot = new GameObject();
                worldPivot.name = context.sattellites[id].name + "_worldPivot";
                worldPivot.transform.parent = firstParent;
                worldPivot.transform.localPosition = context.sattellites[id].mainDirection.normalized * context.sattellites[id].mainDistance;
                worldPivot.transform.localRotation = Quaternion.Euler(context.sattellites[id].mainRotation);
                firstParent = worldPivot.transform;
            }

            GameObject satRot = new GameObject();
            satRot.name = context.sattellites[id].name + "_orbit";
            satRot.transform.parent = firstParent;
            CelestialMechanics.CelestialOrbit orbit = satRot.AddComponent<CelestialMechanics.CelestialOrbit>();

            orbit.eccentricity = context.sattellites[id].eccentricity;
            orbit.periapsis = context.sattellites[id].periapsis;
            orbit.longitude = context.sattellites[id].longitude;
            orbit.inclination = context.sattellites[id].inclination;
            orbit.argument = context.sattellites[id].argument;
            orbit.startEpoch = context.sattellites[id].meanAnomaly;
            orbit.direction = (context.sattellites[id].directionOrbit) ? 1 : -1;

            return satRot.transform;
        }
        private void CelestialUpdateMovement()
        {
            if (CelestialsTransform != null)
            {
                if (Camera.main != null)
                    CelestialsTransform.transform.position = Camera.main.transform.position;
                CelestialsTransform.transform.localRotation = Quaternion.Euler(0, context.worldLongitude * 360f, 0);
            }

            float eclipseMax = 0;
            int ch = 0;

            for (int i = 0; i < context.sattellites.Count; i++)
            {
                if (!context.sattellites[i].isRendered)
                    continue;

                ASkyPlanet currentPlanet = celestialInstances[ch];
                ch++;

                if (PlayerCamera != null)
                    currentPlanet.dist = Vector3.Distance(currentPlanet.planet.transform.position, PlayerCamera.transform.position);

                if (context.sattellites[i].planetMovement == CelestialMovement.World)
                {
                    currentPlanet.celestialOrbit.transform.LookAt(ASkyLighting._instance.CelestialsTransform, context.sattellites[i].up);
                }

                float orbitalPeriod = (float)context.sattellites[i].turnsPeriod / (float)context.sattellites[i].periodDays;
                currentPlanet.movementTime = Mathf.Repeat(CGTimeFull * orbitalPeriod, context.sattellites[i].turnsPeriod);

                float rotationPeriod = (float)context.sattellites[i].turnsPeriodRot / (float)context.sattellites[i].periodDaysRot;
                currentPlanet.rotationTime = Mathf.Repeat(CGTimeFull * rotationPeriod, context.sattellites[i].turnsPeriodRot);

                if (context.sattellites[i].isTimelineAcceleration)
                {
                    float timeSun = CGTime * Mathf.PI * 2.0f - Mathf.PI / 2.0f;
                    float sunx = Mathf.Cos(timeSun);
                    float suny = Mathf.Cos(Mathf.PI * context.nightTimePercentage) + Mathf.Sin(timeSun);

                    float atan = Mathf.Atan(suny / sunx) / Mathf.PI;
                    float elevation = (sunx > 0) ? (atan + 0.5f) / 2 : ((atan + 0.5f) / 2 + 0.5f);

                    currentPlanet.celestialOrbit.CG = elevation;
                    currentPlanet.celestialRotation.CG = elevation;
                }
                else
                {
                    currentPlanet.celestialOrbit.CG = currentPlanet.movementTime;
                    currentPlanet.celestialRotation.CG = currentPlanet.rotationTime;
                }

                if (context.sattellites[i].skyBoxIndex != CelestialSkyboxIndex.Disabled)
                {
                    float eclipse = CheckMoonForIntersection(i, currentPlanet);
                    if (eclipse > eclipseMax)
                    {
                        eclipseMax = eclipse;
                    }
                }

                int moonIndex = (int)context.sattellites[i].skyBoxIndex - 1;
                m_MoonTransform[moonIndex].forward = Vector3.Normalize(currentPlanet.planet.transform.position - CelestialsTransform.transform.position);
                m_MoonTransform[moonIndex].localScale = new Vector3(-1, 1, 1);
                m_MoonTransform[moonIndex].parent = this.transform;

                currentPlanet.Calculate();

            }



            eclipsePower = eclipseMax;

        }

        private List<string> celestialShaderLabels = new List<string>()
        {
            "_MoonMatrix",
            "_MoonDir",
            "_MoonHalo",
            "_MoonMatrix02",
            "_MoonDir02",
            "_MoonHalo02",
            "_MoonMatrix03",
            "_MoonDir03",
            "_MoonHalo03",
            "_MoonMatrix04",
            "_MoonDir04",
            "_MoonHalo04",
        };

        private List<int> celestialShaderIDs = new List<int>();

        private bool CheckMaterialLabels(int count)
        {
            if (celestialShaderLabels.Count < (count * 3))
            {
                Debug.LogError("WRONG NUMBER OF CELESTIALS, PLEASE CHANGE materialLabels");
                return false;
            }
            else if (celestialShaderIDs.Count == 0)
            {
                for (int index = 0; index < celestialShaderLabels.Count; ++index)
                {
                    celestialShaderIDs.Add(Shader.PropertyToID(celestialShaderLabels[index]));
                }
            }
            return true;
        }

        public void CelestialUpdateVisual()
        {
            if (!context.useCelestials)
            {
                for (int i = 0; i < 4; i++)
                    ClearMoonIndex(i);
                return;

            }

            if (!CheckMaterialLabels(context.sattellites.Count))
            {
                return;
            }
            int ch = 0;
            for (int i = 0; i < context.sattellites.Count; i++)
            {
                if (!context.sattellites[i].isRendered)
                    continue;

                ASkyPlanet currentPlanet = celestialInstances[ch];
                ch++;

                float satelliteTime = CGTime;
                if (context.sattellites[i].isColorMovement)
                    satelliteTime = currentPlanet.movementTime;

                context.sattellites[i].color.Evaluate(satelliteTime);
                context.sattellites[i].curve.Evaluate(satelliteTime);
                context.sattellites[i].gamma.Evaluate(satelliteTime);
                context.sattellites[i].atmosphereDensity.Evaluate(satelliteTime);

                currentPlanet.atmoSphereColor = context.sattellites[i].color.color;
                currentPlanet.atmoThickness = context.sattellites[i].gamma.value;
                currentPlanet.atmoColorCoeff = context.sattellites[i].atmosphereDensity.value;
                currentPlanet.atmoStrength = Mathf.Lerp(context.sattellites[i].curve.value, context.sattellites[i].eclipseAtmoIntensity, eclipsePower);
                currentPlanet.sunDirection = SunDirection;
                currentPlanet.atmosphere = context.sattellites[i].isAtmopshere;

                float distFactor = 1000f / currentPlanet.dist;
                float sizeFactor = context.sattellites[i].scale / 50f;
                float finalFactor = (distFactor * sizeFactor);

                float size = finalFactor * 0.258f;

                int moonIndex = (int)context.sattellites[i].skyBoxIndex - 1;

                if (context.sattellites[i].moonDirLight != CelestialDirectionLight.Disabled)
                {
                    context.sattellites[i].moonLightColor.Evaluate(satelliteTime);
                    context.sattellites[i].moonLightIntensity.Evaluate(satelliteTime);
                    context.sattellites[i].moonLightShadowStrenght.Evaluate(satelliteTime);
                    context.sattellites[i].moonLightShadowBias.Evaluate(satelliteTime);
                    context.sattellites[i].intensityPower = Mathf.Abs(Vector3.Dot(m_MoonTransform[moonIndex].forward, -SunDirection) + 1) / 2;
                }

                Color haloColor = context.sattellites[i].haloColor.linear;
                Vector3 MoonDirection = -m_MoonTransform[moonIndex].forward;
                float dot = Vector3.Dot(m_MoonTransform[moonIndex].forward, Vector3.up);

                if (dot <= 0)
                {
                    size = 0;
                }

                skyMaterial.SetMatrix(celestialShaderIDs[(moonIndex * 3) + 0], m_MoonTransform[moonIndex].worldToLocalMatrix);
                skyMaterial.SetVector(celestialShaderIDs[(moonIndex * 3) + 1], new Vector4(MoonDirection.x, MoonDirection.y, MoonDirection.z, size / 2));
                if (context.sattellites[i].isHalo)
                {
                    skyMaterial.SetColor(celestialShaderIDs[(moonIndex * 3) + 2], new Vector4(haloColor.r, haloColor.g, haloColor.b, context.sattellites[i].haloSize));
                }
                else
                {
                    skyMaterial.SetColor(celestialShaderIDs[(moonIndex * 3) + 2], Vector4.zero);
                }
            }


        }
        void UpdatePlanetShadows()
        {

            for (int id = 0; id < celestialInstancesWithShadows.Count; id++)
                UpdatePlanetShadow(id);

        }

        private Vector4[] planetsShadows = new Vector4[3];
        void UpdatePlanetShadow(int id)
        {
            int ch = 0;
            for (int i = 0; i < planetsShadows.Length; i++)
            {
                planetsShadows[i] = Vector4.zero;
            }
            for (int i = 0; i < celestialInstancesWithShadows.Count; i++)
            {
                if (i != id)
                {
                    Vector3 pos = celestialInstancesWithShadows[i].transform.position;
                    planetsShadows[ch] = new Vector4(pos.x, pos.y, pos.z, celestialInstancesWithShadows[i].planetRenderer.transform.localScale.x);
                    ch++;
                }
            }
            celestialInstancesWithShadows[id].planetRenderer.material.SetVectorArray("_PlanetsDataArray", planetsShadows);
        }
        void SetSatellitesRenderQueue()
        {
            Array.Sort(planetsSort, (x, y) => x.dist.CompareTo(y.dist));

            int startQueue = 3100;
            for (int i = 0; i < planetsSort.Length; i++)
                planetsSort[i].planetRenderer.material.renderQueue = startQueue + i;
        }

        public void DeleteCelestialRoot()
        {
            if (CelestialsTransform == null)
                CelestialsTransform = transform.Find("CelestialsRoot");

            if (CelestialsTransform != null)
#if UNITY_EDITOR
                DestroyImmediate(CelestialsTransform.gameObject);
#else
            Destroy(CelestialsTransform.gameObject);
#endif
        }
        public void CreateCelestials()
        {
            celestialInstances = new List<ASkyPlanet>();
            DeleteCelestialRoot();

            GameObject sgo = new GameObject();
            sgo.transform.parent = transform;
            sgo.name = "CelestialsRoot";
            sgo.transform.localPosition = Vector3.zero;
            sgo.transform.localRotation = Quaternion.Euler(0, context.worldLongitude * 360f, 0);
            CelestialsTransform = sgo.transform;


            int childs = CelestialsTransform.childCount;
            for (int i = childs - 1; i >= 0; i--)
                Destroy(CelestialsTransform.GetChild(i).gameObject);

            celestialInstances.Clear();
            celestialInstancesWithShadows.Clear();

            moonsDirLightIndex = new int[2];

            if (!context.useCelestials)
                return;

            for (int i = 0; i < context.sattellites.Count; i++)
                if (context.sattellites[i].isRendered)
                {
                    ASkyPlanet newPlanet = (isActiveOnClient) ? CreatePlanetVisual(i) : CreatePlanet(i);
                    if (newPlanet != null)
                        celestialInstances.Add(newPlanet);
                }


            int ch = 0;
            int chR = 0;

            for (int i = 0; i < context.sattellites.Count; i++)
            {
                if (context.sattellites[i].isRendered)
                {

                    ASkyPlanet currentPlanet = celestialInstances[chR];
                    chR++;

                    if (context.sattellites[i].isShadowCasting)
                    {
                        celestialInstancesWithShadows.Add(currentPlanet);
                        ch++;
                    }

                }
            }
            Shader.SetGlobalInt("_PlanetsCount", ch);
            planetsSort = celestialInstancesWithShadows.ToArray();
        }
        #endregion

        #region LogicUpdates

        public float CheckMoonForIntersection(int id, ASkyPlanet planet)
        {

            Vector3 aP = m_SunTransform.forward;
            Vector3 bP = m_MoonTransform[(int)context.sattellites[id].skyBoxIndex - 1].forward * -1;
            float dist = Vector3.Distance(aP, bP);

            float moonSize = context.sattellites[id].scale / planet.dist;

            if (moonSize + context.sunSize < dist)
                return 0;
            else
            {
                float intersect = ((moonSize + context.sunSize) - dist);
                float intersectFinal = Mathf.Clamp01(intersect / (context.sunSize * 2));
                return intersectFinal;
            }

        }
        void UpdateTime()
        {
            var instantTimeLine = _timeline;
            var instantFullTimeline = fullTimeline;

            if (Application.isPlaying)
            {
                if (useServerTime)
                {
                    instantTimeLine = ExternalTime;
                    instantFullTimeline = ExternalTime;
                }
                else if (playTime)
                {
                    instantTimeLine += (Time.deltaTime / dayInSeconds) * k_DayDuration;
                    instantFullTimeline += (Time.deltaTime / dayInSeconds) * k_DayDuration;
                }

                if (timeOfDayCheat || timeOfDayShiftCheat || timeOfDayFlowCheat)
                {
                    if (timeOfDayCheat)
                    {
                        instantTimeLine = timeOfDayCheatValue;
                    }
                    if (timeOfDayShiftCheat)
                    {
                        instantTimeLine += timeOfDayShiftCheatValue;
                    }
                    if (timeOfDayFlowCheat)
                    {
                        instantTimeLine = timeOfDayFlowCheatStartValue + (((Time.time - timeOfDayFlowCheatStartTime) / dayInSeconds) * timeOfDayFlowCheatFactor);
                    }
                    instantFullTimeline = instantTimeLine;
                }

                if (instantTimeLine > k_DayDuration) instantTimeLine %= 24f;
                if (instantTimeLine < 0) instantTimeLine = k_DayDuration;

                if (instantFullTimeline < 0) instantFullTimeline = instantTimeLine;
            }

            Hour = Mathf.Floor(instantTimeLine);
            Minute = Mathf.Floor((instantTimeLine - Hour) * 60);

            CGTime = instantTimeLine / k_DayDuration;
            CGTimeFull = instantFullTimeline / k_DayDuration;
        }

        void SunMovement()
        {
            if (m_SunTransform != null)
            {
                m_SunTransform.localEulerAngles = new Vector3(0, 0, 0);

                float timeSun = CGTime * Mathf.PI * 2.0f - Mathf.PI / 2.0f;
                float sunx = Mathf.Cos(timeSun);
                float suny = Mathf.Cos(Mathf.PI * context.nightTimePercentage) + Mathf.Sin(timeSun);

                float elevation = Mathf.Atan(suny / sunx) * 180f / Mathf.PI;

                elevation = sunx < 0 ? elevation - 180 : elevation;

                m_SunTransform.Rotate(new Vector3(1, 0, 0), elevation, Space.World);

                float angle = (context.worldLongitude * 360f - k_RightAngle) * Mathf.PI / 180f;
                m_SunTransform.Rotate(new Vector3(0, 1, 0), context.worldLongitude * 360f - k_RightAngle, Space.World);
                m_SunTransform.Rotate(new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)), context.worldLatitude, Space.World);
            }
        }

        #endregion

        #region VisualUpdates

        void SunVisual()
        {
            context.sunLightIntensity.Evaluate(CGTime);
            context.sunColor.Evaluate(CGTime);
            context.sunLightShadowStrenght.Evaluate(CGTime);
            context.sunLightShadowBias.Evaluate(CGTime);

            skyMaterial.SetFloat("_SunSize", context.sunSize);
            skyMaterial.SetFloat("_SunSpotSize", context.sunSpotSize);
            skyMaterial.SetColor("_SunColor", context.sunColor.color);
            skyMaterial.SetVector("_SunDir", SunDirection);
        }

        void Atmosphere()
        {
            context.topSky.Evaluate(CGTime);
            context.horizonSky.Evaluate(CGTime);
            context.fxAmbient.Evaluate(CGTime);
            context.fxAmbient02.Evaluate(CGTime);
            context.fxAmbient03.Evaluate(CGTime);

            context.skyElevation.Evaluate(CGTime);
            context.atmosphereThickness.Evaluate(CGTime);

            context.exposure.Evaluate(CGTime);

            skyMaterial.SetColor("_TopSkyColor", context.topSky.color);
            skyMaterial.SetColor("_HorizonSkyColor", context.horizonSky.color);
            skyMaterial.SetFloat("_TopSky", context.skyElevation.value);
            skyMaterial.SetFloat("_AtmosphereThickness", context.atmosphereThickness.value);
            skyMaterial.SetColor("_GroundColor", context.groundColor.color);

            if (!context.useHorizonFade)
            {
                skyMaterial.DisableKeyword("HORIZONFADE");
            }
            else
            {
                skyMaterial.EnableKeyword("HORIZONFADE");
                skyMaterial.SetFloat("_HorizonFade", horizonFade);
            }

            Vector4 emissionPower = Vector3.one;
            emissionPower.x = context.fxAmbient.value;
            emissionPower.y = context.fxAmbient02.value;
            emissionPower.z = context.fxAmbient03.value;
            Shader.SetGlobalVector("_EmissionTodPower", emissionPower);

            skyMaterial.SetFloat("_Exposure", context.exposure.value);
        }

        void UpdateAmbient()
        {
            RenderSettings.ambientMode = context.ambientMode;

            switch (context.ambientMode)
            {
                case UnityEngine.Rendering.AmbientMode.Skybox:
                    context.ambientIntensity.Evaluate(CGTime);
                    RenderSettings.ambientIntensity = context.ambientIntensity.value;
                    break;
                case UnityEngine.Rendering.AmbientMode.Trilight:
                    context.ambientGround.Evaluate(CGTime);
                    context.ambientEquator.Evaluate(CGTime);
                    context.ambientSky.Evaluate(CGTime);
                    RenderSettings.ambientGroundColor = context.ambientGround.color;
                    RenderSettings.ambientEquatorColor = context.ambientEquator.color;
                    RenderSettings.ambientSkyColor = context.ambientSky.color;
                    break;
                case UnityEngine.Rendering.AmbientMode.Flat:
                    context.ambientColor.Evaluate(CGTime);
                    RenderSettings.ambientLight = context.ambientColor.color;
                    break;
                case UnityEngine.Rendering.AmbientMode.Custom:
                    break;
            }

            context.groundColor.Evaluate(CGTime);

        }

        void Stars()
        {
            if ((starsCubemap == null) || !context.useStars)
            {
                skyMaterial.DisableKeyword("STARS");
                return;
            }

            skyMaterial.EnableKeyword("STARS");

            Matrix4x4 sunMatrix = m_SunTransform.worldToLocalMatrix;
            Matrix4x4 starsMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(context.starsOffsets), Vector3.one);

            skyMaterial.SetMatrix("_SunMatrix", sunMatrix);
            skyMaterial.SetMatrix("_StarsMatrix", starsMatrix);

            context.starsColor.Evaluate(CGTime);
            context.starsIntensity.Evaluate(CGTime);
            context.starsLightIntensity.Evaluate(CGTime);
            skyMaterial.SetTexture("_StarsCubemap", starsCubemap);
            skyMaterial.SetColor("_StarsColor", context.starsColor.color);
            skyMaterial.SetFloat("_StarsIntensity", context.starsIntensity.value);
        }

        void UpdateEnvironment()
        {
            if (context.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox && (Time.realtimeSinceStartup - lastUpdateTime > 1f))
            {
                lastUpdateTime = Time.realtimeSinceStartup;
                DynamicGI.UpdateEnvironment();
            }
        }

        void WeatherUpdate()
        {
            if (!isFullInitialized)
            {
                if (InitBakedModules())
                    isFullInitialized = true;
                else
                {
                    isFullInitialized = false;
                    return;
                }
            }

            if (context.useClouds)
                context.weather.UpdateClouds();
        }

        private void Eclipse()
        {
            if (!context.useCelestials || !context.isEclipse)
                return;

            skyMaterial.SetFloat("_AtmosphereThickness", Mathf.Lerp(context.atmosphereThickness.value, context.nightItem.atmosphereThickness, eclipsePower));
            skyMaterial.SetFloat("_TopSky", Mathf.Lerp(context.skyElevation.value, context.nightItem.skyElevation, eclipsePower));

            if (eclipsePower > 0.5f)
            {
                if (eclipsePower > 0.75f)
                {
                    skyMaterial.SetFloat("_SunIntensity", Mathf.Lerp(context.nightItem.eclipseSunPower, 0, (eclipsePower - 0.75f) * 4));
                    skyMaterial.SetColor("_TopSkyColor", Color.Lerp(context.nightItem.eclipseTopSkyColor, context.nightItem.topSky, (eclipsePower - 0.75f) * 4));

                }
                else
                {
                    skyMaterial.SetFloat("_SunIntensity", context.nightItem.eclipseSunPower);
                    skyMaterial.SetColor("_TopSkyColor", context.nightItem.eclipseTopSkyColor);
                }
            }
            else
            {

                skyMaterial.SetFloat("_SunIntensity", Mathf.Lerp(context.sunLightIntensity.value, context.nightItem.eclipseSunPower, eclipsePower * 2));
                skyMaterial.SetColor("_TopSkyColor", Color.Lerp(context.topSky.color, context.nightItem.eclipseTopSkyColor, eclipsePower * 2));
            }

            skyMaterial.SetColor("_SunColor", Color.Lerp(context.sunColor.color, context.nightItem.eclipseSunColor, eclipsePower));
            skyMaterial.SetColor("_HorizonSkyColor", Color.Lerp(context.horizonSky.color, context.nightItem.horizonSky, eclipsePower));
            skyMaterial.SetColor("_GroundColor", Color.Lerp(context.groundColor.color, context.nightItem.groundColor, eclipsePower));

            skyMaterial.SetColor("_StarsColor", Color.Lerp(context.starsColor.color, context.nightItem.starsColor, eclipsePower));
            skyMaterial.SetFloat("_StarsIntensity", Mathf.Lerp(context.starsIntensity.value, context.nightItem.startIntensity, eclipsePower));
            Shader.SetGlobalFloat("_eclipsePower", eclipsePower);
            Vector3 emissionPower = Vector3.zero;
            emissionPower.x = Mathf.Lerp(context.fxAmbient.value, context.nightItem.fxAmbient, eclipsePower);
            emissionPower.y = Mathf.Lerp(context.fxAmbient02.value, context.nightItem.fxAmbient02, eclipsePower);
            emissionPower.z = Mathf.Lerp(context.fxAmbient03.value, context.nightItem.fxAmbient03, eclipsePower);
            skyMaterial.SetVector("_EmissionPower", emissionPower);

            switch (context.ambientMode)
            {
                case UnityEngine.Rendering.AmbientMode.Skybox:
                    RenderSettings.ambientIntensity = Mathf.Lerp(context.ambientIntensity.value, context.nightItem.ambientIntensity, eclipsePower);
                    break;
                case UnityEngine.Rendering.AmbientMode.Trilight:
                    RenderSettings.ambientGroundColor = Color.Lerp(context.ambientGround.color, context.nightItem.ambientGround, eclipsePower);
                    RenderSettings.ambientEquatorColor = Color.Lerp(context.ambientEquator.color, context.nightItem.ambientEquator, eclipsePower);
                    RenderSettings.ambientSkyColor = Color.Lerp(context.ambientSky.color, context.nightItem.ambientSky, eclipsePower);
                    break;
            }

            if (isFullInitialized && context.useClouds)
                for (int i = 0; i < context.weather.cloudSystem.cloudsLayers.Count; i++)
                {
                    context.weather.cloudSystem.cloudsLayers[i].myMaterial.SetColor("_SkyColor", Color.Lerp(context.weather.cloudSystem.cloudsSettings.skyColor.color, context.nightItem.cloudSkyColor, eclipsePower));
                    context.weather.cloudSystem.cloudsLayers[i].myMaterial.SetColor("_MoonColor", Color.Lerp(context.weather.cloudSystem.cloudsSettings.moonHighlight.color, context.nightItem.cloudMoonHighlight, eclipsePower));
                    context.weather.cloudSystem.cloudsLayers[i].myMaterial.SetColor("_SunColor", Color.Lerp(context.weather.cloudSystem.cloudsSettings.sunHighlight.color, context.nightItem.cloudSunHighlight, eclipsePower));
                    context.weather.cloudSystem.cloudsLayers[i].myMaterial.SetFloat("_lightIntensity", Mathf.Lerp(context.weather.cloudSystem.cloudsSettings.lightIntensity.value, context.nightItem.cloudLightIntensity, eclipsePower));
                }

            skyMaterial.SetFloat("_Exposure", Mathf.Lerp(context.exposure.value, context.nightItem.exposure, eclipsePower));

        }

        private void ClearMoonIndex(int index)
        {
            m_MoonTransform[index].forward = new Vector3(0, -1, 0);
            Vector3 MoonDirection = -m_MoonTransform[index].forward;

            if (index == 0)
            {
                skyMaterial.SetMatrix("_MoonMatrix", m_MoonTransform[index].worldToLocalMatrix);
                skyMaterial.SetVector("_MoonDir", new Vector4(MoonDirection.x, MoonDirection.y, MoonDirection.z, 0));
                skyMaterial.SetColor("_MoonHalo", Vector4.zero);
            }
            else
            {
                skyMaterial.SetMatrix("_MoonMatrix0" + (index + 1).ToString(), m_MoonTransform[index].worldToLocalMatrix);
                skyMaterial.SetVector("_MoonDir0" + (index + 1).ToString(), new Vector4(MoonDirection.x, MoonDirection.y, MoonDirection.z, 0));
                skyMaterial.SetColor("_MoonHalo0" + (index + 1).ToString(), Vector4.zero);
            }
        }

        #endregion

        #region RenderToSkybox
        public bool InitBakedModules()
        {
            if (Application.isPlaying && GetCamera())
            {
                if (context.useClouds)
                    context.weather.cloudSystem.InitClouds(this);
                CreateBakeSkyboxCamera();
                return true;
            }
            else
                return false;


        }
        private void CreateCameraHolder()
        {
            if (renderCameraHolder == null)
            {
                GameObject obj = new GameObject();
                if (isHide)
                    obj.hideFlags = HideFlags.HideAndDontSave;
                obj.name = "Support Cameras";
                obj.transform.parent = transform;
                renderCameraHolder = obj;
            }
        }

        private void CreateSkyboxRenderCamera()
        {
            if (!skyboxRenderCamera)
            {
                GameObject camObj = new GameObject();
                camObj.name = "Skybox Camera";
                if (isHide)
                    camObj.hideFlags = HideFlags.HideAndDontSave;
                camObj.transform.SetParent(renderCameraHolder.transform);
                camObj.transform.localPosition = Vector3.zero;
                camObj.transform.localRotation = Quaternion.identity;

                skyboxRenderCamera = camObj.AddComponent<Camera>();
                skyboxRenderCamera.farClipPlane = PlayerCamera.farClipPlane * context.weather.cloudSystem.worldScale;
                skyboxRenderCamera.nearClipPlane = PlayerCamera.nearClipPlane;
                skyboxRenderCamera.aspect = PlayerCamera.aspect;
                skyboxRenderCamera.useOcclusionCulling = false;
                skyboxRenderCamera.renderingPath = RenderingPath.Forward;
                skyboxRenderCamera.fieldOfView = PlayerCamera.fieldOfView;
                skyboxRenderCamera.clearFlags = CameraClearFlags.Skybox;
                skyboxRenderCamera.cullingMask = (1 << context.cloudRenderingLayer);
                skyboxRenderCamera.enabled = false;
            }
            cloudsRenderTarget = new RenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, 24, RenderTextureFormat.DefaultHDR);
            skyboxRenderCamera.targetTexture = cloudsRenderTarget;

        }

        void CreateBakeSkyboxCamera()
        {
            CreateCameraHolder();
            CreateSkyboxRenderCamera();

            PlayerCamera.clearFlags = (context.weather.cloudSystem.useBakeClouds) ? CameraClearFlags.Color : CameraClearFlags.Skybox;

            SkyBoxRender = PlayerCamera.gameObject.GetComponent<ASkyLightingSkyboxRendering>();
            if (SkyBoxRender == null)
                SkyBoxRender = PlayerCamera.gameObject.AddComponent<ASkyLightingSkyboxRendering>();

            Material mat = new Material(skyboxRendering);

            SkyBoxRender.material = mat;
            SkyBoxRender.Apply();
        }
        #endregion

        #region DirectionLight

        public void UpdateSun(ref LightSkyBehaviour lightSkyBehaviour)
        {
            lightSkyBehaviour.forward = m_SunTransform.forward;
            lightSkyBehaviour.intensity = context.sunLightIntensity.value;// 
            lightSkyBehaviour.color = context.sunColor.color;// Color.Lerp(, Color.black.linear, eclipsePower);
            lightSkyBehaviour.shadowStrenght = context.sunLightShadowStrenght.value;
            lightSkyBehaviour.shadowBias = context.sunLightShadowBias.value;
            lightSkyBehaviour.Dot();
            lightSkyBehaviour.flare = context.sunFlare;
        }

        public void UpdateMoon(ref LightSkyBehaviour lightSkyBehaviour, int moon)
        {
            lightSkyBehaviour.forward = -m_MoonTransform[moonsDirLightIndex[moon]].forward;
            lightSkyBehaviour.intensity = context.sattellites[moonsDirLightIndex[moon]].moonLightIntensity.value * context.sattellites[moonsDirLightIndex[moon]].intensityPower;
            lightSkyBehaviour.color = context.sattellites[moonsDirLightIndex[moon]].moonLightColor.color;
            lightSkyBehaviour.shadowStrenght = context.sattellites[moonsDirLightIndex[moon]].moonLightShadowStrenght.value;
            lightSkyBehaviour.shadowBias = context.sattellites[moonsDirLightIndex[moon]].moonLightShadowBias.value;
            lightSkyBehaviour.Dot();
            lightSkyBehaviour.flare = null;
        }

        public void UpdateStars(ref LightSkyBehaviour lightSkyBehaviour)
        {
            lightSkyBehaviour.forward = Vector3.down;
            lightSkyBehaviour.intensity = context.starsLightIntensity.value;
            lightSkyBehaviour.color = context.starsLightColor;
            lightSkyBehaviour.shadowStrenght = context.starsLightShadowStrenght;
            lightSkyBehaviour.shadowBias = context.starsLightShadowBias;
            lightSkyBehaviour.flare = null;
        }

#if UNITY_EDITOR
        public string DebugLight(int number)
        {
            string current = "Light Source" + number;
            current += " | Intensity: " + lightSkyBehaviours[number].intensity.ToString("F4");
            current += " | Dot: " + lightSkyBehaviours[number].dot.ToString("F4");

            return current;
        }
#endif
        void InitializeBehaviours()
        {
            if (lightSkyBehaviours == null || lightSkyBehaviours.Length == 0)
            {
                lightSkyBehaviours = new LightSkyBehaviour[4];
                for (int i = 0; i < 4; i++)
                {
                    lightSkyBehaviours[i] = new LightSkyBehaviour();
                }

                lightSkyBehaviours[0].name = "Sun";
                lightSkyBehaviours[1].name = context.sattellites[0].name;
                lightSkyBehaviours[2].name = context.sattellites[1].name;
                lightSkyBehaviours[3].name = "Stars";

            }
        }


        public void DirectionLightUpdate()
        {
            InitializeBehaviours();

            UpdateSun(ref lightSkyBehaviours[0]);
            UpdateMoon(ref lightSkyBehaviours[1], 0);
            UpdateMoon(ref lightSkyBehaviours[2], 1);
            UpdateStars(ref lightSkyBehaviours[3]);

#if UNITY_EDITOR
            if (isDebug)
            {
                debugLightBehavioursLog = string.Empty;
            }
#endif

            sort[0] = new LightSkyData(1, lightSkyBehaviours[1].intensity);
            sort[1] = new LightSkyData(2, lightSkyBehaviours[2].intensity);
            sort[2] = new LightSkyData(3, lightSkyBehaviours[3].intensity);
            Array.Sort(sort, (x, y) => x.intensity.CompareTo(y.intensity));

            if (lightSkyBehaviours[0].IsLighting())
            {
                if (context.isEclipse && context.useCelestials)
                {
                    if (eclipsePower > 0)
                    {
                        if (eclipsePower == 1)
                            MoonPrime(sort[2].number, sort[1].number);
                        else
                        {
                            UpdateDirectionLight(DirectionLightEnum.DirectionLight, LightSource.Sun);
                            UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)sort[2].number);

                            m_DirectionalLight.intensity = Mathf.Lerp(context.sunLightIntensity.value, context.nightItem.eclipseSunPower, eclipsePower);
                            RenderSettings.flareStrength = Mathf.Lerp(1, 0, eclipsePower);
                            UpdateHazeData(m_DirectionalLight.intensity, m_DirectionalLight.color, 1);
                        }
                    }
                    else
                        DefaultSun(sort[2].number);
                }
                else
                    DefaultSun(sort[2].number);

            }
            else
                MoonsLast(sort[2].number, sort[1].number);


#if UNITY_EDITOR
            if (isDebug)
            {
                debugLightBehavioursLog += "\n[Flare " + RenderSettings.flareStrength.ToString("F2");
                debugLightBehavioursLog += " | Main Light Intensity " + m_DirectionalLight.intensity.ToString("F2");
                debugLightBehavioursLog += " | Second Light Intensity " + m_DirectionalLight02.intensity.ToString("F2");
            }
#endif
        }

        public void UpdateDirectionLight(DirectionLightEnum dirLight, LightSource lightSource)
        {
            Light _DirectionLight = dirLight == DirectionLightEnum.DirectionLight ? m_DirectionalLight : m_DirectionalLight02;
            int number = (int)lightSource;

            if (dirLight == DirectionLightEnum.SupportDirectionLight)
                if (_DirectionLight.gameObject.activeSelf == false)
                    _DirectionLight.gameObject.SetActive(true);


            _DirectionLight.transform.forward = lightSkyBehaviours[number].forward;
            _DirectionLight.color = lightSkyBehaviours[number].color;
            _DirectionLight.intensity = lightSkyBehaviours[number].intensity;
            _DirectionLight.shadowStrength = lightSkyBehaviours[number].shadowStrenght;
            _DirectionLight.shadowBias = lightSkyBehaviours[number].shadowBias;

            if (lightSkyBehaviours[number].flare)
                _DirectionLight.flare = lightSkyBehaviours[number].flare;
            else
                _DirectionLight.flare = null;

#if UNITY_EDITOR
            if (isDebug)
            {
                if (dirLight == DirectionLightEnum.DirectionLight)
                    debugLightBehavioursLog += "EclipsePower :" + eclipsePower.ToString("F3") + " | Main Light: " + "[" + lightSource.ToString() + "]";
                else
                    debugLightBehavioursLog += " | Second Light: " + "[" + lightSource.ToString() + "]";
                //debugLightBehavioursLog += (debugStr == "") ? "" : debugStr + lightSkyBehaviours[number].name;
            }
#endif
        }

        public void UpdateSupportDirectionLightOff()
        {
            m_DirectionalLight02.intensity = 0;

            if (m_DirectionalLight02.gameObject.activeSelf == true)
                m_DirectionalLight02.gameObject.SetActive(false);

#if UNITY_EDITOR
            if (isDebug)
            {
                debugLightBehavioursLog += " | Second Light: [Off]";
            }
#endif
        }


        void GetMoonsLightIntensity(int id, int id02, ref float intensity, ref Color color)
        {
            float sum = lightSkyBehaviours[id].intensity + lightSkyBehaviours[id02].intensity;
            float diffLimit = 0.125f;

            float diff = Mathf.Abs(lightSkyBehaviours[id].intensity - lightSkyBehaviours[id02].intensity);
            if (diff < diffLimit)
            {
                if (lightSkyBehaviours[id].intensity > lightSkyBehaviours[id02].intensity)
                {
                    intensity = lightSkyBehaviours[id].intensity;
                    color = lightSkyBehaviours[id].color;
                }
                else
                {
                    intensity = lightSkyBehaviours[id02].intensity;
                    color = lightSkyBehaviours[id02].color;
                }

                float direct = diff / diffLimit;

                float sumIntensity = Mathf.Lerp(0, m_DirectionalLight.intensity, direct);
                Color sumColor = Color.Lerp(Color.black, m_DirectionalLight.color, direct);

                if (sumIntensity < 0.02f) sumIntensity = 0.02f;
                intensity = sumIntensity;
                color = sumColor;

            }
            else
            {
                if (lightSkyBehaviours[id].intensity > lightSkyBehaviours[id02].intensity)
                {
                    intensity = lightSkyBehaviours[id].intensity;
                    color = lightSkyBehaviours[id].color;
                }
                else
                {
                    intensity = lightSkyBehaviours[id02].intensity;
                    color = lightSkyBehaviours[id02].color;
                }
            }


        }

        void CelestialFight(int id, int id02)
        {
            float sum = lightSkyBehaviours[id].intensity + lightSkyBehaviours[id02].intensity;

            float diffLimit = 0.125f;
            float diff = Mathf.Abs(lightSkyBehaviours[id].intensity - lightSkyBehaviours[id02].intensity);
            //float diffCoeff = 1 / 0.2f;

            if (diff < diffLimit)
            {
                if (lightSkyBehaviours[id].intensity > lightSkyBehaviours[id02].intensity)
                {
                    UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)id);
                    UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)id02);
                }
                else
                {
                    UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)id02);
                    UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)id);
                }

                float direct = diff / diffLimit;

                float sumIntensity = Mathf.Lerp(0, m_DirectionalLight.intensity, direct);
                Color sumColor = Color.Lerp(Color.black, m_DirectionalLight.color, direct);


                UpdateHazeData(sumIntensity, sumColor, direct);

#if UNITY_EDITOR
                if (isDebug)
                {
                    debugLightBehavioursLog += " |D " + direct.ToString("F3") + " SI " + sumIntensity.ToString("F3") + "| ";
                }
#endif

                if (m_DirectionalLight.intensity < 0.02f) m_DirectionalLight.intensity = 0.02f;
                if (m_DirectionalLight02.intensity < 0.02f) m_DirectionalLight02.intensity = 0.02f;
            }
            else
            {
                if (lightSkyBehaviours[id].intensity > lightSkyBehaviours[id02].intensity)
                {
                    UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)id);
                    UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)id02);
                }
                else
                {
                    UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)id02);
                    UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)id);
                }

                UpdateHazeData(m_DirectionalLight.intensity, m_DirectionalLight.color, 1);
            }

        }


        void MoonPrime(int firstId, int secondId)
        {
            CelestialFight(firstId, secondId);
            RenderSettings.flareStrength = 0;
        }

        void DefaultSun(int firstId)
        {
            UpdateDirectionLight(DirectionLightEnum.DirectionLight, LightSource.Sun);

            if (lightSkyBehaviours[(int)LightSource.Sun].dot < sunDotLimit)
            {
                intensityDirection = lightSkyBehaviours[0].dot * (1 / sunDotLimit);
                intensityHorizon = intensityDirection / 2 + 0.5f;

                float otherIntensity = 0;
                Color otherColor = Color.black;
                GetMoonsLightIntensity(sort[2].number, sort[1].number, ref otherIntensity, ref otherColor);

                m_DirectionalLight.intensity = Mathf.Lerp(otherIntensity, lightSkyBehaviours[0].intensity, intensityHorizon);
                m_DirectionalLight.color = Color.Lerp(otherColor, lightSkyBehaviours[0].color, intensityHorizon);

                UpdateHazeData(m_DirectionalLight.intensity, m_DirectionalLight.color, intensityDirection);
                RenderSettings.flareStrength = intensityHorizon;

                UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, (LightSource)firstId);

                m_DirectionalLight02.intensity = Mathf.Lerp(otherIntensity, 0, intensityHorizon);
                m_DirectionalLight02.color = otherColor;
            }
            else
            {
                UpdateHazeData(lightSkyBehaviours[0].intensity, lightSkyBehaviours[0].color, 1);
                UpdateSupportDirectionLightOff();
                RenderSettings.flareStrength = 1;
            }

        }

        void MoonsLast(int firstId, int secondId)
        {

            //

            if (lightSkyBehaviours[0].dot > -sunDotLimit)
            {
                intensityDirection = Mathf.Lerp(1, 0, Mathf.Abs(lightSkyBehaviours[0].dot) * (1 / sunDotLimit));
                intensityHorizon = intensityDirection / 2;
                intensityDirection = 1.0f - intensityDirection;

                UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)firstId);
                m_DirectionalLight.intensity = Mathf.Lerp(lightSkyBehaviours[firstId].intensity, 0, intensityHorizon);
                m_DirectionalLight.color = m_DirectionalLight.color;// Color.Lerp(m_DirectionalLight.color, lightSkyBehaviours[0].color, intensityHorizon);

                UpdateDirectionLight(DirectionLightEnum.SupportDirectionLight, LightSource.Sun);
                m_DirectionalLight02.intensity = Mathf.Lerp(Mathf.Lerp(lightSkyBehaviours[firstId].intensity, lightSkyBehaviours[0].intensity, 0.5f), 0, intensityDirection);
                Color colorStrange = Color.Lerp(lightSkyBehaviours[firstId].color, lightSkyBehaviours[0].color, intensityHorizon);
                m_DirectionalLight02.color = colorStrange;

                float intensityStrange = Mathf.Lerp(lightSkyBehaviours[firstId].intensity, lightSkyBehaviours[0].intensity, intensityHorizon);
                UpdateHazeData(intensityStrange, colorStrange, intensityDirection);

                RenderSettings.flareStrength = intensityHorizon;
            }
            else
                MoonPrime(firstId, secondId);
            /*
            else
            {
                UpdateDirectionLight(DirectionLightEnum.DirectionLight, (LightSource)firstId, " State: MoonLast ");
                UpdateHazeData(m_DirectionalLight.intensity, m_DirectionalLight.color, intensityDirection);
                intensityHorizon = 0;
                intensityDirection = 1;
                RenderSettings.flareStrength = 0;
            }
            */
        }

        void UpdateHazeData(float intensity, Color color, float directIntensity)
        {
            hazeCore.hazeColor = color;
            hazeCore.hazeIntensity = intensity;
            hazeCore.hazeDirectIntensity = directIntensity;
        }

        #endregion

        #region Streching
        public static void StretchDayPart(ref DayPart[] dayParts, float nightTimePercentage)
        {
            for (int i = 0; i < dayParts.Length; i++)
            {
                SelectStretch(ref dayParts[i].percent.x, dayParts[i].defaultPercent.x, nightTimePercentage);
                SelectStretch(ref dayParts[i].percent.y, dayParts[i].defaultPercent.y, nightTimePercentage);
            }
        }

        static void SelectStretch(ref float currentPercent, float defaultPercent, float nightTimePercentage)
        {
            if (defaultPercent < 0.5f)
            {
                if (defaultPercent < 0.25f)
                    CalculateStretch(ref currentPercent, defaultPercent, 0, nightTimePercentage);
                else
                    CalculateStretch(ref currentPercent, defaultPercent, 1, nightTimePercentage);
            }
            else
            {
                if (defaultPercent > 0.75f)
                    CalculateStretch(ref currentPercent, defaultPercent, 3, nightTimePercentage);
                else
                    CalculateStretch(ref currentPercent, defaultPercent, 2, nightTimePercentage);
            }
        }
        static void CalculateStretch(ref float currentPercent, float defaultPercent, int type, float nightTimePercentage)
        {
            switch (type)
            {
                case 0:
                {
                    float half = defaultPercent;
                    float correct = nightTimePercentage / 0.5f;
                    currentPercent = (half * correct);
                    break;
                }
                case 1:
                {
                    float half = (0.5f - defaultPercent);
                    float correct = (1f - nightTimePercentage) / 0.5f;
                    currentPercent = 0.5f - (half * correct);
                    break;

                }
                case 2:
                {
                    float half = defaultPercent - 0.5f;
                    float correct = (1f - nightTimePercentage) / 0.5f;
                    currentPercent = 0.5f + (half * correct);
                    break;
                }
                case 3:
                {
                    float half = 1f - defaultPercent;
                    float correct = nightTimePercentage / 0.5f;
                    currentPercent = 1f - (half * correct);
                    break;
                }
            }
        }

        #endregion

#if UNITY_EDITOR
        void OnGUI()
        {
            if (!isDebug)
                return;

            GUI.Label(new Rect(10, 10, 800, 80), debugLightBehavioursLog + " " + lightSkyBehaviours.Length.ToString());
            GUI.Label(new Rect(10, 40, 200, 20), "HazeIntensity: " + hazeCore.hazeIntensity);

            Color temp = GUI.color;
            GUI.color = hazeCore.hazeColor;
            GUI.Button(new Rect(220, 40, 40, 20), "", UnityEditor.EditorStyles.helpBox);
            GUI.color = GUI.contentColor;

            GUI.Label(new Rect(10, 60, 160, 20), "horizon " + intensityHorizon);
            GUI.HorizontalSlider(new Rect(150, 65, 50, 20), intensityHorizon, 0, 1);
            GUI.Label(new Rect(210, 60, 160, 20), "direction " + intensityDirection);
            GUI.HorizontalSlider(new Rect(360, 65, 50, 20), intensityDirection, 0, 1);
            for (int i = 0; i < lightSkyBehaviours.Length; i++)
            {
                if (lightSkyBehaviours[i] != null)
                {
                    GUI.Label(new Rect(55, 115 + 20 * i, 400, 20), lightSkyBehaviours[i].name + " " + DebugLight(i));
                    temp = GUI.color;
                    GUI.color = lightSkyBehaviours[i].color;
                    GUI.Button(new Rect(10, 115 + 20 * i, 40, 20), "", UnityEditor.EditorStyles.helpBox);
                    GUI.color = GUI.contentColor;
                }
            }
        }
#endif

        private void OnDrawGizmos()
        {
            if (context.useCelestials && CelestialsTransform)
            {
                Gizmos.color = Color.yellow;

                Gizmos.DrawLine(CelestialsTransform.transform.position - Vector3.forward * 50f, CelestialsTransform.transform.position + Vector3.forward * 50f);
                Gizmos.DrawLine(CelestialsTransform.transform.position - Vector3.right * 50f, CelestialsTransform.transform.position + Vector3.right * 50f);
                Gizmos.DrawLine(CelestialsTransform.transform.position, CelestialsTransform.transform.position + Vector3.up * 50f);
            }
        }
    }
}
