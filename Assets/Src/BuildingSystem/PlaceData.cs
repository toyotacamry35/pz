using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem.Delta;
using UnityEngine;
using SharedCode.Utils;
using SharedCode.Serializers;

namespace Assets.Src.BuildingSystem
{
    public abstract class PlaceData : VisualData
    {
        [Bind]
        public BuildState State { get; protected set; } = BuildState.None;

        public SharedCode.Utils.Vector3 Position { get; protected set; } = SharedCode.Utils.Vector3.zero;

        public SharedCode.Utils.Quaternion Rotation { get; protected set; } = SharedCode.Utils.Quaternion.identity;

        public SharedCode.Utils.Vector3 Scale { get; protected set; } = SharedCode.Utils.Vector3.one;
        
        public bool IsClient { get; set; } = false;
        public bool IsServer { get; set; } = false;
        public Guid PlaceId { get; set; } = Guid.Empty;

        public bool IsEmpty { get { return (PlaceId == Guid.Empty); } }

        // abstract methods -----------------------------------------------------------------------
        public abstract void GotClient();

        public abstract void LostClient();

        public abstract void GotServer();

        public abstract void LostServer();

        public abstract void Bind(IEntitiesRepository entitiesRepository, Guid placeId);

        public abstract void Unbind(IEntitiesRepository entitiesRepository);

        public abstract void SetVisualCheat(bool enable);

        public abstract bool CalculatePlaceholder(PlaceholderData data);

        public abstract ElementData ShowPlaceholder(ElementData placeholder, PlaceholderData data);

        public abstract bool HidePlaceholder(ElementData placeholder);
    }

    public abstract class PlaceDataEx<PlaceDefType, PlaceEntityType, PlaceStructureType, ElementDataType, ElementDeltaObjectType, ElementDeltaObjectTypeReplica, ElementBehaviourType> : PlaceData
        where PlaceDefType : IEntityObjectDef
        where PlaceEntityType : IEntity
        where PlaceStructureType : PlaceStructure<PlaceDefType, ElementDataType>, new()
        where ElementDataType : ElementDataEx<ElementDeltaObjectType, ElementDeltaObjectTypeReplica>, new()
        where ElementDeltaObjectType : IDeltaObject
        where ElementDeltaObjectTypeReplica : ElementReplica
        where ElementBehaviourType : ElementBehaviour<ElementDataType>
    {
        [Bind("Def")]
        public IEntityObjectDef EntityObjectDef { get; protected set; } = null;

        public PlaceDefType PlaceDef => (PlaceDefType)EntityObjectDef;

        public PlaceStructureType Structure { get; } = new PlaceStructureType();

        public override void GotClient()
        {
            IsClient = true;
            Structure.ForEach((ElementDataType data) =>
            {
                data.ElementGameObject?.GetComponent<ElementBehaviourType>()?.GotClient();
            });
        }

        public override void LostClient()
        {
            IsClient = false;
            Structure.ForEach((ElementDataType data) =>
            {
                data.ElementGameObject?.GetComponent<ElementBehaviourType>()?.LostClient();
            });
        }

        public override void GotServer()
        {
            IsServer = true;
            Structure.ForEach((ElementDataType data) =>
            {
                data.ElementGameObject?.GetComponent<ElementBehaviourType>()?.GotServer();
            });
        }

        public override void LostServer()
        {
            IsServer = false;
            Structure.ForEach((ElementDataType data) =>
            {
                data.ElementGameObject?.GetComponent<ElementBehaviourType>()?.LostServer();
            });
        }

        public override void Bind(IEntitiesRepository entitiesRepository, Guid placeId)
        {
            BuildUtils.Debug?.Report(true, $"repo {entitiesRepository.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            PlaceId = placeId;
            AsyncUtils.RunAsyncTask(() => BindTo(entitiesRepository), entitiesRepository);
        }

        public override void Unbind(IEntitiesRepository entitiesRepository)
        {
            BuildUtils.Debug?.Report(true, $"repo {entitiesRepository.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            UnbindFrom();
        }

        public override void SetVisualCheat(bool enable)
        {
            Structure.SetVisualCheat(enable);
        }

        private void UnbindFrom()
        {
            var elements = UnbindElements(null);
            RemoveElements(elements);
            Structure.Unbind(PlaceDef);
            InvokeUnbindFinished();
            PlaceId = Guid.Empty;
        }

        // Threadpool methods ---------------------------------------------------------------------
        private async Task BindTo(IEntitiesRepository entitiesRepository)
        {
            using (var wrapper = await entitiesRepository.Get<PlaceEntityType>(PlaceId))
            {
                var place = wrapper.Get<PlaceEntityType>(PlaceId);
                if (place != null)
                {
                    RegisterElementEvents(place);
                    var replica = ReplicaFactory.Get(place);
                    if (replica != null)
                    {
                        BindProperties(replica, place);
                        var elements = BindElements(replica);
                        await UnityQueueHelper.RunInUnityThread(() =>
                        {
                            SetInitialState(replica);
                            SetInitialStateSpecific(replica);
                            Structure.Bind(PlaceDef, Position, Rotation);
                            CreateElements(elements);
                            InvokeBindFinished();
                        });
                    }
                    else
                    {
                        BuildUtils.Error?.Report($"can't create replica for place {PlaceId} in repo {entitiesRepository.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    }
                }
                else
                {
                    BuildUtils.Error?.Report($"can't find place {PlaceId} in repo {entitiesRepository.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
            }
        }

        protected async Task Elements_OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<Guid, IPositionedBuildingElementAlways> args)
        {
            await OnItemAddedOrUpdatedInternal(args.Value);
        }

        protected async Task Elements_OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<Guid, IPositionedFenceElementAlways> args)
        {
            await OnItemAddedOrUpdatedInternal(args.Value);
        }

        private async Task OnItemAddedOrUpdatedInternal(IDeltaObject element)
        {
            var elementReplica = ReplicaFactory.Get(element) as ElementDeltaObjectTypeReplica;
            if (elementReplica != null)
            {
                var data = new ElementDataType();
                data.BindProperties(elementReplica, element);
                await UnityQueueHelper.RunInUnityThread(() => { CreateElement(elementReplica, data); });
            }
        }

        protected async Task Elements_OnItemRemoved(DeltaDictionaryChangedEventArgs<Guid, IPositionedBuildingElementAlways> args)
        {
            await OnItemremovedInternal(args.OldValue);
        }

        protected async Task Elements_OnItemRemoved(DeltaDictionaryChangedEventArgs<Guid, IPositionedFenceElementAlways> args)
        {
            await OnItemremovedInternal(args.OldValue);
        }

        private async Task OnItemremovedInternal(IDeltaObject element)
        {
            var elementReplica = ReplicaFactory.Get(element) as ElementDeltaObjectTypeReplica;
            if (elementReplica != null)
            {
                var data = new ElementDataType();
                var _elementId = data.ExtractElementId(elementReplica);
                if (Structure.TryGetValue(_elementId, out data))
                {
                    data.UnbindProperties(elementReplica, element);
                    await UnityQueueHelper.RunInUnityThread(() => { RemoveElement(elementReplica, data, _elementId); });
                }
            }
        }

        // Unity methods --------------------------------------------------------------------------
        protected void UpdateElementBehaviour(ElementDataType elementData, bool got, bool updateServer, bool updateClient)
        {
            if ((elementData != null) && (elementData.ElementGameObject != null))
            {
                var elementBehaviour = elementData.ElementGameObject.GetComponent<ElementBehaviourType>();
                if (elementBehaviour != null)
                {
                    if (IsServer && updateServer)
                    {
                        if (got)
                        {
                            elementBehaviour.GotServer();
                        }
                        else
                        {
                            elementBehaviour.LostServer();
                        }
                    }
                    if (IsClient && updateClient)
                    {
                        if (got)
                        {
                            elementBehaviour.GotClient();
                        }
                        else
                        {
                            elementBehaviour.LostClient();
                        }
                    }
                }
            }
        }

        protected void DestroyElementBehaviour(ElementDataType elementData)
        {
            if ((elementData != null) && (elementData.ElementGameObject != null))
            {
                bool destroyed = false;
                var elementBehaviour = elementData.ElementGameObject.GetComponent<ElementBehaviourType>();
                if (elementBehaviour != null)
                {
                    destroyed = elementBehaviour.DestroyGameObject();
                }
                if (!destroyed)
                {
                    elementData.DestroyElementGameObject();
                }
            }
        }

        protected void CreateElement(ElementDeltaObjectTypeReplica elementReplica, ElementDataType data)
        {
            data.Valid = true;
            data.PlaceId = PlaceId;
            data.Bind(elementReplica);
            data.CreateElementGameObject(InstantiateElement(data), false);
            Structure.Add(data.ElementId, data);
            UpdateElementBehaviour(data, true, true, true);
        }

        protected void RemoveElement(ElementDeltaObjectTypeReplica elementReplica, ElementDataType data, Guid elementId)
        {
            data.Unbind(elementReplica);
            Structure.Remove(elementId);
            UpdateElementBehaviour(data, false, true, true);
            DestroyElementBehaviour(data);
        }

        protected GameObject InstantiateElement(ElementDataType data)
        {
            GameObject result = null;

            if (!Application.isPlaying || (data == null))
            {
                return result;
            }

            var prefab = data.ExtractPrefab();
            var prefabDef = data.ExtractPrefabDef();

            UnityEngine.Vector3 transformPosition;
            UnityEngine.Quaternion transformRotation;

            data.PrefabPosition = prefab.Target.transform.position;
            data.PrefabRotation = prefab.Target.transform.rotation;
            data.PrefabScale = prefab.Target.transform.localScale;
            GetElementTransform(data, out transformPosition, out transformRotation);

            result = GameObjectInstantiate.Invoke(prefab, prefabDef, transformPosition, transformRotation, false);

            var behaviour = result.AddComponent<ElementBehaviourType>();
            behaviour.SetData(data);

            result.SetActive(true);
            return result;
        }

        // abstract methods -----------------------------------------------------------------------
        protected abstract void SetInitialStateSpecific(PropertyReplica elementToBindReplica);

        protected abstract void RegisterElementEvents(PlaceEntityType place);

        protected abstract void UnregisterElementEvents(PlaceEntityType place);

        protected abstract List<Tuple<ElementDeltaObjectTypeReplica, ElementDataType>> BindElements(PropertyReplica placeReplica);

        protected abstract List<Tuple<ElementDeltaObjectTypeReplica, ElementDataType, Guid>> UnbindElements(PropertyReplica placeReplica);

        protected void CreateElements(List<Tuple<ElementDeltaObjectTypeReplica, ElementDataType>> elements)
        {
            foreach( var element in elements)
            {
                CreateElement(element.Item1, element.Item2);
            }
        }

        protected void RemoveElements(List<Tuple<ElementDeltaObjectTypeReplica, ElementDataType, Guid>> elements)
        {
            foreach (var element in elements)
            {
                RemoveElement(element.Item1, element.Item2, element.Item3);
            }
        }

        protected abstract void GetElementTransform(ElementDataType data, out UnityEngine.Vector3 transformPosition, out UnityEngine.Quaternion transformRotation);
    }
}
