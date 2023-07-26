using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl;
using Assets.Src.Camera;
using Core.Environment.Logging.Extension;
using UnityEngine;
using JetBrains.Annotations;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Aspects.Doings;
using Src.Locomotion.Unity;
using TimeUnits = System.Int64;
using SVector2 = SharedCode.Utils.Vector2;
using SVector3 = SharedCode.Utils.Vector3;


namespace Assets.Src.Aspects.Doings
{
    internal class BotBrain : CharacterBrain, IDisposableCharacterBrain
    {
        private const string DefaultBot = "/Bots/SimpleBot";

        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Brain.Character");

        public BotActionsStatesMachine2 BotActions;
        private CameraGuideController _cameraGuideController;
        private Vector3 _guide;
        private float _smoothTime;
//        private Dictionary<BotInputAction, BotSpellActivator> _triggers = new Dictionary<BotInputAction, BotSpellActivator>();
        private bool _freeCamera;
        private float _stuckTimer;
        private float _stuckThreshold;
        private PositionRotation _stuckPose;


        public event Action OnFrameStart;

        public event Action OnFrameEnd;

        public GameObject GameObject => (this != null) ? gameObject : null;

        public bool IsBot => true;

        internal async Task<BotBrain> Init(
            ISpellDoer doer,
            OuterRef entityRef,
            IEntitiesRepository repository,
            TargetHolder targetHolder,
            CameraGuideController.ISettings guideControllerSettingsProvider,
            BotActionDef botDef)
        {
            UnityQueueHelper.AssertInUnityThread();
            
            ClientBotRespawner.Register(entityRef.Guid);
            _freeCamera = false;
            BotActions = new BotActionsStatesMachine2(botDef, doer, entityRef, repository);
            _guide = transform.forward;

            BotActions.Transform = transform;
            BotActions._camera = _guide;

            _stuckThreshold = botDef.StuckTime; 
                
            _cameraGuideController = new CameraGuideController(guideControllerSettingsProvider);
            await base.Init(
                entityRef: entityRef,
                repository: repository, 
                spellDoer: doer, 
                targetHolder: targetHolder, 
                cameraController: _cameraGuideController, 
                inputProvider: BotActions);
            return this;
        }

        private void Update()
        {
            if (_freeCamera)
            {
                UpdateCamera();
            }
            else
            {
                var dummy = Vector3.zero;
                if (_smoothTime > 0)
                    _cameraGuideController.Guide = Vector3.SmoothDamp(_cameraGuideController.Guide, _guide, ref dummy, _smoothTime);
                else
                    _cameraGuideController.Guide = _guide;
            }

            if (BotActions == default(BotActionsStatesMachine2))
                return;

            CheckStuck();
            
            BotActions.Transform = transform;
            BotActions._camera = _guide;
            var newCameraDirection = BotActions.CameraDirection;
            if (newCameraDirection.HasValue)
            {
                _guide = newCameraDirection.Value;
                _smoothTime = BotActions.SmoothTime;
            }
        }

        private void CheckStuck()
        {
            if (_stuckThreshold <= 0)
                return;
            
            var pose = new PositionRotation(transform.position, transform.rotation);
            if (pose.Position.ApproximatelyEqual(_stuckPose.Position, 0.001f) && pose.Rotation.ApproximatelyEqual(_stuckPose.Rotation, 0.001f))
            {
                var stuckTimer = _stuckTimer + Time.unscaledDeltaTime;
                if (_stuckTimer < _stuckThreshold && stuckTimer >= _stuckThreshold)
                    Logger.IfError()
                        ?.Message(
                            "{0} is stuck at {2} with actions: [{1}]",
                            BotActions,
                            string.Join(" ", BotActions.CurrentActionsStack.Select(x => $"{x.GetType().Name}:{x.____GetDebugShortName()}")),
                            pose.Position)
                        .Write();
                _stuckTimer = stuckTimer;
            }
            else
                _stuckTimer = 0;
            
            _stuckPose = pose;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            BotActions?.Dispose();
        }
    }

}
