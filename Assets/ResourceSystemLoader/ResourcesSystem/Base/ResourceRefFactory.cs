using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using System;

namespace Assets.ResourceSystem.ResourcesSystem.Base
{
    public class ResourceRefFactory : IResourcesRepository
    {
        public static ResourceRefFactory Instance { get; } = new ResourceRefFactory();

        private IResourcesRepository _repository;

        public void Initialize(IResourcesRepository repository)
        {
            _repository = repository;
        }

        public T LoadResource<T>(in ResourceIDFull id) where T : IResource => _repository.LoadResource<T>(in id);
    }
}
