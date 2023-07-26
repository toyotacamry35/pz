using ReactivePropsNs;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;

namespace Uins
{
    public class RewardElementVM : BindingVmodel
    {
        public RewardDef Def { get; }

        private readonly ReactiveProperty<int> _count = new ReactiveProperty<int>();
        public IStream<int> Count => _count;

        public bool IsMultiply => Def?.ExperienceMultiplier > 0;

        public RewardElementVM(RewardDef def, int count)
        {
            Def = def;
            _count.Value = count;
        }

        public void SetCount(int value)
        {
            _count.Value = value;
        }
    }
}