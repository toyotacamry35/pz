using UnityEngine;

namespace Src.Locomotion.Unity
{
    internal static class LocomotionColliderUtils
    {
        internal static ContactPointObjectType GetObjectTypeByLayer(int layer, int groundLayerMask, int actorLayerMask)
        {
            var layerBit = 1 << layer;
            
            if ((layerBit & groundLayerMask) != 0)
                return ContactPointObjectType.Ground;
            
            if ((layerBit & actorLayerMask) != 0)
                return ContactPointObjectType.Actor;

            return ContactPointObjectType.Other;
        }
    }
}