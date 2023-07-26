using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class DebugTimelineChart : MonoBehaviour
    {
        public delegate Value ValueProviderDelegate(float dt);
        
        [HideInInspector][SerializeField] private string _id; 
        public string Id => _id;
        public string Name;
        public Material Material;
        public Rect Area = new Rect(1f / 20, 1f / 20, 18f / 20, 2f / 10);
        public Color Color = Color.green;
        public Color BackColor = new Color(0, 0, 0, 0.1f);
        public float OverRange = 0.5f;
        public Color OverColor = Color.yellow;
        public Mode _mode = Mode.Value;
        public UpdateType _updateType = UpdateType.External;
        public BoundSettings UpperBound;
        public BoundSettings LowerBound;
        public Line[] Lines;
        
        
        public enum Mode
        {
            Value,
            AbsValue,
            Delta,
            AbsDelta,
            Speed,
            AbsSpeed,
            DeltaAngle,
            AbsDeltaAngle,
            SpeedAngle,
            AbsSpeedAngle,
        }

        public enum UpdateType
        {
            External,
            Update,
            LateUpdate,
            FixedUpdate
        }

        public ValueProviderDelegate ValueProvider;

        private readonly Queue<float> _history = new Queue<float>(2000);
        private Value _prevValue;
        private float _minSample;
        private float _maxSample;
        private Coroutine _coroutine;
        private readonly WaitForEndOfFrame _waiter = new WaitForEndOfFrame();

        private int SamplesCount => Mathf.CeilToInt(Screen.width * Area.width);

        private void OnEnable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(RenderLoop());
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }

        private void Reset()
        {
            GenerateId();
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
                GenerateId();
        }
        
        [ContextMenu("Generate Id")]
        private void GenerateId()
        {
            _id = Guid.NewGuid().ToString();
        }
        
        private void Update()
        {
            if (_updateType == UpdateType.Update)
                InvokeProvider(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (_updateType == UpdateType.LateUpdate)
                InvokeProvider(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (_updateType == UpdateType.FixedUpdate)
                InvokeProvider(Time.fixedDeltaTime);
        }

        internal void SampleValue(Value value, float deltaTime)
        {
            if (deltaTime <= 0)
                return;

            switch (value.ValueType)
            {
                case Value.Type.Float:
                    SampleFloat(value.Float, _prevValue.Float, deltaTime);
                    break;
                case Value.Type.Vector3:
                    SampleVector(value.Vector, _prevValue.Vector, deltaTime);
                    break;
                case Value.Type.Bool:
                    SampleBool(value.Bool);
                    break;
                case Value.Type.Invalid:
                    SampleInternal(0);
                    break;
            }

            _prevValue = value;
        }

        private void SampleFloat(float value, float prevFloat, float deltaTime)
        {
            float sample;
            switch (_mode)
            {
                case Mode.Value:
                    sample = value;
                    break;
                case Mode.AbsValue:
                    sample = Mathf.Abs(value);
                    break;
                case Mode.Delta:
                    sample = value - prevFloat;
                    break;
                case Mode.AbsDelta:
                    sample = Mathf.Abs(value - prevFloat);
                    break;
                case Mode.Speed:
                    sample = (value - prevFloat) / deltaTime;
                    break;
                case Mode.AbsSpeed:
                    sample = Mathf.Abs((value - prevFloat) / deltaTime);
                    break;
                case Mode.DeltaAngle:
                    sample = Mathf.DeltaAngle(prevFloat, value);
                    break;
                case Mode.AbsDeltaAngle:
                    sample = Mathf.Abs(Mathf.DeltaAngle(prevFloat, value));
                    break;
                case Mode.SpeedAngle:
                    sample = Mathf.DeltaAngle(prevFloat, value) / deltaTime;
                    break;
                case Mode.AbsSpeedAngle:
                    sample = Mathf.Abs(Mathf.DeltaAngle(prevFloat, value)) / deltaTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SampleInternal(sample);
        }

        private void SampleVector(Vector3 vector, Vector3 prevVector, float deltaTime)
        {
            float sample;
            Vector3 dummy;
            switch (_mode)
            {
                case Mode.Value:
                case Mode.AbsValue:
                    sample = vector.magnitude;
                    break;
                case Mode.Delta:
                case Mode.AbsDelta:
                    sample = (vector - prevVector).magnitude;
                    break;
                case Mode.Speed:
                case Mode.AbsSpeed:
                    sample = (vector - prevVector).magnitude / deltaTime;
                    break;
                case Mode.DeltaAngle:
                case Mode.AbsDeltaAngle:
                    Quaternion.FromToRotation(prevVector, vector).ToAngleAxis(out sample, out dummy);
                    break;
                case Mode.SpeedAngle:
                case Mode.AbsSpeedAngle:
                    Quaternion.FromToRotation(prevVector, vector).ToAngleAxis(out sample, out dummy);
                    sample /= deltaTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SampleInternal(sample);
        }

        private void SampleBool(bool value)
        {
            SampleInternal(value ? 1 : 0);
        }
        
        private void SampleInternal(float sample)
        {
            _history.Enqueue(sample);

            int maxCount = SamplesCount;
            while (_history.Count > maxCount)
                _history.Dequeue();

            if (!float.IsNaN(sample))
            {
                if (sample > _maxSample)
                    _maxSample = sample;
                if (sample < _minSample)
                    _minSample = sample;
            }
        }

        private void InvokeProvider(float deltaTime)
        {
            if (ValueProvider != null)
                SampleValue(ValueProvider(deltaTime), deltaTime);
        }
        
        private void Clear()
        {
            _history.Clear();
            _maxSample = 0;
            _minSample = 0;
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
            var rect = new Rect(Mathf.FloorToInt(Screen.width * Area.xMin) + 0.5f,
                Mathf.FloorToInt(Screen.height * Area.yMin),
                Mathf.CeilToInt(Screen.width * Area.width),
                Mathf.CeilToInt(Screen.height * Area.height));
            try
            {
                GL.PushMatrix();
                GL.LoadPixelMatrix();
                Material.SetPass(0);
                GL.Clear(true, false, new Color(0, 0.1f, 0, 0));

                GL.Begin(GL.QUADS);
                GL.Color(BackColor);
                GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0));
                GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0));
                GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0));
                GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0));
                GL.End();

                int i = SamplesCount - _history.Count;
                if (_history.Count > 0)
                {
                    float upper;
                    switch (UpperBound.Type)
                    {
                        case BoundType.None:
                            upper = _maxSample;
                            break;
                        case BoundType.Limited:
                            upper = Mathf.Min(_maxSample, UpperBound.Value);
                            break;
                        case BoundType.Fixed:
                            upper = UpperBound.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    float lower;
                    switch (LowerBound.Type)
                    {
                        case BoundType.None:
                            lower = _minSample;
                            break;
                        case BoundType.Limited:
                            lower = Mathf.Max(_minSample, LowerBound.Value);
                            break;
                        case BoundType.Fixed:
                            lower = LowerBound.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    float range = upper - lower;
                    float factor = range > float.Epsilon ? 1 / range : 0;
                    float zeroValue = Mathf.Clamp01(-lower * factor);
                    _minSample = float.MaxValue;
                    _maxSample = float.MinValue;
                    
                    GL.Begin(GL.LINES);
                    GL.Color(Color);
                    foreach (var val in _history)
                    {
                        if (!float.IsNaN(val))
                        {
                            var value = (val - lower) * factor;
                            float normValue = Mathf.Clamp01(value);
                            bool over = value > 1;
                            
                            GL.Vertex(new Vector3(rect.xMin + i, rect.yMin + rect.height * zeroValue, 0));
                            if (over)
                                GL.Color(Color.Lerp(Color, OverColor, OverRange > 0 ? (value - 1) / OverRange : 1));
                            GL.Vertex(new Vector3(rect.xMin + i, rect.yMin + rect.height * normValue, 0));
                            if (over)
                                GL.Color(Color);
 
                            if (val < _minSample)
                                _minSample = val;
                            if (val > _maxSample)
                                _maxSample = val;
                        }
                        ++i;
                    }

                    foreach (var line in Lines)
                    {
                        GL.Color(line.Color);
                        float normValue = line.Value * factor;
                        if (normValue >= 0 && normValue <= 1)
                        {
                            var h = rect.yMin + rect.height * normValue;
                            GL.Vertex(new Vector3(rect.xMin, h, 0));
                            GL.Vertex(new Vector3(rect.xMax, h, 0));
                        }
                    }
                }
            }
            finally
            {
                GL.End();
                GL.PopMatrix();
            }
        }
        
        
        public struct Value
        {
            public enum Type { Invalid, Float, Vector3, Bool }
            
            private readonly Vector3 _vector;
            public readonly Type ValueType;

            public float Float => _vector.x;
            public Vector3 Vector => _vector;
            public bool Bool => _vector.x > 0;

            public Value(float value)
            {
                ValueType = Type.Float;
                _vector = new Vector3(value, 0, 0);
            }

            public Value(Vector3 value)
            {
                ValueType = Type.Vector3;
                _vector = value;
            }
            
            public Value(bool value)
            {
                ValueType = Type.Bool;
                _vector = new Vector3(value ? 1 : 0, 0, 0);
            }            
        }
        
        public enum BoundType { None, Limited, Fixed }

        [Serializable]
        public struct Line
        {
            public float Value;
            public Color Color;
        }
        
        [Serializable]
        public struct BoundSettings
        {
            public BoundType Type;
            public float Value;
        }
    }

    public static class DebugTimelineChartExtensions
    {
        public static void SetValueProvider(this DebugTimelineChart c, DebugTimelineChart.ValueProviderDelegate provider)
        {
            if (c)
                c.ValueProvider = provider;
        }

        public static void SetValueProvider(this DebugTimelineChart c, Func<float,float> provider)
        {
            if (c)
                c.ValueProvider = dt => new DebugTimelineChart.Value(provider(dt));
        }

        public static void SetValueProvider(this DebugTimelineChart c, Func<float,Vector3> provider)
        {
            if (c)
                c.ValueProvider = dt => new DebugTimelineChart.Value(provider(dt));
        }

        public static void SetValueProvider(this DebugTimelineChart c, Func<float,bool> provider)
        {
            if (c)
                c.ValueProvider = dt => new DebugTimelineChart.Value(provider(dt));
        }
        
        public static void ResetValueProvider(this DebugTimelineChart c)
        {
            if (c)
                c.ValueProvider = null;
        }
        
        public static void Sample(this DebugTimelineChart c, float value, float deltaTime)
        {
            if (c)
                c.SampleValue(new DebugTimelineChart.Value(value), deltaTime);
        }

        public static void Sample(this DebugTimelineChart c, Vector3 value, float deltaTime)
        {
            if (c)
                c.SampleValue(new DebugTimelineChart.Value(value), deltaTime);
        }

        public static void Sample(this DebugTimelineChart c, bool value, float deltaTime)
        {
            if (c)
                c.SampleValue(new DebugTimelineChart.Value(value), deltaTime);
        }
    }
}