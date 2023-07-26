using SharedCode.DeltaObjects.Building;
using System.Collections.Generic;

namespace SharedCode.Aspects.Building
{
    public class BuildingElementDef : BuildElementDef
    {
        public BuildingElementType Type { get; set; }

        public float Weight             { get; set; }
        public float SupportWeight      { get; set; }

        public bool CanBuildOnGround    { get; set; }

        public uint BlockFace           { get; set; }
        public uint BlockEdge           { get; set; }
        public uint BlockEdgeDouble     { get; set; }

        public uint BlockEdgeRequired   { get; set; }
        public int MinBlockEdgeCount    { get; set; }

        public bool HasSide             { get; set; }

        //public Dictionary<int, BuildingFragmentDef> Fragments { get; set; }
    }
}
