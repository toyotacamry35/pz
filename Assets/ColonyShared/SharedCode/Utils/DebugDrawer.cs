#if UNITY_5_3_OR_NEWER
//using Assets.Src.Aspects.RegionsScenicObjects;
#endif
using SharedCode.Aspects.Regions;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public static class DebugDrawer
    {
        //[Conditional("DEBUG_DRAW")]
        public static void DrawBox(Vector3 pos, Vector3 size, float duration, Color color)
        {
#if UNITY_5_3_OR_NEWER
            //DebugExtension.DebugBox(pos.ToXYZ(), size.ToXYZ(), UnityEngine.Quaternion.identity, color, duration, true);
#endif
        }
        //[Conditional("DEBUG_DRAW")]
        public static void DrawShape(ConcaveShapeDef shapeDef, Vector3 pos, Vector3 size, float duration, Color color)
        {
#if UNITY_5_3_OR_NEWER
           foreach (var triangle in shapeDef.Triangles)
            {
                //UnityEngine.Debug.DrawLine(triangle.PointA.ToXYZ(), triangle.PointB.ToXYZ(), color, duration, true);
                //UnityEngine.Debug.DrawLine(triangle.PointB.ToXYZ(), triangle.PointC.ToXYZ(), color, duration, true);
                //UnityEngine.Debug.DrawLine(triangle.PointC.ToXYZ(), triangle.PointA.ToXYZ(), color, duration, true);
            }
#endif
        }
        //[Conditional("DEBUG_DRAW")]
        public static void DrawHash(IEnumerable<Vector3Int> hash, int cellSize, float duration, Color color)
        {
            foreach (var hashCell in hash)
                DrawBox(new Vector3(hashCell.x, hashCell.y, hashCell.z) * cellSize, new Vector3(cellSize, cellSize, cellSize), duration, color);
        }
    }
}
