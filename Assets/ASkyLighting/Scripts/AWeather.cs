using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOD;
using System;
using System.Reflection;

namespace TOD
{
    [Serializable]
    public class AWeather
    {
        public bool updateWeather = true;

        [SerializeField]
        public AClouds cloudSystem;

        public List<AWeatherPreset> currentWeatherPresets = new List<AWeatherPreset>();

        public AWeatherPreset startWeatherPreset;

        public AWeatherPreset currentActiveWeatherPreset;
        public AWeatherPreset currentActiveWeatherPreset02;

        public WindZone windZone;

        [HideInInspector] public AWeatherPreset lastActiveWeatherPreset;
        [HideInInspector] public bool weatherFullyChanged = false;

        public void UpdateWeather()
        {

            if (currentActiveWeatherPreset != currentActiveWeatherPreset02)
            {
                lastActiveWeatherPreset = currentActiveWeatherPreset;
                currentActiveWeatherPreset = currentActiveWeatherPreset02;

                if (currentActiveWeatherPreset != null)
                {
                    weatherFullyChanged = false;
                }
            }


            cloudSystem.UpdateClouds(currentActiveWeatherPreset, true);

            if (!weatherFullyChanged)
                CalcWeatherTransitionState();
        }

        public void SetupShader(Transform windZone, float time, Vector3 sunDir, Vector3 munDir)
        {
            float windStrenght = 0;

            if (currentActiveWeatherPreset != null)
                windStrenght = currentActiveWeatherPreset.WindStrenght;

            cloudSystem.CloudsShaderUpdate(windZone, windStrenght, time, sunDir, munDir);
        }

        void CalcWeatherTransitionState()
        {
            bool changed = false;

            // First Layer
            if (currentActiveWeatherPreset.cloudConfig.Count > 0 && cloudSystem.cloudsLayers.Count > 0)
            {
                if ((cloudSystem.cloudsLayers[0].Coverage >= currentActiveWeatherPreset.cloudConfig[0].Coverage - 0.01f &&
                    cloudSystem.cloudsLayers[0].Coverage <= ASkyLighting._instance.context.weather.currentActiveWeatherPreset.cloudConfig[0].Coverage
                    + 0.01f) || cloudSystem.cloudsLayers[0].Coverage <= 0f)
                    changed = true;
                else
                    changed = false;
            }
            else if (cloudSystem.cloudsLayers.Count > 0)
            {
                if (cloudSystem.cloudsLayers[0].Coverage <= 0f)
                    changed = true;
                else
                    changed = false;
            }
            else
            {
                changed = true;
            }

            weatherFullyChanged = changed;
        }

        public void ChangeWeather(int weatherId)
        {
            if (weatherId < 0 || weatherId > currentWeatherPresets.Count)
                return;

            if (currentWeatherPresets[weatherId] != currentActiveWeatherPreset)
            {
                currentActiveWeatherPreset02 = currentWeatherPresets[weatherId];
            }
        }

        public void UpdateClouds()
        {
            if (cloudSystem.useCloudShadows)
            {
                if (cloudSystem.useMovingShadows)
                    cloudSystem.UpdateCloudDir(ASkyLighting._instance.GetDirectionalLight().transform);
            }

            UpdateWeather();
            SetupShader(windZone.transform, ASkyLighting.CGTime, ASkyLighting._instance.SunDirection, -ASkyLighting._instance.m_MoonTransform[1].forward);

            if (cloudSystem.useCloudShadows)
            {
                Vector3 camPos = ASkyLighting._instance.transform.position;
                cloudSystem.cloudShadow.transform.position = new Vector3(camPos.x, camPos.y + 70f, camPos.z);
            }

            
        }

        public void CopyFrom(AWeather other)
        {
            if (other == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("WeatherCopy Error");
#endif
                return;
            }

            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }

            cloudSystem.CopyFrom(cloudSystem);
        }
    }

    [Serializable]
    public class AClouds
    {
        [SerializeField]
        public ACloudsSettings cloudsSettings = new ACloudsSettings();
        public List<ACloudsLayer> cloudsLayers = new List<ACloudsLayer>();

        public Projector cloudShadow;
        public Vector4 cloudsShadowTime = Vector4.zero;

        public Vector2 cloudAnim;
        public List<bool> showCloudLayer = new List<bool>();
        public Transform Clouds;

        [Range(0.1f, 1f)]
        [Tooltip("Defines the speed of clouds will change when weather conditions changed.")]
        public float cloudTransitionSpeed = 1f;

        public bool isCloudPresets;
        public bool isCloudLayers;
        public bool isCloudGlobal;

        public bool cloudsHDR = true;
        public bool useBakeClouds = true;
        public bool useCloudSpeed = true;
        public bool useCloudShadows = true;
        public bool useMovingShadows = true;

        [Range(1f, 3f)]
        public float worldScale = 1.25f;
        [Tooltip("When enabled, clouds will stay at this height. When disabled clouds height will be calculated by player position.")]
        public bool fixedAltitude = false;
        [Tooltip("The altitude of the clouds when 'FixedAltitude' is enabled.")]
        public float cloudsAltitude = 0f;

        public Vector4 LightCloudDir = Vector4.zero;

        public void CreateShowCloudLayers()
        {
            showCloudLayer.Clear();
            for (int i = 0; i<cloudsSettings.cloudsLayersVariables.Count; i++)
                showCloudLayer.Add(false);
        }

        public void CloudsShaderUpdate(Transform windZone, float windStrenght, float time, Vector3 sunDir, Vector3 munDir)
        {
            if (cloudsSettings.useWindZoneDirection)
            {
                cloudsSettings.cloudsWindDirectionX = windZone.transform.forward.x;
                cloudsSettings.cloudsWindDirectionY = windZone.transform.forward.z;
            }

            if (useCloudSpeed)
            {
                cloudAnim += new Vector2(((cloudsSettings.cloudsTimeScale * (windStrenght * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsWindStrengthModificator)
                    * Time.deltaTime, ((cloudsSettings.cloudsTimeScale * (windStrenght * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsWindStrengthModificator) 
                    * Time.deltaTime);

                if (cloudAnim.x > 1f)
                    cloudAnim.x = -1f;
                else if (cloudAnim.x < -1f)
                    cloudAnim.x = 1f;

                if (cloudAnim.y > 1f)
                    cloudAnim.y = -1f;
                else if (cloudAnim.y < -1f)
                    cloudAnim.y = 1f;

                if (cloudAnim.x == 0)
                    cloudAnim.x = 0.1f;
                if (cloudAnim.y == 0)
                    cloudAnim.y = 0.1f;
            }

            cloudsSettings.skyColor.Evaluate(time);
            cloudsSettings.sunHighlight.Evaluate(time);
            cloudsSettings.moonHighlight.Evaluate(time);
            cloudsSettings.lightIntensity.Evaluate(time);

            for (int i = 0; i < cloudsSettings.cloudsLayersVariables.Count; i++)
            {
                cloudsLayers[i].myMaterial.SetFloat("_Offset", cloudsSettings.cloudsLayersVariables[i].LayerOffset);
                cloudsLayers[i].myMaterial.SetFloat("_Scale", 4000 * cloudsSettings.cloudsLayersVariables[i].Scaling);


                cloudsLayers[i].myMaterial.SetColor("_BaseColor", cloudsLayers[i].FirstColor);
                cloudsLayers[i].myMaterial.SetColor("_SkyColor", cloudsSettings.skyColor.color);
                cloudsLayers[i].myMaterial.SetColor("_MoonColor", cloudsSettings.moonHighlight.color);
                cloudsLayers[i].myMaterial.SetColor("_SunColor", cloudsSettings.sunHighlight.color);
                cloudsLayers[i].myMaterial.SetFloat("_CloudCover", cloudsLayers[i].Coverage);
                cloudsLayers[i].myMaterial.SetFloat("_Density", cloudsLayers[i].Density);
                cloudsLayers[i].myMaterial.SetFloat("_CloudAlpha", cloudsLayers[i].Alpha);
                if (useCloudSpeed)
                    cloudsLayers[i].myMaterial.SetVector("_timeScale", cloudAnim);
                cloudsLayers[i].myMaterial.SetFloat("_lightIntensity", cloudsSettings.lightIntensity.value);
                cloudsLayers[i].myMaterial.SetFloat("_direct", cloudsLayers[i].DirectLightIntensity);

                cloudsLayers[i].myMaterial.SetFloat("_solarTime", 0);
                float hdrClouds = cloudsHDR ? 1f : 0f;
                cloudsLayers[i].myMaterial.SetFloat("_hdr", hdrClouds);

                cloudsLayers[i].myMaterial.SetVector("_SunDirection", sunDir);
                cloudsLayers[i].myMaterial.SetVector("_MoonDirection", munDir);

                if (useCloudShadows && cloudShadow != null && i == 0)
                {
                    cloudShadow.material.SetVector("_Pos", new Vector4(cloudShadow.transform.position.x, cloudShadow.transform.position.z, 0, 0));
                    cloudsShadowTime.z = time;
                    cloudShadow.material.SetVector("_OffsetTime", cloudsShadowTime);
                    cloudShadow.material.SetFloat("_Offset", cloudsSettings.cloudsLayersVariables[i].LayerOffset);
                    cloudShadow.material.SetFloat("_Scale", 4000 * cloudsSettings.cloudsLayersVariables[i].Scaling);
                    cloudShadow.material.SetFloat("_CloudCover",cloudsLayers[i].Coverage + cloudsSettings.cloudShadowsSize);
                    cloudShadow.material.SetFloat("_CloudAlpha", cloudsLayers[i].Alpha);
                    cloudShadow.material.SetFloat("_ShadowPower", cloudsSettings.cloudShadowsPower);
                    if (useCloudSpeed)
                        cloudShadow.material.SetVector("_timeScale", cloudAnim);
                    else
                        cloudShadow.material.SetVector("_timeScale", new Vector2(0.1f, 0.1f));

                    if (useMovingShadows)
                        cloudShadow.material.SetVector("_LightDir", LightCloudDir);
                    else
                        cloudShadow.material.SetVector("_LightDir", Vector4.zero);
                }

            }
        }

        public void UpdateClouds(AWeatherPreset i, bool withTransition)
        {
            if (i == null)
                return;

            float speed = 500f * Time.deltaTime;

            if (withTransition)
                speed = cloudTransitionSpeed * Time.deltaTime;

            for (int q = 0; q < cloudsLayers.Count; q++)
            {
                if (i.cloudConfig.Count > q)
                {
                    cloudsLayers[q].FirstColor = Color.Lerp(cloudsLayers[q].FirstColor, i.cloudConfig[q].BaseColor, speed);
                    cloudsLayers[q].DirectLightIntensity = Mathf.Lerp(cloudsLayers[q].DirectLightIntensity, i.cloudConfig[q].DirectLightInfluence, speed);
                    cloudsLayers[q].Coverage = Mathf.Lerp(cloudsLayers[q].Coverage, i.cloudConfig[q].Coverage, speed);
                    cloudsLayers[q].Density = Mathf.Lerp(cloudsLayers[q].Density, i.cloudConfig[q].Density, speed);
                    cloudsLayers[q].Alpha = Mathf.Lerp(cloudsLayers[q].Alpha, i.cloudConfig[q].Alpha, speed);
                }
                else
                {
                    cloudsLayers[q].Density = Mathf.Lerp(cloudsLayers[q].Density, 0f, speed);
                    cloudsLayers[q].Coverage = Mathf.Lerp(cloudsLayers[q].Coverage, -1f, speed);
                    cloudsLayers[q].Alpha = Mathf.Lerp(cloudsLayers[q].Alpha, 0.5f, speed);
                }
            }
        }

        public void UpdateCloudDir(Transform directionalLight)
        {
            float altitude = (!fixedAltitude) ? 600f : cloudsAltitude;
            float firstAngle = Mathf.Deg2Rad * (90.0f - directionalLight.eulerAngles.x);
            float c1 = altitude / Mathf.Cos(firstAngle);
            float a1 = Mathf.Sqrt(c1 * c1 - altitude * altitude);

            float secondAngle = Mathf.Deg2Rad * (90.0f - directionalLight.eulerAngles.y);
            float c2 = a1 * -Mathf.Cos(secondAngle);
            float c3 = a1 * -Mathf.Sin(secondAngle);
            float scale = 5000f;
            LightCloudDir = new Vector4(c2 / scale, c3 / scale, 0, 0);
        }

        public void CopyFrom(AClouds other)
        {
            if (other == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("WeatherCopy Error");
#endif
                return;
            }

            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }

            cloudsSettings.CopyFrom(cloudsSettings);
        }

        public void InitClouds(ASkyLighting timeOfDayManager)
        {
            if (timeOfDayManager.context.weather.windZone == null)
                timeOfDayManager.context.weather.windZone = GameObject.FindObjectOfType<WindZone>();

            if (Clouds == null)
            {
                Clouds = timeOfDayManager.transform.Find("CloudsRoot");
                
                if (Clouds == null)
                {
                    GameObject cloudsGO = new GameObject();
                    if (timeOfDayManager.isHide)
                        cloudsGO.hideFlags = HideFlags.HideAndDontSave;
                    cloudsGO.transform.parent = timeOfDayManager.transform;
                    cloudsGO.name = "CloudsRoot";
                    cloudsGO.transform.position = new Vector3(0, timeOfDayManager.context.weather.cloudSystem.cloudsAltitude,0);
                    cloudsGO.transform.localScale = new Vector3(4000f, 4000f, 4000f);
                    Clouds = cloudsGO.transform;
                }
            }
            int childs = Clouds.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(Clouds.GetChild(i).gameObject);
            }

            cloudsLayers = new List<ACloudsLayer>();

            for (int i = 0; i < cloudsSettings.cloudsLayersVariables.Count; i++)
                cloudsLayers.Add(ACloudsLayer.Create(this, i, Clouds, timeOfDayManager.context.cloudRenderingLayer));

            DayPart start = Array.Find(timeOfDayManager.dayParts, obj => obj.state == DayName.Morning);
            cloudsShadowTime.x = (start.percent.x + start.percent.y) / 2;
            DayPart end = Array.Find(timeOfDayManager.dayParts, obj => obj.state == DayName.Evening);
            cloudsShadowTime.y = (end.percent.x + end.percent.y) / 2;
            cloudsShadowTime.w = (cloudsShadowTime.x + cloudsShadowTime.y) / 2;
        }
        
        public void SwitchClouds(bool isActive)
        {
            if (isActive)
            {
                if (Application.isPlaying)
                {
                    InitClouds(ASkyLighting._instance);
                    
                    if (!Clouds.gameObject.activeSelf)
                        Clouds.gameObject.SetActive(true);
                }
            }
            else
            {
                if (Clouds!=null)
                    if (Clouds.gameObject.activeSelf)
                        Clouds.gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    public class ACloudsLayer
    {
        public GameObject myObj;
        public Material myMaterial;
        public Material myShadowMaterial;
        public float DirectLightIntensity = 10f;
        public Color FirstColor = Color.white;
        public float Coverage = 0f; 
        public float Density = 0f;
        public float Alpha = 0f;

        public static ACloudsLayer Create(AClouds cloudSystem, int id, Transform cloudsRoot, int cloudRenderingLayer)
        {
            GameObject layer = new GameObject();
            layer.name = "Clouds Layer: " + id.ToString();
            layer.transform.SetParent(cloudsRoot);
            layer.transform.localEulerAngles = new Vector3(0f, 0f, -180f);
            layer.transform.localScale = new Vector3(1f, 1f, 1f);
            layer.transform.localPosition = new Vector3(0f, cloudSystem.cloudsSettings.cloudsLayersVariables[id].layerAltitude, 0f);
            layer.layer = cloudRenderingLayer;

            ACloudsLayer newLayer = new ACloudsLayer();
            newLayer.myObj = layer;
            MeshFilter layerMeshFilter = layer.AddComponent<MeshFilter>();
            MeshRenderer layerMeshRenderer = layer.AddComponent<MeshRenderer>();
            newLayer.myMaterial = new Material(ASkyLighting._instance.skyboxClouds);
            layerMeshRenderer.sharedMaterial = newLayer.myMaterial;
            layerMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            layerMeshFilter.sharedMesh = cloudSystem.cloudsSettings.CreateCloudsLayer(id, false);
            newLayer.myMaterial.SetTexture("_CloudsMap", cloudSystem.cloudsSettings.cloudsLayersVariables[id].myCloudsTexture);
            layerMeshRenderer.sharedMaterial.renderQueue = 3000 + cloudSystem.cloudsSettings.cloudsLayersVariables.Count - id;

            return newLayer;
        }
    }

    [Serializable]
    public class ACloudsLayerVariables
    {
        public string Name;
        [Range(1, 100)]
        public int Quality = 25;
        public int segmentCount = 3;
        public float thickness = 0.4f;
        public bool curved;
        public float curvedIntensity = 0.1f;

        public Texture myCloudsTexture;
        [Range(0.5f, 2f)]
        public float Scaling = 1f;
        public bool canCastShadows = false;
        public float layerAltitude = 0.1f;
        public float LayerOffset = 0.5f;

        public void CopyFrom(ACloudsLayerVariables other)
        {
            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }
        }
    }

    [Serializable]
    public class ACloudsSettings
    {
        public bool isSelect = false;

        [SerializeField]
        public List<ACloudsLayerVariables> cloudsLayersVariables = new List<ACloudsLayerVariables>();

        public float cloudsTimeScale = 1f;
        public float cloudsWindStrengthModificator = 0.001f;
        public int cloudQuality = 1;

        public bool useWindZoneDirection;
        public float cloudsWindDirectionX = 1f;
        public float cloudsWindDirectionY = 1f;

        public ColorGradient skyColor;
        public ColorGradient sunHighlight;
        public ColorGradient moonHighlight;
        public FloatCurve lightIntensity;
        public FloatCurve cloudsDissapear;

        public float cloudShadowsPower = 0.4f;
        public float cloudShadowsSize = 0.1f;

        public Mesh CreateCloudsLayer(int layerID, bool isShadowMesh)
        {
            int sliceQuality = 1;

            if (!isShadowMesh)
                sliceQuality = cloudsLayersVariables[layerID].Quality;

            //Setting arrays up
            Vector3[] vertices = new Vector3[(cloudsLayersVariables[layerID].segmentCount * cloudsLayersVariables[layerID].segmentCount) * sliceQuality];
            Vector2[] uvMap = new Vector2[vertices.Length];
            int[] triangleConstructor = new int[(cloudsLayersVariables[layerID].segmentCount - 1) * (cloudsLayersVariables[layerID].segmentCount - 1) * sliceQuality * 2 * 3];
            Color[] vertexColor = new Color[vertices.Length];
            float tempRatio = 1.0f / ((float)cloudsLayersVariables[layerID].segmentCount - 1);
            Vector3 posGainPerVertices = new Vector3(tempRatio * 2f, 1.0f / (Mathf.Clamp(sliceQuality - 1, 1, 999999)) * cloudsLayersVariables[layerID].thickness, tempRatio * 2f);
            float posGainPerUV = tempRatio;

            // Lets Create our mesh yea!
            int iteration = 0;
            int vIncrement = 0;
            int increment = 0;
            float curvature = 0.0f;

            float depthColor = -1.0f;
            float mirrorColor = 0.0f;
            //computes slices by vertices row, each time the row ends, do the next one.
            for (int s = 0; s < sliceQuality; s++)
            {
                depthColor = -1 + (s * (2 / (float)sliceQuality));

                if (s < sliceQuality * 0.5f)
                    mirrorColor = 0 + (1.0f / ((float)sliceQuality * 0.5f)) * s;
                else
                    mirrorColor = 2 - (1.0f / ((float)sliceQuality * 0.5f)) * (s + 1);

                if (sliceQuality == 1 || isShadowMesh)
                    mirrorColor = 1;
                //horizontal vertices
                for (int h = 0; h < cloudsLayersVariables[layerID].segmentCount; h++)
                {
                    int incrementV = cloudsLayersVariables[layerID].segmentCount * iteration;
                    //vertical vertices
                    for (int v = 0; v < cloudsLayersVariables[layerID].segmentCount; v++)
                    {

                        if (cloudsLayersVariables[layerID].curved)
                            curvature = Vector3.Distance(new Vector3(posGainPerVertices.x * v - 1f, 0.0f, posGainPerVertices.z * h - 1f), Vector3.zero);

                        if (sliceQuality == 1 || isShadowMesh)
                            vertices[v + incrementV] = new Vector3(posGainPerVertices.x * v - 1f, 0f + (Mathf.Pow(curvature, 2f) * cloudsLayersVariables[layerID].curvedIntensity), posGainPerVertices.z * h - 1f);
                        else
                        {
                            vertices[v + incrementV] = new Vector3(posGainPerVertices.x * v - 1f, posGainPerVertices.y * s - (cloudsLayersVariables[layerID].thickness / 2f) + (Mathf.Pow(curvature, 2f) * cloudsLayersVariables[layerID].curvedIntensity), posGainPerVertices.z * h - 1f);
                        }
                        uvMap[v + incrementV] = new Vector2(posGainPerUV * v, posGainPerUV * h);
                        vertexColor[v + incrementV] = new Vector4(depthColor, depthColor, depthColor, mirrorColor);
                    }
                    iteration += 1;

                    //Triangle construction
                    if (h >= 1)
                    {
                        for (int tri = 0; tri < cloudsLayersVariables[layerID].segmentCount - 1; tri++)
                        {
                            triangleConstructor[0 + increment] = (0 + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);//
                            triangleConstructor[1 + increment] = (cloudsLayersVariables[layerID].segmentCount + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);
                            triangleConstructor[2 + increment] = (1 + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);//
                            triangleConstructor[3 + increment] = ((cloudsLayersVariables[layerID].segmentCount + 1) + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);
                            triangleConstructor[4 + increment] = (1 + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);
                            triangleConstructor[5 + increment] = (cloudsLayersVariables[layerID].segmentCount + tri) + vIncrement + (s * cloudsLayersVariables[layerID].segmentCount);
                            increment += 6;
                        }
                        vIncrement += cloudsLayersVariables[layerID].segmentCount;
                    }
                }
            }
            if (!isShadowMesh)
            {
                Mesh slicedCloudMesh = new Mesh();
                slicedCloudMesh.Clear();
                slicedCloudMesh.name = "Clouds";
                slicedCloudMesh.vertices = vertices;
                slicedCloudMesh.triangles = triangleConstructor;
                slicedCloudMesh.uv = uvMap;
                slicedCloudMesh.colors = vertexColor;
                slicedCloudMesh.RecalculateNormals();
                slicedCloudMesh.RecalculateBounds();
                CalcMeshTangents(slicedCloudMesh);

                return slicedCloudMesh;
            }
            else
            {
                Mesh shadowMesh = new Mesh();
                shadowMesh.Clear();
                shadowMesh.name = "CloudsShadows";
                shadowMesh.vertices = vertices;
                shadowMesh.triangles = triangleConstructor;
                shadowMesh.uv = uvMap;
                shadowMesh.colors = vertexColor;
                shadowMesh.RecalculateNormals();
                shadowMesh.RecalculateBounds();
                CalcMeshTangents(shadowMesh);

                return shadowMesh;
            }
        }

        public static void CalcMeshTangents(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;

            int triangleCount = triangles.Length;
            int vertexCount = vertices.Length;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];

            Vector4[] tangents = new Vector4[vertexCount];

            for (long a = 0; a < triangleCount; a += 3)
            {
                long i1 = triangles[a + 0];
                long i2 = triangles[a + 1];
                long i3 = triangles[a + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uv[i1];
                Vector2 w2 = uv[i2];
                Vector2 w3 = uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }


            for (long a = 0; a < vertexCount; ++a)
            {
                Vector3 n = normals[a];
                Vector3 t = tan1[a];
                Vector3.OrthoNormalize(ref n, ref t);

                tangents[a].x = t.x;
                tangents[a].y = t.y;
                tangents[a].z = t.z;

                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }
            mesh.tangents = tangents;
        }

        public void CopyFrom(ACloudsSettings other)
        {
            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }

            for (int i=0; i< cloudsLayersVariables.Count; i++)
            {
                cloudsLayersVariables[i].CopyFrom(cloudsLayersVariables[i]);
            }

            skyColor.GetGradient(other.skyColor);
            sunHighlight.GetGradient(other.sunHighlight);
            moonHighlight.GetGradient(other.moonHighlight);
            lightIntensity.GetCurve(other.lightIntensity);
            cloudsDissapear.GetCurve(other.cloudsDissapear);
        }

    }

    

    

    
}
