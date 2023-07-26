using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ResourceSystem.Account;
using ReactivePropsNs;

namespace Uins
{
    public class AccountLevelRewardsPackVM : BindingVmodel
    {
        public ReactiveProperty<AccountLevelRewardsPackDef> Def = new ReactiveProperty<AccountLevelRewardsPackDef>();
        public IStream<bool> IsConsumed;
        public IStream<bool> IsInteractable;
        public ReactiveProperty<AchievedConsumedState> State = new ReactiveProperty<AchievedConsumedState>();
        /// <summary>
        /// For Wield (for ButtonSpritesSetSetter in inspector)
        /// </summary>
        public IStream<int> StateInt; 

        public ReactiveProperty<int> Level = new ReactiveProperty<int>();

        internal Action<AccountLevelRewardsPackVM, bool> SelectedCallback;
        internal Action<AccountLevelRewardsPackVM> ConsumeCallback;
        internal Action WhenElemIsAddedToSceneCallback;

        public AccountLevelRewardsPackVM(
            AccountLevelRewardsPackDef def, 
            int lvl, 
            IStream<(int, int)> anyExpChangedStream, 
            Action<AccountLevelRewardsPackVM, bool> selectedCallback, 
            Action<AccountLevelRewardsPackVM> consumeCallback, 
            Action whenElemIsAddedToSceneCallback)
        {
            Def.Value = def;
            SelectedCallback = selectedCallback;
            ConsumeCallback = consumeCallback;
            WhenElemIsAddedToSceneCallback = whenElemIsAddedToSceneCallback;
            Level.Value = lvl;

            anyExpChangedStream.Action(D, (exp, unconsumedExp) =>
            {
                var totalExp = exp + unconsumedExp;
                var isAchieved = CalcIsAchieved(totalExp);
                var isConsumed = CalcIsAchieved(exp);
                if (!isAchieved)
                {
                    State.Value = AchievedConsumedState.Locked;
                    return;
                }
                State.Value = isConsumed 
                    ? AchievedConsumedState.Consumed 
                    : AchievedConsumedState.Achieved;
            });

            StateInt   = State.Func(D, s => (int)s);
            IsConsumed = State.Func(D, s => s == AchievedConsumedState.Consumed);
            IsInteractable = State.Func(D, s => s == AchievedConsumedState.Achieved);
        }

        bool CalcIsAchieved(int exp) => Level.HasValue && Level.Value <= LevelUpDatasHelpers.CalcAccLevel(exp);

        // --- Util types: -----------------------------------
        public enum AchievedConsumedState
        {
            Locked,
            Achieved, // Достигнут ли необходимый уровень для доступности:
            Consumed  // Скликнули (забрали награду) ли этот элемент:
        }
    }
    
}