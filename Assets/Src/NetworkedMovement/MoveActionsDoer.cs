using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.RegionsScenicObjects;
using ResourcesSystem.Loader;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using ResourceSystem.Utils;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Locomotion;
using UnityEngine;
using UnityEngine.AI;
using SharedCode.Entities.Engine;
using SharedCode.Utils.DebugCollector;
using SharedCode.Serializers;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    /// <summary>
    /// Этот класс - логика про MoveActions, вынесенная из класса `Pawn` просто для лучшей огранизации кода.
    /// Разделить их полностью пока не удалось. Поэтому есть ряд ссылок отсюда в `Pawn`.
    /// Works only on server.
    /// </summary>
    internal class MoveActionsDoer : IMoveActionsDoer, ILocomotionInputSource<MobInputs>, /*ILocomotionDebugable*/IDebugInfoProvider, 
        ILocomotionInputReceiver, IGuideProvider, IStopAndRestartable
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(MoveActionsDoer));

        private const string MoveActionsConstantsPath = @"/AI/Defs/MobMoveActionsConstants";
        private static MobMoveActionsConstantsDef MoveActionsConstants;
        private readonly LocomotionInputMediator<MobInputs> _externalInput = new LocomotionInputMediator<MobInputs>();

        private readonly IMoveActionPosition _selfPosition;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly IEntitiesRepository _repository;
        private readonly OuterRef _entityRef;
        private Guid _entityId => _entityRef.Guid;

        // is Off when lostOwnership until got back
        private bool _isActive;// = true;
        private bool _disposed;
        private long _startedAt;
        private long _finishedAt;

        //#Dbg
        public bool DBG_IsActive => _isActive;

        private readonly InputState<MobInputs> _input = new InputState<MobInputs>();
        public ActionHandler _newAction;
        public ActionHandler _currentAction;

        internal MobLocomotionReactions Reactions { get; } = new MobLocomotionReactions();

        public IMoveAction CurrentAction => _currentAction.Action;

        public long LastActionStartedAt => _startedAt;

        public long LastActionFinishedAt => _finishedAt;

        public SharedCode.Utils.Vector3 Guide { get; }

        public event Action ActionAdded;

        public event Action ActionStarted;
        public event Action CurrActionWillBeFinished; ///#PZ-13568

        ///#PZ-13761: #Dbg: 
        private Pawn _pawn;

        internal MoveActionsDoer(IMoveActionPosition selfPosition, NavMeshAgent navMeshAgent, OuterRef entityRef, IEntitiesRepository repository, Pawn pawn)
        //internal MoveActionsDoer(IMoveActionPosition selfPosition, NavMeshAgent navMeshAgent, SpellDoerAsync spellDoer, Pawn dbg_pawn)
        {
            _selfPosition = selfPosition ?? throw new ArgumentNullException(nameof(selfPosition));
            _navMeshAgent = navMeshAgent ?? throw new ArgumentNullException(nameof(navMeshAgent));
            _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            MoveActionsConstants = MoveActionsConstants ?? GameResourcesHolder.Instance.LoadResource<MobMoveActionsConstantsDef>(MoveActionsConstantsPath) ?? new MobMoveActionsConstantsDef();
            _pawn = pawn;
            _isActive = true;
        }

        //Stop & clean to be clean & could be used again later
        public void Stop()
        {
            _isActive = false;
            _newAction = default;
            if (_currentAction.Action == null)
                return;

            ///#PZ-17474: ??: TODO: Может быть не нужно стопать акт.спелл перемещения - если это каким-то чудом подхватывается новым MAD'ом на новом Cl при создании всего (ServerLoco и MAD'а) - посмотреть внимательно код и/или ask Vitaly||Vova_I
            ///     //Если такое чудо происходит, то квар, кружляя в стрейфе, переходит своим ownership'ом с Cl на Cl, но при этом продолжает выполнять свой стрейф.
            OnActionFinished(ref _currentAction, FinishReasonType.Fail);
            _finishedAt = SyncTime.Now;
            _externalInput.Clean();

            //#note: Подписчиков `Reactions` чистить не нужно т.к. подписки только в MoveAction'ах и только на время их исполнения, а мы чуть выше тут финишировали current

            //todo: и возможно сделать 2 ресет-метода  - 1 на отпускании, 2ОЙ на подключении снова. Похоже, что достаточно одного этого
        }
        public void Restart()
        {
#if DEBUG
            DBG_EnsureIsClear();
#endif
            _isActive = true;
        }

        //#Dbg:
        public void DBG_EnsureIsClear()
        {
            if (_isActive || !_newAction.Equals(default(ActionHandler)) || _currentAction.Action != null || !_externalInput.DBG_IsCLear())
                Logger.IfError()?.Message($"DBG_EnsureIsClear failed: {_isActive} || {!_newAction.Equals(default(ActionHandler))} || {_currentAction.Action != null} || {!_externalInput.DBG_IsCLear()}");
        }

        internal void Dispose()
        {
            _disposed = true;
            _newAction = default(ActionHandler);
            if (_currentAction.Action != null)
                OnActionFinished(ref _currentAction, FinishReasonType.Fail);
        }

        // === 1. Internal API: =============================================================

        internal void Update(Pawn.SimulationLevel simulationLevel)
        {
            if (!_isActive || _disposed) return;

            Collect.IfActive?.Event("MoveActionDoer.Update", _entityId);
 
            if (_newAction.Action != null)
            {
                var newAction = _newAction;
                _newAction = default(ActionHandler);
                if (_currentAction.Action != null)
                {
                    OnActionFinished(ref _currentAction, FinishReasonType.Fail);
                    _finishedAt = SyncTime.Now;
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Start action: {0} (effect:{1} orderId: {2})",  newAction.Action, newAction.Effect.____GetDebugAddress(), newAction.SpellId).Write();
                if (newAction.Action.Init())
                {
                    // Logger.IfInfo()?.Message($"{_entityId} inited new move action").Write();
                    _currentAction = newAction;
                    _startedAt = SyncTime.Now;
                    ActionStarted?.Invoke();
                    Collect.IfActive?.EventBgn($"MoveActionDoer.Action.{_currentAction.Effect.____GetDebugRootName()}", _entityId, _currentAction.SpellId);
                }
                else
                    OnActionFinished(ref newAction, FinishReasonType.Fail);
            }

            if (_currentAction.Action != null)
            {
                //old:
                //if (_agent != null)
                //    if (!_agent.enabled)
                //        _agent.enabled = true;

                var actionResult = _currentAction.Action.Tick(simulationLevel);

                switch (actionResult)
                {
                    //cancel spell if we're on server
                    case MoveActionResult.Finished:
                        OnActionFinished(ref _currentAction, FinishReasonType.Success);
                        _finishedAt = SyncTime.Now;
                        break;
                    case MoveActionResult.Failed:
                        OnActionFinished(ref _currentAction, FinishReasonType.Fail);
                        _finishedAt = SyncTime.Now;
                        break;
                }


            }

            _lastUpdated = DateTime.UtcNow;
            ++_updatesCount;
        }

        internal void DrawOnGUI()
        {
            if (!DebugExtension.Draw)
                return;
            if (_currentAction.Effect == null)
                return;
            int index = 0;
            var texRect = new Rect((_selfPosition.Position + new UnityEngine.Vector3(0, 2.5f, 0)).WorldToGuiPoint(), new UnityEngine.Vector2(400, 40));
            index = Pawn.Label($"{_currentAction.Effect.MoveType} {_currentAction.SpellId}", index, texRect);
            index = Pawn.Label($"{_currentAction.Effect.____GetDebugAddress()}", index, texRect);
        }

        bool IMoveActionsDoer.OnMoveEffectStarted(SpellId spellId, MoveEffectDef effect, SpellDef spell, SpellPredCastData cast)
        {
            if (!_isActive)
            {
                Logger.IfError()?.Message("#PZ-17474: UNEXPECTED: SUBSCRIPTION `OnMoveEffectStarted` LEFT, while !_isActive");
                return false;
            }

            var result = OnMoveEffectStarted(spellId, effect, spell, cast);
            if (!result)
                StopMoveSpell(spellId, effect, FinishReasonType.Fail);
            return result;
        }
        
        private bool OnMoveEffectStarted(SpellId spellId, MoveEffectDef effect, SpellDef spell, SpellPredCastData cast)
        {
            if (!_isActive || _disposed) return false;

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OnMoveEffectStarted. effect:{0} spell:{3} spellId:{1} cast:{2}",  effect.____GetDebugAddress(), spellId, cast, spell.____GetDebugAddress()).Write();
            Collect.IfActive?.Event($"MoveActionDoer.Effect.{effect.____GetDebugRootName()}", _entityId);

            MoveEffectDef.EffectType et = effect.MoveType;
            IMoveAction currentAction = null;
            if (et == MoveEffectDef.EffectType.FollowPathToTarget && effect.Target.Target.GetGo(cast) == null && effect.Vec3.Target.GetVec3(cast, default) != default)
                et = MoveEffectDef.EffectType.FollowPathToPosition;
            switch (et)
            {
                case MoveEffectDef.EffectType.FollowPathToPosition:
                    {
                        var target = new MoveActionPointTarget(effect.Vec3.Target.GetVec3(cast, _navMeshAgent.transform.position));
                        if (spell.IsInfinite)
                            currentAction = new MoveActionFollowPath(agent: _navMeshAgent,
                                target: target,
                                acceptedRange: effect.AcceptedRange,
                                rotationType: effect.Rotation,
                                moveModifier: effect.Modifier,
                                speedFactor: effect.SpeedFactor,
                                self: _selfPosition,
                                entityId: _entityId,
                                reactions: Reactions,
                                constants: MoveActionsConstants,
                                dbg_pawn: _pawn);
                        else
                            currentAction = new MoveActionFollowPathAndKeepDistance(agent: _navMeshAgent,
                                target: target,
                                acceptedRangeMin: 0,
                                acceptedRangeMax: effect.AcceptedRange,
                                rotationType: effect.Rotation,
                                moveModifier: effect.Modifier,
                                speedFactor: effect.SpeedFactor,
                                self: _selfPosition,
                                entityId: _entityId,
                                reactions: Reactions,
                                constants: MoveActionsConstants,
                                dbg_pawn: _pawn);
                    }
                    break;
                case MoveEffectDef.EffectType.FollowPathToTarget:
                    {
                        var target = new MoveActionTransformTarget(effect, cast, _navMeshAgent.transform);
                        if (spell.IsInfinite && !effect.KeepDistance)
                            currentAction = new MoveActionFollowPath(agent: _navMeshAgent,
                                target: target,
                                acceptedRange: effect.AcceptedRange,
                                rotationType: effect.Rotation,
                                moveModifier: effect.Modifier,
                                speedFactor: effect.SpeedFactor,
                                self: _selfPosition,
                                entityId: _entityId,
                                reactions: Reactions,
                                constants: MoveActionsConstants,
                                dbg_pawn: _pawn);
                        else
                            currentAction = new MoveActionFollowPathAndKeepDistance(agent: _navMeshAgent,
                                target: target,
                                acceptedRangeMin: effect.KeepDistance ? effect.AcceptedRange - effect.KeepDistanceTreshold : 0,
                                acceptedRangeMax: effect.KeepDistance ? effect.AcceptedRange + effect.KeepDistanceTreshold : effect.AcceptedRange,
                                rotationType: effect.Rotation,
                                moveModifier: effect.Modifier,
                                speedFactor: effect.SpeedFactor,
                                self: _selfPosition,
                                entityId: _entityId,
                                reactions: Reactions,
                                constants: MoveActionsConstants,
                                dbg_pawn: _pawn);
                    }
                    break;
                case MoveEffectDef.EffectType.StrafeAroundTarget:
                    currentAction = new StrafeAction(self: _selfPosition,
                                                      entityId: _entityId,
                                                      target: new MoveActionTransformTarget(effect, cast, _navMeshAgent.transform),
                                                      rotationType: effect.Rotation,
                                                      direction: effect.FixedDirection.ToXYZ(),
                                                      moveModifier: effect.Modifier,
                                                      speedFactor: effect.SpeedFactor);
                    break;
                case MoveEffectDef.EffectType.LookAt:
                    currentAction = new LookAtAction(self: _selfPosition,
                                                      target: new MoveActionTransformTarget(effect, cast, _navMeshAgent.transform),
                                                      entityId: _entityId);
                    break;
                case MoveEffectDef.EffectType.JumpToTargetPosition:
                    if (effect.Target != null)
                        currentAction = new JumpToTargetPositionAction(self: _selfPosition,
                                                                        target: new MoveActionTransformTarget(effect, cast, _navMeshAgent.transform),
                                                                        reactions: Reactions,
                                                                        entityId: _entityId);
                    else
                        currentAction = new JumpToTargetPositionAction(self: _selfPosition,
                                                                        target: new MoveActionPointTarget(effect.Vec3.Target.GetVec3(cast, _selfPosition.Position)),
                                                                        reactions: Reactions,
                                                                        entityId: _entityId);
                    break;

            }

            if (currentAction != null)
            {
                _newAction = new ActionHandler(currentAction, spellId, effect);
                ActionAdded?.Invoke();
                return true;
            }
            else if (effect != null)
                Logger.IfError()?.Message($"NULL ACTION FROM {effect.____GetDebugShortName()}").Write();
            return false;
        }

        void IMoveActionsDoer.OnMoveEffectFinished(SpellId orderId, MoveEffectDef effect)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OnMoveEffectFinished. effect:{0} spellId:{1}",  effect.____GetDebugAddress(), orderId).Write();

            if (_currentAction.Effect == effect && _currentAction.SpellId == orderId)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Terminate current action {0}",  _currentAction.Action).Write();
                CurrActionWillBeFinished?.Invoke();
                _currentAction.Action?.Terminate();
                _currentAction.Action?.Dispose();
                _currentAction = default(ActionHandler);
                _finishedAt = SyncTime.Now;
            }

            if (_newAction.Effect == effect && _newAction.SpellId == orderId)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Abort new action {0}",  _newAction.Action).Write();
                _newAction = default(ActionHandler);
            }
        }

        /// <summary>
        /// Just calls Wizard.StopCastSpell
        /// </summary>
        private void OnActionFinished(ref ActionHandler actionHandler, FinishReasonType reason)
        {
            if (actionHandler.Action != null)
            {
                // Logger.IfInfo()?.Message($"{_entityId} action finished").Write();
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OnActionFinished | action:{2} effect:{0} spellId:{1}",  actionHandler.Effect.____GetDebugAddress(), actionHandler.SpellId, actionHandler.Action).Write();
                CurrActionWillBeFinished?.Invoke();
                actionHandler.Action.Dispose();
                Collect.IfActive?.EventEnd(actionHandler.SpellId);
                if (actionHandler.SpellId != SpellId.Invalid && actionHandler.Effect.StopSpell)
                    StopMoveSpell(actionHandler.SpellId, actionHandler.Effect, reason);
                actionHandler = default(ActionHandler);
            }
        }

        private void StopMoveSpell(SpellId sid, MoveEffectDef edef, FinishReasonType reason)
        {
            var repository = _repository;
            if (repository != null)
                AsyncUtils.RunAsyncTask(
                    async () =>
                    {
                        using (var ent = await repository.Get(_entityRef.TypeId, _entityRef.Guid))
                        {
                            ent.TryGet<IHasMobMovementSyncAlways>(_entityRef.TypeId, _entityRef.Guid, ReplicationLevel.Always, out var mob);
                            if (mob != null)
                                await mob.MovementSync.StopMovement(sid, edef, reason == FinishReasonType.Success);
                        }
                    });
        }
        
        #region ILocomotionInputSource<MobInputs>

        //private readonly InputState<MobInputs> _defaultInput = new InputState<MobInputs>();
        int _dbgCounter = 10;

        IInputState<MobInputs> ILocomotionInputSource<MobInputs>.GetInput()
        {
            var guide = LocomotionHelpers.WorldToLocomotionVector(_pawn.transform.forward);

            _input.Clear();
            _input[MobInputs.SpeedFactor] = 1;
//            _input[CharacterInputs.Guide] = guide.Horizontal.normalized;
            //foreach (var handler in _locomotionInputActionHandlers)
            //    handler.FetchInputValue(_input);

            _externalInput.ApplyTo(_input);
            _currentAction.Action?.GetLocomotionInput(_input);
            if (Logger.IsTraceEnabled && --_dbgCounter > 0)
                Logger.IfTrace()?.Message(_entityId, $"--++== B. WORK:  MAsD.GetInput V (: {_input}). <{0}>").Write();
            return _input;
        }

        #endregion ILocomotionInputSource<MobInputs>

        public void SetInput(InputAxis it, float value) => _externalInput.SetInput(it, value);
        public void SetInput(InputAxes it, SharedCode.Utils.Vector2 value) => _externalInput.SetInput(it, value);
        public void SetInput(InputTrigger it, bool value) => _externalInput.SetInput(it, value);
        public void PushInput(object causer, string inputName, float value) => _externalInput.PushInput(causer, inputName, value);
        public void PopInput(object causer, string inputName) => _externalInput.PopInput(causer, inputName);
        public struct ActionHandler
        {
            public readonly IMoveAction Action;
            public readonly SpellId SpellId;
            public readonly MoveEffectDef Effect;

            public ActionHandler(IMoveAction action, SpellId spellId, MoveEffectDef effect)
            {
                Action = action;
                SpellId = spellId;
                Effect = effect;
            }
        }

        // --- ILocomotionDebugable: ------------------------------------
        private DateTime _lastUpdated;
        private long _updatesCount;

        // --- IDebugInfoProvider: ---------------------------------
        public string GetDebugInfo()
        {
            return $"MoveActionDoer:: MovingAction:  {CurrentAction?.GetType().Name ?? "---"}\n"
                 + $"MoveActionDoer:: LastUpdateTime:  {SharedHelpers.TimeStamp(_lastUpdated)}\n"
                 + $"MoveActionDoer:: _updatesCount:  {_updatesCount}";
        }

    }

}