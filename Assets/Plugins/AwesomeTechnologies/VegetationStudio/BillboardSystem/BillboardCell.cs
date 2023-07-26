using System.Collections.Generic;
using AwesomeTechnologies.Utility.Quadtree;
using UnityEngine;

namespace AwesomeTechnologies.Billboards 
{
    public class BillboardCell: IHasRect
    {
        public Vector3 CellCorner;
        public Vector3 CellSize;
        public bool InitDone;
        public Bounds CellBounds;
        public float VegetationCellSize;
        public VegetationPackage CurrentvegetationPackage;
        public List<Material> MaterialList;
        public UnityTerrainData UnityTerrainData;

        private Rect _rectangle;
        public bool IsVisible;
        public int DistanceBand = 0;
        public bool HasBillboards;
        //public Vector3 CurrentOffset = Vector3.zero;
    
        public readonly List<BillboardMeshGenerator> BillboardMeshGeneratorList = new List<BillboardMeshGenerator>();

        public Rect Rectangle
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;
            }
        }

        public void SetVisible(bool value)
        {
            IsVisible = value;
        }

        public void Init()
        { 
            if (InitDone) return;

            Vector3 cellExtent = CellSize / 2;
            Vector3 cellCenter = CellCorner + cellExtent;
            CellBounds = new Bounds(cellCenter, CellSize);

            //CellBounds.center = new Vector3(CellBounds.center.x, middleHeight + _unityTerrainData.terrainPosition.y, CellBounds.center.z);
            //CellBounds.size = new Vector3(CellBounds.size.x, maxHeight - minHeight + 2f, CellBounds.size.z);

            _rectangle = RectExtension.CreateRectFromBounds(CellBounds);
            CreateMeshGenerators();

            InitDone = true;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(CellBounds.center, CellBounds.extents.magnitude + VegetationCellSize + 10f);
        }

        void CreateMeshGenerators()
        {
            ClearMeshGenetators();

            if (CurrentvegetationPackage)
            {

                for (int i = 0; i <= CurrentvegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    BillboardMeshGenerator billboardMeshGenerator =
                        new BillboardMeshGenerator
                        {
                            TreeLayerMask = 0,
                            BillboardParent = null,
                            BillboardIndex = i,
                            CelBounds = CellBounds,                            
                        };

                    Bounds vegetationItemBounds = CurrentvegetationPackage.VegetationInfoList[i].Bounds;
                    billboardMeshGenerator.VegetationItemSize = Mathf.Max(vegetationItemBounds.extents.x,
                                                                    vegetationItemBounds.extents.y,
                                                                    vegetationItemBounds.extents.z) * 2f;

                    BillboardMeshGeneratorList.Add(billboardMeshGenerator);
                }
            }   
        }

        public void ClearMeshGenetators()
        {
            for (int i = 0; i <= BillboardMeshGeneratorList.Count - 1; i++)
            {
                BillboardMeshGeneratorList[i].ClearMeshes();
            }

            BillboardMeshGeneratorList.Clear();
        }

        public void ClearMeshes()
        {            
            for (int i = 0; i <= BillboardMeshGeneratorList.Count - 1; i++)
            {
                BillboardMeshGeneratorList[i].ClearMeshes();
            }
            HasBillboards = false;
        }

        public void AddVegetationCells(List<VegetationCell> vegetationCellList, Vector3 terrainPosition)
        {
            

            for (int i = 0; i <= BillboardMeshGeneratorList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = CurrentvegetationPackage.VegetationInfoList[i];
                if (vegetationItemInfo.VegetationType != VegetationType.Tree ||
                    !vegetationItemInfo.UseBillboards) continue; //||!CurrentvegetationPackage.VegetationInfoList[i].IncludeInTerrain) continue;

                for (int j = 0; j <= vegetationCellList.Count - 1; j++)
                {
                    //vegetationCellList[j].Preprocess();
                    //var currentVegetationList = vegetationCellList[j].GetCurrentVegetationList(i);// = new List<Matrix4x4>();
                    var currentVegetationList = vegetationCellList[j]
                        .DirectSpawnVegetationLocalspace(vegetationItemInfo.VegetationItemID,true);
                    if (currentVegetationList != null)
                    {
                        for (int k = 0; k <= currentVegetationList.Count - 1; k++)
                        {
                            BillboardMeshGeneratorList[i].AddQuad(currentVegetationList[k], terrainPosition, vegetationItemInfo.Bounds.extents.y);
                        }
                    }
                    //vegetationCellList[j].GetVegetationInfoList(i, returnList);                    
                }
            }
        }

        public void GenerateBillboardMeshes(Vector3 terrainPosition)
        {
            for (int i = 0; i <= BillboardMeshGeneratorList.Count - 1; i++)
            {
                //BillboardMeshGeneratorList[i].BillboardMaterial = MaterialList[i];
                BillboardMeshGeneratorList[i].ProcessMesh(terrainPosition);
            }
            HasBillboards = true;
        }

        public void Move(Vector3 offset)//, List<Vector3> moveVertexTempList)
        {
            CellCorner += offset;
            CellBounds.center += offset;
            _rectangle = RectExtension.CreateRectFromBounds(CellBounds);
            //for (int i = 0; i <= BillboardMeshGeneratorList.Count - 1; i++)
            //{
            //    BillboardMeshGeneratorList[i].CelBounds = CellBounds;
            //    BillboardMeshGeneratorList[i].MoveMeshes(offset, moveVertexTempList);
            //}

            //CurrentOffset += offset;
        }
    }
}
