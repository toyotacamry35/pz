using System;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using SharedCode.Repositories;
using Src.Aspects.Doings;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class BuildingElementBehaviour : ElementBehaviour<BuildingElementData>, IAttackTargetComponent
    {
        private DelayAndDestroyVisualTimer destroyTimer = new DelayAndDestroyVisualTimer();

        protected override void AwakeElement()
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, null);
        }

        protected override void DestroyElement(BuildingElementData data)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, null);
            destroyTimer = null;
        }

        protected override bool DestroyGameObject(BuildingElementData data)
        {
            if (data.IsDestroyed())
            {
                var delayTime = (data.Depth * SharedCode.Utils.BuildUtils.BuildParamsDef.DepthSeconds);
                var destroyTime = (data.BuildRecipeDef.Visual?.SelfDestructTime ?? 3.0f);
                destroyTimer.Set(delayTime, destroyTime, data, gameObject);
            }
            return destroyTimer.IsInProgress();
        }

        protected override void UpdateElement(BuildingElementData data, bool force, bool isServer, bool isClient)
        {
            if (force)
            {
                VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
            }
            destroyTimer.Update();
        }

        protected override void CreateServer(BuildingElementData data)
        {
            data.Visible = true;
        }

        protected override void DestroyServer(BuildingElementData data)
        {
            data.Selected = false;
            data.Visible = false;
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data); // because events already unbinded and not called in data.Visible setter
        }

        protected override void CreateVisual(BuildingElementData data)
        {
            data.Visible = true;
            if (data.State == BuildState.Completed)
            {
                var animator = gameObject.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Opened", (data.Interaction == 1));
                }
            }
        }

        protected override void DestroyVisual(BuildingElementData data)
        {
            data.Selected = false;
            data.Visible = data.IsDestroyed();
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data); // because events already unbinded and not called any data.Selected or data.Visible or data.Delay setter
        }

        protected override void BindPropertyChanged(BuildingElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            if (propertyArgs.PropertyName.Equals("State"))
            {
                if ((data.State != BuildState.Destroyed) && (data.State != BuildState.Removed))
                {
                    VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
                }
            }
            else if (propertyArgs.PropertyName.Equals("Interaction"))
            {
                if (data.State == BuildState.Completed)
                {
                    var animator = gameObject.GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        animator.SetBool("Opened", (data.Interaction == 1));
                    }
                }
            }
        }

        protected override void PlaceholderChanged(BuildingElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void ValidChanged(BuildingElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void SelectedChanged(BuildingElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void VisibleChanged(BuildingElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        OuterRef IAttackTargetComponent.EntityId => new OuterRef(GetData().PlaceId, ReplicaTypeRegistry.GetIdByType(typeof(IBuildingPlace)));

        Guid IAttackTargetComponent.SubObjectId => GetData().ElementId;
    }
}