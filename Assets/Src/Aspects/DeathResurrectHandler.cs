using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.Src.Aspects.Impl;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.EntitySystem;
using UnityEngine;
using static SharedCode.Wizardry.UnityEnvironmentMark;

using SharedPosRot = SharedCode.Entities.GameObjectEntities.PositionRotation;
using SharedCode.Entities;
using GeneratedCode.Repositories;
using Uins.Cursor;
using SharedCode.Serializers;

namespace Assets.Src.Aspects
{
    [DisallowMultipleComponent]
    public class DeathResurrectHandler : EntityGameObjectComponent
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("DeathResurrectHandler");

        [UsedImplicitly] private string _playAnimation;
        [UsedImplicitly] private float _destroyDelay;

        IViewCreatorDetacher _charViewBack;
        private IViewCreatorDetacher _charClView => _charViewBack == null ? _charViewBack = GetComponent<IViewCreatorDetacher>() : _charViewBack;

        //=== Protected =======================================================

     

        protected override void GotServer() => SubscribeUnsubscribeMortalServer(SubscribeUnsubscribe.Subscribe);
        protected override void GotClient()
        {
            // Register in dic. on awake (i.e. sync-ly) to avoid need of concurrent dic. instead of plain
            CorpseViewProvider.RegisterMortal(GetOuterRef<IEntity>());
            SubscribeUnsubscribeMortalClient(SubscribeUnsubscribe.Subscribe);
        }
        protected override void LostServer() => SubscribeUnsubscribeMortalServer(SubscribeUnsubscribe.Unsubscribe);
        protected override void LostClient() => SubscribeUnsubscribeMortalClient(SubscribeUnsubscribe.Unsubscribe);

        //=== Private =========================================================

        private void SubscribeUnsubscribeMortalServer(SubscribeUnsubscribe instruction)
        {
            var repo = ServerRepo;
            if (repo == null)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(TypeId, EntityId))
                {
                    wrapper.TryGet<IHasMortalAlways>(TypeId, EntityId, ReplicationLevel.Always, out var mortal);
                    if (mortal != null)
                    {
                        switch (instruction)
                        {
                            case SubscribeUnsubscribe.Subscribe:
                                mortal.Mortal.DieEvent += OnDieServer;
                                mortal.Mortal.ResurrectEvent += OnResurrectServer;
                                break;
                            case SubscribeUnsubscribe.Unsubscribe:
                                mortal.Mortal.DieEvent -= OnDieServer;
                                mortal.Mortal.ResurrectEvent -= OnResurrectServer;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                        }

                        if (instruction == SubscribeUnsubscribe.Subscribe)
                        {
                            // If we missed 1st `ResurrectEvent` - call reaction manually:
                            if (await mortal.Mortal.GetIsAlive())
                            {
                                await UnityQueueHelper.RunInUnityThread(() =>
                                {
                                    (_charClView as EntityGameObjectComponent)?.ForceStatusChanged();
                                });
                                    
                                var at = SharedPosRot.InvalidInstatnce;
                                var posdObj = PositionedObjectHelper.GetPositioned(wrapper, TypeId, EntityId);
                                if (posdObj != null)
                                {
                                    var trans = posdObj.Transform;
                                    at = new SharedPosRot(trans.Position, trans.Rotation);
                                }

                                await OnResurrectServer(EntityId, TypeId, at);
                            }
                        }
                    }
                }
            });
        }

        private void SubscribeUnsubscribeMortalClient(SubscribeUnsubscribe instruction)
        {
            var repo = ClientRepo;
            if (repo == null)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(Ego.TypeId, Ego.EntityId))
                {
                    if (wrapper.TryGet<IHasMortalClientBroadcast>(Ego.TypeId, Ego.EntityId, ReplicationLevel.ClientBroadcast, out var mortal))
                        if (mortal != null)
                        {
                            switch (instruction)
                            {
                                case SubscribeUnsubscribe.Subscribe:
                                    mortal.Mortal.DieEvent += OnDieClient;
                                    mortal.Mortal.ResurrectEvent += OnResurrectClient;
                                    break;
                                case SubscribeUnsubscribe.Unsubscribe:
                                    mortal.Mortal.DieEvent -= OnDieClient;
                                    mortal.Mortal.ResurrectEvent -= OnResurrectClient;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                            }

                            // If we missed 1st `ResurrectEvent` - call reaction manually:
                            if (await mortal.Mortal.GetIsAlive())
                            {

                                await UnityQueueHelper.RunInUnityThread(() =>
                                {
                                    (_charClView as EntityGameObjectComponent)?.ForceStatusChanged();

                                });
                                var at = SharedPosRot.InvalidInstatnce;
                                var posdObj = PositionedObjectHelper.GetPositioned(wrapper, TypeId, EntityId);
                                if (posdObj != null)
                                {
                                    var trans = posdObj.Transform;
                                    at = new SharedPosRot(trans.Position, trans.Rotation);
                                }

                                await OnResurrectClient(EntityId, TypeId, at);
                            }
                        }
                }
            });
        }

        private Task OnDieServer(Guid id, int typeId, SharedPosRot pose)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [DeBoDe].OnDieServer. {id}").Write();

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (!this)
                    return;
                transform.position = (Vector3)pose.Position;
                transform.rotation = (Quaternion)pose.Rotation;
                _charClView?.OnDieU(ServerOrClient.Server);
                foreach (var l in gameObject.GetComponents(typeof(IDeathResurrectListenerComponent)))
                    ((IDeathResurrectListenerComponent)l).OnDeath(ServerOrClient.Server);
                //gameObject.SetActive(false);
            });

            return Task.CompletedTask;
        }

        private Task OnDieClient(Guid guid, int type, SharedPosRot pose)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"<Dead>  [DeBoDe].OnDieClient. {guid}").Write();
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                transform.position = (Vector3)pose.Position;
                transform.rotation = (Quaternion)pose.Rotation;
                //#order 1: `DetachView` is called inside
                OnDieClientU();
                //#order 2: `DestroyView` is called inside
                _charClView?.OnDieU(ServerOrClient.Client);
                foreach (var l in gameObject.GetComponents(typeof(IDeathResurrectListenerComponent)))
                    ((IDeathResurrectListenerComponent)l).OnDeath(ServerOrClient.Client);
                //gameObject.SetActive(false);
            });
            return Task.CompletedTask;
        }

        private Task OnResurrectServer(Guid id, int typeId, SharedPosRot at)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW==- DBDtch.OnResurrectServer.  at: {at.Position}, ?:{at.IsValid}.").Write();

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (this == null)
                {
                    Logger.IfWarn()?.Message($"#Note: -==VIEW==- DBDtch.OnResurrectServer <{id}>. this==null. (Probably g.o. was deleted during async operations)").Write();
                    return;
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW==- DBDtch.OnResurrectServer <{id}>").Write();

                var pose = PositionRotation.FromShared(at);
                _charClView?.OnResurrectU(ServerOrClient.Server, pose);
                foreach (var l in gameObject.GetComponents(typeof(IDeathResurrectListenerComponent)))
                    ((IDeathResurrectListenerComponent)l).OnResurrect(ServerOrClient.Server, pose);
            });

            return Task.CompletedTask;
        }

        private Task OnResurrectClient(Guid guid, int type, SharedPosRot at)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW==- DBDtch.OnResurrectClient.  at: {at.Position}, ?:{at.IsValid}.").Write();

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (this == null)
                {
                    Logger.IfWarn()?.Message($"#Note: -==VIEW==- DBDtch.OnResurrectClient <{guid}>. this==null. (Probably g.o. was deleted during async operations)").Write();
                    return;
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"(pz5094)  -==VIEW==- DBDtch.OnResurrectClient gameObject.SetActive(true) <{guid}>").Write();

                var pose = PositionRotation.FromShared(at);
                _charClView?.OnResurrectU(ServerOrClient.Client, pose);  //2
                foreach (var l in gameObject.GetComponents(typeof(IDeathResurrectListenerComponent)))
                    ((IDeathResurrectListenerComponent)l).OnResurrect(ServerOrClient.Client, pose);  //3
            });

            return Task.CompletedTask;
        }

        private void OnDieClientU()
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("(0) --DOLL-- DBDtch.OnDieClientU.").Write();;

            var rootObject = transform.root;

            var visualSlots = rootObject.GetComponentsInChildren<VisualSlot>();
            if (visualSlots != null)
                foreach (var visualSlot in visualSlots)
                    visualSlot.Unsubscribe();
            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW  0.-2 ==- DBDtch.OnDieClientUnity(-2).").Write();

            //#order! #1
            if (_charClView is IEntityPawn pawn && pawn.View.Value is IAnimatedView view)
            {
                var animator = view.Animator;
                animator.AssertIfNull(nameof(animator));
                if (_playAnimation != null && animator != null)
                {  // FIXME: это, вообще, нормально делать подобное здесь?
                    animator.SetTrigger(_playAnimation);
                    animator.SetBool("Death_bool", true); // Tmp-crutch: (PZ-9521) fast-fixing playing Idle sounds after death. Problem should be solved properly within task #PZ-10184.
                }
            }
            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW  0.-1 ==- DBDtch.OnDieClientUnity(-1).").Write();

            //#order! #2
            var viewGo = _charClView.DetachView();

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($" -==VIEW  0 ==- DBDtch.OnDieClientUnity(0). ViewGo: {viewGo}").Write();
            if (viewGo)
                CorpseViewProvider.EnqueAbandonedViewGo(GetOuterRef<IEntity>(), viewGo);
        }
    }

    public enum SubscribeUnsubscribe : byte
    {
        Subscribe,
        Unsubscribe
    }
}
