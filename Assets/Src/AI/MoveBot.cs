using Assets.Src.Aspects.Doings;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.Shared.Impl;
using ColonyShared.SharedCode.InputActions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;
using ILogger = NLog.ILogger;
using Vector2 = SharedCode.Utils.Vector2;
using Vector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class MoveBot : BehaviourNode
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private UnityEngine.Vector3? _targetPosition;
        private MoveBotDef _selfDef;
        private BotBrain _botBrain;
        private object _causer;
        private DateTime _startTime;

        private Legionary _targetAgent;
        private Vector3? _targetPoint;

        private static readonly ResourceRef<InputActionValueDef> MoveForward = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveForward");
        private static readonly ResourceRef<InputActionValueDef> MoveBackward = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveBackward");
        private static readonly ResourceRef<InputActionValueDef> MoveLeft = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveLeft");
        private static readonly ResourceRef<InputActionValueDef> MoveRight = new ResourceRef<InputActionValueDef>(@"/UtilPrefabs/Input/Actions/MoveRight");

        protected override ValueTask OnCreate(BehaviourNodeDef def)
        {
            _selfDef = def as MoveBotDef;
            return base.OnCreate(def);
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            _startTime = DateTime.Now;
            _causer = new object();
            var @ref = HostStrategy.CurrentLegionary.Ref;

            var targetExpr = (TargetSelector)await _selfDef.Target.ExprOptional(HostStrategy);
            if (targetExpr != null)
            {
                _targetAgent = await targetExpr.SelectTarget(HostStrategy.CurrentLegionary);
                _targetPoint = await targetExpr.SelectPoint(HostStrategy.CurrentLegionary);
            }
            if (_targetAgent != null && !_targetAgent.IsValid)
                _targetAgent = null;

            if (!_targetPoint.HasValue && _targetAgent != null)
                _targetPoint = HostStrategy.CurrentLegionary.GetPos(_targetAgent);

            if (_targetPoint.HasValue)
                _targetPosition = new UnityEngine.Vector3(_targetPoint.Value.x, _targetPoint.Value.y, _targetPoint.Value.z);

            await UnityQueueHelper.RunInUnityThread(() =>
            {
                var targetGo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(@ref);
                var componentns = targetGo.GetComponentsInChildren<Component>();
                _botBrain = targetGo.GetComponentInChildren<BotBrain>();
            });

            if (_botBrain != null)
            {
                _botBrain.BotActions.SetValue(MoveForward, 0);
                _botBrain.BotActions.SetValue(MoveBackward, 0);
                _botBrain.BotActions.SetValue(MoveLeft, 0);
                _botBrain.BotActions.SetValue(MoveRight, 0);
            }

            return ScriptResultType.Running;
        }

        protected override async ValueTask<ScriptResultType> OnTick()
        {
            try
            {
                var repo = HostStrategy.CurrentLegionary.Repository;
                var @ref = HostStrategy.CurrentLegionary.Ref;

                var nowTime = DateTime.Now;
                if (_selfDef.TimeoutSeconds > 0 && (nowTime - _startTime).TotalSeconds > _selfDef.TimeoutSeconds)
                    return ScriptResultType.Succeeded;

                var targetRef = _targetAgent?.Ref;
                if (targetRef?.IsValid ?? false)
                {
                    using (var wrapper = await repo.Get(targetRef.Value))
                    {
                        var position = PositionedObjectHelper.GetPosition(wrapper, targetRef.Value.TypeId, targetRef.Value.Guid);
                        if (position.HasValue)
                            _targetPosition = new UnityEngine.Vector3(position.Value.x, position.Value.y, position.Value.z);
                    }
                }

                if (_targetPosition.HasValue && _botBrain.BotActions != null)
                {
                    UnityEngine.Vector3 currentCoordinate = default;
                    UnityEngine.Vector3 botCameraDiraction = default;
                    await UnityQueueHelper.RunInUnityThread(() =>
                    {
                        currentCoordinate = _botBrain.BotActions.Transform.position;
                        botCameraDiraction = _botBrain.BotActions._camera;
                    });

                    if ((_targetPosition.Value - currentCoordinate).ToXZ().sqrMagnitude <= 1)
                        return ScriptResultType.Succeeded;

                    _botBrain.BotActions.CameraDirection = (_targetPosition.Value - currentCoordinate).normalized;
                    _botBrain.BotActions.SmoothTime = _selfDef.SmoothTime;

                    UnityEngine.Vector2 camFlatDirection = botCameraDiraction.ToXZ().normalized;
                    UnityEngine.Vector2 direction = (_targetPosition.Value - currentCoordinate).ToXZ().normalized;

                    var projModOnCamForwardDirection = UnityEngine.Vector2.Dot(direction, camFlatDirection);
                    var projModOnCamLateralDirection = UnityEngine.Vector2.Dot(direction, -UnityEngine.Vector2.Perpendicular(camFlatDirection));
                    Vector2 projOnCamBasis = new Vector2(projModOnCamForwardDirection, projModOnCamLateralDirection).normalized;

                    if (_botBrain != null)
                    {
                        _botBrain.BotActions.SetValue(MoveForward, UnityEngine.Mathf.Max(projOnCamBasis.x, 0));
                        _botBrain.BotActions.SetValue(MoveBackward, UnityEngine.Mathf.Max(-projOnCamBasis.x, 0));
                        _botBrain.BotActions.SetValue(MoveRight, UnityEngine.Mathf.Max(projOnCamBasis.y, 0));
                        _botBrain.BotActions.SetValue(MoveLeft, UnityEngine.Mathf.Max(-projOnCamBasis.y, 0));
                    }

                    return ScriptResultType.Running;
                }
            }
            catch
            {
                return ScriptResultType.Failed;
            }

            return ScriptResultType.Running;
        }

        public override ValueTask OnFinish()
        {
            if (_botBrain != null)
            {
                _botBrain.BotActions.SetValue(MoveForward, 0);
                _botBrain.BotActions.SetValue(MoveBackward, 0);
                _botBrain.BotActions.SetValue(MoveLeft, 0);
                _botBrain.BotActions.SetValue(MoveRight, 0);
            }
            return base.OnFinish();
        }

        public override ValueTask OnTerminate()
        {
            return base.OnTerminate();
        }
    }
}
