using Assets.Src.Effects.FX;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace Assets.Src.App.FXs
{

    public class FXInfo
    {
        public GameObject parent;
        public Vector3 position;
        public Quaternion rotation;

        public BaseResource markerId;
        public FXParamsOnObj.FXParamsValue[] pair;

        public FXInfo(GameObject setParent, Vector3 setPosition, Quaternion setRotation, BaseResource setMarker, FXParamsOnObj.FXParamsValue[] setPair)
        {
            parent = setParent;
            position = setPosition;
            rotation = setRotation;
            markerId = setMarker;
            pair = setPair;
        }

        public FXInfo(GameObject setParent, Vector3 setPosition, Quaternion setRotation)
        {
            parent = setParent;
            position = setPosition;
            rotation = setRotation;
        }

        public FXInfo(GameObject setParent, Vector3 setPosition, Vector3 provocationImpactDirection)
        {
            parent = setParent;
            position = setPosition;
            rotation = Quaternion.FromToRotation(Vector3.up, -provocationImpactDirection);
        }
    }
}
