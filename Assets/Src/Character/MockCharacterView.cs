using System;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.Character.Events;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils.Extensions;
using Src.Animation;
using Src.Aspects.Doings;
using Src.Tools;
using Uins.Sound;
using UnityEngine;

namespace Assets.Src.Character
{
    public class MockCharacterView : MonoBehaviour, ICharacterView
    {
        [SerializeField] Animator _animator;
        [SerializeField] AnimationStateInfoStorage _animationInfo; 
        [SerializeField] AnimationTrajectoriesStorage _animationTrajectories;
        [SerializeField] VisualDoll _doll;

        private AnimationDoer _animationDoer;
        private AttackDoerSupport _attackDoerSupport;
        private AttackEventSubscriptionHandler[] _attackSubscriptionHandlers;
        private OuterRef _entityRef;
        private VisualDollDef _dollDef;

        public VisualDoll Doll => _doll;
        public VisualDollDef DollDef => _dollDef;
        public Animator Animator => _animator;
        public GameObject GameObject => gameObject;
        public bool Enabled { get; set; }
        public IAnimationDoer AnimationDoer => _animationDoer;
        public BodyTwistIK TwistMotor => null;
        public TurningWithStepping TurningMotor => null;
        public AttackEventSubscriptionHandler[] AttackSubscriptionHandlers => _attackSubscriptionHandlers;
        public IAttackDoerSupport AttackDoerSupport => _attackDoerSupport;
   //     public MutationStageDef MutationStage { get; private set; }
        public bool BindBodyPitchWithCamera { get; set; }
        public bool HasAuthority { get; private set; }
        
        public void Detach(OuterRef entityRef, IEntitiesRepository entityRepo)
        {
            transform.parent = null;
            _animationDoer.UnsetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            _attackDoerSupport?.Dispose();
        }

        public void SetGuideProvider(IGuideProvider cameraGuideProvider) {}

        public void InitClientAuthority(OuterRef entityRef)
        {
            HasAuthority = true;
        }

        public void Attach(OuterRef entityRef, IEntitiesRepository entityRepo, VisualDollDef dollDef)
        {
            _animator.AssertIfNull(nameof(_animator));
            _dollDef = dollDef;
            _animationDoer = new AnimationDoer(_animator, _animationInfo, new AnimationModifiersFactoryMock(), _entityRef);
            _animationDoer.SetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            _attackDoerSupport = new AttackDoerSupport(_animator, _animationTrajectories, _animationDoer);
            _attackSubscriptionHandlers = GetComponentsInChildren<AttackEventSubscriptionHandler>();
            _animator.gameObject.SetActive(false);
     //       MutationStage = mutationStage;
            SoundControl.Instance?.OnResurrect(gameObject.GetComponentInChildren<AkGameObj>(true));
        }

        public void SetMutationStage(MutationStageDef mutationStage) {}
        
        public void InitClient(OuterRef entityRef) {}
        
        public void OnPutItemInHand((BaseItemResource, Guid) item) {}

        public void OnRemoveItemFromHand((BaseItemResource, Guid) item) {}

        private void Update()
        {
            _animationDoer?.Update();
        }
    }
}