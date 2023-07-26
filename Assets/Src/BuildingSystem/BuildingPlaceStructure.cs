using System;
using SharedCode.DeltaObjects.Building;
using SharedCode.Aspects.Building;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class BuildingPlaceStructure : PlaceStructure<BuildingPlaceDef, BuildingElementData>
    {
        public class ClientBuildingStructure : BuildingStructure
        {
            protected class ClientBlockCreationData
            {
                public Vector3 Start { get; } = Vector3.zero;
                public Quaternion Rotation { get; } = Quaternion.identity;
                public int Size { get; } = 0;
                public float BlockSize { get; } = 0;

                public bool VisualCheatEnable { get; set; } = false;

                public ClientBlockCreationData(Vector3 start, Quaternion rotation, int size, float blockSize)
                {
                    Start = start;
                    Rotation = rotation;
                    Size = size;
                    BlockSize = blockSize;
                }
            }

            protected class ClientBlock : Block
            {
                private GameObject gameObject = null;
                private bool updateEdgesInProgress = false;
                private bool updateFacesInProgress = false;

                protected override void OnUpdateFaces()
                {
                    if (gameObject != null)
                    {
                        if (!updateFacesInProgress)
                        {
                            var edgeHelperBehaviour = gameObject.GetComponent<EdgeHelperBehaviour>();
                            if (edgeHelperBehaviour != null)
                            {
                                edgeHelperBehaviour.BlockFace = (EdgeHelperBehaviour.Faces)(BlockFace);
                                edgeHelperBehaviour.UpdateFaces();
                            }
                        }
                    }
                }

                protected override void OnUpdateEdges()
                {
                    if (gameObject != null)
                    {
                        if (!updateEdgesInProgress)
                        {
                            var edgeHelperBehaviour = gameObject.GetComponent<EdgeHelperBehaviour>();
                            if (edgeHelperBehaviour != null)
                            {
                                edgeHelperBehaviour.BlockEdge = (EdgeHelperBehaviour.Edges)(BlockEdge);
                                edgeHelperBehaviour.UpdateEdges();
                            }
                        }
                    }
                }

                protected override void OnBeginSwitchEdges()
                {
                    updateEdgesInProgress = true;
                }

                protected override void OnEndSwitchEdges()
                {
                    updateEdgesInProgress = false;
                }

                public ClientBlock(SharedCode.Utils.Vector3Int blockCount, int x, int y, int z, ClientBlockCreationData blockCreationData) : base(blockCount, x, y, z)
                {
                    if (blockCreationData.VisualCheatEnable)
                    {
                        CreateVisualObject(x, y, z, blockCreationData);
                    }
                }

                private void CreateVisualObject(int x, int y, int z, ClientBlockCreationData blockCreationData)
                {
                    if (blockCreationData.VisualCheatEnable)
                    {
                        var placeRadius = (blockCreationData.Size - 1) * blockCreationData.BlockSize / 2.0f;
                        var placeStart = new Vector3(-placeRadius, 0.0f, -placeRadius);
                        var blockPosition = new Vector3((x * blockCreationData.BlockSize) + placeStart.x,
                                                        (y * blockCreationData.BlockSize) + placeStart.y,
                                                        (z * blockCreationData.BlockSize) + placeStart.z);
                        var position = blockCreationData.Start + (blockCreationData.Rotation * blockPosition);
                        var buildParamsDef = SharedCode.Utils.BuildUtils.BuildParamsDef;
                        gameObject = GameObjectInstantiate.Invoke(buildParamsDef.EdgeHelperPrefab, buildParamsDef.EdgeHelperPrefabDef, position, blockCreationData.Rotation, true);
                    }
                }

                public void UpdateVisualCheat(int x, int y, int z, ClientBlockCreationData blockCreationData)
                {
                    if (blockCreationData.VisualCheatEnable && (gameObject == null))
                    {
                        CreateVisualObject(x, y, z, blockCreationData);
                        OnUpdateFaces();
                        OnUpdateEdges();
                    }
                    else if (!blockCreationData.VisualCheatEnable && (gameObject != null))
                    {
                        UnityEngine.Object.Destroy(gameObject);
                        gameObject = null;
                    }
                }
            }

            private ClientBlockCreationData blockCreationData = null;
            private bool visualCheatEnable = false;


            protected override Block CreateBlock(SharedCode.Utils.Vector3Int blockCount, int x, int y, int z)
            {
                return new ClientBlock(blockCount, x, y, z, blockCreationData);
            }

            protected override void OnCreate(int size, int height, SharedCode.Utils.Vector3 position, SharedCode.Utils.Quaternion rotation, float blockSize)
            {
                blockCreationData = new ClientBlockCreationData((Vector3)(position), (Quaternion)(rotation), size, blockSize);
            }

            protected override void OnDestroy()
            {
                blockCreationData = null;
            }

            protected override uint OnSet(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side)
            {
                blockCreationData.VisualCheatEnable = visualCheatEnable;
                return CanPlaceData.REASON_OK;
            }

            protected override void OnClear(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side)
            {
            }

            public void SetVisualCheat(bool enable)
            {
                if (visualCheatEnable == enable)
                {
                    return;
                }
                visualCheatEnable = enable;
                if (Empty())
                {
                    return;
                }
                blockCreationData.VisualCheatEnable = visualCheatEnable;
                for (var x = 0; x < count.x; ++x)
                {
                    for (var z = 0; z < count.z; ++z)
                    {
                        for (var y = 0; y < count.y; ++y)
                        {
                            var block = blocks[x, y, z] as ClientBlock;
                            if (block != null)
                            {
                                block.UpdateVisualCheat(x, y, z, blockCreationData);
                            }
                        }
                    }
                }
            }
        }

        private ClientBuildingStructure buildingStructure = new ClientBuildingStructure();

        protected override void ElementAdded(Guid key, BuildingElementData data)
        {
            buildingStructure.Set(key, data.Block.x, data.Block.y, data.Block.z, data.BuildRecipeDef, data.Face, data.Side);
        }

        protected override void ElementRemoved(Guid key, BuildingElementData data)
        {
            buildingStructure.Clear(key, data.Block.x, data.Block.y, data.Block.z, data.BuildRecipeDef, data.Face, data.Side, false);
        }

        public override void Bind(BuildingPlaceDef placeDef, SharedCode.Utils.Vector3 Position, SharedCode.Utils.Quaternion Rotation)
        {
            if (placeDef != null)
            {
                buildingStructure.Create(placeDef.Size, placeDef.Height, Position, Rotation, placeDef.BlockSize);
            }
            else
            {
                buildingStructure.Destroy();
            }
        }

        public override void Unbind(BuildingPlaceDef placeDef)
        {
            buildingStructure.Destroy();
        }

        public override void SetVisualCheat(bool enable)
        {
            buildingStructure.SetVisualCheat(enable);
        }

        public BuildingStructure.CheckResult Check(int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side, bool checkOnlyInitialBlocks)
        {
            return buildingStructure.Check(x, y, z, buildRecipeDef, face, side, true, checkOnlyInitialBlocks);
        }
    }
}