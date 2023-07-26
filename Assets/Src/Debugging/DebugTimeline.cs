using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Src.Aspects;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Utils.DebugCollector;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;
using Event = SharedCode.Utils.DebugCollector.Event;
using EventType = SharedCode.Utils.DebugCollector.EventType;
using Vector2 = SharedCode.Utils.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Src.Debugging
{
    public class DebugTimeline : MonoBehaviour, ITarget
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    
        public Material Material;
        public Material TextMaterial;
        public DebugTimelineConfig Config;
        public int BufferCapacity = 1000;
        public int IntervalLength = 1000;
        public int CharsCountInTexture = 16;
        public int ZeroIndexInTexture = 1;
        
        private readonly Dictionary<int,EventHolder> _displayedEvents = new Dictionary<int,EventHolder>();
        private Collection _eventsCollection;
        private bool _paused;
        private bool _updateRequired;
        private ulong _currentTime;
        private (ulong, ulong) _pauseTimeRange;
        private bool _updateFilters;
        private Coroutine _coroutine;
        private readonly WaitForEndOfFrame _waiter = new WaitForEndOfFrame();

        private ulong Now => Collect.Instance.TimeNow;

        private void OnEnable()
        {
            if (!Material)
                 Logger.IfError()?.Message("Material not defined",  gameObject).Write();
            Collect.Instance.RegisterTarget(this);
            _eventsCollection = new Collection(BufferCapacity, IntervalLength);
            _currentTime = Now;
            _updateFilters = true;
            _paused = false;
            EntityInDebugFocus.Changed += OnEntityInDebugFocusChanged;
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(RenderLoop());
        }

        private void OnDisable()
        {
            Collect.Instance.UnregisterTarget(this);
            _eventsCollection = null;
            EntityInDebugFocus.Changed -= OnEntityInDebugFocusChanged;
            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        private void Update()
        {
            var now = Now;
            
            if (Config.GlobalFilter.Update(_updateFilters))
                _updateRequired = true;

            if (!Config)
                return;
            
            if (Config.GlobalFilter.Update(_updateFilters))
                _updateRequired = true;

            foreach (var rule in Config.Rules)
                if (rule.Update(_updateFilters))
                    _updateRequired = true;
            
            _updateFilters = false;
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Pause) && !UnityEngine.Input.GetKey(KeyCode.LeftShift) && !UnityEngine.Input.GetKey(KeyCode.RightShift))
            {
                _paused = !_paused;
                _currentTime = now;
                if (_paused)
                {
                    if (!_eventsCollection.Empty)
                    {
                        _eventsCollection.FinishAllUnfinishedEvents(_currentTime);
                        _pauseTimeRange = (_eventsCollection.GetEarliestEvent().BgnTime, _currentTime);
                    }
                }
                else
                {
                    _eventsCollection.Clear();
                }
            }

            if (!_paused)
            {
//                if (Math.Abs((long)_currentTime - (long)now) < 100)
//                    _currentTime += (ulong)Math.Round(Time.deltaTime * 1000); // более плавное движение... 
//                else
                    _currentTime = now;                                        // ...чем так
                
                _eventsCollection.UpdateUnfinishedEvents(_currentTime);
            }
            else
            {
                var speedMod = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift) ? 10 : 1;
                var step = (ulong) Mathf.Max(Config.TimeRewindSpeed * Config.TimeRange * speedMod, 1);
                if (UnityEngine.Input.GetKey(KeyCode.PageDown))
                {
                    _currentTime = Math.Max(_currentTime - step, _pauseTimeRange.Item1);
                    _updateRequired = true;
                }
                else if (UnityEngine.Input.GetKey(KeyCode.PageUp))
                {
                    _currentTime = Math.Min(_currentTime + step, _pauseTimeRange.Item2);
                    _updateRequired = true;
                }
            }

            if (_updateRequired)
            {
                _updateRequired = false;
                _displayedEvents.Clear();
                ConvertEvents(CurrentTimeRange, Config.GlobalFilter, Config.Rules, _eventsCollection, _displayedEvents);
            }
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
            if (!Material || !Config)
                return;

            QualitySettings.antiAliasing = 2;
            
            var rect = new Rect(
                Mathf.FloorToInt(Screen.width * Config.Area.xMin) + 0.5f,
                Mathf.FloorToInt(Screen.height * Config.Area.yMin),
                Mathf.CeilToInt(Screen.width * Config.Area.width),
                Mathf.CeilToInt(Screen.height * Config.Area.height));
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            Material.SetPass(0);
            GL.Clear(true, false, new Color(0, 0.1f, 0, 0));

            GL.Begin(GL.QUADS);
            GL.Color(Config.BackColor);
            GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0));
            GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0));
            GL.End();

            var timeRange = CurrentTimeRange;
            var (markBgn, markEnd) = (timeRange.Item1 / Config.TimeMarkPeriod, timeRange.Item2 / Config.TimeMarkPeriod + 1);
            var subMarkPeriod = Config.TimeMarkPeriod / Config.TimeSubMarks;

            GL.Begin(GL.QUADS);
            GL.Color(Config.SubMarkColor);
            for (ulong mark = markBgn; mark < markEnd; ++mark)
            {
                for(uint i = 0; i < Config.TimeSubMarks; i += 2)
                {
                    var (subMarkMin, subMarkMax) = (mark * Config.TimeMarkPeriod + i * subMarkPeriod, mark * Config.TimeMarkPeriod + (i + 1) * subMarkPeriod);
                    if(subMarkMax < timeRange.Item1 || subMarkMin > timeRange.Item2)
                        continue; 
                    var (xMin, xMax) = (CalcX(rect, subMarkMin, timeRange), CalcX(rect, subMarkMax, timeRange));
                    DrawRect(Rect.MinMaxRect(rect.xMin + xMin, rect.yMin, rect.xMin + xMax, rect.yMax));
                }
            }
            GL.End();

            GL.Begin(GL.QUADS);
            GL.Color(Config.MarkColor);
            for (ulong mark = markBgn; mark < markEnd; ++mark)
            {
                if ((mark & 1) == 0)
                    continue;
                var (markMin, markMax) = (mark * Config.TimeMarkPeriod, (mark + 1) * Config.TimeMarkPeriod);
                if(markMax < timeRange.Item1 || markMin > timeRange.Item2)
                    continue;
                var (xMin, xMax) = (CalcX(rect, markMin, timeRange), CalcX(rect, markMax, timeRange));
                DrawRect(Rect.MinMaxRect(rect.xMin + xMin, rect.yMin, rect.xMin + xMax, rect.yMax));
            }
            GL.End();

            try
            {
                if (_displayedEvents.Count != 0)
                {
                    int minRow = int.MaxValue;
                    int maxRow = int.MinValue;
                    foreach (var rule in Config.Rules)
                    {
                        if (rule != null)
                        {
                            minRow = Math.Min(minRow, rule.Row);
                            maxRow = Math.Max(maxRow, rule.Row);
                        }
                    }

                    GL.Begin(GL.QUADS);
                    try
                    { // Interval-события
                        foreach (var ev in _displayedEvents.Values)
                        {
                            if (ev.Event.Type != EventType.Interval)
                                continue;
                            if (ev.Event.EndTime < timeRange.Item1 || ev.Event.BgnTime > timeRange.Item2)
                                continue;
                            var (yMin, yMax) = CalcY(rect, ev.Rule.Row, minRow, maxRow);
                            var (xMin, xMax) = (CalcX(rect, ev.Event.BgnTime, timeRange), CalcX(rect, ev.Event.EndTime, timeRange));
                            var color = ev.Rule.Color;
                            color.a *= Config.BarsAlpha;
                            GL.Color(color);
                            DrawRect(Rect.MinMaxRect(rect.xMin + xMin, rect.yMin + yMin, rect.xMin + xMax, rect.yMin + yMax));
                        }
                    }
                    finally
                    {
                        GL.End();
                    }
                
                    GL.Begin(GL.QUADS);
                    try
                    { // Instant-события и начала Interval-событий
                        foreach (var ev in _displayedEvents.Values)
                        {
                            if (ev.Event.BgnTime < timeRange.Item1 || ev.Event.BgnTime > timeRange.Item2)
                                continue;
                            var (yMin, yMax) = CalcY(rect, ev.Rule.Row, minRow, maxRow);
                            float x = CalcX(rect, ev.Event.BgnTime, timeRange);
                            var color = ev.Event.Type == EventType.Instant ? ev.Rule.Color : Config.IntervalBgnColor; 
                            GL.Color(color);
                            DrawRect(Rect.MinMaxRect(rect.xMin + x, rect.yMin + yMin, rect.xMin + x + 1, rect.yMin + yMax));
                        }
                    }
                    finally
                    {
                        GL.End();
                    }
                    
                    if (_paused)
                    {
                        TextMaterial.SetPass(0);
                        GL.Begin(GL.QUADS);
                        try
                        { // Текст на interval-событиях
                            foreach (var ev in _displayedEvents.Values)
                            {
                                if (ev.Event.Type != EventType.Interval)
                                    continue;
                                if (ev.Event.EndTime < timeRange.Item1 || ev.Event.BgnTime > timeRange.Item2)
                                    continue;
                                var (yMin, yMax) = CalcY(rect, ev.Rule.Row, minRow, maxRow);
                                var (xMin, xMax) = (CalcX(rect, ev.Event.BgnTime, timeRange), CalcX(rect, ev.Event.EndTime, timeRange));
                                ulong time = (Math.Min(ev.Event.EndTime, _currentTime) - ev.Event.BgnTime);
                                var r = CalcNumberRect(time, new Vector2(rect.xMin + (xMin + xMax) * 0.5f, rect.yMin + (yMin + yMax) * 0.5f), (yMax - yMin) - Config.TextPadding * 2, Config.TextRatio, Config.TextSpacing);
                                GL.Color(Config.TextColor);
                                DrawNumber(time, r, Config.TextSpacing);
                            }
                        }
                        finally
                        {
                            GL.End();
                        }
                    }
                }
            }
            finally
            {
                GL.PopMatrix();
            }
        }

        private Rect CalcNumberRect(ulong value, Vector2 center, float height, float ratio, float spacing)
        {
            int count = Math.Max((int)Math.Floor(Math.Log10(value)) + 1, 1);
            float width = height * ratio * count + (count - 1) * spacing;
            return new Rect(center.x - width * 0.5f, center.y - height * 0.5f, width, height);
        }

        private void DrawNumber(ulong value, Rect rect, float spacing)
        {
            int count = Math.Max((int)Math.Floor(Math.Log10(value)) + 1, 1);
            float charWdt = (rect.width - (count - 1) * spacing) / count;
            for (int i = 0; i < count; ++i)
            {
                ulong nextValue = value / 10;
                int digit = (int)(value - nextValue * 10);
                value = nextValue;
                DrawDigit(digit, new Rect(rect.xMax - (i + 1) * charWdt - i * spacing, rect.yMin, charWdt, rect.height));
            }
        }

        private void DrawDigit(int digit, Rect rect)
        {
            var uv = new Rect(1.0f / CharsCountInTexture * (digit + ZeroIndexInTexture), 0, 1.0f / CharsCountInTexture, 1);
            GL.TexCoord(new Vector3(uv.xMax, uv.yMin));
            GL.Vertex(new Vector3(rect.xMax, rect.yMin));
            GL.TexCoord(new Vector3(uv.xMin, uv.yMin));
            GL.Vertex(new Vector3(rect.xMin, rect.yMin));
            GL.TexCoord(new Vector3(uv.xMin, uv.yMax));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax));
            GL.TexCoord(new Vector3(uv.xMax, uv.yMax));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax));
        }

        private void DrawRect(Rect rect)
        {
            GL.Vertex(new Vector3(rect.xMax, rect.yMin));
            GL.Vertex(new Vector3(rect.xMin, rect.yMin));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax));
        }

        private void OnEntityInDebugFocusChanged((IEntityPawn oldPawn, IEntityPawn newPawn) pawns)
        {
            _updateFilters = true;
        }

        private (float, float) CalcY(in Rect rect, int row, int minRow, int maxRow)
        {
            var rowsCount = maxRow - minRow + 1;
            row -= minRow;
            return (rect.height * (1 - (row + 1) / (float)rowsCount), rect.height * (1 - row / (float)rowsCount) - 1);
        }

        private float CalcX(in Rect rect, ulong timestamp, (ulong,ulong) timeRange)
        {
            if(timestamp < timeRange.Item1)
                return 0;
            if(timestamp > timeRange.Item2)
                return rect.width;
            return rect.width * (timestamp - timeRange.Item1) / (timeRange.Item2 - timeRange.Item1);
        }

        private static void ConvertEvents((ulong, ulong) timeRange, IFilter filter, Rule[] rules, Collection collection, Dictionary<int,EventHolder> events)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Update time range: {timeRange.Item1} {timeRange.Item2}").Write();
            foreach (var ev in collection.GetEvents(timeRange.Item1, timeRange.Item2))
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Event: {ev.Name} {ev.Type} {ev.BgnTime} {ev.EndTime}").Write();
                if (TryMatchRule(ev, filter, rules, out var rule))
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Event {ev.Name} {ev.Type} {ev.BgnTime} {ev.EndTime} with Rule {rule.Pattern}").Write();
                    if (!events.ContainsKey(ev.Uid))
                        events.Add(ev.Uid, new EventHolder(ev, rule));
                }
            }
        }

        private static bool TryMatchRule(Event ev, IFilter filter, Rule[] rules, out Rule rule)
        {
            if (!filter.IsValid || filter.IsMatch(ev))
                foreach (var r in rules)
                {
                    if (r.IsMatch(ev))
                    {
                        rule = r;
                        return true;
                    }
                }
            rule = null;
            return false;
        }
        
        private (ulong,ulong) CurrentTimeRange => (_currentTime - Config.TimeRange, _currentTime);

        private interface IFilter
        {
            bool IsMatch(in Event ev);
            
            bool IsValid { get; }
        }
        
        private readonly struct EventHolder
        {
            public readonly Event Event;
            public readonly Rule Rule;

            public EventHolder(Event @event, Rule rule)
            {
                Event = @event;
                Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            }
        }

        private class EntityChecker
        {
            private readonly bool _not;
            private readonly Guid _entityId;

            public EntityChecker(string entity)
            {
                if (entity == null)
                    _entityId = Guid.Empty;
                else
                {
                    entity = entity.Trim();
                    if (entity.StartsWith("!"))
                    {
                        _not = true;
                        entity = entity.Substring(1);
                    }

                    if (entity == string.Empty)
                        _entityId = Guid.Empty;
                    else
                    if (entity.Equals("player", StringComparison.OrdinalIgnoreCase))
                        _entityId = GameState.Instance?.CharacterRuntimeData?.CharacterId ?? Guid.Empty;
                    else 
                    if (entity.Equals("focus", StringComparison.OrdinalIgnoreCase))
                        _entityId = EntityInDebugFocus.EntityRef.Guid;
                    else
                        Guid.TryParse(entity, out _entityId);
                }
            }
            
            public bool IsMatch(Guid entityId)
            {
                var res = _entityId == Guid.Empty || entityId == Guid.Empty || entityId == _entityId;
                return _not ? !res : res;
            }
        }

        [Serializable]
        public class Filter : IFilter
        {
            public string Pattern;
            public bool IsRegex;
            public string Entity;

            private Regex _regex;
            private bool _isRegex;
            private string _pattern;
            private EntityChecker _entityId;
            private string _entity;

            public bool IsValid => _regex != null;

            public bool Update(bool force)
            {
                bool rv = false;
                if (force || _pattern != Pattern || _isRegex != IsRegex)
                {
                    _pattern = Pattern;
                    _isRegex = IsRegex;
                    _regex = _isRegex && !string.IsNullOrWhiteSpace(_pattern) ? new Regex(_pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled) : null;
                    rv = true;
                }

                if (force || _entity != Entity)
                {
                    _entity = Entity;
                    _entityId = !string.IsNullOrWhiteSpace(_entity) ? new EntityChecker(_entity) : null;
                    rv = true;
                }
                
                return rv;
            }
            
            public bool IsMatch(in Event ev)
            {
                if (_entityId != null && !_entityId.IsMatch(ev.Entity))
                    return false;
                if (_isRegex)
                    return _regex != null && _regex.IsMatch(ev.Name);
                return _pattern == ev.Name;
            }
        }

        [Serializable]
        public class Rule : Filter
        {
            public Color Color;
            public int Row;
        }

        void ITarget.AddInstantEvent(int eventId, string eventName, Guid entityId, ulong timestamp)
        {
            if(!_paused) // приостанавливаем сбор на время паузы, чтобы новые события не вытеснили текущие, пока мы их рассматриваем
                if (_eventsCollection.AddInstant(eventId, eventName, entityId, timestamp))
                    _updateRequired = true;
        }

        void ITarget.AddIntervalBgnEvent(int eventId, string eventName, object eventCauser, Guid entityId, ulong timestamp)
        {
            if (!_paused)
                if( _eventsCollection.AddIntervalBgn(eventId, eventName, eventCauser, entityId, timestamp) )
                    _updateRequired = true;
        }

        void ITarget.AddIntervalEndEvent(object eventCauser, ulong timestamp)
        {
            if (!_paused)
                if (_eventsCollection.AddIntervalEnd(eventCauser, timestamp))
                    _updateRequired = true;
        }
    }
}
