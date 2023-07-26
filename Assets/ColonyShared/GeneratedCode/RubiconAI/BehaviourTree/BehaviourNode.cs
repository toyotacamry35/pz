using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Detective;
using UnityEngine;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;

namespace Assets.Src.RubiconAI.BehaviourTree
{

    public class BehaviourStatus
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public StrategyStatus ParentStrategy;
        public string Strategy;
        public string StatusDescription;
        public ScriptResultType Result;
        public bool Ticked = false;
        public bool Started = false;
        public bool Terminated = false;
        public bool Finished = false;
        public List<BehaviourStatus> SubNodesTicks = new List<BehaviourStatus>();
        public Dictionary<string, ExpressionStatus> Expressions = new Dictionary<string, ExpressionStatus>();
        private StrategyStatus _stratStatus;
        public StrategyStatus SubStrategyStatus { get { return _stratStatus; } set { if (_stratStatus != null) Logger.IfError()?.Message("Double assign sub strat").Write(); _stratStatus = value; } }
        public BehaviourNode Node;
    }

    public class ExpressionStatus
    {

    }
    public abstract class BehaviourNode
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public bool Terminated;
        public bool TickedThisFrame;
        public BehaviourNode Parent;
        public BehaviourNodeDef _def;
        // For debug purposes only:
        public BehaviourStatus Status;
        public Ticker Ticker { get; protected set; }
        private ScriptResultType _internalStatus = ScriptResultType.None;
        public Strategy HostStrategy { get; private set; }
        protected virtual ValueTask OnCreate(BehaviourNodeDef def) { return default; }
        public virtual IEnumerable<BehaviourNode> GetSubNodes() { return Array.Empty<BehaviourNode>(); }
        public async ValueTask OnCreate(Strategy hostStrategy, BehaviourNodeDef def)
        {
            _def = def;
            _internalStatus = ScriptResultType.None;
            LastStatus = _internalStatus;
            HostStrategy = hostStrategy;

            if (AIProfiler.EnableProfile)
                AIProfiler.BeginSample(this.GetType().Name, "OnCreate");
            await OnCreate(def);
            if (AIProfiler.EnableProfile)
                AIProfiler.EndSample();
        }

        public async ValueTask Run(bool assertStart = false)
        {
            if (!HostStrategy.CurrentLegionary.TickedNodes.Add(this))
                Logger.IfError()?.Message("fuck my life").Write();
            TickedThisFrame = true;
            if (AIProfiler.EnableProfile)
                AIProfiler.BeginSample(this.GetType().Name);
            bool started = false;
            if (_internalStatus == ScriptResultType.None)
            {
                if (Status != null)
                    Status.Started = true;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Node start | Def:{_def} Node:{this.GetHashCode()}").Write();
                var t = OnStart();
                if (t.IsCompleted)
                    _internalStatus = t.Result;
                else
                    _internalStatus = await t;
                started = true;
            }
            if (!started && assertStart)
            {
               Logger.IfError()?.Message("No start where one was assumed").Write();
            }
            if (_internalStatus == ScriptResultType.Running)
            {
                var t = OnTick();
                if (t.IsCompleted)
                    _internalStatus = t.Result;
                else
                    _internalStatus = await t;
            }
            LastStatus = _internalStatus;
            if (_internalStatus == ScriptResultType.Failed)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Node failed | Def:{_def} Status:{StatusDescription} Node:{this.GetHashCode()}").Write();
                
                var tfail = OnFail();
                if (!tfail.IsCompleted)
                    await tfail;

                var tfinish = OnFinish();
                if (!tfinish.IsCompleted)
                    await tfinish;
                _internalStatus = ScriptResultType.None;
            }
            else if (_internalStatus == ScriptResultType.Succeeded)
            {
                
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Node succeeded | Def:{_def} Node:{this.GetHashCode()}").Write();

                var tsuccess = OnSuccess();
                if (!tsuccess.IsCompleted)
                    await tsuccess;
                var tfinish = OnFinish();
                if (!tfinish.IsCompleted)
                    await tfinish;
                _internalStatus = ScriptResultType.None;
            }
            if (Status != null)
            {
                Status.Result = LastStatus;
                Status.StatusDescription = StatusDescription;
                Status.Ticked = true;
                Status.Node = this;

                foreach (var subNode in GetSubNodes())
                {
                    Status.SubNodesTicks.Add(subNode.Status);
                }
            }
            if (AIProfiler.EnableProfile)
                AIProfiler.EndSample();
        }

        public async ValueTask Terminate()
        {
            if (Status != null)
            {
                Status.Terminated = true;
            }
            if (_internalStatus == ScriptResultType.Running)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Node terminate | Def:{_def} Node:{this.GetHashCode()}").Write();

                var tfinish = OnFinish();
                if (!tfinish.IsCompleted)
                    await tfinish;
                var tterminate = OnTerminate();
                if (!tterminate.IsCompleted)
                    await tterminate;
                if (Status != null)
                {
                    Status.Finished = true;
                }
            }
            _internalStatus = ScriptResultType.None;
            LastStatus = _internalStatus;
        }
        protected virtual ValueTask<ScriptResultType> OnTick()
        {
            return new ValueTask<ScriptResultType>(ScriptResultType.Running);
        }
        public virtual ValueTask<ScriptResultType> OnStart()
        {
            return new ValueTask<ScriptResultType>(ScriptResultType.Running);
        }
        public virtual ValueTask OnSuccess() { return new ValueTask(); }
        public virtual ValueTask OnFail() { return new ValueTask(); }
        public virtual ValueTask OnFinish() { return new ValueTask(); }
        public virtual ValueTask OnTerminate() { return new ValueTask(); }

        public string StatusDescription { get; protected set; }
        public ScriptResultType LastStatus { get; set; }
    }
    public abstract class Ticker
    {
        public abstract bool ShouldTick();
        public abstract void Tick();
    }

    public class BehaviourNodeTickEvent : Detective.Event
    {
        public string StatusDescription;
        public ScriptResultType TickStatus;
        public BehaviourNode Node;
        public override string ToString()
        {
            return $"{ Node.GetType().Name} - {TickStatus} - {StatusDescription}";
        }
    }
}
