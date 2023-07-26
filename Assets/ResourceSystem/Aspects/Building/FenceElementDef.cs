using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;

namespace SharedCode.Aspects.Building
{
    public class FenceElementDef : BuildElementDef
    {
        public class FenceLinkPoint
        {
            public bool InitialLink { get; set; }
            public bool AddLink { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }

        public Utils.Vector3 Center { get; set; } // box collider center
        public Utils.Vector3 Extents { get; set; } // box collider extents + distance from center to link edges (Z)
        //
        public List<ResourceRef<FenceElementDef>> AvailableToLink { get; set; } // objects that we can link
        public ResourceRef<FenceLinkDef> LinkDef { get; set; } // link object (pillar)
        public List<FenceLinkPoint> LinkPoints { get; set; } // link points
        public float LinkAngle { get; set; } // maximun rotation angle for linked elements (from stright line) --/-- this one (0...180)
        public float LinkHeight { get; set; } // maximun Height difference for linked elements (from horisontalt line) --|__ this one (0...inf)
        //
        public Dictionary<int, FenceFragmentDef> Fragments { get; set; } // for dynamic fences (fragments)
    }
}
