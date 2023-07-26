using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;

namespace ResourceSystem.Aspects.AccessRights
{
    public class AccessPredicateDef : SaveablePredicateDef
    {
        public new static AccessPredicateDef Empty => _Empty.Target;
        
        private static readonly ResourceRef<AccessPredicateDef> _Empty = new ResourceRef<AccessPredicateDef>("/UtilPrefabs/EmptyAccessPredicate");
    }
}
