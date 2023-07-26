using ReactivePropsNs;
using SharedCode.Aspects.Sessions;

namespace Uins
{
    public class RealmRulesVM : BindingVmodel
    {
        public RealmRulesDef Def { get; }
        public IStream<bool> Available { get; }

        public RealmRulesVM(RealmRulesDef def, IStream<bool> available)
        {
            Def = def;
            Available = available;
        }
    }
}