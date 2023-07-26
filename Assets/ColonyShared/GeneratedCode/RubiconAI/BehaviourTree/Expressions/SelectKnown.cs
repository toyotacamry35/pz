using System.Collections.Generic;
using JetBrains.Annotations;
using Assets.Src.SpawnSystem;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.MovementSync;
using SharedCode.Utils;
using NLog;
using System.Threading.Tasks;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using System.Collections.Concurrent;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{

    interface ICollectionSelector
    {
        ValueTask GetLegionaries([NotNull]Legionary legionary, List<Legionary> list);
    }

    class SelectKnown : TargetSelector, ICollectionSelector
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Strategy HostStrategy { get; set; }
        private Condition _filter;
        private Metric _metric;
        private SelectKnownDef _def;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (SelectKnownDef)def;
            if (_def.Filter.Target != null)
            {
                var t = hostStrategy.MetaExpression.Get(hostStrategy, _def.Filter.Target);
                if (t.IsCompleted)
                    _filter = (Condition)t.Result;
                else
                    _filter = (Condition)await t;
            }
            if (_def.Metric.Target != null)
            {
                var t = hostStrategy.MetaExpression.Get(hostStrategy, _def.Metric.Target);
                if (t.IsCompleted)
                    _metric = (Metric)t.Result;
                else
                    _metric = (Metric)await t;
            }
        }

        struct LegionaryWithValue
        {
            public float Value;
            public Legionary Legionary;
        }
        System.Random _random = new System.Random(42);
        List<Legionary> _legionariesCachedList = new List<Legionary>();
        Legionary _targetSearchLegionary = null;
        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            if (!(legionary is MobLegionary ml))
                return null;
            Legionary selectedLegionary = null;
            _legionariesCachedList.Clear();
            Legionary lastSelectedLegionary = null;
            AIProfiler.BeginSample("FilterSelectKnown");
            lastSelectedLegionary = await ForeachLegionary(ml, null);
            AIProfiler.EndSample();
            if (_legionariesCachedList.Count == 0 && lastSelectedLegionary == null)
                return null;
            if (_metric != null)
            {
                if (lastSelectedLegionary == null)
                    return null;
                selectedLegionary = lastSelectedLegionary;
            }
            else if (_legionariesCachedList.Count > 0)
            {
                var leg = _legionariesCachedList[_random.Next(0, _legionariesCachedList.Count)];
                selectedLegionary = leg;
            }
            else
            {
                selectedLegionary = lastSelectedLegionary;
            }
            //if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Selected:{0} At:{1}", selectedLegionary, selectedLegionary?.Transform?.position).Write();
            return selectedLegionary;
        }

        async ValueTask<(float, Legionary, Legionary)> FilterLegionary(Legionary legionary, Legionary knownLeg, float lastSelectedMetricValue, Legionary lastSelectedLegionary)
        {
            AIProfiler.BeginSample("FilterLegionary");
            if (_targetSearchLegionary != null && knownLeg != _targetSearchLegionary)
                return (lastSelectedMetricValue, null, lastSelectedLegionary);
            if (!knownLeg.Valid())
                return (lastSelectedMetricValue, null, lastSelectedLegionary);
            if (_filter != null)
            {
                AIProfiler.BeginSample("EvaluateOther");
                var e = await _filter.EvaluateOther(legionary, knownLeg);
                AIProfiler.EndSample();
                if (!e)
                {
                    AIProfiler.EndSample();
                    return (lastSelectedMetricValue, null, lastSelectedLegionary);
                }
                else if (_targetSearchLegionary == knownLeg)
                {
                    var ret = _targetSearchLegionary; // if this is the one we are checking for - return immediately when we decided that he's available

                    lastSelectedLegionary = _targetSearchLegionary;
                    _targetSearchLegionary = null;
                    AIProfiler.EndSample();
                    return (lastSelectedMetricValue, ret, lastSelectedLegionary);

                }
            }
            else if (_targetSearchLegionary == knownLeg)
            {
                var ret = _targetSearchLegionary; // if this is the one we are checking for - return immediately when we decided that he's available
                lastSelectedLegionary = _targetSearchLegionary;
                _targetSearchLegionary = null;
                AIProfiler.EndSample();
                return (lastSelectedMetricValue, ret, lastSelectedLegionary);

            }
            if (_metric != null || _def.Shuffle)
            {
                AIProfiler.BeginSample("ApplyMetric");
                var m = _def.Shuffle ? _random.Next() : await _metric.EvaluateOther(legionary, knownLeg);
                if (_def.InverseMetric)
                {
                    if (m <= lastSelectedMetricValue && knownLeg != null)
                    {
                        lastSelectedMetricValue = m;
                        lastSelectedLegionary = knownLeg;
                    }
                }
                else
                {
                    if (m >= lastSelectedMetricValue && knownLeg != null)
                    {
                        lastSelectedMetricValue = m;
                        lastSelectedLegionary = knownLeg;
                    }
                }
                AIProfiler.EndSample();
            }
            else
                _legionariesCachedList.Add(knownLeg);
            AIProfiler.EndSample();
            return (lastSelectedMetricValue, null, lastSelectedLegionary);
        }

        ValueTask<Legionary> TargetSelector.SelectTarget(Legionary legionary)
        {
            return SelectTarget(legionary);
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            var t = HostStrategy.CurrentLegionary.GetOtherDataFromMemory(await SelectTarget(legionary));
            return t.Def != null ? t.Pos : (Vector3?)null;
        }
        static List<Legionary> _emptyList = new List<Legionary>();
        ConcurrentStack<List<Legionary>> freeMemories = new ConcurrentStack<List<Legionary>>();
        public async ValueTask<Legionary> ForeachLegionary(MobLegionary legionary, List<Legionary> collection)
        {
            Legionary lastSelectedLegionary = default;
            AIProfiler.BeginSample("ForeachLegionaryInSelectKnown");
            float lastSelectedMetricValue = !_def.InverseMetric ? float.MinValue : float.MaxValue;
            if (_def.MemoryCategory.Target != null)
            {
                if (_targetSearchLegionary != null)
                {
                    AIProfiler.BeginSample("CheckTargetInSelectKnown");
                    if (legionary.Knowledge.Memory.GetStat(_targetSearchLegionary, _def.MemoryCategory.Target) != 0)
                    {
                        Legionary leg = default;
                        (lastSelectedMetricValue, leg, lastSelectedLegionary) = await FilterLegionary(legionary, _targetSearchLegionary, lastSelectedMetricValue, lastSelectedLegionary);
                        if (leg == null)
                            lastSelectedLegionary = null;
                        //yield return _targetSearchLegionary;
                    }
                    else
                    if (legionary.AssignedLegion != null)
                        if (legionary.AssignedLegion.Knowledge.Memory.GetStat(_targetSearchLegionary, _def.MemoryCategory.Target) != 0)
                        {
                            Legionary leg = default;
                            (lastSelectedMetricValue, leg, lastSelectedLegionary) = await FilterLegionary(legionary, _targetSearchLegionary, lastSelectedMetricValue, lastSelectedLegionary);
                            if (leg == null)
                                lastSelectedLegionary = null;
                        }
                    AIProfiler.EndSample();
                    AIProfiler.EndSample();
                    return lastSelectedLegionary;
                    //yield break;
                }
                AIProfiler.BeginSample("ForeachMemoryPiece");
                freeMemories.TryPop(out var list);
                if (list == null)
                    list = new List<Legionary>();
                legionary.Knowledge.Memory.FilterMemoryPieces(_def.MemoryCategory.Target, list);
                foreach (var memory in list)
                {
                    Legionary leg = default;
                    (lastSelectedMetricValue, leg, lastSelectedLegionary) = await FilterLegionary(legionary, memory, lastSelectedMetricValue, lastSelectedLegionary);
                    if (collection != null)
                        collection.Add(memory);
                    //yield return memory.Key.About;

                }
                if (legionary.AssignedLegion != null)
                {
                    list.Clear();
                    legionary.AssignedLegion.Knowledge.Memory.FilterMemoryPieces(_def.MemoryCategory.Target, list);
                    foreach (var memory in list)
                    {
                        Legionary leg = default;
                        (lastSelectedMetricValue, leg, lastSelectedLegionary) = await FilterLegionary(legionary, memory, lastSelectedMetricValue, lastSelectedLegionary);
                        if (collection != null)
                            collection.Add(memory);
                        //                            yield return memory.Key.About;

                    }
                }
                list.Clear();
                freeMemories.Push(list);
                AIProfiler.EndSample();
            }
            else
            if (_def.Category.Target != null)
            {
                var knownStuff = legionary.Knowledge.GetKnownStuff(_def.Category.Target);
                if (knownStuff != null)
                {
                    if (_targetSearchLegionary != null)
                    {
                        AIProfiler.BeginSample("CheckTargetInMemory");
                        if (knownStuff.Legionaries.ContainsKey(_targetSearchLegionary.Ref))
                        {
                            Legionary leg = default;
                            (lastSelectedMetricValue, leg, lastSelectedLegionary) = await FilterLegionary(legionary, _targetSearchLegionary, lastSelectedMetricValue, lastSelectedLegionary);
                            if (leg == null)
                                lastSelectedLegionary = null;
                            //yield return _targetSearchLegionary;
                        }
                        //yield break;
                        AIProfiler.EndSample();
                        AIProfiler.EndSample();
                        return lastSelectedLegionary;
                    }
                    AIProfiler.BeginSample("ForeachKnown");
                    foreach (var ent in knownStuff.Legionaries)
                    {
                        Legionary.LegionariesByRef.TryGetValue(ent.Key, out var leg);
                        var obj = leg;
                        if (obj == null || !obj.IsValid)
                            continue;
                        Legionary sel = default;
                        (lastSelectedMetricValue, sel, lastSelectedLegionary) = await FilterLegionary(legionary, leg, lastSelectedMetricValue, lastSelectedLegionary);
                        if (collection != null)
                            collection.Add(leg);
                    }
                    AIProfiler.EndSample();
                }
            }

            AIProfiler.EndSample();
            return lastSelectedLegionary;
        }

        public async ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        {
            _targetSearchLegionary = targetLegionary;
            if (_targetSearchLegionary == null || !_targetSearchLegionary.IsValid)
            {
                _targetSearchLegionary = null;
                return false;
            }
            var tarLeg = await SelectTarget(HostStrategy.CurrentLegionary);
            var could = tarLeg != null && targetLegionary == tarLeg && tarLeg.IsValid;
            _targetSearchLegionary = null;
            return could;
        }

        public async ValueTask GetLegionaries([NotNull] Legionary legionary, List<Legionary> list)
        {
            if (!(legionary is MobLegionary ml))
                return;
            Legionary lastSelectedLegionary = await ForeachLegionary(ml, list);

        }
    }
}
