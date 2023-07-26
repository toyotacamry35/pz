using Assets.Src.Lib.ProfileTools;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTerrainGrass
{

    [System.Serializable]
    public enum RTSize
    {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(WindZone))]
    public class Wind : MonoBehaviour
    {

        [System.Serializable]
        public class WindVariant
        {
            public string name;

            public float windFrequency = 0.25f;

            public float foliagePrimaryStrenght = 1.0f;
            public float foliageSecondaryStrenght = 1.0f;
            public float foliageLeafTurbulence = 2.0f;
            public float foliageWaveSize = 2.0f;

            public float grassStrenght = 1.0f;
            public float grassSize = 0.01f;
            public float grassSpeed = 0.02f;

        }

        private int AfsFoliageWindPID;
        private int AfsFoliageWindPulsMagnitudeFrequencyPID;
        private int AfsFoliageWindMultiplierPID;
        private int AfsFoliageTimeFrequencyPID;
        private int AfsFoliageWaveSizePID;

        private int AfsTimeFrequencyPID;
        private int WavingTintCustomPID;
        private int WaveAndDistanceCustomPID;

        private float domainTime_Wind = 0.0f;
        private Vector3 WindDirection;
        private Vector3 WindPulseMagnitude;
        private float WindPulseFrenquency;
        private float WindTurbulence;
        private float temp_WindFrequency = 0.25f;
        private float GrassWindSpeed;

        private float domainTime_2ndBending = 0.0f;
        [HideInInspector]
        [Range(0.01f, 2.0f)]
        public float WindFrequency = 0.25f;
        //[Header("Foliage")]
        [HideInInspector]
        [Range(0.1f, 10.0f)]
        public float FoliagePrimaryStrenght = 1.0f;
        [HideInInspector]
        [Range(0.1f, 10.0f)]
        public float FoliageSecondaryStrenght = 1.0f;
        [HideInInspector]
        [Range(0.0f, 10.0f)]
        public float FoliageLeafTurbulence = 2.0f;
        [HideInInspector]
        [Range(0.1f, 25.0f)]
        public float FoliageWaveSize = 2.0f;

        

        [HideInInspector]
        public int currentWind = 0;
        //[HideInInspector]
        public List<WindVariant> windVariants;

        [Header("Wind Multipliers")]
        [HideInInspector]
        [Space(4)]
        public float GrassStrenght = 1.0f;
        [HideInInspector]
        public float Foliage = 1.0f;

        [Space(4)]
        [HideInInspector]
        [Range(0.001f, 0.1f)]
        public float size = 0.01f;
        [HideInInspector]
        [Range(0.0001f, 0.2f)]
        [Space(5)]
        public float speed = 0.02f;
        public float speedLayer0 = 0.476f;
        public float speedLayer1 = 1.23f;
        public float speedLayer2 = 2.93f;

        [Header("Noise")]
        [Space(4)]
        public int GrassGustTiling = 4;
        public float GrassGustSpeed = 0.278f;

        [Header("Jitter")]
        [Space(4)]
        public float JitterFrequency = 3.127f;
        public float JitterHighFrequency = 21.0f;

        [Header("Render Texture Settings")]
        [Space(4)]
        public RTSize Resolution = RTSize._512;
        public Texture WindBaseTex;
        public Shader WindCompositeShader;

        [Header("Visual Noise")]
        public Texture2D noiseVegetation;

        [Range(0.01f, 30.0f)]
        public float visualNoiseScale = 15f;
        [Range(0.01f, 10.0f)]
        public float visualLinesScale = 3.5f;
        [Range(0.0f, 1.0f)]
        public float visualLinesOffset = 0.3f;
        [Range(0.0f, 2.0f)]
        public float visualLinesPower = 1.0f;

        //
        private RenderTexture WindRenderTexture;
        private Material m_material;

        private Vector2 uvs = new Vector2(0, 0);
        private Vector2 uvs1 = new Vector2(0, 0);
        private Vector2 uvs2 = new Vector2(0, 0);
        private Vector2 uvs3 = new Vector2(0, 0);

        private WindZone windZone;
        private float mainWind;
        private float turbulence;

        private int AtgWindDirSizePID;
        private int AtgWindStrengthMultipliersPID;
        private int AtgSinTimePID;
        private int AtgGustPID;

        private int AtgWindUVsPID;
        private int AtgWindUVs1PID;
        private int AtgWindUVs2PID;
        private int AtgWindUVs3PID;

        private int AtgWindRTPID;

        Vector2 tempWindstrengths = Vector2.zero;

        private Vector4 WindDirectionSize = Vector4.zero;

#if UNITY_EDITOR        
        private Camera _sceneCamera;
#endif        

        void OnEnable()
        {
            if (WindCompositeShader == null)
            {
                WindCompositeShader = Shader.Find("WindComposite");
            }
            if (WindBaseTex == null)
            {
                WindBaseTex = Profile.Load<Texture>("Default wind base texture");
            }

            SetupRT();
            afsSyncFrequencies();
            GetPIDs();
            windZone = GetComponent<WindZone>();
            
            // fix Dimensions of color surface does not match dimensions of depth surface
            float time = Time.time;
            float delta = Time.deltaTime;
            AfsTransform(time, delta);
            RenderWind(time, delta);

#if UNITY_EDITOR
            foreach( var cam in Camera.allCameras)
                if (cam.name == "SceneCamera")
                    _sceneCamera = cam;
#endif
        }

        void SetupRT()
        {
            if (WindRenderTexture == null || m_material == null)
            {
                WindRenderTexture = new RenderTexture((int)Resolution, (int)Resolution, 0, RenderTextureFormat.ARGBHalf, /*ARGB32, /*,*/ RenderTextureReadWrite.Linear);
                WindRenderTexture.useMipMap = true;
                WindRenderTexture.wrapMode = TextureWrapMode.Repeat;
                m_material = new Material(WindCompositeShader);
            }
        }

        public void SelectWindVariant()
        {
            WindFrequency = windVariants[currentWind].windFrequency;
            FoliagePrimaryStrenght = windVariants[currentWind].foliagePrimaryStrenght;
            FoliageSecondaryStrenght = windVariants[currentWind].foliageSecondaryStrenght;
            FoliageLeafTurbulence = windVariants[currentWind].foliageLeafTurbulence;
            FoliageWaveSize = windVariants[currentWind].foliageWaveSize;
            GrassStrenght = windVariants[currentWind].grassStrenght;
            size = windVariants[currentWind].grassSize;
            speed = windVariants[currentWind].grassSpeed;
        }

        void afsSyncFrequencies()
        {
            temp_WindFrequency = WindFrequency;
            domainTime_Wind = 0.0f;
            domainTime_2ndBending = 0.0f;

        }

        void GetPIDs()
        {
            AtgWindDirSizePID = Shader.PropertyToID("_AtgWindDirSize");
            AtgWindStrengthMultipliersPID = Shader.PropertyToID("_AtgWindStrengthMultipliers");
            AtgSinTimePID = Shader.PropertyToID("_AtgSinTime");
            AtgGustPID = Shader.PropertyToID("_AtgGust");
            AtgWindUVsPID = Shader.PropertyToID("_AtgWindUVs");
            AtgWindUVs1PID = Shader.PropertyToID("_AtgWindUVs1");
            AtgWindUVs2PID = Shader.PropertyToID("_AtgWindUVs2");
            AtgWindUVs3PID = Shader.PropertyToID("_AtgWindUVs3");

            AfsFoliageWindPID = Shader.PropertyToID("_AfsFoliageWind");
            AfsFoliageWindPulsMagnitudeFrequencyPID = Shader.PropertyToID("_AfsFoliageWindPulsMagnitudeFrequency");
            AfsFoliageWindMultiplierPID = Shader.PropertyToID("_AfsFoliageWindMultiplier");
            AfsFoliageTimeFrequencyPID = Shader.PropertyToID("_AfsFoliageTimeFrequency");
            AfsFoliageWaveSizePID = Shader.PropertyToID("_AfsFoliageWaveSize");

            AfsTimeFrequencyPID = Shader.PropertyToID("_AfsTimeFrequency");
            WavingTintCustomPID = Shader.PropertyToID("_WavingTintCustom");
            WaveAndDistanceCustomPID = Shader.PropertyToID("_WaveAndDistanceCustom");

            AtgWindRTPID = Shader.PropertyToID("_AtgWindRT");
        }

        void OnValidate()
        {
            if (WindCompositeShader == null)
            {
                WindCompositeShader = Shader.Find("WindComposite");
            }
            if (WindBaseTex == null)
            {
                WindBaseTex = Profile.Load<Texture>("Default wind base texture");
            }
        }

        void AfsWind(float time, float delta)
        {
            float WindstrengthMagnitude = 0.0f;
            if (windZone.windPulseFrequency != 0.0f)
            {
                WindstrengthMagnitude = windZone.windPulseMagnitude * (1.0f + Mathf.Sin(time * windZone.windPulseFrequency) + 1.0f + Mathf.Sin(time * windZone.windPulseFrequency * 3.0f)) * 0.5f;
                WindPulseMagnitude = new Vector3(WindDirectionSize.x * mainWind * WindstrengthMagnitude, WindDirectionSize.y * mainWind * WindstrengthMagnitude, WindDirectionSize.z * mainWind * WindstrengthMagnitude); //WindStrength * m_WindZone.windPulseMagnitude;
            }
            else
            {
                WindPulseMagnitude = new Vector3(0, 0, 0);
            }

            Shader.SetGlobalVector(AfsFoliageWindPID, new Vector4(WindDirectionSize.x, WindDirectionSize.y, WindDirectionSize.z, Mathf.Max(0.0f, turbulence)));
            Shader.SetGlobalVector(AfsFoliageWindPulsMagnitudeFrequencyPID, new Vector4(WindPulseMagnitude.x, WindPulseMagnitude.y, WindPulseMagnitude.z, WindPulseFrenquency));
            Shader.SetGlobalVector(AfsFoliageWindMultiplierPID, new Vector4(FoliagePrimaryStrenght, FoliageSecondaryStrenght, 0.0f, 0.0f));

            Shader.SetGlobalFloat(AfsFoliageWaveSizePID, (0.5f / FoliageWaveSize));

            domainTime_Wind = (domainTime_Wind + temp_WindFrequency * delta * 2.0f); // % TwoPI; / % TwoPI causes hiccups
            domainTime_2ndBending = domainTime_2ndBending + delta;

            Shader.SetGlobalVector(AfsTimeFrequencyPID, new Vector4(domainTime_Wind, domainTime_2ndBending, 0.375f * (1.0f + temp_WindFrequency * FoliageLeafTurbulence), 0.193f * (1.0f + temp_WindFrequency * FoliageLeafTurbulence)));

            Shader.SetGlobalVector(AfsFoliageTimeFrequencyPID, new Vector4(domainTime_Wind, domainTime_2ndBending, 0.375f * (1.0f + FoliageLeafTurbulence), 0.193f * (1.0f + FoliageLeafTurbulence)));
            Shader.SetGlobalTexture(WavingTintCustomPID, noiseVegetation);

            Shader.SetGlobalVector(WaveAndDistanceCustomPID, new Vector4(visualNoiseScale, visualLinesScale, visualLinesOffset, visualLinesPower));
        }

        void AfsTransform(float time, float delta)
        {
            mainWind = windZone.windMain;
            turbulence = windZone.windTurbulence;

            WindDirectionSize.x = transform.forward.x;
            WindDirectionSize.y = transform.forward.y;
            WindDirectionSize.z = transform.forward.z;
            WindDirectionSize.w = size;

            AfsWind(time, delta);
        }

        void RenderWind(float time, float delta)
        {
            var windVec = new Vector2(WindDirectionSize.x, WindDirectionSize.z) * delta * speed;

            uvs -= windVec * speedLayer0;
            uvs.x = uvs.x - (int)uvs.x;
            uvs.y = uvs.y - (int)uvs.y;

            uvs1 -= windVec * speedLayer1;
            uvs1.x = uvs1.x - (int)uvs1.x;
            uvs1.y = uvs1.y - (int)uvs1.y;

            uvs2 -= windVec * speedLayer2;
            uvs2.x = uvs2.x - (int)uvs2.x;
            uvs2.y = uvs2.y - (int)uvs2.y;

            uvs3 -= windVec * GrassGustSpeed;
            uvs3.x = uvs3.x - (int)uvs3.x;
            uvs3.y = uvs3.y - (int)uvs3.y;

            //	Set global shader variables for grass and foliage shaders


            tempWindstrengths.x = GrassStrenght * mainWind;
            tempWindstrengths.y = Foliage * mainWind;

            Shader.SetGlobalVector(AtgWindDirSizePID, WindDirectionSize);

            Shader.SetGlobalVector(AtgWindStrengthMultipliersPID, tempWindstrengths);
            Shader.SetGlobalVector(AtgGustPID, new Vector2(GrassGustTiling, turbulence + 0.5f));
            //	Jitter frequncies and strength

            var jitter = new Vector4(Mathf.Sin(time * JitterFrequency), Mathf.Sin(time * JitterFrequency * 0.2317f + 2.0f * Mathf.PI), Mathf.Sin(time * JitterHighFrequency), turbulence * 0.1f);
            Shader.SetGlobalVector(AtgSinTimePID, jitter);

            //	Set UVs
            Shader.SetGlobalVector(AtgWindUVsPID, uvs);
            Shader.SetGlobalVector(AtgWindUVs1PID, uvs1);
            Shader.SetGlobalVector(AtgWindUVs2PID, uvs2);
            Shader.SetGlobalVector(AtgWindUVs3PID, uvs3);

            Graphics.Blit(WindBaseTex, WindRenderTexture, m_material);
            Shader.SetGlobalTexture(AtgWindRTPID, WindRenderTexture);
        }
        // Update is called once per frame
        void OnRenderObject()
        {
            float time = Time.time;
            float delta = Time.deltaTime;

            AfsTransform(time, delta);

#if UNITY_EDITOR
            if (Application.isPlaying && Camera.current != _sceneCamera)
#endif
            {
                RenderWind(time, delta);
            }
        }
    }
}
