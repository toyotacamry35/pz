using Assets.Src.Aspects.Impl;
using Assets.Src.InteractionSystem;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog.Fluent;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Locomotion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uins;
using UnityEngine;

using SVector3 = SharedCode.Utils.Vector3;

#if false
namespace Assets.Src.Aspects.Doings
{
    public enum ActionState
    {
        Running,
        Complete,
        Failed
    }

    public abstract class BotAction
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Bot");

        public ActionState State { get; protected set; } = ActionState.Running;
        public IReadOnlyDictionary<string, object> LocalState => _localKnowledge;
        protected Dictionary<string, object> _localKnowledge { get; } = new Dictionary<string, object>();

        protected BotActionDef Def { get; }
        protected BotActionsStateMachine StateMachine { get; }

        protected BotAction(BotActionDef def, BotActionsStateMachine basm)
        {
            Def = def;
            StateMachine = basm;
        }

        public virtual void LogAction(BotActionsStateMachine basm)
        {
            Logger.IfDebug()?.Message(this.GetType().Name + " " + basm.EntityId).Write();
        }

        protected abstract BotInputs GetInputsImpl();

        public BotInputs GetInputs()
        {
            if (State == ActionState.Running)
            {
                try
                {
                    return GetInputsImpl();
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Message(e).Write();
                    return default(BotInputs);
                }
            }
            return default(BotInputs);
        }
    }

    public class BotSelectTarget : BotAction
    {
        public BotSelectTarget(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotSelectTargetDef;
            if (defClass.ObjectName == null)
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }

            var _currentPosition = StateMachine._transform.position;
            float maxDistance = float.MaxValue;
            GameObject result = null;
            var collidersInSphere = Physics.OverlapSphere(_currentPosition, PhysicsChecker.CheckRadius(defClass.SearchRadius, "BotLookToTarget"));
            foreach (var collider in collidersInSphere)
            {
                var gameObj = collider.transform.gameObject;
                float distance = Vector3.Distance(_currentPosition, gameObj.transform.position);
                if (gameObj.name.Contains(defClass.ObjectName) && distance < maxDistance)
                {
                    maxDistance = distance;
                    result = gameObj;
                }
            }

            if (result == null)
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }

            Vector3 cameraDirection = result.transform.position - StateMachine._transform.position;
            _localKnowledge["Target"] = result;
            State = ActionState.Complete;
            return default(BotInputs);
        }
    }

    public class BotLookToTarget : BotAction
    {
        public BotLookToTarget(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            object target;
            if (!_localKnowledge.TryGetValue("Target", out target))
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }

            Vector3 cameraDirection = ((GameObject)target).transform.position - StateMachine._transform.position;
            State = ActionState.Complete;
            return new BotInputs { CameraDirection = cameraDirection };
        }
    }

    public class BotPinTargetPosition : BotAction
    {
        public BotPinTargetPosition(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            object targetObj;
            if (!_localKnowledge.TryGetValue("Target", out targetObj))
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }

            var target = (GameObject)targetObj;
            var _targetPosition = target.transform.position;

            _localKnowledge["Point"] = _targetPosition;
            State = ActionState.Complete;
            return default(BotInputs);
        }
    }

    public class BotGoToTarget : BotAction
    {
        private Vector3 _lastCoord;
        private DateTime _lastTime;

        public BotGoToTarget(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            _lastCoord = basm._transform.position;
            _lastTime = DateTime.Now;
        }
        protected override BotInputs GetInputsImpl()
        {
            object targetObj;
            if (!_localKnowledge.TryGetValue("Target", out targetObj))
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }
            var target = (GameObject)targetObj;
            var _targetPosition = target.transform.position;

            if (DateTime.Now - _lastTime > TimeSpan.FromSeconds(2))
            {
                if ((_lastCoord - StateMachine._transform.position).sqrMagnitude < 0.01f)
                    Logger.IfError()?.Message().Message("Bot {0}: Seems I am stuck at position {1}", StateMachine.EntityId, _lastCoord).Write();

                _lastCoord = StateMachine._transform.position;
                _lastTime = DateTime.Now;
            }


            Vector3 camDirection = StateMachine._camera;
            Vector3 camFlatDirection = StateMachine._camera.ToXZ().normalized;

            var direction = _targetPosition - StateMachine._transform.position;
            var projModOnCamForwardDirection = Vector2.Dot(direction, camFlatDirection);
            var projModOnCamLateralDirection = -Vector2.Dot(direction, Vector2.Perpendicular(camFlatDirection));
            Vector2 projOnCamBasis = new Vector2(projModOnCamForwardDirection, projModOnCamLateralDirection).normalized;
            var projX = projOnCamBasis.x;
            var projY = projOnCamBasis.y;

            var inputs = new BotInputs();
            inputs.AxisForward = projX;
            inputs.AxisLateral = projY;
            if (direction.magnitude < 1)
                State = ActionState.Complete;
            return inputs;
        }
    }

    public class BotGoToPoint : BotAction
    {
        private Vector3 _lastCoord;
        private DateTime _lastTime;

        public BotGoToPoint(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            _lastCoord = basm._transform.position;
            _lastTime = DateTime.Now;
        }
        protected override BotInputs GetInputsImpl()
        {
            object targetPointObj;
            if (!_localKnowledge.TryGetValue("Point", out targetPointObj))
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }
            var _targetPosition = (Vector3)targetPointObj;

            if (DateTime.Now - _lastTime > TimeSpan.FromSeconds(2))
            {
                if ((_lastCoord - StateMachine._transform.position).sqrMagnitude < 0.01f)
                    Logger.IfError()?.Message().Message("Bot {0}: Seems I am stuck at position {1}", StateMachine.EntityId, _lastCoord).Write();

                _lastCoord = StateMachine._transform.position;
                _lastTime = DateTime.Now;
            }


            Vector3 camDirection = StateMachine._camera;
            Vector3 camFlatDirection = StateMachine._camera.ToXZ().normalized;

            var direction = _targetPosition - StateMachine._transform.position;
            var projModOnCamForwardDirection = Vector2.Dot(direction, camFlatDirection);
            var projModOnCamLateralDirection = -Vector2.Dot(direction, Vector2.Perpendicular(camFlatDirection));
            Vector2 projOnCamBasis = new Vector2(projModOnCamForwardDirection, projModOnCamLateralDirection).normalized;
            var projX = projOnCamBasis.x;
            var projY = projOnCamBasis.y;

            var inputs = new BotInputs();
            inputs.AxisForward = projX;
            inputs.AxisLateral = projY;
            if (direction.magnitude < 1)
                State = ActionState.Complete;
            return inputs;
        }
    }

    public class BotInteractWithTarget : BotAction
    {
        private BotGoToTarget _botGoToTarget;
        private BotGoToTargetDef _botGoToTargetDef;
        private BotDoAction _botDoAction;
        private BotDoActionDef _botDoActionDef;
        private long _actionsTimer = 200;
        private long _lastActionTime;
        private BotActionType _actionMode;

        public BotInteractWithTarget(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotInteractWithTargetDef;
            _actionMode = defClass.ActionMode;
            _botGoToTargetDef = new BotGoToTargetDef() { ObjectName = defClass.ObjectName, SearchRadius = defClass.SearchRadius };
            _botGoToTarget = new BotGoToTarget(_botGoToTargetDef, basm);
            _botDoActionDef = new BotDoActionDef() { ActionName = defClass.ActionName, AxisForward = defClass.AxisForward, AxisLateral = defClass.AxisLateral };
            _botDoAction = new BotDoAction(_botDoActionDef, basm);
        }

        protected override BotInputs GetInputsImpl()
        {
            var inputs = _botGoToTarget.GetInputs();
            if (_botGoToTarget.State == ActionState.Running)
                return inputs;

            if (_botGoToTarget.State == ActionState.Failed)
            {
                State = ActionState.Failed;
                return default(BotInputs);
            }

            _localKnowledge["Target"] = _botGoToTarget.LocalState["Target"];

            switch(_actionMode)
            {
                case BotActionType.Continuous:
                    {
                        return _botDoAction.GetInputs();
                    }
                case BotActionType.RepeatedClicking:
                    {
                        var currentTime = global::ColonyShared.SharedCode.Utils.SyncTime.NowUnsynced;
                        if (_lastActionTime + _actionsTimer > currentTime)
                        {
                            inputs.AxisForward = 0;
                            inputs.AxisLateral = 0;
                            return inputs;
                        }
                        else
                        {
                            _lastActionTime = currentTime;
                            return _botDoAction.GetInputs();
                        }
                    }
                case BotActionType.OneTime:
                    {
                        State = ActionState.Complete;
                        return _botDoAction.GetInputs();
                    }
                default:
                    {
                        State = ActionState.Complete;
                        return new BotInputs();
                    }
            }
        }
    }

    public class BotExploreTarget : BotAction
    {
        public bool _explored = false;
        private bool _explorationAttempted = false;
        private BotInteractWithTarget _botExploreTarget;
        private BotInteractWithTargetDef _botExploreTargetDef;
        private Task<SpellDef> _checkIfExplored = default(Task<SpellDef>);
        private bool _interactCheckFired = false;

        public BotExploreTarget(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotExploreTargetDef;
            _botExploreTargetDef = new BotInteractWithTargetDef() { ActionName = BotInputAction.Interact, ObjectName = defClass.ObjectName, ActionMode = BotActionType.OneTime };
            _botExploreTarget = new BotInteractWithTarget(_botExploreTargetDef, basm);
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotExploreTargetDef;
            if (_checkIfExplored == default(Task<SpellDef>))
            {
                if (_botExploreTarget.State == ActionState.Running)
                    return _botExploreTarget.GetInputs();

                if(_botExploreTarget.State == ActionState.Failed)
                {
                    State = ActionState.Failed;
                    return default(BotInputs);
                }

                _localKnowledge["Target"] = _botExploreTarget.LocalState["Target"];

                if (!_interactCheckFired)
                {
                    var target = (GameObject)_localKnowledge["Target"];
                    var spellDoer = StateMachine._transform.gameObject.GetComponent<BotBrain>().GetSpellDoerAsync();
                    var targetRef = target?.Kind<EntityGameObject>()?.OuterRef ?? OuterRef<IEntityObject>.Invalid;
                    var interactiveTarget = target?.Kind<Interactive>();
                    if (targetRef != default(OuterRef<IEntityObject>) && interactiveTarget != default(Interactive))
                    {
                        var ctx = new SpellActivatorContext(
                            spellDoer: spellDoer,
                            caster: spellDoer.gameObject,
                            targetRef: target?.Kind<EntityGameObject>()?.OuterRef ?? OuterRef<IEntityObject>.Invalid,
                            interactiveTarget: target?.Kind<Interactive>(),
                            direction: SVector3.AlignToSector(new SVector3(LocomotionHelpers.LocomotionToWorldVector(new LocomotionVector(0, 0, 0))), 8, SVector3.up)
                        );
                        _checkIfExplored = AsyncUtilsUnity.RunAsyncTaskFromUnity(() => ctx.InteractiveTarget.ChooseSpell(ctx.Caster, SpellDescription.AttackSpellKey), ctx.SpellDoer.LocalRepository);
                    }
                    else
                    {
                        Logger.Log(NLog.LogLevel.Error, "Some fields in SpellActivatorContext is null in BotMineResource action");
                        return default(BotInputs);
                    }
                }
                return new BotInputs();
            }
            else
            {
                switch (_checkIfExplored.Status)
                {
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                        State = ActionState.Failed;
                        break;
                    case TaskStatus.RanToCompletion:
                        if (_checkIfExplored.Result != default(SpellDef))
                            _explored = true;
                        State = ActionState.Complete;
                        break;
                    default:
                        break;
                }
                return new BotInputs();
            }
        }
    }

    public class BotMineResource : BotAction
    {
        private BotExploreTarget _botExploreTarget;
        private BotExploreTargetDef _botExploreTargetDef;
        private BotInteractWithTarget _botMineTarget;
        private BotInteractWithTargetDef _botMineTargetDef;
        private bool _explored = false;
        private long _glitchTimer = 20000;
        private long _lastActionTime = long.MaxValue;

        public BotMineResource(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotMineResourceDef;
            _botExploreTargetDef = new BotExploreTargetDef() { ObjectName = defClass.ObjectName };
            _botExploreTarget = new BotExploreTarget(_botExploreTargetDef, basm);
            _botMineTargetDef = new BotInteractWithTargetDef() { ActionName = BotInputAction.Mine, ObjectName = defClass.ObjectName, ActionMode = BotActionType.Continuous };
            _botMineTarget = new BotInteractWithTarget(_botMineTargetDef, basm);
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotMineResourceDef;
            if (!_explored)
            {
                var inputs = _botExploreTarget.GetInputs();
                if (_botExploreTarget.State != ActionState.Running)
                {
                    if (_botExploreTarget.State == ActionState.Complete)
                    {
                        _explored = true;
                    }
                    else
                    {
                        State = ActionState.Failed;
                    }
                }
                return inputs;
            }

            var currentTime = global::ColonyShared.SharedCode.Utils.SyncTime.NowUnsynced;
            if (_lastActionTime + _glitchTimer > currentTime)
            {
                var inp = _botMineTarget.GetInputs();
                if (_botMineTarget.State != ActionState.Running)
                    State = _botMineTarget.State;
                return inp;
            }
            else
            {
                _lastActionTime = currentTime;
                return new BotInputs();
            }
        }
    }

    public class BotWalk : BotAction
    {
        public BotWalk(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotWalkDef;
            return new BotInputs()
            {
                AxisForward = defClass.AxisForward, 
                AxisLateral = defClass.AxisLateral,
            };
        }
    }

    public class BotWalkTowards : BotAction
    {
        public BotWalkTowards(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotWalkTowardsDef;
            return new BotInputs()
            {
                AxisForward = defClass.AxisForward, 
                AxisLateral = defClass.AxisLateral,
                CameraDirection = (Vector3)defClass.Direction,
            };
        }
    }

    public class BotWalkTo : BotAction
    {
        public BotWalkTo(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotWalkToDef;
            var direction = ((Vector3)defClass.Point - StateMachine._transform.position).normalized; 
            return new BotInputs()
            {
                AxisForward = defClass.AxisForward, 
                AxisLateral = defClass.AxisLateral,
                CameraDirection = direction,
            };
        }
    }
    
    public class BotWait : BotAction
    {
        private readonly DateTime _finishTime;

        public BotWait(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = Def as BotWaitDef;
            _finishTime = DateTime.Now + TimeSpan.FromSeconds(defClass.DurationSeconds);
        }

        protected override BotInputs GetInputsImpl()
        {
            if(DateTime.Now> _finishTime)
                State = ActionState.Complete;

            return default(BotInputs);
        }
    }

    public class BotJump : BotAction
    {
        private readonly float _startTime;

        public BotJump(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            _startTime = Time.time;
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotJumpDef;
            return  Time.time - _startTime <= 0.5f ? new BotInputs { AxisForward = defClass.AxisForward, AxisLateral = defClass.AxisLateral, Jump = true } : new BotInputs();
        }
    }

    public class BotDoAction : BotAction
    {
        public BotDoAction(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
        }

        protected override BotInputs GetInputsImpl()
        {
            var defClass = Def as BotDoActionDef;
            State = ActionState.Complete;
            return new BotInputs() { AxisForward = defClass.AxisForward, AxisLateral = defClass.AxisLateral, ActionName = defClass.ActionName };
        }
    }

    public class BotDumpStats : BotAction
    {
        public BotDumpStats(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var entity = basm._transform.GetComponent<EntityGameObject>();
            var id = entity.EntityId;
            Cluster.Cheats.ClusterCheats.DumpSpecificCharacterStats(id, true, true);
        }

        protected override BotInputs GetInputsImpl()
        {
            State = ActionState.Complete;
            return new BotInputs();
        }
    }

    public class BotDoSequence : BotAction
    {
        private readonly BotActionsStateMachine _sm;

        public BotDoSequence(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotDoSequenceDef;
            _sm = new BotActionsStateMachine(defClass.ActionsContainer, basm.EntityId, defClass.IsLooped, basm) { _transform = basm._transform };
        }

        protected override BotInputs GetInputsImpl()
        {
            if(_sm.Completed)
                State = ActionState.Complete;
            return _sm.GetInputs();
        }
    }

    public class BotDoRandom : BotAction
    {
        private readonly BotActionsStateMachine _sm;

        public BotDoRandom(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotDoRandomDef;
            var _actionDefList = new BotActionsSequenceDef();
            _actionDefList.Actions = new List<ResourcesSystem.Base.ResourceRef<BotActionDef>>();
            _actionDefList.Actions.Add(defClass.ActionsContainer.Actions[UnityEngine.Random.Range(0, defClass.ActionsContainer.Actions.Count)]);
            _sm = new BotActionsStateMachine(_actionDefList, basm.EntityId, defClass.IsLooped, basm) { _transform = basm._transform };
        }

        protected override BotInputs GetInputsImpl()
        {
            if (_sm.Completed)
                State = ActionState.Complete;
            return _sm.GetInputs();
        }
    }

    public class BotPing : BotAction
    {
        private readonly Task[] _pingTasks = new Task[6];

        public BotPing(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            var defClass = def as BotPingDef;

            IEntitiesRepository repo = GameState.Instance.ClientClusterNode;
            if (repo.AssertIfNull(nameof(repo)))
                return;

            var remoteRepoId = GameState.Instance.CharacterRuntimeData.CurrentPrimaryWorldSceneRepositoryId;
            _pingTasks[0] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    var result = await entity.PingRead();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingReadUnityRepositoryEntity");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > defClass.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }

                }
            }, repo);

            _pingTasks[1] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityClientFull>(remoteRepoId);
                    var result = await entity.PingWrite();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingWriteUnityRepositoryEntity");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > defClass.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }

                }
            }, repo);

            var characterId = basm.EntityId;
            if (Guid.Empty == characterId)
            {
                UI.Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return;
            }

            _pingTasks[2] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await entity.PingRead();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingReadCharacter");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > defClass.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }
                }
            }, repo);

            _pingTasks[3] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    var result = await entity.PingWrite();
                    if (result)
                    {
                        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("PingWriteCharacter");
                            sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                            if (duration > defClass.WarningTime)
                                Logger.IfWarn()?.Message(sb.ToString()).Write();
                            else
                                Logger.IfInfo()?.Message(sb.ToString()).Write();
                        }
                    }
                }
            }, repo);

            _pingTasks[4] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id))
                    {
                        var entity = wrapper2.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id);
                        var result = await entity.PingRead();
                        if (result)
                        {
                            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                            if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("PingReadWizard");
                                sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                                if (duration > defClass.WarningTime)
                                    Logger.IfWarn()?.Message(sb.ToString()).Write();
                                else
                                    Logger.IfInfo()?.Message(sb.ToString()).Write();
                            }
                        }
                    }
                }
            }, repo);

            _pingTasks[5] = AsyncUtilsUnity.RunAsyncTaskFromUnity(async () =>
            {
                var startTime = DateTime.UtcNow;
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
                {
                    var characterEntity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    using (var wrapper2 = await repo.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id))
                    {
                        var entity = wrapper2.Get<IWizardEntityClientFull>(characterEntity.Wizard.Id);
                        var result = await entity.PingWrite();
                        if (result)
                        {
                            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
                            if (duration > defClass.WarningTime || duration > defClass.InfoTime)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("PingWriteWizard");
                                sb.AppendFormat("ping: {0} msec", duration).AppendLine();
                                if (duration > defClass.WarningTime)
                                    Logger.IfWarn()?.Message(sb.ToString()).Write();
                                else
                                    Logger.IfInfo()?.Message(sb.ToString()).Write();
                            }
                        }
                    }
                }
            }, repo);
        }

        public override void LogAction(BotActionsStateMachine basm) { }

        protected override BotInputs GetInputsImpl()
        {
            // Uncomment this section to await ping result before proceeding to next action
            //bool tasksCompleted = true;
            //foreach (var pingTask in _pingTasks)
            //{
            //    if (!(pingTask == null || pingTask.IsCompleted || pingTask.IsFaulted || pingTask.IsCanceled))
            //    {
            //        tasksCompleted = false;
            //    }
            //}
            //if (tasksCompleted == true)
            //{
                State = ActionState.Complete;
            //}
            return new BotInputs();
        }
    }

    public class BotCommitSuicideByJumpingFromHeight : BotAction
    {
        private readonly long _liftingStartedTime;
        private readonly long _liftingTimeInMs;

        public BotCommitSuicideByJumpingFromHeight(BotActionDef def, BotActionsStateMachine basm) : base(def, basm)
        {
            _liftingStartedTime = global::ColonyShared.SharedCode.Utils.SyncTime.NowUnsynced;
            var defClass = def as BotCommitSuicideByJumpingFromHeightDef;
            _liftingTimeInMs = defClass.LiftingTime * 1000;
        }

        protected override BotInputs GetInputsImpl()
        {
            var rigidBody = StateMachine._transform.root.GetComponentInChildren<Rigidbody>();
            var defClass = Def as BotCommitSuicideByJumpingFromHeightDef;
            var currentTime = global::ColonyShared.SharedCode.Utils.SyncTime.NowUnsynced;
            if (_liftingStartedTime + _liftingTimeInMs > currentTime)
                rigidBody.velocity = Vector3.up * defClass.LiftingSpeed;
            else
                State = ActionState.Complete;
            return new BotInputs();
        }
    }
}
#endif