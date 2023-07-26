using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Src.Locomotion.Unity
{
    public static class NavMeshAgentExtensions
    {
        /// <summary>
        /// Возвращает плоскость линка  
        /// </summary>
        public static Plane OffMeshLinkPlane(this OffMeshLinkData link)
        {
            var linkDir = link.endPos - link.startPos;
            var linkNormal = Vector3.Cross(Vector3.Cross(linkDir, link.offMeshLink ? link.offMeshLink.transform.up : Vector3.up), linkDir);
            return new Plane(linkNormal, link.startPos);
        }
        
        /// <summary>
        /// Проекция точки на плоскость линка в заданном направлении 
        /// </summary>
        public static bool OffMeshLinkPlaneRaycast(this OffMeshLinkData link, Vector3 pt, Vector3 dir, out Vector3 hit)
        {
            var linkPlane = link.OffMeshLinkPlane();
            float distance;
            if (linkPlane.normal != Vector3.zero)
            {
                var ray = new Ray(pt, Vector3.down);
                linkPlane.Raycast(ray, out distance);
                hit = ray.GetPoint(distance);
                return true;
            }
            hit = pt;
            return false;
        }

        /// <summary>
        /// Возвращает плоскость при пересечении которой (т.е. при смене GetSide с + на -) считается, что движение по линку завершено.  
        /// </summary>
        public static Plane OffMeshLinkFinishPlane(this OffMeshLinkData link)
        {
            Assert.IsTrue(link.valid, nameof(link.valid));
            var upVector = link.offMeshLink ? link.offMeshLink.transform.up : Vector3.up;
            var finishLine = Vector3.Cross(link.endPos - link.startPos, upVector);
            return new Plane(Vector3.Cross(finishLine, Vector3.up), link.endPos);
        }
        
        /// <summary>
        /// Проверка что точка находится за концом линка
        /// </summary>
        public static bool BeyondOffMeshLinkEnd(this OffMeshLinkData link, Vector3 point)
        {
            return link.valid && !OffMeshLinkFinishPlane(link).GetSide(point);
        }
    }
}