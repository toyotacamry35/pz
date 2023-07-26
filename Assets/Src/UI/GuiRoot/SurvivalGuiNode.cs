using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.Src.Aspects;
using Assets.Src.ContainerApis;
using Assets.Src.Lib.Cheats;
using Assets.Src.SpawnSystem;
using ColonyDI;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Science;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Uins
{
    /// <summary>
    /// Управление GUI выживания
    /// </summary>
    public class SurvivalGuiNode : DependencyEndNode, IPawnSource
    {
        public delegate void PawnDelegate(ISubjectPawn pawn);

        public event PawnDelegate PawnSpawned;
        public event PawnDelegate PawnDestroyed;

        private EntityApiWrapper<EntityGuidApi> _entityGuidApiWrapper;


        //=== Props ===============================================================

        [Dependency]
        private GameState GameState { get; set; }

        public static SurvivalGuiNode Instance { get; private set; }

        public GameObject OurUserPawn { get; private set; }

        private ReactiveProperty<(EntityGameObject, EntityGameObject)> _pawnChangesStream =
            new ReactiveProperty<(EntityGameObject, EntityGameObject)>();

        /// <summary>
        /// Появился или сменился Pawn на User. В т.ч., когда сменился из-за смены User
        /// </summary>
        public IStream<(EntityGameObject, EntityGameObject)> PawnChangesStream => _pawnChangesStream;

        private TouchableEgoProxy<IWorldCharacterClientFull> _touchableEntityProxy =
            new TouchableEgoProxy<IWorldCharacterClientFull>(UnityEntityTouchContainerFactory<IWorldCharacterClientFull>.Instance);

        public ITouchable<IWorldCharacterClientFull> TouchableEntityProxy => _touchableEntityProxy;

        private ReactiveProperty<int> _accountLevelStreamRp = new ReactiveProperty<int>() {Value = 0};

        public IStream<int> AccountLevelStream => _accountLevelStreamRp;

        public ReactiveProperty<GenderDef> GenderRp { get; } = new ReactiveProperty<GenderDef>();

        public Guid PlayerGuid { get; private set; }

        public IStream<ListStream<TechnologyDef>> KnownTechnologiesStream { get; private set; }

        public ListStream<FriendInfo> FriendList { get; } = new ListStream<FriendInfo>();


        //=== Unity ===============================================================

        private void Awake()
        {
            var technologiesStreamPlug = new TechnologiesStreamPlug();
            KnownTechnologiesStream = technologiesStreamPlug.GetKnownTechnologiesStream(_pawnChangesStream);
            var accountLevelStream = _touchableEntityProxy
                .Child(D, character => character.AccountStats)
                .ToStream(D, accountStatsClientFull => accountStatsClientFull.AccountExperience)
                .Func(D, LevelUpDatasHelpers.CalcAccLevel);
            accountLevelStream.Bind(D, _accountLevelStreamRp);

            _touchableEntityProxy
                .ToStream(D, character => character.Gender)
                .Bind(D, GenderRp);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _pawnChangesStream.Dispose();
            _touchableEntityProxy.Dispose();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        [Cheat]
        public static void DebugAccLevel(int level = 1) //TODO убрать после тестов
        {
            if (Instance.AssertIfNull(nameof(Instance)))
                return;

            Instance._accountLevelStreamRp.Value = level;
            UI.CallerLog($"Account level is {Instance._accountLevelStreamRp.Value}");
        }

        public void OnOurPawnChanged(ISubjectPawn prevPawn, ISubjectPawn newPawn)
        {
            OurUserPawn = newPawn?.Ego.gameObject;

            try
            {
                if (newPawn != null && OurUserPawn != null)
                {
                    OurUserPawn.GetOrAddComponent<NewPlacesNotifier>();
                    _entityGuidApiWrapper = EntityApi.GetWrapper<EntityGuidApi>(newPawn.Ego.OuterRef);
                    _entityGuidApiWrapper.EntityApi.SubscribeOnEntityGuid(GetPlayerGuid);
                }
                else
                {
                    _entityGuidApiWrapper.Dispose();
                    _entityGuidApiWrapper = null;
                }

                _pawnChangesStream.Value = (
                    prevPawn?.Ego,
                    newPawn?.Ego
                );

                if (prevPawn != null)
                {
                    _touchableEntityProxy.Disconnect();
                }

                if (newPawn != null)
                {
                    _touchableEntityProxy.Connect(
                        newPawn.Repository,
                        newPawn.EntityRef.TypeId,
                        newPawn.EntityRef.Guid,
                        ReplicationLevel.ClientFull);
                }

                UI.CallerLog($"PAWN: {(OurUserPawn != null ? OurUserPawn.transform.FullName() : "null")}"); //DEBUG
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Exception(e).Write();
            }
        }

        public void OnPawnSpawn(ISubjectPawn pawn)
        {
            PawnSpawned?.Invoke(pawn);
        }

        public void OnPawnDestroy(ISubjectPawn pawn)
        {
            PawnDestroyed?.Invoke(pawn);
        }


        //=== Protected ===========================================================

        public override void AfterDependenciesInjected()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            GameState.AssertIfNull(nameof(GameState));
            GameState.IsInGameRp.Action(D, WindowsManager.OnIsInGameChanged);
            
            var friendsVM = GameState.IsInGameRp
                .Transform(D, isInGame => isInGame ? new InGameFriendsVM() : null);
            var friendInfoList = friendsVM.SubListStream(D, vm => vm.Friends);
            friendInfoList.CopyTo(D, FriendList);
        }

        //=== Private =============================================================

        private void GetPlayerGuid(Guid entityGuid)
        {
            PlayerGuid = entityGuid;
        }
    }
}