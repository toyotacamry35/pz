using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Core.Environment.Logging.Extension;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class UseStrategy : BehaviourNode
    {
        private Condition _goalCondition;
        private Strategy _strategy;
        private UseStrategyDef _def;
        List<IInvalidatableSelector> _invalidatableState;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (UseStrategyDef)def;
            AIProfiler.BeginSample("Use strategy get goal");
            _goalCondition = (Condition)await HostStrategy.MetaExpression.GetOptional(HostStrategy, _def.GoalCondition.Target);
            AIProfiler.EndSample();
            AIProfiler.BeginSample("Use strategy create strategy");
            _strategy = new Strategy(_def.Strategy, HostStrategy);
            AIProfiler.EndSample();
            _strategy.MetaExpression = HostStrategy.MetaExpression;
            AIProfiler.BeginSample("Use strategy get invalidatable state");
            foreach (var cValue in _def.ValidState)
            {
                if (cValue.Value is BehaviourExpressionDef)
                {
                    var expr = await _strategy.MetaExpression.Get(_strategy, (BehaviourExpressionDef)cValue.Value);
                    if (expr is IInvalidatableSelector)
                    {
                        if (_invalidatableState == null)
                            _invalidatableState = new List<IInvalidatableSelector>();
                        _invalidatableState.Add((IInvalidatableSelector)expr);
                    }

                }
            }
            AIProfiler.EndSample();
        }
        bool _justStarted = false;
        public override async ValueTask<ScriptResultType> OnStart()
        {
            _justStarted = true;
            _strategy.AssignToLegionary(HostStrategy.CurrentLegionary);
            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            if (_goalCondition != null)
            {
                AIProfiler.BeginSample("EvaluateGoal");
                if (await _goalCondition.Evaluate(HostStrategy.CurrentLegionary))
                {
                    AIProfiler.EndSample();
                    return ScriptResultType.Succeeded;
                }
                AIProfiler.EndSample();
            }
            AIProfiler.BeginSample("CheckForInvalidState");
            if (_invalidatableState != null)
                foreach (var state in _invalidatableState)
                    if (await state.Invalid())
                    {
                        AIProfiler.EndSample();
                        return ScriptResultType.Failed;
                    }
            if (Status != null)
            {
                if (_strategy.Description != null)
                    Logger.IfError()?.Message("Description already assigned").Write();
                _strategy.Description = new StrategyStatus();
                Status.SubStrategyStatus = _strategy.Description;
            }
            AIProfiler.EndSample();
            AIProfiler.BeginSample("RunStrategy");
            var result = await _strategy.Run(_justStarted);
            _strategy.Description = null;
            _justStarted = false;
            AIProfiler.EndSample();
            HostStrategy.CurrentLegionary.AllSubStrategies.TryAdd(_strategy, true);
            if (result == ScriptResultType.Failed || result == ScriptResultType.Running)
                return result;
            if (result == ScriptResultType.Succeeded && _goalCondition == null)
                return result;
            if (result == ScriptResultType.Succeeded && _goalCondition != null)
            {
                return ScriptResultType.Running;
            }
            return result;
        }
        public override async ValueTask OnTerminate()
        {
            if (_strategy != null)
                HostStrategy.CurrentLegionary.AllSubStrategies.TryRemove(_strategy, out var _);
            await _strategy.Terminate();
        }
        public override async ValueTask OnFinish()
        {
            if (_strategy != null)
                HostStrategy.CurrentLegionary.AllSubStrategies.TryRemove(_strategy, out var _);
            await _strategy.Terminate();
        }
    }

    public interface IInvalidatableSelector
    {
        ValueTask<bool> Invalid();
    }
}
