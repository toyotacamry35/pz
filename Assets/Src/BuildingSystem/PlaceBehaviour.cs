using Assets.Src.SpawnSystem;
using GeneratedCode.Repositories;
using SharedCode.DeltaObjects.Building;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using SharedCode.Utils;
using System;
using System.Reflection;

namespace Assets.Src.BuildingSystem
{
    public abstract class PlaceBehaviour<PlaceDataType, IPlaceType> : EntityGameObjectComponent
        where PlaceDataType : PlaceData
    {
        private bool bindedToServer = false;
        private PlaceDataType placeData = null;

        // Helpers --------------------------------------------------------------------------------
        protected IEntitiesRepository GetEntitiesRepository() { return bindedToServer ? ServerRepo : ClientRepo; }
        protected PlaceDataType GetData() { return placeData; }

        // EntityGameObjectComponent methods ------------------------------------------------------
        void Awake()
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (BuildSystem.Builder.IsEnabled)
            {
                AwakePlace();
            }
        }

        protected override void DestroyInternal()
        {
            if (BuildSystem.Builder != null)
            {
                BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

                if (BuildSystem.Builder.IsEnabled)
                {
                    if (placeData != null)
                    {
                        DestroyPlace(placeData);
                        placeData = null;
                    }
                }
            }
        }

        protected override void GotClient()
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (BuildSystem.Builder.IsEnabled)
            {
                Bind(ClientRepo, false);
                if (placeData != null)
                {
                    placeData.GotClient();
                    CreateVisual(placeData);
                }
            }
        }

        protected override void GotServer()
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (BuildSystem.Builder.IsEnabled)
            {
                Bind(ServerRepo, true);
                if (placeData != null)
                {
                    placeData.GotServer();
                    CreateServer(placeData);
                }
            }
        }

        protected override void LostClient()
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (BuildSystem.Builder.IsEnabled)
            {
                if (placeData != null)
                {
                    placeData.LostClient();
                    DestroyVisual(placeData);
                }
                Unbind(ClientRepo, false);
            }
        }

        protected override void LostServer()
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (BuildSystem.Builder.IsEnabled)
            {
                if (placeData != null)
                {
                    placeData.LostServer();
                    DestroyServer(placeData);
                }
                Unbind(ServerRepo, true);
            }
        }

        // Utity methods --------------------------------------------------------------------------
        void Update()
        {
            if (BuildSystem.Builder.IsEnabled)
            {
                if ((placeData != null) && IsClient)
                {
                    UpdateVisual(placeData);
                }
            }
        }

        // Unity thread methods -------------------------------------------------------------------
        private void Bind(IEntitiesRepository entitiesRepository, bool isServer)
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (placeData == null)
            {
                bindedToServer = isServer;
                if (TypeId == ReplicaTypeRegistry.GetIdByType(typeof(IPlaceType)))
                {
                    placeData = BuildSystem.Builder.RegisterPlace(entitiesRepository, GetPlaceType(), Data_BindFinished, Data_UnbindFinished, Data_BindPropertyChanged, EntityId) as PlaceDataType;
                }
            }
        }

        private void Unbind(IEntitiesRepository entitiesRepository, bool isServer)
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if ((placeData != null) && (bindedToServer == isServer))
            {
                BuildSystem.Builder.UnregisterPlace(entitiesRepository, GetPlaceType(), placeData.PlaceId);
            }
        }

        private void Data_BindPropertyChanged(object sender, PropertyData.PropertyArgs propertyArgs)
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if ((placeData != null) && (placeData == sender))
            {
                BindPropertyChanged(placeData, propertyArgs);
            }
        }

        private void Data_BindFinished(object sender, EventArgs eventArgs)
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if ((placeData != null) && (placeData == sender))
            {
                BindFinished(placeData);
            }
        }

        private void Data_UnbindFinished(object sender, EventArgs eventArgs)
        {
            BuildUtils.Debug?.Report(true, $"enable: {BuildSystem.Builder.IsEnabled}, place data: {placeData != null}, IsClient: {IsClient}, IsServer {IsServer}, TypeId: {TypeId}, EntityId: {EntityId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            if (placeData != null)
            {
                if (placeData == sender)
                {
                    UnbindFinished(placeData);
                }
                placeData.BindFinished -= Data_BindFinished;
                placeData.UnbindFinished -= Data_UnbindFinished;
                placeData.BindPropertyChanged -= Data_BindPropertyChanged;
                placeData = null;
            }
        }

        // abstract methods -----------------------------------------------------------------------
        protected abstract PlaceType GetPlaceType();

        protected abstract void AwakePlace();

        protected abstract void DestroyPlace(PlaceDataType data);

        // return false if you want to destroy place right now by building system
        protected abstract bool DestroyGameObject(PlaceDataType data);

        protected abstract void CreateServer(PlaceDataType data);

        protected abstract void DestroyServer(PlaceDataType data);

        protected abstract void CreateVisual(PlaceDataType data);

        protected abstract void DestroyVisual(PlaceDataType data);

        protected abstract void UpdateVisual(PlaceDataType data);

        protected abstract void BindPropertyChanged(PlaceDataType data, PropertyData.PropertyArgs propertyArgs);

        protected abstract void BindFinished(PlaceDataType data);

        protected abstract void UnbindFinished(PlaceDataType data);
    }
}