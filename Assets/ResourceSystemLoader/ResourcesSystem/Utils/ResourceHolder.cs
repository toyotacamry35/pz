using Assets.ColonyShared.SharedCode.Utils;
using Assets.ResourceSystemLoader.ResourcesSystem.Utils;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Environment.Logging.Extension;

namespace ResourceIndexer.Validator
{
    public class ResourceHolder
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ValidatorCollection.ValidateHandler[] ValidatorsHandler = { ValidatorCollection.InvalidValidation, ValidatorCollection.DuplicatesValidation };

        public int NumberOfExceptions { get; private set; } = 0;
        public event LoadingResourceErrorEventHandler OnLoadingResourceError;

        public IEnumerable<IResource> AllResources => this._gameResources?.LoadedResources?.AllResources;

        private GameResources _gameResources;

        public static (IEnumerable<string> errors, IEnumerable<(Guid id, string root)> allKnownGuids) CheckResources(ILoader loader, CancellationToken ct)
        {
            List<string> errors = new List<string>(); 
            ResourceHolder resourceHolder = new ResourceHolder();
            resourceHolder.OnLoadingResourceError += (ex) =>
            {
                errors.Add(ex.MessageWithInnerExceptions());
            };

            resourceHolder.LoadResources(loader, ct);

            if (resourceHolder.AllResources == null)
                throw new Exception($"Can't load resources from {loader}");

            errors.AddRange(resourceHolder.ValidateResourcesIDs());
            return (errors, resourceHolder.AllResources.Where(v => v is ISaveableResource).Select(v => (((ISaveableResource)v).Id, v.Address.Root)).Distinct());
        }

        private static void LogProgress(int current, int total)
        {
            if ((10 * current / total) != (10 * (current - 1) / total))
            {
                 Logger.IfDebug()?.Message("  ..." + (int)(100 * current / (float)total) + "%").Write();;
            }
        }

        public void LoadResources(ILoader loader, CancellationToken ct)
        {
            Logger.IfDebug()?.Message("Loading Resources from {0}", loader).Write();

            _gameResources = new GameResources(loader);
            _gameResources.Converters.Add(new DefReferenceConverter(_gameResources.Deserializer, false));
            _gameResources.Converters.Add(UnityRefSkipConverter.Instance);
            _gameResources.CreateNetIDs();
            _gameResources.OnLoadingResourceError += (e) =>
            {
                NumberOfExceptions++;
                OnLoadingResourceError?.Invoke(e);
            };

            int count = 0;
            var allFiles = loader.AllPossibleRoots;
            var allFilesCount = allFiles.Count();

            foreach (string resourcePath in allFiles)
            {
                ct.ThrowIfCancellationRequested();

                _gameResources.LoadResource<IResource>(resourcePath);
                LogProgress(++count, allFilesCount);
            }

            Logger.IfDebug()?.Message($"Loaded {AllResources?.Count() ?? 0} Resources.").Write();
        }

        public IEnumerable<string> ValidateResourcesIDs()
        {
            IEnumerable<string> errors = Enumerable.Empty<string>();

            foreach (var validator in ValidatorsHandler)
            {
                var validatorErrors = validator(AllResources);
                errors = errors.Concat(validatorErrors);
            }

            return errors;
        }
    }
}
