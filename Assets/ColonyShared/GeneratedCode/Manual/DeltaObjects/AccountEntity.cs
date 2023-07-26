
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ResourceSystem.Account;
using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountEntity : IHookOnInit, IHookOnDatabaseLoad
    {
        [NotNull] private static readonly Logger Logger = LogManager.GetLogger("AccountEntity");
        public static ResourceIDFull OverrideRulesList;
        public Task OnInit()
        {
            FillDef();
            Gender = Constants.CharacterConstants.DefaultGender;

            return Task.CompletedTask;
        }

        public async ValueTask<bool> ClearAndConsumeOldRealmRewardsImpl()
        {
            CharRealmData.CurrentRealm = default;
            CharRealmData.CurrentRealmCharState = RealmCharStateEnum.Inactive;
            var accChar = Characters.First();
            var rew = accChar.CurrentSessionRewards;
            var exp = CalcRewardsTotalExp(rew, CharRealmData.CurrentRealmRulesCached);
            rew.Clear();

            //if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041: U1.UnconsumedExperience({UnconsumedExperience}) := MIN(+exp:{exp} : {UnconsumedExperience + exp}, maxAllwd:{LevelUpDatasHelpers.MaxAllowedUnconsumedExp(Experience)}  ==>  {Math.Min(UnconsumedExperience + exp, LevelUpDatasHelpers.MaxAllowedUnconsumedExp(Experience))}");
            UnconsumedExperience = Math.Min(UnconsumedExperience + exp, LevelUpDatasHelpers.MaxAllowedUnconsumedExp(Experience));
            CharRealmData.CurrentRealmRulesCached = null;
            var login = Characters.First().CharacterName;
            Characters.Clear();
            var newChar = await CreateNewCharacter(login, Id);
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntity>())
            {
                var entityService = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                await entityService.UpdateCharacter(CurrentUserId, newChar.CharacterId);
            }
            return true;
        }

        public static int CalcRewardsTotalExp(IDictionary<RewardDef, int> rew, RealmRulesDef currentRealmRules)
        {
            var exp = 0;
            float multiplier = 1;
            foreach (var r in rew)
            {
                exp += r.Key.Experience * r.Value;

                if (Math.Abs(r.Key.ExperienceMultiplier) > float.Epsilon)
                    multiplier *= r.Key.ExperienceMultiplier;
            }

            exp = (int)(exp * currentRealmRules.ExpMultiplier * multiplier);
            return exp;
        }

        public async ValueTask<bool> ConsumeRewardsImpl()
        {
            //мы пока не проверяем, что этого не сделали во время сессии
            //поскольку геймплейные данные всегда ориентируются на значения, закешированные в энтити это не можжет быть
            //за абьюзено, но мало ли что в будущем появится
            await ClearAndConsumeOldRealmRewards();
            return true;
        }

        public Task OnDatabaseLoad()
        {
            FillDef();

            return Task.CompletedTask;
        }

        private void FillDef()
        {
            var def = GameResourcesHolder.Instance.LoadResource<RealmRulesQueriesConfigDef>(
                OverrideRulesList != default ? OverrideRulesList : new ResourceIDFull("/Sessions/GameRuleQueriesConfig"));
            var levelsDef = GameResourcesHolder.Instance.LoadResource<LevelUpDatasDef>("/UtilPrefabs/Res/Prototypes/LevelUpDatasDef");

            AvailableRealmQueries = new DeltaDictionary<RealmRulesQueryDef, RealmRulesQueryState>();

            var accLevel = CalcerPlayerLevel.Calc(levelsDef.DataList, Experience);
            if (Constants.WorldConstants.IsRelease)
                def.Release.Target.Queries
                    .Select(defRef => defRef.Target)
                    .ForEach(queryDef =>
                        AvailableRealmQueries.Add(queryDef, new RealmRulesQueryState(queryDef.RealmRules.Target.MinLevel <= accLevel))
                    );
            else
                def.Develop.Target.Queries
                    .Select(defRef => defRef.Target)
                    .ForEach(queryDef =>
                        AvailableRealmQueries.Add(queryDef, new RealmRulesQueryState(queryDef.RealmRules.Target.MinLevel <= accLevel))
                    );

        }

        public ValueTask<CreateNewCharacterResult> CreateNewCharacterImpl(string name, Guid accountId)
        {
            var newCharacter = new AccountCharacter { Id = Guid.NewGuid() };
            Logger.IfInfo()?.Message("Creating character with id {character_id} for account {account_id}", newCharacter.Id, Id).Write();
            Characters.Add(newCharacter);
            return new ValueTask<CreateNewCharacterResult>(new CreateNewCharacterResult
            {
                Result = CreateNewCharacterResultType.Success,
                CharacterId = newCharacter.Id
            });
        }

        public ValueTask<DeleteCharacterResultType> DeleteAccountCharacterImpl(Guid characterId)
        {
            var character = Characters.FirstOrDefault(x => x.Id == characterId);
            if (character == null)
                return new ValueTask<DeleteCharacterResultType>(DeleteCharacterResultType.ErrorCharacterNotFound);
            Characters.Remove(character);
            return new ValueTask<DeleteCharacterResultType>(DeleteCharacterResultType.Success);
        }

        public ValueTask SetCurrentUserIdImpl(Guid userId)
        {
            CurrentUserId = userId;
            return new ValueTask();
        }

        public ValueTask<Guid> GetCurrentUserIdImpl()
        {
            return new ValueTask<Guid>(CurrentUserId);
        }


        public Task TryConsumeUnconsumedExpImpl(int val)
        {
            SystemLogger.IfDebug()?.Message($"TryConsumeUnconsumedExpImpl({val}) called").Write();

            if (val > 0)
            {
                if (val <= UnconsumedExperience)
                {
                    var prevExp = Experience;
                    var prevUnconsumed = UnconsumedExperience;
                    try
                    {
                        var addedDelta = TryAddExperienceDo(val);
                        if (addedDelta > 0)
                        {
                            //if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041: U2.UnconsumedExperience({UnconsumedExperience}) -= addedDelta:{addedDelta}  ==>  {UnconsumedExperience - addedDelta}");
                            UnconsumedExperience -= addedDelta;
                        }
                    }
                    catch (Exception e)
                    {
                        Experience = prevExp;
                        UnconsumedExperience = prevUnconsumed;
                        Logger.IfError()?.Message($"Got an exception: \"{e}\". Values of `Experience` & `UnconsumedExperience` restored.");
                    }
                }
                else
                    Logger.IfError()?.Message($"val:{val} > UnconsumedExperience {UnconsumedExperience} !").Write();
            }
            else if (val == 0)
                Logger.IfWarn()?.Message("TryConsumeUnconsumedExpImpl({val == 0}) called").Write();
            else if (val < 0)
                Logger.IfWarn()?.Message($"TryConsumeUnconsumedExpImpl(val < 0) : {val}").Write();
            FillDef();
            return Task.CompletedTask;
        }

        //#tmp: usings: only1 - `LoginServiceEntty.AddAccountExperience`, у кот-го нет using'ов
        public ValueTask<int> AddExperienceImpl(int deltaVal)
        {
            TryAddExperienceDo(deltaVal);
            return new ValueTask<int>(Experience);
        }

        /// <summary>
        /// Trying add deltaVal, checking, not crossing maxConsumedExp allowed val.
        /// </summary>
        /// <returns> Factually added delta. </returns>
        private int TryAddExperienceDo(int deltaVal)
        {
            var prevExp = Experience;
            if (deltaVal > 0)
            {
                var maxConsumedExpVal = LevelUpDatasHelpers.CalcTotalExpNeededToReachLvl(LevelUpDatasHelpers.MaxLevel);
                if (maxConsumedExpVal < Experience)
                {
                    Logger.IfError()?.Message($"#UNEXPECTED: maxConsumedExpVal < Experience : {maxConsumedExpVal} < {Experience}!");
                    return 0;
                }

                if (deltaVal > maxConsumedExpVal - Experience)
                {
                    Logger.IfError()?.Message($"AddExperienceImpl deltaVal:{deltaVal} > maxConsumedExpVal - Experience : {maxConsumedExpVal - Experience} (=={maxConsumedExpVal} - {Experience}). We'll clamp it here.");
                    deltaVal = maxConsumedExpVal - Experience;
                }
            }

            //if (DbgLog.Enabled) DbgLog.Log($"#PZ-17041: E1.Experience({Experience}) = MAX(0, +deltaVal:{deltaVal})  ==>  {Math.Max(0, Experience + deltaVal)}");
            Experience = Math.Max(0, Experience + deltaVal);
            return Experience - prevExp;
        }

        public Task SetGenderImpl(GenderDef val)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"SetGenderImpl({val?.____GetDebugAddress()}) called").Write();

            if (Gender != val)
                Gender = val;

            return Task.CompletedTask;
        }

        public Task<int> CalcAccLevelImpl()
        {
            return Task.FromResult(LevelUpDatasHelpers.CalcAccLevel(Experience));
        }
    }
}