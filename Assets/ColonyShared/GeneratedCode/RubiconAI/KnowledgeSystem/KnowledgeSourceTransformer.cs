using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using UnityEngine;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Entities.GameObjectEntities;
using Assets.Src.SpawnSystem;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.Src.RubiconAI.KnowledgeSystem
{
    class KnowledgeSourceTransformer : IKnowledgeSource
    {
        public KnowledgeCategoryDef Category { get; private set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<OuterRef<IEntity>, VisibilityDataSample> Legionaries { get; } = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();

        public KnowledgeSourceDef Def { get; set; }
        long _lastTimeUpdated;
        static long _updateInNoLessThan = SyncTime.FromSeconds(0.4f);
        public event Func<OuterRef<IEntity>, ValueTask> OnLearnedAboutLegionary;
        public event Func<OuterRef<IEntity>, ValueTask> OnForgotAboutLegionary;

        Condition _filter;
        IKnowledgeSource _source;
        Knowledge _knowledge;
        KnowledgeSourceTransformerDef _def;
        public async ValueTask LoadDef(Knowledge knowledge, KnowledgeSourceDef def)
        {
            _knowledge = knowledge;

            var kDef = ((KnowledgeSourceTransformerDef)def);
            _def = kDef;
            Category = kDef.Category;
            _source = knowledge.GetKnownStuff(kDef.GetFrom);
            if (_source == null)
                Logger.IfError()?.Message($"{def.____GetDebugShortName()} has no source {kDef.GetFrom.Target.____GetDebugShortName()}").Write();
            _filter = (Condition)await knowledge.Meta.Get(null, kDef.Filter.Target);
            _source.OnForgotAboutLegionary += _source_OnForgotAboutLegionary;
            _source.OnLearnedAboutLegionary += _source_OnLearnedAboutLegionary;
        }
        StatModifierDef _tempDef = new StatModifierDef();
        private async ValueTask _source_OnLearnedAboutLegionary(OuterRef<IEntity> ent)
        {
            Legionary.LegionariesByRef.TryGetValue(ent, out var leg);
            var obj = leg;
            if (obj == null || !obj.IsValid)
                return;
            if (await _filter.EvaluateOther(obj, obj))
            {
                if (_def.InterpretAsStat.Target != null)
                    _knowledge.Memory.SetStatMod(obj, 1000000000, _def.InterpretAsStat, 1f, new StatModKey() { Assigner = _knowledge.Legionary, Def = _tempDef });
                Legionaries.Add(ent, _source.Legionaries[ent]);
                await OnLearnedAboutLegionary.InvokeAndWaitAll(ent);
            }
        }

        private async ValueTask _source_OnForgotAboutLegionary(OuterRef<IEntity> ent)
        {

            Legionaries.Remove(ent);
            await OnForgotAboutLegionary.InvokeAndWaitAll(ent);
            if (_def.InterpretAsStat.Target != null)
            {
                Legionary.LegionariesByRef.TryGetValue(ent, out var leg);
                var obj = leg;
                if (obj == null || !obj.IsValid)
                    return;
                if (obj.Valid() && await _filter.EvaluateOther(obj, obj))
                    _knowledge.Memory.SetStatMod(obj, SyncTime.FromSeconds(_def.TimeToRemember), _def.InterpretAsStat, 1f, new StatModKey() { Assigner = _knowledge.Legionary, Def = _tempDef });
                else
                    _knowledge.Memory.RemoveStatMod(obj, _def.InterpretAsStat, new StatModKey() { Assigner = _knowledge.Legionary, Def = _tempDef });

            }
        }

        public async ValueTask UpdateKnowledge()
        {
            if (SyncTime.NowUnsynced - _lastTimeUpdated < _updateInNoLessThan)
                return;
            _lastTimeUpdated = SyncTime.NowUnsynced;
            await _source.UpdateKnowledge();
            foreach (var ent in _source.Legionaries.Keys)
            {
                Legionary.LegionariesByRef.TryGetValue(ent, out var leg);
                var knownLegionary = leg;
                if (knownLegionary == null || !knownLegionary.IsValid)
                    continue;
                if (await _filter.EvaluateOther(knownLegionary, knownLegionary))
                {
                    bool had = Legionaries.ContainsKey(ent);
                    if (!had)
                    {
                        Legionaries.Add(ent, _source.Legionaries[ent]);
                        await OnLearnedAboutLegionary.InvokeAndWaitAll(ent);
                    }
                    if (_def.InterpretAsStat.Target != null)
                        _knowledge.Memory.SetStatMod(knownLegionary, 1000000000, _def.InterpretAsStat, 1f, new StatModKey() { Assigner = _knowledge.Legionary, Def = _tempDef });
                }
                else
                {
                    if (_def.InterpretAsStat.Target != null)
                        _knowledge.Memory.RemoveStatMod(knownLegionary, _def.InterpretAsStat, new StatModKey() { Assigner = _knowledge.Legionary, Def = _tempDef });
                    if (Legionaries.Remove(ent))
                        await OnForgotAboutLegionary.InvokeAndWaitAll(ent);
                }
            }
        }

        public BaseResource GetId()
        {
            if (_def.InterpretAsStat.Target == null)
                return Category;
            return _def.InterpretAsStat;
        }
    }
}
