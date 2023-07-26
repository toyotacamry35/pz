using ResourcesSystem.Loader;

namespace ResourcesSystem
{
    public static class GameResourcesExtensionsUnityRuntime
    {
        public static void ConfigureForUnityRuntime(this GameResources resources)
        {
            resources.Converters.Add(Assets.Src.ResourceSystem.Converters.UnityRuntimeRefConverter.Instance);
            resources.Converters.Add(Assets.Src.ResourceSystem.Converters.UnityRuntimeObjectConverter.Instance);
            resources.Converters.Add(new DefReferenceConverter(resources.Deserializer, false));
        }
    }
}
