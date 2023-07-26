using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils.DebugCollector;

using TimeUnits = System.Int64;

namespace SharedCode.Utils
{
    public class CurveLoggerTarget : ITarget
    {
        private static readonly NLog.Logger NLogger = LogManager.GetLogger(nameof(CurveLoggerTarget));

        internal static CurveLoggerTarget Instance => _instance ?? (_instance = new CurveLoggerTarget());
        private static CurveLoggerTarget _instance;

        private static CurveLogger _logger;

        private readonly Dictionary<EventId, List<CurveData>> _curvesPool = new Dictionary<EventId, List<CurveData>>();
        // last given values
        private readonly Dictionary<string, float> _curvesBaseValuesGiver  = new Dictionary<string, float>();
        private readonly Dictionary<object, CurveData> _curvesByCausers = new Dictionary<object, CurveData>();
        private const float BaseValuesStep = 0.2f;
        private const float ValuesStep = 0.1f;
        private const int SafetyMaxPoolCount = 200;
        // for same event + ent.Id
        private const int SafetyMaxCurvesDuplicates = 10;


    // --- API -----------------------------------------------------------------------------------------------

        public static void RegisterTarget()
        {
            _logger = CurveLogger.Get("DbgCollectorTrgtLogger");
            _logger.Active = true;
            Collect.Instance.RegisterTarget(Instance);
        }

        public static void UnregisterTarget(bool dump)
        {
            Collect.Instance.UnregisterTarget(Instance);
            _logger.Active = false;
            if (dump)
                _logger.DumpDataToFile(false, Guid.NewGuid(), null);
        }

    // --- ITarget: ------------------------------------------------------------------------------------------


        ///#Dbg:
        private int Dbg_InstantEventCount;
        private int Dbg_IntervalBgnEventCount;
        private int Dbg_IntervalEndEventCount;

        public void AddInstantEvent(int eventId, string eventName, Guid entityId, ulong timestamp)
        {
            Dbg_InstantEventCount++;

            var curveData = GetFreeCurveData(entityId, eventName);
            if (curveData == null)
            {
                NLogger.IfError()?.Message($"{nameof(GetFreeCurveData)}({entityId}, {eventName}) returned null [{Dbg_InstantEventCount}, {Dbg_IntervalBgnEventCount}, {Dbg_IntervalEndEventCount}]").Write();
                return;
            }
            _logger?.IfActive?.AddData(curveData.CurveName, (TimeUnits)timestamp, curveData.CurrValue);
        }

        public void AddIntervalBgnEvent(int eventId, string eventName, object eventCauser, Guid entityId, ulong timestamp)
        {
            Dbg_IntervalBgnEventCount++;

            var curveData = GetFreeCurveData(entityId, eventName);
            if (curveData == null)
            {
                NLogger.IfError()?.Message($"{nameof(GetFreeCurveData)}({entityId}, {eventName}) returned null [{Dbg_InstantEventCount}, {Dbg_IntervalBgnEventCount}, {Dbg_IntervalEndEventCount}]").Write();
                return;
            }
            _curvesByCausers.Add(eventCauser, curveData);
            //_logger?.IfActive?.AddData(curveData.CurveName, (TimeUnits)timestamp, curveData.CurrValue);
            _logger?.IfActive?.AddData(curveData.CurveName, (TimeUnits)timestamp, curveData.Acquire(eventCauser));
        }

        public void AddIntervalEndEvent(object eventCauser, ulong timestamp)
        {
            Dbg_IntervalEndEventCount++;

            var curveData = EjectCurveDataFromByCauserDic(eventCauser);
            if (curveData == null)
            {
                NLogger.IfError()?.Message($"{nameof(EjectCurveDataFromByCauserDic)}({eventCauser}) returned null. [{Dbg_InstantEventCount}, {Dbg_IntervalBgnEventCount}, {Dbg_IntervalEndEventCount}]").Write();
                return;
            }

            //_logger?.IfActive?.AddData(curveData.CurveName, (TimeUnits)timestamp, curveData.CurrValue);
            _logger?.IfActive?.AddData(curveData.CurveName, (TimeUnits)timestamp, curveData.Release());
        }


    // --- Privates: ------------------------------------------------------------------------------------------

        CurveData GetFreeCurveData(Guid entId, string eventName)
        {
            var key = new EventId(entId, eventName);
            List<CurveData> curveDatas;
            if (!_curvesPool.GetOrCreate(key, out curveDatas, SafetyMaxPoolCount))
            {
                NLogger.IfError()?.Message($"{nameof(SafetyMaxPoolCount)}({SafetyMaxPoolCount}) is reached!").Write();
                return null;
            }

            CurveData cuData = GetOrCreateFreeDataFromList(curveDatas, eventName);
            return cuData;
        }

        // Create new one if all busy
        CurveData GetOrCreateFreeDataFromList(List<CurveData> curveDatas, string eventName)
        {
            for (int i = 0;  i < curveDatas.Count;  ++i)
                if (curveDatas[i].IsFree)
                    return curveDatas[i];

            // if no free data:

            if (curveDatas.Count >= SafetyMaxCurvesDuplicates)
            {
                NLogger.IfError()?.Message($"{nameof(SafetyMaxCurvesDuplicates)}({SafetyMaxCurvesDuplicates}) is reached! (curve: {curveDatas[0].CurveName})").Write();
                return null;
            }

            var tuple = GetNewCurveNameAndBaseValue(eventName);
            var newCurveData = new CurveData(tuple.Item1, tuple.Item2);
            curveDatas.Add(newCurveData);

            return newCurveData;
        }

        (string, float) GetNewCurveNameAndBaseValue(string eventName)
        {
            var baseVal = BaseValuesStep + _curvesBaseValuesGiver.GetOrCreate(eventName);
            _curvesBaseValuesGiver[eventName] = baseVal;
            return ($"{eventName}_{baseVal}", baseVal);
        }

        CurveData EjectCurveDataFromByCauserDic(object eventCauser)
        {
            CurveData curveData;
            if (!_curvesByCausers.TryGetValue(eventCauser, out curveData))
            {
                NLogger.IfError()?.Message($"No item at {nameof(_curvesByCausers)} by causer {eventCauser}").Write();
                return null;
            }

            _curvesByCausers.Remove(eventCauser);
            return curveData;
        }

        // #No_need: ------------------------------------------------------------------------------------------
        // // Заготовка на случай, если всё же бу нужны по логгеру на кажд.entty
        // private const string LoggerNamePrefix = "DbgCollectorTrgt_";
        // private readonly Dictionary<Guid, CurveLogger> NO_NEED_loggers = new Dictionary<Guid, CurveLogger>();
        // 
        // // Пока нам нужен 1 единственный логгер.
        // // Заготовка на случай, если всё же бу нужны по логгеру на кажд.entty
        // private CurveLogger NO_NEED_GetLoggerIfActive(Guid entityId)
        // {
        //     if (entityId == Guid.Empty)
        //     {
        //         NLogger.IfError()?.Message("passed empty guid").Write();
        //         return null;
        //     }
        //     CurveLogger logger;
        //     if (!_loggers.TryGetValue(entityId, out logger))
        //     {
        //         logger = CurveLogger.Get(LoggerNamePrefix + entityId.ToString());
        //         _loggers.Add(entityId, logger);
        //     }
        // 
        //     return logger?.IfActive;
        // }

    // --- Util types: ------------------------------------------------------------------------------------------

        class CurveData
        {
            internal readonly string CurveName;
            internal float CurrValue;
            private readonly float _baseValue;
            private object _eventCauser;

            internal CurveData(string curveName, float baseValue, object eventCauser = null)
            {
                CurveName = curveName;
                CurrValue = baseValue;
                _baseValue = baseValue;
                _eventCauser = eventCauser;
            }

            internal bool IsFree => CurrValue == _baseValue; //? may be opt by bool falg

            internal float Acquire([NotNull] object eventCauser)
            {
                ///#Dbg:
                Debug.Assert(_eventCauser == null);
                Debug.Assert(CurrValue == _baseValue);

                _eventCauser = eventCauser;
                //CurveLoggerTarget.Instance._curvesByCausers.Add(eventCauser, this);
                return CurrValue += ValuesStep;
            }

            internal float Release()
            {
                _eventCauser = null;
                return CurrValue = _baseValue;
            }
        }

        struct EventId
        {
            private readonly Guid _entityId;
            private readonly string _eventName;

            internal EventId(Guid entityId, string eventName)
            {
                _entityId = entityId;
                _eventName = eventName;
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is EventId eId)
                    return Equals(eId);
                return false;
            }

            public bool Equals(EventId other)
            {
                return _entityId.Equals(other._entityId) && string.Equals(_eventName, other._eventName);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_entityId.GetHashCode() * 397) ^ (_eventName != null ? _eventName.GetHashCode() : 0);
                }
            }
        }
    }
}
