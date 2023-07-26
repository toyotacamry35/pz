using SharedCode.Aspects.Building;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharedCode.DeltaObjects.Building
{
    public abstract class BuildingStructure
    {
        public static class BitHelper
        {
            // bits ///////////////////////////////////////////////////////////////////////////////
            public static uint GetBit(int bitIndex)
            {
                return (1U << bitIndex);
            }

            public static uint GetBit(uint bitValue, int bitIndex)
            {
                return (bitValue << bitIndex);
            }

            public static uint CheckBit(uint blockBits, int bitIndex)
            {
                return (blockBits & (1U << bitIndex)) >> bitIndex;
            }

            public static uint GetGroundBits()
            {
                return (1U << EDGE_FORWARD_BOTTOM) | (1U << EDGE_LEFT_BOTTOM) | (1U << EDGE_BACKWARD_BOTTOM) | (1U << EDGE_RIGHT_BOTTOM) | (1U << EDGE_BOTTOM);
            }

            public static int GetNumberOfSetBits(uint blockBits)
            {
                var result = blockBits - ((blockBits >> 1) & 0x55555555U);
                result = (result & 0x33333333U) + ((result >> 2) & 0x33333333U);
                return (int)((((result + (result >> 4)) & 0x0F0F0F0FU) * 0x01010101U) >> 24);
            }

            // indices ////////////////////////////////////////////////////////////////////////////
            public static int GetIndex(BuildingElementFace face) { return (int)(face); }

            public static int GetIndex(BuildingElementSide side) { return (int)(side); }

            // translations and rotations /////////////////////////////////////////////////////////
            public static BuildingElementFace GetOppositeFace(BuildingElementFace face)
            {
                if (face == BuildingElementFace.Forward)
                {
                    return BuildingElementFace.Backward;
                }
                else if (face == BuildingElementFace.Left)
                {
                    return BuildingElementFace.Right;
                }
                else if (face == BuildingElementFace.Backward)
                {
                    return BuildingElementFace.Forward;
                }
                else if (face == BuildingElementFace.Right)
                {
                    return BuildingElementFace.Left;
                }
                else if (face == BuildingElementFace.Top)
                {
                    return BuildingElementFace.Bottom;
                }
                else if (face == BuildingElementFace.Bottom)
                {
                    return BuildingElementFace.Top;
                }
                else
                {
                    return face;
                }
            }

            public static BuildingElementFace GetFace(int faceIndex)
            {
                if ((faceIndex >= FACE_FORWARD) && (faceIndex < FACE_COUNT))
                {
                    return (BuildingElementFace)(faceIndex);
                }
                else
                {
                    return BuildingElementFace.Unknown;
                }
            }

            public static BuildingElementFace GetFace(BuildingElementSide side)
            {
                if (side == BuildingElementSide.Forward)
                {
                    return BuildingElementFace.Forward;
                }
                else if (side == BuildingElementSide.Left)
                {
                    return BuildingElementFace.Left;
                }
                else if (side == BuildingElementSide.Backward)
                {
                    return BuildingElementFace.Backward;
                }
                else if (side == BuildingElementSide.Right)
                {
                    return BuildingElementFace.Right;
                }
                else
                {
                    return BuildingElementFace.Forward;
                }
            }

            public static int RotateIndex(int faceIndex, int sideIndex)
            {
                int rotation = faceIndex;
                if ((rotation >= FACE_FORWARD) && (rotation <= FACE_RIGHT))
                {
                    return ((rotation + (int)(sideIndex)) % 4);
                }
                else
                {
                    return rotation;
                }
            }

            public static BuildingElementSide Rotate(BuildingElementSide side, bool counterClockwise)
            {
                if (side == BuildingElementSide.Forward)
                {
                    return (counterClockwise ? BuildingElementSide.Left : BuildingElementSide.Right);
                }
                else if (side == BuildingElementSide.Left)
                {
                    return (counterClockwise ? BuildingElementSide.Backward : BuildingElementSide.Forward);
                }
                else if (side == BuildingElementSide.Backward)
                {
                    return (counterClockwise ? BuildingElementSide.Right : BuildingElementSide.Left);
                }
                else if (side == BuildingElementSide.Right)
                {
                    return (counterClockwise ? BuildingElementSide.Forward : BuildingElementSide.Backward);
                }
                else
                {
                    return side;
                }
            }

            public static BuildingElementSide Flip(BuildingElementSide side)
            {
                if (side == BuildingElementSide.Forward)
                {
                    return BuildingElementSide.Backward;
                }
                else if (side == BuildingElementSide.Left)
                {
                    return BuildingElementSide.Right;
                }
                else if (side == BuildingElementSide.Backward)
                {
                    return BuildingElementSide.Forward;
                }
                else if (side == BuildingElementSide.Right)
                {
                    return BuildingElementSide.Left;
                }
                else
                {
                    return side;
                }
            }

            public static int FlipIndex(int sideIndex)
            {
                if (sideIndex == SIDE_FORWARD)
                {
                    return SIDE_BACKWARD;
                }
                else if (sideIndex == SIDE_LEFT)
                {
                    return SIDE_RIGHT;
                }
                else if (sideIndex == SIDE_BACKWARD)
                {
                    return SIDE_FORWARD;
                }
                else if (sideIndex == SIDE_RIGHT)
                {
                    return SIDE_LEFT;
                }
                else
                {
                    return sideIndex;
                }
            }

            // Fix ////////////////////////////////////////////////////////////////////////////////
            private static uint RotateEdge(uint blockEdge, int sideIndex)
            {
                if ((sideIndex >= SIDE_FORWARD) && (sideIndex <= SIDE_RIGHT))
                {
                    var result = blockEdge & ~(0xFFFFU);
                    var rest = blockEdge & 0xFU;
                    result |= (((rest << sideIndex) | (rest >> (4 - sideIndex))) & 0xFU);
                    rest = (blockEdge >> 4) & 0xFU;
                    result |= ((((rest << sideIndex) | (rest >> (4 - sideIndex))) & 0xFU) << 4);
                    rest = (blockEdge >> 8) & 0xFU;
                    result |= ((((rest << sideIndex) | (rest >> (4 - sideIndex))) & 0xFU) << 8);
                    rest = (blockEdge >> 12) & 0xFU;
                    result |= ((((rest << sideIndex) | (rest >> (4 - sideIndex))) & 0xFU) << 12);
                    return result;
                }
                return blockEdge;
            }

            public static uint Fix(uint blockEdge, int faceIndex, int sideIndex)
            {
                var result = blockEdge;
                var actualRotation = sideIndex;
                if ((faceIndex >= FACE_FORWARD) && (faceIndex <= FACE_RIGHT))
                {
                    var leftBIt = EDGE_FORWARD_LEFT;
                    var rightBIt = EDGE_RIGHT_FORWARD;
                    if (faceIndex != sideIndex)
                    {
                        leftBIt = EDGE_RIGHT_FORWARD;
                        rightBIt = EDGE_FORWARD_LEFT;
                        actualRotation = BitHelper.FlipIndex(sideIndex);
                    }
                    var top = GetBit(CheckBit(blockEdge, EDGE_FORWARD_TOP), EDGE_FORWARD_TOP);
                    var bottom = GetBit(CheckBit(blockEdge, EDGE_FORWARD_BOTTOM), EDGE_FORWARD_BOTTOM);
                    var left =  GetBit(CheckBit(blockEdge, EDGE_FORWARD_LEFT), leftBIt);
                    var right =  GetBit(CheckBit(blockEdge, EDGE_RIGHT_FORWARD), rightBIt);
                    var forward = GetBit(CheckBit(blockEdge, EDGE_FORWARD), EDGE_FORWARD);
                    result = top | bottom | left | right | forward;
                }
                else if (faceIndex == FACE_TOP)
                {
                    result = GetBit(CheckBit(blockEdge, EDGE_BOTTOM), EDGE_TOP) | ((blockEdge & 0xF0) >> 4);
                }
                else if (faceIndex == FACE_BOTTOM)
                {
                    result = GetBit(CheckBit(blockEdge, EDGE_BOTTOM), EDGE_BOTTOM) | (blockEdge & 0xF0);
                }
                return RotateEdge(result, actualRotation);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private const int FACE_FORWARD          =  0;
        private const int FACE_LEFT             =  1;
        private const int FACE_BACKWARD         =  2;
        private const int FACE_RIGHT            =  3;

        private const int FACE_TOP              =  4;
        private const int FACE_BOTTOM           =  5;

        private const int FACE_DIRECTION_COUNT  =  6;

        private const int FACE_CENTER           =  6;

        private const int FACE_COUNT            =  7;

        private const int FACE_OVER             =  7;
        private const int FACE_UNDER            =  8;

        private const int FACE_EXTENDED_COUNT   =  9;


        ///////////////////////////////////////////////////////////////////////////////////////////
        private const int EDGE_FORWARD_TOP      =  0;
        private const int EDGE_LEFT_TOP         =  1;
        private const int EDGE_BACKWARD_TOP     =  2;
        private const int EDGE_RIGHT_TOP        =  3;

        private const int EDGE_FORWARD_BOTTOM   =  4;
        private const int EDGE_LEFT_BOTTOM      =  5;
        private const int EDGE_BACKWARD_BOTTOM  =  6;
        private const int EDGE_RIGHT_BOTTOM     =  7;

        private const int EDGE_FORWARD_LEFT     =  8;
        private const int EDGE_LEFT_BACKWARD    =  9;
        private const int EDGE_BACKWARD_RIGHT   = 10;
        private const int EDGE_RIGHT_FORWARD    = 11;

        private const int EDGE_REAL_COUNT       = 12;

        private const int EDGE_FORWARD          = 12;
        private const int EDGE_LEFT             = 13;
        private const int EDGE_BACKWARD         = 14;
        private const int EDGE_RIGHT            = 15;

        private const int EDGE_TOP              = 16;
        private const int EDGE_BOTTOM           = 17;

        private const int EDGE_COUNT            = 18;

        ///////////////////////////////////////////////////////////////////////////////////////////
        private const int SIDE_FORWARD          =  0;
        private const int SIDE_LEFT             =  1;
        private const int SIDE_BACKWARD         =  2;
        private const int SIDE_RIGHT            =  3;

        private const int SIDE_COUNT            =  4;

        ///////////////////////////////////////////////////////////////////////////////////////////
        private class AccessBlockData
        {
            public int X { get; } = 0;
            public int Y { get; } = 0;
            public int Z { get; } = 0;
            public BuildingElementFace Face { get; } = BuildingElementFace.Center;
            public BuildingElementFace OppositeFace { get; } = BuildingElementFace.Center;
            public int FaceIndex { get; } = FACE_CENTER;
            public int OppositeFaceIndex { get; } = FACE_CENTER;

            public AccessBlockData(int x, int y, int z, BuildingElementFace face)
            {
                X = x;
                Y = y;
                Z = z;
                Face = face;
                OppositeFace = BitHelper.GetOppositeFace(face);
                FaceIndex = BitHelper.GetIndex(face);
                OppositeFaceIndex = BitHelper.GetIndex(OppositeFace);
            }
        }

        private class AccessEdgeData
        {
            public int X { get; } = 0;
            public int Y { get; } = 0;
            public int Z { get; } = 0;
            public int AccessIndex { get; } = FACE_DIRECTION_COUNT;
            public int EdgeIndex { get; } = EDGE_COUNT;

            public AccessEdgeData(int x, int y, int z, int accessIndex, int edgeIndex)
            {
                X = x;
                Y = y;
                Z = z;
                AccessIndex = accessIndex;
                EdgeIndex = edgeIndex;
            }
        }

        private class SideRecommendData
        {
            public int X { get; } = 0;
            public int Y { get; } = 0;
            public int Z { get; } = 0;
            public bool Check { get; } = false;
            public int FaceIndex { get; } = FACE_COUNT;

            public SideRecommendData(int x, int y, int z, bool check, int faceIndex)
            {
                X = x;
                Y = y;
                Z = z;
                Check = check;
                FaceIndex = faceIndex;
            }
        }

        private static readonly AccessBlockData[] accessBlockDataCollection = new AccessBlockData[7]
        {
            new AccessBlockData( 1,  0,  0, BuildingElementFace.Forward),
            new AccessBlockData( 0,  0,  1, BuildingElementFace.Left),
            new AccessBlockData(-1,  0,  0, BuildingElementFace.Backward),
            new AccessBlockData( 0,  0, -1, BuildingElementFace.Right),
            new AccessBlockData( 0,  1,  0, BuildingElementFace.Top),
            new AccessBlockData( 0, -1,  0, BuildingElementFace.Bottom),
            new AccessBlockData( 0,  0,  0, BuildingElementFace.Center)
        };

        private static readonly AccessEdgeData[,] accessEdgeDataEdgeCollection = new AccessEdgeData[12,3]
        {
            {   //EDGE_FORWARD_TOP
                new AccessEdgeData( 1,  0,  0, FACE_FORWARD,         EDGE_BACKWARD_TOP),
                new AccessEdgeData( 0,  1,  0, FACE_TOP,             EDGE_FORWARD_BOTTOM),
                new AccessEdgeData( 1,  1,  0, FACE_DIRECTION_COUNT, EDGE_BACKWARD_BOTTOM),
            },
            {   //EDGE_LEFT_TOP
                new AccessEdgeData( 0,  0,  1, FACE_LEFT,            EDGE_RIGHT_TOP),
                new AccessEdgeData( 0,  1,  0, FACE_TOP,             EDGE_LEFT_BOTTOM),
                new AccessEdgeData( 0,  1,  1, FACE_DIRECTION_COUNT, EDGE_RIGHT_BOTTOM),
            },
            {   //EDGE_BACKWARD_TOP
                new AccessEdgeData(-1,  0,  0, FACE_BACKWARD,        EDGE_FORWARD_TOP),
                new AccessEdgeData( 0,  1,  0, FACE_TOP,             EDGE_BACKWARD_BOTTOM),
                new AccessEdgeData(-1,  1,  0, FACE_DIRECTION_COUNT, EDGE_FORWARD_BOTTOM),
            },
            {   //EDGE_RIGHT_TOP
                new AccessEdgeData( 0,  0, -1, FACE_RIGHT,           EDGE_LEFT_TOP),
                new AccessEdgeData( 0,  1,  0, FACE_TOP,             EDGE_RIGHT_BOTTOM),
                new AccessEdgeData( 0,  1, -1, FACE_DIRECTION_COUNT, EDGE_LEFT_BOTTOM),
            },
            {   //EDGE_FORWARD_BOTTOM
                new AccessEdgeData( 1,  0,  0, FACE_FORWARD,         EDGE_BACKWARD_BOTTOM),
                new AccessEdgeData( 0, -1,  0, FACE_BOTTOM,          EDGE_FORWARD_TOP),
                new AccessEdgeData( 1, -1,  0, FACE_DIRECTION_COUNT, EDGE_BACKWARD_TOP),
            },
            {   //EDGE_LEFT_BOTTOM
                new AccessEdgeData( 0,  0,  1, FACE_LEFT,            EDGE_RIGHT_BOTTOM),
                new AccessEdgeData( 0, -1,  0, FACE_BOTTOM,          EDGE_LEFT_TOP),
                new AccessEdgeData( 0, -1,  1, FACE_DIRECTION_COUNT, EDGE_RIGHT_TOP),
            },
            {   //EDGE_BACKWARD_BOTTOM
                new AccessEdgeData(-1,  0,  0, FACE_BACKWARD,        EDGE_FORWARD_BOTTOM),
                new AccessEdgeData( 0, -1,  0, FACE_BOTTOM,          EDGE_BACKWARD_TOP),
                new AccessEdgeData(-1, -1,  0, FACE_DIRECTION_COUNT, EDGE_FORWARD_TOP),
            },
            {   //EDGE_RIGHT_BOTTOM
                new AccessEdgeData( 0,  0, -1, FACE_RIGHT,           EDGE_LEFT_BOTTOM),
                new AccessEdgeData( 0, -1,  0, FACE_BOTTOM,          EDGE_RIGHT_TOP),
                new AccessEdgeData( 0, -1, -1, FACE_DIRECTION_COUNT, EDGE_LEFT_TOP),
            },
            {   //EDGE_FORWARD_LEFT
                new AccessEdgeData( 1,  0,  0, FACE_FORWARD,         EDGE_LEFT_BACKWARD),
                new AccessEdgeData( 0,  0,  1, FACE_LEFT,            EDGE_RIGHT_FORWARD),
                new AccessEdgeData( 1,  0,  1, FACE_DIRECTION_COUNT, EDGE_BACKWARD_RIGHT),
            },
            {   //EDGE_LEFT_BACKWARD
                new AccessEdgeData( 0,  0,  1, FACE_LEFT,            EDGE_BACKWARD_RIGHT),
                new AccessEdgeData(-1,  0,  0, FACE_BACKWARD,        EDGE_FORWARD_LEFT),
                new AccessEdgeData(-1,  0,  1, FACE_DIRECTION_COUNT, EDGE_RIGHT_FORWARD),
            },
            {   //EDGE_BACKWARD_RIGHT
                new AccessEdgeData(-1,  0,  0, FACE_BACKWARD,        EDGE_RIGHT_FORWARD),
                new AccessEdgeData( 0,  0, -1, FACE_RIGHT,           EDGE_LEFT_BACKWARD),
                new AccessEdgeData(-1,  0, -1, FACE_DIRECTION_COUNT, EDGE_FORWARD_LEFT),
            },
            {   //EDGE_RIGHT_FORWARD
                new AccessEdgeData( 0,  0, -1, FACE_RIGHT,           EDGE_FORWARD_LEFT),
                new AccessEdgeData( 1,  0,  0, FACE_FORWARD,         EDGE_BACKWARD_RIGHT),
                new AccessEdgeData( 1,  0, -1, FACE_DIRECTION_COUNT, EDGE_LEFT_BACKWARD),
            }
        };

        private static readonly AccessEdgeData[] accessEdgeDataFaceCollection = new AccessEdgeData[6]
        {
            new AccessEdgeData( 1,  0,  0, FACE_FORWARD,  EDGE_BACKWARD), //EDGE_FORWARD
            new AccessEdgeData( 0,  0,  1, FACE_LEFT,     EDGE_RIGHT),    //EDGE_LEFT
            new AccessEdgeData(-1,  0,  0, FACE_BACKWARD, EDGE_FORWARD),  //EDGE_BACKWARD
            new AccessEdgeData( 0,  0, -1, FACE_RIGHT,    EDGE_LEFT),     //EDGE_RIGHT
            new AccessEdgeData( 0,  1,  0, FACE_TOP,      EDGE_BOTTOM),   //EDGE_TOP
            new AccessEdgeData( 0, -1,  0, FACE_BOTTOM,   EDGE_TOP)       //EDGE_BOTTOM
        };

        private static readonly SideRecommendData[] sideRecommendDataCollection = new SideRecommendData[9]
        {
            new SideRecommendData(0,  0, 0, false, FACE_CENTER),
            new SideRecommendData(0, -1, 0, true,  FACE_CENTER),
            new SideRecommendData(0,  1, 0, true,  FACE_CENTER),
            new SideRecommendData(0,  0, 0, false, FACE_BOTTOM),
            new SideRecommendData(0,  0, 0, false, FACE_TOP),
            new SideRecommendData(0, -1, 0, true,  FACE_TOP),
            new SideRecommendData(0, -1, 0, true,  FACE_BOTTOM),
            new SideRecommendData(0,  1, 0, true,  FACE_BOTTOM),
            new SideRecommendData(0,  1, 0, true,  FACE_TOP)
        };

        public class Element
        {
            public Guid Id { get; } = Guid.Empty;
            public BuildingElementSide Side { get; } = BuildingElementSide.Unknown;
            public BuildRecipeDef BuildRecipeDef { get; } = null;
            public int Depth { get; set; } = -1;

            public Element(Guid id, BuildingElementSide side, BuildRecipeDef buildRecipeDef)
            {
                Id = id;
                Side = side;
                BuildRecipeDef = buildRecipeDef;
            }
        }

        protected abstract class Block
        {
            //TODO, add structure weight here
            private int count = 0;
            //private int weight = 0;

            private Element[] faces = new Element[FACE_COUNT];
            private sbyte[] edges = new sbyte[EDGE_COUNT];

            private bool[] access = new bool[FACE_DIRECTION_COUNT];
            private uint blockFace = 0x0U;
            private uint blockEdge = 0x0U;

            protected abstract void OnUpdateFaces();
            protected abstract void OnUpdateEdges();
            protected abstract void OnBeginSwitchEdges();
            protected abstract void OnEndSwitchEdges();

            public bool[] Access { get { return access; } }
            public uint BlockFace { get { return blockFace; } }
            public uint BlockEdge { get { return blockEdge; } }

            public Element GetElement(int index)
            {
                return faces[index];
            }

            public Element SwitchElement(Element element, int index)
            {
                //TODO, add structure weight here
                var previousElement = faces[index];
                faces[index] = element;
                if ((previousElement == null) && (element != null))
                {
                    ++count;
                    blockFace |= BitHelper.GetBit(index);
                    OnUpdateFaces();
                }
                else if ((previousElement != null) && (element == null))
                {
                    --count;
                    blockFace &= ~(BitHelper.GetBit(index));
                    OnUpdateFaces();
                }
                return previousElement;
            }

            public bool ElementsEmpty { get { return (count == 0); } }

            public void SwitchEdge(bool set, int edgeIndex, bool silent)
            {
                if (set)
                {
                    if (edges[edgeIndex] <= 0)
                    {
                        blockEdge |= BitHelper.GetBit(edgeIndex);
                        if (!silent) { OnUpdateEdges(); }
                    }
                    edges[edgeIndex] += 1;
                }
                else
                {
                    edges[edgeIndex] -= 1;
                    if (edges[edgeIndex] < 1)
                    {
                        blockEdge &= ~(BitHelper.GetBit(edgeIndex));
                        if (!silent) { OnUpdateEdges(); }
                    }
                }
            }

            public void SwitchEdges(bool set, uint blockEdge, bool silent)
            {
                if (!silent) { OnBeginSwitchEdges(); }
                for (int edgeIndex = 0; edgeIndex < EDGE_COUNT; ++edgeIndex)
                {
                    if (BitHelper.CheckBit(blockEdge, edgeIndex) > 0)
                    {
                        SwitchEdge(set, edgeIndex, silent);
                    }
                }
                if (!silent)
                {
                    OnEndSwitchEdges();
                    OnUpdateEdges();
                }
            }

            public static bool CheckEdges(Block block, BuildingStructure structure, int x, int y, int z, BuildingElementDef elementDef, BuildingElementFace face, BuildingElementSide side, bool toAdd)
            {
                var wallsMask = 0U;
                var minBlockEdgeCount = elementDef.MinBlockEdgeCount;
                var faceIndex = BitHelper.GetIndex(face);
                var sideIndex = BitHelper.GetIndex(side);
                if (!toAdd)
                {
                    if ((faceIndex >= FACE_FORWARD) && (faceIndex <= FACE_RIGHT) && block.access[FACE_BOTTOM]) // special case for walls deletion
                    {
                        bool somethingUnder = false;
                        var bottomBlock = structure.Access(false, x, y - 1, z, false);
                        if (bottomBlock.faces[faceIndex] != null)
                        {
                            somethingUnder = true;
                        }
                        if (block.access[faceIndex])
                        {
                            var accessBlockData = accessBlockDataCollection[faceIndex];
                            var adjacentBottomBlock = structure.Access(false, x + accessBlockData.X, y - 1, z + accessBlockData.Z, false);
                            if (adjacentBottomBlock.faces[faceIndex] != null)
                            {
                                somethingUnder = true;
                            }
                        }
                        if (!somethingUnder)
                        {
                            return false;
                        }
                    }
                    else if (((faceIndex == FACE_TOP) || (faceIndex == FACE_BOTTOM)) && (BitHelper.GetNumberOfSetBits(block.blockEdge) < 4)) //special case for floor deletion 
                    {
                        if (faceIndex == FACE_TOP)
                        {
                            for (var index = FACE_FORWARD; index <= FACE_RIGHT; ++index)
                            {
                                if (block.faces[index] != null)
                                {
                                    wallsMask |= BitHelper.GetBit(EDGE_FORWARD_TOP + (index - FACE_FORWARD));
                                }
                            }
                        }
                        else if (block.access[FACE_BOTTOM])
                        {
                            var bottomBlock = structure.Access(false, x, y - 1, z, false);
                            for (var index = FACE_FORWARD; index <= FACE_RIGHT; ++index)
                            {
                                if (bottomBlock.faces[index] != null)
                                {
                                    wallsMask |= BitHelper.GetBit(EDGE_FORWARD_BOTTOM + (index - FACE_FORWARD));
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                        minBlockEdgeCount += 1;
                    }
                    else if (faceIndex == FACE_CENTER) //special case for central block with no any ground support
                    {
                        if (BitHelper.GetNumberOfSetBits(elementDef.BlockEdge & BitHelper.GetGroundBits()) == 0)
                        {
                            minBlockEdgeCount += 1;
                        }
                    }
                }
                var fixedBlockEdge = BitHelper.Fix(elementDef.BlockEdge, faceIndex, sideIndex);
                var fixedBlockEdgeDouble = BitHelper.Fix(elementDef.BlockEdgeDouble, faceIndex, sideIndex);
                var fixedBlockEdgeRequired = BitHelper.Fix(elementDef.BlockEdgeRequired, faceIndex, sideIndex);
                var groundBlockEdge =  (elementDef.CanBuildOnGround && (y == 0)) ? BitHelper.GetGroundBits() : 0U;
                var actualBlockEdge = ((block != null) ? block.blockEdge : 0U) | groundBlockEdge;
                var blockEdgeCount = BitHelper.GetNumberOfSetBits(actualBlockEdge & fixedBlockEdge) + BitHelper.GetNumberOfSetBits(actualBlockEdge & fixedBlockEdgeDouble) + BitHelper.GetNumberOfSetBits(wallsMask & fixedBlockEdge);
                var result = (BitHelper.GetNumberOfSetBits(fixedBlockEdgeRequired) == BitHelper.GetNumberOfSetBits(actualBlockEdge & fixedBlockEdgeRequired));
                return result && (blockEdgeCount >= minBlockEdgeCount);
            }

            public List<int> GetUnsupportedFaceIndices(BuildingStructure structure, int x, int y, int z)
            {
                var faceIndicesToCheck = new List<int>();

                for (var faceIndex = FACE_FORWARD; faceIndex < FACE_DIRECTION_COUNT; ++faceIndex)
                {
                    var faceElement = faces[faceIndex];
                    if (faceElement != null)
                    {
                        var faceElementDef = faceElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                        if (faceElementDef.Type != BuildingElementType.Center)
                        {
                            faceIndicesToCheck.Add(faceIndex);
                        }
                    }
                }
                var centerElement = faces[FACE_CENTER];
                if (centerElement != null)
                {
                    faceIndicesToCheck.Add(FACE_CENTER);
                }


                var result = new List<int>();
                if (faceIndicesToCheck.Count > 0)
                {
                    var backupEedges = new List<sbyte>(edges);
                    var backupBlockEdge = blockEdge;

                    for (var index = 0; index < faceIndicesToCheck.Count; ++index)
                    {
                        var faceIndex = faceIndicesToCheck[index];
                        var faceElement = faces[faceIndex];
                        var faceElementDef = faceElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                        var faceBlockEdge = BitHelper.Fix(faceElementDef.BlockEdge, faceIndex, BitHelper.GetIndex(faceElement.Side));
                        SwitchEdges(false, faceBlockEdge, true);
                    }

                    bool somethingAdded = true;
                    while (somethingAdded)
                    {
                        somethingAdded = false;
                        for (var index = 0; index < faceIndicesToCheck.Count; ++index)
                        {
                            var faceIndex = faceIndicesToCheck[index];
                            if (faceIndex != FACE_COUNT)
                            {
                                var faceElement = faces[faceIndex];
                                var faceElementDef = faceElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                                if (CheckEdges(this, structure, x, y, z, faceElementDef, BitHelper.GetFace(faceIndex), faceElement.Side, false))
                                {
                                    faceIndicesToCheck[index] = FACE_COUNT;
                                    var faceBlockEdge = BitHelper.Fix(faceElementDef.BlockEdge, faceIndex, BitHelper.GetIndex(faceElement.Side));
                                    SwitchEdges(true, faceBlockEdge, true);
                                    somethingAdded = true;
                                    break;
                                }
                            }
                        }
                    }

                    edges = backupEedges.ToArray();
                    blockEdge = backupBlockEdge;

                    for (var index = 0; index < faceIndicesToCheck.Count; ++index)
                    {
                        if (faceIndicesToCheck[index] != FACE_COUNT)
                        {
                            result.Add(faceIndicesToCheck[index]);
                        }
                    }
                }
                return result;
            }

            public Block(Vector3Int blockCount, int x, int y, int z)
            {
                access[FACE_FORWARD] = (blockCount.x > (x + 1));
                access[FACE_LEFT] = (blockCount.z > (z + 1));
                access[FACE_BACKWARD] = ((x - 1) >= 0);
                access[FACE_RIGHT] = ((z - 1) >= 0);
                access[FACE_TOP] = (blockCount.y > (y + 1));
                access[FACE_BOTTOM] = ((y - 1) >= 0);
            }
        }

        protected class OperationContext
        {
            private HashSet<int> indices = new HashSet<int>();
            private int depth = 0;
            private List<Element> elements = new List<Element>();
            private BuildingStructure parentStructure = null;

            public bool Recursive { get; } = false;
            public int Depth { get { return depth;  } }

            public OperationContext(bool recursive, BuildingStructure structure)
            {
                Recursive = recursive;
                parentStructure = structure;
            }

            public void Next()
            {
                indices = new HashSet<int>();
                depth += 1;
            }

            public void Reset()
            {
                indices = new HashSet<int>();
                depth = 0;
                elements = new List<Element>();
            }

            public bool AddIndices(int x, int y, int z)
            {
                var packed = parentStructure.PackIndices(x, y, z);
                if (!indices.Contains(packed))
                {
                    indices.Add(packed);
                    return true;
                }
                return false;
            }

            public bool AddElement(Element element)
            {
                if ((element != null) && !elements.Contains(element))
                {
                    element.Depth = depth;
                    elements.Add(element);
                    return true;
                }
                return false;
            }

            public HashSet<int> GetIndices()
            {
                return indices;
            }

            public List<Element> GetElements()
            {
                return elements;
            }
        }

        public struct CheckResult
        {
            public static readonly uint ReasonMask = CanPlaceData.REASON_OUT_OF_PLACE |
                                                     CanPlaceData.REASON_ALREADY_TAKEN |
                                                     CanPlaceData.REASON_EDGES |
                                                     CanPlaceData.REASON_GROUND |
                                                     CanPlaceData.REASON_BLOCK_OVER |
                                                     CanPlaceData.REASON_BLOCK_UNDER |
                                                     CanPlaceData.REASON_WEIGHT |
                                                     CanPlaceData.REASON_NO_RECIPE |
                                                     CanPlaceData.REASON_STRUCTURE_ERROR;
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public BuildingElementFace Face { get; set; }
            public BuildingElementSide Side { get; set; }
            public uint Reason { get; set; }
            public string ReasonDescription { get; set; }
            public uint BlockFace { get; set; }
            public uint BlockEdge { get; set; }

            public CheckResult(int x, int y, int z, BuildingElementFace face, BuildingElementSide side, uint reson, string reasonDescription, uint blockFace, uint blockEdge)
            {
                X = x;
                Y = y;
                Z = z;
                Face = face;
                Side = side;
                Reason = reson;
                ReasonDescription = reasonDescription;
                BlockFace = blockFace;
                BlockEdge = blockEdge;
            }

            public static CheckResult Empty()
            {
                return new CheckResult(0, 0, 0, BuildingElementFace.Unknown, BuildingElementSide.Unknown, CanPlaceData.REASON_OK, string.Empty, 0U, 0U);
            }
        }

        protected Block[,,] blocks = null;
        protected Vector3Int count = Vector3Int.zero;


        // abstract ///////////////////////////////////////////////////////////////////////////////
        protected abstract Block CreateBlock(Vector3Int blockCount, int x, int y, int z);
        protected abstract void OnCreate(int size, int height, Vector3 position, Quaternion rotation, float blockSize);
        protected abstract void OnDestroy();
        protected abstract uint OnSet(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side);
        protected abstract void OnClear(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side);

        // private ////////////////////////////////////////////////////////////////////////////////
        private int PackIndices(int x, int y, int z)
        {
            return count.x * count.y * z + count.x * y + x;
        }

        private Vector3Int UnpackIndices(int indices)
        {
            var z = indices / (count.x * count.y);
            var y = (indices - (z * count.x * count.y)) / count.x;
            var x = indices - (z * count.x * count.y) - (y * count.x);
            return new Vector3Int(x, y, z);
        }

        private bool CanAccess(int x, int y, int z)
        {
            return (x >= 0) && (y >= 0) && (z >= 0) && (count.x > x) && (count.y > y) && (count.z > z);
        }

        private Block Access(bool check, int x, int y, int z, bool create)
        {
            if (!check || CanAccess(x, y, z))
            {
                var block = blocks[x, y, z];
                if ((block == null) && create)
                {
                    block = CreateBlock(count, x, y, z);
                    blocks[x, y, z] = block;
                }
                return block;
            }
            return null;
        }

        private Block AccessAdjacent(Block block, int x, int y, int z, int faceIndex, bool create)
        {
            if ((faceIndex < FACE_FORWARD) || (faceIndex > FACE_BOTTOM))
            {
                return null;
            }
            var accessBlockData = accessBlockDataCollection[faceIndex];
            var adjacentX = x + accessBlockData.X;
            var adjacentY = y + accessBlockData.Y;
            var adjacentZ = z + accessBlockData.Z;
            if (block != null)
            {
                if (!block.Access[faceIndex])
                {
                    return null;
                }
            }
            else if (!CanAccess(adjacentX, adjacentY, adjacentZ))
            {
                return null;
            }
            return Access(false, adjacentX, adjacentY, adjacentZ, create);
        }

        private void SwitchEdges(Block block, bool set, int x, int y, int z, BuildingElementDef elementDef, int faceIndex, int sideIndex, bool create, OperationContext context)
        {
            var blockEdge = BitHelper.Fix(elementDef.BlockEdge, faceIndex, sideIndex);
            block.SwitchEdges(set, blockEdge, false);
            if (context != null)
            {
                context.AddIndices(x, y, z);
            }
            for (var edgeIndex = 0; edgeIndex < EDGE_REAL_COUNT; ++edgeIndex)
            {
                if (BitHelper.CheckBit(blockEdge, edgeIndex) > 0)
                {
                    var accessEdgeData0 = accessEdgeDataEdgeCollection[edgeIndex, 0];
                    var accessEdgeData1 = accessEdgeDataEdgeCollection[edgeIndex, 1];
                    var accessEdgeData2 = accessEdgeDataEdgeCollection[edgeIndex, 2];
                    var access0 = block.Access[accessEdgeData0.AccessIndex];
                    var access1 = block.Access[accessEdgeData1.AccessIndex];
                    if (access0)
                    {
                        var edgeBlock = Access(false, x + accessEdgeData0.X, y + accessEdgeData0.Y, z + accessEdgeData0.Z, create);
                        edgeBlock.SwitchEdge(set, accessEdgeData0.EdgeIndex, false);
                        if (context != null)
                        {
                            context.AddIndices(x + accessEdgeData0.X, y + accessEdgeData0.Y, z + accessEdgeData0.Z);
                        }
                    }
                    if (access1)
                    {
                        var edgeBlock = Access(false, x + accessEdgeData1.X, y + accessEdgeData1.Y, z + accessEdgeData1.Z, create);
                        edgeBlock.SwitchEdge(set, accessEdgeData1.EdgeIndex, false);
                        if (context != null)
                        {
                            context.AddIndices(x + accessEdgeData1.X, y + accessEdgeData1.Y, z + accessEdgeData1.Z);
                        }
                    }
                    if (access0 && access1)
                    {
                        var edgeBlock = Access(false, x + accessEdgeData2.X, y + accessEdgeData2.Y, z + accessEdgeData2.Z, create);
                        edgeBlock.SwitchEdge(set, accessEdgeData2.EdgeIndex, false);
                        if (context != null)
                        {
                            context.AddIndices(x + accessEdgeData2.X, y + accessEdgeData2.Y, z + accessEdgeData2.Z);
                        }
                    }
                }
            }

            for (var edgeIndex = EDGE_REAL_COUNT; edgeIndex < EDGE_COUNT; ++edgeIndex)
            {
                if (BitHelper.CheckBit(blockEdge, edgeIndex) > 0)
                {
                    var accessEdgeData = accessEdgeDataFaceCollection[edgeIndex - EDGE_REAL_COUNT];
                    var access = block.Access[accessEdgeData.AccessIndex];
                    if (access)
                    {
                        var edgeBlock = Access(false, x + accessEdgeData.X, y + accessEdgeData.Y, z + accessEdgeData.Z, create);
                        edgeBlock.SwitchEdge(set, accessEdgeData.EdgeIndex, false);
                        if (context != null)
                        {
                            context.AddIndices(x + accessEdgeData.X, y + accessEdgeData.Y, z + accessEdgeData.Z);
                        }
                    }
                }
            }
        }

        private uint SetFace(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side)
        {
            var faceIndex = BitHelper.GetIndex(face);
            var sideIndex = BitHelper.GetIndex(side);
            Block block = Access(true, x, y, z, true);
            if (block == null)
            {
                return CanPlaceData.REASON_OUT_OF_PLACE;
            }
            var newEdgeElement = new Element(id, side, buildRecipeDef);
            block.SwitchElement(newEdgeElement, faceIndex);
            Block adjacentBlock = AccessAdjacent(block, x, y, z, faceIndex, true);
            if (adjacentBlock != null)
            {
                adjacentBlock.SwitchElement(newEdgeElement, accessBlockDataCollection[faceIndex].OppositeFaceIndex);
            }
            var elementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
            SwitchEdges(block, true, x, y, z, elementDef, faceIndex, sideIndex, true, null);
            return CanPlaceData.REASON_OK;
        }

        private uint SetCenter(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementSide side)
        {
            var sideIndex = BitHelper.GetIndex(side);
            Block block = Access(true, x, y, z, true);
            if (block == null)
            {
                return CanPlaceData.REASON_OUT_OF_PLACE;
            }
            var newCenterElement = new Element(id, side, buildRecipeDef);
            block.SwitchElement(newCenterElement, FACE_CENTER);
            var elementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
            for (var faceIndex = FACE_FORWARD; faceIndex <= FACE_BOTTOM; ++faceIndex)
            {
                if (BitHelper.CheckBit(elementDef.BlockFace, faceIndex) > 0)
                {
                    block.SwitchElement(newCenterElement, BitHelper.RotateIndex(faceIndex, sideIndex));
                }
            }
            SwitchEdges(block, true, x, y, z, elementDef, FACE_CENTER, sideIndex, true, null);
            return CanPlaceData.REASON_OK;
        }

        private uint ClearFace(int x, int y, int z, BuildingElementFace face, BuildingElementSide side, OperationContext context)
        {
            var faceIndex = BitHelper.GetIndex(face);
            var sideIndex = BitHelper.GetIndex(side);
            Block block = Access(true, x, y, z, false);
            if (block == null)
            {
                BuildUtils.Error?.Report($"Trying to clear null element: x: {x}, y: {y}, z: {z}, face: {face}, side: {side}, depth: {context.Depth}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return CanPlaceData.REASON_OUT_OF_PLACE;
            }
            var previousElement = block.SwitchElement(null, faceIndex);
            Block adjacentBlock = AccessAdjacent(block, x, y, z, faceIndex, false);
            if (adjacentBlock != null)
            {
                previousElement = adjacentBlock.SwitchElement(null, accessBlockDataCollection[faceIndex].OppositeFaceIndex);
            }
            if (previousElement != null)
            {
                var elementDef = previousElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                SwitchEdges(block, false, x, y, z, elementDef, faceIndex, sideIndex, false, context);
                if (context.AddElement(previousElement))
                {
                    OnClear(previousElement.Id, x, y, z, previousElement.BuildRecipeDef, face, side);
                }
                BuildUtils.Message?.Report($"Element cleared, id: {previousElement.Id}, x: {x}, y: {y}, z: {z}, face: {face}, side: {side}, depth: {context.Depth}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
            else
            {
                BuildUtils.Error?.Report($"Trying to clear empty element: x: {x}, y: {y}, z: {z}, face: {face}, side: {side}, depth: {context.Depth}, maybe already cleared", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return CanPlaceData.REASON_STRUCTURE_ERROR;
            }
            return CanPlaceData.REASON_OK;
        }

        private uint ClearCenter(int x, int y, int z, BuildingElementSide side, OperationContext context)
        {
            var sideIndex = BitHelper.GetIndex(side);
            Block block = Access(true, x, y, z, false);
            if (block == null)
            {
                BuildUtils.Error?.Report($"Trying to clear null element: x: {x}, y: {y}, z: {z}, face: {BuildingElementFace.Center}, side: {side}, depth: {context.Depth}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return CanPlaceData.REASON_OUT_OF_PLACE;
            }
            var previousElement = block.SwitchElement(null, FACE_CENTER);
            if (previousElement != null)
            {
                var elementDef = previousElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                for (var faceIndex = FACE_FORWARD; faceIndex <= FACE_BOTTOM; ++faceIndex)
                {
                    if (BitHelper.CheckBit(elementDef.BlockFace, faceIndex) > 0)
                    {
                        block.SwitchElement(null, BitHelper.RotateIndex(faceIndex, sideIndex));
                    }
                }
                SwitchEdges(block, false, x, y, z, elementDef, FACE_CENTER, sideIndex, false, context);
                if (context.AddElement(previousElement))
                {
                    OnClear(previousElement.Id, x, y, z, previousElement.BuildRecipeDef, BuildingElementFace.Center, side);
                }
                BuildUtils.Message?.Report($"Element cleared, id: {previousElement.Id}, x: {x}, y: {y}, z: {z}, face: {BuildingElementFace.Center}, side: {side}, depth: {context.Depth}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
            else
            {
                BuildUtils.Error?.Report($"Trying to clear empty element: x: {x}, y: {y}, z: {z}, face: {BuildingElementFace.Center}, side: {side}, depth: {context.Depth}, maybe already cleared", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return CanPlaceData.REASON_STRUCTURE_ERROR;
            }
            return CanPlaceData.REASON_OK;
        }

        private BuildingElementSide RecommendSide(int x, int y, int z, BuildingElementFace face, BuildingElementSide side)
        {
            if ((face == BuildingElementFace.Top) || (face == BuildingElementFace.Bottom) || (face == BuildingElementFace.Center))
            {
                foreach (var sideRecommendData in sideRecommendDataCollection)
                {
                    var block = Access(sideRecommendData.Check, x + sideRecommendData.X, y + sideRecommendData.Y, z + sideRecommendData.Z, false);
                    if (block != null)
                    {
                        var element = block.GetElement(sideRecommendData.FaceIndex);
                        if (element != null)
                        {
                            var elementDef = element.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                            if (elementDef.HasSide)
                            {
                                return element.Side;
                            }
                        }
                    }
                }
            }
            return side;
        }

        // public /////////////////////////////////////////////////////////////////////////////////
        public void Create(int size, int height, Vector3 position, Quaternion rotation, float blockSize)
        {
            BuildUtils.Debug?.Report(true, $"size: {size}, height: {height}, position: {position}, rotation: {rotation}, blockSize: {blockSize}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            Destroy();
            if ((size > 0) && (height > 0))
            {
                count.x = size;
                count.y = height;
                count.z = size;
                blocks = new Block[count.x, count.y, count.z];
                OnCreate(size, height, position, rotation, blockSize);
            }
        }

        public void Destroy()
        {
            BuildUtils.Debug?.Report(true, $"{GetType().Name}.Destroy()", MethodBase.GetCurrentMethod().DeclaringType.Name);

            count.x = 0;
            count.y = 0;
            count.z = 0;
            blocks = null;
            OnDestroy();
        }

        public bool Empty()
        {
            //BuildUtils.Debug?.Report(true, $"{GetType().Name}.Empty()", MethodBase.GetCurrentMethod().DeclaringType.Name);

            return (blocks == null);
        }

        public uint Set(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side)
        {
            BuildUtils.Debug?.Report(true, $"{GetType().Name}.Set(), id: {id}, x: {x}, y: {y}, z: {z}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, face: {face}, side: {side}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (Empty())
            {
                return CanPlaceData.REASON_STRUCTURE_ERROR;
            }

            var result = OnSet(id, x, y, z, buildRecipeDef, face,side);
            if (result != CanPlaceData.REASON_OK)
            {
                return result;
            }

            var elementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
            if (elementDef == null)
            {
                return CanPlaceData.REASON_NO_RECIPE;
            }

            if (face == BuildingElementFace.Center)
            {
                result = SetCenter(id, x, y, z, buildRecipeDef, side);
            }
            else
            {
                result = SetFace(id, x, y, z, buildRecipeDef, face, side);
            }

            return result;
        }

        public List<Element> Clear(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side, bool recursive)
        {
            BuildUtils.Debug?.Report(true, $"{GetType().Name}.Clear(), id: {id}, x: {x}, y: {y}, z: {z}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, face: {face}, side: {side}, recursive: {recursive}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var clearContext = new OperationContext(recursive, this);
            if (Empty())
            {
                return clearContext.GetElements();
            }
            if (clearContext.Recursive)
            {
                if (face == BuildingElementFace.Center)
                {
                    ClearCenter(x, y, z, side, clearContext);
                }
                else
                {
                    ClearFace(x, y, z, face, side, clearContext);
                }
                while (true)
                {
                    var packedIndices = clearContext.GetIndices();
                    if (packedIndices.Count > 0)
                    {
                        // DEBUG //////////////////////////////////////////////////////////////////
                        //foreach (var packed in packedIndices)
                        //{
                        //    Vector3Int indices = UnpackIndices(packed);
                        //    BuildUtils.Error?.Report($"{indices.x}, {indices.y}, {indices.z}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        //}
                        // DEBUG //////////////////////////////////////////////////////////////////

                        clearContext.Next();
                        foreach (var packed in packedIndices)
                        {
                            Vector3Int indices = UnpackIndices(packed);
                            var block = Access(false, indices.x, indices.y, indices.z, false);
                            if (block != null)
                            {
                                var unsupportedFaceIndices = block.GetUnsupportedFaceIndices(this, indices.x, indices.y, indices.z);
                                if ((unsupportedFaceIndices != null) && (unsupportedFaceIndices.Count > 0))
                                {
                                    // DEBUG //////////////////////////////////////////////////////
                                    //for (var index = 0; index < unsupportedFaceIndices.Count; ++index)
                                    //{
                                    //    var unsupportedFaceIndex = unsupportedFaceIndices[index];
                                    //    BuildUtils.Error?.Report($"{Helper.GetFace(unsupportedFaceIndex)}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                                    //}
                                    // DEBUG //////////////////////////////////////////////////////

                                    for (var index = 0; index < unsupportedFaceIndices.Count; ++index)
                                    {
                                        var unsupportedFaceIndex = unsupportedFaceIndices[index];
                                        var unsupportedElement = block.GetElement(unsupportedFaceIndex);
                                        if (unsupportedFaceIndex == FACE_CENTER)
                                        {
                                            ClearCenter(indices.x, indices.y, indices.z, unsupportedElement.Side, clearContext);
                                        }
                                        else
                                        {
                                            ClearFace(indices.x, indices.y, indices.z, BitHelper.GetFace(unsupportedFaceIndex), unsupportedElement.Side, clearContext);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (face == BuildingElementFace.Center)
                {
                    ClearCenter(x, y, z, side, clearContext);
                }
                else
                {
                    ClearFace(x, y, z, face, side, clearContext);
                }
            }
            return clearContext.GetElements();
        }

        public CheckResult Check(int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side, bool allowChange, bool checkOnlyInitialBlocks)
        {
            //BuildUtils.Debug?.Report(true, $"{GetType().Name}.Check(), x: {x}, y: {y}, z: {z}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, face: {face}, side: {side}, allowChange: {allowChange}, checkOnlyInitialBlocks: {checkOnlyInitialBlocks}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new CheckResult(x, y, z, face, side, CanPlaceData.REASON_OK, string.Empty, 0U, 0U);
            
            var elementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
            if (elementDef == null)
            {
                result.Reason = CanPlaceData.REASON_NO_RECIPE;
                result.ReasonDescription = "REASON_NO_RECIPE_0";
                return result;
            }

            if (checkOnlyInitialBlocks)
            {
                if (elementDef.CanBuildOnGround && (elementDef.BlockEdgeRequired > 0))
                {
                    return result;
                }
                result.Reason = CanPlaceData.REASON_STRUCTURE_ERROR;
                result.ReasonDescription = "REASON_STRUCTURE_ERROR_0";
                return result;
            }

            if (Empty())
            {
                result.Reason = CanPlaceData.REASON_STRUCTURE_ERROR;
                result.ReasonDescription = "REASON_STRUCTURE_ERROR_1";
                return result;
            }

            Block block = null;
            if (face == BuildingElementFace.Center)
            {
                if (!CanAccess(x, y, z))
                {
                    result.Reason = CanPlaceData.REASON_OUT_OF_PLACE;
                    result.ReasonDescription = "REASON_OUT_OF_PLACE_0";
                    return result;
                }
                block = Access(false, x, y, z, false);
                if (block != null)
                {
                    result.BlockFace = block.BlockFace;
                    result.BlockEdge = block.BlockEdge;
                    var center = block.GetElement(FACE_CENTER);
                    if (center != null)
                    {
                        result.Reason = CanPlaceData.REASON_ALREADY_TAKEN;
                        result.ReasonDescription = "REASON_ALREADY_TAKEN_0";
                        return result;
                    }
                    var sideIndex = BitHelper.GetIndex(side);
                    for (var faceIndex = FACE_FORWARD; faceIndex <= FACE_RIGHT; ++faceIndex)
                    {
                        if (BitHelper.CheckBit(elementDef.BlockFace, faceIndex) > 0)
                        {
                            var element = block.GetElement(BitHelper.RotateIndex(faceIndex, sideIndex));
                            if (element != null)
                            {
                                result.Reason = CanPlaceData.REASON_ALREADY_TAKEN;
                                result.ReasonDescription = "REASON_ALREADY_TAKEN_1";
                                return result;
                            }
                        }
                    }
                    for (var faceIndex = FACE_TOP; faceIndex <= FACE_BOTTOM; ++faceIndex)
                    {
                        if (BitHelper.CheckBit(elementDef.BlockFace, faceIndex) > 0)
                        {
                            var element = block.GetElement(faceIndex);
                            if (element != null)
                            {
                                result.Reason = CanPlaceData.REASON_ALREADY_TAKEN;
                                result.ReasonDescription = "REASON_ALREADY_TAKEN_2";
                                return result;
                            }
                        }
                    }
                }
            }
            else
            {
                var faceIndex = BitHelper.GetIndex(face);
                if ((faceIndex >= FACE_FORWARD) && (faceIndex <= FACE_BOTTOM))
                {
                    var canAccessBlock = CanAccess(x, y, z);
                    if (canAccessBlock)
                    {
                        block = Access(false, x, y, z, false);
                        if (block != null)
                        {
                            result.BlockFace = block.BlockFace;
                            result.BlockEdge = block.BlockEdge;
                            var element = block.GetElement(faceIndex);
                            if (element != null)
                            {
                                result.Reason = CanPlaceData.REASON_ALREADY_TAKEN;
                                result.ReasonDescription = "REASON_ALREADY_TAKEN_3";
                                return result;
                            }
                        }
                    }
                    var accessBlockData = accessBlockDataCollection[faceIndex];
                    var canAccessAdjacentBlock = CanAccess(x + accessBlockData.X, y + accessBlockData.Y, z + accessBlockData.Z);
                    if (canAccessAdjacentBlock)
                    {
                        var adjacentBlock = Access(false, x + accessBlockData.X, y + accessBlockData.Y, z + accessBlockData.Z, false);
                        if ((adjacentBlock != null) && (adjacentBlock.GetElement(accessBlockData.OppositeFaceIndex) != null))
                        {
                            result.Reason = CanPlaceData.REASON_ALREADY_TAKEN;
                            result.ReasonDescription = "REASON_ALREADY_TAKEN_4";
                            return result;
                        }
                        if (!canAccessBlock && allowChange)
                        {
                            block = adjacentBlock;
                            if (block != null)
                            {
                                result.BlockFace = block.BlockFace;
                                result.BlockEdge = block.BlockEdge;
                            }
                            result.X = x + accessBlockData.X;
                            result.Y = y + accessBlockData.Y;
                            result.Z = z + accessBlockData.Z;
                            result.Face = accessBlockData.OppositeFace;
                        }
                    }
                    if (!canAccessBlock && (!canAccessAdjacentBlock || !allowChange))
                    {
                        result.Reason = CanPlaceData.REASON_OUT_OF_PLACE;
                        result.ReasonDescription = "REASON_OUT_OF_PLACE_1";
                        return result;
                    }
                }
                else
                {
                    result.Reason = CanPlaceData.REASON_STRUCTURE_ERROR;
                    result.ReasonDescription = "REASON_STRUCTURE_ERROR_2";
                    return result;
                }
            }

            if (elementDef.HasSide && allowChange)
            {
                result.Side = RecommendSide(result.X, result.Y, result.Z, result.Face, result.Side);
            }

            if (!Block.CheckEdges(block, this, result.X, result.Y, result.Z, elementDef, result.Face, result.Side, true))
            {
                result.Reason = CanPlaceData.REASON_EDGES;
                result.ReasonDescription = "REASON_EDGES_0";
                return result;
            }

            if (result.Face == BuildingElementFace.Center)
            {
                var underBlock = AccessAdjacent(block, result.X, result.Y, result.Z, FACE_BOTTOM, false);
                if (underBlock != null)
                {
                    var topElement = underBlock.GetElement(FACE_TOP);
                    if (topElement != null)
                    {
                        var topElementDef = topElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                        if (BitHelper.CheckBit(topElementDef.BlockFace, FACE_OVER) > 0)
                        {
                            result.Reason = CanPlaceData.REASON_BLOCK_OVER;
                            result.ReasonDescription = "REASON_BLOCK_OVER_0";
                            return result;
                        }
                    }
                }
                var overBlock = AccessAdjacent(block, result.X, result.Y, result.Z, FACE_TOP, false);
                if (overBlock != null)
                {
                    var bottomElement = overBlock.GetElement(FACE_BOTTOM);
                    if (bottomElement != null)
                    {
                        var bottomElementDef = bottomElement.BuildRecipeDef.ElementDef.Target as BuildingElementDef;
                        if (BitHelper.CheckBit(bottomElementDef.BlockFace, FACE_UNDER) > 0)
                        {
                            result.Reason = CanPlaceData.REASON_BLOCK_UNDER;
                            result.ReasonDescription = "REASON_BLOCK_UNDER_0";
                            return result;
                        }
                    }
                }
            }

            return result;
        }
    }
}
