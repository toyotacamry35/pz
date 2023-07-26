using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;

namespace Uins
{
    public class LevelContentVM : BindingVmodel
    {
        public IStream<int> CurrentTotalExp { get; }

        public IStream<int> IncomingTotalExp { get; }

        public LevelContentVM(StartGameWindowVM vm, IStream<int> rewardsTotalStream)
        {
            var touchableAccount = vm.TouchableAccount;
            var expStream = touchableAccount.ToStream(D, account => account.Experience);
            var unconsumedExpStream = touchableAccount.ToStream(D, acc => acc.UnconsumedExperience);

            var totalExpStream = expStream.Zip(D, unconsumedExpStream);
            CurrentTotalExp = totalExpStream.Func(D, (exp, unconsumedExp) => exp + unconsumedExp);

            IncomingTotalExp = CurrentTotalExp
                .Zip(D, rewardsTotalStream)
                .Func(D, (exp, incomingExp) => Math.Min(exp + incomingExp, LevelUpDatasHelpers.MaxAllowedExperience));
        }
    }
}