using System;
using Assets.ResourceSystem.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;

namespace ResourcesSystem.Loader
{
    public static class GameResourcesHolder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static GameResources _instance;

        public static GameResources Instance
        {
            get => _instance;
            set
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"GameResources installed: {value}").Write();
                _instance = value;
                ResourceRefFactory.Instance.Initialize(value.Deserializer);
            }
        }

        static GameResourcesHolder()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Initialize ResourceRefFactory").Write();
            ResourceRefFactory.Instance.Initialize(GameResourcesHolderStub.Instance);
        }

        private class GameResourcesHolderStub : IResourcesRepository
        {
            public static IResourcesRepository Instance = new GameResourcesHolderStub();

            private GameResourcesHolderStub() { }
            public T LoadResource<T>(in ResourceIDFull id) where T : IResource => throw new InvalidOperationException($"Untimely resource loading: {id}");
        }
    }
}
