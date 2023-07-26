using Assets.Src.NetworkedMovement;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Repositories;
using SharedCode.Aspects.Regions;
using SharedCode.Entities.Regions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Src.ResourceSystem;
using SharedCode.EntitySystem;
using Assets.Src.Server.Impl;

namespace Assets.Src.Aspects.RegionsScenicObjects
{
    public class ShapedRegion : MonoBehaviour
    {
        public JdbMetadata RegionDef;
        Guid id;
        ResourceRef<ConcaveShapeDef> Def;


#if ONGUI
        private void OnDrawGizmos()
#else
            private void NotOnDrawGizmos()
#endif
        {
            if (!DebugExtension.Draw)
                return;
            if (Def.Target != null)
                foreach (var triangle in Def.Target.Triangles)
                {
                    Color color = Color.red;
                    if (Pawn.EnteredDebugRegions.Contains(id))
                        color = Color.green;
                    Debug.DrawLine(triangle.PointA.ToXYZ(), triangle.PointB.ToXYZ(), color);
                    Debug.DrawLine(triangle.PointB.ToXYZ(), triangle.PointC.ToXYZ(), color);
                    Debug.DrawLine(triangle.PointC.ToXYZ(), triangle.PointA.ToXYZ(), color);
                }
        }
    }
}
