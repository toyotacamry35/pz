using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using System;
using Core.Environment.Logging.Extension;
using SharedCode.Shared;

namespace Assets.Src.App
{
    public class RuntimeObjectResolver : IUnityObjectResolver
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly AssetBundleResolver _bundleResolver;

        public RuntimeObjectResolver(AssetBundleResolver bundle)
        {
            _bundleResolver = bundle;
        }

        public UnityEngine.Object Resolve(string path, Type type)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var bundle = _bundleResolver.Resolve(path);
            if (bundle == null)
            {
                 Logger.IfError()?.Message("Cant resolve bundle at path {0}", path).Write();
                return null;
            }

            var asset = bundle.LoadAsset(path, type);
            if (asset != null)
                return asset;

            path = PathExtension.GetPathWithoutExtension(path);

            asset = bundle.LoadAsset(path, type);
            if (asset != null)
                return asset;

             Logger.IfError()?.Message("Cant resolve asset of type {0} at path {1} in bundle {2}",  type, path, bundle.name).Write();
            return null;
        }
    }
}
