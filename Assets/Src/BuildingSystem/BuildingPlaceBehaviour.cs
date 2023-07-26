using ColonyShared.SharedCode.Utils;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using System;
using System.Reflection;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    class BuildingPlaceBehaviour : PlaceBehaviour<BuildingPlaceData, IBuildingPlace>
    {
        //TODO building: test code, do not programmatically animate object
        private bool needUpdateVisual = false;
        //
        protected override PlaceType GetPlaceType() { return PlaceType.BuildingPlace; }

        protected override void AwakePlace()
        {
            //TODO building: test code, do not programmatically animate object
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            var meshRenderer = GetComponent<MeshRenderer>();
            if(meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }

        protected override void DestroyPlace(BuildingPlaceData data) { }

        protected override bool DestroyGameObject(BuildingPlaceData data) { return false; }

        protected override void CreateServer(BuildingPlaceData data) { }

        protected override void DestroyServer(BuildingPlaceData data)
        {
        }

        protected override void CreateVisual(BuildingPlaceData data)
        {
            //TODO building: test code, do not programmatically animate object
            if (!BuildSystem.Builder.IsSimpleMode)
            {
                needUpdateVisual = true;
                transform.position = new Vector3(data.Position.x, data.Position.y - 1.2f, data.Position.z);
                transform.rotation = (Quaternion)data.Rotation;
                var meshRenderer = GetComponent<MeshRenderer>();
                if(meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                }
            }
       }

        protected override void DestroyVisual(BuildingPlaceData data)
        {
        }

        protected override void UpdateVisual(BuildingPlaceData data)
        {
            //TODO building: test code, do not programmatically animate object
            if (data.DataValid && needUpdateVisual)
            {
                float distance = 2.2f;
                if ((data.BuildTimestamp > 0) && (data.BuildTime > 0))
                {
                    var currentTime = SyncTime.Now;
                    var seconds = SyncTime.ToSeconds(currentTime - data.BuildTimestamp);
                    if (seconds <= data.BuildTime)
                    {
                        float factor = Math.Min(1.0f, seconds / data.BuildTime);
                        transform.position = new Vector3(data.Position.x, data.Position.y - 1.2f + distance * factor, data.Position.z);
                    }
                    else
                    {
                        transform.position = new Vector3(data.Position.x, data.Position.y - 1.2f + distance, data.Position.z);
                    }
                }
                if ( data.State == BuildState.Completed)
                {
                    transform.position = new Vector3(data.Position.x, data.Position.y - 1.2f + distance, data.Position.z);
                    needUpdateVisual = false;
                }
            }
        }

        protected override void BindPropertyChanged(BuildingPlaceData data, PropertyData.PropertyArgs propertyArgs)
        {
            if (propertyArgs.PropertyName.Equals("State") && (data.State == BuildState.Completed))
            {
                BuildSystem.Builder.InvokeBuildingPlaceChanged();
            }
        }

        protected override void BindFinished(BuildingPlaceData data)
        {
            BuildSystem.Builder.InvokeBuildingPlaceChanged();
        }

        protected override void UnbindFinished(BuildingPlaceData data)
        {
            BuildSystem.Builder.InvokeBuildingPlaceChanged();
        }
    }
}