using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Building;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class FencePlaceData : PlaceDataEx<FencePlaceDef, IFencePlaceAlways, FencePlaceStructure, FenceElementData, IPositionedFenceElementAlways, PositionedFenceElementAlwaysReplica, FenceElementBehaviour>
    {
        private static Dictionary<string, PropertyDataHelper.CopyInfo> copyActionsCache = null;
        protected override Dictionary<string, PropertyDataHelper.CopyInfo> GetСopyActionsCache() { return copyActionsCache; }
        protected override void SetСopyActionsCache(Dictionary<string, PropertyDataHelper.CopyInfo> cache) { copyActionsCache = cache; }

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
            var data = _data as FencePlaceholderData;
            if (data != null)
            {
                return data.Calculate();
            }
            return false;
        }

        public override ElementData ShowPlaceholder(ElementData _placeholder, PlaceholderData _data)
        {
            IsClient = true; // only fake fence place for placeholder calculating
            var placeholder = _placeholder as FenceElementData;
            var data = _data as FencePlaceholderData;
            if ((placeholder != null) && (data != null))
            {
                if (placeholder.BuildRecipeDef == data.BuildRecipeDef)
                {
                    if ((placeholder.Valid != data.CanPlace.Result) || ((Vector3)placeholder.Position != data.Position) || ((Quaternion)placeholder.Rotation != data.Rotation))
                    {
                        placeholder.SetPlace(data.CanPlace.Result, data.Position, data.Rotation);
                        if (placeholder.ElementGameObject != null)
                        {
                            Vector3 transformPosition;
                            Quaternion transformRotation;
                            GetElementTransform(placeholder, out transformPosition, out transformRotation);
                            placeholder.ElementGameObject.SetPositionAndRotation(transformPosition, transformRotation);
                            placeholder.ElementGameObject.GetComponent<FenceElementBehaviour>()?.UpdateElement(true);
                        }
                    }
                    return placeholder;
                }
                else
                {
                    HidePlaceholder(_placeholder);
                }
            }
            placeholder = new FenceElementData();
            placeholder.PlaceId = Guid.Empty;
            placeholder.BindToPlaceholder(data.BuildRecipeDef, data.CanPlace.Result, data.Position, data.Rotation);
            placeholder.CreateElementGameObject(InstantiateElement(placeholder), SharedCode.Utils.BuildUtils.BuildParamsDef.ElasticEnable);
            UpdateElementBehaviour(placeholder, true, false, true);
            return placeholder;
        }

        public override bool HidePlaceholder(ElementData _placeholder)
        {
            var placeholder = _placeholder as FenceElementData;
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
            var fencePlaceAlwaysReplica = elementToBindReplica as FencePlaceAlwaysReplica;
            if (fencePlaceAlwaysReplica != null)
            {
                Position = fencePlaceAlwaysReplica.Position;
                Rotation = fencePlaceAlwaysReplica.Rotation;
                Scale = fencePlaceAlwaysReplica.Scale;
            }
        }

        protected override void RegisterElementEvents(IFencePlaceAlways place)
        {
            place.Elements.OnItemAddedOrUpdated += Elements_OnItemAddedOrUpdated;
            place.Elements.OnItemRemoved += Elements_OnItemRemoved;
        }

        protected override void UnregisterElementEvents(IFencePlaceAlways place)
        {
            place.Elements.OnItemAddedOrUpdated -= Elements_OnItemAddedOrUpdated;
            place.Elements.OnItemRemoved -= Elements_OnItemRemoved;
        }

        protected override void GetElementTransform(FenceElementData data, out Vector3 transformPosition, out Quaternion transformRotation)
        {
            data.Calculate(out transformPosition, out transformRotation);
        }

        protected override List<Tuple<PositionedFenceElementAlwaysReplica, FenceElementData>> BindElements(PropertyReplica _placeReplica)
        {
            var placeReplica = _placeReplica as FencePlaceAlwaysReplica;
            var result = new List<Tuple<PositionedFenceElementAlwaysReplica, FenceElementData>>();
            foreach (var element in placeReplica.Elements)
            {
                var data = new FenceElementData();
                data.BindProperties(element.Value.Key, element.Value.Value);
                result.Add(new Tuple<PositionedFenceElementAlwaysReplica, FenceElementData>(element.Value.Key, data));
            }
            return result;
        }

        protected override List<Tuple<PositionedFenceElementAlwaysReplica, FenceElementData, Guid>> UnbindElements(PropertyReplica _placeReplica)
        {
            var result = new List<Tuple<PositionedFenceElementAlwaysReplica, FenceElementData, Guid>>();
            if (_placeReplica == null)
            {
                Structure.ForEach(element => result.Add(new Tuple<PositionedFenceElementAlwaysReplica, FenceElementData, Guid>(null, element, element.ElementId)));
            }
            else
            {
                var placeReplica = _placeReplica as FencePlaceAlwaysReplica;
                foreach (var element in placeReplica.Elements)
                {
                    var data = new FenceElementData();
                    var _elementId = data.ExtractElementId(element.Value.Key);
                    if (Structure.TryGetValue(_elementId, out data))
                    {
                        data.UnbindProperties(element.Value.Key, element.Value.Value);
                        result.Add(new Tuple<PositionedFenceElementAlwaysReplica, FenceElementData, Guid>(element.Value.Key, data, _elementId));
                    }
                }
            }
            return result;
        }
    }
}