using System;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using Src.Aspects.Doings;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class FenceElementBehaviour : ElementBehaviour<FenceElementData>, IAttackTargetComponent
    {
        private DelayAndDestroyVisualTimer destroyTimer = new DelayAndDestroyVisualTimer();

        protected override void AwakeElement()
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, null);
        }

        protected override void DestroyElement(FenceElementData data)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, null);
            destroyTimer = null;
        }

        protected override bool DestroyGameObject(FenceElementData data)
        {
            if (data.IsDestroyed())
            {
                var delayTime = 0.0f;
                var destroyTime = (data.BuildRecipeDef.Visual?.SelfDestructTime ?? 3.0f);
                destroyTimer.Set(delayTime, destroyTime, data, gameObject);
            }
            return destroyTimer.IsInProgress();
        }

        protected override void UpdateElement(FenceElementData data, bool force, bool isServer, bool isClient)
        {
            if (force)
            {
                VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
            }
            destroyTimer.Update();
        }

        protected override void CreateServer(FenceElementData data)
        {
            data.Visible = true;
        }

        protected override void DestroyServer(FenceElementData data)
        {
            data.Selected = false;
            data.Visible = false;
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data); // because events already unbinded and not called in data.Visible setter
        }

        protected override void CreateVisual(FenceElementData data)
        {
            data.Visible = true;
            if (!data.Placeholder)
            {
                data.CreateLinks();
            }
            if (data.State == BuildState.Completed)
            {
                var animator = gameObject.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Opened", (data.Interaction == 1));
                }
            }
        }

        protected override void DestroyVisual(FenceElementData data)
        {
            bool destroyed = data.IsDestroyed();
            if (!data.Placeholder)
            {
                data.DestroyLinks(destroyed);
            }
            data.Selected = false;
            data.Visible = destroyed;
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data); // because events already unbinded and not called any data.Selected or data.Visible or data.Delay setter
        }

        protected override void BindPropertyChanged(FenceElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            if (propertyArgs.PropertyName.Equals("State"))
            {
                VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
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

        protected override void PlaceholderChanged(FenceElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void ValidChanged(FenceElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void SelectedChanged(FenceElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }

        protected override void VisibleChanged(FenceElementData data, PropertyData.PropertyArgs propertyArgs)
        {
            VisualBehaviour.UpdateVisualBehaviour(gameObject, data);
        }
        
        OuterRef IAttackTargetComponent.EntityId => new OuterRef(GetData().PlaceId, EntitiesRepository.GetIdByType(typeof(IFencePlace)));

        Guid IAttackTargetComponent.SubObjectId => GetData().ElementId;
    }
}