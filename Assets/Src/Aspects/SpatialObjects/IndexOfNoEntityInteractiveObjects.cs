using Assets.ColonyShared.GeneratedCode.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SVector3 = SharedCode.Utils.Vector3;
using SVector3Int = SharedCode.Utils.Vector3Int;

namespace Assets.Src.Aspects.SpatialObjects
{
    public class IndexOfNoEntityInteractiveObjects : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private List<NoEntityIndexItem> _objectList;
        [SerializeField] private float _cellsize;
        private SVector3Int cellSizes;
        private SpatialHashTable<MonoBehaviour> _spatialHash;

        private void Start()
        {
            cellSizes = new SVector3Int((int)_cellsize, (int)_cellsize, (int)_cellsize);
            _spatialHash = new SpatialHashTable<MonoBehaviour>(cellSizes);

            for (int i = 0; i < _objectList.Count; i++)
            {
                if (!_objectList[i].GameObject)
                    continue;
                var nearestToOriginCorner = _objectList[i].CellMin;
                var farthestFromOriginCorner = _objectList[i].CellMax;
                for (var x = nearestToOriginCorner.X; x <= farthestFromOriginCorner.X; x++)
                    for (var y = nearestToOriginCorner.Y; y <= farthestFromOriginCorner.Y; y++)
                        for (var z = nearestToOriginCorner.Z; z <= farthestFromOriginCorner.Z; z++)
                            _spatialHash.AddObjectByKey(new CellVector3(x, y, z), _objectList[i].GameObject);
            }

            AABBTriggeredCoordinator.Instance.AddIndex(this);
        }

        private void OnDestroy()
        {
            AABBTriggeredCoordinator.Instance.RemoveIndex(this);
        }

        private CellVector3[] _affectedCells = new CellVector3[8];
        private CellVector3[] _previouslyAffectedCells = new CellVector3[8];
        private readonly List<CellVector3> _cellsToActivate = new List<CellVector3>();
        private readonly List<CellVector3> _cellsToDeactivate = new List<CellVector3>();

        public void SetTriggeredForNearestCells(Vector3 center)
        {
            var sCenter = (SVector3)center;
            var currentStartCoords = _affectedCells[0].GetStartCoords(cellSizes);
            var halfCellSize = _cellsize / 2;
            var coordXshift = sCenter.x - halfCellSize < currentStartCoords.x ? -_cellsize : _cellsize;
            var coordYshift = sCenter.y - halfCellSize < currentStartCoords.y ? -_cellsize : _cellsize;
            var coordZshift = sCenter.z - halfCellSize < currentStartCoords.z ? -_cellsize : _cellsize;

            // current cell
            _affectedCells[0] = new CellVector3(sCenter, cellSizes);
            // nearestXcell
            _affectedCells[1] = new CellVector3(new SVector3(center.x + coordXshift, center.y, center.z), cellSizes);
            // nearestYcell
            _affectedCells[2] = new CellVector3(new SVector3(center.x, center.y + coordYshift, center.z), cellSizes);
            // nearestZcell
            _affectedCells[3] = new CellVector3(new SVector3(center.x, center.y, center.z + coordZshift), cellSizes);
            // nearestXYcell
            _affectedCells[4] = new CellVector3(new SVector3(center.x + coordXshift, center.y + coordYshift, center.z), cellSizes);
            // nearestXZcell
            _affectedCells[5] = new CellVector3(new SVector3(center.x + coordXshift, center.y, center.z + coordZshift), cellSizes);
            // nearestYZcell
            _affectedCells[6] = new CellVector3(new SVector3(center.x, center.y + coordYshift, center.z + coordZshift), cellSizes);
            // nearestXYZcell
            _affectedCells[7] = new CellVector3(new SVector3(center.x + coordXshift, center.y + coordYshift, center.z + coordZshift), cellSizes);

            _cellsToActivate.Clear();
            foreach (var cell in _affectedCells)
                if (Array.IndexOf(_previouslyAffectedCells, cell) == -1)
                    _cellsToActivate.Add(cell);

            _cellsToDeactivate.Clear();
            foreach (var cell in _previouslyAffectedCells)
                if (Array.IndexOf(_affectedCells, cell) == -1)
                    _cellsToDeactivate.Add(cell);

            //#if UNITY_EDITOR
            //            // DEBUG
            //            foreach(var cell in _affectedCells)
            //            {
            //                Color color;
            //                if (cellsToActivate.Contains(cell))
            //                    color = Color.red;
            //                else
            //                    color = Color.grey;

            //                DebugExtension.DebugBox((Vector3Int)cell.GetCenter(cellSizes), (Vector3Int)(cellSizes / 2), Quaternion.identity, color, 0.25f);
            //            }

            //            foreach (var cell in cellsToDeactivate)
            //                DebugExtension.DebugBox((Vector3Int)cell.GetCenter(cellSizes), (Vector3Int)(cellSizes / 2), Quaternion.identity, Color.blue, 0.25f);
            //            //
            //#endif

            ChangeAABBState(_cellsToActivate, true);
            ChangeAABBState(_cellsToDeactivate, false);

            (_previouslyAffectedCells, _affectedCells) = (_affectedCells, _previouslyAffectedCells);
        }

        private void ChangeAABBState(List<CellVector3> cells, bool enter)
        {
            foreach (var cell in cells)
            {
                if (_spatialHash.GetObjectsByKey(out var affectedComponentsOfCell, out var bigObjects, cell))
                    foreach (var affectedComponent in affectedComponentsOfCell.Concat(bigObjects))
                    {
                        if (enter)
                            (affectedComponent as IAABBTriggered)?.OnAABBEnter();
                        else
                            (affectedComponent as IAABBTriggered)?.OnAABBExit();
                    }
            }
        }

        public void Set(List<NoEntityIndexItem> list, float cellSize)
        {
            _objectList = list;
            _cellsize = cellSize;
        }

        public override string ToString()
        {
            return $"CellSize: '{_cellsize}'";
        }
    }

    [Serializable]
    public struct NoEntityIndexItem
    {
        public MonoBehaviour GameObject;
        public CellVector3 CellMin;
        public CellVector3 CellMax;
    }
}