using System;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using UnityEngine;

namespace Uins
{
    /// <summary>
    /// Задача: Cre. TouchableEgoProxy на entty. Используя его, превращает:
    ///     а) async д-е в синхронные;
    ///     б) д-е модели в представление удобное д/отображения (НО пока без специфики Unity).
    /// </summary>
    public class AccountGuiWindowVM : IHasDisposableComposite, IIsDisposed //#no_need: : BindingVmodel
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly GameState _gameState;
        private readonly TouchableEgoProxy<IAccountEntityClientFull> _touchableAcc        = TouchableUtils.CreateEgoProxy<IAccountEntityClientFull>();
        
        public StartGameGuiNode StartGameNode { get; }
        
        public IStream<(int, int)> AnyExpChangedStream;
        public IStream<GenderDef> GenderStream;
        public IStream<int> GenderIntStream0Based;

        public ReactiveProperty<string> AccName  = new ReactiveProperty<string>();
        public ReactiveProperty<int> TotalExp = new ReactiveProperty<int>();

        private int? _exp;
        // (1of3) Отображение большой красивой цифры "тек.уровня" и бара под ней прогресса до следующего ур-ня на экране прогресса аккаунта - единственное место в игре, где фигурирует опыт заслуженный, а не скликнутый (игромеханический). С Данилой обсудил возможную путаницу игрока - так и задумано.
        public ReactiveProperty<int> Level    = new ReactiveProperty<int>();
        // (1of3) Отображение большой красивой цифры "тек.уровня" и бара под ней прогресса до следующего ур-ня на экране прогресса аккаунта - единственное место в игре, где фигурирует опыт заслуженный, а не скликнутый (игромеханический). С Данилой обсудил возможную путаницу игрока - так и задумано.
        public ReactiveProperty<int> DeltaExpNeededToGetNextLevel = new ReactiveProperty<int>(); 
        // (1of3) Отображение большой красивой цифры "тек.уровня" и бара под ней прогресса до следующего ур-ня на экране прогресса аккаунта - единственное место в игре, где фигурирует опыт заслуженный, а не скликнутый (игромеханический). С Данилой обсудил возможную путаницу игрока - так и задумано.
        public ReactiveProperty<float> CurrLvlExpProgressPercent = new ReactiveProperty<float>();
        public ReactiveProperty<int> ExpGainedOnCurrLvl = new ReactiveProperty<int>();
        public ReactiveProperty<bool> HasAchievedPacksToConsume = new ReactiveProperty<bool>();

        public ReactiveProperty<LocalizedString> Title = new ReactiveProperty<LocalizedString>();
        public ReactiveProperty<UnityRef<Sprite>> Stripe   = new ReactiveProperty<UnityRef<Sprite>>();

        public ReactiveProperty<AccountLevelRewardsPackVM> SelectedRewardsPack = new ReactiveProperty<AccountLevelRewardsPackVM>();
        public ReactiveProperty<bool> NeedRecalcScroll = new ReactiveProperty<bool>();

        // --- IHasDisposableComposite: -------------
        protected DisposableComposite D => ((IHasDisposableComposite)this).DisposableComposite;
        DisposableComposite IHasDisposableComposite.DisposableComposite { get; } = new DisposableComposite();


        // --- Privates: -------------------
        private Guid _accId;
        private IEntitiesRepository _repo;

        public ListStream<AccountLevelRewardsPackVM> LevelAchievementsList = new ListStream<AccountLevelRewardsPackVM>();

        internal Action OnClickPlayButton;
        public AccountGuiWindowVM(GameState gameState, StartGameGuiNode startGameNode, Action onClickPlayButton)
        {
            _gameState = gameState;
            _gameState.AssertIfNull(nameof(_gameState));
            OnClickPlayButton = onClickPlayButton;
            StartGameNode = startGameNode;
            Init();
        }

        private void Init()
        {
            var accountIdStream  = _gameState.AccountIdStream.WhereProp(D, accountId => !accountId.Equals(Guid.Empty));
            var repositoryStream = _gameState.ClientClusterRepository.WhereProp(D, repository => repository != null);
            accountIdStream.Action(D, id => _accId = id);
            repositoryStream.Action(D, repo => _repo = repo);

            Utils.ConnectEntity(D, _touchableAcc,       repositoryStream, accountIdStream, AccountEntity.StaticTypeId);

            _touchableAcc.ToStream(D, acc => acc.AccountId).Action(D, name => AccName.Value = name);
            var expStream = _touchableAcc.ToStream(D, acc => acc.Experience);
            var unconsumedExpStream = _touchableAcc.ToStream(D, acc => acc.UnconsumedExperience);
            GenderStream = _touchableAcc.ToStream(D, acc => acc.Gender);
            GenderIntStream0Based = GenderStream.Func(D, gender => Constants.CharacterConstants.Genders.IndexOf(gender));

            AnyExpChangedStream = expStream.Zip(D, unconsumedExpStream);
            AnyExpChangedStream.Action(D, (exp, unconsumedExp) =>
            {
                _exp = exp;
                TotalExp.Value = exp + unconsumedExp;
                Level.Value = LevelUpDatasHelpers.CalcAccLevel(TotalExp.Value);
                DeltaExpNeededToGetNextLevel.Value = LevelUpDatasHelpers.GetDeltaExpNeededToGetNextLevel(Level.Value); 
                System.Diagnostics.Debug.Assert(DeltaExpNeededToGetNextLevel.Value > 0);
                HasAchievedPacksToConsume.Value = Level.Value > LevelUpDatasHelpers.CalcAccLevel(exp);
                var expToReachCurrLvl = LevelUpDatasHelpers.CalcTotalExpNeededToReachLvl(Level.Value);
                ExpGainedOnCurrLvl.Value = TotalExp.Value - expToReachCurrLvl;
                CurrLvlExpProgressPercent.Value = (float)ExpGainedOnCurrLvl.Value / DeltaExpNeededToGetNextLevel.Value; 

                // if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041-1: total:{TotalExp.Value} = exp:{exp} + uncnsmd:{unconsumedExp}");
                // if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041 2: gained:{ExpGainedOnCurrLvl.Value} = ttl:{TotalExp.Value} - toRch:{expToReachCurrLvl}");
                // if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041 3: CurrLvl%:{CurrLvlExpProgressPercent.Value} = gained:{ExpGainedOnCurrLvl.Value} / dltNxtLvl:{DeltaExpNeededToGetNextLevel.Value}");

                var currLvlRewardsPack = LevelUpDatasHelpers.GetCurrLvlRewardsPack(exp);
                Title.Value  = currLvlRewardsPack.Title.Target.Name;
                Stripe.Value = currLvlRewardsPack.Title.Target.StripeIcon;
            });

            FillLevelAchievementsList(AnyExpChangedStream);

            // for HasActiveSession: ----------------
            PrepareHasActiveSession(repositoryStream);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            _touchableAcc?.Dispose();
        }

        private void FillLevelAchievementsList(IStream<(int, int)> anyExpChangedStream)
        {
            var list = LevelUpDatasHelpers.LevelAchievementsList;
            for (int i =1;  i < list.Count;  ++i) //from i=1 - Don't show 1st lvl 'cos it's already achieved from very start
            {
                var packDef = list[i];
                if (packDef != null && (packDef.Title.IsValid || packDef.Achievement.IsValid)) //don't show technicalemptyelements
                    LevelAchievementsList.Add(new AccountLevelRewardsPackVM(packDef, i + 1, anyExpChangedStream, OnSelected, OnClickAccLvlRewardsPack, WhenRewardPackIsAddedToSceneCallback));
            }
        }

        private void WhenRewardPackIsAddedToSceneCallback()
        {
            NeedRecalcScroll.Value = true;
        }

        private void OnSelected(AccountLevelRewardsPackVM rpvm, bool isOn)
        {
            if (isOn)
                SelectedRewardsPack.Value = rpvm;
            else if (SelectedRewardsPack.HasValue && rpvm == SelectedRewardsPack.Value) // Off only self (for case of invalid unhover/hover callbacks order)
                SelectedRewardsPack.Value = null;
        }

        private float _lastOnClick;
        private const float OnClickCooldown = 0.5f;
        private void CallActionOnAccEntityClFull(Func<IAccountEntityClientFull, Task> asyncCallback)
        {
            if (Time.realtimeSinceStartup < _lastOnClick + OnClickCooldown)
                return;
            
            _lastOnClick = Time.realtimeSinceStartup;
            
            if (_repo == null || _accId == default)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await _repo.Get<IAccountEntityClientFull>(_accId))
                {
                    if (wrapper != null)
                    {
                        var accEnt = wrapper.Get<IAccountEntityClientFull>(
                            EntitiesRepository.GetIdByType(typeof(IAccountEntityClientFull)),
                            _accId,
                            ReplicationLevel.ClientFull);
                        await asyncCallback(accEnt);
                    }
                }
            });
        }

        internal void ChoseGender(GenderDef gender)
        {
            CallActionOnAccEntityClFull(async (accEnt) =>
            {
                await accEnt.SetGender(gender);
            });
        }

        private void OnClickAccLvlRewardsPack(AccountLevelRewardsPackVM rpvm)
        {
            if (!rpvm.Def.HasValue)
            {
                Logger.Error("!rpvm.Def.HasValue. " + rpvm);
                return;
            }

            if (!_exp.HasValue)
            {
                Logger.Error("!_exp.HasValue. " + _exp);
                return;
            }

            CallActionOnAccEntityClFull(async (accEnt) =>
            {
                var lvl = LevelUpDatasHelpers.LevelByDef(rpvm.Def.Value);
                //Assert:
                if (rpvm.Level.HasValue && rpvm.Level.Value != lvl)
                    Logger.Error($"rpvm.Level.Value({rpvm.Level.Value}) != lvl({lvl}).");

                //await accEnt.ConsumeDeltaAccountExperience(deltaExpToConsume);
                //await accEnt.TrySetConsumedExp(LevelUpDatasHelpers.CalcTotalExpNeededToReachLvl(lvl));
                await accEnt.TryConsumeUnconsumedExp(LevelUpDatasHelpers.CalcDeltaExpFromCurrExpToLvl(_exp.Value, lvl));
            });
        }

        internal void ConsumeAllAchievedRewardsPacks()
        {
            if (!_exp.HasValue)
            {
                Logger.Error("!_exp.HasValue. " + _exp);
                return;
            }
            if (!Level.HasValue)
            {
                Logger.Error("!Level.HasValue. " + Level);
                return;
            }

            var deltaExpToConsume = LevelUpDatasHelpers.CalcDeltaExpFromCurrExpToLvl(_exp.Value, Level.Value);
            CallActionOnAccEntityClFull(async (accEnt) =>
            {
                await accEnt.TryConsumeUnconsumedExp(deltaExpToConsume);
            });
        }

    #region HasActiveSession

        internal IStream<bool> HasActiveSession;
        private const float RealmTimerUpdatePeriodSeconds = 60;

        private void PrepareHasActiveSession(IStream<IEntitiesRepository> repositoryStream)
        {
            var charRealmData = _touchableAcc.Child(D, account => account.CharRealmData);
            var realmOuterRefStream = charRealmData.ToStream(D, full => full.CurrentRealm);
            var realmCharStateStream = charRealmData.ToStream(D, c => c.CurrentRealmCharState);
            var realmEntityProxy = new OuterRefProxy<IRealmEntityClientFull>(realmOuterRefStream, repositoryStream);
            D.Add(realmEntityProxy);
            var currentRealmRulesDef = realmEntityProxy.ToStreamWithDefault(D, realm => realm.Def);
            var realmStartTime = realmEntityProxy.ToStreamWithDefault(D, realm => realm.StartTime, -1);
            var timer = TimeTicker.Instance.GetUtcTimer(RealmTimerUpdatePeriodSeconds);
            var realmDeadStream = currentRealmRulesDef
                .Zip(D, realmStartTime)
                .ZipSecondOrDefault(D, timer)
                .Func(
                    D, (def, startTime, now) => def == null || startTime == -1 || RealmEntity.IsRealmDead(def, startTime)
                );

            HasActiveSession = realmCharStateStream
                .Zip(D, realmEntityProxy.EntityExists)
                .Zip(D, realmDeadStream)
                .Func(D, (realmCharState, exists, realmDead) => realmCharState == RealmCharStateEnum.Active && exists && !realmDead);
        }

    #endregion

        public bool IsDisposed { get; private set; }
    }

}