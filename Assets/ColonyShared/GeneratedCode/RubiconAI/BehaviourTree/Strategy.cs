using System;
using System.Collections.Generic;
using NLog;
using SharedCode.Utils;
using System.Threading.Tasks;
using ColonyShared.SharedCode.InputActions;
using Core.Environment.Logging.Extension;

//using Src.InputActions;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    public class Strategy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public event Action<float> DemandsUpdate;
        
        public StrategyDef Def { get; private set; }
        public AIEvent HostEvent { get; private set; }
        public StrategyStatus Description { get; set; } // Stores data about itself & is consumed inside debugging block "Office.Visit().CurrentInvestigation?.BeginInvestigating". So assigned to null at end of every such block.
        public MetaExpression MetaExpression { get; set; }
        public ScriptResultType LastState { get; private set; } // Unites in itself Global & local state (sometimes is used f.e. to pass state from called method to caller one (f.e. see `.Run()`))
        public MobLegionary CurrentLegionary { get; private set; }

        private BehaviourNode _rootNode;
        private StrategyStatus _lastTickStatus;
        private readonly Strategy _hostStrategy; 
        private readonly Dictionary<NodeToChild, BehaviourNode> _nodeInstances = new Dictionary<NodeToChild, BehaviourNode>();

        public Strategy(StrategyDef def, Strategy hostStrategy = null)
        {
            MetaExpression = new MetaExpression(this);
            Def = def;
            _hostStrategy = hostStrategy;
        }

        public Strategy(StrategyDef def, AIEvent hostEvent)
        {
            MetaExpression = new MetaExpression(this);
            Def = def;
            HostEvent = hostEvent;
        }
        public void ShouldTickImmediately()
        {
            if (_hostStrategy != null)
                _hostStrategy.ShouldTickImmediately(); // sub-strategy can't tick by itself - only by parent-strategy calls its tick from inside self tick
            else
                DemandsUpdate?.Invoke(0);
        }
        public void ShouldTickWithDelay(float delay)
        {
            if (_hostStrategy != null)
                _hostStrategy.ShouldTickWithDelay(delay);
            else
                DemandsUpdate?.Invoke(delay);
        }

        public void AssignToLegionary(MobLegionary runningLegionary)
        {
            CurrentLegionary = runningLegionary;
        }

        public async ValueTask<ScriptResultType> Init(MobLegionary runningLegionary)
        {
            await MetaExpression.Clear(this);
            CurrentLegionary = runningLegionary;

            if (Def != null) // "==null" case see in call this method from `Knowledge`
            {
                // Trigger lazy init-tion of all selectors:
                foreach (var defSelector in Def.Selectors)
                    await MetaExpression.Get(this, defSelector.Value.Target);

                _rootNode = await GetNode(null, Def.Plan);
                if (Description != null)
                {
                    _rootNode.Status = new BehaviourStatus();
                    _rootNode.Status.Node = _rootNode;
                }
            }

            return LastState = ScriptResultType.Running;
        }

        public async ValueTask<ScriptResultType> Run(bool assertStart = false)
        {
            if (LastState == ScriptResultType.Failed || LastState == ScriptResultType.Succeeded || LastState == ScriptResultType.None)
            {
                await Init(CurrentLegionary);                             ///#PZ-6289: (?): `.Init` sets `LastState` inside. But here we override its val... Какое-то неинтуитивное использование...
                if (LastState != ScriptResultType.Running)
                    return LastState = ScriptResultType.Failed;
                LastState = ScriptResultType.None;
            }
            LastState = await Tick(assertStart);
            if (LastState == ScriptResultType.Failed || LastState == ScriptResultType.Succeeded)
                ShouldTickImmediately();

            if ((LastState == ScriptResultType.Succeeded || LastState == ScriptResultType.Failed) && _rootNode.LastStatus == ScriptResultType.Running)
                Logger.IfError()?.Message("AI had a weird tick").Write();

            if (Description != null)
            {
                if (Description.RootNodeTick.Result != LastState)
                    Logger.IfError()?.Message("Debugger and actual status mismatch").Write();
                if (Description.Result != ScriptResultType.None)
                    Logger.IfError()?.Message("Reused result").Write();
                Description.Result = LastState;
            }
            return LastState;
        }

        public async ValueTask Terminate()
        {
            LastState = ScriptResultType.Failed;
            if (_rootNode == null)
                return;
            await Terminate(_rootNode);
        }

        public async ValueTask<BehaviourNode> GetNode(BehaviourNode parent, BehaviourNodeDef childDef)
        {
            BehaviourNode node = null;
            if (!_nodeInstances.TryGetValue(new NodeToChild() { ChildDef = childDef, ParentNode = parent }, out node))
            {
                var newNode = (BehaviourNode)Activator.CreateInstance(DefToType.GetType(childDef.GetType()));
                await newNode.OnCreate(this, childDef);
                _nodeInstances.Add(new NodeToChild() { ChildDef = childDef, ParentNode = parent }, newNode);
                newNode.Parent = parent;
                node = newNode;
                if (Description != null)
                {
                    node.Status = new BehaviourStatus();
                    node.Status.Node = node;
                }
            }
            return node;
        }

        async ValueTask<ScriptResultType> Tick(bool assertStart = false)
        {
            if (!CurrentLegionary.TickedNodes.Add(this))
                Logger.IfError()?.Message("Fuck my life").Write();
            if (Description != null)
            {
                foreach (var node in _nodeInstances.Values)
                {
                    node.Status = new BehaviourStatus();
                    node.Status.Node = node;
                }
                Description.RootNodeTick = _rootNode.Status;
                _rootNode.Status.Strategy = Def.____GetDebugShortName();
            }
            await _rootNode.Run(assertStart);
            var status = _rootNode.LastStatus;


            foreach (var node in _nodeInstances.Values)
                node.Status = null;

            if (_lastTickStatus != null && Description != null && _lastTickStatus == Description)
                Logger.IfError()?.Message("Description was not updated before new tick").Write();
            _lastTickStatus = Description;
            return status;
        }

        public async ValueTask Terminate(BehaviourNode node)
        {
            if (node.LastStatus == ScriptResultType.Failed)
                return;

            await node.Terminate();
        }
    }
}
