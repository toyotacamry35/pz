using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Shared;
using UnityEngine;
using Assets.Src.Aspects;
using Assets.Src.Character;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using Src.Animation;

using SharedPosRot = SharedCode.Entities.GameObjectEntities.PositionRotation;
using Assets.Src.AI.DamageIndication;
using Assets.Src.Aspects.Impl.Stats;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Character.Events;
using Assets.Src.Effects.Blood;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.Utils.Extensions;
using Src.Aspects.Impl.Stats;
using UnityEngine.Serialization;
using SharedCode.Serializers;

namespace Assets.Src.NetworkedMovement
{
    [RequireComponent(typeof(Animator))]
    public class MobView : MonoBehaviour, ISubjectView, IHitPointAdjuster
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [FormerlySerializedAs("Animator")] public Animator _animator;
        [SerializeField] private AnimationStateInfoStorage _animationInfo; 

        public Animator Animator => _animator;
        public IAnimationDoer AnimationDoer => _animationDoer;

        private AnimationDoer _animationDoer;
        private bool _hasIdleParameter;
        private const string IdleParameterName = "Idle";
        private StatResource _hpStatResource;
        private bool _attached;

        // --- Unity: -------------------------------------------

        public GameObject GameObject => gameObject;

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                _animator.enabled = true;
            }
        }
        
        private void Awake()
        {
            if(_animator == null)
                _animator = GetComponent<Animator>();
            _hpStatResource = GlobalConstsHolder.StatResources.HealthCurrentStat.Target;
        }
        private void Start()
        {
            _animator.cullingMode = AnimatorCullingMode.CullCompletely; //AlwaysAnimate
            _hasIdleParameter = _animator.parameters.Any(x => x.name == IdleParameterName);
        }

        private void Update()
        {
            _animationDoer?.Update();
        }

        private void OnDestroy()
        {
            if (_attached)
                Logger.IfError()?.Message($"View destroyed without detach!").Write();
//            SubscribeUnsubscribe(Aspects.SubscribeUnsubscribe.Unsubscribe);
        }

        // --- API: -------------------------------------------

        public void Attach(OuterRef entityRef, IEntitiesRepository entityRepo, VisualDollDef dollDef)
        {
            if (!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            _attached = true;
            _animationDoer = new AnimationDoer(_animator, _animationInfo, new AnimationModifiersFactory(), entityRef);
            _animationDoer?.SetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            SubscribeUnsubscribe(entityRef, Aspects.SubscribeUnsubscribe.Subscribe);
        }

        private void SubscribeUnsubscribe(OuterRef entityRef, SubscribeUnsubscribe instruction)
        {
         //   if (_pawn)
         //   {
                ///old:
                // // 1. +/-= Start-/End-AnimationEffect
                // switch (instruction)
                // {
                //     case Aspects.SubscribeUnsubscribe.Subscribe:
                //         _pawn.OnClientStartAnimationEffect += AnimationEffectStart;
                //         _pawn.OnClientEndAnimationEffect += AnimationEffectEnd;
                //         break;
                //     case Aspects.SubscribeUnsubscribe.Unsubscribe:
                //         _pawn.OnClientStartAnimationEffect -= AnimationEffectStart;
                //         _pawn.OnClientEndAnimationEffect -= AnimationEffectEnd;
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                // }

                // 2. +/-= PawnBecame-Far/-Near
//                if (ServerProvider.Server != null)
//                {
//                    switch (instruction)
//                    {
//                        case Aspects.SubscribeUnsubscribe.Subscribe:
//                            _pawn.PawnActivityLevelChanged += OnPawnActivityLevelChanged;
//                            break;
//                        case Aspects.SubscribeUnsubscribe.Unsubscribe:
//                            _pawn.PawnActivityLevelChanged -= OnPawnActivityLevelChanged;
//                            break;
//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
//                    }
//                }
         //   }

            // 3. +/-= OnDie:
            var repo = NodeAccessor.Repository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(entityRef.TypeId, entityRef.Guid))
                {
                    wrapper.TryGet<IHasMortalClientBroadcast>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast, out var mortalEntity);
                    if (mortalEntity != null)
                    {
                        switch (instruction)
                        {
                            case Aspects.SubscribeUnsubscribe.Subscribe: mortalEntity.Mortal.DieEvent += OnDie; break;
                            case Aspects.SubscribeUnsubscribe.Unsubscribe: mortalEntity.Mortal.DieEvent -= OnDie; break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                        }
                    }
                }
            }, repo);

            var hpIndicator = GetComponent<HpIndicationSwitchersController>();
            if (hpIndicator != null)
            {
                HpIndicationSwitchersController.SubscribeHpIndicatorController(_hpStatResource, entityRef.TypeId, entityRef.Guid, hpIndicator, repo, instruction);
            }
        }



        public void Detach(OuterRef entityRef, IEntitiesRepository entityRepo)
        {
            _attached = false;
            transform.parent = null;
            _animationDoer?.SetAnimationDoerToEntity(entityRef.To<IHasAnimationDoerOwnerClientBroadcast>(), entityRepo);
            SubscribeUnsubscribe(entityRef, Aspects.SubscribeUnsubscribe.Unsubscribe);
       //     OnPawnActivityLevelChanged(Pawn.ActivityLevel.Full); // While corpse don't provides similar events, we set as "near" & stay in this state
          //  _pawn = null;
        }

        private async Task OnDie(Guid entityId, int typeId, SharedPosRot corpsePlace)
        {
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                if (_animator)
                {
                    //  Animator.SetInteger("Death_Type", 1); //need input AnimationSubTypeValue
                    _animator.SetTrigger("Death");
                    _animator.SetBool("Death_bool", true); // Tmp-crutch: (PZ-9521) fast-fixing playing Idle sounds after death. Problem should be solved properly within task #PZ-10184.
                }
            });
        }

//        internal void AnimationEffectStart(MoveEffectDef currEffect)
//        {
//            if (!Animator)
//                return;
//
//            if (currEffect != null)
//            {
//                if (!string.IsNullOrEmpty(currEffect.AnimationSubType))
//                    Animator.SetInteger(currEffect.AnimationSubType, currEffect.AnimationSubTypeValue);
//                if (!string.IsNullOrEmpty(currEffect.AnimationBool))
//                    Animator.SetBool(currEffect.AnimationBool, true);
//            }
//            else
//            {
//                if (_hasIdleParameter)
//                    Animator.SetBool("Idle", true);
//            }
//        }

//        internal void AnimationEffectEnd(MoveEffectDef currEffect)
//        {
//            if (!Animator)
//                return;
//
//            if (currEffect != null)
//            {
//                if (!string.IsNullOrEmpty(currEffect.AnimationSubType))
//                {
//                    if (Animator.GetInteger(currEffect.AnimationSubType) == currEffect.AnimationSubTypeValue)
//                        Animator.SetInteger(currEffect.AnimationSubType, 0);
//                }
//                if (!string.IsNullOrEmpty(currEffect.AnimationBool))
//                    Animator.SetBool(currEffect.AnimationBool, false);
//            }
//            else
//            {
//                if (_hasIdleParameter)
//                    Animator.SetBool("Idle", false);
//            }
//        }


        public (Transform, Vector3) AdjustHitPoint(Vector3 worldPoint)
        {
            var hitObject = PlaceFXHelper.FindNearestGameObjectTransform(transform, worldPoint, t => t.GetComponent<FXBloodMarkerOnObj>()) ??
                            PlaceFXHelper.FindNearestGameObjectTransform(GetComponentInChildren<SkinnedMeshRenderer>()?.rootBone ?? transform, worldPoint);

            if (hitObject)
                return (hitObject, hitObject.position);

            return (transform, worldPoint);
        }

        public bool HasAuthority => false;
    }
}
