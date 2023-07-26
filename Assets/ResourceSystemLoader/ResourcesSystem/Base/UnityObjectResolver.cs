namespace Assets.ColonyShared.SharedCode.ResourcesSystem.Base
{
    public interface IUnityObjectResolver
    {
        UnityEngine.Object Resolve(string path, System.Type type);
    }

    public static class UnityObjectResolverHolder
    {
        public static IUnityObjectResolver Instance { get; set; }
    }
}
