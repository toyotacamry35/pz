// ReSharper disable NotAccessedField.Global
// ReSharper disable InconsistentNaming
using UnityEngine;
#if ENABLE_DOTTRACE        
using System;
using System.Collections;
using JetBrains.Profiler.Api;
using Core.Environment.Logging.Extension;
#endif

namespace Src.Debugging
{
    public class DotTraceController : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public KeyCode Key = KeyCode.ScrollLock;
        public Rect Area;
        public Color ReadyColor = Color.gray;
        public Color CollectingColor = Color.green;
        public float BlinkPeriod = 1;
        public AnimationCurve BlinkCurve;
        public Material Material;

#if ENABLE_DOTTRACE        
        private bool _collecting;
        private readonly WaitForEndOfFrame _waiter = new WaitForEndOfFrame();
        private static DotTraceController _instance;
        
        public static DotTraceController Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void OnEnable()
        {
            var features = MeasureProfiler.GetFeatures();
            var ready = (features & MeasureFeatures.Ready) != 0;
            var detach = (features & MeasureFeatures.Detach) != 0;
            
            if (ready)
                if (Logger.IsInfoEnabled)  Logger.IfInfo()?.Message("Dot Trace profiler is ready").Write();
            else
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Dot Trace profiler is NOT ready").Write();
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Dot Trace profiler detach is {(detach ? "" : "NOT ")}allowed").Write();

            if (ready)
            {
                StartCoroutine(RenderLoop());
            }
            else
                enabled = false;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            var alt = UnityEngine.Input.GetKey(KeyCode.LeftAlt) | UnityEngine.Input.GetKey(KeyCode.RightAlt);
            var ctrl = UnityEngine.Input.GetKey(KeyCode.LeftControl) | UnityEngine.Input.GetKey(KeyCode.RightControl);
            var shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) | UnityEngine.Input.GetKey(KeyCode.RightShift);

            if (UnityEngine.Input.GetKeyDown(Key) && ctrl && !alt && !shift)
            {
                if (!_collecting)
                {
                    StartProfiling();
                }
                else
                {
                    StopProfiling();
                }
            }
        }

        public void StartProfiling()
        {
            _collecting = true;
            MeasureProfiler.StartCollectingData();
            if (Logger.IsInfoEnabled)  Logger.IfInfo()?.Message("Start Collecting Data").Write();;
        }

        public void StopProfiling()
        {
            _collecting = false;
            MeasureProfiler.SaveData();
            if (Logger.IsInfoEnabled)  Logger.IfInfo()?.Message("Save Collectied Data").Write();;
        }

        private IEnumerator RenderLoop()
        {
            while (true)
            {
                yield return _waiter;
                Render();
            }
        }

        private void Render()
        {
            try
            {
                Color color;
                if (_collecting)
                {
                    float alphaFactor;
                    if (BlinkPeriod > 0)
                    {
                        float t = Time.unscaledTime % BlinkPeriod / BlinkPeriod;
                        alphaFactor = BlinkCurve.Evaluate(t);
                    }
                    else
                        alphaFactor = 1;

                    color = CollectingColor;
                    color.a *= alphaFactor;
                }
                else
                {
                    color = ReadyColor;
                }

                GL.PushMatrix();
                GL.LoadPixelMatrix();
                Material.SetPass(0);
                GL.Begin(GL.QUADS);
                GL.Color(color);
                DrawRect(TransformRect(Area));
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Exception").Write();
            }
            finally
            {
                GL.End();
                GL.PopMatrix();
            }

        }
        
        private Rect TransformRect(Rect rect)
        {
            return new Rect(
                Mathf.FloorToInt(Screen.width * rect.xMin) + 0.5f,
                Mathf.FloorToInt(Screen.height * rect.yMin),
                Mathf.CeilToInt(Screen.width * rect.width),
                Mathf.CeilToInt(Screen.height * rect.height));
        }
        
        private void DrawRect(Rect rect)
        {
            GL.TexCoord(Vector3.one);
            GL.Vertex(new Vector3(rect.xMax, rect.yMin));
            GL.Vertex(new Vector3(rect.xMin, rect.yMin));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax));
        }
#else
        public static DotTraceController Instance => null;
        
        public void StopProfiling()
        {}

        public void StartProfiling()
        {}
#endif
    }
}
