using System;
using System.Collections.Generic;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using SharedCode.Aspects.Item.Templates;
using Src.Aspects.Doings;
using UnityEngine.Serialization;
using Assets.Src.Character.Effects;
using SharedCode.Entities.GameObjectEntities;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Server.Impl;
using Assets.Src.Character.Events;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Utils.Extensions;
using Src.Animation;
using Src.Tools;
using Uins.Sound;
using static SharedCode.Aspects.Item.Templates.Constants;

namespace Assets.Src.Character
{
    public class ThirdPersonCharacterView : MonoBehaviour, ICharacterView, IHitPointAdjuster
    {
        private const string AttackTypeParameterPath = @"/UtilPrefabs/Res/AnimatorParameters/AttackType";
        private static readonly int Pitch = Animator.StringToHash(nameof(Pitch));

        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("ThirdPersonCharacterView");

        [FormerlySerializedAs("PlayerInfoPivot"), SerializeField] Transform _playerInfoPivot;
        [FormerlySerializedAs("Animator"), SerializeField] Animator _animator;
        [SerializeField] AnimationStateInfoStorage _animationInfo; 
        [SerializeField] AnimationTrajectoriesStorage _animationTrajectories;
        [SerializeField] VisualDoll _doll;
        [SerializeField] Transform _rootBone;
        [SerializeField] private BodyTwistIK _twistMotor;
        [SerializeField] private TurningWithStepping _turningMotor;
        public GameObject _fxAnimatorPrefab;
        public Animator FXAnimator { get; set; }
        public FXRuleUpdater _fxRuleUpdater;

        public bool BindBodyPitchWithCamera { get; set; }
        
        private AttackEventSubscriptionHandler[] _attackSubscriptionHandlers;
        private readonly List<Guid> _itemInHand = new List<Guid>();
        private AnimationDoer _animationDoer;
        private IAnimationModifiersFactory _animationModifiers;
        private AttackDoerSupport _attackDoerSupport;
        private AnimationParameterDef _attackTypeParameter;
        private bool _hasAuthority;
        private IGuideProvider _cameraGuide;
        private OuterRef _entityRef;
        private Coroutine _routine;
        private GameObject _resurrectionFX;
        private VisualDollDef _dollDef;

        public VisualDoll Doll => _doll;

        public VisualDollDef DollDef => _dollDef;
        
        public GameObject GameObject => gameObject;
        
        public Animator Animator => _animator;

        public bool HasAuthority => _hasAuthority;

        public IAnimationDoer AnimationDoer => _animationDoer;

        public BodyTwistIK TwistMotor => _twistMotor;

        public TurningWithStepping TurningMotor => _turningMotor;
        
        public AttackEventSubscriptionHandler[] AttackSubscriptionHandlers => _attackSubscriptionHandlers;

        public IAttackDoerSupport AttackDoerSupport => _attackDoerSupport;

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                Animator.enabled = value;
            }
        }

        public JdbMetadata FxEffects;

        private bool _controlBlocked;

        public void Attach(OuterRef entityRef, IEntitiesRepository entityRepo, VisualDollDef dollDef)
        {
            if (!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            _animator.AssertIfNull(nameof(_animator));
            _playerInfoPivot.AssertIfNull(nameof(_playerInfoPivot));
            
            _dollDef = dollDef;
            if (WorldConstants.UseMockCharacterView < MockView.NoAnimations)
            {
                _animationModifiers = new AnimationModifiersFactory();
            }
            else
            {
                _animationModifiers = new AnimationModifiersFactoryMock();
                _animator.enabled = false;
            }
            _animationDoer = new AnimationDoer(_animator, _animationInfo, _animationModifiers, entityRef);
            _animationDoer.SetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            _attackTypeParameter = GameResourcesHolder.Instance.LoadResource<AnimationParameterDef>(AttackTypeParameterPath);
        }

        public void SetMutationStage(MutationStageDef mutationStage)
        {
            _resurrectionFX = mutationStage.ResurrectionFX?.Target;
            if (_resurrectionFX == null)
                Logger.IfWarn()?.Message("No {0} set in {1}", nameof(_resurrectionFX), mutationStage).Write();
            //set player Wwise fraction switch
            var switchGroup = mutationStage.CharacterSoundStateGroup;
            var state = mutationStage.CharacterSoundState;
            var akGameObj = gameObject.GetComponentInChildren<AkGameObj>();
            if (state != null && switchGroup != null && akGameObj != null)
                AkSoundEngine.SetSwitch(AkSoundEngine.GetIDFromString(switchGroup), AkSoundEngine.GetIDFromString(state), akGameObj.gameObject);
        }

        public void SetGuideProvider(IGuideProvider cameraGuideProvider)
        {
            _cameraGuide = cameraGuideProvider; 
            _twistMotor?.SetGuideProvider(cameraGuideProvider);
        }

        public void InitClientAuthority(OuterRef entityRef)
        {
            if (!_fxAnimatorPrefab)
                Logger.IfWarn()?.Message("No '{0}' set in '{1}'", nameof(_fxAnimatorPrefab), nameof(ThirdPersonCharacterView)).Write();
            else
            {
                var fxGameObject = Instantiate(_fxAnimatorPrefab, transform);
                var animatorRef = fxGameObject.GetComponent<FXAnimatorReference>();
                if (animatorRef)
                    FXAnimator = animatorRef.Animator;
                else
                    Logger.IfWarn()?.Message("No component '{0}' on '{1}' prefab", nameof(FXAnimatorReference), fxGameObject.name).Write();
            }
            _hasAuthority = true;

            _fxRuleUpdater?.Dispose();
            _fxRuleUpdater = new FXRuleUpdater(GatherFxRules(), entityRef.To<IEntityObject>(), 0.33f);
            _fxRuleUpdater.Run();

            _attackDoerSupport = new AttackDoerSupport(_animator, _animationTrajectories, _animationDoer);
            _attackSubscriptionHandlers = GetComponentsInChildren<AttackEventSubscriptionHandler>();

            var akGameObj = gameObject.GetComponentInChildren<AkGameObj>();
            if (akGameObj != null)
            {
                AkSoundEngine.SetRTPCValue("player_authority", 1, akGameObj.gameObject);
                SoundControl.Instance?.OnResurrect(akGameObj);
            }
            
            foreach(var lodGroup in GetComponentsInChildren<LODGroup>())
                lodGroup.ForceLOD(0);
        }

        private void Update()
        {
            _animationDoer?.Update();
            _fxRuleUpdater?.UpdateAnimators(FXAnimator, Animator);
            UpdatePitch();
        }

        private void UpdatePitch()
        {
            if (BindBodyPitchWithCamera)
            {
                var pitch = _cameraGuide != null ? Mathf.PI / 2 - Mathf.Acos(_cameraGuide.Guide.y) : 0;
                _animator.SetFloat(Pitch, Mathf.DeltaAngle(0, pitch * Mathf.Rad2Deg));
            }
        }
        
        public void OnPutItemInHand((BaseItemResource, Guid) item)
        {
            if (item.Item1 != null && !_itemInHand.Contains(item.Item2))
            {
                _itemInHand.Add(item.Item2);
                var itemResource = item.Item1 as ItemResource;
                if (itemResource?.CharacterAnimatorModifiers != null)
                {
                    object causer = item.Item2;
                    var modifiers = new List<IAnimationModifier>();
                    foreach (var def in itemResource.CharacterAnimatorModifiers)
                    {
                        _animationDoer.ModifiersFactory.Create(def, modifiers);
                        foreach (var mod in modifiers)
                            _animationDoer.Push(causer, mod);
                    }
                }   
            }
        }

        public void OnRemoveItemFromHand((BaseItemResource, Guid) item)
        {
            if (item.Item1 != null && _itemInHand.Remove(item.Item2))
            {
                var itemResource = item.Item1 as ItemResource;
                if (itemResource?.CharacterAnimatorModifiers != null)
                {
                    object causer = item.Item2;
                    var modifiers = new List<IAnimationModifier>();
                    foreach (var def in itemResource.CharacterAnimatorModifiers)
                    {
                        _animationDoer.ModifiersFactory.Create(def, modifiers);
                        foreach (var mod in modifiers)
                            _animationDoer.Pop(causer, mod);
                    }
                }
            }
        }

        public void Detach(OuterRef entityRef, IEntitiesRepository entityRepo)
        {
            transform.parent = null;
            _animationDoer.UnsetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            _attackDoerSupport?.Dispose();
            _attackDoerSupport = null;
            _fxRuleUpdater?.Dispose();
            _fxRuleUpdater = null;
        }

        public void InitClient(OuterRef entityRef)
        {
            if (_hasAuthority)
                return;

            _fxRuleUpdater = new FXRuleUpdater(GatherFxRules(), entityRef.To<IEntityObject>(), 5, onlyMainAnimator: true);
            _fxRuleUpdater.Run();
        }

        private IEnumerable<FXRuleDef> GatherFxRules()
        {
            var fxDefs = FxEffects.Get<FXs>();
            if (fxDefs == null)
            {
                Debug.LogError($"FxEffects not set on {gameObject.name} in {name}");
                return Enumerable.Empty<FXRuleDef>();
            }
            return fxDefs.Rules;
        }


        public (Transform, Vector3) AdjustHitPoint(Vector3 worldPoint)
        {
            var hitObject = _doll.GetNearestBone(worldPoint);
            return (hitObject, hitObject ? hitObject.position : transform.position);
        }
    }
}