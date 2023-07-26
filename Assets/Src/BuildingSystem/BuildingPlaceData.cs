using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.DeltaObjects.Building;
using SharedCode.Aspects.Building;
using UnityEngine;
using System.Collections.Generic;
using System;
using SharedCode.EntitySystem;

namespace Assets.Src.BuildingSystem
{
    public class BuildingPlaceData : PlaceDataEx<BuildingPlaceDef, IBuildingPlaceAlways, BuildingPlaceStructure, BuildingElementData, IPositionedBuildingElementAlways, PositionedBuildingElementAlwaysReplica, BuildingElementBehaviour>
    {
        private static Dictionary<string, PropertyDataHelper.CopyInfo> copyActionsCache = null;
        protected override Dictionary<string, PropertyDataHelper.CopyInfo> GetСopyActionsCache() { return copyActionsCache; }
        protected override void SetСopyActionsCache(Dictionary<string, PropertyDataHelper.CopyInfo> cache) { copyActionsCache = cache; }

        [Bind]
        public long BuildTimestamp { get; protected set; } = 0;
        [Bind]
        public float BuildTime { get; protected set; } = 0.0f;
        [Bind]
        public int Visual { get; protected set; } = -1;
        
        public OuterRef<IEntity> Owner { get; protected set; } = OuterRef<IEntity>.Invalid;

        public bool DataValid => (State != BuildState.None);

        protected override VisualBehaviour.VisualState GetVisualStateEx()
        {
            return VisualBehaviour.VisualState.Invisible;
        }
        protected override string GetVisualCommonNameEx()
        {
            return string.Empty;
        }
        protected override string GetVisualVersionNameEx()
        {
            return string.Empty;
        }
        protected override float GetFracturedChunkScaleEx()
        {
            return 1.0f;
        }

        public override bool CalculatePlaceholder(PlaceholderData _data)
        {
            var data = _data as BuildingPlaceholderData;
            if (data != null)
            {
                return data.Calculate(this);
            }
            return false;
        }

        public override ElementData ShowPlaceholder(ElementData _placeholder, PlaceholderData _data)
        {
            if (IsEmpty && BuildSystem.Builder.IsSimpleMode)
            {
                IsClient = true; // empty building place for initial element placeholder calculating
            }
            var placeholder = _placeholder as BuildingElementData;
            var data = _data as BuildingPlaceholderData;
            if ((placeholder != null) && (data != null))
            {
                if (placeholder.BuildRecipeDef == data.BuildRecipeDef)
                {
                    if (data.CacheInvalidOverride || (placeholder.Valid != data.CanPlace.Result) || ((Vector3Int)placeholder.Block != data.Block) || (placeholder.Type != data.Type) || (placeholder.Face != data.Face) || (placeholder.Side != data.Side))
                    {
                        placeholder.SetPlace(data.CanPlace.Result, data.Position, data.Rotation, data.Block, data.Type, data.Face, data.Side);
                        if (placeholder.ElementGameObject != null)
                        {
                            Vector3 transformPosition;
                            Quaternion transformRotation;
                            GetElementTransform(placeholder, out transformPosition, out transformRotation);
                            placeholder.ElementGameObject.SetPositionAndRotation(transformPosition, transformRotation);
                            placeholder.ElementGameObject.GetComponent<BuildingElementBehaviour>()?.UpdateElement(true);
                        }
                    }
                    return placeholder;
                }
                else
                {
                    HidePlaceholder(_placeholder);
                }
            }
            placeholder = new BuildingElementData();
            placeholder.PlaceId = PlaceId;
            placeholder.BindToPlaceholder(data.BuildRecipeDef, data.CanPlace.Result, data.Position, data.Rotation, data.Block, data.Type, data.Face, data.Side);
            placeholder.CreateElementGameObject(InstantiateElement(placeholder), SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticEnable);
            UpdateElementBehaviour(placeholder, true, false, true);
            return placeholder;
        }

        public override bool HidePlaceholder(ElementData _placeholder)
        {
            var placeholder = _placeholder as BuildingElementData;
            if (placeholder != null)
            {
                UpdateElementBehaviour(placeholder, false, false, true);
                placeholder.Unbind(null);
                DestroyElementBehaviour(placeholder);
                return true;
            }
            return false;
        }

        // abstract methods -----------------------------------------------------------------------
        protected override void SetInitialStateSpecific(PropertyReplica elementToBindReplica)
        {
            var buildingPlaceAlwaysReplica = elementToBindReplica as BuildingPlaceAlwaysReplica;
            if (buildingPlaceAlwaysReplica != null)
            {
                Position = buildingPlaceAlwaysReplica.Position;
                Rotation = buildingPlaceAlwaysReplica.Rotation;
                Scale = buildingPlaceAlwaysReplica.Scale;
                Owner = buildingPlaceAlwaysReplica.Owner;
            }
        }

        protected override void RegisterElementEvents(IBuildingPlaceAlways place)
        {
            if (place != null)
            {
                place.Elements.OnItemAddedOrUpdated += Elements_OnItemAddedOrUpdated;
                place.Elements.OnItemRemoved += Elements_OnItemRemoved;
            }
        }

        protected override void UnregisterElementEvents(IBuildingPlaceAlways place)
        {
            if (place != null)
            {
                place.Elements.OnItemAddedOrUpdated -= Elements_OnItemAddedOrUpdated;
                place.Elements.OnItemRemoved -= Elements_OnItemRemoved;
            }
        }

        protected override void GetElementTransform(BuildingElementData data, out Vector3 transformPosition, out Quaternion transformRotation)
        {
            data.Calculate(this, out transformPosition, out transformRotation);
        }

        protected override List<Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData>> BindElements(PropertyReplica _placeReplica)
        {
            var placeReplica = _placeReplica as BuildingPlaceAlwaysReplica;
            var result = new List<Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData>>();
            foreach( var element in placeReplica.Elements)
            {
                var data = new BuildingElementData();
                data.BindProperties(element.Value.Key, element.Value.Value);
                result.Add(new Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData>(element.Value.Key, data));
            }
            return result;
        }

        protected override List<Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData, Guid>> UnbindElements(PropertyReplica _placeReplica)
        {
            var result = new List<Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData, Guid>>();
            if (_placeReplica == null)
            {
                Structure.ForEach(element => result.Add(new Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData, Guid>(null, element, element.ElementId)));
            }
            else
            {
                var placeReplica = _placeReplica as BuildingPlaceAlwaysReplica;
                foreach (var element in placeReplica.Elements)
                {
                    var data = new BuildingElementData();
                    var _elementId = data.ExtractElementId(element.Value.Key);
                    if (Structure.TryGetValue(_elementId, out data))
                    {
                        data.UnbindProperties(element.Value.Key, element.Value.Value);
                        result.Add(new Tuple<PositionedBuildingElementAlwaysReplica, BuildingElementData, Guid>(element.Value.Key, data, _elementId));
                    }
                }
            }
            return result;
        }
    }
}
