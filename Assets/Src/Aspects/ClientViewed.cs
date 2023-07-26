using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Character.Events;
using Assets.Src.NetworkedMovement;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Animation;
using Src.Locomotion;
using Src.Locomotion.Unity;
using Src.NetworkedMovement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Assets.Src.Aspects
{
    [Obsolete("Вместо ClientViewed надо использовать аналогичные поля в Pawn")]
    public class ClientViewed : EntityGameObjectComponent
    {
        [FormerlySerializedAs("View"), SerializeField] public GameObject _viewPrefab;
        [SerializeField, Tooltip("model pivot manually set offset to nicely match g.o. pivot"), FormerlySerializedAs("Offset")] public Vector3 _offset;
    }
}

namespace Assets.Src.NetworkedMovement
{
    // По идее, это нужно объединить с Pawn и назвать MobPawn  
    public partial class Pawn : IMobPawn
    {
        [FormerlySerializedAs("View"), SerializeField] private GameObject _viewPrefab;
        [SerializeField, Tooltip("model pivot manually set offset to nicely match g.o. pivot"), FormerlySerializedAs("Offset")] private Vector3 _offset;
        
        private readonly PooledReactiveProperty<ISubjectView> _view = PooledReactiveProperty<ISubjectView>.Create().InitialValue(null);
        private readonly PooledReactiveProperty<IsVisibleByCameraDetector> _isVisibleByCamera = PooledReactiveProperty<IsVisibleByCameraDetector>.Create();

//        public Animator Animator => _view.Value.Animator;

        IReactiveProperty<ISubjectView> ISubjectPawn.View => _view;
        
        public bool Alive { get; private set; }
        IReactiveProperty<IEntityView> IEntityPawn.View => _view;
        public OuterRef EntityRef => Ego.OuterRef;
        public IEntitiesRepository Repository => Ego.ClientRepo;

        EntityGameObject IEntityPawn.Ego => Ego;

        // --- Overrides: ----------------------------------------------------------------

        private void DestroyView()
        {
            Logger.IfDebug()?.Message($"UI.[ClVi-ed].DestroyView <{EntityId}> {gameObject.name}").Write();
            var view = _view.Value;
            _view.Value = null;
            if (view != null)
            {
                ForgetView(view);
                view.Detach(OuterRef, Repository);
                if (view.GameObject != null)
                    DestroyImmediate(view.GameObject);
            }
        }

        // --- API: ----------------------------------------------------------------

        private ISubjectView InstantiateAndInitViewIfShould()
        {
            if (_view.Value != null)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"[ClVi-ed].InstantiateAndInitViewIfShould `{nameof(_view.Value)}` != null. So do nothing.").Write();
                return _view.Value;
            }

            if (_viewPrefab.AssertIfNull(nameof(_viewPrefab)))
                return null;

            var instance = Instantiate(_viewPrefab, transform.position + _offset, Quaternion.identity, transform);
            if (instance == null)
                throw new Exception($"Can't create view instance for {name}! Prefab:{_viewPrefab}");
            var view = instance.GetComponent<ISubjectView>();
            if (view == null)
                throw new Exception($"View instance {_viewPrefab} has no {nameof(ISubjectView)}");
            view.Attach(OuterRef, Repository, null);
            view.GameObject.transform.localRotation = Quaternion.identity;
            _view.Value = view;
            
            GetComponent<VisualEventProxy>()?.SetRoot(gameObject); 

            return view;
        }

        public GameObject DetachView()
        {
            var view = _view.Value;
            _view.Value = null;
            var viewGo = view.GameObject;
            ForgetView(view);
            view.AnimationDoer?.UnsetAnimationDoerToEntity(Ego.OuterRef.To<IHasAnimationDoerOwnerClientBroadcast>(), ClientRepo);
            view.Detach(OuterRef, Repository);
            GetComponent<VisualEventProxy>()?.SetRoot(viewGo); // FX'ы должны продолжать играться на оторванном view 
            return viewGo;
        }

        public void OnResurrectU(UnityEnvironmentMark.ServerOrClient context, PositionRotation pose)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "OnResurrectU({0}, {1})",  context, pose.Position).Write();
//            transform.position = pose.Position;
//            transform.rotation = pose.Rotation;
    
            Alive = true;

            switch (context)
            {
                case UnityEnvironmentMark.ServerOrClient.Server:
                {
                    OnResurrectServerU(pose);
                    break;
                }
                case UnityEnvironmentMark.ServerOrClient.Client:
                {
                    OnResurrectClientU(pose);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
                }
            }
        }
        
        public void OnDieU(UnityEnvironmentMark.ServerOrClient context)
        {
            Alive = false;
            
            switch (context)
            {
                case UnityEnvironmentMark.ServerOrClient.Server:
                {
                    OnDieServerU();
                    break;
                }
                case UnityEnvironmentMark.ServerOrClient.Client:
                {
                    OnDieClientU();
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(context), context, null);
                }
            }
        }

        public void OnResurrectClientU(PositionRotation pose)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "InitClient").Write();
            Assert.IsTrue(IsClient);
            
            var view = InstantiateAndInitViewIfShould();
            if (view != null) // It's safe-check to avoid farther errors if somebody forget to pass view-prefab (f.e.)
                OnResurrectClientU(view, pose);
            else
                Logger.IfError()?.Message($"[ClVie-d] OnResurrectClientU. view==null. \"{gameObject.name}\" <--<--<<-- <--<--<<-- <--<--<<-- <--<--<<-- <--<--<<--").UnityObj(gameObject).Write();

            //#todo: Now it 'll be fire when mob became visible (not only on namely "resurrect"). Should be fixed later.
            view.AnimationDoer.Push(this, view.AnimationDoer.ModifiersFactory.Trigger(GlobalConstsHolder.GlobalConstsDef.AnimationParameters.Target.Resurrect));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _view.Dispose();
            _isVisibleByCamera.Dispose();
        }
    }
}