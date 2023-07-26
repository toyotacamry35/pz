using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourcesSystem.Loader
{
    public class SaveableIdStorage
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IReadOnlyDictionary<Guid, ResourceIDFull> _resources;

        private readonly LoadedResourcesHolder _loadedResources;
        private readonly IResourcesRepository _deserializer;

        public SaveableIdStorage(IResourcesRepository deserializer, LoadedResourcesHolder loadedResources)
        {
            _deserializer = deserializer;
            _loadedResources = loadedResources;
            try
            {
                var index = deserializer.LoadResource<SaveableResourceIndex>(ResourceIDFull.Parse("/SaveableIds"));
                _resources = index.Resources.ToDictionary(v => v.Id, v => ResourceIDFull.Parse(v.Root));
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Saveable id load error").Write();
            }
        }

        public IResource GetResourceById(Guid id)
        {
            if (!_resources.TryGetValue(id, out var res))
            {
                Logger.IfError()?.Message("Cannot find resource with ID {resource_id}").Write();
                return null;
            }
            _deserializer.LoadResource<ISaveableResource>(res);
            return _loadedResources.GetLoadedSaveable(id);
        }
    }
}
