using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.AccessRights;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class SaveablePredicateDef : SaveableBaseResource
    {
        public ResourceRef<PredicateDef> Predicate;

        private static readonly ResourceRef<SaveablePredicateDef> _Empty = new ResourceRef<SaveablePredicateDef>("/UtilPrefabs/EmptyPredicate");
        public static SaveablePredicateDef Empty => _Empty.Target;

        // это всё не работает     
        // public static readonly AccessPredicateDef Empty = CreateEmpty();
        //
        // private static AccessPredicateDef CreateEmpty([CallerFilePath] string root = "")
        // {
        //     var empty = new AccessPredicateDef {Predicate = PredicateDef.True, Id = Guid.Parse("0E32C217-E04A-4E05-A77B-8AC8A79A0A6F")};
        //     ((IResource) empty).Address = new ResourceIDFull(root);
        //     GameResourcesHolder.Instance.LoadResource();
        //     return empty;
        // }
    }
}