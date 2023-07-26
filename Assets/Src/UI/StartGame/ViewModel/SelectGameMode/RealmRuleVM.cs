using ReactivePropsNs;
using SharedCode.Aspects.Sessions;

namespace Uins
{
    public class RealmRuleVM : BindingVmodel
    {
        public RealmRuleDef Def { get; }
        public IStream<bool> Available { get; }

        public RealmRuleVM(RealmRuleDef def, IStream<bool> available)
        {
            Def = def;
            Available = available;
        }
    }
}