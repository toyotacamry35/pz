using System;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class EdgeHelperBehaviour : MonoBehaviour
    {
        [Flags]
        public enum Faces
        {
            FORWARD          = 0x00001,
            LEFT             = 0x00002,
            BACKWARD         = 0x00004,
            RIGHT            = 0x00008,
            TOP              = 0x00010,
            BOTTOM           = 0x00020,
            CENTER           = 0x00040,
            OVER             = 0x00080,
            UNDER            = 0x00100,
            DO_NOT_PICK      = 0x00200
        }

        [Flags]
        public enum Edges
        {
            FORWARD_TOP      = 0x00001,
            LEFT_TOP         = 0x00002,
            BACKWARD_TOP     = 0x00004,
            RIGHT_TOP        = 0x00008,
            FORWARD_BOTTOM   = 0x00010,
            LEFT_BOTTOM      = 0x00020,
            BACKWARD_BOTTOM  = 0x00040,
            RIGHT_BOTTOM     = 0x00080,
            FORWARD_LEFT     = 0x00100,
            LEFT_BACKWARD    = 0x00200,
            BACKWARD_RIGHT   = 0x00400,
            RIGHT_FORWARD    = 0x00800,
            FORWARD          = 0x01000,
            LEFT             = 0x02000,
            BACKWARD         = 0x04000,
            RIGHT            = 0x08000,
            TOP              = 0x10000,
            BOTTOM           = 0x20000,
            DO_NOT_PICK      = 0x40000
        }

        public GameObject[] FaceElements;
        public GameObject[] EdgeElements;

        public Faces BlockFace;
        public Edges BlockEdge;

        private static readonly int FACE_COUNT = 9;
        private static readonly int EDGE_COUNT = 18;

        private static uint CheckBit(uint block, int index)
        {
            return (block & (1U << index)) >> index;
        }

        void Awake()
        {
            UpdateFaces();
            UpdateEdges();
        }

        public void UpdateFaces()
        {
            var blockFace = (uint)(BlockFace);
            for (var index = 0; index < FACE_COUNT; ++index)
            {
                FaceElements[index].SetActive(CheckBit(blockFace, index) > 0);
            }
        }

        public void UpdateEdges()
        {
            var blockEdge = (uint)(BlockEdge);
            for (var index = 0; index < EDGE_COUNT; ++index)
            {
                EdgeElements[index].SetActive(CheckBit(blockEdge, index) > 0);
            }
        }
    }
}
