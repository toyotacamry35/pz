using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TOD
{
    public enum ReflectionType
    {
        CupemapBlend = 0,
        Probe = 1,
    }
    [ExecuteInEditMode]
    public class ReflectionsBlender : MonoBehaviour
    {
        public ReflectionType reflectionType;
        public float offset = 0.02f;
        public Cubemap reflection;
        RenderTexture renderTexture;
        [Range(0, 1)]
        public float reflectionIntensity;

        public ReflectionProbe probe;
        Coroutine updater;

        Cubemap cube;

        float currentTime = 0;
        int index = 0;
        int indexNext = 0;

        private void OnEnable()
        {
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflection = reflection;
        }


        [ContextMenu("Switch")]
        public void SwitchRefl()
        {
            if (reflectionType == ReflectionType.CupemapBlend)
            {
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
                updater = StartCoroutine(ReflectionProbeUpdate());
                reflectionType = ReflectionType.Probe;
            }
            else
            {
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
                reflectionType = ReflectionType.CupemapBlend;
                StopCoroutine(updater);
            }
        }
        
        private void OnDisable()
        {
            if (reflectionType == ReflectionType.Probe)
            {
                StopCoroutine(updater);
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            }
        }
        /*
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 200, 20), debug);
        }
        */

        [ContextMenu("Start")]
        public void Start()
        {
            renderTexture = new RenderTexture(128, 128, 0, RenderTextureFormat.DefaultHDR);
            renderTexture.useMipMap = true;
            renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            RenderSettings.customReflection = reflection;

            cube = new Cubemap(128, TextureFormat.RGBAHalf, true);
        }
        
        private IEnumerator ReflectionProbeUpdate()
        {
            var waiter = new WaitForEndOfFrame();
            var currentSlice = -1;
            for (; ; )
            {
                for (int i = 0; i < 2; i++)
                    yield return waiter;

                currentSlice = probe.RenderProbe();
                DynamicGI.UpdateEnvironment();

                while (!probe.IsFinishedRendering(currentSlice))
                    yield return waiter;
            }
        }
        

        float GetPartLenght(int id)
        {
            DayPart dayPart = ASkyLighting._instance.dayParts[index];

            if (dayPart.percent.x > dayPart.percent.y)
            {

                float lengthEnd = 1.0f - dayPart.percent.x;
                float lengthStart = dayPart.percent.y;
                float length = lengthStart + lengthEnd;
                return length / 2;
            }
            else
            {
                float lenght = dayPart.percent.y - dayPart.percent.x;
                return lenght / 2;
            }
        }

        void AddRenderTexture(int index, int indexNext, float currentTime)
        {

            if (ASkyLighting.eclipsePower > 0 && ASkyLighting._instance.context.isEclipse)
            {
                if (ASkyLighting.eclipsePower == 1)
                {
                    ReflectionProbe.BlendCubemap(ASkyLighting._instance.nightParts.reflections, ASkyLighting._instance.nightParts.reflections, currentTime, renderTexture);
                    Graphics.CopyTexture(renderTexture, RenderSettings.customReflection);
                }
                else
                {
                    ReflectionProbe.BlendCubemap(ASkyLighting._instance.dayParts[index].reflections, ASkyLighting._instance.dayParts[indexNext].reflections, currentTime, renderTexture);
                    Graphics.CopyTexture(renderTexture, cube);
                    ReflectionProbe.BlendCubemap(cube, ASkyLighting._instance.nightParts.reflections, ASkyLighting.eclipsePower, renderTexture);
                    Graphics.CopyTexture(renderTexture, RenderSettings.customReflection);

                }
            }
            else
            {
                ReflectionProbe.BlendCubemap(ASkyLighting._instance.dayParts[index].reflections, ASkyLighting._instance.dayParts[indexNext].reflections, currentTime, renderTexture);
                Graphics.CopyTexture(renderTexture, RenderSettings.customReflection);
            }

        }

        public void Update()
        {
            if (reflectionType == ReflectionType.Probe)
                return;
            if (ASkyLighting._instance == null)
                return;

            if (RenderSettings.defaultReflectionMode == UnityEngine.Rendering.DefaultReflectionMode.Skybox)
            {
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
                RenderSettings.customReflection = reflection;
            }

            RenderSettings.reflectionIntensity = reflectionIntensity;
            float time = ASkyLighting.CGTime;
            
            Utils.GetDayElementCurrent(ASkyLighting._instance.dayParts, out currentTime, out index);

            DayPart dayPart = ASkyLighting._instance.dayParts[index];

            if (dayPart.percent.x > dayPart.percent.y)
            {
                if ((time < dayPart.percent.y - offset) && (time > dayPart.percent.x + offset))
                    AddRenderTexture(index, index, 0.5f);
                
                else
                {
                    float normalizeLocal = -1;

                    if (time <= 0.5)
                    {
                        indexNext = index + 1;
                        float start = dayPart.percent.y - offset;
                        float currentLocalTime = time - start;
                        normalizeLocal = currentLocalTime / offset * 0.5f;

                        AddRenderTexture(index, indexNext, normalizeLocal);
                    }                    
                    else
                    {
                        indexNext = ASkyLighting._instance.dayParts.Length - 1;
                        float start = dayPart.percent.x + offset;
                        float currentLocalTime = time - start;
                        normalizeLocal = currentLocalTime / offset * 0.5f + 1.0f;

                        AddRenderTexture(indexNext, index, normalizeLocal);
                    }

                    
                }
            }
            else
            {
                if ((dayPart.percent.y - time > offset) && (time - dayPart.percent.x > offset))
                {
                    AddRenderTexture(index, index, 0.5f);
                }
                else
                {
                    float normalizeLocal = -1;

                    if (currentTime >= 0.5)
                    {
                        if (index == (ASkyLighting._instance.dayParts.Length - 1))
                            indexNext = 0;
                        else
                            indexNext = index + 1;

                        float start = dayPart.percent.y - offset;
                        float currentLocalTime = time - start;
                        normalizeLocal = currentLocalTime / offset * 0.5f;
                    }
                    else
                    {
                        if (index == 0)
                        {
                            index = ASkyLighting._instance.dayParts.Length - 1;
                            indexNext = index + 1;
                        }
                        else
                        {
                            indexNext = index;
                            index = index - 1;
                        }

                        float start = dayPart.percent.x;
                        float currentLocalTime = time - start;
                        normalizeLocal = currentLocalTime / offset * 0.5f + 0.5f;
                    }

                    AddRenderTexture(index, indexNext, normalizeLocal);

                }
            }

        }
    }
}
