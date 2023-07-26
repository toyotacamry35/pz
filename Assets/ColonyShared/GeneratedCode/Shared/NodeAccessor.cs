using GeneratedCode.Manual.Repositories;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Shared
{
    public static class NodeAccessor
    {
        private static IEntitiesRepository _repository;
        public static IEntitiesRepository Repository
        {
            get
            {
                //((IEntitiesRepositoryDataExtension) _repository)?.InitializeAsyncEntitiesRepositoryRequestContext();
                return _repository;
            }
            set { _repository = value; }
        }
    }
}
