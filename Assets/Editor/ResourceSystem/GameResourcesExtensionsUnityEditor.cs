using ResourcesSystem.Loader;

namespace Assets.Src.ResourceSystem.Editor
{
    public static class GameResourcesExtensionsUnityEditor
    {
        public static void ConfigureForUnityEditor(this GameResources resources)
        {
            resources.Converters.Add(Converters.UnityOnImportRefConverter.Instance);
            resources.Converters.Add(Converters.UnityOnImportObjectConverter.Instance);
            resources.Converters.Add(new DefReferenceConverter(resources.Deserializer, false));
        }
        public static void ConfigureForUnityImport(this GameResources resources)
        {
            resources.Converters.Add(Converters.UnityOnImportRefConverter.Instance);
            resources.Converters.Add(Converters.UnityOnImportObjectConverter.Instance);
            resources.Converters.Add(new DefReferenceConverter(resources.Deserializer, true));
        }
    }
}
