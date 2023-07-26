using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using ResourceSystem.Aspects.AccessRights;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class OwnerInformation
    {
        public Task SetOwnerImpl(OuterRef<IEntity> owner)
        {
            Owner = owner;
            return Task.CompletedTask;
        }

        public Task SetLockPredicateImpl(AccessPredicateDef predicate)
        {
            LockPredicate = predicate ?? AccessPredicateDef.Empty; // опять наткнулся на то, что выставление в null не реплицируется из-за протобафа. поэтому приходится выстаявлять заглушку указывающую, что предиката нет  
            return Task.CompletedTask;
        }
    }
}