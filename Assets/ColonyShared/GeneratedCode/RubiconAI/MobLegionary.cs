using Assets.Src.Detective;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree;
using Assets.Src.RubiconAI.KnowledgeSystem;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.AI;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.Aspects.Combat;
using System.Threading;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects.Misc;

namespace Assets.Src.RubiconAI
{
    public class MobLegionary : Legionary, IInputActionHandlersFactory
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // Used for debugging
        public HashSet<object> TickedNodes = new HashSet<object>();
        public VisibilityDataSample FromUnitySelfSample;
        public SharedCode.Utils.Vector3 Forward { get; set; }
        public SharedCode.Utils.Vector3? GetPos(Legionary leg)
        {
            var data = GetOtherDataFromMemory(leg);
            if (data.Ref.Guid == Guid.Empty)
                return null;
            return data.Pos;
        }
        public VisibilityDataSample GetOtherDataFromMemory(Legionary leg)
        {
            if (leg == null)
                return default(VisibilityDataSample);
            if (leg == this)
            {
                //Logger.IfError()?.Message($"GET SELF {FromUnitySelfSample.Pos}").Write();
                return FromUnitySelfSample;
            }
            else if (leg is MobLegionary ml && ml.FromUnitySelfSample.Def != null)
                return ((MobLegionary)leg).FromUnitySelfSample;
            //FOR NOW - it doesn't work as intended, probably some problems with grids on replicated side
            if (WorldSpaceGuid != Guid.Empty)
            {
                return VisibilityGrid.Get(WorldSpaceGuid, leg.Repository).GetGridData(leg.Ref);
            }
            return default(VisibilityDataSample);


        }
        public InputActionStatesGenerator InputActions { get; set; }
        InputActionsProcessor Processor { get; set; }
        public AttackDoerServerAnimationTrajectory AttackDoer { get; private set; }
        public Task InitInputActions(IHasInputActionHandlers ent, VisualDollDef bodyDef)
        {
            var hiah = ent;
            InputActions = new InputActionStatesGenerator();
            Processor = new InputActionsProcessor(hiah.InputActionHandlers.BindingsSource, this, InputActions, Ref.Guid);
            AttackDoer = new AttackDoerServerAnimationTrajectory(Ref, Repository, bodyDef);
            return Task.CompletedTask;
        }
        public Dictionary<string, long> TemporaryBlackboard { get; set; } = new Dictionary<string, long>(15);
        public Knowledge Knowledge { get; set; }
        public ConcurrentDictionary<Strategy, bool> AllSubStrategies = new ConcurrentDictionary<Strategy, bool>();
        internal int ExecutingHighFreqUpdatedBehNodes = 0;
        public bool IsExecutingHighFreqUpdatedBehNodes => ExecutingHighFreqUpdatedBehNodes > 0;
        public Strategy ExecutingStrategy
        {
            get
            {
                return _currentStrategy;
            }
            set
            {
                if (_currentStrategy != null)
                    _currentStrategy.DemandsUpdate -= DemandsUpdate;
                _currentStrategy = value;
                if (_currentStrategy != null)
                    _currentStrategy.DemandsUpdate += DemandsUpdate;
            }
        }

        public ISpellDoer SpellDoer { get; internal set; }

        public event Action<float> DemandsUpdate;

        private readonly Brain _brain;
        public readonly LegionaryDef Def;
        public LegionaryEntityDef SDef;

        private Strategy _currentStrategy;
        private ConcurrentDictionary<Strategy, bool> _eventStrategies = new ConcurrentDictionary<Strategy, bool>();

        // Ctor:
        public MobLegionary([NotNull]Brain brain, LegionaryDef def, IEntitiesRepository repo) : base(repo)
        {
            _brain = brain;
            Def = def;
            Knowledge = new Knowledge(this);
        }
        public ValueTask Init() => Knowledge.Init();
        // --- Overrides: ----------------------------------------------------


        // --- API: ----------------------------------------------------

        //public static Legionary Get(GameObject go) => go.GetComponent<ISpatialLegionary>()?.Legionary;

        //bool _initializing = false;

        //static int _interlockedRaceConditionTracker = 0;
        
        private static Pool<List<IKnowledgeSource>> _sourcesPool = 
            new Pool <List<IKnowledgeSource>>(5, 2, ()=>new List<IKnowledgeSource>(), x=>x.Clear());
        public void InitAllKnowledge()
        {
            //lock(this)
            //{
            //    if (_initializing)
            //        throw new Exception("RACE CONDITION");
            //    _initializing = true;
            //}
            //if (Interlocked.Increment(ref _interlockedRaceConditionTracker) != 1)
            //   throw new Exception("RACE CONDITION IN AI, PIZDETS");
            var sources = _sourcesPool.Take();
            try
            {

                sources.Clear();
                if (Def != null)
                {
                    foreach (var kDef in SDef.CommonSenses)
                    {
                        var source = (IKnowledgeSource)Activator.CreateInstance(DefToType.GetType(kDef.Target.GetType()));
                        source.Def = kDef;

                        sources.Add(source);

                        Knowledge.Register(kDef.Target.Category, source);
                    }
                    foreach (var kDef in Def.KnowledgeSources)
                    {
                        var source = (IKnowledgeSource)Activator.CreateInstance(DefToType.GetType(kDef.Target.GetType()));
                        source.Def = kDef;

                        sources.Add(source);

                        Knowledge.Register(kDef.Target.Category, source);
                    }
                }

                foreach (var source in sources)
                {
                    if (source is SpatialKnowledgeSource sks)
                        sks.Legionary = this;
                    source.LoadDef(Knowledge, source.Def);
                }
            }
            finally
            {
                _sourcesPool.Return(sources);
            }
            //Interlocked.Decrement(ref _interlockedRaceConditionTracker);
        }
        public async Task TerminateEverything()
        {
            if (AttackDoer != null)
                AttackDoer.Dispose();
            Legionary _;
            LegionariesByRef.TryRemove(_ref, out _);
            IsValid = false;
            UnassignFromLegion();
            if (ExecutingStrategy != null)
            {
                await ExecutingStrategy.Terminate();
                foreach (var eStrat in _eventStrategies)
                    await eStrat.Key.Terminate();
            }
            ExecutingStrategy = null;
            _eventStrategies.Clear();
        }

        public async ValueTask Do([NotNull]Strategy strategy)
        {
            strategy.AssignToLegionary(this);
            try
            {
                if (ExecutingStrategy != null)
                    await ExecutingStrategy.Terminate();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"{e.ToString()}").Write();
            }
            ExecutingStrategy = strategy;
        }

        // Used only for dbg purposes.
        public event Action<StrategyDef, AIEvent> OnEventHandled;

        public bool HandleEvent(AIEvent aiEvent)
        {
            if (aiEvent.Initiator == null)
                return false;
            var eventType = aiEvent.GetType();
            if (Def == null)
            {
                OnEventHandled?.Invoke(null, aiEvent);
                return false;
            }
            var handler = Def.EventHandlers.FirstOrDefault(x => x.EventDef == aiEvent.StaticData);
            // if no `HandlerStrategy` in `handler`, try get another handler from `_currentStrategy`:
            if (handler.HandlerStrategy.Target == null)
            {
                if (_currentStrategy == null)
                {
                    OnEventHandled?.Invoke(null, aiEvent);
                    return false;
                }
                handler = _currentStrategy.Def.EventHandlers.FirstOrDefault(x => x.EventDef == aiEvent.StaticData);
                if (handler.HandlerStrategy.Target == null)
                {
                    OnEventHandled?.Invoke(null, aiEvent);
                }
            }
            if (handler.HandlerStrategy.Target == null)
                foreach (var subStrat in AllSubStrategies)
                {
                    handler = subStrat.Key.Def.EventHandlers.FirstOrDefault(x => x.EventDef == aiEvent.StaticData);
                    if (handler.HandlerStrategy.Target != null)
                        break;
                }
            if (handler.HandlerStrategy.Target == null)
            {
                OnEventHandled?.Invoke(null, aiEvent);
                return false;
            }
            var strategy = new Strategy(handler.HandlerStrategy, aiEvent);
            DemandsUpdate?.Invoke(0);
            strategy.AssignToLegionary(this);
            _eventStrategies.TryAdd(strategy, true);
            OnEventHandled?.Invoke(handler.HandlerStrategy, aiEvent);
            return true;
        }
        long _lastTimeKnowledgeWasUpdated = 0;
        public async ValueTask Update()
        {
            TickedNodes.Clear();

            AIProfiler.BeginSample("UpdateLegionary");
            //Logger.IfError()?.Message($"Memory: {string.Join(",", Knowledge.Memory.MemoryPieces.Select(x => x.Key.StatDef.____GetDebugShortName()))}").Write();
            //Logger.IfError()?.Message($"Knowledge: {string.Join("  ; ", Knowledge.KnownStuff.Where(x => !(x.Value is KnowledgeSourceTransformer)).Select(x => $"{ x.Key.____GetDebugShortName()} {string.Join(",", x.Value.Legionaries.Select(y => y.Key.Guid.ToString().Substring(10)))}"))}").Write();
            //Logger.IfError()?.Message($"Knowledge: {string.Join("  ; ", Knowledge.KnownStuff.Where(x => (x.Value is KnowledgeSourceTransformer)).Select(x => $"{ (((KnowledgeSourceTransformerDef)x.Value.Def).InterpretAsStat.Target == null ? x.Key.____GetDebugShortName() : ((KnowledgeSourceTransformerDef)x.Value.Def).InterpretAsStat.Target.____GetDebugShortName())} {string.Join(",", x.Value.Legionaries.Select(y => y.Key.Guid.ToString().Substring(10)))}"))}").Write();
            if (_lastTimeKnowledgeWasUpdated + 300 < SyncTime.NowUnsynced)
            {
                _lastTimeKnowledgeWasUpdated = SyncTime.NowUnsynced;
                await Knowledge.Update();
            }

            // 1. Handle event-strategies:
            Strategy newEventStrategy = null;
            foreach (var eventStrategy in _eventStrategies)
            {
                var executingScript = eventStrategy;
                //try
                //{
                using (Office.Visit().CurrentInvestigation(Ref.Guid)?.BeginInvestigating(new TickStrategyEvent() { Strategy = executingScript.Key.Description = new StrategyStatus() }))
                {
                    AIProfiler.BeginSample("Run ", ((IResource)executingScript.Key.Def).Address);

                    var state = await executingScript.Key.Run();
                    if (state == ScriptResultType.Running)
                    {
                        if (newEventStrategy != null)
                            await newEventStrategy.Terminate();
                        newEventStrategy = executingScript.Key;
                    }

                    // Clear consumed dbg data:
                    if (executingScript.Key.Description != null)
                        executingScript.Key.Description = null;

                    AIProfiler.EndSample();
                }
                //}
                //catch (Exception e)
                //{
                //    executingScript.LastState = ScriptResultType.Failed;
                //    executingScript.Terminate();
                //    throw e;
                //}
            }
            _eventStrategies.Clear();

            if (newEventStrategy != null)
            {
                await Do(newEventStrategy);
            }
            else if (ExecutingStrategy != null)
            {
                //try
                //{
                TickedNodes.Clear();
                TickStrategyEvent debugEvent = null;
                using (Office.Visit().CurrentInvestigation(Ref.Guid)?.BeginInvestigating(debugEvent = new TickStrategyEvent() { Strategy = ExecutingStrategy.Description = new StrategyStatus() }))
                {
                    if (AIProfiler.EnableProfile)
                        AIProfiler.BeginSample("Run ", ((IResource)ExecutingStrategy.Def).Address);
                    var state = await ExecutingStrategy.Run();
                    //if (state == ScriptResultType.Succeeded || state == ScriptResultType.Failed)
                    //    _executingStrategy = null;
                    if (ExecutingStrategy.Description != null)
                        ExecutingStrategy.Description = null;
                    if (debugEvent != null)
                        debugEvent.TickedNodes = new HashSet<object>(TickedNodes);
                    if (AIProfiler.EnableProfile)
                        AIProfiler.EndSample();
                }
                //}
                //catch (Exception e)
                //{
                //    _executingStrategy.LastState = ScriptResultType.Failed;
                //    _executingStrategy.Terminate();
                //    throw e;
                //}
            }

            LegionaryKnowledgeAndMemoryStatus knows = null;
            using (Office.Visit().CurrentInvestigation(Ref.Guid)?.BeginInvestigating(knows = new LegionaryKnowledgeAndMemoryStatus()))
            {
                if (knows != null)
                {
                    knows.MemorySnapshot = new MemorySnapshot(Knowledge.Memory, knows);
                    knows.KnowledgeSnapshot = new KnowledgeSnapshot(Knowledge, knows.MemorySnapshot, knows);
                    knows.Cooldowns = new CooldownsStatus(this);
                }
            }
            AIProfiler.BeginSample("Think");
            if (_brain != null)
                await _brain.Think();
            AIProfiler.EndSample();
            AIProfiler.EndSample(); //UpdateLegionary
        }

        public override void OnAssignToLegion(Legion newLegion)
        {
            AssignedLegion.Knowledge.Connect(Knowledge);
        }

        public override void OnUnassignToLegion()
        {

            AssignedLegion.Knowledge.Disconnect(Knowledge);
        }

        #region IInputActionHandlersFactory

        public T Create<T>(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId) where T : IInputActionHandler
        {
            if (typeof(T) == typeof(IInputActionTriggerHandler))
                return (T)CreateTriggerHandler(action, desc, bindingId);
            if (typeof(T) == typeof(IInputActionValueHandler))
                return (T)CreateValueHandler(action, desc, bindingId);
            throw new NotSupportedException($"{typeof(T)}");
        }

        private IInputActionTriggerHandler CreateTriggerHandler(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId)
        {
            switch (desc)
            {
                case IInputActionHandlerSpellDescriptor d:
                    return new InputActionHandlerSpell(d, SpellDoer, bindingId);
                case IInputActionHandlerSpellOnceDescriptor d:
                    return new InputActionHandlerSpell(d, SpellDoer, bindingId);
                case IInputActionHandlerSpellContinuousDescriptor d:
                    return new InputActionHandlerSpellContinuous(d, SpellDoer, bindingId);
                case IInputActionHandlerSpellBreakerDescriptor d:
                    return new InputActionHandlerSpellBreaker(d, SpellDoer, Ref, Repository, bindingId);
                default:
                    throw new NotSupportedException($"{desc}");
            }
        }

        private IInputActionValueHandler CreateValueHandler(InputActionDef action, IInputActionHandlerDescriptor desc, int bindingId)
        {
            switch (desc)
            {
                default:
                    throw new NotSupportedException($"{desc}");
            }
        }
        #endregion
    }




}
