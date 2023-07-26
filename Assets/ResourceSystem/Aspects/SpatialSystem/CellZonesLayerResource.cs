using System;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Collections;
using UnityEngine;

namespace Assets.Src.SpatialSystem
{
    public class CellZonesLayerResource : BaseResource
    {
        public UnityRef<TextAsset> IndexTexture { get; set; }
        public ResMap<ColorPoint, BaseResource> Zones { get; set; }
    }

    public struct ColorPoint
    {
        public int X;
        public int Y;
    }
}